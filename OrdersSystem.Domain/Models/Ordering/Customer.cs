using OrdersSystem.Domain.Enums;
using OrdersSystem.Domain.Models.Auth;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class Customer
    {
        public Customer() { }

        [Key]
        public Guid Id { get; private set; }
        public virtual User User { get; set; }

        [Required]
        public string Name { get; private set; }
        [Required]
        public string Address { get; private set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; private set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; private set; }
        public string Role { get; } = UserRole.Customer;
        public ICollection<Order>? Orders { get; private set; } = new List<Order>();
    }
}
