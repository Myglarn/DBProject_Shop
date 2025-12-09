using Microsoft.EntityFrameworkCore;
using DBProject_Shop.Data;
using DBProject_Shop.Models;
using System.Runtime.CompilerServices;
using DBProject_Shop.Helpers;


Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory, "shop.db"));
await SeedingData.SeedAsync();


while (true)
{
    Console.WriteLine("\nCommands: Customers (1) | Orders (2) | Products (3) | Categories (4) | Exit (5)");
    Console.WriteLine(">");
    Console.WriteLine("Please choose a command");
    var choice = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(choice))
    {
        continue;
    }
    if (choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice != "5")
    {
        Console.WriteLine("You need to choose either 1, 2, 3, 4 or 5");
        continue;
    }    

    switch (choice)
    {
        case "1":
            await CustomerMenuAsync();
            break;

        case "2":
            await OrderMenuAsync();

            break;

        case "3":
            await ProductMenuAsync();
            break;
        
        case "4":
            await CategoryMenuAsync();
            break;

        case "5":
            Console.WriteLine("Exiting application...");
            return;

        default:
            Console.WriteLine("Invalid input");
            return;
    }
}

//-----------------
// CUSTOMER MENU
//-----------------
static async Task CustomerMenuAsync()
{
    while (true)
    {
        Console.WriteLine("\nCommands: Add Customer (1) | List Customers (2) | Edit Customer (3) | Delete Customer (4) | Exit (5) ");
        Console.WriteLine(">");
        Console.WriteLine("Please choose a command");
        var customerCommand = Console.ReadLine()?.Trim();
        if (customerCommand == null)
        {
            Console.WriteLine("Invalid input");
        }
        switch (customerCommand)
        {
            case "1":
                await CustomerHelpers.AddCustomerAsync();
                break;

            case "2":
                await CustomerHelpers.ListCustomersAsync();
                break;

            case "3":
                await CustomerHelpers.EditCustomerAsync();
                break; 
            
            case "4":
                await CustomerHelpers.DeleteCustomerAsync();
                break;

            case "5":
                Console.WriteLine("Returning to main menu...");
                return;

            default:
                Console.WriteLine("Invalid input");
                break;
        }
    }
}

//-----------------
// ORDER MENU
//-----------------
static async Task OrderMenuAsync()
{
    while (true)
    {
        Console.WriteLine("\nCommands: Add Order (1) | List Orders (2) | List Orders paged (3) | Find order by customer (4) | Delete Order (5) | Exit (6) ");
        Console.WriteLine(">");
        Console.WriteLine("Please choose a command");
        var orderCommand = Console.ReadLine()?.Trim() ?? string.Empty;
        if (orderCommand == null)
        {
            Console.WriteLine("Invalid input, please try again");
        }
        switch (orderCommand)
        {
            case "1":
                await OrderHelpers.AddOrderAsync();
                break;

            case "2":
                await OrderHelpers.ListOrdersAsync();
                break;

            case "3":
                await OrderHelpers.OrdersPagedAsync();
                break;

            case "4":
                await OrderHelpers.ListCustomerOrdersAsync();
                break;

            case "5":
                await OrderHelpers.DeleteOrderAsync();
                break;

            case "6":
                Console.WriteLine("Returning to main menu...");
                return;

            default:
                Console.WriteLine("Invalid input, please try again");
                break;
        }
    }
}

//-----------------
// PRODUCT MENU
//-----------------
static async Task ProductMenuAsync()
{
    while (true)
    {
        Console.WriteLine("\nCommands: Add Product (1) | List Product (2) | Edit Product (3) | Find products by category (4) | Delete Product (5) | Exit (6) ");
        Console.WriteLine(">");
        Console.WriteLine("Please choose a command");
        var prodCommand = Console.ReadLine()?.Trim();
        if (prodCommand == null)
        {
            Console.WriteLine("Invalid input");
        }
        switch (prodCommand)
        {
            case "1":
                await ProductHelpers.AddProductAsync();
                break;

            case "2":
                await ProductHelpers.ListProductsAsync();
                break;

            case "3":
                await ProductHelpers.EditProductAsync();
                break;

            case "4":
                await ProductHelpers.FindProductsByCategory();
                break;
            case "5":
                await ProductHelpers.DeleteProductAsync();
                break;

            case "6":
                Console.WriteLine("Returning to main menu...");
                return;

            default:
                Console.WriteLine("Invalid input");
                break;
        }
    }
}

//-----------------
// CATEGORY MENU
//-----------------
static async Task CategoryMenuAsync()
{
    while (true)
    {
        Console.WriteLine("\nCommands: Add Category (1) | List Categories (2) | Edit Category (3) | Delete Category (4) | Exit (5) ");
        Console.WriteLine(">");
        Console.WriteLine("Please choose a command");
        var catCommand = Console.ReadLine()?.Trim();
        if (catCommand == null)
        {
            Console.WriteLine("Invalid input");
        }
        switch (catCommand)
        {
            case "1":
                await CategoryHelpers.AddCategoryAsync();
                break;

            case "2":
                await CategoryHelpers.ListCategoriesAsync();
                break;

            case "3":
                await CategoryHelpers.EditCategoryAsync();
                break;

            case "4":
                await CategoryHelpers.DeleteCategoryAsync();
                break;

            case "5":
                Console.WriteLine("Returning to main menu...");
                return;

            default:
                Console.WriteLine("Invalid input");
                break;
        }
    }
}