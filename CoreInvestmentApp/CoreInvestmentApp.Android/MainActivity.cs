using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using OxyPlot.Xamarin.Forms.Platform.Android;
using ImageCircle.Forms.Plugin.Droid;
using Messier16.Forms.Android.Controls;
using Acr.UserDialogs;

namespace CoreInvestmentApp.Droid
{
    [Activity(Label = "Core Invest", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            SegmentedControl.FormsPlugin.Android.SegmentedControlRenderer.Init();
            PlotViewRenderer.Init();
            ImageCircleRenderer.Init();
            Messier16Controls.InitAll();
            UserDialogs.Init(this);

            LoadApplication(new App());
        }
    }
}

