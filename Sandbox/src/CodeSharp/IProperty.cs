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

using System.Reflection;

namespace CodeSharp
{
    /// <summary>
    /// Represents a property.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface IProperty
    {
        /// <summary>
        /// Mark the property public.
        /// </summary>
        IProperty Public { get; }

        /// <summary>
        /// Mark the property overrides given <paramref name="property"/>.
        /// </summary>
        /// <param name="property">Property to be overriden.</param>
        IProperty Override(PropertyInfo property);

        /// <summary>
        /// Create the getter.
        /// </summary>
        /// <returns>
        /// The definition for the property getter.
        /// </returns>
        IMethod Getter();

        /// <summary>
        /// Create the setter.
        /// </summary>
        /// <returns>
        /// The definition of the property getter.
        /// </returns>
        ISetter Setter();
    }
}
