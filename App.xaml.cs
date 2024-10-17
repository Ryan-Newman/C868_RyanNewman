namespace C868_RyanNewman
{
    public partial class App : Application
    {
        public static TermService TermService { get; private set; }
        public static CourseService CourseService { get; private set; }
        private SQLiteAsyncConnection _database;
        private TermService _termService;

        public App(TermService termService)
        {
            InitializeComponent();
            //DeleteDatabase();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C971.db3");

            InitializeServicesAndDatabase(dbPath);

            MainPage = new AppShell();
        }

        private async void InitializeServicesAndDatabase(string dbPath)
        {
            // Initialize services
            TermService = new TermService(dbPath);
            CourseService = new CourseService(dbPath);

            // Seed the database if it's empty or on first run
            await SeedDatabaseAsync();
        }

        private async Task SeedDatabaseAsync()
        {
            // Create tables if they don't exist
            await TermService.CreateTablesAsync();

            // Check if terms exist, if not, seed one term with one course
            var terms = await TermService.GetTerms();
            if (terms == null || !terms.Any())
            {
                await TermService.SeedOneTermAsync();
            }
            else
            {
                Console.WriteLine("Terms already exist in the database, skipping seed.");
            }
        }

        public static void DeleteDatabase()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C971.db3");
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
