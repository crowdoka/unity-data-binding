using System;
using System.Reflection;
using Crowdoka.DataBinding;

namespace TypeInspector
{
    /// <summary>
    ///     Abstract class for make property referencies in editor
    /// </summary>
    public abstract class PropertyReferenceBase
    {
        protected PropertyInfo propertyCache;
        
        public abstract Type GetSourceType();
        public abstract PropertyInfo GetProperty();
        
        /// <summary>
        ///     Setup value to target property
        /// </summary>
        /// <param name="instance"> instance where is target property </param>
        /// <param name="data"> value which will set to target property </param>
        public void Set(object instance, object data)
        {
            Init();
            
            var property = GetProperty();

            if (property == null)
            {
                return;
            }

            property.SetValue(instance, data);
        }
        
        /// <summary>
        ///     Get current value from target property from 'instance' object
        /// </summary>
        /// <param name="instance"> insatnce of target object which contain target propetry </param>
        /// <returns></returns>
        public object Get(object instance)
        {
            Init();

            var obj = instance;
            if (instance.GetType().BaseType != null && instance.GetType().BaseType.IsGenericType && instance.GetType().BaseType.GetGenericTypeDefinition() == typeof(ViewModelBase<>))
            {
                obj = instance.GetType().GetProperty("Data").GetValue(instance);
            }

            return GetProperty()?.GetValue(obj);
        }
        
        /// <summary>
        ///     Make initialization
        /// </summary>
        public void Init()
        {
            
#if CAN_USE_EXPR_TREE
            if (setterDelegate != null && getterDelegate != null)
            {
                return;
            }
            
            if (!IsValid())
            {
                return;
            }

            var property = GetProperty();

            getterDelegate = GetValueGetter<object>(property);
            setterDelegate = GetValueSetter<object>(property);
#endif
        }
        
        /// <summary>
        ///     Return information about property operations is available
        /// </summary>
        /// <returns> true if all is ok </returns>
        public bool IsValid()
        {
            return GetProperty() != null;
        }

#if CAN_USE_EXPR_TREE
        public static Func<T, object> GetValueGetter<T>(PropertyInfo propertyInfo)
        {
            var instance = Expression.Parameter(typeof(object), "i");
            var property = Expression.Property(Expression.TypeAs(instance, propertyInfo.DeclaringType), propertyInfo);
            var convert = Expression.TypeAs(property, typeof(object));
            return (Func<T, object>)Expression.Lambda(convert, instance).Compile();
        }
        
        public static Action<T, object> GetValueSetter<T>(PropertyInfo propertyInfo)
        {
            var instance = Expression.Parameter(typeof(object), "i");
            var property = Expression.TypeAs(instance, propertyInfo.DeclaringType);
            var argument = Expression.Parameter(typeof(object), "a");
            var setterCall = Expression.Call(
                property,
                propertyInfo.GetSetMethod(),
                Expression.Convert(argument, propertyInfo.PropertyType));
            return (Action<T, object>)Expression.Lambda(setterCall, instance, argument).Compile();
        }
#endif
    }
}
