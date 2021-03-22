namespace Crowdoka.DataBinding.Generated
{
	using System;
	using System.Collections;
	using System.Linq;
	using UniRx;
	using UnityEngine;

    /// <summary>
    ///     Generated on build to remove Reflection
    /// </summary>
    [RequireComponent(typeof(ListBinding))]
    public class RuntimeListBinding_Screen1ViewModel: RuntimeListBindingBase
    {
        private ListBinding origin;

        [SerializeField, HideInInspector] private TestMVVM.Screen1ViewModel source;

        [SerializeField, HideInInspector] private ViewModelCore prefab;
        [SerializeField, HideInInspector] private Transform container;

        private IWatcher srcWatcher;
        private IDisposable srcSubscription;

        private MonoBehaviourPool<ViewModelCore> pool = null;

        public void OnEnable()
        {
            Initialize();
        }

        public void OnDisable()
        {
            srcSubscription?.Dispose();
        }

        public void Initialize()
        {
            if (origin == null)
			{
                origin = GetComponent<ListBinding>();
			}

            source = (TestMVVM.Screen1ViewModel)origin.Source.Target;
            prefab = origin.Prefab;
            container = origin.Container;

            origin.enabled = false;

            pool = new MonoBehaviourPool<ViewModelCore>(prefab, container);

            CreateSrcStateWatcher();
        }

        private void CreateSrcStateWatcher()
        {
            srcWatcher = new StateWatcher<IEnumerable>(source.Data.TestList,
                data => data.OfType<object>()
                    .Select(item => item.GetHashCode())
                    .Aggregate(0, (int acc, int item) => unchecked(acc += acc * 314159 + item)));
            srcSubscription = srcWatcher.Watch().Subscribe(SrcStateChanged);
        }
        
        private void SrcStateChanged(object objValue)
        {
            pool.ReleaseAll();
            foreach (var element in source.Data.TestList)
			{
                var newObject = pool.Get();
                newObject.SetData(element);
			}
        }
    }
}