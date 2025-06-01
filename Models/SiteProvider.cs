namespace BackendHtml.Models
{
    public class SiteProvider : BaseProvider
    {
        public SiteProvider(IConfiguration configuration) : base(configuration)
        {
        }

        CategoryRepository? category;
        // public CategoryRepository Category => category ??= new CategoryRepository(Connection);

        //public CategoryRepository Category => category ??= new CategoryRepository(Connection);    

    }
}