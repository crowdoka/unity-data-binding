using System;

namespace TypeInspector
{
    /// <summary>
    ///     Make filter of targets for PropertyReference
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TargetTypeFilterAttribute : Attribute
    {
        public Type ClassType { get; set; }


        /// <summary>
        ///    Make filter of target types for PropertyReference
        /// </summary>
        /// <param name="classType"> Type which will filter targets </param>
        public TargetTypeFilterAttribute(Type classType)
        {
            ClassType = classType;
        }
    }
}