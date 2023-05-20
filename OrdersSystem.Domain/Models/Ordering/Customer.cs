using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}
