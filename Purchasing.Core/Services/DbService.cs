using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace Purchasing.Core.Services
{
    public interface IDbService
    {
        DbConnection GetConnection(string connectionString = null);
    }

    public class DbService : IDbService
    {
        public DbConnection GetConnection(string connectionString = null)
        {
            //If connection string is null, use the default sql ce connection
            if (connectionString == null)
            {
                connectionString = ConfigurationManager.ConnectionStrings["MainDb"].ConnectionString;
            }

            var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }
    }
}