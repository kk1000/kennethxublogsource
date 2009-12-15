using System;
using System.ComponentModel;

namespace CodeSharp.Proxy
{
    /// <summary>
    /// Specifies that <see cref="NotifyPropertyChangeFactory"/> should 
    /// use a different method to raise the 
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </summary>
    /// <remarks>
    /// <see cref="NotifyPropertyChangeFactory.SetMarkingAttribute{TA}()"/>
    /// </remarks>
    /// <seealso cref="NotifyPropertyChangeFactory"/>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class OnPropertyChangeAttribute : Attribute
    {
        private readonly string _onPropertyChangedMethod;

        /// <summary>
        /// Initialize a new instance of <see cref="OnPropertyChangeAttribute"/>
        /// with specified method name that can be used to raise the
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="onPropertyChangedMethod">
        /// The method to raise <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// event. The method must has one string parameter and return type is void.
        /// </param>
        public OnPropertyChangeAttribute(string onPropertyChangedMethod)
        {
            _onPropertyChangedMethod = onPropertyChangedMethod;
        }

        /// <summary>
        /// Gets the method to raise <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// event. The method must has one string parameter and return type is void.
        /// </summary>
        public string OnPropertyChangedMethod
        {
            get { return _onPropertyChangedMethod; }
        }
    }
}
