app.service('Step1Service', function ($http, configurationService) {
    var step1Service = [];

    step1Service.GetAttorneyForCode = function () {
        return $http.get(configurationService.basePath + "GetAttorneyForCode");
    };


    step1Service.GetOrderingAttorneyList = function (userGuid,FirmID) {
        return $http.get(configurationService.basePath + "GetAttorneyList?userId=" + userGuid + "&FirmID=" + FirmID);
    };

    step1Service.GetNotificationList = function (attyId, orderId) {
        return $http.get(configurationService.basePath + "GetNotificationList?attyId=" + attyId + "&orderId=" + orderId);
    };


    step1Service.GetOrderWizardStep1Details = function (orderId) {
        return $http.get(configurationService.basePath + "GetOrderWizardStep1Details?orderId=" + orderId);
    };

    step1Service.InsertOrderWizardStep1 = function (orderObj) {
        return $http.post(configurationService.basePath + "InsertOrderWizardStep1", orderObj);
    };

    step1Service.UpdateOrderWizardStep1 = function (orderObj) {
        return $http.post(configurationService.basePath + "UpdateOrderWizardStep1", orderObj);
    };


    step1Service.SubmitOrder = function (orderObj) {
        return $http.post(configurationService.basePath + "SubmitOrder", orderObj);
    };

    return step1Service;
});