namespace System.Extension
{
    /// <summary>
    /// Marks a class as being sealable.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A sealable object can be made immutable by sealing it. An instance is 
    /// said "sealed" after it's <see cref="Seal"/> method is called. In 
    /// general, a sealed instance should not allow any further changes.
    /// Any attempt to change a sealed instance should result in an
    /// <see cref="InstanceSealedException"/> being thrown.
    /// </para>
    /// <para>
    /// New sealable class can be created by implementing the <c>ISealable</c>
    /// interface or inherit from <see cref="Sealable"/> class.
    /// </para>
    /// <para>
    /// <see cref="InstanceSealedException"/> is a subclass of 
    /// <see cref="System.NotSupportedException"/> so that a sealable collection
    /// can throw the InstanceSealedException when it is in sealed state.
    /// </para>
    /// </remarks>
    /// <seealso cref="Sealable"/>
    /// <seealso cref="InstanceSealedException"/>
    /// <auther>Kenneth Xu</auther>
    public interface ISealable
    {
        /// <summary>
        /// Seal the instance. 
        /// </summary>
        void Seal();

        /// <summary>
        /// Read only proerty to indicate if the instance is sealed.
        /// </summary>
        /// <value><c>true</c> if and only if the the instance is sealed.</value>
        bool IsSealed {get;}	
    }
}
