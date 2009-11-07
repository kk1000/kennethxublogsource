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
    }
}
