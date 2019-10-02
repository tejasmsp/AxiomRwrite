app.service('Step2Service', function ($http, configurationService) {
    var step2Service = [];

    //step2Service.GetAttorneyForCode = function () {
    //    return $http.get(configurationService.basePath + "GetAttorneyForCode");
    //};


    //step2Service.GetOrderingAttorneyList = function (userGuid) {
    //    return $http.get(configurationService.basePath + "GetAttorneyList?userId=" + userGuid);
    //};

    //step2Service.GetNotificationList = function (attyId, orderId) {
    //    return $http.get(configurationService.basePath + "GetNotificationList?attyId=" + attyId + "&orderId=" + orderId);
    //};


    step2Service.GetOrderWizardStep2Details = function (orderId) {
        return $http.get(configurationService.basePath + "GetOrderWizardStep2Details?orderId=" + orderId);
    };

    step2Service.InsertOrUpdateOrderWizardStep2 = function (orderObj) {
        return $http.post(configurationService.basePath + "InsertOrUpdateOrderWizardStep2", orderObj);
    };

    step2Service.SelectFirmForBillingStep2 = function () {
        return $http.get(configurationService.basePath + "SelectFirmForBillingStep2");
    };

    step2Service.GetStateFirmStep2 = function () {
        return $http.get(configurationService.basePath + "GetStateFirmStep2");
    };
    

    return step2Service;
});