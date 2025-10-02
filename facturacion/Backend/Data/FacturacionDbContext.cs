using Microsoft.EntityFrameworkCore;
using FacturacionAPI.Models;

namespace FacturacionAPI.Data
{
    public class FacturacionDbContext : DbContext
    {
        public FacturacionDbContext(DbContextOptions<FacturacionDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.Name);
            });

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.DocumentNumber);
            });

            // Invoice configuration
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Tax).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Invoices)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // InvoiceDetail configuration
            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Invoice)
                    .WithMany(i => i.InvoiceDetails)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.InvoiceDetails)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop HP", Description = "Laptop HP Pavilion 15\"", Price = 599.99m, Stock = 10, Category = "Electrónicos" },
                new Product { Id = 2, Name = "Mouse Logitech", Description = "Mouse inalámbrico Logitech", Price = 29.99m, Stock = 25, Category = "Accesorios" },
                new Product { Id = 3, Name = "Teclado Mecánico", Description = "Teclado mecánico RGB", Price = 89.99m, Stock = 15, Category = "Accesorios" },
                new Product { Id = 4, Name = "Monitor Samsung", Description = "Monitor Samsung 24\" Full HD", Price = 199.99m, Stock = 8, Category = "Electrónicos" },
                new Product { Id = 5, Name = "Impresora Canon", Description = "Impresora multifuncional Canon", Price = 149.99m, Stock = 12, Category = "Oficina" }
            );

            // Seed Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "Juan Pérez", Email = "juan.perez@email.com", Phone = "555-0001", Address = "Calle 123, Ciudad", DocumentNumber = "12345678", DocumentType = "DNI" },
                new Customer { Id = 2, Name = "María García", Email = "maria.garcia@email.com", Phone = "555-0002", Address = "Avenida 456, Ciudad", DocumentNumber = "87654321", DocumentType = "DNI" },
                new Customer { Id = 3, Name = "Carlos López", Email = "carlos.lopez@email.com", Phone = "555-0003", Address = "Plaza 789, Ciudad", DocumentNumber = "11223344", DocumentType = "DNI" }
            );
        }
    }
}