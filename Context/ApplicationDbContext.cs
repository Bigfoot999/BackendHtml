using BackendHtml.Models;
using LoginRegisterExample.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendHtml.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AIContent> AIContent { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
