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
    /// use a different method to raise the 
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </summary>
    /// <remarks>
    /// <see cref="NotifyPropertyChangeFactory.SetMarkingAttribute{TA}()"/>
    /// </remarks>
    /// <seealso cref="NotifyPropertyChangeFactory"/>
    /// <author>Kenneth Xu</author>
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
