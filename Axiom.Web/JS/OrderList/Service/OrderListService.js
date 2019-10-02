app.service('OrderListService', function ($http, configurationService) {
    var OrderListService = [];

    OrderListService.GetOrderList = function (OrderID) {
        return $http.get(configurationService.basePath + "GetOrderList?OrderID=" + OrderID);
    };

    OrderListService.ArchiveOrder = function (orderObj) {
        return $http.post(configurationService.basePath + "ArchiveOrder", orderObj);
    };




    OrderListService.DeleteDraftOrder = function (OrderId) {
        return $http.post(configurationService.basePath + "DeleteDraftOrder?OrderId=" + OrderId);
    }
    //OrderListService.CancelOrder = function (orderObj) {
    //    return $http.post(configurationService.basePath + "CancelOrder", orderObj);
    //};
    return OrderListService;
});