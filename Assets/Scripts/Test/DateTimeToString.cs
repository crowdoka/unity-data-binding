namespace TestMVVM
{
	using System;
	using System.Globalization;
	using Crowdoka.DataBinding;

	public class DateTimeToString : IAdapter<DateTime, string, DateTimeToStringAdapterOptions>
	{
		public string Convert(DateTime source, DateTimeToStringAdapterOptions options) => source.ToString(options?.Format, CultureInfo.InvariantCulture);

		public DateTime ConvertBack(string destination, DateTimeToStringAdapterOptions options) => DateTime.ParseExact(destination, options?.Format, CultureInfo.InvariantCulture);
	}
}