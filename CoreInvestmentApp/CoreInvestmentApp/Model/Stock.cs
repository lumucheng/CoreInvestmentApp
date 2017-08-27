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
        public decimal Growth;
        public decimal Dividend;
        public decimal DividendYield;
        public decimal CurrentValue;
        public decimal Volume;
        public decimal MarketCap;
        public decimal DebtToEquity;
        public decimal EpsEstimatedGrowth;
        public decimal AdjClosePrice { get; set; }
		public decimal FiftyTwoWeekHigh { get; set; }
		public decimal FiftyTwoWeekLow { get; set; }
        public decimal BasicEps { get; set; }
        public decimal EpsGrowth { get; set; }
        public decimal PEG { get; set; }

		public string Sector { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }

        public StockIdentifier StockIdentifier { get; set; }
		public List<EarningPerShare> EpsList { get; set; }
		public List<FreeCashFlow> CashFlowList { get; set; }
		public List<DebtToEquity> DebtToEquityList { get; set; }
		public List<ReturnOnEquity> ReturnToEquityList { get; set; }
		public List<ReturnOnAsset> ReturnToAssetList { get; set; }
        public List<Dividend> DividendList { get; set; }

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

        public string CurrentValueString
        {
            get { return Util.FormatNumberToCurrency(CurrentValue, CURRENCY_TYPE.USD); }
        }

        public string VolumeString
        {
            get { return Volume.ToString("#,##0.00"); }
        }

        public string AdjClosePriceString
        {
            get { return Util.FormatNumberToCurrency(AdjClosePrice, CURRENCY_TYPE.DOLLAR_SIGN); }
        }

        public string FiftyTwoWeekHighString
        {
            get { return Util.FormatNumberToCurrency(FiftyTwoWeekHigh, CURRENCY_TYPE.DOLLAR_SIGN); }
        }

        public string FiftyTwoWeekLowString
        {
            get { return Util.FormatNumberToCurrency(FiftyTwoWeekLow, CURRENCY_TYPE.DOLLAR_SIGN); }
        }

        public string MarketCapString
        {
            get { return Util.FormatNumberEnglishUnits(MarketCap); }
        }

        public string BasicEpsString
        {
            get { return Util.FormatNumberToCurrency(BasicEps, CURRENCY_TYPE.DOLLAR_SIGN); }
        }

        public string EpsGrowthString
        {
            get { return Util.FormatNumberToPercent(EpsGrowth); }
        }

        
    }
}
