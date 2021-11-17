using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PursiX.Behaviors
{
	//http://xamaringuyshow.com/2018/09/26/xamarin-forms-shadow-effect-tutorial-38/
	//*******************************************************************************************
	//PursiX.Android -> LabelShadowEffect.cs

	public class ShadowEffect : RoutingEffect
	{
		public float Radius { get; set; }

		public Color Color { get; set; }

		public float DistanceX { get; set; }

		public float DistanceY { get; set; }

		public ShadowEffect() : base("PursiX.LabelShadowEffect")
        {

        }
	}
}
