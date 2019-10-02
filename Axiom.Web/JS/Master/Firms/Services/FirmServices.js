app.service('FirmServices', function ($http, configurationService) {
    var FirmService = [];

    FirmService.GetFirmList = function (FirmId) {
        return $http.get(configurationService.basePath + "GetFirmList?FirmId=" + FirmId);
    };

    FirmService.InsertFirm = function (Firmobj, EmpID, UserAccessID) {
        return $http.post(configurationService.basePath + "InsertFirm?EmpID=" + EmpID + "&UserID=" + UserAccessID, Firmobj);
    };

    FirmService.UpdateFirm = function (Firmobj, EmpID, UserAccessID) {
        return $http.post(configurationService.basePath + "UpdateFirm?EmpID=" + EmpID + "&UserID=" + UserAccessID, Firmobj);
    };

    FirmService.DeleteFirm = function (FirmId) {
        return $http.post(configurationService.basePath + "DeleteFirm?FirmId=" + FirmId);
    };

    FirmService.UpdateMergeFirm = function (locID, parentFirmId, needtoMerge) {
        return $http.post(configurationService.basePath + "UpdateMergeFirm?locID=" + locID + "&parentFirmId=" + parentFirmId + "&needtoMerge=" + needtoMerge);
    };

    FirmService.GetFirmById = function (FirmId) {
        return $http.get(configurationService.basePath + "GetFirmById?FirmId=" + FirmId);
    };

    //#region ASSOCIATED FIRM

    FirmService.GetAssociatedFirmList = function (FirmId) {
        return $http.get(configurationService.basePath + "GetAssociatedFirmList?FirmId=" + FirmId);
    };

    FirmService.InsertAssociatedFirm = function (ParentFirmID, AssocicatedFirmID) {
        return $http.post(configurationService.basePath + "InsertAssociatedFirm?ParentFirmID=" + ParentFirmID + "&AssocicatedFirmID=" + AssocicatedFirmID);
    };

    FirmService.DeleteAssociatedFirm = function (ParentFirmID, AssocicatedFirmID) {
        return $http.post(configurationService.basePath + "DeleteAssociatedFirm?ParentFirmID=" + ParentFirmID + "&AssocicatedFirmID=" + AssocicatedFirmID);
    };

    //#endregion

    //#region MEMBER OF ID
    FirmService.GetMemberOfIDList = function (FirmId) {
        return $http.get(configurationService.basePath + "GetMemberOfIDList?FirmId=" + FirmId);
    };

    FirmService.InsertMemberOfID = function (FirmId, CompanyID, MemberID) {
        return $http.post(configurationService.basePath + "InsertMemberOfID?FirmId=" + FirmId + "&CompanyID=" + CompanyID + "&MemberID=" + MemberID);
    };
    FirmService.DeleteMemberOfID = function (FirmBillingRateID) {
        return $http.post(configurationService.basePath + "DeleteMemberOfID?FirmBillingRateID=" + FirmBillingRateID);
    };

    FirmService.GetAdditionalContacts = function (FirmID, Type) {
        return $http.get(configurationService.basePath + "GetAdditionalContacts?FirmID=" + FirmID + "&Type=" + Type);
    };

    FirmService.SaveAdditionalContacts = function (objAdditionalContact) {
        return $http.post(configurationService.basePath + "SaveAdditionalContacts", objAdditionalContact);
    };

    FirmService.DeleteAdditionalContact = function (ContactID) {
        return $http.get(configurationService.basePath + "DeleteAdditionalContact?ContactID=" + ContactID);
    };

    FirmService.SaveFirmMonthlyBilling = function (FirmId, Name, Email) {
        return $http.get(configurationService.basePath + "SaveFirmMonthlyBilling?FirmId=" + FirmId + "&Name=" + Name + "&Email=" + Email);
    };


    FirmService.GetFirmFormsList = function (FirmID, isFaceSheet, isRequestform) {
        return $http.get(configurationService.basePath + "GetFirmFormsList?FirmID=" + FirmID + "&isFaceSheet=" + isFaceSheet + "&isRequestform=" + isRequestform);
    };

    FirmService.InsertFirmForm = function (model) {
        return $http.post(configurationService.basePath + "InsertFirmForm", model);
    };

    FirmService.DeleteFirmForm = function (FirmFormID) {
        return $http.post(configurationService.basePath + "DeleteFirmForm?FirmFormID=" + FirmFormID);
    };

    FirmService.UpdateAssociatedFirmDefaultBill = function (ParentFirmID, FirmID) {
        return $http.post(configurationService.basePath + "UpdateAssociatedFirmDefaultBill?FirmID=" + ParentFirmID + "&AssociatedFirmID=" + FirmID);
    };

    //#endregion


    return FirmService;
});