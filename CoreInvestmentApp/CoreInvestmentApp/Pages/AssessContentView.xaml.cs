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
            UpdateInvestorConfidence();
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

            // LIST
            ChkBoxLegal.Checked = stock.LegalRisk;
            ChkBoxLegal.CheckedChanged += (sender, e) =>
            {
                stock.LegalRisk = ChkBoxLegal.Checked;
                UpdateInvestorConfidence();
            };

            ChkBoxInflation.Checked = stock.InflationRisk;
            ChkBoxInflation.CheckedChanged += (sender, e) =>
            {
                stock.InflationRisk = ChkBoxInflation.Checked;
                UpdateInvestorConfidence();
            };

            ChkBoxStructure.Checked = stock.StructureSystemRisk;
            ChkBoxStructure.CheckedChanged += (sender, e) =>
            {
                stock.StructureSystemRisk = ChkBoxStructure.Checked;
                UpdateInvestorConfidence();
            };

            ChkBoxTechnology.Checked = stock.TechnologyRisk;
            ChkBoxTechnology.CheckedChanged += (sender, e) =>
            {
                stock.TechnologyRisk = ChkBoxTechnology.Checked;
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

            if (stock.LegalRisk)
            {
                listCount++;
            }
            if (stock.InflationRisk)
            {
                listCount++;
            }
            if (stock.StructureSystemRisk)
            {
                listCount++;
            }
            if (stock.TechnologyRisk)
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
    }
}