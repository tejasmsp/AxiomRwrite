﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using UploadSweepService.Entity;

namespace UploadSweepService
{


    public static class DbAccess
    {
        public static string sConnectionString = ConfigurationManager.ConnectionStrings["Axiom"].ConnectionString;

        public static DataTable GetDataList(string spName, int OrderNo)
        {
            List<string> recordList = new List<string>();
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
                cmd.Connection = objConn;
                cmd.CommandText = spName;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dt.TableName = "Result";
                return dt;
            }
        }
        
        public static void ServiceSweepUpdateRcvdProcess(string spName, int RcvdID)
        {
            List<string> recordList = new List<string>();
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@RcvdID", SqlDbType.Int).Value = RcvdID;                
                cmd.Connection = objConn;
                cmd.CommandText = spName;                
                cmd.ExecuteNonQuery();
            }
        }
        public static DataTable GetLastUploadedRecord(string spName, int OrderNo, int PartNo)
        {
            List<string> recordList = new List<string>();
            using (SqlConnection objConn = new SqlConnection(sConnectionString))
            {
                objConn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = OrderNo;
                cmd.Parameters.Add("@PartNo", SqlDbType.Int).Value = PartNo;
                cmd.Connection = objConn;
                cmd.CommandText = spName;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dt.TableName = "Result";
                return dt;
            }
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
