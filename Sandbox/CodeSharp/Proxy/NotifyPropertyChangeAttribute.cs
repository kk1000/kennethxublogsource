using System;
using System.ComponentModel;

namespace CodeSharp.Proxy
{
    /// <summary>
    /// Specifies that <see cref="NotifyPropertyChangeFactory"/> should 
    /// generate proxy for the interface indicated by this attribute.
    /// </summary>
    /// <remarks>
    /// <see cref="NotifyPropertyChangeFactory.SetMarkingAttribute{TA}()"/>
    /// </remarks>
    /// <seealso cref="NotifyPropertyChangeFactory"/>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class NotifyPropertyChangeAttribute : Attribute
    {
        private readonly Type _baseType;
        private string _onPropertyChangedMethodName = NotifyPropertyChangeFactory.DefaultOnPropertyChangedMethodName;

        /// <summary>
        /// Initialize a new instance of <see cref="NotifyPropertyChangeAttribute"/>.
        /// </summary>
        public NotifyPropertyChangeAttribute()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="NotifyPropertyChangeAttribute"/>
        /// with specified base type to be used for generated proxy.
        /// </summary>
        /// <param name="baseType"></param>
        public NotifyPropertyChangeAttribute(Type baseType)
        {
            _baseType = baseType;
        }

        /// <summary>
        /// Gets base type for the generated proxy, or null to use
        /// <see cref="NotifyPropertyChangeFactory"/>'s default base type.
        /// </summary>
        public Type BaseType
        {
            get { return _baseType; }
        }

        /// <summary>
        /// The name of the method used to raise the 
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event. Or null
        /// to prevent raising event.
        /// </summary>
        public string OnPropertyChangedMethodName
        {
            get { return _onPropertyChangedMethodName; }
            set { _onPropertyChangedMethodName = value; }
        }
    }
}
