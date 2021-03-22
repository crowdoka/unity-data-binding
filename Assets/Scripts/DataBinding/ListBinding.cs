namespace Crowdoka.DataBinding
{
    using System;
	using System.Collections;
	using System.Linq;
	using System.Reflection;
    using TypeInspector;
    using UniRx;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor.Callbacks;
	using System.Text;
#endif

    /// <summary>
    ///     Binding for source is a list of ViewModelCore
    /// </summary>
    [DisallowMultipleComponent]
    public class ListBinding : BindingBase
    {
        [SerializeField]
        [TargetTypeFilter(typeof(ViewModelCore))]
        [TypeFilter(nameof(FilterSourceTypes))]
        [PropertyFilter(nameof(FilterSourceProperties))]
        public MonoPropertyReference Source;

        [SerializeField]
        public ViewModelCore Prefab;
        [SerializeField]
        public Transform Container;

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

        public bool IsValid()
        {
            return Source != null && Source.IsValid();
        }

        public void Initialize()
        {
            if (!IsValid())
            {
                enabled = false;
                Debug.LogError("State is not valid!", this);
                return;
            }

            pool = new MonoBehaviourPool<ViewModelCore>(Prefab, Container);

            CreateSrcStateWatcher();
        }

        private void CreateSrcStateWatcher()
        {
            var enumerable = Source.Get() as IEnumerable;
            if (enumerable == null)
            {
                return;
            }

            srcWatcher = new StateWatcher<IEnumerable>(enumerable,
                data => data.OfType<object>()
                    .Select(item => item.GetHashCode())
                    .Aggregate(0, (int acc, int item) => unchecked(acc += acc * 314159 + item)));
            srcSubscription = srcWatcher.Watch().Subscribe(SrcStateChanged);
        }
        
        private void SrcStateChanged(object value)
        {
            var enumerable = Source.Get() as IEnumerable;
            if (enumerable == null)
            {
                return;
            }

            pool.ReleaseAll();
            foreach (var element in enumerable)
			{
                var newObject = pool.Get();
                newObject.SetData(element);
			}
        }

        private bool FilterSourceTypes(Type type)
        {
            return typeof(ViewModelCore).IsAssignableFrom(type);
        }

        private bool FilterSourceProperties(PropertyInfo property)
        {
            return property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable));
        }

#if UNITY_EDITOR
		protected override string GenerateClass()
		{
			var className = GetRuntimeClassName();

			var generatedClass = $@"namespace Crowdoka.DataBinding.Generated
{{
	using System;
	using System.Collections;
	using System.Linq;
	using UniRx;
	using UnityEngine;

    /// <summary>
    ///     Generated on build to remove Reflection
    /// </summary>
    [RequireComponent(typeof(ListBinding))]
    public class {className}: RuntimeListBindingBase
    {{
        private ListBinding origin;

        [SerializeField, HideInInspector] private {Source.GetSourceType().FullName} source;

        [SerializeField, HideInInspector] private ViewModelCore prefab;
        [SerializeField, HideInInspector] private Transform container;

        private IWatcher srcWatcher;
        private IDisposable srcSubscription;

        private MonoBehaviourPool<ViewModelCore> pool = null;

        public void OnEnable()
        {{
            Initialize();
        }}

        public void OnDisable()
        {{
            srcSubscription?.Dispose();
        }}

        public void Initialize()
        {{
            if (origin == null)
			{{
                origin = GetComponent<ListBinding>();
			}}

            source = ({Source.GetSourceType().FullName})origin.Source.Target;
            prefab = origin.Prefab;
            container = origin.Container;

            origin.enabled = false;

            pool = new MonoBehaviourPool<ViewModelCore>(prefab, container);

            CreateSrcStateWatcher();
        }}

        private void CreateSrcStateWatcher()
        {{
            srcWatcher = new StateWatcher<IEnumerable>(source.Data.{Source.PropertyName},
                data => data.OfType<object>()
                    .Select(item => item.GetHashCode())
                    .Aggregate(0, (int acc, int item) => unchecked(acc += acc * 314159 + item)));
            srcSubscription = srcWatcher.Watch().Subscribe(SrcStateChanged);
        }}
        
        private void SrcStateChanged(object objValue)
        {{
            pool.ReleaseAll();
            foreach (var element in source.Data.{Source.PropertyName})
			{{
                var newObject = pool.Get();
                newObject.SetData(element);
			}}
        }}
    }}
}}";
			return generatedClass;
		}

        protected override string GetRuntimeClassName()
        {
            var className = new StringBuilder();
            className.Append($"RuntimeListBinding");
            className.Append($"_{Source.GetSourceType().Name}");

            return className.ToString();
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            OnScriptsReloaded<ListBinding, RuntimeListBindingBase>();
        }
#endif
    }
}