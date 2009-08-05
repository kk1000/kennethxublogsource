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

using System.Data;
using System.Threading;
using Spring.Data.Common;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Attributes;

namespace Spring.Data.Support
{
    /// <summary>
    /// An implementation of <see cref="IDbConnectionStateListener"/> that
    /// executes a stored procedure or update statement with the name of 
    /// current thread's principal.
    /// </summary>
    /// <remarks>
    /// This is important in the case of pooled connections which are all
    /// authenticated to database with same fixed user id. With the client
    /// identity set on the orcle session, database can use this information
    /// to perform logging and/or access control.
    /// </remarks>
    /// <author>Kenneth Xu</author>
    public class CurrentTheadIdentityToDbSetter : DbConnectionStateListenerBase
    {
        private CommandType _commandType = CommandType.StoredProcedure;

        /// <summary>
        /// The type of command to execute to set the user name.
        /// </summary>
        public CommandType CommandType
        {
            set { _commandType = value; }
        }

        /// <summary>
        /// The text to execute.
        /// </summary>
        [Required] public string CommandText { private get; set; }

        /// <summary>
        /// The name of parameter to set the current user name.
        /// </summary>
        public string ParameterName { private get; set; }

        /// <summary>
        /// Run a stored procedure or update statement with parameter values as
        /// <c>Thread.CurrentPrincipal.Identity.Name</c> upon connection open.
        /// </summary>
        /// <param name="connection">
        /// The connection that was just opened.
        /// </param>
        public override void AfterConnectionOpen(IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = CommandText;
                command.CommandType = _commandType;
                IDataParameter parameter = command.CreateParameter();
                if (!string.IsNullOrEmpty(ParameterName)) parameter.ParameterName = ParameterName;
                parameter.DbType = DbType.String;
                parameter.Value = Thread.CurrentPrincipal.Identity.Name;
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
    }
}
