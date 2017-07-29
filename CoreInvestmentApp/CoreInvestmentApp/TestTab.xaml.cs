using Messier16.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreInvestmentApp
{
    public class TestTab : PlatformTabbedPage
    {
        public TestTab()
        {
            InitializeComponent();
            BarBackgroundColor = App.BarBackgroundColors[3];
            SelectedColor = App.SelectedColors[0];
            BarBackgroundApplyTo = BarBackgroundApplyTo.None;

            Children.Add(new ConfigurationPage { Icon = "feed" });
            Children.Add(new BasicContentPage("YouTube") { Icon = "youtube" });
            Children.Add(new BasicContentPage("Twitter") { Icon = "twitter" });
            Children.Add(new BasicContentPage("Info") { Icon = "info" });
        }
    }
}