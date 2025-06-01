using System.Data;
using Npgsql; // Thêm namespace này để dùng NpgsqlConnection

namespace BackendHtml.Models;
public abstract class RepositoryBase
{
    protected IDbConnection connection;

    public RepositoryBase(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string not found");
        connection = new NpgsqlConnection(connectionString);
    }
}
