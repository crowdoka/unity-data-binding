namespace Crowdoka.DataBinding
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MonoBehaviourPool<T> where T : MonoBehaviour
	{
		private T prefab = null;
		private Transform container = null;
		private List<T> pool = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="container"></param>
		/// <param name="initialSize"></param>
		public MonoBehaviourPool(T prefab, Transform container, int initialSize = 0)
		{
			this.prefab = prefab;
			this.container = container;
			pool = new List<T>();

			for (var i = 0; i < initialSize; i++)
			{
				var toPool = Object.Instantiate(this.prefab, this.container);
				toPool.gameObject.SetActive(false);
				pool.Add(toPool);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public T Get()
		{
			foreach (var pooled in pool)
			{
				if (pooled.gameObject.activeSelf == false)
				{
					pooled.gameObject.SetActive(true);
					return pooled;
				}
			}

			var toPool = Object.Instantiate(prefab, container);
			pool.Add(toPool);

			return toPool;
		}

		/// <summary>
		/// 
		/// </summary>
		public void ReleaseAll()
		{
			ReleaseRange();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromIndex"></param>
		/// <param name="toIndex"></param>
		public void ReleaseRange(int fromIndex = 0, int toIndex = int.MaxValue)
		{
			for (var i = Mathf.Max(fromIndex, 0); i < Mathf.Min(toIndex, pool.Count); i++)
			{
				pool[i].gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toRelease"></param>
		public void Release(T toRelease)
		{
			foreach (var pooled in pool)
			{
				if (toRelease.Equals(pooled))
				{
					pooled.gameObject.SetActive(false);
					return;
				}
			}
		}
	}
}
