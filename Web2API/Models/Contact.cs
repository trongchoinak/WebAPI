using System.ComponentModel.DataAnnotations;

namespace Web2API.Models
{
    public class ContactModel

    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc"), EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tin nhắn là bắt buộc")]
        public string Message { get; set; }
    }
}
