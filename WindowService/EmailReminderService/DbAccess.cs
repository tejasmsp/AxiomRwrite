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
using EmailReminderService.Entity;

namespace EmailReminderService
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

        public static void InsertUpdateData(SqlCommand cmd)
        {
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                cmd.Connection = objConn;
                cmd.ExecuteNonQuery();
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


        public static OrderPatient GetOrderPatient(int OrderNo)
        {
            SqlCommand cmdOrderPatient = new SqlCommand();
            cmdOrderPatient.CommandType = CommandType.StoredProcedure;
            cmdOrderPatient.CommandText = "ServiceEmailReminderGetOrderPatient";
            cmdOrderPatient.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            var dtorderPatinet = GetDataList(cmdOrderPatient);
            var orderPatinetList = ConvertDataTable<OrderPatient>(dtorderPatinet);
            return orderPatinetList != null ? orderPatinetList[0] : null;
        }
        public static BillAttorney GetBillAttorney(int OrderNo)
        {
            SqlCommand cmdbillAtty = new SqlCommand();
            cmdbillAtty.CommandType = CommandType.StoredProcedure;
            cmdbillAtty.CommandText = "ServiceEmailReminderGetBillingAttoryney";
            cmdbillAtty.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            var dtbillAtty = GetDataList(cmdbillAtty);
            var billAttyList = ConvertDataTable<BillAttorney>(dtbillAtty);
            return billAttyList != null ? billAttyList[0] : null;
        }
        public static Attorney GetBillAttorney(string strBillAtty)
        {
            SqlCommand cmdAtty = new SqlCommand();
            cmdAtty.CommandType = CommandType.StoredProcedure;
            cmdAtty.CommandText = "ServiceEmailReminderGetAttorneys";
            cmdAtty.Parameters.Add("@BillAtty", SqlDbType.VarChar).Value = strBillAtty;
            var dtAtty = GetDataList(cmdAtty);
            var attyList = ConvertDataTable<Attorney>(dtAtty);
            return attyList != null ? attyList[0] : null;
        }
        public static string GetFeeApprovalAttorneyEmail(int OrderNo)
        {
            SqlCommand cmdAtty = new SqlCommand();
            cmdAtty.CommandType = CommandType.StoredProcedure;
            cmdAtty.CommandText = "GetAttorneyEmailList";
            cmdAtty.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            cmdAtty.Parameters.Add("@IsOrderConfirm", SqlDbType.Int).Value = 0;
            cmdAtty.Parameters.Add("@IsFeeApproval", SqlDbType.Int).Value = 1;
            var dtAtty = GetDataList(cmdAtty);
            string FeeApprovalEmails = "";
            if (dtAtty != null)
            {
                for (int i = 0; i < dtAtty.Rows.Count; i++)
                {
                    FeeApprovalEmails = FeeApprovalEmails + Convert.ToString(dtAtty.Rows[i][0]);
                    FeeApprovalEmails += (i < dtAtty.Rows.Count) ? "," : string.Empty;
                }
            }           
            return FeeApprovalEmails;
        }
        public static Location GetPartLocation(int OrderNo, int PartNo)
        {
            SqlCommand cmdlocation = new SqlCommand();
            cmdlocation.CommandType = CommandType.StoredProcedure;
            cmdlocation.CommandText = "ServiceEmailReminderGetPartLocation";
            cmdlocation.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            cmdlocation.Parameters.Add("@PartNo", SqlDbType.Int).Value = PartNo;
            var dtlocation = GetDataList(cmdlocation);
            var locationList = ConvertDataTable<Location>(dtlocation);
            return locationList != null ? locationList[0] : null;
        }
        public static void UpdateProposalData(int OrderNo, int PartNo, int ProposalFeeID)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            sqlCmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = PartNo;
            sqlCmd.Parameters.Add("@ProposalFeeID", SqlDbType.Int).Value = ProposalFeeID;
            sqlCmd.CommandText = "ServiceEmailReminderUpdateProposalFee";
            InsertUpdateData(sqlCmd);
        }

        public static List<AdditionalContact> GetNotificationEmails(int OrderNo, string OrderAttorney)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            sqlCmd.Parameters.Add("@OrderAttorney", SqlDbType.VarChar).Value = OrderAttorney;
            sqlCmd.Parameters.Add("@isFromQuickFormService", SqlDbType.Bit).Value = 0;
            sqlCmd.CommandText = "ServiceEmailReminderGetNotificationEmails";
            var dt = GetDataList(sqlCmd);
            return ConvertDataTable<AdditionalContact>(dt);
        }
        public static List<AuthorizationToBeCalledOnEmail> GetAuthorizationToBeCalledOnEmail(int followUpDay, int isMonthly)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@FollowUPDays", SqlDbType.Int).Value = followUpDay;
            sqlCmd.Parameters.Add("@isMonthly", SqlDbType.Int).Value = isMonthly;
            sqlCmd.CommandText = "ServiceEmailReminderGetAuthorizationToBeCalledOnEmail";
            var dt = GetDataList(sqlCmd);
            return ConvertDataTable<AuthorizationToBeCalledOnEmail>(dt);
        }
        public static void UpdateAuthorizationToBeCalledOnEmail(int OrderNo, int PartNo, int Ismonthly)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
            sqlCmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = PartNo;
            sqlCmd.Parameters.Add("@isMonthly", SqlDbType.Int).Value = Ismonthly;

            sqlCmd.CommandText = "ServiceEmailReminderUpdateAuthorizationToBeCalledOnEmail";
            InsertUpdateData(sqlCmd);
        }
        public static ClientAcct GetClientAcctExecService(string AttyId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceEmailReminderGetClientAcctExecService";
            cmd.Parameters.Add("@AttyId", SqlDbType.VarChar).Value = AttyId;
            var dt = GetDataList(cmd);
            var list = ConvertDataTable<ClientAcct>(dt);
            return list != null ? list[0] : null;
        }
        public static AttornyAuthorization GetAttornyEmailidForAuthorization(int OrderNo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceEmailReminderGetAttornyEmailidForAuthorization";
            cmd.Parameters.Add("@OrderNo", SqlDbType.VarChar).Value = OrderNo;
            var dt = GetDataList(cmd);
            var list = ConvertDataTable<AttornyAuthorization>(dt);
            return list != null ? list[0] : null;
        }
        public static void InsertOrderNotes(int OrderNo, int PartNo, string NotesClient, string NotesInternal, string UserId)
        {
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = OrderNo;
            sqlCmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = PartNo;
            sqlCmd.Parameters.Add("@NotesClient", SqlDbType.VarChar).Value = NotesClient;
            sqlCmd.Parameters.Add("@NotesInternal", SqlDbType.VarChar).Value = NotesInternal;
            sqlCmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserId;
            sqlCmd.CommandText = "InsertOrderNotes";
            InsertUpdateData(sqlCmd);
        }
        public static List<ProposalFees> GetProposalFees()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ServiceEmailReminderGetProposalFees";
            var dt = GetDataList(cmd);
            return ConvertDataTable<ProposalFees>(dt);


        }
    }

}
