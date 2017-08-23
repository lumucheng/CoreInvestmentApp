using CoreInvestmentApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreInvestmentApp.Model
{
    public class Stock
    {
        private string growth;
        private string dividend;
        private string average;
        private string currentValue;
        private string volume;
        private string marketCap;
        private string debtToEquity;

        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string AdjClosePrice { get; set; }
        public string FiftyTwoWeekHigh { get; set; }
        public string FiftyTwoWeekLow { get; set; }
        public string Sector { get; set; }
        public List<EarningPerShare> EpsList { get; set; }
        public List<FreeCashFlow> CashFlowList { get; set; }
        public List<DebtToEquity> DebtToEquityList { get; set; }
        public List<ReturnOnEquity> ReturnToEquityList { get; set; }
        public List<ReturnOnAsset> ReturnToAssetList { get; set; }
        public string BasicEps { get; set; }
        public string EpsGrowth { get; set; }
        public string PEG { get; set; }
        public string AskPrice { get; set; }
        public StockIdentifier StockIdentifier { get; set; }

        public Stock()
        {
            EpsList = new List<EarningPerShare>();
            CashFlowList = new List<FreeCashFlow>();
            DebtToEquityList = new List<DebtToEquity>();
            StockIdentifier = new StockIdentifier();
        }

        public string Name
        {
            get { return StockIdentifier.Name; }
        }

        public string CurrentValue
        {
            get
            {
                decimal percent = Decimal.Parse(currentValue);
                return percent.ToString("0.##");
            }
            set { currentValue = value; }
        }

        public string Growth
        {
            get
            {
                if (growth == "na")
                {
                    return "--";
                }

                decimal percent = Decimal.Parse(growth);

                return String.Format("{0:P2}", percent);
            }
            set { growth = value; }
        }

        public string Dividend
        {
            get
            {
                if (dividend == "na")
                {
                    return " --";
                }
                return dividend;
            }
            set { dividend = value; }
        }

        public string Average
        {
            get
            {
                if (average == "na")
                {
                    return " --";
                }
                return average;
            }
            set { average = value; }
        }

        public string Volume
        {
            get
            {
                decimal vol = Decimal.Parse(volume);
                return vol.ToString("#,##0.00");
            }
            set { volume = value; }
        }

        public string MarketCap
        {
            get
            {
                decimal market = Decimal.Parse(marketCap);
                return Util.FormatNumber(market);
            }
            set { marketCap = value; }
        }

        public string DebtToEquity
        {
            get
            {
                decimal debtDecimal = Decimal.Parse(debtToEquity) * 100;
                return String.Format("{0:P2}", debtDecimal);
            }
            set { debtToEquity = value; }
        }
    }
}
