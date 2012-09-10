using System.Data.Common;
using System.Web.Configuration;
using System.Data.SqlClient;

namespace Purchasing.Web.Services
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
                connectionString = WebConfigurationManager.ConnectionStrings["MainDb"].ConnectionString;
            }

            var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }
    }
}