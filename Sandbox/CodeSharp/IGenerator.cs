using System;

namespace CodeSharp
{
    /// <summary>
    /// Common contract for different types of code generator.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Define a new class.
        /// </summary>
        /// <param name="name">
        /// Name of the class.
        /// </param>
        /// <returns>
        /// A class definition.
        /// </returns>
        IClass Class(string name);

        /// <summary>
        /// Define a new parameter.
        /// </summary>
        /// <param name="type">
        /// The type of the parameter.
        /// </param>
        /// <param name="parameterName">
        /// The name of the parameter.
        /// </param>
        /// <returns>
        /// A parameter definition.
        /// </returns>
        IParameter Arg(Type type, string parameterName);

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
        IParameter Arg<T>(string parameterName);

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
        IParameter ArgRef<T>(string parameterName);

        /// <summary>
        /// Define a new out parameter.
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
        IParameter ArgOut<T>(string parameterName);

        /// <summary>
        /// Create an <see cref="IOperand"/> from a string.
        /// </summary>
        /// <param name="value">
        /// The string value.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IOperand"/>.
        /// </returns>
        IOperand Const(string value);

        /// <summary>
        /// Create an <see cref="IOperand"/> representing a <c>null</c> value.
        /// </summary>
        /// <param name="type">Type of the operand.</param>
        /// <returns>
        /// The operand representing a <c>null</c>.
        /// </returns>
        IOperand Null(Type type);
    }
}
