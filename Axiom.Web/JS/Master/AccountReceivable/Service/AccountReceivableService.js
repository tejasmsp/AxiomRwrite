app.service('AccountReceivableService', function ($http, configurationService) {
    var AccountReceivableService = [];

    AccountReceivableService.GetAccountReceivable = function (ArID) {
        return $http.get(configurationService.basePath + "GetAccountReceivable?ArID=" + ArID);
    };
    AccountReceivableService.GetAccountReceivableListBySearch = function (FirmID, FirmName, CheckType, CheckNo, InvoiceNo, UserAccessId) {   
        return $http.get(configurationService.basePath + "GetAccountReceivableListBySearch?FirmID=" + FirmID + "&FirmName=" + FirmName + "&CheckType=" + CheckType + "&CheckNo=" + CheckNo + "&InvoiceNo=" + InvoiceNo + "&UserAccessId=" + UserAccessId);
    };

    AccountReceivableService.InsertOrUpdateAccountReceivable = function (model) {
        return $http.post(configurationService.basePath + "InsertOrUpdateAccountReceivable", model);
    };

    AccountReceivableService.GetAccountReceivableInvoice = function (ArID,UserId) {
        return $http.get(configurationService.basePath + "GetAccountReceivableInvoice?ArID=" + ArID + "&UserId="+ UserId);
    };
    AccountReceivableService.GetAccountReceivableInvoicesById = function (InvoiceId) {
        return $http.get(configurationService.basePath + "GetAccountReceivableInvoicesById?InvoiceId=" + InvoiceId);
    };
    AccountReceivableService.InsertAccountReceivableInvoice = function (model) {
        return $http.post(configurationService.basePath + "InsertAccountReceivableInvoice", model);
    };

    AccountReceivableService.UpdateAccountReceivableInvoice = function (model) {
        return $http.post(configurationService.basePath + "UpdateAccountReceivableInvoice", model);
    };
    AccountReceivableService.GetInvoicesList = function (UserId) {
        return $http.get(configurationService.basePath + "GetInvoicesList?UserId=" + UserId);
    };
    AccountReceivableService.InsertAccountReceivableInvoiceForCreditCheck = function (model) {
        return $http.post(configurationService.basePath + "InsertAccountReceivableInvoiceForCreditCheck", model);
    };

    AccountReceivableService.UpdateAccountReceivableInvoiceForCreditCheck = function (model) {
        return $http.post(configurationService.basePath + "UpdateAccountReceivableInvoiceForCreditCheck", model);
    };
    AccountReceivableService.InsertCheckInvoicePaymentForDebitCheck = function (model) {
        return $http.post(configurationService.basePath + "InsertCheckInvoicePaymentForDebitCheck", model);
    };

    AccountReceivableService.UpdateCheckInvoicePaymentForDebitCheck = function (model) {
        return $http.post(configurationService.basePath + "UpdateCheckInvoicePaymentForDebitCheck", model);
    };
    AccountReceivableService.DeleteInvoicePayment = function (CheckInvoiceId, UserAccessId) {
        return $http.post(configurationService.basePath + "DeleteInvoicePayment?CheckInvoiceId=" + CheckInvoiceId + "&CreatedBy=" + UserAccessId);
    };
    //AccountReceivableService.GetVoidAndAllInVoices = function (PageIndex, InvoiceNo, OrderNo, UserId, BilledAttorney, SoldAttorney, VoidInvoices, AllInvoices, FirmID, FirmName) {
    //    return $http.get(configurationService.basePath + "GetVoidInvoicesBySearch?PageIndex=" + PageIndex + "&InvoiceNo=" + InvoiceNo + "&OrderNo=" + OrderNo + "&UserId=" + UserId+
    //        + "&BilledAttorney=" + BilledAttorney + "&SoldAttorney=" + SoldAttorney + "&VoidInvoices=" + VoidInvoices + "&AllInvoices=" + AllInvoices
    //        + "&FirmID=" + FirmID + "&FirmName=" + FirmName );
    //};

    //void invoices
    AccountReceivableService.GetVoidInvoicesBySearch = function (FirmID, FirmName, VoidInvoices, AllInvoices,InvoiceNo,UserId) {
        return $http.get(configurationService.basePath + "GetVoidInvoicesBySearch?FirmID=" + FirmID + "&FirmName=" + FirmName + "&VoidInvoices=" + VoidInvoices + "&AllInvoices=" + AllInvoices + "&InvoiceNo=" + InvoiceNo + "&UserId=" + UserId);
    };
    AccountReceivableService.SetInvoiceStatus = function (InvNo, Status) {
        return $http.post(configurationService.basePath + "SetInvoiceStatus?InvNo=" + InvNo + "&Status=" + Status);
    };
    AccountReceivableService.BounceAndCancelCheck = function (ArID, CreatedBy,CheckType) {
        return $http.post(configurationService.basePath + "BounceAndCancelCheck?ArID=" + ArID + "&CreatedBy=" + CreatedBy + "&CheckType=" + CheckType);
    };
    AccountReceivableService.GetChangeLogOfCheck = function (ArID) {
        return $http.get(configurationService.basePath + "GetChangeLogOfCheck?ArID=" + ArID);
    };
    AccountReceivableService.GetARInvoiceChangeLogByInvoiceId = function (ArID, InvNo) {
        return $http.get(configurationService.basePath + "GetARInvoiceChangeLogByInvoiceId?ArID=" + ArID + "&InvNo=" + InvNo);
    };
    AccountReceivableService.GetInvoiceDetailByInvoiceId = function (InvoiceNo) {
        return $http.get(configurationService.basePath + "GetInvoiceDetailByInvoiceId?InvoiceNo=" + InvoiceNo);
    };
    return AccountReceivableService;
});