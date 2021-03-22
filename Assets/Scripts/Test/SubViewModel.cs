namespace TestMVVM
{
	using Crowdoka.DataBinding;

	public class SubViewModel : ViewModelBase<SubModel>
	{
	}

	public class SubModel
	{
		public string Name { get; set; }
	}
}