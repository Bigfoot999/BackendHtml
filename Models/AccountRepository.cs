using Dapper;
using LoginRegisterExample.Models;

namespace BackendHtml.Models
{
    public class AccountRepository : RepositoryBase
    {
        public AccountRepository(IConfiguration configuration) : base(configuration)
        {
            // Constructor to initialize the connection string
            // and create a new NpgsqlConnection instance.
        }
        public async Task<User> GetUserById(String id)
        {
            // var sql = "SELECT * FROM AIContent WHERE UserID = @UserID";
            var sql = "SELECT * FROM \"Users\" WHERE \"Id\" = @UserID";

            var parameters = new { UserID = id };
            var result = await connection.QuerySingleAsync<User>(sql, parameters);

            return result ?? throw new InvalidOperationException("User not found.");
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var sql = "SELECT * FROM \"Users\" WHERE \"Email\" = @Email";

            var parameters = new { Email = email };
            var result = await connection.QuerySingleOrDefaultAsync<User>(sql, parameters);

            return result ?? throw new InvalidOperationException("User not found.");
        }
        public async Task<bool> CreateUser(User user)
        {
            var sql = "INSERT INTO \"Users\" (\"Id\", \"Fullname\", \"Email\", \"PasswordHash\") VALUES (@Id, @Fullname, @Email, @PasswordHash)";

            var parameters = new
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

            var result = await connection.ExecuteAsync(sql, parameters);
            return result > 0; // Returns true if one or more rows were affected
        }
        public async Task<bool> UpdateUser(User user)
        {
            var sql = "UPDATE \"Users\" SET \"Fullname\" = @Fullname, \"Email\" = @Email, \"PasswordHash\" = @PasswordHash WHERE \"Id\" = @Id";

            var parameters = new
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

            var result = await connection.ExecuteScalarAsync(sql, parameters);
            return result != null && (int)result > 0; // Return true if result is not null and greater than 0
        }

    }
}