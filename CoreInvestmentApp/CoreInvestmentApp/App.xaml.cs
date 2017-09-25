using CoreInvestmentApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreInvestmentApp.Model;
using Xamarin.Forms;
using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Pages;
using PCLCrypto;
using Realms;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace CoreInvestmentApp
{
    public static class ViewModelLocator
    {
        static StockViewModel stockVM;
        static OxyExData exData;

        public static StockViewModel StockViewModel => stockVM ?? (stockVM = new StockViewModel());
        public static OxyExData OxyExData = exData ?? (exData = new OxyExData());
    }

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if (Util.IsNetworkAvailable())
            {
                string email = Util.UserName;
                string p = Util.Password;

                if (email != null && p != null)
                {
                    // Placeholder Page
                    MainPage = new Page();
                    CheckAPI(email, p);
                }
                else
                {
                    MainPage = new LoginPage();
                }
            }
            else
            {
                MainPage = new LoginPage();
                UserDialogs.Instance.Alert("Please ensure you have a working connection", "Error", "OK");
            }
        }

        private async void CheckAPI(string email, string p)
        {
			HttpClient client = Util.HttpC;

			var values = new Dictionary<string, string>
			{
					{ "email", email },
					{ "p", p }
			};

			var content = new FormUrlEncodedContent(values);
			var response = await client.PostAsync(Util.CoreInvestUrlLogin, content);
			var responseString = await response.Content.ReadAsStringAsync();

			JObject jsonObject = JObject.Parse(responseString);

			if ((bool)jsonObject["status"])
			{
                MainPage = new RootPage();
			}
            else 
            {
                MainPage = new LoginPage();
            }
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
