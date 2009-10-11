using Microsoft.Win32;

namespace DonkeyInput
{
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
