using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using Newtonsoft.Json.Linq;
using PCLCrypto;
using Xamarin.Forms;

namespace CoreInvestmentApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            ImageLogo.Source = ImageSource.FromFile("core_logo.jpg");
        }

        void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
			UserDialogs.Instance.ShowLoading("Loading..", MaskType.Black);
			CallAPI().ContinueWith((task) =>
			{
				UserDialogs.Instance.HideLoading();
			});
        }

        private async Task CallAPI()
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

            if ((bool)jsonObject["status"])
            {
                // Login success

                // Save to DB for future login
                // User user = new User();
                // user.Email = email;
                // user.P = hashHex;
                // Util.SaveUserToDB(user);

                Util.SaveCredentials(email, hashHex);

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
