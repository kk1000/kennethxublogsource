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
    /// Manages the browser protocol hander
    /// </summary>
    /// <author>Kenneth Xu</author>
    class ProtocolHandler
    {
        private const string _protocolKey = @"HKEY_CLASSES_ROOT\{0}";
        private const string _protocolCommandKey = @"HKEY_CLASSES_ROOT\{0}\shell\open\command";

        public static void RegisterProtocolHandler()
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
