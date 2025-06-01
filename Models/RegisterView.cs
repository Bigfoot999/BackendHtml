using System.ComponentModel.DataAnnotations;

namespace BackendHtml.Models
{
    public class RegisterView
    {

        public string Username { get; set; } = null!;
        public string Fullname { get; set; } = null!;

        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

    }
}
