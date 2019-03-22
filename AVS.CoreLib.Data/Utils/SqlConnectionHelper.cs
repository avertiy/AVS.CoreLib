using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AVS.CoreLib.Data.Utils
{
    public static class SqlConnectionHelper
    {
        public static bool TestConnectionString(string connectionString)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("");
                Debug.WriteLine("Open connection failed: " + ex.Message, "SqlConnection");
                Debug.WriteLine("");
                return false;
            }

        }
        /// <summary>
        /// Get the connection string from connectionStrings section of the app config
        /// and tests the specified connection string trying to open the SqlConnection
        /// </summary>
        public static string GetConnectionString(string name)
        {
            var str = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
            if(string.IsNullOrEmpty(str))
                throw new ApplicationException($"{name} connection string is missing in app config");
            
            if (!TestConnectionString(str))
                throw new ApplicationException($"Unable to open SqlConnection using the \"{name}\" connection string [{str}]");
            return str;
        }
    }
}