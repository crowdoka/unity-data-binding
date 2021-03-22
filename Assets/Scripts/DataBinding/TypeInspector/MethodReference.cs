using System;
using System.Reflection;

namespace TypeInspector
{
    /// <summary>
    ///     Class for make mthod referencies in editor
    /// </summary>
    [Serializable]
    public class MethodReference
    {
        protected MethodInfo methodCache;

        public TypeReference TargetType;
        public string MethodName;

        /// <summary>
        ///     Return type of selected object instance
        /// </summary>
        /// <returns> see <see cref="TypeReference.Get"/> </returns>
        public Type GetSourceType()
        {
            return TargetType.Get();
        }

        /// <summary>
        ///     Return target method of selected source object 
        /// </summary>
        /// <returns> if all is ok, return MethodInfo object, or return null </returns>
        public MethodInfo GetMethod()
        {
            if (!TargetType.IsValid() || string.IsNullOrEmpty(MethodName))
            {
                return null;
            }

            if (methodCache != null)
            {
                return methodCache;
            }

            methodCache = TargetType.Get().GetMethod(MethodName);

            return TargetType.Get().GetMethod(MethodName);
        }

        /// <summary>
        ///     Invoke method
        /// </summary>
        /// <param name="instance"> instance where is target method </param>
        /// <param name="data"> value which will called to the target method </param>
        public object Invoke(object instance, object data)
        {
            Init();
            
            var method = GetMethod();

            if (method == null)
            {
                return null;
            }

            return method.Invoke(instance, new[] { data });
        }
        
        /// <summary>
        ///     Make initialization
        /// </summary>
        public void Init()
        {
        }
        
        /// <summary>
        ///     Return information about property operations is available
        /// </summary>
        /// <returns> true if all is ok </returns>
        public bool IsValid()
        {
            return GetMethod() != null;
        }
    }
}
