using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSharp
{
    /// <summary>
    /// Holder of all extension methods.
    /// </summary>
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
