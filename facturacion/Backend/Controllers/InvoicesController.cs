using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Controllers
{
    public class CreateInvoiceRequest
    {
        public int CustomerId { get; set; }
        public DateTime DueDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<CreateInvoiceDetailRequest> Details { get; set; } = new List<CreateInvoiceDetailRequest>();
    }

    public class CreateInvoiceDetailRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; } = 0;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly FacturacionDbContext _context;

        public InvoicesController(FacturacionDbContext context)
        {
            _context = context;
        }

        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetInvoices()
        {
            try
            {
                // Check if any invoices exist
                var count = await _context.Invoices.CountAsync();
                
                if (count == 0)
                {
                    return Ok(new List<object>());
                }

                // Try the simplest possible query first
                var invoices = await _context.Invoices
                    .AsNoTracking()
                    .Take(10)
                    .ToListAsync();

                // Convert to simple objects
                var result = invoices.Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.CustomerId,
                    InvoiceDate = i.InvoiceDate.ToString("yyyy-MM-dd"),
                    DueDate = i.DueDate.ToString("yyyy-MM-dd"),
                    i.SubTotal,
                    i.Tax,
                    i.Total,
                    i.Status,
                    CreatedDate = i.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = ex.Message, 
                    innerException = ex.InnerException?.Message,
                    source = ex.Source
                });
            }
        }

        // GET: api/Invoices/test
        [HttpGet("test")]
        public ActionResult<object> TestInvoices()
        {
            return Ok(new { 
                message = "Invoices controller is working", 
                timestamp = DateTime.Now,
                invoiceCount = _context.Invoices.Count()
            });
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceDetails)
                        .ThenInclude(d => d.Product)
                    .Where(i => i.Id == id)
                    .Select(i => new
                    {
                        i.Id,
                        i.InvoiceNumber,
                        i.CustomerId,
                        CustomerName = i.Customer.Name,
                        CustomerEmail = i.Customer.Email,
                        CustomerPhone = i.Customer.Phone,
                        i.InvoiceDate,
                        i.DueDate,
                        i.SubTotal,
                        i.Tax,
                        i.Total,
                        i.Notes,
                        i.Status,
                        i.CreatedDate,
                        InvoiceDetails = i.InvoiceDetails.Select(d => new
                        {
                            d.Id,
                            d.ProductId,
                            ProductName = d.Product.Name,
                            d.Quantity,
                            d.UnitPrice,
                            d.Discount,
                            d.Total
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (invoice == null)
                {
                    return NotFound();
                }

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/Invoices/customer/5
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetInvoicesByCustomer(int customerId)
        {
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceDetails)
                        .ThenInclude(d => d.Product)
                    .Where(i => i.CustomerId == customerId)
                    .OrderByDescending(i => i.CreatedDate)
                    .Select(i => new
                    {
                        i.Id,
                        i.InvoiceNumber,
                        i.CustomerId,
                        CustomerName = i.Customer.Name,
                        i.InvoiceDate,
                        i.DueDate,
                        i.SubTotal,
                        i.Tax,
                        i.Total,
                        i.Notes,
                        i.Status,
                        i.CreatedDate
                    })
                    .ToListAsync();

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/Invoices/search/{term}
        [HttpGet("search/{term}")]
        public async Task<ActionResult<IEnumerable<object>>> SearchInvoices(string term)
        {
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.Customer)
                    .Where(i => i.InvoiceNumber.Contains(term) || i.Customer.Name.Contains(term))
                    .OrderByDescending(i => i.CreatedDate)
                    .Select(i => new
                    {
                        i.Id,
                        i.InvoiceNumber,
                        i.CustomerId,
                        CustomerName = i.Customer.Name,
                        i.InvoiceDate,
                        i.DueDate,
                        i.SubTotal,
                        i.Tax,
                        i.Total,
                        i.Notes,
                        i.Status,
                        i.CreatedDate
                    })
                    .ToListAsync();

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(CreateInvoiceRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Verify customer exists
                var customer = await _context.Customers.FindAsync(request.CustomerId);
                if (customer == null || !customer.IsActive)
                {
                    return BadRequest("Cliente no encontrado o inactivo.");
                }

                // Generate invoice number
                var lastInvoice = await _context.Invoices
                    .OrderByDescending(i => i.Id)
                    .FirstOrDefaultAsync();
                var invoiceNumber = $"INV-{(lastInvoice?.Id + 1 ?? 1):D6}";

                // Create invoice
                var invoice = new Invoice
                {
                    InvoiceNumber = invoiceNumber,
                    CustomerId = request.CustomerId,
                    InvoiceDate = DateTime.Now,
                    DueDate = request.DueDate,
                    Notes = request.Notes,
                    Status = "Pending"
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                decimal subTotal = 0;
                
                // Create invoice details
                foreach (var detail in request.Details)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product == null || !product.IsActive)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Producto con ID {detail.ProductId} no encontrado o inactivo.");
                    }

                    if (product.Stock < detail.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Stock insuficiente para el producto {product.Name}. Stock disponible: {product.Stock}");
                    }

                    var detailSubTotal = detail.Quantity * detail.UnitPrice;
                    var discountAmount = detailSubTotal * (detail.Discount / 100);
                    var detailTotal = detailSubTotal - discountAmount;

                    var invoiceDetail = new InvoiceDetail
                    {
                        InvoiceId = invoice.Id,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        SubTotal = detailSubTotal,
                        Discount = detail.Discount,
                        Total = detailTotal
                    };

                    _context.InvoiceDetails.Add(invoiceDetail);
                    
                    // Update product stock
                    product.Stock -= detail.Quantity;
                    
                    subTotal += detailTotal;
                }

                // Calculate totals (assuming 18% tax)
                var tax = subTotal * 0.18m;
                var total = subTotal + tax;

                invoice.SubTotal = subTotal;
                invoice.Tax = tax;
                invoice.Total = total;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return invoice with includes
                var createdInvoice = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceDetails)
                        .ThenInclude(d => d.Product)
                    .FirstOrDefaultAsync(i => i.Id == invoice.Id);

                return CreatedAtAction("GetInvoice", new { id = invoice.Id }, createdInvoice);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // PUT: api/Invoices/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateInvoiceStatus(int id, [FromBody] string status)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            var validStatuses = new[] { "Pending", "Paid", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                return BadRequest("Estado de factura inválido. Estados válidos: Pending, Paid, Cancelled");
            }

            invoice.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.Id == id);
                
            if (invoice == null)
            {
                return NotFound();
            }

            if (invoice.Status == "Paid")
            {
                return BadRequest("No se puede eliminar una factura pagada.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Restore product stock
                foreach (var detail in invoice.InvoiceDetails)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.Stock += detail.Quantity;
                    }
                }

                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}