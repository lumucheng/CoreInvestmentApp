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
            // Detail = new NavigationPage(new ContractsPage());
            Detail = new NavigationPage(new WatchlistPage());
        }

        void NavigateTo(Menu.MenuItem menu)
        {
            Page displayPage = (Page)Activator.CreateInstance(menu.TargetType);

            Detail = new NavigationPage(displayPage);

            IsPresented = false;
        }
    }
}
