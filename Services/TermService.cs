namespace C868_RyanNewman.Services
{
    public class TermService
    {
        private SQLiteAsyncConnection conn;
        private string _dbPath;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
            }
        }

        public async Task InitAsync()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "C868.db3");
            Console.WriteLine($"_dbPath: {_dbPath}");

            if (conn == null)
            {
                conn = new SQLiteAsyncConnection(_dbPath);
                await conn.CreateTableAsync<Term>();
            }
        }

        // Fetch terms for the current user
        public async Task<List<Term>> GetTermsForUserAsync(int userId)
        {
            try
            {
                return await conn.Table<Term>().Where(t => t.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching terms for user {userId}: {ex.Message}");
                return new List<Term>();
            }
        }

        // Fetch a term by ID for a specific user
        public async Task<Term> GetTermByIdForUserAsync(int termId, int userId)
        {
            try
            {
                return await conn.Table<Term>().FirstOrDefaultAsync(t => t.Id == termId && t.UserId == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching term by ID for user {userId}: {ex.Message}");
                return null;
            }
        }

        // Add a term with the user ID
        public async Task<int> AddTermAsync(Term term)
        {
            if (term == null || term.UserId <= 0)
            {
                Console.WriteLine("Invalid term or UserId.");
                return 0;
            }

            try
            {
                return await conn.InsertAsync(term);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding term: {ex.Message}");
                return 0;
            }
        }

        // Update a term with user ID check
        public async Task<int> UpdateTermAsync(Term term)
        {
            if (term == null || term.UserId <= 0)
            {
                Console.WriteLine("Invalid term or UserId.");
                return 0;
            }

            try
            {
                return await conn.UpdateAsync(term);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating term: {ex.Message}");
                return 0;
            }
        }

        // Delete a term with user ID check
        public async Task<int> DeleteTermAsync(Term term)
        {
            if (term == null || term.UserId <= 0)
            {
                Console.WriteLine("Invalid term or UserId.");
                return 0;
            }

            try
            {
                return await conn.DeleteAsync(term);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting term: {ex.Message}");
                return 0;
            }
        }

        // Seed one term for a specific user
        public async Task SeedOneTermForUserAsync(int userId)
        {
            var term = new Term
            {
                Name = "Spring Term",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(6),
                UserId = userId
            };

            try
            {
                await conn.InsertAsync(term);

                // Seed one course for this term
                var courseService = new CourseService(_dbPath);
                await courseService.SeedOneCourseForTermAsync(term.Id, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding term for user {userId}: {ex.Message}");
            }
        }
        private int _userId;

        public void SetUserId(int userId)
        {
            _userId = userId;
        }

    }

}