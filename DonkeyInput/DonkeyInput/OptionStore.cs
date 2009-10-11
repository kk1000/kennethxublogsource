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
using Microsoft.Win32;

namespace DonkeyInput
{
    /// <summary>
    /// Operations to save <see cref="ServerOption"/> to and retrieve it from
    /// persistent store.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class OptionStore
    {
        private const string _protocolKey = @"HKEY_CLASSES_ROOT\{0}";
        private const string _protocolCommandKey = @"HKEY_CLASSES_ROOT\{0}\shell\open\command";
        private const string _donkeyInputKey = @"HKEY_CURRENT_USER\Software\Sharneng\DonkeyInput";
        private const string _valueNamePort = "Port";
        private const string _valueNameUserName = "UserName";
        private const string _valueNamePassword = "Password";
        private const string _valueNameServer = "Server";

        public ServerOption Value
        {
            get
            {
                return new ServerOption
                           {
                               Server = GetValue(_valueNameServer, ServerOption.DefaultServer),
                               Port = GetValue(_valueNamePort, ServerOption.DefaultPort),
                               UserName = GetValue(_valueNameUserName, ServerOption.DefaultUserName),
                               Password = GetValue(_valueNamePassword, ServerOption.DefaultPassword)
                           };
            }
            set
            {
                SetValue(_valueNameServer, value.Server);
                SetValue(_valueNamePort, value.Port);
                SetValue(_valueNameUserName, value.UserName);
                SetValue(_valueNamePassword, value.Password);
            }
        }

        private static T GetValue<T>(string name, T defaultValue)
        {
            object value = Registry.GetValue(_donkeyInputKey, name, defaultValue);
            return value == null ? defaultValue : (T)value;
        }

        private static void SetValue<T>(string name, T value)
        {
            Registry.SetValue(_donkeyInputKey, name, value);
        }

        public void RegisterProtocolHandler()
        {
            var command = "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\" \"%1\"";
            RegisterProtocol("ed2k", command);
        }

        private static void RegisterProtocol(string protocol, string command)
        {
            string protocolCommandKey = string.Format(_protocolCommandKey, protocol);
            string oldCommand = (string) Registry.GetValue(protocolCommandKey,null, null);
            if (oldCommand==command) return;

            var protocolKey = string.Format(_protocolKey, protocol);
            Registry.SetValue(protocolKey, null, string.Format("URL: {0} Protocol", protocol));
            Registry.SetValue(protocolKey, "URL Protocol", string.Empty);
            Registry.SetValue(protocolCommandKey, null, command);
        }
    }
}
