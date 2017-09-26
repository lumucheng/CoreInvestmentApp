using Acr.UserDialogs;
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
                        LabelDividendReviewPrice.Text = "USD" + (dividend * (dividendYield * 0.01M)).ToString("F2");
                        LabelDividendEntryPrice.Text = "USD" + (dividend * (dividendYield * 0.01M) * 0.8M).ToString("F2");
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
                UserDialogs.Instance.Alert("Please ensure dividend is a valid number.", "Error", "OK");
                EntryDividendYield.Text = "0";
            }
            else
            {
                if (decimal.TryParse(EntryDividend.Text, out dividend))
                {
                    if (dividendYield != 0)
                    {
                        LabelDividendReviewPrice.Text = "USD" + (dividend * (dividendYield * 0.01M)).ToString("F2");
                        LabelDividendEntryPrice.Text = "USD" + (dividend * (dividendYield * 0.01M) * 0.8M).ToString("F2");
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