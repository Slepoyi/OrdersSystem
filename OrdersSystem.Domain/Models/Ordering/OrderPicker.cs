using OrdersSystem.Domain.Enums;
using OrdersSystem.Domain.Models.Auth;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class OrderPicker
    {
        public OrderPicker() { }
        public OrderPicker(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        [Key]
        public Guid Id { get; private set; }
        public virtual User User { get; set; }
        [Required]
        public string Name { get; private set; }
        public string Role { get; } = UserRole.Picker;
        public ICollection<Order>? Orders { get; private set; } = new List<Order>();
    }
}
