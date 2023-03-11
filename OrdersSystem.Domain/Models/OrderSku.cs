using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models
{
    public class OrderSku
    {
        public OrderSku(Guid id, string name,
            string description, double cost,
            uint stockBalance, byte[] photo,
            uint amount)
        {
            Id = id;
            Name = name;
            Description = description;
            Cost = cost;
            StockBalance = stockBalance;
            Photo = photo;
            Amount = amount;
        }
        public OrderSku(Guid id, string name,
           string description, double cost,
           uint stockBalance, uint amount)
        {
            Id = id;
            Name = name;
            Description = description;
            Cost = cost;
            StockBalance = stockBalance;
            Amount = amount;
        }

        [Key]
        public Guid Id { get; private set; }
        [Required]
        public string Name { get; private set; }
        [Required]
        public string Description { get; private set; }
        [Required]
        public double Cost { get; private set; }
        [Required]
        public uint StockBalance { get; private set; }
        public byte[]? Photo { get; private set; }
        [Required]
        public uint Amount { get; private set; }

        public void SetAmount(uint amount)
        {
            Amount = amount;
        }
    }
}
