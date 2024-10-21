namespace C868_RyanNewman.ViewModels
{
    [QueryProperty(nameof(Username), "username")]
    [QueryProperty(nameof(Password), "password")]
    [QueryProperty(nameof(AutoLogin), "autoLogin")]
    public class UserLoginViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private readonly TermService _termService;
        private readonly CourseService _courseService;

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        private bool _autoLogin;
        public bool AutoLogin
        {
            get => _autoLogin;
            set
            {
                _autoLogin = value;
                OnPropertyChanged(nameof(AutoLogin));
                if (_autoLogin) LoginCommand.Execute(null); // Trigger auto-login
            }
        }
        public UserLoginViewModel(UserService userService, TermService termService, CourseService courseService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _termService = termService ?? throw new ArgumentNullException(nameof(termService));
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));

            LoginCommand = new Command(async () => await LoginAsync());
            NavigateToRegisterCommand = new Command(async () => await NavigateToRegisterPageAsync());
        }

        public async Task InitializeAsync(string username, string password, bool autoLogin = false)
        {
            Username = username;
            Password = password;

            OnPropertyChanged(nameof(Username));
            OnPropertyChanged(nameof(Password));

            if (autoLogin)
            {
                await LoginAsync();
            }
        }
        private async Task LoginAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert("Error", "Username or Password cannot be empty.", "OK");
                    return;
                }

                Console.WriteLine("Attempting login...");
                var user = await _userService.LoginUserAsync(Username, Password);

                if (user != null)
                {
                    Console.WriteLine("Login successful");

                    App.SetCurrentUser(user.Id);
                    // Set the UserId for term and course services
                    _termService.SetUserId(user.Id);
                    _courseService.SetUserId(user.Id);

                    // Navigate to the main page after setting the user context
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    Console.WriteLine("Login failed");
                    await Shell.Current.DisplayAlert("Error", "Invalid username or password.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoginAsync error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToRegisterPageAsync()
        {
            Console.WriteLine("Attempting to navigate to RegisterPage");

            try
            {
                await Shell.Current.GoToAsync(nameof(RegisterPage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
            }
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}