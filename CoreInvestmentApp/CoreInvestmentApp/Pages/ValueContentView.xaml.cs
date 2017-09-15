using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OxyPlot.Annotations;
using Acr.UserDialogs;
using OxyPlot.Axes;
using System.Text.RegularExpressions;

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


            if (stock.Dividend > 0)
            {
                EntryCashDividend.Text = Util.FormatNumberToCurrency(stock.Dividend, CURRENCY_TYPE.USD);
            }
            else
            {
                EntryCashDividend.Text = Util.FormatNumberToCurrency(stock.UserEnteredDividend, CURRENCY_TYPE.USD);
            }
            
            LabelCurrentYield.Text = Util.FormatNumberToPercent(stock.DividendYield);

            UpdateDividendLabels();
                   
            UpdateBookLabels();

            UpdateEpsLabels();
        }

        private void UpdateEpsLabels()
        {
            List<EarningPerShare> SortedList = stock.EpsList.OrderByDescending(o => o.Date).ToList();

            double totalThree = 0.0f;
            double totalFive = 0.0f;

            if (SortedList.Count() >= 4)
            {
                if (SortedList[0].Value > 0 && SortedList[3].Value >=0)
                {
                    totalThree = (Math.Pow(Math.Abs((SortedList[0].Value / SortedList[3].Value)), (1.0 / 3.0)) - 1) * 100;
                }
            }
            if (SortedList.Count() >= 6)
            {
                if (SortedList[0].Value >= 0 && SortedList[5].Value >= 0)
                {
                    totalFive = (Math.Pow(Math.Abs((SortedList[0].Value / SortedList[5].Value)), 0.2) - 1) * 100;
                }
            }

            LabelThreeYears.Text = totalThree.ToString("F2");
            LabelFiveYears.Text = totalFive.ToString("F2");

            double estimatedGrowth = (totalFive > totalThree) ? totalThree : totalFive;

            if (SortedList.Count > 0)
            {
                LabelAnnual.Text = SortedList[0].Value.ToString();
            }
            else
            {
                LabelAnnual.Text = "-";
            }

            LabelTTM.Text = stock.BasicEpsString;
            LabelGrowth.Text = Util.FormatNumberToPercent(stock.EpsGrowth);

            if (!stock.UserEnteredGrowthPercent)
            {
                stock.EpsEstimatedGrowth = Convert.ToDecimal(Math.Round(estimatedGrowth, 2));
            }

            EntryEstimate.Text = stock.EpsEstimatedGrowth.ToString("F2");
            decimal entryPrice = stock.BasicEps * stock.EpsEstimatedGrowth;
            LabelEpsEntryPrice.Text = "  " + Util.FormatNumberToCurrency(entryPrice, CURRENCY_TYPE.USD) + "  ";
            stock.GrowthEntryPrice = entryPrice;

            decimal reviewPrice = 1.25M * entryPrice;
            LabelEpsReviewPrice.Text = "  " + Util.FormatNumberToCurrency(reviewPrice, CURRENCY_TYPE.USD) + "  ";
        }

        private void UpdateDividendLabels()
        {
            decimal dividendValue = stock.Dividend;

            if (stock.Dividend == 0)
            {
                dividendValue = stock.UserEnteredDividend;
                EntryCashDividend.IsEnabled = true;
            }

            decimal dividendEntry = (dividendValue / Decimal.Parse(EntryExpectedDividendYield.Text)) * 100;

            stock.DivdendEntryPrice = dividendEntry;
            LabelDividendEntryPrice.Text = Util.FormatNumberToCurrency(dividendEntry, CURRENCY_TYPE.USD);
            EntryCashDividend.Text = Util.FormatNumberToCurrency(dividendValue, CURRENCY_TYPE.USD);

            decimal dividendReviewPrice = dividendEntry * 1.25M;
            LabelDivdendReviewPrice.Text = Util.FormatNumberToCurrency(dividendReviewPrice, CURRENCY_TYPE.USD);

            stock.DivdendEntryPrice = dividendEntry;

            Util.SaveStockToDB(stock);
        }

        private void UpdateBookLabels()
        {
            decimal priceToBook = stock.PriceToBook;
            decimal currentRatio = stock.CurrentRatio;

            if (stock.PriceToBook == 0) 
            {
                priceToBook = stock.UserEnteredPriceToBook;
                EntryPriceToBook.IsEnabled = true;
            }
            EntryPriceToBook.Text = Util.FormatNumberToCurrency(priceToBook, CURRENCY_TYPE.USD);

            if (stock.CurrentRatio == 0)
            {
                currentRatio = stock.UserEnteredCurrentRatio;
                EntryCurrentRatio.IsEnabled = true;
            }
            EntryCurrentRatio.Text = currentRatio.ToString("F2");

			LabelBookValue.Text = Util.FormatNumberToCurrency(stock.BookValuePerShare, CURRENCY_TYPE.USD);
			EntryPriceToBook.Text = Util.FormatNumberToCurrency(priceToBook, CURRENCY_TYPE.USD);

            decimal entryPriceBookValue = stock.BookValuePerShare * 0.80M;
            LabelEntryBookValuePrice.Text = Util.FormatNumberToCurrency(entryPriceBookValue, CURRENCY_TYPE.USD);

            decimal bookExpectedReturn = entryPriceBookValue * 1.25M;
            LabelBookValuerReviewPrice.Text = Util.FormatNumberToCurrency(bookExpectedReturn, CURRENCY_TYPE.USD);

            stock.AssetEntryPrice = entryPriceBookValue;
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

            var xAxis = new LinearAxis();
            xAxis.Position = AxisPosition.Bottom;
            xAxis.IsZoomEnabled = false;
            xAxis.IsPanEnabled = false;
            xAxis.Key = "x";

            var yAxis = new LinearAxis();
            yAxis.Position = AxisPosition.Left;
            yAxis.IsZoomEnabled = false;
            yAxis.IsPanEnabled = false;
            yAxis.Key = "y";

            plotModel1.Axes.Add(xAxis);
            plotModel1.Axes.Add(yAxis);

            areaSeries1.XAxisKey = "x";
            areaSeries1.YAxisKey = "y";

            foreach (EarningPerShare eps in stock.EpsList)
            {
				var pointAnnotation1 = new PointAnnotation();
				pointAnnotation1.X = Convert.ToDouble(eps.Date.Year);
				pointAnnotation1.Y = Convert.ToDouble(eps.Value);
				pointAnnotation1.Text = String.Format("{0}", eps.Value);
				plotModel1.Annotations.Add(pointAnnotation1);
                areaSeries1.Points.Add(new DataPoint(eps.Date.Year, eps.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            EPSModel = plotModel1;
            EPSChart.IsEnabled = false;
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

            var xAxis = new LinearAxis();
            xAxis.Position = AxisPosition.Bottom;
            xAxis.IsZoomEnabled = false;
            xAxis.IsPanEnabled = false;
            xAxis.Key = "x";

            var yAxis = new LinearAxis();
            yAxis.Position = AxisPosition.Left;
            yAxis.IsZoomEnabled = false;
            yAxis.IsPanEnabled = false;
            yAxis.Key = "y";

            plotModel1.Axes.Add(xAxis);
            plotModel1.Axes.Add(yAxis);

            areaSeries1.XAxisKey = "x";
            areaSeries1.YAxisKey = "y";

            Dictionary<int, decimal> dividendByYear = new Dictionary<int, decimal>();

            foreach (Dividend divdend in stock.DividendList)
            {
                int year = divdend.Date.Year;
                decimal value;

                if (dividendByYear.ContainsKey(year))
                {
                    value = dividendByYear[year];
                    value += divdend.Value;
                    dividendByYear[year] = value;
                }
                else
                {
                    dividendByYear.Add(year, divdend.Value);
                }
            }

            foreach (int key in dividendByYear.Keys)
            {
                var val = Convert.ToDouble(dividendByYear[key]);
                                             
				var pointAnnotation1 = new PointAnnotation();
				pointAnnotation1.X = Convert.ToDouble(key);
                pointAnnotation1.Y = val;
				pointAnnotation1.Text = String.Format("{0}", val);
				plotModel1.Annotations.Add(pointAnnotation1);
                areaSeries1.Points.Add(new DataPoint(key, val));
            }

            plotModel1.Series.Add(areaSeries1);

            DividendModel = plotModel1;
            DividendChart.IsEnabled = false;
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

            var xAxis = new LinearAxis();
            xAxis.Position = AxisPosition.Bottom;
            xAxis.IsZoomEnabled = false;
            xAxis.IsPanEnabled = false;
            xAxis.Key = "x";

            var yAxis = new LinearAxis();
            yAxis.Position = AxisPosition.Left;
            yAxis.IsZoomEnabled = false;
            yAxis.IsPanEnabled = false;
            yAxis.Key = "y";

            plotModel1.Axes.Add(xAxis);
            plotModel1.Axes.Add(yAxis);

            areaSeries1.XAxisKey = "x";
            areaSeries1.YAxisKey = "y";

            foreach (BookValue bookValue in stock.BookValueList)
            {
				var pointAnnotation1 = new PointAnnotation();
				pointAnnotation1.X = Convert.ToDouble(bookValue.Date.Year);
				pointAnnotation1.Y = Convert.ToDouble(bookValue.Value);
				pointAnnotation1.Text = String.Format("{0}", bookValue.Value);
				plotModel1.Annotations.Add(pointAnnotation1);
                areaSeries1.Points.Add(new DataPoint(bookValue.Date.Year, bookValue.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            BookValueModel = plotModel1;
            BookValueChart.IsEnabled = false;
        }

        void Handle_EntryExpectedBookPrice_Completed(object sender, System.EventArgs e)
        {
            //decimal tryParse = 0.0M;
            //Decimal.TryParse(EntryExpectedBookPrice.Text, out tryParse);

            //EntryExpectedBookPrice.Text = tryParse.ToString();
            //UpdateBookLabels();
        }

        void Handle_EntryExpectedDividendYield_Completed(object sender, System.EventArgs e)
        {
            decimal tryParse = 0.0M;
            Decimal.TryParse(EntryExpectedDividendYield.Text, out tryParse);
            
            if (tryParse != 0.0M)
            {
                EntryExpectedDividendYield.Text = tryParse.ToString();
                UpdateDividendLabels();
            }
            else
            {
                LabelDivdendReviewPrice.Text = Util.FormatNumberToCurrency(tryParse, CURRENCY_TYPE.USD);
                LabelDividendEntryPrice.Text = Util.FormatNumberToCurrency(tryParse, CURRENCY_TYPE.USD);
            }
        }

        private void EntryEstimate_Completed(object sender, EventArgs e)
        {
            decimal estimate = 0.0M;
            bool result = Decimal.TryParse(EntryEstimate.Text, out estimate);

            if (result)
            {
                stock.EpsEstimatedGrowth = estimate;
            }

            EntryEstimate.Text = estimate.ToString("F2");

            decimal entryPrice = stock.BasicEps * stock.EpsEstimatedGrowth;
            LabelEpsEntryPrice.Text = "  " + Util.FormatNumberToCurrency(entryPrice, CURRENCY_TYPE.USD) + "  ";
            stock.GrowthEntryPrice = entryPrice;

            decimal reviewPrice = 1.2M * entryPrice;
            LabelEpsReviewPrice.Text = "  " + Util.FormatNumberToCurrency(reviewPrice, CURRENCY_TYPE.USD) + "  ";

            stock.UserEnteredGrowthPercent = true;

            Util.SaveStockToDB(stock);
        }

        private void EntryCashDividend_Completed(object sender, EventArgs e)
        {
            string text = EntryCashDividend.Text;
            text = Regex.Replace(text, "[^0-9.]", "");

            decimal dividend = 0.0M;
            bool result = Decimal.TryParse(text, out dividend);

            if (result)
            {
                stock.UserEnteredDividend = dividend;
            }

            UpdateDividendLabels();
        }

        private void EntryPriceToBook_Completed(object sender, System.EventArgs e)
        {
			string text = EntryCashDividend.Text;
            text = Regex.Replace(text, "[^0-9.]", "");

			decimal priceToBook = 0.0M;
			bool result = Decimal.TryParse(text, out priceToBook);

			if (result)
			{
				stock.UserEnteredPriceToBook = priceToBook;
			}

            UpdateBookLabels();
        }

        private void EntryCurrentRatio_Completed(object sender, System.EventArgs e)
        {
			string text = EntryCurrentRatio.Text;
			text = Regex.Replace(text, @"[^\d]", "");

			decimal currentRatio = 0.0M;
			bool result = Decimal.TryParse(text, out currentRatio);

			if (result)
			{
                stock.CurrentRatio = currentRatio;
			}

            UpdateBookLabels();
        }
    }
}