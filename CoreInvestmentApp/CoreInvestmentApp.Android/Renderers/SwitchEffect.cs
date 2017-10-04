using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CoreInvestmentApp.Droid.Renderers;

[assembly: ResolutionGroupName("CoreInvestApp")]
[assembly: ExportEffect(typeof(SwitchEffect), "SwitchEffect")]
namespace CoreInvestmentApp.Droid.Renderers
{
    public class SwitchEffect : PlatformEffect
    {
        private Android.Support.V7.Widget.SwitchCompat _thisSwitch;

        protected override void OnAttached()
        {
            try
            {
                _thisSwitch = Control as Android.Support.V7.Widget.SwitchCompat;
                if (_thisSwitch != null)
                {
                    ChangeColor();
                    _thisSwitch.CheckedChange += MySwitch_CheckedChange;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot Set Checked Color");
            }
        }

        private void MySwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            ChangeColor();
        }

        protected override void OnDetached()
        {
            _thisSwitch.CheckedChange -= MySwitch_CheckedChange;
            _thisSwitch = null;
        }

        private void ChangeColor()
        {
            if (_thisSwitch.Checked)
            {
                _thisSwitch.ThumbDrawable.SetTint(Android.Graphics.Color.Green);
                _thisSwitch.TrackDrawable.SetTint(Android.Graphics.Color.LightGreen);
            }
            else
            {
                _thisSwitch.TrackDrawable.SetTint(Android.Graphics.Color.LightGray);
                _thisSwitch.ThumbDrawable.SetTint(Android.Graphics.Color.Gray);
            }
        }
    }
}