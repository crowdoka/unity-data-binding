namespace TestMVVM
{
    using System;
	using Crowdoka.DataBinding;

	public class TestEnumToString : IAdapter<TestEnum, string>
	{
		public string Convert(TestEnum source) => source.ToString();

		public TestEnum ConvertBack(string destination) => (TestEnum)Enum.Parse(typeof(TestEnum), destination);
	}
}