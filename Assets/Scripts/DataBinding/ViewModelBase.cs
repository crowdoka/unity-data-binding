namespace Crowdoka.DataBinding
{
	using UnityEngine;

	public abstract class ViewModelBase<T> : ViewModelCore where T : new()
	{
		public T Data { get; set; } = new T();

		public override void SetData(object data)
		{
			Data = (T)data;
		}
	}
}