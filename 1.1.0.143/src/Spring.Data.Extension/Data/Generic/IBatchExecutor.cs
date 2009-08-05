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

namespace Spring.Data.Generic
{
    /// <summary>
    /// Interface to use <see cref="Data.IAdoOperations"/> for batch update.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface IBatchExecutor
    {
        /// <summary>
        /// Executes batch of non queries with common command and different
        /// parameters. 
        /// </summary>
        /// <typeparam name="T">
        /// The type of the data object.
        /// </typeparam>
        /// <param name="operation">
        /// An <see cref="Spring.Data.IAdoOperations"/> object to perform
        /// database updates.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="data">
        /// A collection of data object to be updated in batch.
        /// </param>
        /// <param name="dataToParameters">
        /// Delegate that converts data object to parameters.
        /// </param>
        /// <returns>
        /// The total updated count if 0 or positive. When -1 is returned,
        /// it indicates that the update count cannot be obtained due the
        /// the limitation of the batch implementation.
        /// </returns>
        int ExecuteNonQuery<T>(
            IAdoOperations operation,
            System.Data.CommandType cmdType,
            string cmdText,
            System.Collections.Generic.ICollection<T> data,
            System.Converter<T, Common.IDbParameters> dataToParameters);
    }
}