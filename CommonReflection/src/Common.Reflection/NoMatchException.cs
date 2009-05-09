#region License

/*
 * Copyright (C) 2009 the original author or authors.
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
using System.Reflection;
using System.Text;

namespace Common.Reflection
{
    /// <summary>
    /// Exception to indication that there is no matching method found in a
    /// specific type.
    /// </summary>
    public class NoMatchException : SystemException
    {
        /// <summary>
        /// Construct a new instance of <see cref="NoMatchException"/>.
        /// </summary>
        /// <param name="type">
        /// The type on which the method is being searched.
        /// </param>
        /// <param name="bindingFlags">
        /// The binding flags used to match the method.
        /// </param>
        /// <param name="methodName">
        /// The name of the named.
        /// </param>
        /// <param name="parameters">
        /// Number and types of method parameter to match.
        /// </param>
        public NoMatchException(Type type, BindingFlags bindingFlags, string methodName, params Type[] parameters)
            :base(buildMessage(type, bindingFlags, methodName, parameters))
        {
        }

        private static string buildMessage(Type type, BindingFlags bindingFlags, string methodName, params Type[] parameters)
        {
            StringBuilder sb = new StringBuilder()
                .Append("No matching method found in the type ")
                .Append(type.FullName)
                .Append(" for signature ").Append(methodName).Append("(");
            foreach (Type parameter in parameters)
            {
                sb.Append(parameter.FullName).Append(", ");
            }
            sb.Length -= 2;
            sb.Append(") with binding flags: ").Append(bindingFlags).Append(".");
            return sb.ToString();
        }
    }
}
