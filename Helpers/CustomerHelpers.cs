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
    /// <summary>
    /// Helper class containing various methods
    /// with complete CRUD operations for Customers 
    /// </summary>
    public static class CustomerHelpers
    {
        //-----------------
        // CREATE
        //-----------------

        /// <summary>
        /// Method for adding a customer to the database.
        /// Also encrypting sensitive information when adding it
        /// </summary>
        /// <returns></returns>
        public static async Task AddCustomerAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine("Please fill in the following details:");
            Console.WriteLine();
            Console.WriteLine("Name (Required): ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name) || name.Length > 100)
            {
                Console.WriteLine("Name is required! (Max lenght 100)");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Email (Required):");
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

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Address (Required. Street name and number):");
            var address = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(address) || address.Length > 100)
            {
                Console.WriteLine("Address is required! (Max length 100)");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Phone number (Optional):");
            var phone = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(phone))
            {
                phone = "No phone number found";
            }

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose a password");
            var password = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Password is required!");
                return;
            }
            db.Customers.Add(new Customer
            {
                CustomerName = name,
                CustomerEmail = EncryptionHelper.Encrypt(email),
                CustomerAddress = EncryptionHelper.Encrypt(address),
                PhoneNumber = EncryptionHelper.Encrypt(phone),
                Password = EncryptionHelper.Encrypt(password)

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

        //-----------------
        // READ
        //-----------------

        /// <summary>
        /// Method for listing customers with encrypted sensitive information
        /// Password protection for viewing a certain customers information
        /// After log in sensitive information is decrypted
        /// </summary>
        /// <returns></returns>
        public static async Task ListCustomerDetailsAsync()
        {
            using var db = new ShopContext();

            var customers = await db.Customers.AsNoTracking()
                                              .OrderBy(x => x.CustomerId)
                                              .ToListAsync();

            if (!await db.Customers.AnyAsync())
            {
                Console.WriteLine("No customers found");
                return;
            }
            Console.WriteLine("Customer Id | Name | Email (Encrypted) | Address (Encrypted) | Phone number (Encrypted)");
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerId} | {customer.CustomerName} | {customer.CustomerEmail} | {customer.CustomerAddress} | {customer.PhoneNumber}");
            }

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Would you like to list a specific customers details? (y/n)");
            var choice = Console.ReadLine()?.Trim() ?? string.Empty;
            if (choice == "y")
            {
                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Please choose customer id");
                if (!int.TryParse(Console.ReadLine(), out var cId))
                {
                    Console.WriteLine("Invalid input, please use numbers");
                    return;
                }

                var customerToView = await db.Customers.FirstAsync(c => c.CustomerId == cId);
                var customerInfo = await db.Customers.AsNoTracking().Where(c => c.CustomerId == cId).ToListAsync();

                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Please enter password to view sensitve info");
                var pass = Console.ReadLine()?.Trim() ?? string.Empty;
                if (pass != EncryptionHelper.Decrypt(customerToView.Password))
                {
                    Console.WriteLine("Invalid password, please try again");
                    return;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("-----------");
                    Console.WriteLine("Customer name | Customer Email | Address | Phone number");
                    foreach (var info in customerInfo)
                    {
                        Console.WriteLine($"{info.CustomerName} | {EncryptionHelper.Decrypt(info.CustomerEmail)} | {EncryptionHelper.Decrypt(info.CustomerAddress)} | {EncryptionHelper.Decrypt(info.PhoneNumber)}");
                    }
                }                    
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// A simple method for listing customer IDs and Name
        /// when asking for an id choice.
        /// </summary>
        /// <returns></returns>
        public static async Task QuickCustomerListAsync()
        {
            using var db = new ShopContext();

            var cusList = await db.Customers.AsNoTracking().ToListAsync();
            Console.WriteLine();
            foreach (var cus in cusList)
            {
                Console.WriteLine($"{cus.CustomerId} - {cus.CustomerName}");
            }
        }

        //-----------------
        // UPDATE
        //-----------------

        /// <summary>
        /// Method for editing a specific customer
        /// Password protection before beeing able to edit
        /// </summary>
        /// <returns></returns>
        public static async Task EditCustomerAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose a customer to edit (id#):");
            await QuickCustomerListAsync();

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

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please enter password to edit customer");
            var pass = Console.ReadLine()?.Trim() ?? string.Empty;
            if (pass != EncryptionHelper.Decrypt(customerToEdit.Password))
            {
                Console.WriteLine("Invalid password, please try agian");
                return;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Please choose an option to edit: Name (1) | Email (2) | Address (3) | Phone (4) | Password (5)");
                var choice = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;
                if (choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice != "5")
                {
                    Console.WriteLine("Invalid input, please try again");
                    return;
                }
                
                var whileChoice = "";
                while (whileChoice != "n")
                {
                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("-----------");
                            Console.WriteLine($"Current name: {customerToEdit.CustomerName} ");
                            Console.WriteLine("Please type in the new name");
                            var newName = Console.ReadLine()?.Trim() ?? string.Empty;
                            if (!string.IsNullOrEmpty(newName))
                            {
                                customerToEdit.CustomerName = newName;
                            }
                            else
                            {
                                Console.WriteLine("No changes were made");
                            }
                            break;

                        case "2":
                            Console.WriteLine("-----------");
                            Console.WriteLine($"Current email: {EncryptionHelper.Decrypt(customerToEdit.CustomerEmail)}");
                            Console.WriteLine("Please type in the new email");
                            var newEmail = Console.ReadLine()?.Trim() ?? string.Empty;
                            if (!string.IsNullOrEmpty(newEmail))
                            {
                                customerToEdit.CustomerEmail = EncryptionHelper.Encrypt(newEmail);
                            }
                            else
                            {
                                Console.WriteLine("No changes were made");
                            }
                            break;

                        case "3":
                            Console.WriteLine("-----------");
                            Console.WriteLine($"Current Address: {EncryptionHelper.Decrypt(customerToEdit.CustomerAddress)}");
                            Console.WriteLine("Please type in the new address");
                            var newAddress = Console.ReadLine()?.Trim() ?? string.Empty;
                            if (!string.IsNullOrEmpty(newAddress))
                            {
                                customerToEdit.CustomerAddress = EncryptionHelper.Encrypt(newAddress);
                            }
                            else
                            {
                                Console.WriteLine("No changes were made");
                            }
                            break;

                        case "4":
                            Console.WriteLine("-----------");
                            Console.WriteLine($"Current Phone number: {EncryptionHelper.Decrypt(customerToEdit.PhoneNumber)}");
                            Console.WriteLine("Please type in the new phone number");
                            var newNumber = Console.ReadLine()?.Trim() ?? string.Empty;
                            if (!string.IsNullOrEmpty(newNumber))
                            {
                                customerToEdit.PhoneNumber = EncryptionHelper.Encrypt(newNumber);
                            }
                            else
                            {
                                Console.WriteLine("No changes were made");
                            }
                            break;

                        case "5":
                            Console.WriteLine("-----------");
                            Console.WriteLine($"Current password is: {EncryptionHelper.Decrypt(customerToEdit.Password)}");
                            Console.WriteLine("Please choose a new password");
                            var newPass = Console.ReadLine()?.Trim() ?? string.Empty;
                            if (!string.IsNullOrEmpty(newPass))
                            {
                                customerToEdit.Password = EncryptionHelper.Encrypt(newPass);
                            }
                            else
                            {
                                Console.WriteLine("No changes were made");
                            }
                            break;
                        default:

                            break;
                    }

                    try
                    {
                        await db.SaveChangesAsync();
                        Console.WriteLine("-----------");
                        Console.WriteLine($"Customer Id: #{customerToEdit.CustomerId} has been edited!");
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
                        throw;
                    }

                    Console.WriteLine("-----------");
                    Console.WriteLine("Would you like to edit something else? (y/n)");
                    whileChoice = Console.ReadLine()?.Trim() ?? string.Empty; 
                }
            }
        }

        //-----------------
        // DELETE
        //-----------------

        /// <summary>
        /// Method for removing a customer from the database.
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteCustomerAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose a customer to delete (id#)");
            await QuickCustomerListAsync();

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

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine($"Please enter your password for customer #{customerId} to confirm deletion");
            var password = Console.ReadLine()?.Trim() ?? string.Empty;
            if (password != EncryptionHelper.Decrypt(customer.Password))
            {
                Console.WriteLine("Wrong password, please try again");
                return;
            }
            else
            {
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
}
