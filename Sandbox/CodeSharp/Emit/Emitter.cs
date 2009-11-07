using System;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// An <see cref="IGenerator"/> implementation using Reflection.Emit.
    /// </summary>
    public class Emitter : IGenerator
    {
        private readonly ModuleBuilder _moduleBuilder;

        /// <summary>
        /// Create a new instance of <see cref="Emitter"/> that emits code
        /// to the dynamic module defined by <see cref="ModuleBuilder"/>.
        /// </summary>
        /// <param name="moduleBuilder">
        /// The <see cref="ModuleBuilder"/> of the dynamic module.
        /// </param>
        public Emitter(ModuleBuilder moduleBuilder)
        {
            if (moduleBuilder == null) throw new ArgumentNullException("moduleBuilder");
            _moduleBuilder = moduleBuilder;
        }

        /// <summary>
        /// Define a new class.
        /// </summary>
        /// <param name="name">
        /// Name of the class.
        /// </param>
        /// <returns>
        /// A class definition.
        /// </returns>
        public IClass Class(string name)
        {
            return new Class(name);
        }

        /// <summary>
        /// Define a new parameter.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parameter.
        /// </typeparam>
        /// <param name="parameterName">
        /// The name of the parameter.
        /// </param>
        /// <returns>
        /// A parameter definition.
        /// </returns>
        public IParameter Arg<T>(string parameterName)
        {
            return new Parameter(typeof(T), parameterName, ParameterDirection.In);
        }

        /// <summary>
        /// Define a new ref parameter.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parameter.
        /// </typeparam>
        /// <param name="parameterName">
        /// The name of the parameter.
        /// </param>
        /// <returns>
        /// A parameter definition.
        /// </returns>
        public IParameter ArgRef<T>(string parameterName)
        {
            return new Parameter(typeof(T), parameterName, ParameterDirection.Ref);
        }

        /// <summary>
        /// Generate the concrete type of given <paramref name="class"/>
        /// </summary>
        /// <param name="class">
        /// The class definition used to generate the class type.
        /// </param>
        /// <returns>
        /// The type object of the generated class.
        /// </returns>
        public Type Generate(IClass @class)
        {
            return ((Class) @class).Generate(_moduleBuilder);
        }
    }
}
