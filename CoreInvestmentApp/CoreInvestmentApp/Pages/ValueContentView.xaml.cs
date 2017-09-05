using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreInvestmentApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ValueContentView : ContentView
    {
        Stock stock;
        public PlotModel EPSModel { get; set; }
        public PlotModel DividendModel { get; set; }
        public PlotModel BookValueModel { get; set; }

        public ValueContentView(Stock stock)
        {
            InitializeComponent();
            this.stock = stock;

            CreateEPSChart();
            CreateDividendChart();
            CreateBookValueChart();

            EPSChart.BindingContext = this;
            DividendChart.BindingContext = this;
            BookValueChart.BindingContext = this;

            LabelCashDividend.Text = Util.FormatNumberToCurrency(stock.Dividend, CURRENCY_TYPE.USD);
            LabelCurrentYield.Text = Util.FormatNumberToPercent(stock.DividendYield);

            UpdateDividendLabels();
                   
            LabelBookValue.Text = Util.FormatNumberToCurrency(stock.BookValuePerShare, CURRENCY_TYPE.USD);
            LabelPriceToBook.Text = stock.PriceToBook.ToString("0.##");

            UpdateBookLabels();

            List<EarningPerShare> SortedList = stock.EpsList.OrderByDescending(o => o.Date).ToList();

            double totalThree = 0.0f;
            double totalFive = 0.0f;

            if (SortedList.Count() >= 3)
            {
                totalThree = Math.Pow((SortedList[0].Value - SortedList[2].Value), 0.5) - 1;
            }

            if (SortedList.Count() >= 5)
            {
                totalThree = Math.Pow((SortedList[0].Value - SortedList[4].Value), 0.25) - 1;

            }
            LabelThreeYears.Text = (totalThree / 3.0).ToString("F2");
            LabelFiveYears.Text = (totalFive / 5.0).ToString("F2");

            LabelAnnual.Text = SortedList[0].Value.ToString();
            LabelTTM.Text = stock.BasicEpsString;
            LabelGrowth.Text = Util.FormatNumberToPercent(stock.EpsGrowth);

            LabelPEG.Text = "--";
            if (stock.AdjClosePrice > 0 && stock.BasicEps > 0 && stock.EpsGrowth > 0)
            {
                decimal peg = (stock.AdjClosePrice / stock.BasicEps) * stock.EpsGrowth;
                LabelPEG.Text = Util.FormatNumberToCurrency(peg, CURRENCY_TYPE.DOLLAR_SIGN);
            }

            if (stock.EpsEstimatedGrowth > 0)
            {
                EntryEstimate.Text = stock.EpsEstimatedGrowth.ToString();
                decimal entryPrice = stock.BasicEps * stock.EpsEstimatedGrowth;
                LabelEntryPrice.Text = Util.FormatNumberToCurrency(entryPrice, CURRENCY_TYPE.USD);
            }

            LabelEntryPrice.Text = Util.FormatNumberToCurrency(stock.GrowthEntryPrice, CURRENCY_TYPE.USD);
        }

        private void UpdateDividendLabels()
        {
            decimal dividendEntry = (stock.DividendYield / Decimal.Parse(EntryExpectedDividendYield.Text)) * 100;
            stock.DivdendEntryPrice = dividendEntry;
            LabelDividendEntryPrice.Text = Util.FormatNumberToCurrency(dividendEntry, CURRENCY_TYPE.USD);

            decimal dividendExpectedReturn = dividendEntry * 1.25M;
            LabelDividendExpectedReturn.Text = Util.FormatNumberToPercent(dividendExpectedReturn);

            decimal dividendStopLoss = dividendEntry * 0.8M;
            LabelDivdendStopLoss.Text = Util.FormatNumberToPercent(dividendStopLoss);
        }

        private void UpdateBookLabels()
        {
            decimal entryPriceBookValue = stock.PriceToBook * (1 - Decimal.Parse(EntryExpectedBookPrice.Text));
            stock.AssetEntryPrice = entryPriceBookValue;
            LabelEntryBookValuePrice.Text = Util.FormatNumberToCurrency(entryPriceBookValue, CURRENCY_TYPE.USD);

            decimal bookExpectedReturn = entryPriceBookValue * 1.25M;
            LabelBookValueExpectedReturn.Text = Util.FormatNumberToPercent(bookExpectedReturn);

            decimal bookStopLoss = entryPriceBookValue * 0.8M;
            LabelBookValueStopLoss.Text = Util.FormatNumberToPercent(bookStopLoss);
        }

        private void CreateEPSChart()
        {
            var plotModel1 = new PlotModel { Title = "Earnings Per Share (USD)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 3,
                Color = OxyColor.FromRgb(210, 198, 1),
                Fill = OxyColor.FromRgb(209, 220, 114)
            };

            foreach (EarningPerShare eps in stock.EpsList)
            {
                areaSeries1.Points.Add(new DataPoint(eps.Date.Year, eps.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            EPSModel = plotModel1;

        }
        private void CreateDividendChart()
        {
            var plotModel1 = new PlotModel { Title = "Dividend (USD)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 3,
                Color = OxyColor.FromRgb(85, 161, 77),
                Fill = OxyColor.FromRgb(143, 199, 150)
            };

            Dictionary<int, decimal> dividendByYear = new Dictionary<int, decimal>();

            foreach (Dividend divdend in stock.DividendList)
            {
                int year = divdend.Date.Year;
                decimal value;

                if (dividendByYear.ContainsKey(year))
                {
                    value = dividendByYear[year];
                    value += divdend.Value;
                }
                else
                {
                    dividendByYear.Add(year, divdend.Value);
                }
            }

            foreach (int key in dividendByYear.Keys)
            {
                areaSeries1.Points.Add(new DataPoint(key, Convert.ToDouble(dividendByYear[key])));
            }

            plotModel1.Series.Add(areaSeries1);

            DividendModel = plotModel1;
        }

        private void CreateBookValueChart()
        {
            var plotModel1 = new PlotModel { Title = "Book Value (USD)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 3,
                Color = OxyColor.FromRgb(174, 86, 20),
                Fill = OxyColor.FromRgb(190, 160, 126)
            };

            foreach (BookValue bookValue in stock.BookValueList)
            {
                areaSeries1.Points.Add(new DataPoint(bookValue.Date.Year, bookValue.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            BookValueModel = plotModel1;
        }

        void Handle_EntryExpectedBookPrice_Completed(object sender, System.EventArgs e)
        {
            UpdateBookLabels();
        }

        void Handle_EntryExpectedDividendYield_Completed(object sender, System.EventArgs e)
        {
            UpdateDividendLabels();
        }

        void Handle_Completed(object sender, System.EventArgs e)
        {
            stock.EpsEstimatedGrowth = Decimal.Parse(EntryEstimate.Text);
            decimal entryPrice = stock.BasicEps * stock.EpsEstimatedGrowth;
            stock.GrowthEntryPrice = entryPrice;

            LabelEntryPrice.Text = Util.FormatNumberToCurrency(entryPrice, CURRENCY_TYPE.USD);
        }
    }
}