using CoreInvestmentApp.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CoreInvestmentApp.Classes;

namespace CoreInvestmentApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessContentView : ContentView
    {
        Stock stock;
        public PlotModel EPSModel { get; set; }
        public PlotModel OCFModel { get; set; }
        public PlotModel DTEModel { get; set; }
        public PlotModel ROEModel { get; set; }
        public PlotModel ROAModel { get; set; }

        public AssessContentView(Stock stock)
        {
            InitializeComponent();

            this.stock = stock;
            CreateEPSChart();
            CreateOCFChart();
            CreateDTEChart();
            CreateROEChart();
            CreateROAChart();

            EPSChart.BindingContext = this;
            OCFChart.BindingContext = this;
            DTEChart.BindingContext = this;
            ROEChart.BindingContext = this;
            ROAChart.BindingContext = this;

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
                EntryEstimate.Text = Util.FormatNumberToPercent(stock.EpsEstimatedGrowth);
                decimal entryPrice = stock.BasicEps * stock.EpsEstimatedGrowth;
                LabelEntryPrice.Text = Util.FormatNumberToCurrency(entryPrice, CURRENCY_TYPE.USD);
            }

            LabelDebtToEquity.Text = Util.FormatNumberToPercent(stock.DebtToEquity);
        }

        private void CreateEPSChart()
        {
            var plotModel1 = new PlotModel { Title = "Earnings Per Share (USD)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 1,
            };

            foreach (EarningPerShare eps in stock.EpsList)
            {
                areaSeries1.Points.Add(new DataPoint(eps.Date.Year, eps.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            EPSModel =  plotModel1;
        }

        private void CreateOCFChart()
        {
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 1,
            };

            double total = 0.0f;
            foreach (FreeCashFlow fcf in stock.CashFlowList)
            {
                total += fcf.Value;
            }

            double avg = total / stock.CashFlowList.Count();
            double hundredthousands = 100000;
            double million = 1000000;
            double billion = 1000000000;
            double unit;
            string unitStr;

            if (avg / billion > 1)
            {
                unitStr = "Billion";
                unit = billion;
            }
            else if (avg / million > 1)
            {
                unitStr = "Million"; 
                unit = million;
            }
            else{
                unitStr = "Hundred Thousands";
                unit = hundredthousands;
            }

            foreach (FreeCashFlow fcf in stock.CashFlowList)
            {
                areaSeries1.Points.Add(new DataPoint(fcf.Date.Year, fcf.Value / unit));
            }

            string title = String.Format("Operating Cash Flow ({0} USD)", unitStr);
            var plotModel1 = new PlotModel { Title = title };
            plotModel1.Series.Add(areaSeries1);

            OCFModel = plotModel1;
        }

        private void CreateDTEChart()
        {
            var plotModel1 = new PlotModel { Title = "Debt To Equity (%)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 1,
            };

            foreach (DebtToEquity dte in stock.DebtToEquityList)
            {
                areaSeries1.Points.Add(new DataPoint(dte.Date.Year, dte.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            DTEModel = plotModel1;
        }

        private void CreateROEChart()
        {
            var plotModel1 = new PlotModel { Title = "Return on Equity (%)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 1,
            };

            foreach (ReturnOnEquity roe in stock.ReturnToEquityList)
            {
                areaSeries1.Points.Add(new DataPoint(roe.Date.Year, roe.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            ROEModel = plotModel1;
        }

        private void CreateROAChart()
        {
            var plotModel1 = new PlotModel { Title = "Return on Assets (%)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 1,
            };

            foreach (ReturnOnAsset roa in stock.ReturnToAssetList)
            {
                areaSeries1.Points.Add(new DataPoint(roa.Date.Year, roa.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            ROAModel = plotModel1;
        }

        void Handle_Completed(object sender, System.EventArgs e)
        {
            stock.EpsEstimatedGrowth = Decimal.Parse(EntryEstimate.Text);
			decimal entryPrice = stock.BasicEps * stock.EpsEstimatedGrowth;
			LabelEntryPrice.Text = Util.FormatNumberToCurrency(entryPrice, CURRENCY_TYPE.USD);
        }
    }
}