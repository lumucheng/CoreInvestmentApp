using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreInvestmentApp.Model
{
    public class PortfolioStock : RealmObject
    {
        [PrimaryKey]
        public int PortfolioID { get; set; }

        public DateTimeOffset DateAdded { get; set; }

        public int PurchasePriceBeforeDecimalPoint { get; set; }

        public int PurchasePriceAfterDecimalPoint { get; set; }

        public int Quantity { get; set; }

        public String StockName { get; set; }

        public String StockTicker { get; set; }

        public String StockSector { get; set; }

        [IgnoredAttribute]
        public String PurchasePriceString
        {
            get
            {
                string price = string.Format("USD{0}.{1}", PurchasePriceBeforeDecimalPoint.ToString(), PurchasePriceAfterDecimalPoint.ToString() + "0");
                return price;
            }
        }

        [IgnoredAttribute]
        public String DateAddedString
        {
            get
            {
                string date = DateAdded.ToString("dd MMM yyyy");
                return date;
            }
        }

        [IgnoredAttribute]
        public decimal CurrentValue { get; internal set; }
    }
}
