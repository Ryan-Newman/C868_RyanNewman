namespace C868_RyanNewman.ViewModels
{
    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class CourseDetailsViewModel : BaseViewModel, INotifyPropertyChanged
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
                OnPropertyChanged(nameof(IsCourseStartDateAlertEnabled));
                OnPropertyChanged(nameof(IsCourseEndDateAlertEnabled));
            }
        }

        public int CourseId
        {
            get => _courseId;
            set
            {
                _courseId = value;
                LoadCourseAsync(value);
            }
        }

        // List for Status options
        public List<string> StatusOptions { get; } = new List<string>
        {
          "In Progress",
          "Completed",
          "Dropped",
          "Plan to Take"
        };

        public bool IsCourseStartDateAlertEnabled
        {
            get => Course.IsCourseStartDateAlertEnabled;
            set
            {
                Course.IsCourseStartDateAlertEnabled = value;
                OnPropertyChanged(nameof(IsCourseStartDateAlertEnabled));
            }
        }

        public bool IsCourseEndDateAlertEnabled
        {
            get => Course.IsCourseEndDateAlertEnabled;
            set
            {
                Course.IsCourseEndDateAlertEnabled = value;
                OnPropertyChanged(nameof(IsCourseEndDateAlertEnabled));
            }
        }

        // Status for Performance Assessment
        public string PerformanceAssessmentStatus
        {
            get => Course.PerformanceAssessmentStartDate == null
                ? "Assessment Not Scheduled"
                : $"Assessment Scheduled for {Course.PerformanceAssessmentStartDate:MMM dd, yyyy}";
            set
            {
                Course.PerformanceAssessmentStatus = value;
            }
        }

        // Status for Objective Assessment
        public string ObjectiveAssessmentStatus
        {
            get => Course.ObjectiveAssessmentStartDate == null
                ? "Assessment Not Scheduled"
                : $"Assessment Scheduled for {Course.ObjectiveAssessmentStartDate:MMM dd, yyyy}";
            set
            {
                Course.ObjectiveAssessmentStatus = value;
            }
        }

        public CourseDetailsViewModel(CourseService courseService)
        {
            _courseService = courseService;
            SaveCourseCommand = new Command(async () => await OnSaveCourseAsync());
            DeleteCourseCommand = new Command(async () => await DeleteCourse());
            _course = new Course();
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

            // Notify that the course has changed to update the UI
            OnPropertyChanged(nameof(Course));
            OnPropertyChanged(nameof(IsCourseStartDateAlertEnabled));
            OnPropertyChanged(nameof(IsCourseEndDateAlertEnabled));
            Title = $"Course Details - {_course.Title}";
        }
        public ICommand SaveCourseCommand { get; }
        public ICommand DeleteCourseCommand { get; }

        #region ValidationLogic
        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            // Regex pattern for phone number with optional dashes, parentheses, or spaces
            var phonePattern = @"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$";
            return Regex.IsMatch(phone, phonePattern);
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            // Regex pattern for a valid email format
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
        #endregion

        public async Task CheckCourseStartDateAlertAsync()
        {
            if (Course.IsCourseStartDateAlertEnabled == false || Course == null) return;

            string startDate = Course.StartDate.ToString("MM/dd/yyyy");
            string endDate = Course.EndDate.ToString("MM/dd/yyyy");
            string message;

            if (Course.StartDate > DateTime.Now)
            {
                message = $"The course will start on {startDate} and end on {endDate}";
            }
            else if (Course.EndDate < DateTime.Now)
            {
                message = $"The course started on {startDate} and ended on {endDate}.";
            }
            else
            {
                message = $"The course started on {startDate} and is ongoing.";
            }

            // Show alert using Shell.Current
            await Shell.Current.DisplayAlert("Course Start Date", message, "OK");
        }
        public async Task CheckCourseEndDateAlertAsync()
        {
            if (Course.IsCourseEndDateAlertEnabled == false || Course == null) return;

            string startDate = Course.StartDate.ToString("MM/dd/yyyy");
            string endDate = Course.EndDate.ToString("MM/dd/yyyy");
            string message;

            if (Course.StartDate > DateTime.Now)
            {
                message = $"The course will start on {startDate} and end on {endDate}";
            }
            else if (Course.EndDate < DateTime.Now)
            {
                message = $"The course started on {startDate} and ended on {endDate}.";
            }
            else
            {
                message = $"The course will end on {endDate}.";
            }

            // Show alert using Shell.Current
            await Shell.Current.DisplayAlert("Course End Date", message, "OK");
        }
        private async Task OnSaveCourseAsync()
        {
            // Validate the course data
            if (string.IsNullOrWhiteSpace(Course.Title))
            {
                await Shell.Current.DisplayAlert("Error", "Course name cannot be empty.", "OK");
                return;
            }

            if (Course.EndDate < Course.StartDate)
            {
                await Shell.Current.DisplayAlert("Error", "End date cannot be earlier than start date.", "OK");
                return;
            }
            if (Course.DueDate < Course.StartDate || Course.DueDate > Course.EndDate)
            {
                await Shell.Current.DisplayAlert("Error", "Due date cannot be earlier than start date or after end date.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Course.StartDate.ToString()))
            {
                await Shell.Current.DisplayAlert("Error", "Start date cannot be empty.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Course.EndDate.ToString()))
            {
                await Shell.Current.DisplayAlert("Error", "End date cannot be empty.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Course.InstructorName))
            {
                await Shell.Current.DisplayAlert("Error", "Instructor name cannot be empty.", "OK");
                return;
            }
            if (!IsValidPhoneNumber(Course.InstructorPhoneNumber))
            {
                await Shell.Current.DisplayAlert("Error", "Phone number must be a 10-digit number and can include dashes or parentheses.", "OK");
                return;
            }
            if (!IsValidEmail(Course.InstructorEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Invalid email format.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Course.InstructorName) ||
            string.IsNullOrWhiteSpace(Course.InstructorPhoneNumber) ||
            string.IsNullOrWhiteSpace(Course.InstructorEmail))
            {
                await Shell.Current.DisplayAlert("Error", "Instructor information cannot be empty.", "OK");
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

                // Navigate back to the previous page (CourseSelectionPage)
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save course: {ex.Message}", "OK");
            }
        }

        private async Task DeleteCourse()
        {
            if (_course == null) return;

            // Show a warning confirmation dialog
            bool isConfirmed = await Shell.Current.DisplayAlert("Confirm Delete",
            $"Are you sure you want to delete the term '{_course.Title}'?",
            "Delete",
            "Cancel");

            if (isConfirmed)
            {
                // Delete the term from the database
                await _courseService.DeleteCourseAsync(_course);
                await Shell.Current.GoToAsync(".."); // Navigate back to the previous page
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
