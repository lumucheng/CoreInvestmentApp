using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SegmentedControl.FormsPlugin.Android;
using OxyPlot.Xamarin.Forms.Platform.Android;
using ImageCircle.Forms.Plugin.Droid;

namespace CoreInvestmentApp.Droid
{
    [Activity(Label = "CoreInvestmentApp", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            SegmentedControlRenderer.Init();
            PlotViewRenderer.Init();
            ImageCircleRenderer.Init();

            LoadApplication(new App());
        }
    }
}

