namespace TestMVVM
{
	using Crowdoka.DataBinding;

	public class BoolToString : IAdapter<bool, string>
	{
		public string Convert(bool source) => source.ToString();

		public bool ConvertBack(string destination) => bool.Parse(destination);
	}
}