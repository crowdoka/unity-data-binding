namespace Crowdoka.DataBinding.Generated
{
	using System;
	using UniRx;
	using UnityEngine;

    /// <summary>
    ///     Generated on build to remove Reflection
    /// </summary>
    [RequireComponent(typeof(PropertyBinding))]
    public class RuntimePropertyBinding_SubViewModel_Name_TextMeshProUGUI_text: RuntimePropertyBindingBase
    {
        private PropertyBinding origin;

        [SerializeField, HideInInspector] private BindingDirection bindingWayType;

        [SerializeField, HideInInspector] private TestMVVM.SubViewModel source;

        
        

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
            source = (TestMVVM.SubViewModel)origin.Source.Target;
            
            
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
            
            srcWatcher = new StateWatcher<TestMVVM.SubViewModel>(source, o => o.Data.Name);
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
            var value = (System.String)objValue;
            destination.text = value;
            dstWatcher?.UpdateState();
        }

        private void DstStateChanged(object objValue)
        {
            var value = (System.String)objValue;
            source.Data.Name = value;
            srcWatcher?.UpdateState();
        }
    }
}