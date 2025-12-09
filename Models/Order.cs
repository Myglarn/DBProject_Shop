using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Models
{
    public class Order
    {
        // PK
        public int OrderId { get; set; }

        // FK
        public int CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }
        
        [Required]
        public string? Status { get; set; } = string.Empty;
        
        public decimal TotalAmount { get; set; }
        
        public Customer? Customer { get; set; } 
        
        public List<OrderRow> OrderRowsList { get; set; } = new();
    }
}
