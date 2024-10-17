namespace C868_RyanNewman
{
    public partial class MainPage : ContentPage
    {
        private readonly TermViewModel termViewModel;
        Term term;
        public MainPage(TermViewModel vm)
        {
            InitializeComponent();
            this.termViewModel = vm;
            BindingContext = vm;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                if (!termViewModel.IsBusy)
                {
                    await termViewModel.GetTerms(); // Refresh terms on page appearing
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing terms: {ex.Message}");
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnEditButtonClicked(Object sender, EventArgs e)
        {
            var selectedTerm = (sender as Button)?.BindingContext as Term;

            if (selectedTerm != null)
            {
                // Navigate to TermDetailsPage and pass the term ID
                await Shell.Current.GoToAsync($"{nameof(TermDetailsPage)}?Id={selectedTerm.Id}");
            }
        }


        private async void OnGetTermsButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (!termViewModel.IsBusy)
                {
                    await termViewModel.GetTerms();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnAddTermsButtonClicked(Object sender, EventArgs e)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C971.db3");
            try
            {
                if (!termViewModel.IsBusy)
                {
                    await Shell.Current.GoToAsync($"{nameof(AddTermPage)}?dbPath={dbPath}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

    }

}

