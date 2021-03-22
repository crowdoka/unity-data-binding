namespace TestMVVM
{
	using Crowdoka.DataBinding;
	using UnityEngine;

	[CreateAssetMenu(menuName = "Data Binding/Adapter options/DateTime to string adapter options")]
	public class DateTimeToStringAdapterOptions : AdapterOptionsBase
	{
		public string Format = "YYYY/MM/DD";
	}
}