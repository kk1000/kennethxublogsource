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

namespace CodeSharp
{
    /// <summary>
    /// Represents a parameter of method, delegate, indexer and etc.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface IParameter : IOperand
    {
        /// <summary>
        /// The direction of parameter.
        /// </summary>
        ParameterDirection Direction { get; }

        /// <summary>
        /// Name of parameter.
        /// </summary>
        string Name { get; }
    }
}
