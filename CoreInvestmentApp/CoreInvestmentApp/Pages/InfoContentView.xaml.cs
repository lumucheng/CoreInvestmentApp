using CoreInvestmentApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CoreInvestmentApp.Classes;

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
            LabelDescription.Text = stock.Description;
            LabelAdjClosePrice.Text = stock.AdjClosePriceString;
            LabelVolume.Text = stock.VolumeString;
            LabelFiftyTwoHigh.Text = stock.FiftyTwoWeekHighString;
            LabelFiftyTwoLow.Text = stock.FiftyTwoWeekLowString;
            LabelSector.Text = stock.Sector;
            LabelName.Text = stock.Name;
            LabelTicker.Text = stock.StockIdentifier.Ticker;
            LabelMarketCap.Text = stock.MarketCapString;
            EditorRemarks.Text = stock.Remarks;
            circleImage.Source = stock.ImageUrl;

            // Bind data to chart.
            // pieChart.BindingContext = ViewModelLocator.OxyExData;
        }

        private void Handle_Completed(object sender, System.EventArgs e)
        {
            string remarks = EditorRemarks.Text;
            if (remarks != null)
            {
                stock.Remarks = EditorRemarks.Text.Trim();
                Util.SaveStockToDB(stock);
            }
        }
    }
}