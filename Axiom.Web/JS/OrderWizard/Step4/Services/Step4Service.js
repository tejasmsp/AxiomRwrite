app.service('Step4Service', function ($http, configurationService) {
    var Step4Service = [];


    Step4Service.GetOrderWizardStep4Details = function (orderId) {
        return $http.get(configurationService.basePath + "GetOrderWizardStep4Details?orderId=" + orderId);
    };

    Step4Service.InsertOrUpdateOrderWizardStep4 = function (orderObj) {
        return $http.post(configurationService.basePath + "InsertOrUpdateOrderWizardStep4", orderObj);
    };



    return Step4Service;
});