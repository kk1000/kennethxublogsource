namespace DonkeyInput
{
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