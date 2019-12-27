app.service('TransferRecordService', function ($http, configurationService) {
    var TransferRecordService = [];

    //TransferRecordService.GetEnitityListForTrasferDropdown = function (MemberID) {
    //    return $http.get(configurationService.basePath + "GetEnitityListForTrasferDropdown?MemberID=" + MemberID);
    //};
    //TransferRecordService.GetBillingRateListByID = function (MemberID, RecordType) {
    //    return $http.get(configurationService.basePath + "GetBillingRateListByID?MemberID=" + MemberID + "&RecordType=" + RecordType);
    //};
    //TransferRecordService.UpdateBillingRate = function (BillingRateList) {
    //    return $http.post(configurationService.basePath + "UpdateBillingRate", BillingRateList);
    //};
    TransferRecordService.GetEnitityListForTrasferDropdown = function (EnitityTypeId, CompanyNo) {
        return $http.get(configurationService.basePath + "GetEnitityListForTrasferDropdown?EnitityTypeId=" + EnitityTypeId + "&CompanyNo=" + CompanyNo, null);
    };
    TransferRecordService.SubmitRecordsToTransfer = function (objTransferEntity) {
        return $http.post(configurationService.basePath + "SubmitRecordsToTransfer", objTransferEntity);
        // EnitityTypeId, SourceEntityId, TargetEntityId, UserId
    };
    
    return TransferRecordService;
});