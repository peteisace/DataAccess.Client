using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Peteisace.DataAccess.Client
{
    public class SqlParameterCache
    {
        private Dictionary<string, IDbDataParameter[]> _cache = new Dictionary<string, IDbDataParameter[]>();

        public SqlParameter[] Get(SqlCommand cmd)
        {
            // Grab the connection
            SqlConnection c = cmd.Connection;

            // Ensure we are sp
            if(cmd.CommandType != CommandType.StoredProcedure)
            {
                throw new InvalidOperationException("Deriving parameters works only with stored procedures.");
            }

            // Create key
            var key = $"{c.ConnectionString.GetHashCode()}_{cmd.CommandText.GetHashCode()}";

            // Try get result
            if(!this._cache.ContainsKey(key))
            {
                // Check open
                if(c.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must be open when deriving parameters. Open the connection earlier.");
                }

                // Horrid... Create then remove
                SqlCommandBuilder.DeriveParameters(cmd);
                cmd.Parameters.RemoveAt(0); // Stupid return_value
                // Now copy to array
                SqlParameter[] sqlParameters = new SqlParameter[cmd.Parameters.Count];
                cmd.Parameters.CopyTo(sqlParameters, 0);
                // Store
                this._cache.Add(key, sqlParameters);
                // Clear
                cmd.Parameters.Clear();
            }

            // We know we will have value.
            return (SqlParameter[])this._cache[key];
        }

        public IDbDataParameter[] GetAgnosticParameters(IDbCommand command, DbProviderFactory dbProviderFactory)
        {
            // Grab the connection
            IDbConnection c = command.Connection;

            // Ensure we are sp.
            if(command.CommandType != CommandType.StoredProcedure)
            {
                throw new InvalidOperationException("Deriving parameters works only with stored procedures.");
            }

            // Create the key
            var key = $"{c.ConnectionString.GetHashCode()}_{command.CommandText.GetHashCode()}";

            // Try to get the result.
            if(!this._cache.ContainsKey(key))
            {
                // Check open
                if(c.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must be open when deriving parameters. Open the connection earlier.");
                }

                // Horrid
                var commandBuilder = dbProviderFactory.CanCreateCommandBuilder ? dbProviderFactory.CreateCommandBuilder() : null;
                if(commandBuilder != null)
                {
                    // well then call the static method
                    var methods = commandBuilder.GetType().GetMethods(BindingFlags.Static | BindingFlags.Public);
                    // Look for derive parameters.
                    foreach(var m in methods)
                    {
                        if(m.Name == "DeriveParameters" && m.GetParameters().Length == 1)
                        {
                            // Call the damn method
                            m.Invoke(null, new object[] { command });

                            // So now we should be good and have a nice list of parameters
                            command.Parameters.RemoveAt(0); // Stupid return value.
                            // Copy to array
                            IDbDataParameter[] parameters = new IDbDataParameter[command.Parameters.Count];
                            command.Parameters.CopyTo(parameters, 0);
                            // Store
                            this._cache.Add(key, parameters);
                            // Clear
                            command.Parameters.Clear();                            
                        }
                    }
                }                
            }

            throw new InvalidOperationException("Derive parameters not supported by your data provider.");
        }
    }
}