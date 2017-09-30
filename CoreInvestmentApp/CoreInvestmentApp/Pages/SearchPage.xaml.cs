using CoreInvestmentApp.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using CoreInvestmentApp.Model;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Realms;
using Acr.UserDialogs;

namespace CoreInvestmentApp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchPage : ContentPage
	{
        private ObservableCollection<StockIdentifier> identifierList = new ObservableCollection<StockIdentifier>();
        public ObservableCollection<StockIdentifier> IdentifierList
        {
            get 
            {
                return identifierList;
            }
            set 
            {
                identifierList = value;
            }
        }
         
		public SearchPage ()
		{
			InitializeComponent ();
            Title = "Search";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Command = new Command(() => DismissPage()),
			});
		}

        private void DismissPage()
        {
            Navigation.PopModalAsync();
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);
            SearchAPI().ContinueWith((task) =>
            {
                UserDialogs.Instance.HideLoading();
            });
        }

        private async Task SearchAPI()
        {
            string query = EntrySearch.Text.ToUpper();
            query = query.Trim();

            HttpClient client = Util.GetAuthHttpClient();
            // var uri = new Uri(string.Format(Util.IntrinioAPIUrl + "/companies?query={0}", query));
            var uri = new Uri(string.Format(Util.IntrinioAPIUrl + "/companies?identifier={0}", query));

            if (Util.IsNetworkAvailable())
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(json);
                    StockIdentifier stockIdentifier = JsonConvert.DeserializeObject<StockIdentifier>(jObject.ToString());

                    if (stockIdentifier != null)
                    {
                        IdentifierList = new ObservableCollection<StockIdentifier>();
                        IdentifierList.Add(stockIdentifier);
                        StockIdentifierListView.ItemsSource = IdentifierList;
                    }
                    else
                    {
                        UserDialogs.Instance.Toast("No results found", TimeSpan.FromMilliseconds(1000));
                    }
                }
                else
                {
                    UserDialogs.Instance.Alert("No results found", null, "OK");
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Please ensure you have a working connection", "Error", "OK");
            }
        }

        private void StockIdentifierListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var vRealmDb = Realm.GetInstance();
            StockIdentifier stockIdentifier = (StockIdentifier)e.SelectedItem;
            Stock stock = new Stock();
            stock.StockIdentifier = stockIdentifier;

            string jsonObj = JsonConvert.SerializeObject(stock);
            RealmStockJson stockJson = new RealmStockJson();
            stockJson.JsonObjStr = jsonObj;
            stockJson.StockTicker = stockIdentifier.Ticker;

            vRealmDb.Write(() => {
                vRealmDb.Add(stockJson, true);
            });

            MessagingCenter.Send<string>("refresh", "refresh");
            Navigation.PopModalAsync();
        }
    }
}