using OrdersSystem.Domain.Models.Ordering;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Auth
{
    public class User
    {
        public User() { }
        public User(Guid id, string role,
            string password)
        {
            Id = id;
            Role = role;
            Password = password;
        }

        [Key]
        public Guid Id { get; private set; }
        public virtual Customer? Customer { get; private set; }
        public Guid? CustomerId { get; private set; }
        public virtual OrderPicker? OrderPicker { get; private set; }
        public Guid? OrderPickerId { get; private set; }
        [Required]
        public string Role { get; private set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(8)]
        public string Password { get; private set; }
    }
}
