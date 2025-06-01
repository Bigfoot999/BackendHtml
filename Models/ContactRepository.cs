using System.Threading.Tasks;
using Dapper;

namespace BackendHtml.Models
{


    public class ContactRepository : RepositoryBase
    {


        public ContactRepository(IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<List<Contact>> GetContacts()
        {
            return (await connection.QueryAsync<Contact>("SELECT * FROM \"Contacts\"")).ToList();
        }

        public async Task<int> Add(Contact obj)
        {
            return await connection.ExecuteAsync(
                "INSERT INTO \"Contacts\" (\"Name\", \"Email\", \"Phone\", \"Message\") VALUES (@Name, @Email, @Phone, @Message)",
                obj
            );
        }

    }
}