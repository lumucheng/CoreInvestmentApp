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
using FormsCommunityToolkit.Effects.Droid;
using Plugin.FacebookClient;
using Android.Content;
using Plugin.InAppBilling;

namespace CoreInvestmentApp.Droid
{
    [Activity(Label = "Core Invest", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            FacebookClientManager.Initialize(this);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            SegmentedControl.FormsPlugin.Android.SegmentedControlRenderer.Init();
            PlotViewRenderer.Init();
            ImageCircleRenderer.Init();
            Messier16Controls.InitAll();
            UserDialogs.Init(this);
            Effects.Init();

            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            FacebookClientManager.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }
    }
}

