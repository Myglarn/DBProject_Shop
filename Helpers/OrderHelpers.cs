using DBProject_Shop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Helpers
{
    public static class OrderHelpers
    {
        // CREATE
        public static async Task AddOrderAsync()
        {
            using var db = new ShopContext();

            Console.WriteLine("Please choose a customer id to start your new order");
            await CustomerHelpers.ListCustomersAsync();

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
                Console.WriteLine("Choose a product to add to your order");
                await ProductHelpers.ListProductsAsync();
                
                Console.WriteLine();
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
            order.TotalAmount = orderRows.Sum(o => o.UnitPrice * o.Quantity);

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
        
        // READ
        public static async Task ListOrdersAsync()
        {
            using var db = new ShopContext();
            var orders = await db.Orders.AsNoTracking()
                                        .Include(x => x.Customer).Include(x => x.OrderRowsList)                                        
                                        .OrderBy(x => x.OrderId)
                                        .ToListAsync();
            if (!await db.Orders.AnyAsync())
            {
                Console.WriteLine("No orders found");
                return;
            }
            
            Console.WriteLine("Order id | Customer Id | Customer Name | Order date | Status | Total amount");
            foreach (var order in orders)
            {
                Console.WriteLine($"{order.OrderId} | {order.Customer?.CustomerId} | {order.Customer?.CustomerName} | {order.OrderDate} | {order.Status} | {order.TotalAmount}");
            }
        }

        public static async Task OrdersPagedAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine("Please choose a page number");
            if (!int.TryParse(Console.ReadLine(), out var page))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }

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

            Console.WriteLine($"Page = {page} / {totalPages}, page size = {pageSize}");
            Console.WriteLine("Orders sorted by Date");
            Console.WriteLine("Order date | Order Id | Total amount | Status");
            foreach (var order in orders)
            {
                Console.WriteLine($"{order.OrderDate:d} | {order.OrderId} | {order.TotalAmount} | {order.Status}");
            }
        }

        public static async Task ListCustomerOrdersAsync()
        {
            using var db = new ShopContext();
            Console.WriteLine();
            Console.WriteLine("Please choose the id of the customer whos orders you would like to view");
            await CustomerHelpers.ListCustomersAsync();

            if (!int.TryParse(Console.ReadLine(), out var cId))
            {
                Console.WriteLine("Invalid input, please use numbers");
                return;
            }

            
            var customer = await db.Customers.FirstAsync(c => c.CustomerId == cId);
            var orders = await db.Orders.Where(o => o.CustomerId == cId).Include(o => o.OrderRowsList).ToListAsync();
            
            Console.WriteLine($"Customer: {customer.CustomerId} - {customer.CustomerName} | Number of orders: {customer.OrdersList.Count()}");            
            Console.WriteLine($"Orders from Customer #{customer.CustomerId}");
            Console.WriteLine("Order Id | Total amount");
            foreach (var order in orders)
            {
                Console.WriteLine($"{order.OrderId} | {order.TotalAmount} ");
            }

            Console.WriteLine("Would you like to view a specific order? (y/n)");
            var choice = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;
            if (choice == "y")
            {
                Console.WriteLine("Please type in the order you wish to view (id#)");
                if (!int.TryParse(Console.ReadLine(), out var orderId))
                {
                    Console.WriteLine("Invalid input, please use numbers");
                    return;
                }
                var orderToView = await db.Orders.Include(o => o.OrderRowsList)
                                                 .ThenInclude(o => o.Product)
                                                 .FirstOrDefaultAsync(o => o.OrderId == orderId);
                
                Console.WriteLine("Order ID | Order date | Status | Total Amount ");
                Console.WriteLine($"{orderToView.OrderId} | {orderToView.OrderDate:d} | {orderToView.Status} | {orderToView.TotalAmount}");
                Console.WriteLine();

                Console.WriteLine($"Products in order #{orderToView.OrderId}");
                Console.WriteLine("Product Id | Product name | Unit price | Quantity");
                foreach (var product in orderToView.OrderRowsList)
                {
                    Console.WriteLine($"{product.ProductId} | {product.Product?.ProductName} | {product.UnitPrice} | {product.Quantity}");
                }
            }
            else
            {
                return;
            }
        }

        // UPDATE  KOM TIILBAKS TILL DENNA

        //public static async Task EditOrderAsync() // ??
        //{
        //    using var db = new ShopContext();
        //    Console.WriteLine("Please choose an order to edit (id#)");
        //    await ListOrdersAsync();

        //    if (!int.TryParse(Console.ReadLine(), out var oId))
        //    {
        //        Console.WriteLine("Incorrect input, please use numbers");
        //        return;
        //    }
        //    var orderToEdit = await db.OrderRows.AsNoTracking().Include(o => o.OrderRowsList).ThenInclude(o => o.Product).ThenInclude(o => o.ProductName).Where(o => o.Produ).ToListAsync();

        //    while (true)
        //    {
        //        Console.WriteLine("Please make a choice:");
        //        Console.WriteLine("Delete product from order (1) | Edit quantity (2) | Add another product (3)");
        //        var choice = Console.ReadLine()?.Trim() ?? string.Empty;
        //        switch (choice)
        //        {
        //            case "1":
        //                Console.WriteLine("Please choose a product to delte:");
        //                foreach (var product in orderToEdit)
        //                {
        //                    Console.WriteLine($"{product.OrderRowsList.}");
        //                }
        //                break;

        //            case "2":
        //                break;

        //            case "3":
        //                break;

        //            default:
        //                break;
        //        }
        //    }
        //}

        // DELETE
        public static async Task DeleteOrderAsync()
        {
            using var db = new ShopContext();
            
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
                Console.WriteLine($"Order #{orderToDelete.OrderId} removed from the database");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Db Error: " + ex.GetBaseException().Message);
            }

        }
    }
}
