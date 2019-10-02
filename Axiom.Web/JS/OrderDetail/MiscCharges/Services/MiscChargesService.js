app.service('MiscChargesService', function ($http, configurationService) {
    var miscchargesService= [];

    miscchargesService.GetMiscChargesByOrderId = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "GetMiscChargesByOrderId?OrderId=" + OrderId + "&PartNo=" + PartNo);
    };
    miscchargesService.GetMiscChrgsDropDown = function () {
        return $http.get(configurationService.basePath + "GetMiscChrgsDropDown");
    };
    miscchargesService.GetPartListByOrderId = function (OrderId) { //From OrderApiController
        return $http.get(configurationService.basePath + "GetPartListByOrderId?OrderId=" + OrderId);
    };
    miscchargesService.GetMiscChargeAttorneyDropDown = function (OrderId) { 
        return $http.get(configurationService.basePath + "GetMiscChargeAttorneyDropDown?OrderId=" + OrderId);
    };
    miscchargesService.SaveMiscCharges = function (model) { 
        return $http.post(configurationService.basePath + "SaveMiscCharges", model);
    };
    miscchargesService.GetMiscChargesByMiscChrgId = function (MiscChrgId) {
        return $http.get(configurationService.basePath + "GetMiscChargesByMiscChrgId?MiscChrgId=" + MiscChrgId);
    };
    miscchargesService.DeleteMiscChargesByMiscChrgId = function (MiscChrgId) {
        return $http.post(configurationService.basePath + "DeleteMiscChargesByMiscChrgId?MiscChrgId=" + MiscChrgId);
    };
    return miscchargesService;
});