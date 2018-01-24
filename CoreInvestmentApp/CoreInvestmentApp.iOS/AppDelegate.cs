using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using OxyPlot.Xamarin.Forms.Platform.iOS;
using ImageCircle.Forms.Plugin.iOS;
using Messier16.Forms.iOS.Controls;
using FormsCommunityToolkit.Effects.iOS;
using Plugin.FacebookClient;

namespace CoreInvestmentApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            SegmentedControl.FormsPlugin.iOS.SegmentedControlRenderer.Init();
            PlotViewRenderer.Init();
            ImageCircleRenderer.Init();
            Messier16Controls.InitAll();
            Effects.Init();
            FacebookClientManager.Initialize(app, options);

			UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
			UIApplication.SharedApplication.SetStatusBarHidden(false, false);
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override void OnActivated(UIApplication app)
        {
            FacebookClientManager.OnActivated();
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return FacebookClientManager.OpenUrl(app, url, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return FacebookClientManager.OpenUrl(application, url, sourceApplication, annotation);
        }
    }
}
