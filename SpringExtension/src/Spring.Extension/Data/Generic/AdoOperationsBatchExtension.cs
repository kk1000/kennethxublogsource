using System;
using System.Collections.Generic;
using System.Data;
using Spring.Data.Common;
using Spring.Data.Support;

namespace Spring.Data.Generic
{
    /// <summary>
    /// Extension Methods for <see cref="IAdoOperations"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public static class AdoOperationsBatchExtension
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
        public static int ExecuteNonQuery<T>(
            this IAdoOperations operation,
            CommandType cmdType,
            string cmdText, 
            ICollection<T> data,
            Converter<T, IDbParameters> dataToParameters)
        {
            // Argument checking
            if (data == null || data.Count == 0) return 0;
            if (cmdText == null) throw new ArgumentNullException("cmdText");
            if (dataToParameters == null) throw new ArgumentNullException("dataToParameters");

            if (data.Count > 1)
            {
                // Let's try batch
                IBatchExecutorFactory factory = operation as IBatchExecutorFactory;
                if (factory != null)
                {
                    return factory.GetExecutor().ExecuteNonQuery(
                        operation, cmdType, cmdText, data, dataToParameters);
                }
            }

            Setter setter = new Setter();
            int result = 0;
            foreach (T row in data)
            {
                setter.Parameters = dataToParameters(row);
                result += operation.ExecuteNonQuery(cmdType, cmdText, setter);
            }
            return result;
        }

        private class Setter : ICommandSetter
        {
            internal IDbParameters Parameters;

            public void SetValues(IDbCommand dbCommand)
            {
                ParameterUtils.CopyParameters(dbCommand, Parameters);
            }
        }
    }
}
