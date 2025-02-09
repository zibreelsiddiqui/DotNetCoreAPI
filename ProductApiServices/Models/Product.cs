using System.ComponentModel.DataAnnotations;

namespace ProductApiServices.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
