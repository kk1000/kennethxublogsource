#region License

/*
 * Copyright (C) 2009 the original author or authors.
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

namespace Common.Reflection
{
    /// <summary>
    /// Accessor to a property. It contains two delegates, <see cref="Get"/>
    /// is for reading the property and <see cref="Set"/> is for writing the
    /// property. <see cref="Value"/> can also be used to access the property.
    /// </summary>
    /// <typeparam name="T">Type of property value.</typeparam>
    /// <author>Kenneth Xu</author>
    public struct Accessor<T>
    {
        /// <summary>
        /// The getter delegate for getting the property value.
        /// </summary>
        public Func<T> Get { get; set; }

        /// <summary>
        /// The setter delegate for setting the property value.
        /// </summary>
        public Action<T> Set { get; set; }

        /// <summary>
        /// Gets and sets the value of the property bound to this accessor.
        /// </summary>
        public T Value
        {
            get { return Get(); }
            set { Set(value); }
        }
    }

    /// <summary>
    /// Accessor to a instance property. It contains two open delegates, which
    /// are not bound to a specific instance. The <see cref="Get"/> delegate is
    /// for reading the property and <see cref="Set"/> delegate is for writing
    /// the property. <see cref="this"/> can also be used to access the property.
    /// </summary>
    /// <typeparam name="TInstance">
    /// The type that defined or inherited the property.
    /// </typeparam>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public struct Accessor<TInstance, T>
    {
        /// <summary>
        /// The getter delegate that can get the property value for any instance
        /// of type <typeparamref name="TInstance"/> that passed to the delegate.
        /// </summary>
        public Func<TInstance, T> Get { get; set; }

        /// <summary>
        /// The setter delegate that can set the property value for any instance
        /// of type <typeparamref name="TInstance"/> that passed to the delegate.
        /// </summary>
        public Action<TInstance, T> Set { get; set; }

        /// <summary>
        /// Gets and sets the value of the property bound to this accessor.
        /// indexed by any instance of type <typeparamref name="TInstance"/>.
        /// </summary>
        /// <param name="target">
        /// The target instance to set or get the property value.
        /// </param>
        /// <returns>
        /// The property value.
        /// </returns>
        public T this[TInstance target]
        {
            get { return Get(target); }
            set { Set(target, value); }
        }
    }
}