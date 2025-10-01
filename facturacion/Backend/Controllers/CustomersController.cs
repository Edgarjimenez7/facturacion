using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly FacturacionDbContext _context;

        public CustomersController(FacturacionDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null || !customer.IsActive)
            {
                return NotFound();
            }

            return customer;
        }

        // GET: api/Customers/search/{term}
        [HttpGet("search/{term}")]
        public async Task<ActionResult<IEnumerable<Customer>>> SearchCustomers(string term)
        {
            var customers = await _context.Customers
                .Where(c => c.IsActive && (c.Name.Contains(term) || c.Email.Contains(term) || c.DocumentNumber.Contains(term)))
                .OrderBy(c => c.Name)
                .ToListAsync();

            return customers;
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            // Check if email already exists
            if (await _context.Customers.AnyAsync(c => c.Email == customer.Email && c.IsActive))
            {
                return BadRequest("Ya existe un cliente con este email.");
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Check if customer has invoices
            if (await _context.Invoices.AnyAsync(i => i.CustomerId == id))
            {
                return BadRequest("No se puede eliminar un cliente que tiene facturas asociadas.");
            }

            // Soft delete
            customer.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}