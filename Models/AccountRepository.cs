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

    }
}