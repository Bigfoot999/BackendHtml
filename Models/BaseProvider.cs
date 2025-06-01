using System.Data;
using Npgsql;

public abstract class BaseProvider : IDisposable
{
    IDbConnection? connection;
    IConfiguration configuration;
    public BaseProvider(IConfiguration configuration) => this.configuration = configuration;
    protected IDbConnection Connection
    => connection ??= new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));

    public void Dispose()
    {
        if (connection != null)
            connection.Dispose();
    }
}