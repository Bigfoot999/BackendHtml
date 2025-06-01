using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackendHtml.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  //id tự tăng
        public int Id { get; set; }
        public string CategoryName { get; set; } = null;

        public string Description { get; set; } = null;

    }
}
