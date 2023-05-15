using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrdersSystem.Domain.Models.Auth
{
    public class User
    {
        [Required]
        public Guid Id { get; set; }
        [Key]
        public string Username { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(8)]
        [JsonIgnore]
        public string Password { get; set; }
    }
}
