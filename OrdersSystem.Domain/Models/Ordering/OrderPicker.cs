using OrdersSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class OrderPicker
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Role { get; } = UserRole.Picker;
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}
