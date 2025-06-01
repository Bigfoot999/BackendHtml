
using System.Data;
using Dapper;


namespace BackendHtml.Models
{
    public class CategoryRepository : RepositoryBase
    {
        public CategoryRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public IEnumerable<Category> GetCategories()
        {
            return connection.Query<Category>("SELECT * FROM \"Category\"");
        }

        public int Add(Category obj)
        {
            return connection.Execute("INSERT INTO \"Category\"(CategoryName) VALUES (@CategoryName)", obj);
        }
    }
}
