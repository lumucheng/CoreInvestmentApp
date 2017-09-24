using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CoreInvestmentApp.Menu;
using CoreInvestmentApp.SliderPages;
using CoreInvestmentApp.Pages;

namespace CoreInvestmentApp
{
    public class RootPage : MasterDetailPage
    {
        public RootPage()
        {
            var menuPage = new MenuPage();
            menuPage.Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as Menu.MenuItem);

            Master = menuPage;
            Detail = new NavigationPage(new WatchlistPage())
            {
                BarBackgroundColor = Color.FromHex("#4B77BE"),
                BarTextColor = Color.White
            };
        }

        void NavigateTo(Menu.MenuItem menu)
        {
            Page displayPage = (Page)Activator.CreateInstance(menu.TargetType);

            Detail = new NavigationPage(displayPage)
            {
                BarBackgroundColor = Color.FromHex("#4B77BE"),
                BarTextColor = Color.White
            };

            IsPresented = false;
        }
    }
}
