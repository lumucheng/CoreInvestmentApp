using CoreInvestmentApp.Model;
using CoreInvestmentApp.ViewModel;
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
    public partial class Watchlist : ContentPage
    {
        public Watchlist()
        {
            InitializeComponent();
            BindingContext = new StockViewModel();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Pass in data here.
            Navigation.PushAsync(new DetailedStockPage());
        }
    }
}