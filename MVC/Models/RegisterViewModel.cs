using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class RegisterViewModel
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}
