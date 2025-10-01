using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FacturacionAPI.Data;

namespace FacturacionAPI.Controllers
{
    public class SalesReportDto
    {
        public decimal TotalSales { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public List<ProductSalesDto> TopProducts { get; set; } = new List<ProductSalesDto>();
        public List<MonthlySalesDto> MonthlySales { get; set; } = new List<MonthlySalesDto>();
    }

    public class ProductSalesDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlySalesDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int InvoiceCount { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly FacturacionDbContext _context;

        public ReportsController(FacturacionDbContext context)
        {
            _context = context;
        }

        // GET: api/Reports/sales
        [HttpGet("sales")]
        public async Task<ActionResult<SalesReportDto>> GetSalesReport([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-12);
            var end = endDate ?? DateTime.Now;

            var paidInvoices = _context.Invoices
                .Where(i => i.Status == "Paid" && i.InvoiceDate >= start && i.InvoiceDate <= end);

            var totalSales = await paidInvoices.SumAsync(i => i.Total);
            var totalInvoices = await paidInvoices.CountAsync();
            var totalProducts = await _context.Products.Where(p => p.IsActive).CountAsync();
            var totalCustomers = await _context.Customers.Where(c => c.IsActive).CountAsync();

            // Top selling products
            var topProducts = await _context.InvoiceDetails
                .Include(d => d.Product)
                .Include(d => d.Invoice)
                .Where(d => d.Invoice.Status == "Paid" && d.Invoice.InvoiceDate >= start && d.Invoice.InvoiceDate <= end)
                .GroupBy(d => new { d.ProductId, d.Product.Name })
                .Select(g => new ProductSalesDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    QuantitySold = g.Sum(d => d.Quantity),
                    TotalRevenue = g.Sum(d => d.Total)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(5)
                .ToListAsync();

            // Monthly sales
            var monthlySales = await paidInvoices
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .Select(g => new MonthlySalesDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    TotalSales = g.Sum(i => i.Total),
                    InvoiceCount = g.Count()
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToListAsync();

            var report = new SalesReportDto
            {
                TotalSales = totalSales,
                TotalInvoices = totalInvoices,
                TotalProducts = totalProducts,
                TotalCustomers = totalCustomers,
                TopProducts = topProducts,
                MonthlySales = monthlySales
            };

            return report;
        }

        // GET: api/Reports/products/low-stock
        [HttpGet("products/low-stock")]
        public async Task<ActionResult<IEnumerable<object>>> GetLowStockProducts([FromQuery] int threshold = 5)
        {
            var lowStockProducts = await _context.Products
                .Where(p => p.IsActive && p.Stock <= threshold)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Stock,
                    p.Category,
                    p.Price
                })
                .OrderBy(p => p.Stock)
                .ToListAsync();

            return lowStockProducts;
        }

        // GET: api/Reports/customers/top
        [HttpGet("customers/top")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopCustomers([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-12);
            var end = endDate ?? DateTime.Now;

            var topCustomers = await _context.Invoices
                .Include(i => i.Customer)
                .Where(i => i.Status == "Paid" && i.InvoiceDate >= start && i.InvoiceDate <= end)
                .GroupBy(i => new { i.CustomerId, i.Customer.Name, i.Customer.Email })
                .Select(g => new
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.Name,
                    CustomerEmail = g.Key.Email,
                    TotalPurchases = g.Sum(i => i.Total),
                    InvoiceCount = g.Count(),
                    AverageOrderValue = g.Average(i => i.Total)
                })
                .OrderByDescending(c => c.TotalPurchases)
                .Take(10)
                .ToListAsync();

            return topCustomers;
        }

        // GET: api/Reports/revenue/daily
        [HttpGet("revenue/daily")]
        public async Task<ActionResult<IEnumerable<object>>> GetDailyRevenue([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var dailyRevenue = await _context.Invoices
                .Where(i => i.Status == "Paid" && i.InvoiceDate >= start && i.InvoiceDate <= end)
                .GroupBy(i => i.InvoiceDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(i => i.Total),
                    InvoiceCount = g.Count(),
                    AverageOrderValue = g.Average(i => i.Total)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            return dailyRevenue;
        }
    }
}