namespace Peteisace.DataAccess.Client.Configuration
{
    public class ConnectionStringsInfo
    {
        public const string CONNECTIONSTRINGS = "connectionStrings";

        public ConnectionStringsInfo()
        {
        }

        public string Name
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get;
            set;
        }

        public string ProviderName
        {
            get;
            set;
        }
    }
}
