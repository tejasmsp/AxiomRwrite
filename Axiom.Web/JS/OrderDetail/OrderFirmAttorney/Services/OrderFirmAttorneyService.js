app.service('OrderFirmAttorneyService', function ($http, configurationService) {
    var orderfirmattorneyservice = [];

    orderfirmattorneyservice.GetOrderFirmAttorneyByOrderId = function (OrderId) {
        return $http.get(configurationService.basePath + "GetOrderFirmAttorneyByOrderId?OrderId=" + OrderId);
    };
    orderfirmattorneyservice.GetWaiverDetailByOrderFirmAttorneyId = function (OrderFirmAttorneyId,OrderId) {
        return $http.get(configurationService.basePath + "GetWaiverDetailByOrderFirmAttorneyId?OrderFirmAttorneyId=" + OrderFirmAttorneyId+"&OrderId=" + OrderId);
    };

    orderfirmattorneyservice.SaveWaiver = function (OrderId,model) {
        return $http.post(configurationService.basePath + "SaveWaiver?OrderId=" + OrderId, model);
    };

    orderfirmattorneyservice.GetAttorneyByAttyId = function (attyId) {
        return $http.get(configurationService.basePath + "GetAttorneyByAttyId?attyId=" + attyId);
    };
    orderfirmattorneyservice.InsertUpdateOrderFirmAttorney = function (model) {
        return $http.post(configurationService.basePath + "InsertUpdateOrderFirmAttorney", model);
    };
    orderfirmattorneyservice.GetOrderFirmAttorneyByOrderFirmAttorneyId = function (OrderFirmAttorneyId) {
        return $http.get(configurationService.basePath + "GetOrderFirmAttorneyByOrderFirmAttorneyId?OrderFirmAttorneyId="+OrderFirmAttorneyId);
    };
    orderfirmattorneyservice.ChangeOrderAttorneyStatus = function (orderId, attyId, isDisabled, userAccessId) {
        return $http.get(configurationService.basePath + "ChangeOrderAttorneyStatus?OrderId=" + orderId + "&AttyId=" + attyId + "&IsDisabled=" + isDisabled+ "&UserAccessId=" + userAccessId);
    };
    return orderfirmattorneyservice;
});