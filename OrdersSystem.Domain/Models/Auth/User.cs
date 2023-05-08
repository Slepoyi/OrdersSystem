using OrdersSystem.Domain.Models.Ordering;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Auth
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(8)]
        public string Password { get; set; }
    }
}
