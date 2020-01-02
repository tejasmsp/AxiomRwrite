app.service('TransferRecordService', function ($http, configurationService) {
    var TransferRecordService = [];

    TransferRecordService.GetEntityListForTrasferDropdown = function (EnitityTypeId, CompanyNo) {
        return $http.get(configurationService.basePath + "GetEntityListForTrasferDropdown?EnitityTypeId=" + EnitityTypeId + "&CompanyNo=" + CompanyNo, null);
    };
    TransferRecordService.SubmitRecordsToTransfer = function (objTransferEntity) {
        return $http.post(configurationService.basePath + "SubmitRecordsToTransfer", objTransferEntity);
        // EnitityTypeId, SourceEntityId, TargetEntityId, UserId
    };
    
    return TransferRecordService;
});