using DBProject_Shop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;


namespace DBProject_Shop.Helpers
{
    public static class CustomerHelpers
    {
        // CREATE
        public static async Task AddCustomerAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine("Please fill in the following details:");
            Console.WriteLine();
            Console.WriteLine("Name: ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name) || name.Length > 100)
            {
                Console.WriteLine("Name is required! (Max lenght 100)");
                return;
            }

            Console.WriteLine("Email:");
            var email = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(email) || email.Length > 200)
            {
                Console.WriteLine("Email is required! (Max lenght 200)");
                return;
            }
            if (await db.Customers.AnyAsync(x => x.CustomerEmail == email))
            {
                Console.WriteLine("Email allready exists, type in a different email adress.");
                return;
            }

            Console.WriteLine("Address (street name and number):");
            var address = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(address) || address.Length > 100)
            {
                Console.WriteLine("Address is required! (Max lenght 100)");
                return;
            }

            db.Customers.Add(new Customer 
            { 
                CustomerName = name, 
                CustomerEmail = email, 
                CustomerAddress = address 
            });

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Customer added!");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
            }
        }

        // READ
        public static async Task ListCustomersAsync()
        {
            using var db = new ShopContext();

            var customers = await db.Customers.AsNoTracking()
                                              .OrderBy(x => x.CustomerId)
                                              .ToListAsync();
            Console.WriteLine("Customer Id | Name | Email | City");
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerId} | {customer.CustomerName} | {customer.CustomerEmail} | {customer.CustomerAddress}");
            }
        }

        public static async Task EditCustomerAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine("Please choose a customer to edit (id#):");
            await ListCustomersAsync();
            if (!int.TryParse(Console.ReadLine(), out var customerId))
            {
                Console.WriteLine("Invalid input, use numbers");
                return;
            }
            if (!await db.Customers.AnyAsync(c => c.CustomerId == customerId))
            {
                Console.WriteLine("Customer not found!");
                return;
            }
            var customerToEdit = await db.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);

            Console.WriteLine("Please choose an option to edit. Type in: Name | Email | Address");
            var choice = Console.ReadLine();
            if (choice == "Name")
            {
                Console.WriteLine($"Current name: {customerToEdit.CustomerName} ");
                Console.WriteLine("Please type in the new name");
                var newName = Console.ReadLine()?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(newName))
                {
                    customerToEdit.CustomerName = newName;
                }
            }
            else if (choice == "Email")
            {
                Console.WriteLine($"Current email: {customerToEdit.CustomerEmail}");
                Console.WriteLine("Please type in the new email");
                var newEmail = Console.ReadLine()?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(newEmail))
                {
                    customerToEdit.CustomerEmail = newEmail;
                }
            }
            else if (choice == "Address")
            {
                Console.WriteLine($"Current Address: {customerToEdit.CustomerAddress}");
                Console.WriteLine("Please type in the new address");
                var newCity = Console.ReadLine()?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(newCity))
                {
                    customerToEdit.CustomerAddress = newCity;
                }
            }
            else if (choice != "Address" && choice != "Email" && choice != "Name")
            {
                Console.WriteLine("Please type in the correct command");
                return;
            }

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine($"Customer Id: {customerToEdit.CustomerId} has been edited!");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
                throw;
            }
        }

        public static async Task DeleteCustomerAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine("Please choose a customer to delete (id#)");
            await ListCustomersAsync();

            if (!int.TryParse(Console.ReadLine(), out var customerId))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }

            if (!await db.Customers.AnyAsync(c => c.CustomerId == customerId))
            {
                Console.WriteLine("Customer not found!");
                return;
            }
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);

            db.Customers.Remove(customer);
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine($"Customer \"{customer.CustomerName}\" removed from the database");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
            }
        }
    }
}
