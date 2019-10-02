app.service('MiscellaneousChargesServiceService', function ($http, configurationService) {
    var MiscellaneousChargesService = [];

    MiscellaneousChargesService.GetMiscellaneousCharges = function (MiscId) {
        return $http.get(configurationService.basePath + "GetMiscellaneousCharges?MiscId=" + MiscId);
    };
    MiscellaneousChargesService.GetMiscDescById = function (MemberID) {
        return $http.get(configurationService.basePath + "GetMiscDescById?MemberID=" + MemberID);
    };
    MiscellaneousChargesService.UpdateMiscCharges = function (model) {
        return $http.post(configurationService.basePath + "UpdateMiscCharges", model);
    };
    MiscellaneousChargesService.InsertMiscCharges = function (model) {
        return $http.post(configurationService.basePath + "InsertMiscCharges", model);
    };
    //MiscellaneousChargesService.InsertRecordType = function (RecordType) {
    //    return $http.post(configurationService.basePath + "InsertRecordType", RecordType);
    //};
    //MiscellaneousChargesService.GetRecordTypeList = function () {
    //    return $http.get(configurationService.basePath + "GetRecordTypeList");
    //};
    
    return MiscellaneousChargesService;
});