﻿using CoreInvestmentApp.Classes;
using CoreInvestmentApp.iOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CurvedCornersLabel), typeof(CurvedCornersLabelRenderer))]
namespace CoreInvestmentApp.iOS
{
    public class CurvedCornersLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var _xfViewReference = (CurvedCornersLabel)Element;
                Paint(_xfViewReference);
            }
        }

        private void Paint(CurvedCornersLabel view)
        {
            this.Layer.CornerRadius = (float)view.CurvedCornerRadius;
            this.Layer.BackgroundColor = view.CurvedBackgroundColor.ToCGColor();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // re-paint if these properties change at runtime
            if (e.PropertyName == CurvedCornersLabel.BackgroundColorProperty.PropertyName ||
                e.PropertyName == CurvedCornersLabel.CurvedCornerRadiusProperty.PropertyName)
            {
                if (Element != null)
                {
                    Paint((CurvedCornersLabel)Element);
                }
            }
        }
    }
}
