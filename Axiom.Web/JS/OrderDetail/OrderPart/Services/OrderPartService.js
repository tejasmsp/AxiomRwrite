app.service('OrderPartService', function ($http, configurationService) {
    var orderPartService = [];

    orderPartService.GetPartListByOrderId = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "GetPartListByOrderId?OrderId=" + OrderId + "&PartNo=" + PartNo);
    };

    orderPartService.GetLocationByLocIDForPart = function (LocationId) {
        return $http.get(configurationService.basePath + "GetLocationByLocIDForPart?LocationId=" + LocationId);
    };
    orderPartService.InsertOrderPart = function (model) {
        return $http.post(configurationService.basePath + "InsertOrderPart", model);
    };

    orderPartService.UpdateOrderPart = function (model) {
        return $http.post(configurationService.basePath + "UpdateOrderPart", model);
    };
    orderPartService.AddUpdateChronolgy = function (OrderId, PartNo, userGuid, IsChronology) {
        return $http.post(configurationService.basePath + "AddUpdateChronolgy?OrderId=" + OrderId + "&PartNo=" + PartNo + "&userGuid=" + userGuid + "&IsChronology=" + IsChronology);
    };
    orderPartService.CancelPartSendEmail = function (OrderID, csvPartNo, userGuid, companyNo) {
        return $http.post(configurationService.basePath + "CancelPartSendEmail?OrderID=" + OrderID + "&csvPartNo=" + csvPartNo + "&userGuid=" + userGuid + "&companyNo=" + companyNo);
    };

    return orderPartService;
});