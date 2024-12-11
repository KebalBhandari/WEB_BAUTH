using Microsoft.Data.SqlClient;
using System.Data;
using WEB_BA.DataHandler;

namespace WEB_BA.DataProvider
{
    public class AuthDataProvider
    {
        private readonly DataHandlerSQL _dataHandler;

        public AuthDataProvider(IConfiguration configuration)
        {
            _dataHandler = new DataHandlerSQL(configuration);
        }

        public async Task<(string Status, string Message, string RefreshToken)> ValidateLoginAndUpdateSession(string email, string password, string ipAddress, string userAgent)
        {
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@UserEmail", email),
            new SqlParameter("@Password", password),
            new SqlParameter("@IP_Address", ipAddress),
            new SqlParameter("@User_Agent", userAgent)
            };

            DataTable dt = await _dataHandler.ExecuteStoredProcedureAsync("sp_ValidateLoginAndUpdateSession", parameters);

            if (dt.Rows.Count > 0)
            {
                string? status = dt.Rows[0]["Status"]?.ToString();
                string? message = dt.Rows[0]["Message"]?.ToString();
                string? refreshToken = dt.Columns.Contains("RefreshToken") ? dt.Rows[0]["RefreshToken"]?.ToString() : null;
                if (status == null || message == null || refreshToken == null)
                {
                    return ("ERROR", "No response from stored procedure", "NULL");
                }
                else {
                    return (status, message, refreshToken);
                }
            }

            return ("ERROR", "No response from stored procedure", "NULL");
        }

        public async Task<bool> InvalidateSessionAsync(string refreshToken)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PlainRefreshToken", refreshToken)
            };

            DataTable dt = await _dataHandler.ExecuteStoredProcedureAsync("sp_InvalidateSession", parameters);

            if (dt.Rows.Count > 0 && dt.Rows[0]["Status"]?.ToString() == "SUCCESS")
            {
                return true;
            }

            return false;
        }
    }
}
