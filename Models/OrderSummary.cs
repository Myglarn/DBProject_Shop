using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProject_Shop.Models
{
    /*
     * Detta är en keyless entitet (INGEN PK)
     * Den representerar en sql view, en spara SELECT-query
     * Vi använder dessa Views i EF CORE som gör att den kan läsa den precis som en vanlig tabell
     */
    [Keyless]
    public class OrderSummary
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
