app.service('Step3Service', function ($http, configurationService) {
    var step3Service = [];


    step3Service.GetOrderWizardStep3Details = function (orderId) {
        return $http.get(configurationService.basePath + "GetOrderWizardStep3Details?orderId=" + orderId);
    };

    step3Service.InsertOrUpdateOrderWizardStep3 = function (orderObj) {
        return $http.post(configurationService.basePath + "InsertOrUpdateOrderWizardStep3", orderObj);
    };



    return step3Service;
});