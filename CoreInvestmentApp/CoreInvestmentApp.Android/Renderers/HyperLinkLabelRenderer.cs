using System;
using Android.Text.Util;
using Android.Util;
using Android.Widget;
using CoreInvestmentApp.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HyperlinkLabel), typeof(CoreInvestmentApp.Droid.Renderers.HyperLinkLabelRenderer))]
namespace CoreInvestmentApp.Droid.Renderers
{
	public class HyperLinkLabelRenderer : LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			var view = (HyperlinkLabel)Element;
			if (view == null) return;

			TextView textView = new TextView(Forms.Context);
			textView.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
			textView.SetTextColor(view.TextColor.ToAndroid());

			// Setting the auto link mask to capture all types of link-able data
			textView.AutoLinkMask = MatchOptions.All;
			// Make sure to set text after setting the mask
			textView.Text = view.Text;
			textView.SetTextSize(ComplexUnitType.Dip, (float)view.FontSize);

			// overriding Xamarin Forms Label and replace with our native control
			SetNativeControl(textView);
		}
	}
}
