using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Models
{
    public class Product
    {        
        public int ProductId { get; set; }
        
        [Required, MaxLength(150)]
        public string? ProductName { get; set; } = string.Empty;
        
        [Required]
        public int StockQuantity { get; set; }
        
        [Required]
        public decimal ProductPrice { get; set; }
        
        public List<OrderRow> OrderRowsList { get; set; } = new();        
       
        public int CategoryId { get; set; }
        
        public Category? Category { get; set; }      
        
    }
}
