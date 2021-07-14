using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace Peteisace.DataAccess.Client
{
    public static class DBExecutor
    {
        private static SqlParameterCache _cache = new SqlParameterCache();
        private delegate Task<object> CommandActionDelegate(SqlCommand command);

        internal static SqlParameterCache ParameterCache => _cache;
                
        public static async Task ExecuteReader(string connectionString, string procName, ReaderActionDelegate readerAction, params object[] parameters)
        {
            await OpenConnectionAndExec(connectionString, procName, async (cmd) => {

                var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                readerAction.Invoke(new SimpleDataReader(reader));

                return 1;

            }, true, parameters);
        }

        public static async Task<object> ExecuteScalar(string connectionString, string procName, params object[] parameters)
        {
            var scalar = await OpenConnectionAndExec(connectionString, procName, async (cmd) => {

                return await cmd.ExecuteScalarAsync();

            }, true, parameters);

            return scalar;
        }   

        public static async Task ExecuteNonQuery(string connectionString, string procName, params object[] parameters)
        {
            var affected = (int)await OpenConnectionAndExec(connectionString, procName, async (cmd) => {

                return await cmd.ExecuteNonQueryAsync();

            }, true, parameters);
        }     
                
        private static async Task<object> OpenConnectionAndExec(string connectionString, string commandText, CommandActionDelegate actionDelegate, bool derive, object[] parameters)
        {                        
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                using(SqlCommand command = new SqlCommand(commandText, conn))
                {   
                    // sp 
                    command.CommandType = CommandType.StoredProcedure;
                    
                    // Open
                    await conn.OpenAsync();

                    if(derive)
                    {
                        // get the parameters
                        var sqlParameters = _cache.Get(command);

                        // add they values
                        for(int i = 0; i < sqlParameters.Length; i++)
                        {
                            sqlParameters[i].Value = parameters[i];
                        }

                        // Change var
                        parameters = sqlParameters;
                    }
                    
                    // Add the list of sql parameters.
                    command.Parameters.AddRange(parameters);
                    
                    // Execute action
                    return await actionDelegate.Invoke(command);                    
                }
            }
        }
    }
}