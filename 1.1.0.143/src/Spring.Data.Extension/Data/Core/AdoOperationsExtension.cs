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
using System.Data;
using Spring.Data.Common;
using Spring.Data.Support;

namespace Spring.Data.Core
{
    /// <summary>
    /// Extension Methods for <see cref="IAdoOperations"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public static class AdoOperationsExtension
    {

        #region QueryWithRowCallback

        /// <summary>
        /// Query database with given SQL command and have each row processed
        /// by a given <see cref="IRowCallback"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// 
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
        /// <param name="rowCallback">
        /// The row callback that processes each row.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        public static void QueryWithRowCallback(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            IRowCallback rowCallback,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowCallback, ordinalCache, rowsExpected);
            operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor);
        }

        /// <summary>
        /// Query database with given SQL command and have each row processed
        /// by a given <see cref="IRowCallback"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// 
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
        /// <param name="rowCallback">
        /// The row callback that processes each row.
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
        public static void QueryWithRowCallback(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            IRowCallback rowCallback,
            ICommandSetter commandSetter,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowCallback, ordinalCache, rowsExpected);
            operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor, commandSetter);
        }

        /// <summary>
        /// Query database with given SQL command and have each row processed
        /// by a given <see cref="IRowCallback"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// 
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
        /// <param name="rowCallback">
        /// The row callback that processes each row.
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
        public static void QueryWithRowCallback(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            IRowCallback rowCallback,
            IDbParameters parameters,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowCallback, ordinalCache, rowsExpected);
            operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor, parameters);
        }

        /// <summary>
        /// Query database with given SQL command and have each row processed
        /// by a given <see cref="IRowCallback"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// 
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
        /// <param name="rowCallback">
        /// The row callback that processes each row.
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
        public static void QueryWithRowCallback(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            IRowCallback rowCallback,
            string parameterName, Enum dbType, int size, object parameterValue,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowCallback, ordinalCache, rowsExpected);
            operations.QueryWithResultSetExtractor(
                cmdType, cmdText, extractor, parameterName, dbType, size, parameterValue);
        }

        private static IResultSetExtractor MakeResultSetExtractor(
            IRowCallback rowCallback, IDataRecordOrdinalCache ordinalCache, int rowsExpected)
        {
            return new ExtendedRowCallbackResultSetExtractor(rowCallback)
            {
                OrdinalCache = ordinalCache,
                RowsExpected = rowsExpected
            };
        }

        #endregion

        #region QueryWithRowCallbackDelegate

        /// <summary>
        /// Query database with given SQL command and have each row processed
        /// by a given <see cref="RowCallbackDelegate"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// 
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
        /// <param name="rowCallbackDelegate">
        /// The row callback delegate that processes each row.
        /// </param>
        /// <param name="ordinalCache">
        /// The <see cref="IDataRecordOrdinalCache"/> that caches the mapping
        /// from field name to field index.
        /// </param>
        /// <param name="rowsExpected">
        /// Number of rows this query is expected to return. This doesn't need
        /// to be accurate but estimating at the higer end for best performance.
        /// </param>
        public static void QueryWithRowCallbackDelegate(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            RowCallbackDelegate rowCallbackDelegate,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowCallbackDelegate, ordinalCache, rowsExpected);
            operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor);
        }

        /// <summary>
        /// Query database with given SQL command and have each row processed
        /// by a given <see cref="RowCallbackDelegate"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// 
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
        /// <param name="rowCallbackDelegate">
        /// The row callback delegate that processes each row.
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
        public static void QueryWithRowCallbackDelegate(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            RowCallbackDelegate rowCallbackDelegate,
            ICommandSetter commandSetter,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowCallbackDelegate, ordinalCache, rowsExpected);
            operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor, commandSetter);
        }

        /// <summary>
        /// Query database with given SQL command and have each row processed
        /// by a given <see cref="RowCallbackDelegate"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// 
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
        /// <param name="rowCallbackDelegate">
        /// The row callback delegate that processes each row.
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
        public static void QueryWithRowCallbackDelegate(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            RowCallbackDelegate rowCallbackDelegate,
            IDbParameters parameters,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowCallbackDelegate, ordinalCache, rowsExpected);
            operations.QueryWithResultSetExtractor(cmdType, cmdText, extractor, parameters);
        }

        /// <summary>
        /// Query database with given SQL command and have each row processed
        /// by a given <see cref="RowCallbackDelegate"/> with help of optional
        /// ordinal cache and/or the rows expected value for better performance.
        /// </summary>
        /// 
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
        /// <param name="rowCallbackDelegate">
        /// The row callback delegate that processes each row.
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
        public static void QueryWithRowCallbackDelegate(
            this IAdoOperations operations,
            CommandType cmdType,
            string cmdText,
            RowCallbackDelegate rowCallbackDelegate,
            string parameterName, Enum dbType, int size, object parameterValue,
            IDataRecordOrdinalCache ordinalCache,
            int rowsExpected)
        {
            var extractor = MakeResultSetExtractor(rowCallbackDelegate, ordinalCache, rowsExpected);
            operations.QueryWithResultSetExtractor(
                cmdType, cmdText, extractor, parameterName, dbType, size, parameterValue);
        }

        private static IResultSetExtractor MakeResultSetExtractor(
            RowCallbackDelegate rowCallbackDelegate, IDataRecordOrdinalCache ordinalCache, int rowsExpected)
        {
            return new ExtendedRowCallbackResultSetExtractor(rowCallbackDelegate)
            {
                OrdinalCache = ordinalCache,
                RowsExpected = rowsExpected
            };
        }

        #endregion
    }
}
