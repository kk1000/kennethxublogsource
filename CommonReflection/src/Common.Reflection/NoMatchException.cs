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

namespace Common.Reflection
{
    /// <summary>
    /// Exception to indication that there is no matching method found in a
    /// specific type.
    /// </summary>
    public class NoMatchException : SystemException
    {
        /// <summary>
        /// Construct a new instance of <see cref="NoMatchException"/> with
        /// given error <paramref name="message"/> and <paramref name="inner"/>
        /// exeption.
        /// </summary>
        /// <param name="message">
        /// The text message describes the cause of exception.
        /// </param>
        /// <param name="inner">
        /// The inner exception if any.
        /// </param>
        public NoMatchException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Construct a new instance of <see cref="NoMatchException"/> with
        /// given error <paramref name="message"/>.
        /// </summary>
        /// <param name="message">
        /// The text message describes the cause of exception.
        /// </param>
        public NoMatchException(string message)
            : base(message)
        {
        }
    }
}
