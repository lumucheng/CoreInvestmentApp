using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreInvestmentApp.Pages;
using Xamarin.Forms;

namespace CoreInvestmentApp.SliderPages
{
    public class OpportunitiesPage : ContentPage
    {
        public OpportunitiesPage()
        {
            Application.Current.MainPage = new LoginPage();
        }
    }
}
