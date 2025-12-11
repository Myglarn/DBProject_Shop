using DBProject_Shop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop
{
    public class ShopContext :DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderRow> OrderRows => Set<OrderRow>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<OrderSummary> OrderSummaries => Set<OrderSummary>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "shop.db");
            optionsBuilder.UseSqlite($"Filename = {dbPath}");            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderSummary>(e =>
            {
                e.HasNoKey();

                e.ToView("OrderSummary");
            });

            modelBuilder.Entity<Customer>(c =>
            {
                c.HasKey(c => c.CustomerId);

                c.Property(c => c.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100);

                c.Property(c => c.CustomerEmail)
                    .IsRequired()
                    .HasMaxLength(100);

                c.Property(c => c.CustomerAddress)
                    .IsRequired()
                    .HasMaxLength(100);

                c.Property(c => c.PhoneNumber);

                c.Property(c => c.Password).IsRequired();                

                c.HasIndex(c => c.CustomerEmail).IsUnique();

                c.HasMany(c => c.OrdersList);
            });

            modelBuilder.Entity<Order>(o =>
            {
                o.HasKey(o => o.OrderId);

                o.Property(o => o.OrderDate).IsRequired();

                o.Property(o => o.Status).IsRequired();

                o.Property(o => o.TotalAmount).IsRequired();                

                o.HasIndex(o => o.CustomerId);                

                o.HasOne(o => o.Customer)
                    .WithMany(o => o.OrdersList)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderRow>(r =>
            {
                r.HasKey(o => o.OrderRowId);

                r.Property(r => r.Quantity).IsRequired();

                r.Property(r => r.UnitPrice).IsRequired();

                r.HasOne(r => r.Order)
                    .WithMany(r => r.OrderRowsList)
                    .HasForeignKey(r => r.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                r.HasOne(r => r.Product)
                    .WithMany(r => r.OrderRowsList)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(p =>
            {
                p.HasKey(p => p.ProductId);

                p.Property(p => p.StockQuantity).IsRequired();

                p.Property(p => p.ProductPrice).IsRequired();

                p.Property(p => p.ProductName)
                    .IsRequired()
                    .HasMaxLength(150);

                p.HasIndex(p => p.ProductName).IsUnique();

                p.HasOne(p => p.Category)
                    .WithMany(p => p.ProductsList)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);                
            });

            modelBuilder.Entity<Category>(c =>
            {
                c.HasKey(c => c.CategoryId);

                c.Property(c => c.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);

                c.HasMany(c => c.ProductsList).WithOne(c => c.Category);

                c.HasIndex(c => c.CategoryName).IsUnique();
            });
        }
    }
}
