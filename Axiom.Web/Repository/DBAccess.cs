using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Axiom.Data.Repository
{
    public class DBAccess1<T> : IDBAccess<T> where T : class
    {
        private readonly string _connectionString;
        private const int IntCommandTimeout = 120;

        public DBAccess1()
        {           
            _connectionString = ConfigurationManager.ConnectionStrings["AxiomEntities"].ConnectionString;
        }

        public DBAccess1(string cs)
        {
            _connectionString = cs;
        }

        public SqlParameter GetSQLParam(string paramName, SqlDbType type, dynamic value)
        {
            SqlParameter sqlParam = new SqlParameter(paramName, type) {Value = value};
            return sqlParam;
        }

        public DataSet QuerySQL(string storedProcedureName, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = IntCommandTimeout;
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
            }
            return ds;
        }

        public dynamic QueryScalarSQL(string storedProcedureName, params SqlParameter[] parameters)
        {
            dynamic result;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = IntCommandTimeout;
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }
                    conn.Open();
                    result = cmd.ExecuteScalar();
                }
            }
            return result;
        }

        public void ExecuteSQL(string storedProcedureName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = IntCommandTimeout;
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public class DBAccess
    {
        private readonly string _connectionString;
        private const int IntCommandTimeout = 120;

        public DBAccess()
        {
            // _connectionString = @"Data Source=192.168.1.145\STASQL14;Initial Catalog=IACBI;Integrated Security=False;User Id=IACUser;Password=123;MultipleActiveResultSets=True";
            _connectionString = ConfigurationManager.ConnectionStrings["IAC_MDMEntitiesADONET"].ConnectionString;            
        }

        public DBAccess(string cs)
        {
            _connectionString = cs;
        }

        public SqlParameter GetSQLParam(string paramName, SqlDbType type, dynamic value)
        {
            SqlParameter sqlParam = new SqlParameter(paramName, type) { Value = value };
            return sqlParam;
        }

        public DataSet QuerySQL(string storedProcedureName, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = IntCommandTimeout;
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
            }
            return ds;
        }

        public dynamic QueryScalarSQL(string storedProcedureName, params SqlParameter[] parameters)
        {
            dynamic result;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = IntCommandTimeout;
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }
                    conn.Open();
                    result = cmd.ExecuteScalar();
                }
            }
            return result;
        }

        public void ExecuteSQL(string storedProcedureName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = IntCommandTimeout;
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
