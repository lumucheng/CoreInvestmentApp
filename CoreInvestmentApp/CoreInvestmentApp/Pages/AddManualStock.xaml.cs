using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CoreInvestmentApp.Model;
using CoreInvestmentApp.Classes;

namespace CoreInvestmentApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddManualStock : ContentPage
    {
        public AddManualStock()
        {
            InitializeComponent();

            Title = "Add Stock";

            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Cancel",
                Command = new Command(() => Navigation.PopModalAsync()),
            });
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void ValidateInputs()
        {
            bool valid = true;
            decimal entryPrice, growth, eps, dividend, dividendYield, bookValue, pbRatio;
            string companyName;
            string ticker;
            string description;
            string sector;
            string message = "";

            companyName = EntryCompanyName.Text;
            ticker = EntryTicker.Text;
            description = EditorDescription.Text;
            sector = EntrySector.Text;

			if (description == null)
			{
				description = "";
			}
			if (sector == null)
			{
				sector = "";
			}

            if (companyName == null)
            {
                message = "Please enter Company Name";
                valid = false;
            }
            if (ticker == null)
            {
                message = "Please enter Stock Ticker";
                valid = false;
            }   
            if (!decimal.TryParse(EntryPrice.Text, out entryPrice))
            {
                message = "Please ensure Market Price is a valid number";
				EntryPrice.Text = "0";
                valid = false;
            }
			if (!decimal.TryParse(EntryGrowth.Text, out growth))
			{
                message = "Please ensure Growth is a valid number";
				EntryGrowth.Text = "0";
                valid = false;
			}
			if (!decimal.TryParse(EntryEPS.Text, out eps))
			{
                message = "Please ensure EPS is a valid number";
				EntryEPS.Text = "0";
                valid = false;
			}
			if (!decimal.TryParse(EntryDividend.Text, out dividend))
			{
                message = "Please ensure dividend is a valid number";
				EntryDividend.Text = "0";
                valid = false;
            }
			if (!decimal.TryParse(EntryDividendYield.Text, out dividendYield))
			{
                message = "Please ensure dividend yield is a valid number";
				EntryDividendYield.Text = "0";
                valid = false;
			}
			if (!decimal.TryParse(EntryBookValue.Text, out bookValue))
			{
                message = "Please ensure book value is a valid number";
				EntryBookValue.Text = "0";
                valid = false;
			}
			if (!decimal.TryParse(EntryRatio.Text, out pbRatio))
			{
                message = "Please ensure ratio is a valid number";
				EntryRatio.Text = "0";
                valid = false;
			}

            if (valid)
            {
                // Save to DB
                Stock stock = new Stock();
                stock.UserManualEntry = true;
                stock.StockIdentifier.Name = companyName;
                stock.StockIdentifier.Ticker = ticker;
                stock.Description = description;
                stock.Sector = sector;
                stock.NetworkEffect = ChkBoxNetwork.Checked;
                stock.IntangibleAssets = ChkBoxIntangible.Checked;
                stock.CostAdvantage = ChkBoxCost.Checked;
                stock.EfficientScale = ChkBoxEfficientScale.Checked;
                stock.SwitchingCost = ChkBoxSwitch.Checked;
                stock.CostantEPS = ChkboxConstantEPS.Checked;
                stock.OperationFlow = ChkboxCashFlow.Checked;
                stock.Reliable = ChkboxReliable.Checked;
                stock.Efficient = ChkboxEfficient.Checked;
                stock.LegalRisk = (SegControlLegal.SelectedSegment == 0) ? false : true;
                stock.InflationRisk = (SegControlInflation.SelectedSegment == 0) ? false : true;
                stock.StructureSystemRisk = (SegControlStruct.SelectedSegment == 0) ? false : true;
                stock.TechnologyRisk = (SegControlTechnology.SelectedSegment == 0) ? false : true;

                stock.AdjClosePrice = entryPrice;
                stock.CurrentValue = entryPrice;

                // Growth
                stock.UserEnteredGrowthPercent = true;
                stock.UserEnteredGrowthValue = growth;
                stock.UserEnteredTTM = eps;
                stock.GrowthEntryPrice = 0.8M * stock.BasicEps * stock.EpsEstimatedGrowth;

                // Dividend
                stock.UserEnteredDividend = dividend;
                stock.UserEnteredDividendYield = dividendYield;
                if (dividendYield > 0)
                {
                    stock.DivdendEntryPrice = 0.8M * ((dividend / dividendYield) * 100);
                }

                // Asset
                stock.UserBookValuePerShare = bookValue;
                stock.UserEnteredCurrentRatio = pbRatio;
                stock.AssetEntryPrice = 0.8M * stock.BookValuePerShare;

                Util.SaveStockToDB(stock);

				MessagingCenter.Send<string>("refresh", "refresh");
				Navigation.PopModalAsync();
            }
            else 
            {
                UserDialogs.Instance.Alert(message, "Error", "OK");
            }
        }

        private void EntryPrice_Completed(object sender, EventArgs e)
        {
            decimal entryPrice;

            if (!decimal.TryParse(EntryPrice.Text, out entryPrice))
            {
                UserDialogs.Instance.Alert("Please ensure Market Price is a valid number.", "Error", "OK");
                EntryPrice.Text = "0";
            }
        }

        private void EntryGrowth_Completed(object sender, EventArgs e)
        {
            decimal growth;
            decimal eps;

            if (!decimal.TryParse(EntryGrowth.Text, out growth))
            {
                UserDialogs.Instance.Alert("Please ensure growth is a valid number.", "Error", "OK");
                EntryGrowth.Text = "0";
            }
            else
            {
                if (decimal.TryParse(EntryEPS.Text, out eps))
                {
                    LabelGrowthReviewPrice.Text = "USD" + (growth * eps).ToString("F2");
                    LabelGrowthEntryPrice.Text = "USD" + (growth * eps * 0.8M).ToString("F2");
                }
            }
        }

        private void EntryEPS_Completed(object sender, EventArgs e)
        {
            decimal growth;
            decimal eps;

            if (!decimal.TryParse(EntryEPS.Text, out eps))
            {
                UserDialogs.Instance.Alert("Please ensure EPS is a valid number.", "Error", "OK");
                EntryEPS.Text = "0";
            }
            else
            {
                if (decimal.TryParse(EntryGrowth.Text, out growth))
                {
                    LabelGrowthReviewPrice.Text = "USD" + (growth * eps).ToString("F2");
                    LabelGrowthEntryPrice.Text = "USD" + (growth * eps * 0.8M).ToString("F2");
                }
            }
        }

        private void EntryDividend_Completed(object sender, EventArgs e)
        {
            decimal dividend;
            decimal dividendYield;

            if (!decimal.TryParse(EntryDividend.Text, out dividend))
            {
                UserDialogs.Instance.Alert("Please ensure dividend is a valid number.", "Error", "OK");
                EntryDividend.Text = "0";
            }
            else
            {
                if (decimal.TryParse(EntryDividendYield.Text, out dividendYield))
                {
                    if (dividendYield != 0)
                    {
                        LabelDividendReviewPrice.Text = "USD" + (dividend * (dividendYield * 0.01M) * 100M).ToString("F2");
                        LabelDividendEntryPrice.Text = "USD" + (dividend * (dividendYield * 0.01M) * 0.8M * 100M).ToString("F2");
                    }
                }
            }
        }

        private void EntryDividendYield_Completed(object sender, EventArgs e)
        {
            decimal dividend;
            decimal dividendYield;

            if (!decimal.TryParse(EntryDividendYield.Text, out dividendYield))
            {
                UserDialogs.Instance.Alert("Please ensure dividend yield is a valid number.", "Error", "OK");
                EntryDividendYield.Text = "0";
            }
            else
            {
                if (decimal.TryParse(EntryDividend.Text, out dividend))
                {
                    if (dividendYield != 0)
                    {
						LabelDividendReviewPrice.Text = "USD" + (dividend * (dividendYield * 0.01M) * 100M).ToString("F2");
						LabelDividendEntryPrice.Text = "USD" + (dividend * (dividendYield * 0.01M) * 0.8M * 100M).ToString("F2");
                    }
                }
            }
        }

        private void EntryBookValue_Completed(object sender, EventArgs e)
        {
            decimal bookValue;
            decimal pbRatio;

            if (!decimal.TryParse(EntryBookValue.Text, out bookValue))
            {
                UserDialogs.Instance.Alert("Please ensure book value is a valid number.", "Error", "OK");
                EntryBookValue.Text = "0";
            }
            else
            {
                if (decimal.TryParse(EntryRatio.Text, out pbRatio))
                {
                    LabelBookReviewPrice.Text = "USD" + (bookValue * pbRatio).ToString("F2");
                    LabelBookEntryPrice.Text = "USD" + (bookValue * pbRatio * 0.8M).ToString("F2");
                }
            }
        }

        private void EntryRatio_Completed(object sender, EventArgs e)
        {
            decimal bookValue;
            decimal pbRatio;

            if (!decimal.TryParse(EntryRatio.Text, out pbRatio))
            {
                UserDialogs.Instance.Alert("Please ensure ratio is a valid number.", "Error", "OK");
                EntryRatio.Text = "0";
            }
            else
            {
                if (decimal.TryParse(EntryBookValue.Text, out bookValue))
                {
                    LabelBookReviewPrice.Text = "USD" + (bookValue * pbRatio).ToString("F2");
                    LabelBookEntryPrice.Text = "USD" + (bookValue * pbRatio * 0.8M).ToString("F2");
                }
            }
        }
    }
}