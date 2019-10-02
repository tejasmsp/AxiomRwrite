<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceBatchReport.aspx.cs" Inherits="Axiom.Web.Reports.InvoiceBatchReport" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div>
             <rsweb:ReportViewer ID="ReportViewer1" runat="server"  Height="900px">
        </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
