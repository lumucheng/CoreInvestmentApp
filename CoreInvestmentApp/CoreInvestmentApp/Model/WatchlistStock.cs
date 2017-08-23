using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreInvestmentApp.Model
{
    public class WatchlistStock
    {
        public StockIdentifier Stock { get; set; }
        public string RevenueGrowth { get; set; }
        public string Dividend { get; set; }
        public string AverageDailyVolume { get; set; }
        public string ClosePrice { get; set; }
    }
}
