using System;
using System.Net;
using System.Web;

namespace DonkeyInput
{
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
