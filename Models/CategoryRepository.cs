
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;


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
        public async Task<Category> GetCategoryById(int id)
        {
            var sql = "SELECT * FROM \"Category\" WHERE \"Id\" = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Category>(sql, parameters) ?? throw new InvalidOperationException("Category not found.");
        }
        public async Task<int> AddAsync(Category obj)
        {
            // Lấy giá trị Id lớn nhất hiện tại
            var maxId = await connection.ExecuteScalarAsync<int?>("SELECT MAX(\"Id\") FROM \"Category\"") ?? 0;
            obj.Id = maxId + 1; // Gán Id mới

            var sql = "INSERT INTO \"Category\" (\"Id\", \"CategoryName\") VALUES (@Id, @CategoryName)";
            return await connection.ExecuteAsync(sql, obj);
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM \"Category\" WHERE \"Id\" = @Id";
            var parameters = new { Id = id };
            await connection.ExecuteAsync(sql, parameters);
        }
    }
}
