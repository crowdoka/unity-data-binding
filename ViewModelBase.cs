namespace Crowdoka.DataBinding
{
	using UnityEngine;

	public abstract class ViewModelBase<T> : ViewModelCore
	{
		public T Data { get; set; }

		public override void SetData(object data)
		{
			Data = (T)data;
		}
	}
}
