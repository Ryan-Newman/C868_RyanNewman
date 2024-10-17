namespace C868_RyanNewman.Services
{
    public class CourseService
    {
        private readonly SQLite.SQLiteAsyncConnection _database;

        public CourseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Course>().Wait(); // Create Course table if not exists
        }

        public async Task SeedOneCourseForTermAsync(int termId)
        {
            var existingCourses = await GetCoursesForTermAsync(termId);
            if (!existingCourses.Any())
            {
                var course = new Course
                {
                    Title = "Sample Course for Spring Term",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(5),
                    DueDate = DateTime.Now.AddMonths(4),
                    Status = "In Progress",
                    OptionalNotes = "Optional Notes added for Evaluation",
                    InstructorName = "Anika Patel",
                    InstructorPhoneNumber = "555-123-4567",
                    InstructorEmail = "anika.patel@strimeuniversity.edu",
                    PerformanceAssessmentName = "Performance Assessment Sample",
                    PerformanceAssessmentStartDate = DateTime.Now.AddDays(1),
                    PerformanceAssessmentEndDate = DateTime.Now.AddDays(7),
                    PerformanceAssessmentStatus = $"Scheduled for {DateTime.Now.AddDays(1).ToString()}",
                    ObjectiveAssessmentName = "Objective Assessment Sample",
                    ObjectiveAssessmentStartDate = DateTime.Now.AddDays(8),
                    ObjectiveAssessmentEndDate = DateTime.Now.AddDays(15),
                    ObjectiveAssessmentStatus = $"Scheduled for {DateTime.Now.AddDays(8).ToString()} ",
                    TermId = termId
                };
                await AddCourseAsync(course);
            }
            }

            public async Task SeedCourseDatabaseAsync(int termId, SQLiteAsyncConnection asyncConn)
        {
            try
            {
                var allTerms = await GetAllTermsAsync();

                foreach (var term in allTerms)
                {
                    var existingCourses = await GetCoursesForTermAsync(term.Id);

                    if (!existingCourses.Any())
                    {
                        var sampleCourses = GenerateCoursesForTerm(term.Name, term.Id);
                        foreach (var course in sampleCourses)
                        {
                            await AddCourseAsync(course);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error seeding courses: {ex.Message}");
            }
        }

        private List<Course> GenerateCoursesForTerm(string termName, int termId)
        {
            return Enumerable.Range(1, 6)
                .Select(i => new Course
                {
                    Title = $"{termName} Course {i}",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(6),
                    TermId = termId
                })
                .ToList();
        }

        // Method to get all courses for a specific term
        public async Task<List<Course>> GetCoursesForTermAsync(int termId)
        {
            return await _database.Table<Course>().Where(c => c.TermId == termId).ToListAsync();
        }

        public async Task<List<Term>> GetAllTermsAsync()
        {
            return await _database.Table<Term>().ToListAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            try
            {
                return await _database.Table<Course>().FirstOrDefaultAsync(c => c.Id == courseId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching course by ID: {ex.Message}");
                return null; 
            }
        }

        // Method to add a new course
        public async Task<bool> AddCourseAsync(Course course)
        {
            await _database.InsertAsync(course);

            return true;
        }

        // Method to update an existing course
        public async Task<int> UpdateCourseAsync(Course course)
        {
            return await _database.UpdateAsync(course);
        }

        // Method to delete a course
        public async Task<int> DeleteCourseAsync(Course course)
        {
            return await _database.DeleteAsync(course);
        }

        public async Task<int> DeletePerformanceAssessmentAsync(int courseId)
        {
            // Fetch the course by ID
            var course = await GetCourseByIdAsync(courseId); 
            if (course == null)
            {
                throw new Exception("Performance Assessment for Course not found.");
            }

            // Set assessment values to null
            course.PerformanceAssessmentName = null;
            course.PerformanceAssessmentStartDate = null;
            course.PerformanceAssessmentEndDate = null;

            // Update the course in the database
            return await _database.UpdateAsync(course);
        }
        public async Task<int> DeleteObjectiveAssessmentAsync(int courseId)
        {
            // Fetch the course by ID
            var course = await GetCourseByIdAsync(courseId); 
            if (course == null)
            {
                throw new Exception("Objective Assessment for Course not found.");
            }

            // Set assessment values to null
            course.ObjectiveAssessmentName = null;
            course.ObjectiveAssessmentStartDate = null;
            course.ObjectiveAssessmentEndDate = null;

            // Update the course in the database
            return await _database.UpdateAsync(course);
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _database.Table<Course>().ToListAsync();
        }
    }
}
