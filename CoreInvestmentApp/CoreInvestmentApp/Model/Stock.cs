using CoreInvestmentApp.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public decimal BookValuePerShare { get; set; }
        public decimal PriceToBook { get; set; }
        public decimal PriceToEarnings { get; set; }
        public decimal CurrentRatio { get; set; }

		public string Sector { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }

        public DateTimeOffset LastUpdated;

        // NICES
        public bool NetworkEffect { get; set; }
        public bool IntangibleAssets { get; set; }
        public bool CostAdvantage { get; set; }
        public bool EfficientScale { get; set; }
        public bool SwitchingCost { get; set; }

        // CORE
        public bool CostantEPS { get; set; }
        public bool OperationFlow { get; set; }
        public bool Reliable { get; set; }
        public bool Efficient { get; set; }

        // LIST
        public bool LegalRisk { get; set; }
        public bool InflationRisk { get; set; }
        public bool StructureSystemRisk { get; set; }
        public bool TechnologyRisk { get; set; }

		// User Entered Data
		public bool UserEnteredGrowthPercent { get; set; }
		public decimal UserEnteredGrowthValue { get; set; }
		public decimal UserEnteredDividend { get; set; }
		public decimal UserBookValuePerShare { get; set; }
		public decimal UserEnteredCurrentRatio { get; set; }
        public string Remarks;

        // Calculated based on user input
        public decimal GrowthEntryPrice { get; set; }
        public decimal AssetEntryPrice { get; set; }
        public decimal DivdendEntryPrice { get; set; }
        public int InvestorConfidence { get; set; }

        public string GrowthEntryPriceString
        {
            get { return Util.FormatNumberToCurrency(GrowthEntryPrice, CURRENCY_TYPE.USD); }
        }
        public string AssetEntryPriceString
        {
            get { return Util.FormatNumberToCurrency(AssetEntryPrice, CURRENCY_TYPE.USD); }
        }
        public string DivdendEntryPriceString
        {
            get { return Util.FormatNumberToCurrency(DivdendEntryPrice, CURRENCY_TYPE.USD); }
        }
        public string InvestorConfidenceString
        {
            get { return String.Format("{0}%", InvestorConfidence); }
        }

        public StockIdentifier StockIdentifier { get; set; }
		public List<EarningPerShare> EpsList { get; set; }
		public List<FreeCashFlow> CashFlowList { get; set; }
		public List<DebtToEquity> DebtToEquityList { get; set; }
		public List<ReturnOnEquity> ReturnToEquityList { get; set; }
		public List<ReturnOnAsset> ReturnToAssetList { get; set; }
        public List<Dividend> DividendList { get; set; }
        public List<BookValue> BookValueList { get; set; }

        public Stock()
        {
            EpsList = new List<EarningPerShare>();
            CashFlowList = new List<FreeCashFlow>();
            DebtToEquityList = new List<DebtToEquity>();
            ReturnToEquityList = new List<ReturnOnEquity>();
            ReturnToAssetList = new List<ReturnOnAsset>();
            DividendList = new List<Model.Dividend>();
            BookValueList = new List<BookValue>();
            StockIdentifier = new StockIdentifier();
        }

        public string Ticker
        {
            get { return StockIdentifier.Ticker; }
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
            get { return Volume.ToString("#,##"); }
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
