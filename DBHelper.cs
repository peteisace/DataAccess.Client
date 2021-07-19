using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Peteisace.DataAccess.Client.Configuration;
using System.Linq;
using System;

namespace Peteisace.DataAccess.Client
{
    public class DBHelper
    {
        private Dictionary<string, ConnectionStringsInfo> connectionStrings = new Dictionary<string, ConnectionStringsInfo>();

        public DBHelper(ConnectionStringsOptions configuration)
        {
            foreach (var cStringOpts in configuration.ConnectionStrings)
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
            ConnectionStringsInfo value = null;
            this.connectionStrings.TryGetValue(connectionStringName, out value);
            // Now we have the information grab the provider.
            await this.ExecuteReaderAsync(value.ProviderName, value.ConnectionString, storedProcedure, CommandType.StoredProcedure, readerAction, true, parameters);
        }

        public async Task ExecuteReaderAsync(string providerName, string connectionString, string storedProcedure, ReaderActionDelegate readerAction, params object[] parameters)
        {
            await this.ExecuteReaderAsync(providerName, connectionString, storedProcedure, CommandType.StoredProcedure, readerAction, true, parameters);
        }

        public async Task ExecuteReaderAsync(string providerName, string connectionString, string storedProcedure, ReaderActionDelegate readerAction, bool deriveParameters, params object[] parameters)
        {            
            if(!deriveParameters && parameters.Length > 0 && !parameters.All(p => p is IParameterInfo))
            {
                // This is bad.
                throw new InvalidOperationException("Must use instances of IParameterInfo as the parameter collection when using this method.");
            }
            await this.ExecuteReaderAsync(providerName, connectionString, storedProcedure, CommandType.StoredProcedure, readerAction, deriveParameters, parameters);
        }

        public async Task ExecuteReaderAsync(string connectionStringName, string commandText, ReaderActionDelegate readerAction, params IParameterInfo[] parameters)
        {
            ConnectionStringsInfo value = null;
            this.connectionStrings.TryGetValue(connectionStringName, out value);
            // Now we have the information grab the provider.
            await this.ExecuteReaderAsync(value.ProviderName, value.ConnectionString, commandText, CommandType.Text, readerAction, false, parameters);
        }

        public async Task ExecuteReaderAsync(string providerName, string connectionString, string commandText, ReaderActionDelegate readerAction, params IParameterInfo[] parameters)
        {            
            await this.ExecuteReaderAsync(providerName, connectionString, commandText, CommandType.Text, readerAction, false, parameters);
        }
    
        private async Task ExecuteReaderAsync(string providerName, string connectionString, string commandText, CommandType commandType, ReaderActionDelegate readerAction, bool deriveParameters, object[] parameters)
        {
            // Grab the provider factory.
            var providerFactory = DbProviderFactories.GetFactory(providerName);

            // Instantiate connection
            using (var connection = providerFactory.CreateConnection())
            {
                // Set the connection string.
                connection.ConnectionString = connectionString;

                // Set the stored procedure
                using (var command = providerFactory.CreateCommand())
                {
                    // Set the stored procedure.
                    command.CommandType = commandType;

                    // Open the connection.
                    await connection.OpenAsync();

                    if (deriveParameters)
                    {
                        // Get the parameters
                        var dbParameters = DBExecutor.ParameterCache.GetAgnosticParameters(command, providerFactory);

                        // Add they values
                        for (int i = 0; i < dbParameters.Length; i++)
                        {
                            dbParameters[i].Value = parameters[i];
                        }

                        parameters = dbParameters;
                    }
                    else
                    {
                        if(!parameters.All(p => p is IParameterInfo))
                        {
                            throw new InvalidOperationException("Not using DeriveParameters but parameters passed not of type IParameterInfo.");
                        }
                        List<IDbDataParameter> dbDataParameters = new List<IDbDataParameter>();
                        // Go through our parameter info
                        foreach(var info in parameters)
                        {
                            IParameterInfo parameter = info as IParameterInfo;
                            if(parameter != null)
                            {
                                var dbParameter = providerFactory.CreateParameter();
                                dbParameter.ParameterName = parameter.Name;
                                dbParameter.Value = parameter.Value;
                                // Direction?

                                dbDataParameters.Add(dbParameter);
                            }
                        }

                        // And set to be the array. 
                        parameters = dbDataParameters.ToArray();
                    }

                    // Parameters has now been changed to be, or is assumed to be, an array of IDbDataParameter
                    // checked the IDbProvider and CreateParameter does not attach the parameter to the command
                    // so they need to be separately addded.
                    command.Parameters.AddRange(parameters);

                    // Run it.
                    using (var reader = new SimpleDataReader(await command.ExecuteReaderAsync(CommandBehavior.CloseConnection)))
                    {
                        // Pass it the reader.
                        readerAction.Invoke(reader);
                    }

                }
            }
        }
    }
}
