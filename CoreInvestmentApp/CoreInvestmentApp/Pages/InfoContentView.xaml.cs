using CoreInvestmentApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CoreInvestmentApp.Classes;
using Acr.UserDialogs;
using System.Text.RegularExpressions;

namespace CoreInvestmentApp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InfoContentView : ContentView
	{
        Stock stock;
		public InfoContentView (Stock stock)
		{
			InitializeComponent ();

            this.stock = stock;
            LabelAdjClosePrice.Text = stock.AdjClosePriceString;
            LabelVolume.Text = stock.VolumeString;

            if (stock.UserManualEntry)
            {
                LabelAdjClosePrice.IsVisible = false;
                EntryAdjClosePrice.IsVisible = true;
                EntryAdjClosePrice.Text = stock.AdjClosePriceString;
            }
            else 
            {
				LabelAdjClosePrice.IsVisible = true;
                EntryAdjClosePrice.IsVisible = false;
				LabelAdjClosePrice.Text = stock.AdjClosePriceString;
            }

            LabelDescription.Text = stock.Description;
            LabelFiftyTwoHigh.Text = stock.FiftyTwoWeekHighString;
            LabelFiftyTwoLow.Text = stock.FiftyTwoWeekLowString;
            LabelSector.Text = stock.Sector;
            LabelName.Text = stock.Name;
            LabelTicker.Text = stock.StockIdentifier.Ticker;
            LabelMarketCap.Text = stock.MarketCapString;
            EditorRemarks.Text = stock.Remarks;
            circleImage.Source = stock.ImageUrl;
        }

        private void EditorRemarksHandle_Completed(object sender, System.EventArgs e)
        {
            string remarks = EditorRemarks.Text;
            if (remarks != null)
            {
                stock.Remarks = EditorRemarks.Text.Trim();
                Util.SaveStockToDB(stock);
            }
        }

        private void EntryAdjClosePriceHandle_Completed(object sender, System.EventArgs e)
        {
			string text = EntryAdjClosePrice.Text;
			text = Regex.Replace(text, "[^0-9.]", "");

			decimal closePrice = 0.0M;
			bool result = Decimal.TryParse(text, out closePrice);

            if (result)
            {
				stock.AdjClosePrice = closePrice;
                stock.CurrentValue = closePrice;
				Util.SaveStockToDB(stock);
                EntryAdjClosePrice.Text = stock.AdjClosePriceString;
            }
            EntryAdjClosePrice.Text = stock.AdjClosePriceString;
        }
    }
}