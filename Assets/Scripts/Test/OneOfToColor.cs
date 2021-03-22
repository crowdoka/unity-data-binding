namespace TestMVVM
{
    using System;
	using Crowdoka.DataBinding;
	using OneOf;
	using UnityEngine;

	public class OneOfToColor : IAdapter<OneOf<Color, string>, Color>
	{
		public Color Convert(OneOf<Color, string> source)
		{
			Color convertedColor = Color.white;
			source.Switch(
				color => convertedColor = color,
				htmlString => ColorUtility.TryParseHtmlString(htmlString, out convertedColor)
			);
			return convertedColor;
		}

		public OneOf<Color, string> ConvertBack(Color destination)
		{
			return destination;
		}
	}
}