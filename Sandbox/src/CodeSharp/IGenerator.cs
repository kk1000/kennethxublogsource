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
    /// Common contract for different types of code generator.
    /// </summary>
    /// <author>Kenneth Xu</author>
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
