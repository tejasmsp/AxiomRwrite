using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;


namespace Viewer.Class
{
    public static class DbAccess
    {
        public static string sConnectionString = ConfigurationManager.ConnectionStrings["Axiom"].ConnectionString;

        public static DataTable GetDataList(SqlCommand cmd)
        {
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                cmd.Connection = objConn;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dt.TableName = "Result";
                return dt;
            }
        }

        public static DataTable GetFileOrderData(int FileId = 0, int OrderNo = 0, int PartNo = 0, string FileName = "")
        {
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "SignatureGetFileOrder"
            };
            cmd.Parameters.Add("@FileId", SqlDbType.Int).Value = FileId;
            cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            cmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = PartNo;
            cmd.Parameters.Add("@FileName", SqlDbType.VarChar).Value = FileName;
            return GetDataList(cmd);
        }
        public static bool GetOrderByBirthDate(int OrderNo, string DOB)
        {
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "SignatureGetOrderByBirthDate"
            };
            cmd.Parameters.Add("@DateOfBirth", SqlDbType.VarChar).Value = DOB;
            cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;

            var result = GetDataList(cmd);
            if (result != null && result.Rows.Count > 0)
                return true;
            return false;
        }



        public static void InsertFile(FileEntity file)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = file.OrderNo;
            sqlCmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = file.PartNo;
            sqlCmd.Parameters.Add("@FileName", SqlDbType.VarChar).Value = file.FileName;
            sqlCmd.Parameters.Add("@FileTypeId", SqlDbType.Int).Value = file.FileTypeId;
            sqlCmd.Parameters.Add("@IsPublic", SqlDbType.Bit).Value = file.IsPublic;
            sqlCmd.Parameters.Add("@RecordTypeId", SqlDbType.Int).Value = file.RecordTypeId;
            sqlCmd.Parameters.Add("@FileDiskName", SqlDbType.VarChar).Value = file.FileDiskName;
            sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = file.PageNo;
            sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = file.CreatedBy;
            sqlCmd.CommandText = "InsertFile";
            InsertUpdateData(sqlCmd);
        }

        private static void InsertUpdateData(SqlCommand cmd)
        {
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                cmd.Connection = objConn;
                cmd.ExecuteNonQuery();
            }
        }

    }
}
