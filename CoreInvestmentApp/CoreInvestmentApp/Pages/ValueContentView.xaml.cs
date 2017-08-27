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
        public PlotModel DividendModel { get; set; }
        public PlotModel BookValueModel { get; set; }

        public ValueContentView(Stock stock)
        {
            InitializeComponent();
            this.stock = stock;

            CreateDividendChart();

            DividendChart.BindingContext = this;
            BookValueChart.BindingContext = this;

            LabelCashDividend.Text = Util.FormatNumberToCurrency(stock.Dividend, CURRENCY_TYPE.USD);
            LabelCurrentYield.Text = Util.FormatNumberToPercent(stock.DividendYield);
        }

        private void CreateDividendChart()
        {
            var plotModel1 = new PlotModel { Title = "Divdend (USD)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 1,
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

        void Handle_Completed(object sender, System.EventArgs e)
        {
            //stock.EpsEstimatedGrowth = Decimal.Parse(EntryEstimate.Text);
            //decimal entryPrice = stock.BasicEps * stock.EpsEstimatedGrowth;
            //LabelEntryPrice.Text = Util.FormatNumberToCurrency(entryPrice, CURRENCY_TYPE.USD);
        }
    }
}