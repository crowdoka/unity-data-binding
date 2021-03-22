namespace TestMVVM
{
	using System;
	using Crowdoka.DataBinding;

	public partial class TestObjectToString : IAdapter<TestObject, string>
	{
		public string Convert(TestObject source) => source.Name;

		public TestObject ConvertBack(string destination) => new TestObject
		{
			Name = destination
		};
	}
}