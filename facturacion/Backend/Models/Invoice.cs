using System.ComponentModel.DataAnnotations;

namespace FacturacionAPI.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string InvoiceNumber { get; set; } = string.Empty;
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        
        public DateTime DueDate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal SubTotal { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Tax { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Cancelled
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}