using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models
{
    public class Customer
    {
        [Key]
        public Guid Id { get; private set; }
        [Required]
        public string Name { get; private set; }
        [Required]
        public string Address { get; private set; }
        [Required]
        public string Phone { get; private set; }
        public UserRole Role { get; } = UserRole.Customer;
        public List<Order> Orders { get; private set; }
    }
}
