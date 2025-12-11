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
    /// with CRUD operations for orders 
    /// </summary>
    public static class OrderHelpers
    {
        //-----------------
        // CREATE
        //-----------------

        /// <summary>
        /// Method for adding orders to a specific customer id
        /// </summary>
        /// <returns></returns>
        public static async Task AddOrderAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose a customer id to start your new order");
            await CustomerHelpers.QuickCustomerListAsync();

            if (!int.TryParse(Console.ReadLine(), out var customerId))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }
            if (!await db.Customers.AnyAsync(c => c.CustomerId == customerId))
            {
                Console.WriteLine("Customer not found");
                return;
            }

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 0
            };
            var orderRows = new List<OrderRow>();

            var choice = "";
            while (choice != "n")
            {
                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Choose a product to add to your order");
                await ProductHelpers.ListProductsAsync();

                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Product id: ");
                if (!int.TryParse(Console.ReadLine(), out var prodId))
                {
                    Console.WriteLine("Invalid product id!, please try again");
                    continue;
                }

                var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == prodId);
                if (product == null)
                {
                    Console.WriteLine("Product not found, please try again");
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Choose quantity: ");                
                if (!int.TryParse(Console.ReadLine(), out var qty))
                {
                    Console.WriteLine("You need to input numbers, please try again");
                    continue;
                }
                if (qty <= 0)
                {
                    Console.WriteLine("Quantity must be positive, please try again");
                    continue;
                }
                if (await db.Products.AnyAsync(p => p.StockQuantity < qty))
                {
                    Console.WriteLine("Quantity cannot exceed stock quantity");
                    continue;
                }

                var row = new OrderRow
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = qty,
                    UnitPrice = product.ProductPrice
                };

                orderRows.Add(row);
                product.StockQuantity -= qty;
                await db.SaveChangesAsync();

                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Would you like to add another product to your order? (y/n)");
                choice = Console.ReadLine();
                if (choice == "y")
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            order.OrderRowsList.AddRange(orderRows);            

            db.Orders.Add(order);            
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine($"Order complete - Order ID: {order.OrderId}");
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
        /// Method for listing all orders in the database
        /// </summary>
        /// <returns></returns>
        public static async Task ListOrdersAsync()
        {
            using var db = new ShopContext();
                        
            var orders = await db.Orders.AsNoTracking()
                                        .Include(x => x.Customer)                                        
                                        .OrderBy(x => x.OrderId)
                                        .ToListAsync();            

            if (!await db.Orders.AnyAsync())
            {
                Console.WriteLine("No orders found");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Order id | Customer Id | Customer Name | Order date | Status | Total amount");
            foreach (var order in orders)
            {
                Console.WriteLine($"{order.OrderId} | {order.Customer?.CustomerId} | {order.Customer?.CustomerName} | {order.OrderDate} | {order.Status} | {order.TotalAmount}");
            }
        }

        /// <summary>
        /// Method for listing orders in 
        /// chosen amount of orders/page
        /// </summary>
        /// <returns></returns>
        public static async Task OrdersPagedAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose a page number");
            if (!int.TryParse(Console.ReadLine(), out var page))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose number of orders/page");
            if (!int.TryParse(Console.ReadLine(), out var pageSize))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }

            var query = db.Orders
                          .Include(o => o.Customer)
                          .AsNoTracking()
                          .OrderByDescending(o => o.OrderDate);

            var totalOrderCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalOrderCount / (double)pageSize);

            var orders = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine($"Page = {page} / {totalPages}, page size = {pageSize}");            
            Console.WriteLine("-----------");
            Console.WriteLine("Orders sorted by Date");
            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Order date | Order id | Customer id | Customer name | Total amount | Status");
            foreach (var order in orders)
            {
                Console.WriteLine($"{order.OrderDate:d} | {order.OrderId} | {order.CustomerId} | {order.Customer?.CustomerName} | {order.TotalAmount} | {order.Status}");
            }
        }

        /// <summary>
        /// Method for listing a specific customers orders
        /// - Password protection before viewing orders
        /// </summary>
        /// <returns></returns>
        public static async Task ListCustomerOrdersAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose the id of the customer whos orders you would like to view");
            await CustomerHelpers.QuickCustomerListAsync();

            if (!int.TryParse(Console.ReadLine(), out var cId))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }
            if (!await db.Customers.AnyAsync(c => c.CustomerId == cId))
            {
                Console.WriteLine("Customer not found!");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please enter the password for this customer");
            var pass = Console.ReadLine()?.Trim() ?? string.Empty;
            
            var customer = await db.Customers.FirstAsync(c => c.CustomerId == cId);
            var orders = await db.Orders.Where(o => o.CustomerId == cId).ToListAsync();
            if (pass == EncryptionHelper.Decrypt(customer.Password))
            {
                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine($"Customer: {customer.CustomerId} - {customer.CustomerName} | Number of orders: {customer.OrdersList.Count()}");
                
                Console.WriteLine("-----------");
                Console.WriteLine($"Orders from Customer #{customer.CustomerId}");
                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Order Id | Total amount");
                Console.WriteLine("-----------");
                foreach (var order in orders)
                {
                    Console.WriteLine($"{order.OrderId} | {order.TotalAmount} ");
                }

                Console.WriteLine();
                Console.WriteLine("-----------");
                Console.WriteLine("Would you like to view a specific order? (y/n)");
                var choice = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;
                if (choice == "y")
                {
                    Console.WriteLine();
                    Console.WriteLine("-----------");
                    Console.WriteLine("Please type in the order you wish to view (id#)");
                    if (!int.TryParse(Console.ReadLine(), out var orderId))
                    {
                        Console.WriteLine("Invalid input, please use numbers");
                        return;
                    }
                    var orderToView = await db.Orders.Include(o => o.OrderRowsList)
                                                     .ThenInclude(o => o.Product)
                                                     .FirstOrDefaultAsync(o => o.OrderId == orderId);
                    
                    Console.WriteLine("-----------");
                    Console.WriteLine($"Products in order #{orderToView.OrderId} - Order date: {orderToView.OrderDate}");
                    Console.WriteLine("Product Id | Product name | Unit price | Quantity | Total amount");                    
                    Console.WriteLine("-----------");
                    foreach (var product in orderToView.OrderRowsList)
                    {
                        Console.WriteLine($"{product.ProductId} | {product.Product?.ProductName} | {product.UnitPrice} | {product.Quantity} | {orderToView.TotalAmount} ");
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                Console.WriteLine("Invalid password, please try again!");
                return;
            }
        }

        /// <summary>
        /// Method for listing orders with total amount using
        /// the created view "OrderSummary"
        /// </summary>
        /// <returns></returns>
        public static async Task OrderSummaryAsync()
        {
            using var db = new ShopContext();

            var summaries = await db.OrderSummaries.OrderByDescending(o => o.OrderDate).ToListAsync();

            Console.WriteLine("Order Id | Order date | Total amount | Customer name");
            foreach (var sum in summaries)
            {
                Console.WriteLine($"{sum.OrderId} | {sum.OrderDate} | {sum.TotalAmount} | {sum.CustomerName}");
            }
        }

        //-----------------
        // DELETE
        //-----------------

        /// <summary>
        /// Method for removing an order from the database
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteOrderAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine();
            Console.WriteLine("-----------");
            Console.WriteLine("Please choose an order to delete (id#)");
            await ListOrdersAsync();

            if (!int.TryParse(Console.ReadLine(), out var orderId))
            {
                Console.WriteLine("Please try again, use numbers");
                return;
            }     

            var orderToDelete = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            db.Orders.Remove(orderToDelete);
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine();
                Console.WriteLine($"Order #{orderToDelete.OrderId} removed from the database");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
            }
        }
    }
}
