app.service('OrderDetailMasterService', function ($http, configurationService) {
    var orderDetailMasterService = [];

    orderDetailMasterService.GetOrderCompanyDetail = function (OrderId) {
        return $http.get(configurationService.basePath + "GetOrderCompanyDetail?OrderId=" + OrderId);
    };
    orderDetailMasterService.GetClientInformation = function (OrderId) {
        return $http.get(configurationService.basePath + "GetClientInformation?OrderId=" + OrderId);
    };
    orderDetailMasterService.GetPartInternalStatus = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "GetPartInternalStatus?OrderId=" + OrderId + "&PartNo=" + PartNo);
    };

    orderDetailMasterService.GetOrderDetailByOrderId = function (OrderId) {
        return $http.get(configurationService.basePath + "GetOrderDetailByOrderId?OrderId=" + OrderId);
    };
    orderDetailMasterService.GetOrderInformation = function (OrderId) {
        return $http.get(configurationService.basePath + "GetOrderInformation?OrderId=" + OrderId);
    };
    orderDetailMasterService.UpdateBasicOrderIformation = function (objOrder) {
        return $http.post(configurationService.basePath + "UpdateBasicOrderIformation", objOrder);
    };
    orderDetailMasterService.UpdateBillToFirm = function (OrderId,FirmID) {
        return $http.get(configurationService.basePath + "UpdateBillToFirm?OrderID=" + OrderId + "&FirmID=" + FirmID);
    };
    orderDetailMasterService.UpdateBillToAttorney = function (OrderId, AttyID) {
        return $http.get(configurationService.basePath + "UpdateBillToAttorney?OrderID=" + OrderId + "&AttyID=" + AttyID);
    };

    return orderDetailMasterService;
});