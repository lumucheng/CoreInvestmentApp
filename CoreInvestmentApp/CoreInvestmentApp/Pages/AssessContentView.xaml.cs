using CoreInvestmentApp.Model;
using OxyPlot;
using OxyPlot.Axes;
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
            for (int i = 0; i < 5; i++)
            {
                totalFive += SortedList[i].Value;
               
                if (i < 3)
                {
                    totalThree += SortedList[i].Value;
                }
            }

            LabelThreeYears.Text = (totalThree / 3.0).ToString("F2");
            LabelFiveYears.Text = (totalFive / 5.0).ToString("F2");

            LabelAnnual.Text = stock.BasicEps;
            LabelTTM.Text = "--";
            LabelGrowth.Text = String.Format("{0:P2}", Decimal.Parse(stock.EpsGrowth));

            LabelPEG.Text = stock.PEG;
            LabelEntryPrice.Text = "USD" + stock.AskPrice;

            LabelDebtToEquity.Text = stock.DebtToEquity;
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
            var plotModel1 = new PlotModel { Title = "Operating Cash Flow" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 1,
            };

            foreach (FreeCashFlow fcf in stock.CashFlowList)
            {
                areaSeries1.Points.Add(new DataPoint(fcf.Date.Year, fcf.Value));
            }

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
    }
}