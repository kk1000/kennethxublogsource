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

namespace CodeSharp.Emit
{
    /// <summary>
    /// Represents parameter list of method, indexer, delegate and etc.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class ParameterList : IParameterList //, IEnumerable<IParameter>
    {
        private readonly IList<IParameter> _parameters;

        /// <summary>
        /// Create new <see cref="ParameterList"/> consists of given
        /// <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">
        /// The parameters to be consisted of the parameter list.
        /// </param>
        public ParameterList(params IParameter[] parameters)
        {
            _parameters = parameters;
        }

        public ParameterList(IList<IParameter> parameters)
        {
            _parameters = parameters;
        }

        public IEnumerable<IParameter> All
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Gets the <see cref="Parameter">parameter</see> at given position.
        /// </summary>
        /// <param name="position">
        /// Position of the parameter in the parameter list.
        /// </param>
        /// <returns>
        /// The parameter at given position.
        /// </returns>
        public IParameter this[int position]
        {
            get { return _parameters[position]; }
        }

        public int Count
        {
            get { return _parameters.Count; }
        }

        /// <summary>
        /// Gets an array of parameter types.
        /// </summary>
        /// <returns>
        /// An array of parameter types.
        /// </returns>
        public Type[] ToTypes()
        {
            return ToTypes(_parameters);
        }

        /// <summary>
        /// Emits the definition parameters.
        /// </summary>
        /// <param name="invokable">
        /// The method that the parameter list is defined. 
        /// </param>
        internal void Emit(Invokable invokable)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                var parameter = (Parameter) _parameters[i];
                parameter.Emit(i+1, invokable);
            }
        }

        internal static Type[] ToTypes(ICollection<IParameter> parameters)
        {
            var paramTypes = new Type[parameters.Count];
            int i = 0;
            foreach (var parameter in parameters)
            {
                paramTypes[i++] = parameter.Type;
                
            }
            return paramTypes;
        }
    }
}
