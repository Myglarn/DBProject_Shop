using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Models
{
    public class Customer
    {
        // PK
        public int CustomerId {  get; set; }
        [Required, MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string CustomerAdress {  get; set; } = string.Empty;
        public int PhoneNumber { get; set; }
        public List<Order> OrdersList { get; set; } = new();
    }
}
