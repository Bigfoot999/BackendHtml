using System.Collections.Generic;
using BackendHtml.Models;
using Dapper;

namespace BackendHtml.Models;

public class AIRepository : RepositoryBase
{
    public AIRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<AIContent> GetAIContentById(int id)
    {
        // var sql = "SELECT * FROM AIContent WHERE Id = @Id";
        var sql = "SELECT * FROM \"AIContent\" WHERE \"Id\" = @Id";
        var parameters = new { id };
        return await connection.QueryFirstOrDefaultAsync<AIContent>(sql, parameters) ?? throw new InvalidOperationException("AIContent not found.");
    }
    public async Task<IEnumerable<AIContent>> GetAIContentUserById(String id)
    {
        // var sql = "SELECT * FROM AIContent WHERE UserID = @UserID";
        var sql = "SELECT * FROM \"AIContent\" WHERE \"UserID\" = @UserID";
        var parameters = new { UserID = id };
        var result = await connection.QueryAsync<AIContent>(sql, parameters);
        return result ?? throw new InvalidOperationException("AIContent not found.");
    }

    public async Task<IEnumerable<AIContent>> GetAllAIContents()
    {
        var sql = @"SELECT * FROM ""AIContent""  WHERE ""ImageUrl"" IS NOT NULL AND ""CategoryContent"" IS NOT NULL";
        return await connection.QueryAsync<AIContent>(sql);
    }

    public async Task<AIContent> InsertAIContent(AIContent aiContent)
    {
        // var sql = @"
        // INSERT INTO AIContent (content, userid)
        // VALUES (@content, @userid)
        // RETURNING Id, Content, UserID;";
        var sql = @"
                INSERT INTO ""AIContent"" (""Title"",""Content"", ""UserID"", ""Username"")
                VALUES (@Title, @Content, @UserID ,@Username)
                RETURNING ""Title"",""Id"", ""Content"", ""UserID"",""Username"""; ;

        var parameters = new { aiContent.Content, aiContent.UserID, aiContent.Title, aiContent.Username };
        var insertedContent = await connection.QuerySingleAsync<AIContent>(sql, parameters);
        return insertedContent;
    }

    public async Task<int> UpdateAIContent(AIContent aiContent)
    {
        var sql = "UPDATE AIcontent SET Content = @Content WHERE Id = @Id";
        var parameters = new { aiContent.Content, aiContent.Id };
        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<int> DeleteAIContent(int id)
    {
        System.Console.WriteLine("------------------------------------------------");
        System.Console.WriteLine($"Deleting AIContent with Id: {id}");
        var sql = "DELETE FROM \"AIContent\" WHERE \"Id\" = @Id";
        var parameters = new { Id = id };
        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<int> Edit(AIContent obj)
    {

        {
            if (obj == null || obj.Id <= 0)
            {
                return 0;
            }

            const string sql = @"
    UPDATE ""AIContent""
    SET
        ""Title"" = @Title,
        ""Content"" = @Content,
        ""UserID"" = @UserID,
        ""ImageUrl"" = @ImageUrl,
        ""CategoryContent"" = @CategoryContent
    WHERE ""Id"" = @Id";

            return await connection.ExecuteAsync(sql, obj);
        }
    }

    public async Task<IEnumerable<AIContent>> GetAllAIContentsByCategory(string category)
    {
        // var sql = "SELECT * FROM AIContent WHERE UserID = @UserID";
        var sql = "SELECT * FROM \"AIContent\" WHERE \"CategoryContent\" = @CategoryContent";
        var parameters = new { CategoryContent = category };
        var result = await connection.QueryAsync<AIContent>(sql, parameters);
        return result ?? throw new InvalidOperationException("AIContent not found.");
    }
}