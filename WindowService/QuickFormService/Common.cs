using QuickFormService.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Words;
using System.Configuration;
using Aspose.BarCode;
using Aspose.Words.Drawing;

namespace QuickFormService
{
    public static class Common
    {

        public static string GetQueryToExecute(QueryType pdt, DataRow drForm, string[] folders, int orderNo, string partIds, bool displaySSN, string partCnt)
        {
            string query = "";
            if (pdt == QueryType.Common)
            {
                bool isMichigan = false;
                bool isRush = false;
                if (folders.Contains("Michigan"))
                    isMichigan = true;
                if (Convert.ToString(drForm["DocName"]).Contains("RUSH"))
                    isRush = true;
                //if (folders.Contains("Custodian Letters") || folders.Contains("Subpoenas"))
                //    filetype = "pdf";
                query = DbAccess.GetQueryByQueryTypeId(1, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (isMichigan && isRush)
                        query = query.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,7,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,7,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,7,GETDATE()), 107), 9) AS BigDate ");
                    else if (isMichigan && !isRush)
                        query = query.Replace("--Michigan and Rush", " ,DATENAME(DW, DATEADD(day,14,GETDATE())) as [DayName],DATENAME(MM, DATEADD(day,14,GETDATE())) + RIGHT(CONVERT(VARCHAR(12), DATEADD(day,14,GETDATE()), 107), 9) AS BigDate ");
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else if (pdt == QueryType.Confirmation)
            {
                query = DbAccess.GetQueryByQueryTypeId(2, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds).Replace("%%PartCnt%%", Convert.ToString(partCnt));
                if (!displaySSN)
                {
                    // query = query.Replace("Orders.SSN", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(Orders.SSN)),8,4) ");
                    query = query.Replace("OP.ssn", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(OP.ssn)),8,4) ");
                }
            }
            else if (pdt == QueryType.FaceSheet)
            {
                string attyId = Convert.ToString(drForm["DocName"]).Split('_')[2];
                query = DbAccess.GetQueryByQueryTypeId(14, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (!string.IsNullOrEmpty(attyId))
                    {
                        query = query.Replace("%%ATTYNO%%", @"'" + attyId + "'");
                    }
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else if (pdt == QueryType.StatusLetters)
            {
                query = DbAccess.GetQueryByQueryTypeId(4, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else if (pdt == QueryType.Waiver)
            {
                query = DbAccess.GetQueryByQueryTypeId(5, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                if (!displaySSN)
                    query = ReplaceSSN(query);
            }
            else if (pdt == QueryType.Interrogatories)
            {
                query = DbAccess.GetQueryByQueryTypeId(7, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                if (!displaySSN)
                    query = ReplaceSSN(query);
            }
            else if (pdt == QueryType.TargetSheets)
            {
                query = DbAccess.GetQueryByQueryTypeId(8, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
            }
            else if (pdt == QueryType.StatusProgressReports)
            {
                query = DbAccess.GetQueryByQueryTypeId(9, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds).Replace("%%PartCnt%%", Convert.ToString(partCnt));
            }
            else if (pdt == QueryType.CerticicationNOD)
            {
                query = DbAccess.GetQueryByQueryTypeId(10, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
            }
            else if (pdt == QueryType.AttorneyOfRecords)
            {
                query = DbAccess.GetQueryByQueryTypeId(6, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
            }
            else if (pdt == QueryType.CollectionLetters)
            {
                query = DbAccess.GetQueryByQueryTypeId(11, "Query");
                if (!string.IsNullOrEmpty(query))
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
            }
            else if (pdt == QueryType.Notices)
            {
                query = DbAccess.GetQueryByQueryTypeId(12, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else if (pdt == QueryType.AttorneyForms)
            {
                string attyId = Convert.ToString(drForm["DocName"]).Split('_')[2];
                query = DbAccess.GetQueryByQueryTypeId(13, "Query");
                if (!string.IsNullOrEmpty(query))
                {
                    query = ReplaceOrderPartNo(query, orderNo, partIds);
                    if (!string.IsNullOrEmpty(attyId))
                    {
                        query = query.Replace("%%ATTYNO%%", @"'" + attyId + "'");
                    }
                    if (!displaySSN)
                        query = ReplaceSSN(query);
                }
            }
            else
                Log.ServicLog(Convert.ToString(drForm["DocName"]) + " is failed to process.");
            return query;
        }
        public static void AddFilesToPart(FileObject fileObj, string fileDiskName)
        {
            try
            {
                var obj = new FilesToPartEntity();
                obj.OrderNo = fileObj.OrderNo;
                obj.PartNo = fileObj.PartNo;
                obj.FileName = fileObj.FileName;
                obj.FileTypeId = fileObj.FileType;
                obj.RecordTypeId = fileObj.RecordType;
                obj.UserId = fileObj.UserId;
                obj.IsPublic = fileObj.IsPublic;
                obj.Pages = fileObj.Pages;
                obj.FileDiskName = fileDiskName;
                DbAccess.AddFilesToPart(obj, "ServiceQuickFormAddFilesToPart");
            }
            catch (Exception exception)
            {
                Log.ServicLog(exception.Message);
            }
        }
        public static string ReplaceOrderPartNo(string query, int OrderNo, string PartNo)
        {
            return query.Replace("%%ORDERNO%%", Convert.ToString(OrderNo)).Replace("%%PARTNO%%", Convert.ToString(PartNo));
        }
        public static string ReplaceSSN(string query)
        {
            // return query.Replace("LTRIM(RTRIM(Orders.SSN))", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(Orders.SSN)),8,4) ");
            return query.Replace("OP.ssn", " 'XXX-XX-' + SUBSTRING(LTRIM(RTRIM(OP.ssn)),8,4) ");

        }
    }
}
