namespace System.Extension
{
    /// <summary>
    /// This exception is thrown when an attempt is made to change the an
    /// instance of <see cref="ISealable"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exception class is intentionally made as a subclass of 
    /// <see cref="System.NotSupportedException"/>, so that a sealable collection
    /// can behave consistently with any read only collections by throwing this 
    /// exception when it is in the sealed state.
    /// </para>
    /// </remarks>
    /// <author>Kenneth Xu</author>
    public class InstanceSealedException : NotSupportedException
    {
        /// <summary>
        /// Constuct an InstanceSealedException instance.
        /// </summary>
        public InstanceSealedException() : base("Object is sealed") { }
    }
}
