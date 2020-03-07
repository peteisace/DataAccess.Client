using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace Peteisace.DataAccess.Client
{
    public static class DBExecutor
    {
        private delegate Task<object> CommandActionDelegate(SqlCommand command);
                
        public static async Task ExecuteReader(string connectionString, string procName, ReaderActionDelegate readerAction, params object[] parameters)
        {
            await OpenConnectionAndExec(connectionString, procName, async (cmd) => {

                var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                readerAction.Invoke(new SimpleDataReader(reader));

                return 1;

            }, parameters);
        }

        public static async Task<object> ExecuteScalar(string connectionString, string procName, params object[] parameters)
        {
            var scalar = await OpenConnectionAndExec(connectionString, procName, async (cmd) => {

                return await cmd.ExecuteScalarAsync();

            }, parameters);

            return scalar;
        }   

        public static async Task ExecuteNonQuery(string connectionString, string procName, params object[] parameters)
        {
            var affected = (int)await OpenConnectionAndExec(connectionString, procName, async (cmd) => {

                return await cmd.ExecuteNonQueryAsync();

            }, parameters);
        }     
        
        private static async Task<object> OpenConnectionAndExec(string connectionString, string commandText, CommandActionDelegate actionDelegate, object[] parameters)
        {
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                using(SqlCommand command = new SqlCommand(commandText, conn))
                {
                    // Setup parameters
                    foreach(var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                    
                    // Open
                    await conn.OpenAsync();

                    // Execute action
                    return await actionDelegate.Invoke(command);
                }
            }
        }
    }
}