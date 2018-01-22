using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CoreInvestmentApp.Classes;
using Newtonsoft.Json.Linq;
using PCLCrypto;
using Xamarin.Forms;
using Xamarin.Auth;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Plugin.FacebookClient;
using Plugin.FacebookClient.Abstractions;

namespace CoreInvestmentApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            ImageLogo.Source = ImageSource.FromFile("core_logo.png");
            ImageUsername.Source = ImageSource.FromFile("username.png");
            ImageLock.Source = ImageSource.FromFile("lock.png");
        }

        void Handle_LoginClickedAsync(object sender, System.EventArgs e)
        {
            if (Util.IsNetworkAvailable())
            {
                UserDialogs.Instance.ShowLoading("Loading..", MaskType.Black);
                CallLoginAPI().ContinueWith((task) =>
                {
                    UserDialogs.Instance.HideLoading();
                });
            }
            else
            {
                UserDialogs.Instance.Alert("Please ensure you have a working connection", "Error", "OK");
            }
        }

        void Handle_RegisterClicked(object sender, System.EventArgs e)
        {
            var options = new BrowserOptions
            {
                ChromeShowTitle = true,
                UseSafariReaderMode = false,
                UseSafariWebViewController = true
            };

            CrossShare.Current.OpenBrowser("http://www.coreinvest.me/register.php", options);
        }

        void Handle_FacebookClicked(object sender, System.EventArgs e)
        {
            if (Util.IsNetworkAvailable())
            {
                UserDialogs.Instance.ShowLoading("Loading..", MaskType.Black);
                CallFacebookLogin().ContinueWith((task) =>
                {
                    UserDialogs.Instance.HideLoading();
                });
            }
            else
            {
                UserDialogs.Instance.Alert("Please ensure you have a working connection", "Error", "OK");
            }
        }

        private async Task CallFacebookLogin()
        { 
            FacebookResponse<Dictionary<string, object>> result = await CrossFacebookClient.Current.RequestUserDataAsync(new string[] { "email", "first_name", "last_name" }, new string[] { "email" });

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
            var response = await client.PostAsync("http://www.coreinvest.me/facebook_login.php", content);
            var responseString = await response.Content.ReadAsStringAsync();

            JObject jsonObject = JObject.Parse(responseString);

            // Login success
            if ((bool)jsonObject["status"])
            {
                Util.AccessRights = (string)jsonObject["message"];
                Application.Current.MainPage = new RootPage();
            }
            else
            {
                string message = "An error occured on the server.";
                UserDialogs.Instance.Alert(message, "Error", "OK");
            }
        }

        private async Task CallLoginAPI()
        {
			string email = EntryEmail.Text.Trim();
			string p = EntryPassword.Text;

			var data = Encoding.UTF8.GetBytes(p);
			var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha512);
			byte[] hash = hasher.HashData(data);
			string hashHex = Util.ByteArrayToHex(hash).ToLower();

			HttpClient client = Util.HttpC;

			var values = new Dictionary<string, string>
			{
				{ "email", email },
				{ "p", hashHex }
			};

			var content = new FormUrlEncodedContent(values);
			var response = await client.PostAsync("http://www.coreinvest.me/login_api.php", content);
			var responseString = await response.Content.ReadAsStringAsync();

            JObject jsonObject = JObject.Parse(responseString);

			// Login success
			if ((bool)jsonObject["status"])
            {
                if (SwitchRemember.IsToggled)
                {
                    Util.SaveCredentials(email, hashHex);
                }

                Util.AccessRights = (string)jsonObject["message"];
				Application.Current.MainPage = new RootPage();
            }
            else
            {
                string message = "Wrong username or password.";
                UserDialogs.Instance.Alert(message, "Error", "OK");
            }
        }
    }
}
