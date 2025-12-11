# DBProject_Shop

## Description
This project is a database application in EF Core for an e-shop. It contains models for Customers, Orders with order rows, products and categories with relations for all models. Symetric encryption using an XOR-Cipher, security using a simple password that is encrypted. CRUD operations for most models and CLI-commands for easy navigation.

## Key Features Demonstrated in This Project

### Symetric encryption of data using an XOR-Cipher

Example: Customer CRUD-operations 

Each Customer has an encrypted password to use when

- Viewing a specific customers encrypted details

- Editing a customers details or viewing a specific customers orders.

- Deleting a customer from the database

### View and Triggers

A view for showing a simple summary of total amounts for every existing order.
Triggers for calculating total amount when:

- An order is added to the database

- When order rows are added to the order

### Entity Relations 

- Customer 1 : 0..N Order

- Order 1 : 1..N Order rows

- Order rows N : 1 Product

- Product N : 1 Category

  <img width="1123" height="712" alt="ER" src="https://github.com/user-attachments/assets/a3fc931a-db9b-4254-a6ae-82e2382a9960" />

### Known limitations 

- Products and Categories can be added, deleted and edited by anyone. A solution could be to create an Admin model that has sole access to
  adding, editing and deleting products and categories with a password.
  
- No method for editing orders after they are made. 

- Status is always set to pending and never changed.
  A solution could be to create a method or trigger that updates the status of orders based on
  time passed since the order creation for a simple simulation of the staus changing.
     

## How to use the application

### Seeded customer log in credentials:
- User: Arber | password : 12345
- User: Christopher | password: 198610


### Main Menu

Switch with CLI-command menus for:
- Customers
- Orders
- Products
- Categories

### Seeding

Seeding at startup with:
- Customers
- Categories
- Products

### Helpers

Full CRUD functionality for:

- Customers    
- Categories
- Products
- Orders is missing EDIT functionality
- Encryption Helper with methods for encryption/decryption for certain customer information 
with password protection, which is also encrypted.

## Credits

### Developed by: 
Christopher Petti

### Technologies Used:

- SQLite
- EF CORE
- .Net 8

