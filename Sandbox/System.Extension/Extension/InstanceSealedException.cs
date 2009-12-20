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
