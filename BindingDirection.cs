namespace Crowdoka.DataBinding
{
    /// <summary>
    ///     Type of data flow binding
    /// </summary>
    public enum BindingDirection
    {
        /// <summary>
        ///     Any change of source or destination will be change other side
        /// </summary>
        BothWays = 3,
        
        /// <summary>
        ///     Change of source will produce same changes in destination
        /// </summary>
        SourceToDestination = 1,
        
        /// <summary>
        ///     Change of destination will produce same changes in source
        /// </summary>
        DestinationToSource = 2
    }
}