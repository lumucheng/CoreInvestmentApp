using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Pages;
using Xamarin.Forms;

namespace CoreInvestmentApp.SliderPages
{
    public class OpportunitiesPage : ContentPage
    {
        public OpportunitiesPage()
        {
            Util.RemoveCredentials();

            Application.Current.MainPage = new LoginPage();
        }
    }
}
