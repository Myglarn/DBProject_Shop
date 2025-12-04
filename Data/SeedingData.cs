using Microsoft.EntityFrameworkCore;
using DBProject_Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Data
{
    public static class SeedingData
    {
        public static async Task SeedAsync()
        {
            using var db = new ShopContext();

            await db.Database.MigrateAsync();

            if (!await db.Customers.AnyAsync())
            {
                db.Customers.AddRange(
                    new Customer { CustomerName = "Christopher Petti", CustomerAddress = "Sicklingsvägen 7", CustomerEmail = "christopher.petti@gmail.com", PhoneNumber = 0707799793},
                    new Customer { CustomerName = "Arber Mulolli", CustomerAddress = "Programmerargatan 27", CustomerEmail = "Abbe@hotmail.com", PhoneNumber = 0765554575}
                    );
                await db.SaveChangesAsync();
                Console.WriteLine("Seeded db with customers");
            }

            if (!await db.Categories.AnyAsync())
            {
                db.Categories.AddRange(
                    new Category { CategoryName = "Rods"},
                    new Category { CategoryName = "Reels"},
                    new Category { CategoryName = "Lures"},
                    new Category { CategoryName = "Fishing Clothes"}
                    );
                await db.SaveChangesAsync();
                Console.WriteLine("Seeded db with Categories");
            }

            if (!await db.Products.AnyAsync())
            {
                var rod = await db.Categories.FirstAsync(c => c.CategoryName == "Rods");
                var reels = await db.Categories.FirstAsync(c => c.CategoryName == "Reels");
                var lures = await db.Categories.FirstAsync(c => c.CategoryName == "Lures");
                var clothes = await db.Categories.FirstAsync(c => c.CategoryName == "Fishing Clothes");

                db.Products.AddRange(
                    new Product { ProductName = "Perch rod", ProductPrice = 2500m, StockQuantity = 50, CategoryId = rod.CategoryId },
                    new Product { ProductName = "Pike rod", ProductPrice = 3000m, StockQuantity = 40, CategoryId = rod.CategoryId },
                    new Product { ProductName = "Perch reel", ProductPrice = 1500m, StockQuantity = 45, CategoryId = reels.CategoryId },
                    new Product { ProductName = "Pike reel", ProductPrice = 1800m, StockQuantity = 35, CategoryId = reels.CategoryId },
                    new Product { ProductName = "Wobbler - 15g", ProductPrice = 60m, StockQuantity = 150, CategoryId = lures.CategoryId },
                    new Product { ProductName = "Spinner - 20g", ProductPrice = 55m, StockQuantity = 126, CategoryId = lures.CategoryId },
                    new Product { ProductName = "Jigg - 100g", ProductPrice = 230m, StockQuantity = 90, CategoryId = lures.CategoryId },
                    new Product { ProductName = "Fishing hat", ProductPrice = 500m, StockQuantity = 250, CategoryId = clothes.CategoryId },
                    new Product { ProductName = "Fishing jacket", ProductPrice = 3000m, StockQuantity = 190, CategoryId = clothes.CategoryId }
                    );
                await db.SaveChangesAsync();
                Console.WriteLine("Seeded db with products");
            }
        }
    }
}
