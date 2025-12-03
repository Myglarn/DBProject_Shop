using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Models
{
    public class OrderRow
    {
        // PK
        public int OrderRowId { get; set; }
        // FK
        public int OrderId { get; set; }
        // FK
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
