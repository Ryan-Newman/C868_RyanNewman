namespace C868_RyanNewman.Services
{
    public class UserService
    {
        private readonly SQLiteAsyncConnection _database;

        public UserService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<User>().Wait();
        }

        public async Task<int> RegisterUserAsync(string username, string password)
        {
            Console.WriteLine("RegisterUserAsync called");
            var existingUser = await _database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser != null)
            {
                Console.WriteLine("User already exists");
                return 0;
            }

            // Hash the password before saving
            var hashedPassword = HashPassword(password);
            var user = new User { Username = username, PasswordHash = hashedPassword };

            try
            {
                await _database.InsertAsync(user);
                Console.WriteLine("User registered successfully");
                return user.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return 0;
            }
        }


        public async Task<User> LoginUserAsync(string username, string password)
        {
            var user = await _database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }



        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }
    }

}
