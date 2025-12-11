using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBProject_Shop.Migrations
{
    /// <inheritdoc />
    public partial class ViewTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW IF NOT EXISTS OrderSummary AS
            SELECT
                o.OrderId,
                o.OrderDate,
                c.CustomerName,
                c.CustomerEmail,
                IFNULL(SUM(orw.Quantity * orw.UnitPrice), 0) AS TotalAmount
            FROM Orders o
            JOIN Customers c ON c.CustomerId = o.CustomerId
            LEFT JOIN OrderRows orw ON orw.OrderId = o.OrderId
            GROUP BY o.OrderId, o.OrderDate, c.CustomerName, c.CustomerEmail;
            ");
            
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Insert
            AFTER INSERT ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = (
                                   SELECT IFNULL(SUM(Quantity * UnitPrice), 0) 
                                   FROM OrderRows 
                                   WHERE OrderId = NEW.OrderId
                                  )
                WHERE OrderId = NEW.OrderId;
            END;                
            ");
            
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Update
            AFTER UPDATE ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = ( 
                                    SELECT IFNULL (SUM(Quantity * UnitPrice), 0)
                                    FROM OrderRows 
                                    WHERE OrderId = NEW.OrderId
                                  )
                WHERE OrderId = NEW.OrderId;                                  
            END;
            ");
            
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Delete
            AFTER DELETE ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = ( 
                                    SELECT IFNULL (SUM(Quantity * UnitPrice), 0)
                                    FROM OrderRows 
                                    WHERE OrderId = NEW.OrderId
                                  )
                WHERE OrderId = NEW.OrderId;
            END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP VIEW IF EXISTS OrderSummary
            ");

            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_OrderRow_Insert
            ");

            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_OrderRow_Update
            ");

            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_OrderRow_Delete
            ");
        }
    }
}
