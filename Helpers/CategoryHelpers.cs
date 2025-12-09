using DBProject_Shop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Helpers
{
    public static class CategoryHelpers
    {
        // CREATE
        public static async Task AddCategoryAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine("Please choose a category name:");
            var catName = Console.ReadLine()?.Trim() ?? string.Empty;

            var category = new Category { CategoryName = catName };

            db.Categories.Add(category);
            await db.SaveChangesAsync();
                        
            Console.WriteLine("Would you like to add a product to your new category? (y/n)");
            var choice = Console.ReadLine()?.Trim() ?? string.Empty;    
            if (choice == "y")
            {
                Console.WriteLine($"Category \"{category.CategoryName}\" created, with category id: {category.CategoryId} ");
                Console.WriteLine();
                await ProductHelpers.AddProductAsync();
            }
            else if (choice == "n")
            {
                Console.WriteLine();
                Console.WriteLine($"Category \"{category.CategoryName}\" created, with category id: {category.CategoryId} ");
            }           
        }

        // READ
        public static async Task ListCategoriesAsync()
        {
            using var db = new ShopContext();
            var categories = await db.Categories.Include(c => c.ProductsList)                                                                
                                                .ToListAsync();                                                
            
            if (!await db.Categories.AnyAsync())
            {
                Console.WriteLine("No categories found");
                return;
            }

            Console.WriteLine("Category Id | Category Name");
            Console.WriteLine();
            foreach (var category in categories)
            {
                Console.WriteLine($"{category.CategoryId} | {category.CategoryName}");
                Console.WriteLine("With products:");
                Console.WriteLine("--------------");
                    foreach(var product in category.ProductsList)
                    {
                        Console.WriteLine(product.ProductName);
                    }
                Console.WriteLine();
            }
        }

        // UPDATE
        public static async Task EditCategoryAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine("Choose a category to edit (id)");
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
            var categoryToEdit = await db.Categories.FirstAsync(c => c.CategoryId == catId);

            Console.WriteLine($"Would you like to: Edit name (1) | Add a product (2) - To the \"{categoryToEdit.CategoryName}\" category?");
            var choice = Console.ReadLine()?.Trim() ?? string.Empty;
            
            if (choice == "1")
            {
                Console.WriteLine("Choose a new category name:");
                var newName = Console.ReadLine()?.Trim() ?? string.Empty;
                
                categoryToEdit.CategoryName = newName;                
                await db.SaveChangesAsync();

                Console.WriteLine($"Category name edited, new name is: {categoryToEdit.CategoryName}");
            }
            else if (choice == "2")
            {
                await ProductHelpers.AddProductAsync();
            }
            else
            {
                Console.WriteLine("Invalid input, please try again");
                return;
            }
        }

        // DELETE
        public static async Task DeleteCategoryAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine("Please choose a category to delete (id#)");
            if (!int.TryParse(Console.ReadLine(), out var cId))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }
            var categoryToDelete = await db.Categories.FirstAsync(c => c.CategoryId == cId);

            Console.WriteLine($"Category {categoryToDelete.CategoryName} deleted!");
            db.Categories.Remove(categoryToDelete);            
            await db.SaveChangesAsync();            
        }
    }
}
