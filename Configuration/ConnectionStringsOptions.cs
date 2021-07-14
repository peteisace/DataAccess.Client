using System;
using System.Collections.Generic;

namespace Peteisace.DataAccess.Client.Configuration
{
    public class ConnectionStringsOptions
    {
        public const string CONNECTIONSTRINGS_OPTIONS = "connectionStringOptions";

        public ConnectionStringsOptions()
        {
        }

        public List<ConnectionStringsInfo> ConnectionStrings
        {
            get;
            set;
        }
    }
}
