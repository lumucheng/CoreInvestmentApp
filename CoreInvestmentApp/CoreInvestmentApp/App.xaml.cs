using System.Collections.Generic;
using Xamarin.Forms;
using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Pages;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Acr.UserDialogs;
using Plugin.FacebookClient;
using System.Threading.Tasks;
using Plugin.FacebookClient.Abstractions;

namespace CoreInvestmentApp
{
    public partial class App : Application
    {
        IFacebookClient facebookClient;
        
        public App()
        {
            InitializeComponent();

            facebookClient = CrossFacebookClient.Current;

            if (Util.IsNetworkAvailable())
            {
                // Placeholder Page
                MainPage = new Page();

                // Check for Facebook Login first..
                if (facebookClient.IsLoggedIn) {
                    CallFacebookLogin();
                }
                else 
                {   // Check for Core Invest Account
                    string email = Util.UserName;
                    string p = Util.Password;

                    if (email != null && p != null)
                    {
                        CheckAPI(email, p);
                    }
                    else
                    {
                        MainPage = new LoginPage();
                    }
                }
            }
            else
            {
                MainPage = new LoginPage();
                UserDialogs.Instance.Alert("Please ensure you have a working connection", "Error", "OK");
            }
        }

        private async void CallFacebookLogin()
        {
            var result = await facebookClient.RequestUserDataAsync(new string[] { "email", "first_name", "last_name" }, new string[] { "email" }, FacebookPermissionType.Read);
            // FacebookResponse<Dictionary<string, object>> result 

            if (result.Data != null)
            {
                // Save to server
                string email = result.Data["email"].ToString();
                string first_name = result.Data["last_name"].ToString();
                string last_name = result.Data["first_name"].ToString();
                string user_id = result.Data["user_id"].ToString();

                await CallFacebookLoginAPI(email, first_name + last_name, user_id);
            }
            else
            {
                UserDialogs.Instance.Alert("Unable to login to Facebook.", "Error", "OK");
                MainPage = new LoginPage();
            }
        }

        private async Task CallFacebookLoginAPI(string email, string name, string user_id)
        {
            HttpClient client = Util.HttpC;

            var values = new Dictionary<string, string>
            {
                { "email", email },
                { "name", name } ,
                { "user_id", user_id }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(Util.CoreInvestUrlFacebookLogin, content);
            var responseString = await response.Content.ReadAsStringAsync();

            JObject jsonObject = JObject.Parse(responseString);

            // Login success
            if ((bool)jsonObject["status"])
            {
                Util.AccessRights = (string)jsonObject["message"];
                MainPage = new RootPage();
            }
            else
            {
                string message = "An error occured on the server.";
                UserDialogs.Instance.Alert(message, "Error", "OK");
                MainPage = new LoginPage();
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
                Util.AccessRights = (string)jsonObject["message"];
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
