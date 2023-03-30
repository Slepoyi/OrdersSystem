using OrdersSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class Customer
    {
        public Customer(Guid id, string name,
            string address, string phone,
            string email)
        {
            Id = id;
            Name = name;
            Address = address;
            Phone = phone;
            Email = email;
        }

        [Key]
        public Guid Id { get; private set; }
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
        public List<Order>? Orders { get; set; }
    }
}
