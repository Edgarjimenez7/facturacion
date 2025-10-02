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
        }
    }
}