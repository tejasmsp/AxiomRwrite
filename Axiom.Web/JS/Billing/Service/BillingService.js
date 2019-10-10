app.service('BillingService', function ($http, configurationService) {
    var billingService = [];

    billingService.GetBillToAttorneyByFirmId = function (FirmID) {
        return $http.get(configurationService.basePath + "GetBillToAttorneyByFirmId?FirmID=" + FirmID);
    };

    billingService.GetSoldToAttorneyByOrderNo = function (OrderNo, PartNo) {
        return $http.get(configurationService.basePath + "GetSoldToAttorneyByOrderNo?OrderNo=" + OrderNo + "&PartNo=" + PartNo);
    };


    billingService.GetBillToAttorneyDetailsByOrderId = function (OrderNo, PartNo) {
        return $http.get(configurationService.basePath + "GetBillToAttorneyDetailsByOrderId?OrderNo=" + OrderNo + "&PartNo=" + PartNo);
    };

    billingService.GetSoldToAttorneyDetailsByOrderId = function (OrderNo, PartNo) {
        return $http.get(configurationService.basePath + "GetSoldToAttorneyDetailsByOrderId?OrderNo=" + OrderNo + "&PartNo=" + PartNo);
    };

    billingService.GetAllInvoiceByOrderIdAndPartId = function (OrderNo, PartNo) {
        return $http.get(configurationService.basePath + "GetAllInvoiceByOrderIdAndPartId?OrderNo=" + OrderNo + "&PartNo=" + PartNo);
    };

    billingService.EditInvoice = function (InvoiceNumber) {
        return $http.get(configurationService.basePath + "EditInvoice?InvoiceNumber=" + InvoiceNumber);
    };

    billingService.GerRecordTypeListForBilling = function () {
        return $http.get(configurationService.basePath + "GerRecordTypeListForBilling");
    };

    billingService.UpdateInvoice = function (invoiceDetail) {
        return $http.post(configurationService.basePath + "UpdateInvoice", invoiceDetail);
    };

    billingService.DeleteInvoice = function (invoiceNumber, itemNumber) {
        return $http.get(configurationService.basePath + "DeleteInvoice?invoiceNumber=" + invoiceNumber + "&itemNumber=" + itemNumber);
    };

    billingService.GenerateInvoice = function (OrderNo, PartNo, BillToAttorney, SoldAtty,CompanyNo) {
        return $http.post(configurationService.basePath + "GenerateInvoice?OrderNo=" + OrderNo + "&PartNo=" + PartNo + "&BillToAttorney=" + BillToAttorney + "&CompanyNo=" + CompanyNo, SoldAtty);
    };

    billingService.SendEmailForBill = function (AttyID, InvoiceNumber, OrderNumber, PartNumber, Location, Locationname, Patient, UserGuid, CompanyNo) {
        return $http.get(configurationService.basePath + "SendEmailForBill?AttyID=" + AttyID + "&InvoiceNumber=" + InvoiceNumber + "&OrderNumber=" + OrderNumber + "&PartNumber=" + PartNumber + "&Location=" + Location + "&LocationName=" + Locationname + "&Patient=" + Patient + "&UserGuid=" + UserGuid + "&CompanyNo=" + CompanyNo);
    };

    billingService.GetInvMsg = function () {
        return $http.get(configurationService.basePath + "GetInvMsg");
    };

    return billingService;
});