using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Peteisace.DataAccess.Client
{
    public class SqlParameterCache
    {
        private Dictionary<string, SqlParameter[]> _cache = new Dictionary<string, SqlParameter[]>();

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
            return this._cache[key];
        }
    }
}