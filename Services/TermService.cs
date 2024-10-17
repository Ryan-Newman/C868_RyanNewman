namespace C868_RyanNewman.Services
{
    public class TermService
    {
        private SQLiteAsyncConnection conn;
        string _dbPath;
        public string StatusMessage;

        public TermService(string dbPath)
        {
            if (string.IsNullOrEmpty(dbPath))
            {
                throw new ArgumentNullException(nameof(dbPath), "Database path cannot be null or empty.");
            }

            conn = new SQLiteAsyncConnection(dbPath);

            _dbPath = dbPath;
            // Start initialization without blocking the constructor
            InitAsync().ConfigureAwait(false);
        }

        public async Task CreateTablesAsync()
        {
            try
            {
                await conn.CreateTableAsync<Term>();
                await conn.CreateTableAsync<Course>();
                //await conn.CreateTableAsync<Assessment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
            }
        }

        // Checks if a connection has been made
        public async Task InitAsync()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "C971.db3");
            Console.WriteLine($"_dbPath: {_dbPath}");

            if (conn == null)
            {
                conn = new SQLiteAsyncConnection(_dbPath);
                await conn.CreateTableAsync<Term>();
            }
        }

        // Method to fetch a term by ID
        public async Task<Term> GetTermByIdAsync(int termId)
        {
            try
            {
                return await conn.Table<Term>().FirstOrDefaultAsync(t => t.Id == termId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching term by ID: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Term>> GetTerms()
        {
            try
            {
                InitAsync();
                return await conn.Table<Term>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to retrieve data.";
            }
            return new List<Term>();
        }
        public Task<int> AddTermAsync(Term term)
        {
            return conn.InsertAsync(term);
        }

        #region DatabaseSeedingLogic
        // Logic to seed one term for evaluation requirement C6
        public async Task SeedOneTermAsync()
        {

            var term = new Term { Name = "Spring Term", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6) };
            await conn.InsertAsync(term);

            // Assuming Term has an ID that auto-increments
            var termId = term.Id; // or retrieve the ID after insert if not automatically set

            // Now seed one course for this term using CourseService
            var courseService = new CourseService(_dbPath);
            await courseService.SeedOneCourseForTermAsync(termId);
        }

        #endregion

        public async Task<int> UpdateTermAsync(Term term)
        {
            return await conn.UpdateAsync(term);
        }

        public async Task<int> DeleteTermAsync(Term term)
        {
            return await conn.DeleteAsync(term);
        }

    }
}