using System.ComponentModel.DataAnnotations;

namespace FacturacionAPI.Models
{
    public class InvoiceDetail
    {
        public int Id { get; set; }
        
        [Required]
        public int InvoiceId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que 0")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal SubTotal { get; set; }
        
        [Range(0, 100)]
        public decimal Discount { get; set; } = 0;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }

        // Navigation properties
        public virtual Invoice Invoice { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}