app.service('Step5Service', function ($http, configurationService) {
    var Step5Service = [];


    Step5Service.GetOrderWizardStep5AttorneyRecords = function (orderId) {
        return $http.get(configurationService.basePath + "GetOrderWizardStep5AttorneyRecords?orderId=" + orderId);
    };

    Step5Service.DeleteOppositeAttorney = function (OrderFirmAttorneyId) {
        return $http.get(configurationService.basePath + "DeleteOppositeAttorney?OrderFirmAttorneyId=" + OrderFirmAttorneyId);
    };

    Step5Service.UpdateOrderFirmAttorney = function (AttorneyObj) {
        return $http.post(configurationService.basePath + "InsertOrUpdateOrderWizardStep5", AttorneyObj);
    };

    Step5Service.AddOrderFirmAttorney = function (AttorneyObj) {
        return $http.post(configurationService.basePath + "InsertOrUpdateOrderWizardStep5", AttorneyObj);
    };

    Step5Service.GetAttorneyListWithSearch = function (SearchCriteria, SearchCondition, SearchText, OrderId, CompnayNo) {
        return $http.get(configurationService.basePath + "GetAttorneyListWithSearch?SearchCriteria=" + SearchCriteria + '&SearchCondition=' + SearchCondition + '&SearchText=' + SearchText + "&OrderId=" + OrderId + "&CompNo=" + CompnayNo);
    };

    Step5Service.InsertNewFirmFromStep5 = function (modal) {
        return $http.post(configurationService.basePath + "InsertNewFirmFromStep5", modal);
    };
    Step5Service.InsertNewAttorneyFromStep5 = function (modal) {
        return $http.post(configurationService.basePath + "InsertNewAttorneyFromStep5", modal);
    };

    //Step5Service.InsertOrUpdateOrderWizardStep2 = function (orderObj) {
    //    return $http.post(configurationService.basePath + "InsertOrUpdateOrderWizardStep2", orderObj);
    //};



    return Step5Service;
});