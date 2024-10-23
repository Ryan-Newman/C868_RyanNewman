namespace C868_RyanNewman.ViewModels
{
    [QueryProperty(nameof(Id), "Id")]
    public partial class TermDetailsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly TermService _termService;
        private Term _term;

        public TermDetailsViewModel(TermService termService)
        {
            _termService = termService;
            SaveCommand = new Command(async () => await SaveTerm());
            DeleteCommand = new Command(async () => await DeleteTerm());
            _term = new Term();
        }
        // Property for TermId to hold the ID of the term to be fetched
        public int Id
        {
            get => _term.Id;
            set
            {
                if (_term != null)
                {
                    _term.Id = value;
                    LoadTermAsync(value);
                }
            }
        }
        private async void LoadTermAsync(int id)
        {
            if (id <= 0) return; // Validate the id

            _term = await _termService.GetTermByIdForUserAsync(id, App.CurrentUserId); 
            OnPropertyChanged(nameof(Term)); // Notify that the term has changed
            Title = $"Term Details - {_term?.Name}"; // Update title
        }
        public Term Term 
        {
            get => _term;
            set
            {
                if (_term != value)
                {
                    _term = value;
                    OnPropertyChanged(nameof(Term));
                    Title = $"Term Details - {_term?.Name}";
                }
            }
        }
        public ICommand SaveCommand { get; }
        private async Task SaveTerm()
        {
            if(_term == null) return;

            await _termService.UpdateTermAsync(_term);
            await Shell.Current.GoToAsync("..");
        }

        public ICommand DeleteCommand { get; }
        private async Task DeleteTerm()
        {
            if (_term == null) return;

            // Show a warning confirmation dialog
            bool isConfirmed = await Shell.Current.DisplayAlert("Confirm Delete",
            $"Are you sure you want to delete the term '{_term.Name}'?",
            "Delete",
            "Cancel");

            if (isConfirmed) {
                // Delete the term from the database
                await _termService.DeleteTermAsync(_term);
                await Shell.Current.GoToAsync(".."); // Navigate back to the previous page
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
