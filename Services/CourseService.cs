namespace C868_RyanNewman.Services
{
    public class CourseService
    {
        private readonly SQLiteAsyncConnection _database;

        public CourseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Course>().Wait(); // Create Course table if not exists
        }

        public async Task SeedOneCourseForTermAsync(int termId, int userId)
        {
            var existingCourses = await GetCoursesForTermAndUserAsync(termId, userId);
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
                    TermId = termId,
                    UserId = userId // Associate course with the user
                };
                await AddCourseAsync(course);
            }
        }

        public async Task<List<Course>> GetCoursesForTermAndUserAsync(int termId, int userId)
        {
            return await _database.Table<Course>()
                .Where(c => c.TermId == termId && c.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Course>> GetAllCoursesForUserAsync(int userId)
        {
            return await _database.Table<Course>()
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Course> GetCourseByIdForUserAsync(int courseId, int userId)
        {
            try
            {
                return await _database.Table<Course>()
                    .FirstOrDefaultAsync(c => c.Id == courseId && c.UserId == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching course by ID for user {userId}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddCourseAsync(Course course)
        {
            if (course == null || course.UserId <= 0)
            {
                Console.WriteLine("Invalid course or UserId.");
                return false;
            }

            try
            {
                await _database.InsertAsync(course);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding course: {ex.Message}");
                return false;
            }
        }

        public async Task<int> UpdateCourseAsync(Course course)
        {
            if (course == null || course.UserId <= 0)
            {
                Console.WriteLine("Invalid course or UserId.");
                return 0;
            }

            try
            {
                return await _database.UpdateAsync(course);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating course: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteCourseAsync(Course course)
        {
            if (course == null || course.UserId <= 0)
            {
                Console.WriteLine("Invalid course or UserId.");
                return 0;
            }

            try
            {
                return await _database.DeleteAsync(course);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting course: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeletePerformanceAssessmentAsync(int courseId, int userId)
        {
            var course = await GetCourseByIdForUserAsync(courseId, userId);
            if (course == null)
            {
                throw new Exception("Performance Assessment for Course not found.");
            }

            // Set assessment values to null
            course.PerformanceAssessmentName = null;
            course.PerformanceAssessmentStartDate = null;
            course.PerformanceAssessmentEndDate = null;

            return await UpdateCourseAsync(course);
        }

        public async Task<int> DeleteObjectiveAssessmentAsync(int courseId, int userId)
        {
            var course = await GetCourseByIdForUserAsync(courseId, userId);
            if (course == null)
            {
                throw new Exception("Objective Assessment for Course not found.");
            }

            // Set assessment values to null
            course.ObjectiveAssessmentName = null;
            course.ObjectiveAssessmentStartDate = null;
            course.ObjectiveAssessmentEndDate = null;

            return await UpdateCourseAsync(course);
        }
        private int _userId;

        public void SetUserId(int userId)
        {
            _userId = userId;
        }

    }

}