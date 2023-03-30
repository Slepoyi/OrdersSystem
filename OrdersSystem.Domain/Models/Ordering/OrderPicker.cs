using OrdersSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class OrderPicker
    {
        public OrderPicker(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        [Key]
        public Guid Id { get; private set; }
        [Required]
        public string Name { get; private set; }
        public string Role { get; } = UserRole.Picker;
        public List<Order>? Orders { get; set; }
    }
}
