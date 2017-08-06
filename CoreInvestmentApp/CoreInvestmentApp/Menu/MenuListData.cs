using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CoreInvestmentApp.Pages;

namespace CoreInvestmentApp.Menu
{
    public class MenuListData : List<MenuItem>
    {
        public MenuListData()
        {
            this.Add(new MenuItem()
            {
                Title = "Home",
                IconSource = "contacts.png",
                TargetType = typeof(ContractsPage)
            });

            this.Add(new MenuItem()
            {
                Title = "Assesssment",
                IconSource = "leads.png",
                TargetType = typeof(LeadsPage)
            });

            this.Add(new MenuItem()
            {
                Title = "Payment",
                IconSource = "accounts.png",
                TargetType = typeof(AccountsPage)
            });

            this.Add(new MenuItem()
            {
                Title = "Logout",
                TargetType = typeof(OpportunitiesPage)
            });
        }
    }
}
