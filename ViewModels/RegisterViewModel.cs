namespace C868_RyanNewman.ViewModels
{
    public class RegisterViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly UserService _userService;
        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public RegisterViewModel(UserService userService)
        {
            _userService = userService;
            RegisterCommand = new Command(async () => await RegisterAsync());
            NavigateToLoginCommand = new Command(async () => await NavigateToLoginPageAsync());
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Shell.Current.DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            var userId = await _userService.RegisterUserAsync(Username, Password);

           
            if (userId > 0)
            {
                // Registration successful, create a new instance of AppShell and set it as the main page
                await Shell.Current.DisplayAlert("Success", "Registration complete. Redirecting to the main page.", "OK");
                App.CurrentUserId = userId;

                // Redirect to login page with username, password, and auto-login flag
                await Shell.Current.GoToAsync($"//UserLoginPage?username={Username}&password={Password}&autoLogin=true");

            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "User already exists. Please choose a different username.", "OK");
            }
        }

        private async Task NavigateToLoginPageAsync()
        {
            // Navigate back to the login page
            //await Shell.Current.GoToAsync("//UserLoginPage");
            await Shell.Current.GoToAsync("..");

        }
    
    // INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
