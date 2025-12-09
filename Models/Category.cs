using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Models
{
    public class Category
    {
        // PK
        public int CategoryId { get; set; }        
        
        [Required, MaxLength(100)]        
        public string? CategoryName { get; set; } = string.Empty;       
        
        public List<Product> ProductsList { get; set; } = new();
    }
}
