using Microsoft.EntityFrameworkCore;
using DBProject_Shop.Data;
using DBProject_Shop.Models;
using System.Runtime.CompilerServices;
using DBProject_Shop.Helpers;


Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory, "shop.db"));
//Console.WriteLine("Starting database seeding/migrations...");
//var seedTask = SeedingData.SeedAsync();
//var timeout = Task.Delay(TimeSpan.FromSeconds(20));
//var completed = await Task.WhenAny(seedTask, timeout);
//if (completed == timeout)
//{
//    Console.WriteLine("Seeding/migration did not complete within 20s. You can continue, but investigate seeding.");
//}
//else
//{
//    // re-await to propagate exceptions if seedTask faulted
//    await seedTask;
//    Console.WriteLine("Seeding/migration finished.");
//}
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
            await CustomerMenu();
            break;

        case "2":

            break;

        case "3":

            break;
        
        case "4":

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
static async Task CustomerMenu()
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
static async Task OrderMenu()
{
    while (true)
    {
        Console.WriteLine("\nCommands: Add Order (1) | List Orders (2) | Edit Order (3) | Delete Order (4) | Exit (5) ");
        Console.WriteLine(">");
        Console.WriteLine("Please choose a command");
        var orderCommand = Console.ReadLine()?.Trim();
        if (orderCommand == null)
        {
            Console.WriteLine("Invalid input");
        }
        switch (orderCommand)
        {
            case "1":
                break;

            case "2":
                break;

            case "3":
                break;

            case "4":
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
// PRODUCT MENU
//-----------------
static async Task ProductMenu()
{
    while (true)
    {
        Console.WriteLine("\nCommands: Add Product (1) | List Product (2) | Edit Product (3) | Delete Product (4) | Exit (5) ");
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
                break;

            case "2":
                break;

            case "3":
                break;

            case "4":
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
// CATEGORY MENU
//-----------------
static async Task CategoryMenu()
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
                break;

            case "2":
                break;

            case "3":
                break;

            case "4":
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