using System.Data;
using System.Data.SqlClient;

namespace Axiom.Data.Repository
{
    public interface IDBAccess<T> where T : class
    {
        SqlParameter GetSQLParam(string paramName, SqlDbType type, dynamic value);

        DataSet QuerySQL(string storedProcedureName, params SqlParameter[] parameters);

        dynamic QueryScalarSQL(string storedProcedureName, params SqlParameter[] parameters);

        void ExecuteSQL(string storedProcedureName, params SqlParameter[] parameters);
    }
}
