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
using Oracle.DataAccess.Client;
using Spring.Dao;
using Spring.Data.Common;
using Spring.Data.Support;

namespace Spring.Data.Generic
{
    /// <summary>
    /// Subclass of <see cref="AdoTemplate"/> that is specially targeted to
    /// Oracle ODP.Net.
    /// </summary>
    /// <remarks>
    /// This class supports ODP.Net batch update as well as implements the
    /// <see cref="IDataRecord.GetChar"/> using an <see cref="IDataReaderWrapper"/>.
    /// </remarks>
    /// <author>Kenneth Xu</author>
    public class OracleOdpTemplate : AdoTemplate, IBatchExecutorFactory
    {
        /// <summary>
        /// The default batch size.
        /// </summary>
        public const int DEFALT_BATCH_SIZE = 100;

        /// <summary>
        /// Gets and sets the size of the batch to update.
        /// </summary>
        public virtual int BatchSize { get; set; }

        /// <summary>
        /// Construct a new instance of <see cref="OracleOdpTemplate"/>.
        /// </summary>
        public OracleOdpTemplate()
        {
            BatchSize = DEFALT_BATCH_SIZE;
            base.DataReaderWrapperType = typeof (OdpNetDataReaderWrapper);
        }

        #region IBatchExecutorFactory Members

        /// <summary>
        /// Get an instance of <see cref="IBatchExecutor"/>.
        /// </summary>
        /// <returns>An instance of <see cref="IBatchExecutor"/>.</returns>
        public virtual IBatchExecutor GetExecutor()
        {
            return new BatchExecutor(this);
        }

        #endregion

        internal class BatchExecutor : ICommandSetter, IBatchExecutor
        {
            private readonly OracleOdpTemplate _odpTemplate;

            private IDbParameters _batchParameters;
            private int _bindCount;

            public BatchExecutor(OracleOdpTemplate odpTemplate)
            {
                _odpTemplate = odpTemplate;
            }

            public void SetValues(IDbCommand dbCommand)
            {
                ((OracleCommand)dbCommand).ArrayBindCount = _bindCount;
                ParameterUtils.CopyParameters(dbCommand, _batchParameters);
            }

            #region IBatchExecutor Members

            public int ExecuteNonQuery<T>(
                IAdoOperations operation,
                CommandType cmdType,
                string cmdText,
                ICollection<T> data,
                Converter<T, IDbParameters> dataToParamters)
            {
                int totalRows = data.Count;
                int batchSize = _odpTemplate.BatchSize;
                if (totalRows < batchSize) batchSize = totalRows;

                int count = 0, bindCount = 0, result = 0;
                object[][] valueBuffer = null;
                
                foreach (T row in data)
                {
                    IDbParameters parameters = dataToParamters(row);
                    if (parameters != null)
                    {
                        if (valueBuffer == null)
                        {
                            valueBuffer = InitBatchParameters(parameters, batchSize);
                        }

                        string error = ValidateAndCopyParams(parameters, valueBuffer, bindCount++);
                        if (error != null)
                        {
                            throw new InvalidDataAccessApiUsageException(error + " for row: " + row);

                        }
                    }
                    ++count;
                    if (bindCount == batchSize || (count == totalRows) && bindCount > 0)
                    {
                        _bindCount = bindCount;
                        result += operation.ExecuteNonQuery(cmdType, cmdText, this);
                        bindCount = 0;
                    }

                }
                return result;
            }

            #endregion

            private string ValidateAndCopyParams(IDbParameters parameters, object[][] valueBuffer, int bindCount)
            {
                if (valueBuffer.Length != parameters.Count)
                {
                    return "Batch parameter count mismatch. Expected " + valueBuffer.Length +
                           " but got " + parameters.Count;
                }
                for (int i = 0; i < valueBuffer.Length; i++)
                {
                    string name = _batchParameters[i].ParameterName;
                    IDataParameter parameter = parameters[i];
                    if (parameter == null || !parameter.ParameterName.Equals(name))
                    {
                        parameter = parameters[name];
                        if (parameter == null) {
                            return "Batch parameter " + name + " is missing";
                        }
                    }
                    valueBuffer[i][bindCount] = parameter.Value;
                }
                return null; // success
            }

            private object[][] InitBatchParameters(IDbParameters parameters, int batchSize)
            {
                int totalParams = parameters.Count;
                IDbParameters batchParams = _odpTemplate.CreateDbParameters();
                object[][] valueBuffer = new object[totalParams][];
                for (int i = 0; i < totalParams; i++)
                {
                    IDbDataParameter p = (IDbDataParameter)parameters[i];

                    valueBuffer[i] = new object[batchSize];
                    batchParams.AddParameter(
                        p.ParameterName, p.DbType, p.Size, p.Direction, p.IsNullable,
                        p.Precision, p.Scale, p.SourceColumn, p.SourceVersion, valueBuffer[i]);
                }
                _batchParameters = batchParams;
                return valueBuffer;
            }
        }
    }
}
