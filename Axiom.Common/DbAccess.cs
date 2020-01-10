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
using System.IO;
using Axiom.Entity;

namespace Axiom.Common
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
                objConn.Close();
                return dt;
            }
        }

        public static void InsertUpdateData(SqlCommand cmd)
        {
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                cmd.Connection = objConn;
                cmd.ExecuteNonQuery();
                objConn.Close();
            }
        }

        public static List<T> ConvertDataTable<T>(DataTable dt) where T : class, new()
        {
            List<T> lstItems = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
                foreach (DataRow row in dt.Rows)
                    lstItems.Add(ConvertDataRowToGenericType<T>(row));
            else
                lstItems = null;
            return lstItems;
        }

        private static T ConvertDataRowToGenericType<T>(DataRow row) where T : class, new()
        {
            Type entityType = typeof(T);
            T objEntity = new T();
            foreach (DataColumn column in row.Table.Columns)
            {
                object value = row[column.ColumnName];
                if (value == DBNull.Value) value = null;
                PropertyInfo property = entityType.GetProperty(column.ColumnName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
                try
                {
                    if (property != null && property.CanWrite)
                        property.SetValue(objEntity, value, null);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return objEntity;
        }

        public static List<PartDetail> GetPartList()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceAxiomAutoGetPartList";
            cmd.Parameters.Add("@Dtcallback", SqlDbType.DateTime).Value = Convert.ToDateTime("Jan  1 1900 12:00AM");
            var result = GetDataList(cmd);
            var partList = result != null ? ConvertDataTable<PartDetail>(result) : null;
            return partList;
        }

        public static LocationEntity GetPartLocation(string LocID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetLocationById";
            cmd.Parameters.Add("@LocationId", SqlDbType.VarChar).Value = LocID;
            var result = GetDataList(cmd);
            var location = result != null ? ConvertDataTable<LocationEntity>(result).FirstOrDefault() : null;
            return location;
        }

        public static AttorneyEntity GetAttorneyByOrder(int OrderNo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAttorneyByOrder";
            cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            var result = GetDataList(cmd);
            var attyList = result != null ? ConvertDataTable<AttorneyEntity>(result) : null;
            return (attyList != null && attyList.Count > 0) ? attyList[0] : null;
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
                if (str.ToUpper() == "QUERY")
                    return Convert.ToString(result.Rows[0]["Query"]);
                else if (str.ToUpper() == "SUBQUERY")
                    return Convert.ToString(result.Rows[0]["SubQuery"]);
            }
            return "";
        }

        public static DataTable ExecuteSQLQuery(string SQLQuery)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ExecuteSQLQuery";
            cmd.Parameters.Add("@SQLQuery", SqlDbType.VarChar).Value = SQLQuery;
            return GetDataList(cmd);
        }

        public static void InsertFile(FileEntity file)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = file.OrderId;
            sqlCmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = file.PartNo;
            sqlCmd.Parameters.Add("@FileName", SqlDbType.VarChar).Value = file.FileName;
            sqlCmd.Parameters.Add("@FileTypeId", SqlDbType.Int).Value = file.FileTypeId;
            sqlCmd.Parameters.Add("@IsPublic", SqlDbType.Bit).Value = true;
            sqlCmd.Parameters.Add("@RecordTypeId", SqlDbType.Int).Value = 0;
            sqlCmd.Parameters.Add("@FileDiskName", SqlDbType.VarChar).Value = file.FileDiskName;
            sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
            sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = new Guid("7ABB0EFB-88A9-4699-B359-7F17216A8258");
            sqlCmd.CommandText = "InsertFile";
            InsertUpdateData(sqlCmd);
        }

        public static OrderDetailEntity GetOrderDetailByOrderNo(int OrderNo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "QuickFormGetOrderDetails";
            cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = OrderNo;
            var result = GetDataList(cmd);
            var orderList = result != null ? ConvertDataTable<OrderDetailEntity>(result) : null;
            return (orderList != null && orderList.Count > 0) ? orderList[0] : null;
        }

        public static AccntRepDetails GetAccntRepDetail(string acctrep)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAccntRepDetail";
            cmd.Parameters.Add("@AccntRep", SqlDbType.VarChar).Value = acctrep;
            var result = GetDataList(cmd);
            var acctRepList = result != null ? ConvertDataTable<AccntRepDetails>(result) : null;
            return (acctRepList != null && acctRepList.Count > 0) ? acctRepList[0] : null;
        }

        public static List<NotificationEmailEntity> GetAssistantContactNotification(int OrderNo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAssistantContactNotificationInformationByOrderId";
            cmd.Parameters.Add("@orderId", SqlDbType.Int).Value = OrderNo;
            var result = GetDataList(cmd);
            var notificationList = result != null ? ConvertDataTable<NotificationEmailEntity>(result) : null;
            return notificationList;
        }

        public static void GetAttachFileFromDB(int OrderNo, int partno, ref List<string> AttachFileName, ref MemoryStream[] msFile)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "QuickFormGetAttachFileList";
                cmd.Parameters.Add("@OrderNumber", SqlDbType.Int).Value = OrderNo;
                cmd.Parameters.Add("@PartNumber", SqlDbType.Int).Value = partno;
                var result = GetDataList(cmd);
                var attachedFileList = result != null ? ConvertDataTable<QuickFormDocumentAttachmentListEntity>(result) : null;
                if (attachedFileList != null && attachedFileList.Count > 0)
                {
                    int counter = AttachFileName.Count();
                    string uploadRoot = Path.Combine(ConfigurationManager.AppSettings["UploadRoot"].ToString());
                    foreach (var item in attachedFileList)
                    {
                        string finalPath = "";
                        int partNo = item.PartNo;
                        if (partNo <= 0)
                            finalPath = string.Format(@"{0}{1}\{2}", uploadRoot, OrderNo, item.FileDiskName);
                        else
                            finalPath = string.Format(@"{0}{1}\{2}\{3}", uploadRoot, OrderNo, partNo.ToString(), item.FileDiskName);
                        if (File.Exists(finalPath))
                        {
                            using (FileStream fstream = File.OpenRead(finalPath))
                            {
                                MemoryStream mstream = new MemoryStream();
                                mstream.SetLength(fstream.Length);
                                fstream.Read(mstream.GetBuffer(), 0, (int)fstream.Length);
                                mstream.Position = 0L;
                                msFile[counter] = mstream;
                                AttachFileName.Add(item.FileName);
                                counter++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ServicLog(ex.ToString());
            }

        }

        public static void UpdateQuickFormOrderPart(int OrderNo, int PartNo, string AsgnTo)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            sqlCmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = PartNo;
            sqlCmd.Parameters.Add("@AsgnTo", SqlDbType.VarChar).Value = AsgnTo;
            sqlCmd.CommandText = "UpdateQuickFormOrderPart";
            InsertUpdateData(sqlCmd);
        }

        public static string GetFee()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceAxiomAutoGetFee";
            var result = GetDataList(cmd);
            if (result != null && result.Rows.Count > 0)
                return Convert.ToString(result.Rows[0]["RegFee"]);
            else
                return null;
        }

        public static void InsertMiscChgOrderPart(int OrderNo, int PartNo)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = OrderNo;
            sqlCmd.Parameters.Add("@PatNo", SqlDbType.Int).Value = PartNo;
            sqlCmd.CommandText = "InsertMiscChargesForCertifiedMail";
            InsertUpdateData(sqlCmd);
        }

        public static void UpdateOrderPart(int OrderNo, int PartNo, string AsgnTo, DateTime CallBack)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            sqlCmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = PartNo;
            sqlCmd.Parameters.Add("@AsgnTo", SqlDbType.VarChar).Value = AsgnTo;
            sqlCmd.Parameters.Add("@CallBack", SqlDbType.DateTime).Value = CallBack;
            sqlCmd.CommandText = "ServiceAxiomAutoUpdatePart";
            InsertUpdateData(sqlCmd);
        }
        public static void InsertOrderNotes(int OrderNo, int PartNo, Guid UserId, string Note)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = OrderNo;
            sqlCmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = PartNo;
            sqlCmd.Parameters.Add("@NotesClient", SqlDbType.VarChar).Value = Note;
            sqlCmd.Parameters.Add("@NotesInternal", SqlDbType.VarChar).Value = Note;
            sqlCmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = UserId;
            sqlCmd.Parameters.Add("@InternalStatusId", SqlDbType.Int).Value = 1;
            sqlCmd.CommandText = "InsertOrderNotes";
            InsertUpdateData(sqlCmd);
        }
        public static List<QuickFormDocumentListEntity> GetDocsForRequest(int OrderNo, string PartNo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "QuickFormGetDocumentListByType";
            cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = "Requests";
            cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            cmd.Parameters.Add("@PartNo", SqlDbType.VarChar).Value = PartNo;
            var result = GetDataList(cmd);
            return result != null ? ConvertDataTable<QuickFormDocumentListEntity>(result) : null;
        }
        public static List<CompanyDetailForEmailEntity> CompanyDetailForEmail(string spName, int OrderNO)
        {
            List<string> recordList = new List<string>();
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNO;
                cmd.Connection = objConn;
                cmd.CommandText = spName;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dt.TableName = "Result";
                return ConvertDataTable<CompanyDetailForEmailEntity>(dt);
            }
        }
    }
}
