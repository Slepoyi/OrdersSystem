using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Auth
{
    public class LoginModel
    {
        [Required]
        [MinLength(6)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(8)]
        public string Password { get; set; }
    }
}
