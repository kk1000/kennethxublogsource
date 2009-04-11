using System;
using System.Collections.Generic;
using System.Data;
using Spring.Data.Common;
using Spring.Data.Support;

namespace Spring.Data.Generic
{
    public static class AdoOperationsBatchExtension
    {
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
