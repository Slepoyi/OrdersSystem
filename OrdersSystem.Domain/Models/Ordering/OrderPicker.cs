using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class OrderPicker
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}
