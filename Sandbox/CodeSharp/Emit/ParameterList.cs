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