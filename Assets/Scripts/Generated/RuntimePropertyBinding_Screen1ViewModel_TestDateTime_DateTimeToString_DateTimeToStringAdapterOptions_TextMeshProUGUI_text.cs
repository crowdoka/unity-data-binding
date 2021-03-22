namespace Crowdoka.DataBinding.Generated
{
	using System;
	using UniRx;
	using UnityEngine;

    /// <summary>
    ///     Generated on build to remove Reflection
    /// </summary>
    [RequireComponent(typeof(PropertyBinding))]
    public class RuntimePropertyBinding_Screen1ViewModel_TestDateTime_DateTimeToString_DateTimeToStringAdapterOptions_TextMeshProUGUI_text: RuntimePropertyBindingBase
    {
        private PropertyBinding origin;

        [SerializeField, HideInInspector] private BindingDirection bindingWayType;

        [SerializeField, HideInInspector] private TestMVVM.Screen1ViewModel source;

        private TestMVVM.DateTimeToString adapter;
        [SerializeField, HideInInspector] private TestMVVM.DateTimeToStringAdapterOptions adapterOptions;

        [SerializeField, HideInInspector] private TMPro.TextMeshProUGUI destination;

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

        public void Initialize()
        {
            if (origin == null)
			{
                origin = GetComponent<PropertyBinding>();
			}

            bindingWayType = origin.BindingWayType;
            source = (TestMVVM.Screen1ViewModel)origin.Source.Target;
            adapter = new TestMVVM.DateTimeToString();
            adapterOptions = (TestMVVM.DateTimeToStringAdapterOptions)origin.AdapterOptions;
            destination = (TMPro.TextMeshProUGUI)origin.Destination.Target;

            origin.enabled = false;

            CreateSrcStateWatcher();
            CreateDstStateWatcher();
        }

        private void CreateSrcStateWatcher()
        {
            if (!bindingWayType.HasFlag(BindingDirection.SourceToDestination))
            {
                return;
            }
            
            srcWatcher = new StateWatcher<TestMVVM.Screen1ViewModel>(source, o => o.Data.TestDateTime);
            srcSubscription = srcWatcher.Watch().Subscribe(SrcStateChanged);
        }

        private void CreateDstStateWatcher()
        {
            if (!bindingWayType.HasFlag(BindingDirection.DestinationToSource))
            {
                return;
            }

            dstWatcher = new StateWatcher<TMPro.TextMeshProUGUI>(destination, o => o.text);
            dstSubscription = dstWatcher.Watch().Subscribe(DstStateChanged);
        }
        
        private void SrcStateChanged(object objValue)
        {
            var value = (System.DateTime)objValue;
            destination.text = adapter.Convert(value, adapterOptions);
            dstWatcher?.UpdateState();
        }

        private void DstStateChanged(object objValue)
        {
            var value = (System.String)objValue;
            source.Data.TestDateTime = adapter.ConvertBack(value, adapterOptions);
            srcWatcher?.UpdateState();
        }
    }
}