namespace C868_RyanNewman.ViewModels
{
    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class PerformanceAssessmentViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly CourseService _courseService;
        private Course _course;
        private int _courseId;


        public Course Course
        {
            get => _course;
            set
            {
                _course = value;
                OnPropertyChanged(nameof(Course));
                OnPropertyChanged(nameof(IsPerformanceAssessmentAlertEnabled));
                OnPropertyChanged(nameof(PerformanceAssessmentStartDate));
                OnPropertyChanged(nameof(PerformanceAssessmentEndDate));
            }
        }

        public int CourseId
        {
            get => _courseId;
            set
            {
                _courseId = value;
                LoadCourseAsync(value);
                OnPropertyChanged(nameof(PerformanceAssessmentStartDate));
                OnPropertyChanged(nameof(PerformanceAssessmentEndDate));
            }
        }

        public bool IsPerformanceAssessmentAlertEnabled
        {
            get => Course.IsPerformanceAssessmentAlertEnabled;
            set
            {
                Course.IsPerformanceAssessmentAlertEnabled = value;
                OnPropertyChanged(nameof(IsPerformanceAssessmentAlertEnabled));
            }
        }

        public DateTime? PerformanceAssessmentStartDate
        {
            get => _course.PerformanceAssessmentStartDate;
            set
            {
                _course.PerformanceAssessmentStartDate = value;
                OnPropertyChanged(nameof(PerformanceAssessmentStartDate));
            }
        }

        public DateTime? PerformanceAssessmentEndDate
        {
            get => _course.PerformanceAssessmentEndDate;
            set
            {
                _course.PerformanceAssessmentEndDate = value;
                OnPropertyChanged(nameof(PerformanceAssessmentEndDate));
            }
        }


        // Status for Performance Assessment
        public string PerformanceAssessmentStatus
        {
            get => Course.PerformanceAssessmentStartDate == null
                ? "Assessment Not Scheduled"
                : $"Assessment Scheduled for {Course.PerformanceAssessmentStartDate:MMM dd, yyyy}";
        }

        public PerformanceAssessmentViewModel(CourseService courseService)
        {
            _courseService = courseService;
            SaveAssessmentCommand = new Command(async () => await OnSaveCourseAsync());
            DeleteAssessmentCommand = new Command(async () => await DeleteAssessment());
            _course = new Course
            {
                PerformanceAssessmentStartDate = DateTime.Now,
                PerformanceAssessmentEndDate = DateTime.Now
            };
            CancelCommand = new Command(async () => await Cancel());
        }

        public async Task CheckPerformanceAssessmentIsEnabledAsync()
        {
            if (!IsPerformanceAssessmentAlertEnabled || Course == null || !Course.PerformanceAssessmentStartDate.HasValue || !Course.PerformanceAssessmentEndDate.HasValue)
                return;

            string startDate = Course.PerformanceAssessmentStartDate.Value.ToString("MM/dd/yyyy");
            string endDate = Course.PerformanceAssessmentEndDate.Value.ToString("MM/dd/yyyy");
            string message;

            if (Course.PerformanceAssessmentStartDate > DateTime.Now)
            {
                message = $"The performance assessment will start on {startDate} and end on {endDate}.";
            }
            else if (Course.PerformanceAssessmentEndDate < DateTime.Now)
            {
                message = $"The performance assessment started on {startDate} and ended on {endDate}.";
            }
            else
            {
                message = $"The performance assessment started on {startDate} and is ongoing. It will end on {endDate}.";
            }

            await Shell.Current.DisplayAlert("Performance Assessment Dates", message, "OK");
        }

        private async void LoadCourseAsync(int courseId)
        {
            if (courseId <= 0) return; // Validate the courseId

            // Fetch the course by ID
            Course = await _courseService.GetCourseByIdForUserAsync(courseId, App.CurrentUserId);

            if (Course == null)
            {
                await Shell.Current.DisplayAlert("Error", "Course not found.", "OK");
                return;
            }
            if (!Course.PerformanceAssessmentStartDate.HasValue)
            {
                Course.PerformanceAssessmentStartDate = DateTime.Now.AddDays(1); // Default to tomorrow
            }

            if (!Course.PerformanceAssessmentEndDate.HasValue)
            {
                Course.PerformanceAssessmentEndDate = Course.PerformanceAssessmentStartDate.Value.AddDays(1);
            }


            // Notify that the course has changed to update the UI
            OnPropertyChanged(nameof(Course));
            OnPropertyChanged(nameof(IsPerformanceAssessmentAlertEnabled));
            OnPropertyChanged(nameof(PerformanceAssessmentStartDate));
            OnPropertyChanged(nameof(PerformanceAssessmentEndDate));
            Title = $"Assessment Details - {_course.PerformanceAssessmentName}";
        }

        public ICommand SaveAssessmentCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteAssessmentCommand { get; }

        private async Task OnSaveCourseAsync()
        {
            // Validate the course data
            if (string.IsNullOrWhiteSpace(Course.PerformanceAssessmentName))
            {
                await Shell.Current.DisplayAlert("Error", "Course name cannot be empty.", "OK");
                return;
            }

            if (Course.PerformanceAssessmentEndDate < Course.PerformanceAssessmentStartDate)
            {
                await Shell.Current.DisplayAlert("Error", "End date cannot be earlier than start date.", "OK");
                return;
            }
            if (!Course.PerformanceAssessmentStartDate.HasValue)
            {
                await Shell.Current.DisplayAlert("Error", "Start date cannot be empty.", "OK");
                return;
            }

            if (!Course.PerformanceAssessmentEndDate.HasValue)
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
            $"Are you sure you want to delete this performance assessment '{_course.PerformanceAssessmentName}'?",
            "Delete",
            "Cancel");

            if (isConfirmed)
            {
                // Delete the performance assessment from the database
                await _courseService.GetCourseByIdForUserAsync(_courseId, App.CurrentUserId);
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