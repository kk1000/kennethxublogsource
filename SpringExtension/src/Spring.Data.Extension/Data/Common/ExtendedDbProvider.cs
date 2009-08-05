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
using System.Data.Common;
using Common.Logging;

namespace Spring.Data.Common
{
    /// <summary>
    /// <see cref="ExtendedDbProvider"/> wraps an <see cref="IDbProvider"/>
    /// and provide additional features.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class ExtendedDbProvider : DelegatingDbProvider
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ExtendedDbProvider));

        private volatile bool _isNonDbConnectionErrorGiven;

        /// <summary>
        /// Gets and sets a connection state listener to be notified about 
        /// connection state changes.
        /// </summary>
        public IDbConnectionStateListener ConnectionStateListener { get; set; }

        /// <summary>
        /// Returns a new connection object to communicate with the database.
        /// </summary>
        ///
        /// <returns>
        /// A new <see cref="IDbConnection"/> object.
        /// </returns>
        ///
        public override IDbConnection CreateConnection()
        {
            IDbConnection conn = TargetDbProvider.CreateConnection();

            var listener = ConnectionStateListener;
            if (listener != null)
            {
                var dbConnection = conn as DbConnection;
                if (dbConnection != null)
                {
                    dbConnection.StateChange += StateChangeEventHandler;
                }
                else if (!_isNonDbConnectionErrorGiven && _log.IsErrorEnabled)
                {
                    _log.Error(string.Format(
                                   "Unable to detected connection state change as the " +
                                   "connection object from {0} doesn't inherit from {1}. " +
                                   "{2} will not be called.", 
                                   TargetDbProvider, 
                                   typeof (DbConnection).FullName,
                                   listener));
                    _isNonDbConnectionErrorGiven = true;
                }
            }
            return conn;
        }

        private void StateChangeEventHandler(object sender, StateChangeEventArgs e)
        {
            IDbConnectionStateListener listener = ConnectionStateListener;
            if (listener != null)
            {
                listener.AfterStateChange((IDbConnection)sender, e.OriginalState, e.CurrentState);
            }
        }

    }
}