using DBProject_Shop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace DBProject_Shop.Helpers
{
    /// <summary>
    /// Helper class containing various methods
    /// with complete CRUD operations for products 
    /// </summary>
    public static class ProductHelpers
    {
        //-----------------
        // CREATE
        //-----------------
        public static async Task AddProductAsync() 
        {
            using var db = new ShopContext();
            var choice = "";
            while (choice != "n")
            {

                Console.WriteLine();
                Console.WriteLine("Enter product name: ");
                var prodName = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(prodName))
                {
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine("Enter product price: ");
                if (!decimal.TryParse(Console.ReadLine(), out var prodPrice))
                {
                    Console.WriteLine("Invalid input, please use numbers");
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine("Enter initial stock quantity");
                if (!int.TryParse(Console.ReadLine(), out var qty))
                {
                    Console.WriteLine("Invalid input, please use numbers");
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine("Please input the category id you wish to add the product to");
                Console.WriteLine();
                var categories = await db.Categories.AsNoTracking().ToListAsync();
                foreach (var category in categories)
                {
                    Console.WriteLine($"{category.CategoryId} - {category.CategoryName}");
                }
                
                if (!int.TryParse(Console.ReadLine(), out var catId))
                {
                    Console.WriteLine("Invalid input, please use numbers");
                    return;
                }
                if (!await db.Categories.AnyAsync(c => c.CategoryId == catId))
                {
                    Console.WriteLine("Category not found, please try again");
                    return;
                }
                
                var product = new Product
                {
                    ProductName = prodName,
                    ProductPrice = prodPrice,
                    StockQuantity = qty,
                    CategoryId = catId
                };
                db.Products.Add(product);
                try
                {
                    await db.SaveChangesAsync();
                    Console.WriteLine($"Product added!");
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
                }

                Console.WriteLine("Would you like to add another product? (y/n)");
                choice = Console.ReadLine()?.Trim() ?? string.Empty;
                if (choice == "y")
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        //-----------------
        // READ
        //-----------------
        public static async Task ListProductsAsync()
        {
            using var db = new ShopContext();

            var products = await db.Products.AsNoTracking()
                                            .OrderBy(x => x.CategoryId)
                                            .ToListAsync();

            if (!await db.Products.AnyAsync())
            {
                Console.WriteLine("No products found!");
                return;
            }

            Console.WriteLine("Listing all products (Sorted by category)");
            Console.WriteLine("Product Id | Product name | Stock quantity | Product price");
            foreach (var prod in products)
            {
                Console.WriteLine($"{prod.ProductId} | {prod.ProductName} | {prod.StockQuantity} | {prod.ProductPrice}");
            }
        }

        public static async Task FindProductsByCategory()
        {
            using var db = new ShopContext();

            Console.WriteLine("Please choose a category to list its products (id#)");
            var categories = await db.Categories.AsNoTracking().ToListAsync();
            foreach (var category in categories)
            {
                Console.WriteLine($"{category.CategoryId} - {category.CategoryName}");
            }

            if (!int.TryParse(Console.ReadLine(), out var cId))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }
            var catName = await db.Categories.FirstAsync(c => c.CategoryId == cId);
            var prods = await db.Products.Where(c => c.CategoryId == cId).ToListAsync();

            Console.WriteLine($"Products in the \"{catName.CategoryName}\" category");
            Console.WriteLine("Product Id | Product name | Price | Stock quantity");
            foreach (var prod in prods)
            {
                Console.WriteLine($"{prod.ProductId} | {prod.ProductName} | {prod.ProductPrice} | {prod.StockQuantity}");
            }

        }

        //-----------------
        // UPDATE
        //-----------------
        public static async Task EditProductAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine("Please choose the product you wish to edit (id#)");
            await ListProductsAsync();

            if (!int.TryParse(Console.ReadLine(), out var pId))
            {
                Console.WriteLine("Invalid input, please use numbers");
            }
            var prodToEdit = await db.Products.FirstAsync(p => p.ProductId == pId);
            if (!await db.Products.AnyAsync(p => p.ProductId == pId))
            {
                Console.WriteLine("No such product found!");
            }

            Console.WriteLine("Choose what you want to edit:");
            Console.WriteLine("Name (1) | Price (2) | Stock quantity (3)");
            var choice = Console.ReadLine() ?? string.Empty;
            if (choice != "1" && choice != "2" && choice != "3")
            {
                Console.WriteLine("Invalid input, please try again");
                return;
            }

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Please choose a new product name:");
                    var newName = Console.ReadLine()?.Trim() ?? string.Empty;
                    prodToEdit.ProductName = newName;

                    await db.SaveChangesAsync();
                    Console.WriteLine($"Product name edited, new product name is now: {prodToEdit.ProductName}");
                    break;

                case "2":
                    Console.WriteLine("Please choose a new product price:");
                    if (!decimal.TryParse(Console.ReadLine(), out var newPrice))
                    {
                        Console.WriteLine("Invalid input, please try again");
                        return;
                    }
                    prodToEdit.ProductPrice = newPrice;

                    await db.SaveChangesAsync();
                    Console.WriteLine($"Product price edited, new product price is now: {prodToEdit.ProductPrice}");
                    break;

                case "3":
                    Console.WriteLine("Please choose a new stock quantity:");
                    if (!int.TryParse(Console.ReadLine(), out var newQty))
                    {
                        Console.WriteLine("Invalid input, please try again");
                        return;
                    }
                    if (newQty < 0)
                    {
                        Console.WriteLine("Stock quantity can't be a negative number, please try again");
                        return;
                    }
                    prodToEdit.StockQuantity = newQty;

                    await db.SaveChangesAsync();
                    Console.WriteLine($"Stock quantity edited, new quantity is: {prodToEdit.StockQuantity}");
                        break;
                default:
                    break;
            }
        }

        //-----------------
        // DELETE
        //-----------------
        public static async Task DeleteProductAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine("Please select a product to delete (id#)");
            if (!int.TryParse(Console.ReadLine(), out var pId))
            {
                Console.WriteLine("Invalid input, please try again");
                return;
            }

            var prodToDelete = await db.Products.FirstAsync(p => p.ProductId == pId);

            db.Products.Remove(prodToDelete);
            await db.SaveChangesAsync();
            Console.WriteLine($"Product {prodToDelete.ProductName} deleted!");
        }
    }
}
