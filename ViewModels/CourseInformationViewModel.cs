namespace C868_RyanNewman.ViewModels
{
    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class CourseInformationViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly CourseService _courseService;
        private readonly PerformanceAssessmentViewModel _performanceAssessmentViewModel;
        private readonly ObjectiveAssessmentViewModel _objectiveAssessmentViewModel;
        private Course _course;
        private int _courseId;
        public bool HasOptionalNotes
        {
         get =>  !string.IsNullOrWhiteSpace(_course.OptionalNotes);
        }

        public Course Course
        {
            get => _course;
            set
            {
                _course = value;
                OnPropertyChanged(nameof(Course));
                OnPropertyChanged(nameof(HasOptionalNotes));
                OnPropertyChanged(nameof(PerformanceAssessmentStatusText));
                OnPropertyChanged(nameof(IsPerformanceAssessmentEnabled));
                OnPropertyChanged(nameof(ObjectiveAssessmentStatusText));
                OnPropertyChanged(nameof(IsObjectiveAssessmentEnabled));
                OnPropertyChanged(nameof(PerformanceAssessmentBackgroundColor));
                OnPropertyChanged(nameof(ObjectiveAssessmentBackgroundColor));
            }
        }

        public string PerformanceAssessmentStatusText
        {
            get
        {
                if (string.IsNullOrWhiteSpace(Course.PerformanceAssessmentName) ||
                    !Course.PerformanceAssessmentStartDate.HasValue ||
                    !Course.PerformanceAssessmentEndDate.HasValue)
                {
                    return "Performance Assessment: Not Scheduled";
                }

                return $"Performance Assessment: {Course.PerformanceAssessmentName}\nStart: {Course.PerformanceAssessmentStartDate:MMM dd, yyyy} - End: {Course.PerformanceAssessmentEndDate:MMM dd, yyyy}";
            }
        }

        public bool IsPerformanceAssessmentEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Course.PerformanceAssessmentName) &&
                       Course.PerformanceAssessmentStartDate.HasValue &&
                       Course.PerformanceAssessmentEndDate.HasValue;
            }
        }
        public string ObjectiveAssessmentStatusText
        {
            get
        {
                if (string.IsNullOrWhiteSpace(Course.ObjectiveAssessmentName) ||
                    !Course.ObjectiveAssessmentStartDate.HasValue ||
                    !Course.ObjectiveAssessmentEndDate.HasValue)
                {
                    return "Objective Assessment: Not Scheduled";
                }

                return $"Objective Assessment: {Course.ObjectiveAssessmentName}\nStart: {Course.ObjectiveAssessmentStartDate:MMM dd, yyyy} - End: {Course.PerformanceAssessmentEndDate:MMM dd, yyyy}";
            }
        }

        public bool IsObjectiveAssessmentEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Course.ObjectiveAssessmentName) &&
                       Course.ObjectiveAssessmentStartDate.HasValue &&
                       Course.PerformanceAssessmentEndDate.HasValue;
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

        // Status for Performance Assessment
        public string PerformanceAssessmentStatus
        {
            get => Course.PerformanceAssessmentStartDate == null
                ? "Assessment Not Scheduled"
                : $"Assessment Scheduled for {Course.PerformanceAssessmentStartDate:MMM dd, yyyy}";
        }

        // Status for Objective Assessment
        public string ObjectiveAssessmentStatus
        {
            get => Course.ObjectiveAssessmentStartDate == null
                ? "Assessment Not Scheduled"
                : $"Assessment Scheduled for {Course.ObjectiveAssessmentStartDate:MMM dd, yyyy}";
        }


        public CourseInformationViewModel(CourseService courseService)
        {
            _courseService = courseService;
    
            ToggleNotesCommand = new Command(ToggleNotesVisibility);
            ShareOptionalNotesCommand = new Command(async () => await ShareOptionalNotes());
            OpenPerformanceAssessmentCommand = new Command(async () => await OpenPerformanceAssessment());
            OpenObjectiveAssessmentCommand = new Command(async () => await OpenObjectiveAssessment());
            AreNotesVisible = false;
            _course = new Course();
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

            // Notify that the course has changed to update the UI
            OnPropertyChanged(nameof(Course));
            Title = $"Course Details - {_course.Title}";
        }
        private bool _areNotesVisible;
        public bool AreNotesVisible
        {
            get => _areNotesVisible;
            set
            {
                _areNotesVisible = value;
                OnPropertyChanged(nameof(AreNotesVisible));
                NotesButtonText = _areNotesVisible ? "Hide Optional Notes" : "Show Optional Notes";
            }
        }

        private string _notesButtonText = "Show Optional Notes";
        public string NotesButtonText
        {
            get => _notesButtonText;
            set
            {
                _notesButtonText = value;
                OnPropertyChanged(nameof(NotesButtonText));
            }
        }
        public ICommand ObjectiveAssessmentCommand { get; }
        public ICommand PerformanceAssessmentCommand { get; }
        public ICommand ToggleNotesCommand { get; }
        public ICommand ShareOptionalNotesCommand { get; }
        public ICommand OpenPerformanceAssessmentCommand { get; }
        public ICommand OpenObjectiveAssessmentCommand { get; }

        #region BackgroundColorForAssessmentButtons
        public string PerformanceAssessmentBackgroundColor
        {
            get
            {
                // If all data exists (Name, Start Date, and End Date), return green
                if (!string.IsNullOrWhiteSpace(Course.PerformanceAssessmentName) &&
                    Course.PerformanceAssessmentStartDate.HasValue &&
                    Course.PerformanceAssessmentEndDate.HasValue)
                {
                    return "Green";
                }

                // If any of the data is missing, return blue
                return "Blue";
            }
        }
        public string ObjectiveAssessmentBackgroundColor
        {
            get
            {
                // If all data exists (Name, Start Date, and End Date), return green
                if (!string.IsNullOrWhiteSpace(Course.ObjectiveAssessmentName) &&
                    Course.ObjectiveAssessmentStartDate.HasValue &&
                    Course.ObjectiveAssessmentEndDate.HasValue)
                {
                    return "Green";
                }

                // If any of the data is missing, return blue
                return "Blue";
            }
        }
        #endregion
        private async Task OpenPerformanceAssessment()
        {
            await Shell.Current.GoToAsync($"PerformanceAssessmentPage?CourseId={Course.Id}");
        }

        private async Task OpenObjectiveAssessment()
        {
            await Shell.Current.GoToAsync($"ObjectiveAssessmentPage?CourseId={Course.Id}");
        }


        private void ToggleNotesVisibility()
        {
            AreNotesVisible = !AreNotesVisible;
        }

        private async Task ShareOptionalNotes()
        {
            if (!string.IsNullOrWhiteSpace(Course?.OptionalNotes))
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = Course.OptionalNotes,
                    Title = "Share Optional Notes"
                });
            }
            else
            {
                await Shell.Current.DisplayAlert("No Notes", "There are no optional notes to share.", "OK");
            }
        }

        #region AlertLogic

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

        public async Task CheckPerformanceAssessmentDatesAsync()
        {
            if (Course.IsPerformanceAssessmentAlertEnabled == false || Course == null || !Course.PerformanceAssessmentStartDate.HasValue || !Course.PerformanceAssessmentEndDate.HasValue)
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

            // Show alert using Shell.Current
            await Shell.Current.DisplayAlert("Performance Assessment Dates", message, "OK");
        }

        public async Task CheckObjectiveAssessmentDatesAsync()
        {
            if (Course.IsObjectiveAssessmentAlertEnabled == false || Course == null || !Course.ObjectiveAssessmentStartDate.HasValue || !Course.ObjectiveAssessmentEndDate.HasValue)
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

            // Show alert using Shell.Current
            await Shell.Current.DisplayAlert("Objective Assessment Dates", message, "OK");
        }
        #endregion

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
        private async Task PerformanceAssessment()
        {
            if (_course == null) return;
        }

        private async Task ObjectiveAssessment()
        {
            if (_course == null) return;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

