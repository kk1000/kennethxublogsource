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

using NUnit.Framework;

namespace NUnitStuff
{
    /// <summary>
    /// A bad test fixture example by 3rd party that has <see cref="TestFixtureAttribute"/>
    /// on abstract class.
    /// </summary>
    /// <author>Kenneth Xu</author>
    [TestFixture]
    public abstract class ThirdPartyFixture
    {
        /// <summary>
        /// And the fixture actually has some test in it.
        /// </summary>
        [Test] public void ThirdPartyTest() {}
    }

}