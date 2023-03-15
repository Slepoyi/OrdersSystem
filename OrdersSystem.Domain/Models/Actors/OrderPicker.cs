using OrdersSystem.Domain.Models.Ordering;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Actors
{
    public class OrderPicker
    {
        [Key]
        public Guid Id { get; private set; }
        [Required]
        public string Name { get; private set; }
        public UserRole Role { get; } = UserRole.Picker;
        public List<Order> Orders { get; private set; }
    }
}
