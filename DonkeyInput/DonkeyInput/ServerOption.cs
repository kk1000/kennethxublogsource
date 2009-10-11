#region License
/*
* Copyright (C) 2002-2009 the original author or authors.
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

namespace DonkeyInput
{
    /// <summary>
    /// Class represent mldonkey core server options.
    /// </summary>
    /// <author>Kenneth Xu</author>
    internal class ServerOption
    {
        public const string DefaultServer = "127.0.0.1";
        public const int DefaultPort = 4080;
        public const string DefaultUserName = "admin";
        public const string DefaultPassword = "";

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ServerOption)) return false;
            return Equals((ServerOption) obj);
        }

        public bool Equals(ServerOption other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.UserName, UserName) && Equals(other.Password, Password) && Equals(other.Server, Server) && other.Port == Port;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (UserName != null ? UserName.GetHashCode() : 0);
                result = (result*397) ^ (Password != null ? Password.GetHashCode() : 0);
                result = (result*397) ^ (Server != null ? Server.GetHashCode() : 0);
                result = (result*397) ^ Port;
                return result;
            }
        }
    }
}