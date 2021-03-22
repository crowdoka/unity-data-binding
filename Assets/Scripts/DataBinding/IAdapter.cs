namespace Crowdoka.DataBinding
{
	using UnityEngine;

	public interface IAdapter<TSource, TDestination>
	{
		TDestination Convert(TSource source);

		TSource ConvertBack(TDestination destination);
	}

	public interface IAdapter<TSource, TDestination, TAdapterOptions> where TAdapterOptions : AdapterOptionsBase
	{
		TDestination Convert(TSource source, TAdapterOptions options);

		TSource ConvertBack(TDestination destination, TAdapterOptions options);
	}
}
