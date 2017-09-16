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

			var vRealmDb = Realm.GetInstance();
            var vUser = vRealmDb.All<User>().FirstOrDefault();

            if (vUser != null)
            {
                // Placeholder Page
                MainPage = new Page();
                CheckAPI(vUser);
            }
            else 
            {
                MainPage = new LoginPage();
            }
        }

        private async void CheckAPI(User user)
        {
			HttpClient client = Util.HttpC;

			var values = new Dictionary<string, string>
			{
					{ "email", user.Email },
					{ "p", user.P }
			};

			var content = new FormUrlEncodedContent(values);
			var response = await client.PostAsync("http://www.coreinvest.me/login_api.php", content);
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
