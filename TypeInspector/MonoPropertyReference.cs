using System;
using System.Reflection;
using Crowdoka.DataBinding;

namespace TypeInspector
{
    /// <summary>
    ///     Make property reference for specific MonoBehaviour target object
    /// </summary>
    [Serializable]
    public class MonoPropertyReference : PropertyReferenceBase
    {
        public UnityEngine.Object Target;
        public string PropertyName;


        public void Set(object data)
        {
            Set(Target, data);
        }

        public object Get()
        {
            return Get(Target);
        }

        public override Type GetSourceType()
        {
            return Target.GetType();
        }

        public override PropertyInfo GetProperty()
        {
            if (Target == null || string.IsNullOrEmpty(PropertyName))
            {
                return null;
            }

            if (Target.GetType().BaseType != null && Target.GetType().BaseType.IsGenericType && Target.GetType().BaseType.GetGenericTypeDefinition() == typeof(ViewModelBase<>))
			{
                return Target.GetType().BaseType.GetGenericArguments()[0].GetProperty(PropertyName);
            }

            return Target.GetType().GetProperty(PropertyName);
        }
    }
}