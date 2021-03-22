namespace TestMVVM
{
	using System.Collections.Generic;
	using Crowdoka.DataBinding;
	using TMPro;

	public class ListOfStringToListOfOptionData : IAdapter<List<string>, List<TMP_Dropdown.OptionData>>
	{
		public List<TMP_Dropdown.OptionData> Convert(List<string> source) => source.ConvertAll(input => new TMP_Dropdown.OptionData(input));

		public List<string> ConvertBack(List<TMP_Dropdown.OptionData> destination) => destination.ConvertAll(input => input.text);
	}
}