namespace C868_RyanNewman.ViewModels
{
    public partial class AddTermViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _termName;
        private DateTime _startDate;
        private DateTime _endDate;
        private readonly TermService _termService;
        private readonly CourseService _courseService;
        private SQLiteAsyncConnection _database;

        // Event for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;


        public string Name
        {
            get => _termName;
            set
            {
                if (_termName != value)
                {
                    _termName = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        public ICommand SaveTermCommand { get; }
        public ICommand CancelCommand { get; }

        public AddTermViewModel(string dbPath)
        {
            _termService = new TermService(dbPath);  
            _courseService = new CourseService(dbPath);

            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddMonths(6); // default end date

            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));

            SaveTermCommand = new Command(OnSaveTerm);
            CancelCommand = new Command(OnCancel);
        }

        private async void OnSaveTerm()
        {
            Console.WriteLine("SaveTermCommand triggered");
            if (string.IsNullOrEmpty(Name))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a term name.", "OK");
                return;
            }
            var newTerm = new Term
            {
                Name = Name,
                StartDate = StartDate,
                EndDate = EndDate,
                UserId = App.CurrentUserId
            };

            try
            {
                // Save to SQLite database. Switching AddTermWithCoursesAsync to AddTermAsync for testing
                await _termService.AddTermAsync(newTerm);
                // Seed the new Term with 6 course
               // await SeedCoursesForTermAsync(newTerm);
                // Navigate back after saving
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving term: {ex.Message}");
            }
        }

        private async Task SeedCoursesForTermAsync(Term newTerm)
        {
            IsBusy = true;
            try
            {
                await _courseService.SeedOneCourseForTermAsync(newTerm.Id, App.CurrentUserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding courses for new term: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnCancel()
        {
            // Navigate back without saving
            await Shell.Current.GoToAsync("..");
        }

        // Helper method to trigger the PropertyChanged event. This application is not using the MVVM Community Toolkit.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
