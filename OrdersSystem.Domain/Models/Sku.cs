using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models
{
    public class Sku
    {
        public Sku(Guid id, string name,
            string description, double cost,
            uint stockBalance, byte[] photo)
        {
            Id = id;
            Name = name;
            Description = description;
            Cost = cost;
            StockBalance = stockBalance;
            Photo = photo;
        }
        public Sku(Guid id, string name,
            string description, double cost,
            uint stockBalance)
        {
            Id = id;
            Name = name;
            Description = description;
            Cost = cost;
            StockBalance = stockBalance;
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
    }
}
