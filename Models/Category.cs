using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackendHtml.Models
{
    public class Category
    {
        [Key]

        public int Id { get; set; }
        public string CategoryName { get; set; } = null;

        public string? Description { get; set; } = null;

    }
}
