using System;
using System.Collections.Generic;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Represents parameter list of method, indexer, delegate and etc.
    /// </summary>
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

        ///// <summary>
        ///// Returns an enumerator that iterates through the collection.
        ///// </summary>
        ///// <returns>
        ///// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        ///// </returns>
        ///// <filterpriority>1</filterpriority>
        //public IEnumerator<IParameter> GetEnumerator()
        //{
        //    return _parameters.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        /// <summary>
        /// Gets an array of parameter types.
        /// </summary>
        /// <returns>
        /// An array of parameter types.
        /// </returns>
        public Type[] GetTypes()
        {
            var paramTypes = new Type[_parameters.Count];
            for (int i = 0; i < _parameters.Count; i++)
            {
                paramTypes[i] = _parameters[i].Type;
            }
            return paramTypes;
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
    }
}