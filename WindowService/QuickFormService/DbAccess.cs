using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using QuickFormService.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace QuickFormService
{


    public static class DbAccess
    {
        public static string sConnectionString = ConfigurationManager.ConnectionStrings["Axiom"].ConnectionString;

        public static DataTable GetDataList(SqlCommand cmd)
        {
            List<string> recordList = new List<string>();
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

        public static List<string> GetQuickFormUniqueRecords()
        {
            List<string> recordList = new List<string>();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceGetQuickFormUniqueRecords";
            cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = 0;
            var result = GetDataList(cmd);
            foreach (DataRow dr in result.Rows)
            {
                recordList.Add(Convert.ToString(dr["OrderNo"]));
            }
            return recordList;
        }

        public static DataTable GetQuickFormOrderDetails(int OrderNo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceGetQuickFormUniqueRecords";
            cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            return GetDataList(cmd);
        }

        public static DataTable GetQuickFormPrintformattachfiles(int QuickFormID, bool isDocument, bool isDocumentAttach)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceQuickFormPrintformattachfiles";
            cmd.Parameters.Add("@QuickFormID", SqlDbType.Int).Value = QuickFormID;
            if (isDocument)
                cmd.Parameters.Add("@IsDocument", SqlDbType.Int).Value = 1;
            if (isDocumentAttach)
                cmd.Parameters.Add("@IsDocumentAttachment", SqlDbType.Int).Value = 1;
            return GetDataList(cmd);
        }
        public static QueryType GetDocumentType(string documentName, string folderType)
        {
            if (!string.IsNullOrEmpty(documentName))
                documentName = documentName.Trim().ToUpper();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetDocuments";
            cmd.Parameters.Add("@DocFileName", SqlDbType.VarChar).Value = documentName;
            cmd.Parameters.Add("@FolderName", SqlDbType.VarChar).Value = folderType;
            var result = GetDataList(cmd);
            if (result != null && result.Rows.Count > 0)
                return (QueryType)Convert.ToInt32(Enum.Parse(typeof(QueryType), Convert.ToString(result.Rows[0]["QueryType"])));
            else
                return new QueryType();
        }
        public static string GetQueryByQueryTypeId(int QueryTypeID, string str)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetQueries";
            cmd.Parameters.Add("@QueryTypeID", SqlDbType.Int).Value = QueryTypeID;
            var result = GetDataList(cmd);
            if (result != null && result.Rows.Count > 0)
            {
                if (str == "Query")
                    return Convert.ToString(result.Rows[0]["Query"]);
                else if (str == "SubQuery")
                    return Convert.ToString(result.Rows[0]["SubQuery"]);
            }
            return "";
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

        public static void UpdateDocumentStatus(int QuickFormId)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@QuickFormId", SqlDbType.Int).Value = QuickFormId;
            sqlCmd.Parameters.Add("@IsDocumentStatus", SqlDbType.Bit).Value = true;
            sqlCmd.CommandText = "ServiceQuickFormUpdateStatus";
            InsertUpdateData(sqlCmd);
        }

        public static void UpdatePrintStatus(int QuickFormId)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@QuickFormId", SqlDbType.Int).Value = QuickFormId;
            sqlCmd.Parameters.Add("@IsPrintStatus", SqlDbType.Bit).Value = true;
            sqlCmd.CommandText = "ServiceQuickFormUpdateStatus";
            InsertUpdateData(sqlCmd);
        }
        public static void UpdateEmailStatus(int QuickFormId)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@QuickFormId", SqlDbType.Int).Value = QuickFormId;
            sqlCmd.Parameters.Add("@IsEmailStatus", SqlDbType.Bit).Value = true;
            sqlCmd.CommandText = "ServiceQuickFormUpdateStatus";
            InsertUpdateData(sqlCmd);
        }
        public static void UpdateFaxStatus(int QuickFormId)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@QuickFormId", SqlDbType.Int).Value = QuickFormId;
            sqlCmd.Parameters.Add("@ISFaxStatus", SqlDbType.Bit).Value = true;
            sqlCmd.CommandText = "ServiceQuickFormUpdateStatus";
            InsertUpdateData(sqlCmd);
        }
        public static DataTable GetNotificationEmails(int OrderNo, string OrderAttorney)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            sqlCmd.Parameters.Add("@OrderAttorney", SqlDbType.VarChar).Value = OrderAttorney;
            sqlCmd.Parameters.Add("@isFromQuickFormService", SqlDbType.Bit).Value = 1;
            sqlCmd.CommandText = "ServiceEmailReminderGetNotificationEmails";
            return GetDataList(sqlCmd);

        }
        public static DataTable GetAccntRepDetail(string acctrep)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAccntRepDetail";
            cmd.Parameters.Add("@AccntRep", SqlDbType.VarChar).Value = acctrep;
            return GetDataList(cmd);
        }
        public static DataTable GetOrderDetail(int OrderId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "QuickFormGetOrderDetails";
            cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = OrderId;
            return GetDataList(cmd);
        }


        public static DataTable GetQuickFormDetails(string QuickFormIds)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceQuickFormGetQuickFormDetails";
            cmd.Parameters.Add("@QuickFormIDs", SqlDbType.VarChar).Value = QuickFormIds;
            return GetDataList(cmd);
        }
        public static DataTable GetIRSFeesByYear()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceQuickFormIRSFeesByYear";
            return GetDataList(cmd);
        }

        public static void AddFilesToPart(FilesToPartEntity obj, string spName)
        {
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("OrderNo", obj.OrderNo);
                cmd.Parameters.AddWithValue("PartNo", obj.PartNo);
                cmd.Parameters.AddWithValue("FileName", obj.FileName);
                cmd.Parameters.AddWithValue("FileTypeId", obj.FileTypeId);
                cmd.Parameters.AddWithValue("RecordTypeId", obj.RecordTypeId);
                cmd.Parameters.AddWithValue("UserId", obj.UserId);
                cmd.Parameters.AddWithValue("IsPublic", obj.IsPublic);
                cmd.Parameters.AddWithValue("Pages", obj.Pages);
                cmd.Parameters.AddWithValue("FileDiskName", obj.FileDiskName);
                cmd.Connection = objConn;
                cmd.CommandText = spName;
                cmd.ExecuteNonQuery();

            }
        }



        public static DataTable ExecuteSQLQuery(string SQLQuery)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ExecuteSQLQuery";
            cmd.Parameters.Add("@SQLQuery", SqlDbType.VarChar).Value = SQLQuery;
            return GetDataList(cmd);
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }

}
