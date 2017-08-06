using CoreInvestmentApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreInvestmentApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InfoContentView : ContentView
	{
		public InfoContentView ()
		{
			InitializeComponent ();

            // Bind data to chart.
            pieChart.BindingContext = ViewModelLocator.OxyExData;

            circleImage.Source = "https://pbs.twimg.com/profile_images/476066629000785921/6D3DK65E_400x400.jpeg";
        }
	}
}