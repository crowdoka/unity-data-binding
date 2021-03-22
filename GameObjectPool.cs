namespace Crowdoka.DataBinding
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// 
	/// </summary>
	public class GameObjectPool
	{
		private GameObject prefab = null;
		private Transform container = null;
		private List<GameObject> pool = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="container"></param>
		/// <param name="initialSize"></param>
		public GameObjectPool(GameObject prefab, Transform container, int initialSize = 0)
		{
			this.prefab = prefab;
			this.container = container;
			pool = new List<GameObject>();

			for (var i = 0; i < initialSize; i++)
			{
				GameObject toPool = Object.Instantiate(this.prefab, this.container);
				toPool.SetActive(false);
				pool.Add(toPool);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public GameObject Get()
		{
			foreach (var pooled in pool)
			{
				if (pooled.activeSelf == false)
				{
					pooled.SetActive(true);
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
				pool[i].SetActive(false);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toRelease"></param>
		public void Release(GameObject toRelease)
		{
			foreach (var pooled in pool)
			{
				if (toRelease.Equals(pooled))
				{
					pooled.SetActive(false);
					return;
				}
			}
		}
	}
}
