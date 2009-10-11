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
using System;
using System.Net;
using System.Web;

namespace DonkeyInput
{
    /// <summary>
    /// A download link request that can be sent to mldonkey core.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class DownloadRequest
    {
        public ServerOption Option { get; set; }

        public string[] Links { get; set; }

        public void Send()
        {
            foreach (var link in Links)
            {
               SendOne(link);
            }
        }

        public void SendOne(string link)
        {
            var wc = new WebClient
            {
                Credentials = new NetworkCredential(Option.UserName, Option.Password)
            };
            var url = string.Format("http://{0}:{1}/submit?q={2}", 
                Option.Server, Option.Port, HttpUtility.UrlEncode(link));
            var data = wc.OpenRead(url);
            while (data.ReadByte() != -1) { }
            data.Close();
        }

        public static string[] ParseLinks(string linkString)
        {
            return linkString.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
