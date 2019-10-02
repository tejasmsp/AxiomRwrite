app.service('billingRateService', function ($http, configurationService) {
    var billingRateService = [];

    billingRateService.GetBillingRateList = function (MemberID) {
        return $http.get(configurationService.basePath + "GetBillingRateList?MemberID=" + MemberID);
    };
    billingRateService.GetBillingRateListByID = function (MemberID, RecordType) {
        return $http.get(configurationService.basePath + "GetBillingRateListByID?MemberID=" + MemberID + "&RecordType=" + RecordType);
    };
    billingRateService.UpdateBillingRate = function (BillingRateList) {
        return $http.post(configurationService.basePath + "UpdateBillingRate", BillingRateList);
    };
    billingRateService.InsertBillingRate = function (BillingRateList) {
        return $http.post(configurationService.basePath + "InsertBillingRate", BillingRateList);
    };
    billingRateService.InsertRecordType = function (RecordType) {
        return $http.post(configurationService.basePath + "InsertRecordType", RecordType);
    };
    billingRateService.GetRecordTypeList = function () {
        return $http.get(configurationService.basePath + "GetRecordTypeList");
    };
    
    return billingRateService;
});