namespace C868_RyanNewman.ViewModels
{
    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class ObjectiveAssessmentViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private int _courseId;
        private Course _course;
        private CourseService _courseService;
        

        public Course Course
        {
            get => _course;
            set
            {
                _course = value;
                OnPropertyChanged(nameof(Course));
                OnPropertyChanged(nameof(IsObjectiveAssessmentAlertEnabled));
                OnPropertyChanged(nameof(ObjectiveAssessmentStartDate));
                OnPropertyChanged(nameof(ObjectiveAssessmentEndDate));
            }
        }

        public int CourseId
        {
            get => _courseId;
            set
            {
                _courseId = value;
                LoadCourseAsync(value);
                OnPropertyChanged(nameof(ObjectiveAssessmentStartDate));
                OnPropertyChanged(nameof(ObjectiveAssessmentEndDate));
            }
        }

        public bool IsObjectiveAssessmentAlertEnabled
        {
            get => Course.IsObjectiveAssessmentAlertEnabled;
            set
            {
                Course.IsObjectiveAssessmentAlertEnabled = value;
                OnPropertyChanged(nameof(IsObjectiveAssessmentAlertEnabled));
            }
        }

        public DateTime? ObjectiveAssessmentStartDate
        {
            get => _course.ObjectiveAssessmentStartDate;
            set
            {
                _course.ObjectiveAssessmentStartDate = value;
                OnPropertyChanged(nameof(ObjectiveAssessmentStartDate));
            }
        }

        public DateTime? ObjectiveAssessmentEndDate
        {
            get => _course.ObjectiveAssessmentEndDate;
            set
            {
                _course.ObjectiveAssessmentEndDate = value;
                OnPropertyChanged(nameof(ObjectiveAssessmentEndDate));
            }
        }
        // Status for Objective Assessment
        public string ObjectiveAssessmentStatus
        {
            get => Course.ObjectiveAssessmentStartDate == null
                ? "Assessment Not Scheduled"
                : $"Assessment Scheduled for {Course.ObjectiveAssessmentStartDate:MMM dd, yyyy}";
        }
        public ICommand SaveAssessmentCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteAssessmentCommand { get; }

        public ObjectiveAssessmentViewModel(CourseService courseService)
        {
            _courseService = courseService;
            SaveAssessmentCommand = new Command(async () => await SaveAssessmentAsync());
            DeleteAssessmentCommand = new Command(async () => await DeleteAssessment());
            _course = new Course
            {
                ObjectiveAssessmentStartDate = DateTime.Now,
                ObjectiveAssessmentEndDate = DateTime.Now
            };
            CancelCommand = new Command(async () => await Cancel());
        }

        public async Task CheckObjectiveAssessmentIsEnabledAsync()
        {
            if (!IsObjectiveAssessmentAlertEnabled || Course == null || !Course.ObjectiveAssessmentStartDate.HasValue || !Course.ObjectiveAssessmentEndDate.HasValue)
                return;

            string startDate = Course.ObjectiveAssessmentStartDate.Value.ToString("MM/dd/yyyy");
            string endDate = Course.ObjectiveAssessmentEndDate.Value.ToString("MM/dd/yyyy");
            string message;

            if (Course.ObjectiveAssessmentStartDate > DateTime.Now)
            {
                message = $"The objective assessment will start on {startDate} and end on {endDate}.";
            }
            else if (Course.ObjectiveAssessmentEndDate < DateTime.Now)
            {
                message = $"The objective assessment started on {startDate} and ended on {endDate}.";
            }
            else
            {
                message = $"The objective assessment started on {startDate} and is ongoing. It will end on {endDate}.";
            }

            await Shell.Current.DisplayAlert("Objective Assessment Dates", message, "OK");
        }

        private async void LoadCourseAsync(int courseId)
        {
            if (courseId <= 0) return; // Validate the courseId

            // Fetch the course by ID
            Course = await _courseService.GetCourseByIdAsync(courseId);

            if (Course == null)
            {
                await Shell.Current.DisplayAlert("Error", "Course not found.", "OK");
                return;
            }
            if (!Course.ObjectiveAssessmentStartDate.HasValue)
            {
                Course.ObjectiveAssessmentStartDate = DateTime.Now.AddDays(1); // Default to tomorrow
            }

            if (!Course.ObjectiveAssessmentEndDate.HasValue)
            {
                Course.ObjectiveAssessmentEndDate = Course.ObjectiveAssessmentStartDate.Value.AddDays(1); 
            }


            // Notify that the course has changed to update the UI
            OnPropertyChanged(nameof(Course));
            OnPropertyChanged(nameof(IsObjectiveAssessmentAlertEnabled));
            OnPropertyChanged(nameof(ObjectiveAssessmentStartDate));
            OnPropertyChanged(nameof(ObjectiveAssessmentEndDate));
            Title = $"Assessment Details - {_course.ObjectiveAssessmentName}";
        }

        private async Task SaveAssessmentAsync()
        {
            // Validate the course data
            if (string.IsNullOrWhiteSpace(Course.ObjectiveAssessmentName))
            {
                await Shell.Current.DisplayAlert("Error", "Course name cannot be empty.", "OK");
                return;
            }

            if (Course.ObjectiveAssessmentEndDate < Course.ObjectiveAssessmentStartDate)
            {
                await Shell.Current.DisplayAlert("Error", "End date cannot be earlier than start date.", "OK");
                return;
            }
            if (!Course.ObjectiveAssessmentStartDate.HasValue)
            {
                await Shell.Current.DisplayAlert("Error", "Start date cannot be empty.", "OK");
                return;
            }

            if (!Course.ObjectiveAssessmentEndDate.HasValue)
            {
                await Shell.Current.DisplayAlert("Error", "End date cannot be empty.", "OK");
                return;
            }


            if (CourseId <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Term must be selected.", "OK");
                return;
            }

            try
            {
                // Save the course using the service
                await _courseService.UpdateCourseAsync(_course);

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save course: {ex.Message}", "OK");
            }
        }

        private async Task DeleteAssessment()
        {
            if (_course == null) return;

            // Show a warning confirmation dialog
            bool isConfirmed = await Shell.Current.DisplayAlert("Confirm Delete",
            $"Are you sure you want to delete this objective assessment '{_course.ObjectiveAssessmentName}'?",
            "Delete",
            "Cancel");

            if (isConfirmed)
            {
                // Delete the objective assessment from the database
                await _courseService.DeleteObjectiveAssessmentAsync(_courseId);
                await Shell.Current.GoToAsync(".."); // Navigate back to the previous page
            }
        }

        private async Task Cancel()
        {
            await Shell.Current.GoToAsync(".."); 
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
