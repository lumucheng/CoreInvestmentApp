﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CoreInvestmentApp.SliderPages;
using CoreInvestmentApp.Pages;

namespace CoreInvestmentApp.Menu
{
    public class MenuListData : List<MenuItem>
    {
        public MenuListData()
        {
            this.Add(new MenuItem()
            {
                Title = "Watchlist",
                IconSource = "contacts.png",
                TargetType = typeof(WatchlistPage)
            });

            this.Add(new MenuItem()
            {
                Title = "Portfolio",
                IconSource = "leads.png",
                TargetType = typeof(PortfolioPage)
            });

            this.Add(new MenuItem()
            {
                Title = "Feedback",
                TargetType = typeof(FeedbackPage)
            });

            this.Add(new MenuItem()
            {
                Title = "Logout",
                TargetType = typeof(OpportunitiesPage)
            });
        }
    }
}
