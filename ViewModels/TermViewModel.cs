using System.Collections.ObjectModel;
using System.Text.Json;

namespace C868_RyanNewman.ViewModels
{
    public partial class TermViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly TermService termService;
        private readonly CourseService courseService;

        private bool isBusy;

        public bool IsNotBusy => !isBusy;
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C868.db3");

        public ObservableCollection<Term> Terms { get; private set; } = new();

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

        public TermViewModel(TermService termService)
        {
            Title = "Terms";
            this.termService = termService;

            GetTermDetailsCommand = new Command<Term>(async (term) => await GetTermDetails(term));
            AddTermCommand = new Command(async () => await AddTerm());
            RefreshTermsCommand = new Command(async () => await GetTerms());
            EditTermCommand = new Command<Term>(async (term) => await EditTerm(term));
            GetCourseCommand = new Command<Term>(async (term) => await GoToCourseSelectionPage(term));

           _ =  LoadTerms();
        }

        private async Task LoadTerms()
        {
            IsBusy = true;
            try
            {
                Terms.Clear();
                var terms = await termService.GetTermsForUserAsync(App.CurrentUserId);
                foreach (var term in terms)
                {
                    Console.WriteLine($"Term ID: {term.Id}, Name: {term.Name}, Start Date: {term.StartDate}, End Date: {term.EndDate}");
                    Terms.Add(term);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading terms: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to load terms.", "OK");
            }
            finally
            {
                IsBusy = false; // Reset IsBusy regardless of success or failure
            }
        }

        public ICommand RefreshTermsCommand { get; }
        public ICommand EditTermCommand { get; }
        public ICommand GetCourseCommand { get; }

        public async Task GetTerms()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (Terms.Any())
                    Terms.Clear();

                var terms = await termService.GetTermsForUserAsync(App.CurrentUserId);
                foreach (var term in terms)
                {
                    Terms.Add(term);
                    Console.WriteLine($"Term Name: {term.Name}, Start Date: {term.StartDate}, End Date: {term.EndDate}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Loading terms failed with {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to retrieve terms.", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task AddTerm()
        {
            if (IsBusy) return; //Check if busy
            IsBusy = true; // Set to true before navigating
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C868.db3");
            await Shell.Current.GoToAsync($"{nameof(AddTermPage)}?dbPath={dbPath}");
            IsBusy = false; // Set to false after navigating
        }

        private async Task EditTerm(Term term)
        {
            if (term == null || term.Id <= 0)
            {
                Debug.WriteLine("Invalid term or term ID is 0.");
                await Shell.Current.DisplayAlert("Error", "Invalid term selected for editing.", "OK");
                return;
            }

            if (IsBusy) return;

            // Navigate to TermDetailsPage with the selected term's ID
            await Shell.Current.GoToAsync($"{nameof(TermDetailsPage)}?Id={term.Id}");
        }

        private async Task GoToCourseSelectionPage(Term term)
        {
            
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C868.db3");
            await Shell.Current.GoToAsync($"{nameof(CourseSelectionPage)}?TermId={term.Id}");
            
        }

        private async Task GetTermDetails(Term term)
        {
            if (term == null) return;
            Console.WriteLine($"Navigating to TermDetailsPage with Term ID: {term.Id}");
            await Shell.Current.GoToAsync($"{nameof(TermDetailsPage)}?Id={term.Id}");
        }

        public ICommand GetTermDetailsCommand { get; }
        public ICommand AddTermCommand { get; }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
