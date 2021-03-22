namespace Crowdoka.DataBinding
{
    using System;
	using System.Collections.Generic;
	using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;
	using NaughtyAttributes;
	using TypeInspector;
    using UniRx;
    using UnityEngine;

#if UNITY_EDITOR
    using System.Text;
	using UnityEditor.Callbacks;
#endif

    /// <summary>
    ///     Binding for source and destination is MonoBehaviour
    /// </summary>
    public class PropertyBinding : BindingBase
    {
		[SerializeField]
        public BindingDirection BindingWayType = BindingDirection.BothWays;
        
        [SerializeField]
        [TargetTypeFilter(typeof(ViewModelCore))]
        [TypeFilter(nameof(FilterSourceTypes))]
        [OnValueChanged(nameof(OnSourceChanged))]
        public MonoPropertyReference Source;

		[SerializeField]
        [TypeFilter(nameof(FilterAdapterTypes))]
        [OnValueChanged(nameof(OnAdapterChanged))]
        public TypeReference Adapter;

		[SerializeField]
        [ValidateInput(nameof(ValidateAdapterOptionsTypes))]
        [ShowIf(nameof(AdapterOptionsVisibility))]
        public AdapterOptionsBase AdapterOptions;

		[SerializeField]
        [PropertyFilter(nameof(FilterDestinationProperties))]
        public MonoPropertyReference Destination;

        private IWatcher dstWatcher;
        private IWatcher srcWatcher;

        private IDisposable srcSubscription;
        private IDisposable dstSubscription;

        public void OnEnable()
        {
            Initialize();
        }

        public void OnDisable()
        {
            srcSubscription?.Dispose();
            dstSubscription?.Dispose();
        }

        public bool IsValid()
        {
            return Source != null && Source.IsValid() && Destination != null && Destination.IsValid();
        }

        public void Initialize()
        {
            if (!IsValid())
            {
                enabled = false;
                Debug.LogError("State is not valid!", this);
                return;
            }
            
            CreateSrcStateWatcher();
            CreateDstStateWatcher();
        }

        private void CreateSrcStateWatcher()
        {
            if (!BindingWayType.HasFlag(BindingDirection.SourceToDestination))
            {
                return;
            }
            
            srcWatcher = new StateWatcher<MonoPropertyReference>(Source, o => o.Get());
            srcSubscription = srcWatcher.Watch().Subscribe(SrcStateChanged);
        }

        private void CreateDstStateWatcher()
        {
            if (!BindingWayType.HasFlag(BindingDirection.DestinationToSource))
            {
                return;
            }

            dstWatcher = new StateWatcher<MonoPropertyReference>(Destination, o => o.Get());
            dstSubscription = dstWatcher.Watch().Subscribe(DstStateChanged);
        }
        
        private void SrcStateChanged(object value)
        {
            if (Adapter == null || Adapter.Get() == null)
            {
                Destination.Set(value);
            }
            else
            {
                var adapter = FormatterServices.GetUninitializedObject(this.Adapter.Get());
                var parameters = new List<object>() { value };
                if (this.Adapter.Get().GetInterface(typeof(IAdapter<,,>).Name) != null)
                {
                    parameters.Add(AdapterOptions);
                }
				Destination.Set(this.Adapter.Get().GetMethod("Convert").Invoke(adapter, parameters.ToArray()));
            }
            dstWatcher?.UpdateState();
        }

        private void DstStateChanged(object value)
        {
            if (Adapter == null || Adapter.Get() == null)
            {
                Source.Set(value);
            }
            else
            {
                var adapter = FormatterServices.GetUninitializedObject(this.Adapter.Get());
                var parameters = new List<object>() { value };
                if (this.Adapter.Get().GetInterface(typeof(IAdapter<,,>).Name) != null)
				{
                    parameters.Add(AdapterOptions);
				}
				Source.Set(this.Adapter.Get().GetMethod("ConvertBack").Invoke(adapter, parameters.ToArray()));
            }
            srcWatcher?.UpdateState();
        }

        private void OnSourceChanged()
        {
            Adapter = null;
            AdapterOptions = null;
            Destination = null;
        }

        private void OnAdapterChanged()
        {
            AdapterOptions = null;
        }

        private bool AdapterOptionsVisibility()
        {
            if (Adapter == null || Adapter.Get() == null || Adapter.Get().GetInterface(typeof(IAdapter<,,>).Name) == null)
            {
                return false;
            }

            return true;
        }

        private bool FilterSourceTypes(Type type)
        {
            return typeof(ViewModelCore).IsAssignableFrom(type);
        }

        private bool FilterAdapterTypes(Type type)
        {
            if (Source == null || Source.GetProperty() == null)
            {
                return false;
            }

            return (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAdapter<,>))
                && HasConversionOperator(Source.GetProperty().PropertyType, type.GetInterface(typeof(IAdapter<,>).Name).GenericTypeArguments[0]))
                ||
                (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAdapter<,,>))
                && HasConversionOperator(Source.GetProperty().PropertyType, type.GetInterface(typeof(IAdapter<,,>).Name).GenericTypeArguments[0]));
        }

        private bool ValidateAdapterOptionsTypes(AdapterOptionsBase adapterOptions)
        {
            if (adapterOptions == null || Adapter == null || Adapter.Get() == null || Adapter.Get().GetInterface(typeof(IAdapter<,,>).Name) == null)
            {
                this.AdapterOptions = null;
                return true;
			}

            return adapterOptions.GetType() == Adapter.Get().GetInterface(typeof(IAdapter<,,>).Name).GenericTypeArguments[2];
        }

        private bool FilterDestinationProperties(PropertyInfo property)
        {
            if (Source == null || Source.GetProperty() == null)
            {
                return false;
            }

            var typeToCheck = Adapter == null || Adapter.Get() == null ?
                Source.GetProperty().PropertyType :
                Adapter.Get().GetInterface(typeof(IAdapter<,>).Name) != null ?
                Adapter.Get().GetInterface(typeof(IAdapter<,>).Name).GenericTypeArguments[1] :
                Adapter.Get().GetInterface(typeof(IAdapter<,,>).Name).GenericTypeArguments[1];

            return HasConversionOperator(typeToCheck, property.PropertyType);
        }

        public static bool HasConversionOperator(Type from, Type to)
        {
            Func<Expression, UnaryExpression> bodyFunction = body => Expression.Convert(body, to);
            ParameterExpression inp = Expression.Parameter(from, "inp");
            try
            {
                // If this succeeds then we can cast 'from' type to 'to' type using implicit coercion
                Expression.Lambda(bodyFunction(inp), inp).Compile();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

#if UNITY_EDITOR

		protected override string GenerateClass()
		{
			var className = GetRuntimeClassName();

			var sourceSetter = new StringBuilder();
			var destinationSetter = new StringBuilder();

			var adapterDefinition = new StringBuilder();
			var adapterInitialisation = new StringBuilder();

			var adapterOptionsDefinition = new StringBuilder();
			var adapterOptionsInitialisation = new StringBuilder();

			if (Adapter != null && Adapter.Get() != null)
			{
				if (Adapter.Get().GetInterface(typeof(IAdapter<,,>).Name) != null)
				{
					sourceSetter.Append("adapter.ConvertBack(value, adapterOptions)");
					destinationSetter.Append("adapter.Convert(value, adapterOptions)");

					if (AdapterOptions != null)
					{
						adapterOptionsDefinition.Append($"[SerializeField, HideInInspector] private {AdapterOptions.GetType().FullName} adapterOptions;");
						adapterOptionsInitialisation.Append($"adapterOptions = ({AdapterOptions.GetType().FullName})origin.AdapterOptions;");
					}
				}
				else
				{
					sourceSetter.Append("adapter.ConvertBack(value)");
					destinationSetter.Append("adapter.Convert(value)");
				}

				if (Adapter != null && Adapter.Get() != null)
				{
					adapterDefinition.Append($"private {Adapter.Get().FullName} adapter;");
					adapterInitialisation.Append($"adapter = new {Adapter.Get().FullName}();");
				}
			}
			else
			{
				sourceSetter.Append("value");
				destinationSetter.Append("value");
			}

			var generatedClass = $@"namespace Crowdoka.DataBinding.Generated
{{
	using System;
	using UniRx;
	using UnityEngine;

    /// <summary>
    ///     Generated on build to remove Reflection
    /// </summary>
    [RequireComponent(typeof(PropertyBinding))]
    public class {className}: RuntimePropertyBindingBase
    {{
        private PropertyBinding origin;

        [SerializeField, HideInInspector] private BindingDirection bindingWayType;

        [SerializeField, HideInInspector] private {Source.GetSourceType().FullName} source;

        {adapterDefinition}
        {adapterOptionsDefinition}

        [SerializeField, HideInInspector] private {Destination.GetSourceType().FullName} destination;

        private IWatcher dstWatcher;
        private IWatcher srcWatcher;

        private IDisposable srcSubscription;
        private IDisposable dstSubscription;

        public void OnEnable()
        {{
            Initialize();
        }}

        public void OnDisable()
        {{
            srcSubscription?.Dispose();
            dstSubscription?.Dispose();
        }}

        public void Initialize()
        {{
            if (origin == null)
			{{
                origin = GetComponent<PropertyBinding>();
			}}

            bindingWayType = origin.BindingWayType;
            source = ({Source.GetSourceType().FullName})origin.Source.Target;
            {adapterInitialisation}
            {adapterOptionsInitialisation}
            destination = ({Destination.GetSourceType().FullName})origin.Destination.Target;

            origin.enabled = false;

            CreateSrcStateWatcher();
            CreateDstStateWatcher();
        }}

        private void CreateSrcStateWatcher()
        {{
            if (!bindingWayType.HasFlag(BindingDirection.SourceToDestination))
            {{
                return;
            }}
            
            srcWatcher = new StateWatcher<{Source.GetSourceType().FullName}>(source, o => o.Data.{Source.PropertyName});
            srcSubscription = srcWatcher.Watch().Subscribe(SrcStateChanged);
        }}

        private void CreateDstStateWatcher()
        {{
            if (!bindingWayType.HasFlag(BindingDirection.DestinationToSource))
            {{
                return;
            }}

            dstWatcher = new StateWatcher<{Destination.GetSourceType().FullName}>(destination, o => o.{Destination.PropertyName});
            dstSubscription = dstWatcher.Watch().Subscribe(DstStateChanged);
        }}
        
        private void SrcStateChanged(object objValue)
        {{
            var value = ({Source.GetProperty().PropertyType.FullName})objValue;
            destination.{Destination.PropertyName} = {destinationSetter};
            dstWatcher?.UpdateState();
        }}

        private void DstStateChanged(object objValue)
        {{
            var value = ({Destination.GetProperty().PropertyType.FullName})objValue;
            source.Data.{Source.PropertyName} = {sourceSetter};
            srcWatcher?.UpdateState();
        }}
    }}
}}";
			return generatedClass;
		}

        protected override string GetRuntimeClassName()
        {
            var className = new StringBuilder();
            className.Append($"RuntimePropertyBinding");
            className.Append($"_{Source.GetSourceType().Name}");
            className.Append($"_{Source.GetProperty().Name}");

            if (Adapter != null && Adapter.Get() != null)
            {
                className.Append($"_{Adapter.Get().Name}");
                if (Adapter.Get().GetInterface(typeof(IAdapter<,,>).Name) != null)
                {
                    className.Append($"_{AdapterOptions.GetType().Name}");
                }
            }

            className.Append($"_{Destination.GetSourceType().Name}");
            className.Append($"_{Destination.GetProperty().Name}");

            return className.ToString();
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            OnScriptsReloaded<PropertyBinding, RuntimePropertyBindingBase>();
        }
#endif
    }
}