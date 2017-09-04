using CoreInvestmentApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
				Text = "Done",
				Command = new Command(() => Navigation.PopModalAsync()),
			});
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
            string query = EntrySearch.Text;
            query = query.Trim();

            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(string.Format(Util.IntrinioAPIUrl + "/companies?query={0}", query));
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);
                JArray data = (JArray)jsonObject["data"];
                IdentifierList = JsonConvert.DeserializeObject<ObservableCollection<StockIdentifier>>(data.ToString());

                if (IdentifierList.Count > 0)
                {
                    StockIdentifierListView.ItemsSource = IdentifierList;
                }
                else
                {
                    UserDialogs.Instance.Toast("No results found", TimeSpan.FromMilliseconds(1000));
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Something went wrong with the network", "Error", "OK");
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

            Navigation.PopModalAsync();
        }
    }
}