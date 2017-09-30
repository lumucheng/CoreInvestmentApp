using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CoreInvestmentApp.Model;
using Acr.UserDialogs;
using System.Globalization;
using Realms;

namespace CoreInvestmentApp.Pages
{
    public partial class EditPortfolioPage : ContentPage
    {
        PortfolioStock portfolio;

        public EditPortfolioPage(PortfolioStock portfolio)
        {
            InitializeComponent();

            this.portfolio = portfolio;

			Title = "Update Portfolio";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Command = new Command(() => Navigation.PopModalAsync()),
			});

            LayoutInterface();
        }

        private void LayoutInterface()
        {
            LabelStockName.Text = portfolio.StockName;
            LabelStockTicker.Text = portfolio.StockTicker;
            LabelSector.Text = portfolio.StockSector;

            string price = string.Format("{0}.{1}", portfolio.PurchasePriceBeforeDecimalPoint, 
                                         portfolio.PurchasePriceAfterDecimalPoint);
            EntryPrice.Text = price.ToString();
            EntryQuantity.Text = portfolio.Quantity.ToString();
            PickerPurchaseDate.Date = portfolio.DateAdded.Date;
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
			bool valid = true;
			string message = "";
			int quantity = 0, priceBeforeDecimalPoint = 0, priceAfterDecimalPoint = 0;
			decimal price = 0.0M;

			if (EntryQuantity.Text == null || EntryQuantity.Text.Length == 0 || EntryPrice.Text == null || EntryPrice.Text.Length == 0)
			{
				valid = false;
				message = "Please ensure you have entered both quantity & purchased price.";
			}
			if (!int.TryParse(EntryQuantity.Text, out quantity))
			{
				valid = false;
				message = "Please ensure quantity is a whole number.";
			}
			if (!Decimal.TryParse(EntryPrice.Text, out price))
			{
				valid = false;
				message = "Please ensure purchased price is non zero.";
			}

            if (!valid)
            {
                UserDialogs.Instance.Alert(message, "Error", "OK");
            }
            else
            {
                string stringPrice = price.ToString("0.00", CultureInfo.InvariantCulture);
                string[] parts = stringPrice.Split('.');
                priceBeforeDecimalPoint = int.Parse(parts[0]);
                priceAfterDecimalPoint = int.Parse(parts[1]);

                // Save to DB
                var vRealmDb = Realm.GetInstance();



                using (var transaction = vRealmDb.BeginWrite())
				{
                    PortfolioStock updatePortfolio = new PortfolioStock();
                    updatePortfolio.PortfolioID = portfolio.PortfolioID;
                    updatePortfolio.StockName = portfolio.StockName;
                    updatePortfolio.StockTicker = portfolio.StockTicker;
                    updatePortfolio.StockSector = portfolio.StockSector;
                    updatePortfolio.Quantity = quantity;
					updatePortfolio.PurchasePriceBeforeDecimalPoint = priceBeforeDecimalPoint;
					updatePortfolio.PurchasePriceAfterDecimalPoint = priceAfterDecimalPoint;
					updatePortfolio.DateAdded = PickerPurchaseDate.Date;
                    updatePortfolio.UserEnteredPortfolio = portfolio.UserEnteredPortfolio;

                    vRealmDb.Add(updatePortfolio, true);
					transaction.Commit();
				}

                UserDialogs.Instance.Alert("Stock updated.", null, "OK");
                Navigation.PopModalAsync();
            }
        }
    }
}
