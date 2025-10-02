using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Data
{
    public static class SeedData
    {
        public static async Task Initialize(FacturacionDbContext context)
        {
            // Create database if it doesn't exist
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (context.Products.Any())
            {
                return; // Data already seeded
            }

            // Add sample products
            var products = new[]
            {
                new Product
                {
                    Name = "Laptop HP",
                    Description = "Laptop HP Pavilion 15\"",
                    Price = 599.99m,
                    Stock = 10,
                    Category = "Electrónicos",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Name = "Mouse Logitech",
                    Description = "Mouse inalámbrico Logitech",
                    Price = 29.99m,
                    Stock = 25,
                    Category = "Accesorios",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Name = "Teclado Mecánico",
                    Description = "Teclado mecánico RGB",
                    Price = 89.99m,
                    Stock = 15,
                    Category = "Accesorios",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Name = "Monitor Samsung",
                    Description = "Monitor Samsung 24\" Full HD",
                    Price = 199.99m,
                    Stock = 8,
                    Category = "Electrónicos",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Name = "Impresora Canon",
                    Description = "Impresora multifuncional Canon",
                    Price = 149.99m,
                    Stock = 12,
                    Category = "Oficina",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            };

            context.Products.AddRange(products);

            // Add sample customers
            var customers = new[]
            {
                new Customer
                {
                    Name = "Juan Pérez",
                    Email = "juan.perez@email.com",
                    Phone = "555-0001",
                    Address = "Calle 123, Ciudad",
                    DocumentNumber = "12345678",
                    DocumentType = "DNI",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Customer
                {
                    Name = "María García",
                    Email = "maria.garcia@email.com",
                    Phone = "555-0002",
                    Address = "Avenida 456, Ciudad",
                    DocumentNumber = "87654321",
                    DocumentType = "DNI",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Customer
                {
                    Name = "Carlos López",
                    Email = "carlos.lopez@email.com",
                    Phone = "555-0003",
                    Address = "Plaza 789, Ciudad",
                    DocumentNumber = "11223344",
                    DocumentType = "DNI",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            };

            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();

            // Add sample invoices
            var invoices = new[]
            {
                new Invoice
                {
                    InvoiceNumber = "INV-001",
                    CustomerId = 1, // Juan Pérez
                    InvoiceDate = DateTime.Now.AddDays(-10),
                    DueDate = DateTime.Now.AddDays(20),
                    SubTotal = 629.98m,
                    Tax = 113.40m,
                    Total = 743.38m,
                    Notes = "Factura de prueba 1",
                    Status = "Paid",
                    CreatedDate = DateTime.Now.AddDays(-10)
                },
                new Invoice
                {
                    InvoiceNumber = "INV-002",
                    CustomerId = 2, // María García
                    InvoiceDate = DateTime.Now.AddDays(-5),
                    DueDate = DateTime.Now.AddDays(25),
                    SubTotal = 89.99m,
                    Tax = 16.20m,
                    Total = 106.19m,
                    Notes = "Factura de prueba 2",
                    Status = "Pending",
                    CreatedDate = DateTime.Now.AddDays(-5)
                },
                new Invoice
                {
                    InvoiceNumber = "INV-003",
                    CustomerId = 3, // Carlos López
                    InvoiceDate = DateTime.Now.AddDays(-2),
                    DueDate = DateTime.Now.AddDays(28),
                    SubTotal = 199.99m,
                    Tax = 36.00m,
                    Total = 235.99m,
                    Notes = "Factura de prueba 3",
                    Status = "Pending",
                    CreatedDate = DateTime.Now.AddDays(-2)
                }
            };

            context.Invoices.AddRange(invoices);
            await context.SaveChangesAsync();

            // Add sample invoice details
            var invoiceDetails = new[]
            {
                // Detalles para INV-001
                new InvoiceDetail
                {
                    InvoiceId = 1,
                    ProductId = 1, // Laptop HP
                    Quantity = 1,
                    UnitPrice = 599.99m,
                    Discount = 0,
                    Total = 599.99m
                },
                new InvoiceDetail
                {
                    InvoiceId = 1,
                    ProductId = 2, // Mouse Logitech
                    Quantity = 1,
                    UnitPrice = 29.99m,
                    Discount = 0,
                    Total = 29.99m
                },
                // Detalles para INV-002
                new InvoiceDetail
                {
                    InvoiceId = 2,
                    ProductId = 3, // Teclado Mecánico
                    Quantity = 1,
                    UnitPrice = 89.99m,
                    Discount = 0,
                    Total = 89.99m
                },
                // Detalles para INV-003
                new InvoiceDetail
                {
                    InvoiceId = 3,
                    ProductId = 4, // Monitor Samsung
                    Quantity = 1,
                    UnitPrice = 199.99m,
                    Discount = 0,
                    Total = 199.99m
                }
            };

            context.InvoiceDetails.AddRange(invoiceDetails);
            await context.SaveChangesAsync();
        }
    }
}