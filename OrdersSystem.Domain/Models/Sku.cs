using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models
{
    public class Sku
    {
        public Sku(Guid id, string name,
            string description, decimal price,
            uint stockBalance, byte[]? photo)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            StockBalance = stockBalance;
            Photo = photo;
        }

        [Key]
        public Guid Id { get; private set; }
        [Required]
        public string Name { get; private set; }
        [Required]
        public string Description { get; private set; }
        [Required]
        public decimal Price { get; private set; }
        [Required]
        public uint StockBalance { get; private set; }
        public byte[]? Photo { get; private set; }
    }
}
