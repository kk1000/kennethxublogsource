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
    /// <author>Kenneth Xu</author>
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
