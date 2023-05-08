using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Stock
{
    public class Sku
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public byte[]? Photo { get; set; }
    }
}
