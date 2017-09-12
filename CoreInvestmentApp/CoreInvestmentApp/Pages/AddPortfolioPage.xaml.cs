using Acr.UserDialogs;
using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using Realms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreInvestmentApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPortfolioPage : ContentPage
    {
        Stock stock;

        public AddPortfolioPage(Stock stock)
        {
            InitializeComponent();

            this.stock = stock;

            Title = "Add Portfolio";

            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Done",
                Command = new Command(() => Navigation.PopModalAsync()),
            });

            InitLayout();
        }

        private void InitLayout()
        {
            LabelStockName.Text = stock.Name;
            LabelStockTicker.Text = stock.Ticker;
            LabelSector.Text = stock.Sector;
            LabelClosePrice.Text = Util.FormatNumberToCurrency(stock.AdjClosePrice, CURRENCY_TYPE.USD);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            bool valid = true;
            string message = "";
            int id = 0, quantity = 0, priceBeforeDecimalPoint = 0, priceAfterDecimalPoint = 0;
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
                var vPortfolio = vRealmDb.All<PortfolioStock>().LastOrDefault();

                if (vPortfolio == null)
                {
                    id = 1;
                }
                else
                {
                    id = vPortfolio.PortfolioID + 1;
                }

                PortfolioStock portfolio = new PortfolioStock();
                portfolio.Quantity = quantity;
                portfolio.PurchasePriceBeforeDecimalPoint = priceBeforeDecimalPoint;
                portfolio.PurchasePriceAfterDecimalPoint = priceAfterDecimalPoint;
                portfolio.StockName = stock.Name;
                portfolio.StockSector = stock.Sector;
                portfolio.StockTicker = stock.Ticker;
                portfolio.DateAdded = PickerPurchaseDate.Date;
                portfolio.PortfolioID = id;

                vRealmDb.Write(() =>
                {
                    vRealmDb.Add(portfolio, true);
                });

                UserDialogs.Instance.Alert("Stock added to your profile", null, "OK");
                Navigation.PopModalAsync();
            }
        }
    }
}