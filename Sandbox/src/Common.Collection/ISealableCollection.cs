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

using System.Collections.Generic;
using System.Collections;
using System.Extension;

namespace Common.Collection
{
    /// <summary>
    /// A <see cref="ISealableCollection"/> is a <see cref="ICollection"/> 
    /// that can be made readonly when it is <see cref="ISealable.Seal"/>ed.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface ISealableCollection : ICollection, ISealable
    {
    }

    /// <summary>
    /// A <see cref="ISealableCollection{T}"/> is a <see cref="ICollection{T}"/> 
    /// that can be made readonly when it is <see cref="ISealable.Seal"/>ed.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface ISealableCollection<T> : ICollection<T>, ISealable
    {
    }
}
