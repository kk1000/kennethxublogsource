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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSharp
{
    /// <summary>
    /// Holder of all extension methods.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public static class Extensions
    {
        /// <summary>
        /// Convert <see cref="IParameterList"/> to an enumerable of
        /// <see cref="IOperand"/>.
        /// </summary>
        /// <param name="parameters">
        /// An <see cref="IParameterList"/>
        /// </param>
        /// <returns>
        /// An enumerable of <see cref="IOperand"/>.
        /// </returns>
        public static IEnumerable<IOperand> AsOperands(this IParameterList parameters)
        {
            return from p in parameters.All select (IOperand) p;
        }
    }
}
