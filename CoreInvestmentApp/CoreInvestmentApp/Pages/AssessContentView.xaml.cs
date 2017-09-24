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
using OxyPlot.Annotations;

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

        public decimal InvestorScore
        {
            get
            {
                return stock.GrowthEntryPrice;
            }
        }

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

            LabelDebtToEquity.Text = Util.FormatNumberToPercent(stock.DebtToEquity);
            InitCheckBoxes();
            InitSegControl();
            UpdateInvestorConfidence();
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            
        }


        private void InitCheckBoxes()
        {
            // NICES
            ChkBoxNetwork.Checked = stock.NetworkEffect;
            ChkBoxNetwork.CheckedChanged += (sender, e) =>
            {
                stock.NetworkEffect = ChkBoxNetwork.Checked;
                UpdateInvestorConfidence();
            };

            ChkBoxIntangible.Checked = stock.IntangibleAssets;
            ChkBoxIntangible.CheckedChanged += (sender, e) =>
            {
                stock.IntangibleAssets = ChkBoxIntangible.Checked;
                UpdateInvestorConfidence();
            };

            ChkBoxCost.Checked = stock.CostAdvantage;
            ChkBoxCost.CheckedChanged += (sender, e) =>
            {
                stock.CostAdvantage = ChkBoxCost.Checked;
                UpdateInvestorConfidence();
            };

            ChkBoxEfficientScale.Checked = stock.EfficientScale;
            ChkBoxEfficientScale.CheckedChanged += (sender, e) =>
            {
                stock.EfficientScale = ChkBoxEfficientScale.Checked;
                UpdateInvestorConfidence();
            };

            ChkBoxSwitch.Checked = stock.SwitchingCost;
            ChkBoxSwitch.CheckedChanged += (sender, e) =>
            {
                stock.SwitchingCost = ChkBoxSwitch.Checked;
                UpdateInvestorConfidence();
            };

            // CORE
            ChkboxConstantEPS.Checked = stock.CostantEPS;
            ChkboxConstantEPS.CheckedChanged += (sender, e) =>
            {
                stock.CostantEPS = ChkboxConstantEPS.Checked;
                UpdateInvestorConfidence();
            };

            ChkboxCashFlow.Checked = stock.OperationFlow;
            ChkboxCashFlow.CheckedChanged += (sender, e) =>
            {
                stock.OperationFlow = ChkboxCashFlow.Checked;
                UpdateInvestorConfidence();
            };

            ChkboxReliable.Checked = stock.Reliable;
            ChkboxReliable.CheckedChanged += (sender, e) =>
            {
                stock.Reliable = ChkboxReliable.Checked;
                UpdateInvestorConfidence();
            };

            ChkboxEfficient.Checked = stock.Efficient;
            ChkboxEfficient.CheckedChanged += (sender, e) =>
            {
                stock.Efficient = ChkboxEfficient.Checked;
                UpdateInvestorConfidence();
            };
        }

        private void InitSegControl()
        {
            // LIST
            SegControlLegal.SelectedSegment = (stock.LegalRisk) ? 1 : 0;
            SegControlLegal.PropertyChanged += (sender, e) =>
            {
                stock.LegalRisk = Convert.ToBoolean(SegControlLegal.SelectedSegment);
                UpdateInvestorConfidence();
            };

            SegControlInflation.SelectedSegment = (stock.InflationRisk) ? 1 : 0;
            SegControlInflation.PropertyChanged += (sender, e) =>
            {
                stock.InflationRisk = Convert.ToBoolean(SegControlInflation.SelectedSegment);
                UpdateInvestorConfidence();
            };

            SegControlStruct.SelectedSegment = (stock.StructureSystemRisk) ? 1 : 0;
            SegControlStruct.PropertyChanged += (sender, e) =>
            {
                stock.StructureSystemRisk = Convert.ToBoolean(SegControlStruct.SelectedSegment);
                UpdateInvestorConfidence();
            };

            SegControlTechnology.SelectedSegment = (stock.TechnologyRisk) ? 1 : 0;
            SegControlTechnology.PropertyChanged += (sender, e) =>
            {
                stock.TechnologyRisk = Convert.ToBoolean(SegControlTechnology.SelectedSegment);
                UpdateInvestorConfidence();
            };
        }

        private void UpdateInvestorConfidence()
        {
            int percent = 0;
            int nicesCount = 0;
            int listCount = 0;
            int coreCount = 0;

            if (stock.NetworkEffect)
            {
                nicesCount++;
            }
            if (stock.IntangibleAssets)
            {
                nicesCount++;
            }
            if (stock.CostAdvantage)
            {
                nicesCount++;
            }
            if (stock.EfficientScale)
            {
                nicesCount++;
            }
            if (stock.SwitchingCost)
            {
                nicesCount++;
            }

            if (!stock.LegalRisk)
            {
                listCount++;
            }
            if (!stock.InflationRisk)
            {
                listCount++;
            }
            if (!stock.StructureSystemRisk)
            {
                listCount++;
            }
            if (!stock.TechnologyRisk)
            {
                listCount++;
            }

            if (stock.CostantEPS)
            {
                coreCount++;
            }
            if (stock.OperationFlow)
            {
                coreCount++;
            }
            if (stock.Reliable)
            {
                coreCount++;
            }
            if (stock.Efficient)
            {
                coreCount++;
            }

            if (nicesCount >= 2)
            {
                percent = 40;
            }
            else if (nicesCount >= 1)
            {
                percent = 20;
            }

            int listPercent = listCount * 5;
            percent += listPercent;

            int corePercent = coreCount * 10;
            percent += corePercent;

            stock.InvestorConfidence = percent;

            string confidenceString = String.Format("Investor Confidence: {0}%", percent);
            LabelScore.Text = confidenceString;
            LabelScore.HorizontalTextAlignment = TextAlignment.Center;
            LabelScore.LineBreakMode = LineBreakMode.NoWrap;

            // Save to DB
            Util.SaveStockToDB(stock);
        }

        private void CreateEPSChart()
        {
            var plotModel1 = new PlotModel { Title = "Earnings Per Share (USD)" };	
            var areaSeries1 = new AreaSeries
            {
                LabelFormatString = "{1}",
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 3,
                Color = OxyColor.FromRgb(210, 198, 1),
                Fill = OxyColor.FromRgb(209, 220, 114),
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

            List<EarningPerShare> yearList = stock.EpsList.OrderBy(o => o.Date).ToList();
            List<EarningPerShare> valueList = stock.EpsList.OrderBy(o => o.Value).ToList();

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

        private void CreateOCFChart()
        {
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 3,
                Color = OxyColor.FromRgb(85, 161, 77),
                Fill = OxyColor.FromRgb(143, 199, 150)
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
                unitStr = "Hundred K";
                unit = hundredthousands;
            }

            string title = "Operating Cash Flow";
            string subTitle = String.Format("({0} USD)", unitStr);
            var plotModel1 = new PlotModel { Title = title, Subtitle = subTitle };

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

            plotModel1.Series.Add(areaSeries1);

			foreach (FreeCashFlow fcf in stock.CashFlowList)
            {
				var pointAnnotation1 = new PointAnnotation();
				pointAnnotation1.X = Convert.ToDouble(fcf.Date.Year);
				pointAnnotation1.Y = Convert.ToDouble(fcf.Value / unit);
				pointAnnotation1.Text = String.Format("{0}", fcf.Value / unit);
				plotModel1.Annotations.Add(pointAnnotation1);
                areaSeries1.Points.Add(new DataPoint(fcf.Date.Year, fcf.Value / unit));
            }

            OCFModel = plotModel1;
            OCFChart.IsEnabled = false;
        }

        private void CreateDTEChart()
        {
            var plotModel1 = new PlotModel { Title = "Debt To Equity (%)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 3,
                Color = OxyColor.FromRgb(193, 70, 53),
                Fill = OxyColor.FromRgb(212, 151, 141)
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

            foreach (DebtToEquity dte in stock.DebtToEquityList)
            {
				var pointAnnotation1 = new PointAnnotation();
				pointAnnotation1.X = Convert.ToDouble(dte.Date.Year);
				pointAnnotation1.Y = Convert.ToDouble(dte.Value);
				pointAnnotation1.Text = String.Format("{0}", dte.Value);
				plotModel1.Annotations.Add(pointAnnotation1);
                areaSeries1.Points.Add(new DataPoint(dte.Date.Year, dte.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            DTEModel = plotModel1;
            DTEChart.IsEnabled = false;
        }

        private void CreateROEChart()
        {
            var plotModel1 = new PlotModel { Title = "Return on Equity (%)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 3,
                Color = OxyColor.FromRgb(44, 62, 60),
                Fill = OxyColor.FromRgb(129, 148, 144)
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

            foreach (ReturnOnEquity roe in stock.ReturnToEquityList)
            {
				var pointAnnotation1 = new PointAnnotation();
				pointAnnotation1.X = Convert.ToDouble(roe.Date.Year);
				pointAnnotation1.Y = Convert.ToDouble(roe.Value);
				pointAnnotation1.Text = String.Format("{0}", roe.Value);
				plotModel1.Annotations.Add(pointAnnotation1);
                areaSeries1.Points.Add(new DataPoint(roe.Date.Year, roe.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            ROEModel = plotModel1;
            ROEChart.IsEnabled = false;
        }

        private void CreateROAChart()
        {
            var plotModel1 = new PlotModel { Title = "Return on Assets (%)" };
            var areaSeries1 = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                StrokeThickness = 3,
                Color = OxyColor.FromRgb(182, 136, 68),
                Fill = OxyColor.FromRgb(194, 184, 188)
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

            foreach (ReturnOnAsset roa in stock.ReturnToAssetList)
            {
				var pointAnnotation1 = new PointAnnotation();
				pointAnnotation1.X = Convert.ToDouble(roa.Date.Year);
				pointAnnotation1.Y = Convert.ToDouble(roa.Value);
				pointAnnotation1.Text = String.Format("{0}", roa.Value);
				plotModel1.Annotations.Add(pointAnnotation1);
                areaSeries1.Points.Add(new DataPoint(roa.Date.Year, roa.Value));
            }

            plotModel1.Series.Add(areaSeries1);

            ROAModel = plotModel1;
            ROAChart.IsEnabled = false;
        }
    }
}