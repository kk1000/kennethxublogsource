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

namespace NUnitStuff
{
    /// <summary>
    /// Base test cases for value objects that has a default constructor.
    /// </summary>
    /// <typeparam name="T">Type of the value object to be tested.</typeparam>
    /// <author>Kenneth Xu</author>
    public abstract class NewableValueObjectTestFixture<T> : ValueObjectTestFixture<T>
        where T : new()
    {
        /// <summary>
        /// Create a new value object of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="T"/></returns>
        protected override T NewValueObject()
        {
            return new T();
        }
    }
}
