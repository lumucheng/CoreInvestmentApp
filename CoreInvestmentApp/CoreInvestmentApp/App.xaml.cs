using CoreInvestmentApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace CoreInvestmentApp
{
    public static class ViewModelLocator
    {
        static StockViewModel stockVM;
        public static StockViewModel StockViewModel
        => stockVM ?? (stockVM = new StockViewModel());
    }

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new RootPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
