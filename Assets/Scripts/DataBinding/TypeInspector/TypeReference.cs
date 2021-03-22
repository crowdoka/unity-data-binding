
using System;

namespace TypeInspector
{
    /// <summary>
    ///     Make reference for type and create editor interface
    /// </summary>
    [Serializable]
    public class TypeReference
    {
        public string FullName;

        /// <summary>
        ///     Get selected type
        /// </summary>
        /// <returns> if all is ok, returns Type object, or return null </returns>
        public Type Get()
        {
            if (!IsValid())
            {
                return null;
            }

            return Type.GetType(FullName);
        }

        public bool IsValid() => !string.IsNullOrEmpty(FullName) && Type.GetType(FullName) != null;
    }
}