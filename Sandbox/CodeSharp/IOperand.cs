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
using System.Reflection;

namespace CodeSharp
{
    /// <summary>
    /// Represents an operand.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface IOperand
    {
        /// <summary>
        /// Access a field of current operand.
        /// </summary>
        /// <param name="name">
        /// The name of the field.
        /// </param>
        /// <returns>
        /// An <see cref="IOperand"/> representing the field.
        /// </returns>
        IOperand Field(string name);

        /// <summary>
        /// Access a field of current operand.
        /// </summary>
        /// <param name="field">
        /// The definition of the field.
        /// </param>
        /// <returns>
        /// An <see cref="IOperand"/> representing the field.
        /// </returns>
        IOperand Field(IField field);

        /// <summary>
        /// Invoke a method on current operand.
        /// </summary>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="args">Parameters passed to the method.</param>
        /// <returns>
        /// The operand representing the result.
        /// </returns>
        IOperand Invoke(string methodName, params IOperand[] args);

        /// <summary>
        /// Invoke a method specified by <paramref name="methodInfo"/> on
        /// current operand.
        /// </summary>
        /// <param name="methodInfo">
        /// The method to invoke.
        /// </param>
        /// <param name="args">
        /// Parameters passed to the method.
        /// </param>
        /// <returns>
        /// The operand representing the result.
        /// </returns>
        IOperand Invoke(MethodInfo methodInfo, IEnumerable<IOperand> args);

        /// <summary>
        /// Invoke a method specified by <paramref name="methodInfo"/> on
        /// current operand.
        /// </summary>
        /// <param name="methodInfo">
        /// The method to invoke.
        /// </param>
        /// <param name="args">
        /// Parameters passed to the method.
        /// </param>
        /// <returns>
        /// The operand representing the result.
        /// </returns>
        IOperand Invoke(MethodInfo methodInfo, params IOperand[] args);

        /// <summary>
        /// Access a property of current operand.
        /// </summary>
        /// <param name="name">
        /// The name of property.
        /// </param>
        /// <returns>
        /// The operant representing the property.
        /// </returns>
        IOperand Property(string name);

        /// <summary>
        /// Access an indexer of current operand.
        /// </summary>
        /// <param name="args">
        /// Index parameters.
        /// </param>
        /// <returns>
        /// The operand representing the indexer.
        /// </returns>
        IOperand Indexer(params IOperand[] args);

        /// <summary>
        /// Access a property of current operand.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property of the current operand.
        /// </param>
        /// <param name="indexes">
        /// Property indexes.
        /// </param>
        /// <returns>
        /// The operant representing the property.
        /// </returns>
        IOperand Property(PropertyInfo propertyInfo, IEnumerable<IOperand> indexes);

        /// <summary>
        /// Access a property of current operand.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property of the current operand.
        /// </param>
        /// <param name="indexes">
        /// Property indexes.
        /// </param>
        /// <returns>
        /// The operant representing the property.
        /// </returns>
        IOperand Property(PropertyInfo propertyInfo, params IOperand[] indexes);

        /// <summary>
        /// Safe cast to <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type to cast current operand value.</param>
        /// <returns>
        /// Operand represents the result of save cast.
        /// </returns>
        IOperand As(Type type);

        /// <summary>
        /// Safe cast to <paramref name="class"/>.
        /// </summary>
        /// <param name="class">Type to cast current operand value.</param>
        /// <returns>
        /// Operand represents the result of save cast.
        /// </returns>
        IOperand As(IClass @class);

        /// <summary>
        /// The type of the operand.
        /// </summary>
        Type Type { get; }
    }
}
