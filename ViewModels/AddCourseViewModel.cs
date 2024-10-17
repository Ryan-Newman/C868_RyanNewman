namespace C868_RyanNewman.ViewModels
{
    [QueryProperty(nameof(TermId), "TermId")]
    public class AddCourseViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly CourseService _courseService;
        private int _termId;

        // Instructor properties
        private string _instructorName;
        private string _instructorPhone;
        private string _instructorEmail;
        public ICommand SaveCourseCommand { get; }

        public AddCourseViewModel(CourseService courseService)
        {
            _courseService = courseService;

            Course = new Course
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(6),
                DueDate = DateTime.Now.AddMonths(5),
                Status = "Plan to Take" // Default value set to "Plan to Take"                
            };
            

            SaveCourseCommand = new Command(async () => await OnSaveCourseAsync());
        }
        // List for Status options
        public List<string> StatusOptions { get; } = new List<string>
        {
          "In Progress",
          "Completed",
          "Dropped",
          "Plan to Take"
        };

        // Property for the new course
        public Course Course { get; set; }

        // Property to hold the Term ID (passed via query parameter)
        public int TermId
        {
            get => _termId;
            set
            {
                if (_termId != value)
                {
                    _termId = value;
                    OnPropertyChanged(nameof(TermId));
                }
            }
        }

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

            // Ensure the TermId is set correctly
            if (TermId <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Term must be selected.", "OK");
                return;
            }

            // Assign the TermId to the course before saving
            Course.TermId = TermId;
         
            try
            {
                // Save the course using the service
                await _courseService.AddCourseAsync(Course);

                // Navigate back to the previous page (CourseSelectionPage)
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save course: {ex.Message}", "OK");
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
