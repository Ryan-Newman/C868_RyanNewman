
namespace C868_RyanNewman
{
    public partial class App : Application
    {
        public static TermService TermService { get; private set; }
        public static CourseService CourseService { get; private set; }
        public static UserService UserService { get; private set; }
        private SQLiteAsyncConnection _database;
        private TermService _termService;
        private bool _isUserLoggedIn = false;
        public static int CurrentUserId { get; set; }

        public App(TermService termService)
        {
            InitializeComponent();
            //DeleteDatabase();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C868.db3");

            UserService = new UserService(dbPath);

            InitializeServicesAndDatabase(dbPath);

            SetMainPageBaseOnLogicStatus();
            //MainPage = new AppShell();
        }

        private void SetMainPageBaseOnLogicStatus()
        {
            if (_isUserLoggedIn)
            {
                // If the user is logged in, set MainPage to AppShell (main app navigation)
                MainPage = new AppShell();
            }
            else
            {
                // Set MainPage to AppShell with a redirection to UserLoginPage
                MainPage = new AppShell();
                Shell.Current.GoToAsync("//UserLoginPage", true);
            }
        }

        public static void SetCurrentUser(int userId)
        {
            CurrentUserId = userId;
        }

        private async void InitializeServicesAndDatabase(string dbPath)
        {
            // Initialize services
            TermService = new TermService(dbPath);
            CourseService = new CourseService(dbPath);
            UserService = new UserService(dbPath);

            // Seed the database if it's empty or on first run
            await SeedDatabaseAsync(App.CurrentUserId);
        }

        public void OnLoginSuccess(int userId)
        {
            _isUserLoggedIn = true;
            SetCurrentUser(userId);
            MainPage = new AppShell(); // Redirect to main app after successful login
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);
        }

        private async Task SeedDatabaseAsync(int userId)
        {
            // Create tables if they don't exist
            await TermService.CreateTablesAsync();

            // Check if terms exist, if not, seed one term with one course
            var terms = await TermService.GetTermsForUserAsync(userId);
            if (terms == null || !terms.Any())
            {
                await TermService.SeedOneTermForUserAsync(userId);
            }
            else
            {
                Console.WriteLine("Terms already exist in the database, skipping seed.");
            }
        }

        public static void DeleteDatabase()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C868.db3");
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
                Console.WriteLine("Database deleted successfully.");
            }
            else
            {
                Console.WriteLine("Database file does not exist.");
            }
        }
    }
}
