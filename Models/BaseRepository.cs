using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace BackendHtml.Models;

public abstract class BaseRepository
{
    protected IDbConnection connection;
    //protected String connectionString;
    public BaseRepository(IDbConnection connection)
        => this.connection = connection;

}
