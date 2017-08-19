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
using Json.n

namespace CoreInvestmentApp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchPage : ContentPage
	{
		public SearchPage ()
		{
			InitializeComponent ();
		}

        private async Task Entry_Completed(object sender, EventArgs e)
        {
            string query = EntrySearch.Text;
            query = query.Trim();

            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(string.Format(Util.IntrinioAPIUrl + "/companies?query={0}", query));

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();


            }
        }
    }
}