using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Peteisace.DataAccess.Client.Configuration;

namespace Peteisace.DataAccess.Client
{
    public class DBHelper
    {
        private Dictionary<string, ConnectionStringsInfo> connectionStrings = new Dictionary<string, ConnectionStringsInfo>();

        public DBHelper(ConnectionStringsOptions configuration)
        {
            foreach(var cStringOpts in configuration.ConnectionStrings)
            {
                // Reference by name.
                this.connectionStrings.Add(cStringOpts.Name, cStringOpts);
            }
        }

        public DBHelper(IConfiguration configuration) :
            this(configuration.GetSection(ConnectionStringsOptions.CONNECTIONSTRINGS_OPTIONS).Get<ConnectionStringsOptions>())
        {            
        }

        public async Task ExecuteReaderAsync(string connectionStringName, string storedProcedure, ReaderActionDelegate readerAction, params object[] parameters)
        {
            
        }
    }
}
