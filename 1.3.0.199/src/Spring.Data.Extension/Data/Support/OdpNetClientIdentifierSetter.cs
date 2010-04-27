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

using System.Threading;
using Oracle.DataAccess.Client;
using Spring.Data.Common;

namespace Spring.Data.Support
{
    /// <summary>
    /// An implementation of <see cref="IDbConnectionStateListener"/> that sets
    /// <see cref="OracleConnection.ClientId"/> to the name of current 
    /// thread's principal.
    /// </summary>
    /// <remarks>
    /// This is important in the case of pooled connections which are all
    /// authenticated to database with same fixed user id. With the client
    /// identity set on the orcle session, database can use this information
    /// to perform logging and/or access control.
    /// </remarks>
    /// <author>Kenneth Xu</author>
    public class OdpNetClientIdentifierSetter : DbConnectionStateListenerBase
    {
        /// <summary>
        /// <see cref="OracleConnection.ClientId"/> is set to 
        /// <c>Thread.CurrentPrincipal.Identity.Name</c> upon connection open.
        /// </summary>
        /// <param name="connection">
        /// The connection that was just opened.
        /// </param>
        public override void AfterConnectionOpen(System.Data.IDbConnection connection)
        {
            ((OracleConnection) connection).ClientId = Thread.CurrentPrincipal.Identity.Name;
        }
    }
}
