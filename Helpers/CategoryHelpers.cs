using DBProject_Shop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Helpers
{
    /// <summary>
    /// Helper class containing various methods
    /// with complete CRUD operations for categories 
    /// </summary>
    public static class CategoryHelpers
    {
        //-----------------
        // CREATE
        //-----------------

        /// <summary>
        /// Method for adding a category
        /// which also lets you add products to it.
        /// </summary>
        /// <returns></returns>
        public static async Task AddCategoryAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose a category name:");
            var catName = Console.ReadLine()?.Trim() ?? string.Empty;
            if (await db.Categories.AnyAsync(x => x.CategoryName == catName))
            {
                Console.WriteLine("Category name allready exists, type in a different name.");
                return;
            }

            var category = new Category { CategoryName = catName };
            db.Categories.Add(category);
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Category added!");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
            }            

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Would you like to add a product to your new category? (y/n)");
            var choice = Console.ReadLine()?.Trim() ?? string.Empty;    
            if (choice == "y")
            {
                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine($"Category \"{category.CategoryName}\" created, with category id: {category.CategoryId} ");                
                await ProductHelpers.AddProductAsync();
            }
            else if (choice == "n")
            {
                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine($"Category \"{category.CategoryName}\" created, with category id: {category.CategoryId} ");
            }           
        }

        //-----------------
        // READ
        //-----------------

        /// <summary>
        /// Method for listing out all categories and 
        /// all products linked to them.
        /// </summary>
        /// <returns></returns>
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

        //-----------------
        // UPDATE
        //-----------------

        /// <summary>
        /// Mehtod that handles editing of a category
        ///  - Editing the category name
        ///  - Adding products to the allready existing category
        /// </summary>
        /// <returns></returns>
        public static async Task EditCategoryAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine();
            Console.WriteLine("-----------");
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

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine($"Would you like to: Edit name (1) | Add a product (2) - To the \"{categoryToEdit.CategoryName}\" category?");
            var choice = Console.ReadLine()?.Trim() ?? string.Empty;
            
            if (choice == "1")
            {
                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Choose a new category name:");
                var newName = Console.ReadLine()?.Trim() ?? string.Empty;
                if (await db.Categories.AnyAsync(x => x.CategoryName == newName))
                {
                    Console.WriteLine("Category name allready exists, type in a different name.");
                    return;
                }

                categoryToEdit.CategoryName = newName;
                try
                {
                    await db.SaveChangesAsync();
                    Console.WriteLine();
                    Console.WriteLine("-----------");
                    Console.WriteLine($"Category name edited, new name is: {categoryToEdit.CategoryName}");
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
                }                
            }
            else if (choice == "2")
            {
                Console.WriteLine();
                Console.WriteLine("-----------");
                await ProductHelpers.AddProductAsync();
            }
            else
            {
                Console.WriteLine("Invalid input, please try again");
                return;
            }
        }

        //-----------------
        // DELETE
        //-----------------

        /// <summary>
        /// Method that handles deletion of a category
        /// by entering the coresponding ID#
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteCategoryAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose a category to delete (id#)");
            var catList = await db.Categories.AsNoTracking().ToListAsync();
            foreach (var cat in catList)
            {
                Console.WriteLine($"{cat.CategoryId} - {cat.CategoryName}");
            }

            if (!int.TryParse(Console.ReadLine(), out var cId))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }            
            
            var categoryToDelete = await db.Categories.FirstAsync(c => c.CategoryId == cId);            
            db.Categories.Remove(categoryToDelete);

            try
            {
                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine($"Category {categoryToDelete.CategoryName} deleted!");
                await db.SaveChangesAsync();                
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
            }
        }
    }
}
