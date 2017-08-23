using CoreInvestmentApp.Model;
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
	public partial class InfoContentView : ContentView
	{
        Stock stock;
		public InfoContentView (Stock stock)
		{
			InitializeComponent ();

            this.stock = stock;
            LabelDescription.Text = stock.Description;
            LabelAdjClosePrice.Text = "$" + stock.AdjClosePrice;
            LabelVolume.Text = stock.Volume;
            LabelFiftyTwoHigh.Text = "$" +stock.FiftyTwoWeekHigh;
            LabelFiftyTwoLow.Text = "$" + stock.FiftyTwoWeekLow;
            LabelSector.Text = stock.Sector;
            LabelName.Text = stock.Name;
            LabelTicker.Text = stock.StockIdentifier.Ticker;
            LabelMarketCap.Text = stock.MarketCap;
            circleImage.Source = stock.ImageUrl;

            // Bind data to chart.
            // pieChart.BindingContext = ViewModelLocator.OxyExData;
        }
	}
}