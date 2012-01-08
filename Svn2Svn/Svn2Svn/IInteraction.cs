#region License

/*
 * Copyright (C) 2012 the original author or authors.
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
namespace Svn2Svn
{
    /// <summary>
    /// Interface for UI interaction.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface IInteraction
    {
        void Info(string value);
        // ReSharper disable MethodOverloadWithOptionalParameter
        void Info(string format, params object[] values);
        // ReSharper restore MethodOverloadWithOptionalParameter
        void Trace(string value);
        // ReSharper disable MethodOverloadWithOptionalParameter
        void Trace(string format, params object[] values);
        // ReSharper restore MethodOverloadWithOptionalParameter
        void Error(string value);
        // ReSharper disable MethodOverloadWithOptionalParameter
        void Error(string format, params object[] values);
        // ReSharper restore MethodOverloadWithOptionalParameter
        void UpdateProgress(long sourceRevision, long destinationReivison);
    }
}