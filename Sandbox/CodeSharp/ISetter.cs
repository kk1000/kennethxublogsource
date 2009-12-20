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
    /// Represents a property setter.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface ISetter : IInvokable
    {
        /// <summary>
        /// The value to set the property.
        /// </summary>
        IParameter Value { get; }

        /// <summary>
        /// Current method overrides the method
        /// </summary>
        /// <param name="method">
        /// The method to be overridden.
        /// </param>
        ISetter Override(MethodInfo method);
    }
}
