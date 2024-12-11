using Microsoft.Data.SqlClient;
using System.Data;

namespace WEB_BA.DataHandler
{
    public class DataHandlerSQL
    {
        private readonly string _connectionString;

        public DataHandlerSQL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is not configured.");

        }

        public async Task<DataTable> ExecuteStoredProcedureAsync(string storedProcedureName, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader); // The actual loading is synchronous, but retrieval is async
                }
            }
            return dt;
        }
    }
}
