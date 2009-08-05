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

namespace Spring.Data.Common
{
    /// <summary>
    /// An convenient base class for  of <see cref="IDbConnectionStateListener"/> 
    /// implementations that are only interested in connection open and/or
    /// close.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public class DbConnectionStateListenerBase : IDbConnectionStateListener
    {

        #region IDbConnectionStateListener Members

        /// <summary>
        /// To be called when the <paramref name="connection"/> state changed
        /// from <paramref name="original"/> to <paramref name="current"/>.
        /// </summary>
        /// <param name="connection">
        /// The database connection that its state has changed. 
        /// </param>
        /// <param name="original">
        /// The original state of the connection before change.
        /// </param>
        /// <param name="current">
        /// Current state of the connection.
        /// </param>
        public virtual void AfterStateChange(IDbConnection connection, ConnectionState original, ConnectionState current)
        {
            if (original == ConnectionState.Closed && current == ConnectionState.Open)
            {
                AfterConnectionOpen(connection);
            }
            else if (original != ConnectionState.Closed && current == ConnectionState.Closed)
            {
                AfterConnectionClose(connection);
            }

        }

        #endregion

        /// <summary>
        /// This method is called after database is opened.
        /// </summary>
        /// <param name="connection">
        /// The connection that was just opened.
        /// </param>
        public virtual void AfterConnectionOpen(IDbConnection connection)
        {
            
        }

        /// <summary>
        /// This method is called after database is closed.
        /// </summary>
        /// <param name="connection">
        /// The connection that was just closed.
        /// </param>
        public virtual void AfterConnectionClose(IDbConnection connection)
        {
            
        }
    }
}