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
    public static class AdoOperationsExtension
    {
        /// <summary>
        /// Executes batch of non queries with common command and different
        /// parameters. 
        /// </summary>
        /// <typeparam name="T">
        /// The type of the data object.
        /// </typeparam>
        /// <param name="operation">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
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

            ParameterSetter setter = new ParameterSetter();
            int result = 0;
            foreach (T row in data)
            {
                setter.Parameters = dataToParameters(row);
                result += operation.ExecuteNonQuery(cmdType, cmdText, setter);
            }
            return result;
        }

        #region QueryWithRowMapper

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="IRowMapper{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapper">
        /// The row mapper that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        public static IList<T> QueryWithRowMapper<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            IRowMapper<T> rowMapper,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowMapper, ordinalCache, rowsExpected);
            return operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor);
        }

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="IRowMapper{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapper">
        /// The row mapper that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="commandSetter">
        /// The command setter that make necessary changes to the
        /// <see cref="IDbCommand"/> object before it is executed.
        /// For example, set the parameters.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        public static IList<T> QueryWithRowMapper<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            IRowMapper<T> rowMapper,
            ICommandSetter commandSetter,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowMapper, ordinalCache, rowsExpected);
            return operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor, commandSetter);
        }

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="IRowMapper{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapper">
        /// The row mapper that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="commandSetterDelegate">
        /// The command setter that make necessary changes to the
        /// <see cref="IDbCommand"/> object before it is executed.
        /// For example, set the parameters.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        [Obsolete("Use DelegateCommandSetter class instead.")]
        public static IList<T> QueryWithRowMapper<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            IRowMapper<T> rowMapper,
            Action<IDbCommand> commandSetterDelegate,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var commandSetter = new DelegateCommandSetter(commandSetterDelegate);
            return QueryWithRowMapper(operations, cmdType, cmdText, rowMapper,
                commandSetter, ordinalCache, rowsExpected);
        }

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="IRowMapper{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapper">
        /// The row mapper that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="parameters">
        /// The parameters with all neccessary values populated.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        public static IList<T> QueryWithRowMapper<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            IRowMapper<T> rowMapper,
            IDbParameters parameters,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowMapper, ordinalCache, rowsExpected);
            return operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor, parameters);
        }

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="IRowMapper{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapper">
        /// The row mapper that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="parameterValue">
        /// The value of the parameter.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="parameterName">
        /// The name of the parameter.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <param name="dbType">
        /// Type type of the parameter.
        /// </param>
        /// <param name="size">
        /// The size of parameter.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        public static IList<T> QueryWithRowMapper<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            IRowMapper<T> rowMapper,
            string parameterName, Enum dbType, int size, object parameterValue,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowMapper, ordinalCache, rowsExpected);
            return operations.QueryWithResultSetExtractor(
                cmdType, cmdText, extractor, parameterName, dbType, size, parameterValue);
        }

        private static IResultSetExtractor<IList<T>> MakeResultSetExtractor<T>(
            IRowMapper<T> rowMapper, IDataRecordOrdinalCache ordinalCache, int rowsExpected)
        {
            return new ExtendedRowMapperResultSetExtractor<T>(rowMapper)
                       {
                           OrdinalCache = ordinalCache,
                           RowsExpected = rowsExpected
                       };
        }

        #endregion

        #region QueryWithRowMapperDelegate

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="RowMapperDelegate{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapperDelegate">
        /// The row mapper delegate that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        public static IList<T> QueryWithRowMapperDelegate<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            RowMapperDelegate<T> rowMapperDelegate,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowMapperDelegate, ordinalCache, rowsExpected);
            return operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor);
        }

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="RowMapperDelegate{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapperDelegate">
        /// The row mapper delegate that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="commandSetter">
        /// The command setter that make necessary changes to the
        /// <see cref="IDbCommand"/> object before it is executed.
        /// For example, set the parameters.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        public static IList<T> QueryWithRowMapperDelegate<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            RowMapperDelegate<T> rowMapperDelegate,
            ICommandSetter commandSetter,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowMapperDelegate, ordinalCache, rowsExpected);
            return operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor, commandSetter);
        }

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="IRowMapper{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapperDelegate">
        /// The row mapper that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="commandSetterDelegate">
        /// The command setter that make necessary changes to the
        /// <see cref="IDbCommand"/> object before it is executed.
        /// For example, set the parameters.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        [Obsolete("Use DelegateCommandSetter class instead.")]
        public static IList<T> QueryWithRowMapperDelegate<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            RowMapperDelegate<T> rowMapperDelegate,
            Action<IDbCommand> commandSetterDelegate,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var commandSetter = new DelegateCommandSetter(commandSetterDelegate);
            return QueryWithRowMapperDelegate(operations, cmdType, cmdText, rowMapperDelegate,
                commandSetter, ordinalCache, rowsExpected);
        }

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="RowMapperDelegate{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapperDelegate">
        /// The row mapper delegate that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="parameters">
        /// The parameters with all neccessary values populated.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        public static IList<T> QueryWithRowMapperDelegate<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            RowMapperDelegate<T> rowMapperDelegate,
            IDbParameters parameters,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowMapperDelegate, ordinalCache, rowsExpected);
            return operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor, parameters);
        }

        /// <summary>
        /// Query database with given SQL command and mapping each row to a 
        /// object via a <see cref="RowMapperDelegate{T}"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// <typeparam name="T">The type of object that each row maps to.</typeparam>
        /// <param name="operations">
        /// An <see cref="Spring.Data.Generic.IAdoOperations"/> object to 
        /// perform database operation.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="rowMapperDelegate">
        /// The row mapper delegate that maps each row to object of type 
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="parameterValue">
        /// The value of the parameter.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="parameterName">
        /// The name of the parameter.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        /// <param name="dbType">
        /// Type type of the parameter.
        /// </param>
        /// <param name="size">
        /// The size of parameter.
        /// </param>
        /// <returns>
        /// A list of object of type <typeparamref name="T"/> that were mapped
        /// for the rows.
        /// </returns>
        public static IList<T> QueryWithRowMapperDelegate<T>(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            RowMapperDelegate<T> rowMapperDelegate,
            string parameterName, Enum dbType, int size, object parameterValue,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowMapperDelegate, ordinalCache, rowsExpected);
            return operations.QueryWithResultSetExtractor(
                cmdType, cmdText, extractor, parameterName, dbType, size, parameterValue);
        }

        private static IResultSetExtractor<IList<T>> MakeResultSetExtractor<T>(
            RowMapperDelegate<T> rowMapperDelegate, IDataRecordOrdinalCache ordinalCache, int rowsExpected)
        {
            return new ExtendedRowMapperResultSetExtractor<T>(rowMapperDelegate)
            {
                OrdinalCache = ordinalCache,
                RowsExpected = rowsExpected
            };
        }

        #endregion

        private class ParameterSetter : ICommandSetter
        {
            internal IDbParameters Parameters;

            public void SetValues(IDbCommand dbCommand)
            {
                ParameterUtils.CopyParameters(dbCommand, Parameters);
            }
        }
    }
}
