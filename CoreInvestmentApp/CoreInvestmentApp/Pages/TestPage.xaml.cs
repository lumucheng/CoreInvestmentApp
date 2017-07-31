using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BottomBar.XamarinForms;

namespace CoreInvestmentApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPage : ContentPage
    {
        public TestPage()
        {
            InitializeComponent();

            BottomBarPage bottomBarPage = new BottomBarPage();

            string[] tabTitles = { "Favorites", "Friends", "Nearby", "Recents", "Restaurants" };
            string[] tabColors = { null, "#5D4037", "#7B1FA2", "#FF5252", "#FF9800" };
            int[] tabBadgeCounts = { 0, 1, 5, 3, 4 };
            string[] tabBadgeColors = { "#000000", "#FF0000", "#000000", "#000000", "#000000" };

            for (int i = 0; i < tabTitles.Length; ++i)
            {
                string title = tabTitles[i];
                string tabColor = tabColors[i];
                int tabBadgeCount = tabBadgeCounts[i];
                string tabBadgeColor = tabBadgeColors[i];

                FileImageSource icon = (FileImageSource)FileImageSource.FromFile(string.Format("ic_{0}.png", title.ToLowerInvariant()));

                // create tab page
                var tabPage = new TabPage()
                {
                    Title = title,
                    Icon = icon
                };

                // set tab color
                if (tabColor != null)
                {
                    BottomBarPageExtensions.SetTabColor(tabPage, Color.FromHex(tabColor));
                }

                // Set badges
                // BottomBarPageExtensions.SetBadgeCount(tabPage, tabBadgeCount);
                // BottomBarPageExtensions.SetBadgeColor(tabPage, Color.FromHex(tabBadgeColor));

                // set label based on title
                tabPage.UpdateLabel();

                // add tab pag to tab control
                bottomBarPage.Children.Add(tabPage);

                Content = bottomBarPage;
                // App.Current.MainPage = new NavigationPage(bottomBarPage);
            }
        }
    }
}