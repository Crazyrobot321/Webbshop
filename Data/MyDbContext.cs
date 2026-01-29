using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Webbshop.Entities;
using Microsoft.Extensions.Configuration;

namespace Webbshop.Data
{
    internal class MyDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connString = config.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CustomersConstraints(modelBuilder);
            ProductsConstraints(modelBuilder);
            CategoriesConstraints(modelBuilder);
            SuppliersConstraints(modelBuilder);
            OrdersConstraints(modelBuilder);
            OrderItemsConstraints(modelBuilder);
        }
        private static void CustomersConstraints(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                            .Property(c => c.Email)
                                .IsRequired()
                                .HasMaxLength(150);
            modelBuilder.Entity<Customer>()
                    .HasIndex(c => c.Email)
                    .IsUnique();

            modelBuilder.Entity<Customer>()
                .Property(c => c.MobileNr)
                    .IsRequired()
                    .HasMaxLength(20);

            modelBuilder.Entity<Customer>()
                    .HasIndex(c => c.MobileNr)
                    .IsUnique();

            modelBuilder.Entity<Customer>()
                .Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Street)
                    .IsRequired()
                    .HasMaxLength(200);

            modelBuilder.Entity<Customer>()
                .Property(c => c.City)
                    .IsRequired()
                    .HasMaxLength(100);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Country)
                    .IsRequired()
                    .HasMaxLength(100);

            modelBuilder.Entity<Customer>()
                .Property(c => c.DateOfBirth)
                    .IsRequired()
                    .HasColumnType("date");
        }
        private static void ProductsConstraints(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Product>()
                 .Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(75);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                    .IsRequired();
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                    .WithMany(c => c.ProductsSupplied)
                .HasForeignKey(p => p.SupplierId)
                    .IsRequired();
        }
        private static void CategoriesConstraints(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);
        }
        private static void SuppliersConstraints(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>()
                .Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(150);
            modelBuilder.Entity<Supplier>()
                .Property(s => s.ContactEmail)
                    .HasMaxLength(150);
            modelBuilder.Entity<Supplier>()
                .Property(s => s.ContactPhone)
                    .HasMaxLength(20);
            modelBuilder.Entity<Supplier>()
                .Property(s => s.Street)
                    .IsRequired()
                    .HasMaxLength(200);
            modelBuilder.Entity<Supplier>()
                .Property(s => s.City)
                    .IsRequired()
                    .HasMaxLength(100);
            modelBuilder.Entity<Supplier>()
                .Property(s => s.Country)
                    .IsRequired()
                    .HasMaxLength(100);
        }
        private static void OrdersConstraints(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().Property(o => o.OrderDate).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.PaymentMethod).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.DeliveryOption).IsRequired();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                    .WithMany()
                .HasForeignKey(o => o.CustomerId)
                    .IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.LastFourDigits).HasMaxLength(4);
            
            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingStreet)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingCity)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingCountry)
                .IsRequired()
                .HasMaxLength(100);
        }
        private static void OrderItemsConstraints(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();
        }
    }
}
