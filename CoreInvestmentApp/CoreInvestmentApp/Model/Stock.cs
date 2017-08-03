using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreInvestmentApp.Model
{
    public class Stock
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public string CurrentValue { get; set; }
        public string Growth { get; set; }
        public string Dividend { get; set; }
        public string Average { get; set; }
    }
}
