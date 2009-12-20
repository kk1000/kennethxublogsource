#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

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
    /// <author>Kenneth Xu</author>
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
