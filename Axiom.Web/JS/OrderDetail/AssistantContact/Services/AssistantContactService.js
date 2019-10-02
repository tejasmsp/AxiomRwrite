app.service('AssistantContactService', function ($http, configurationService) {
    var assistantContactService = [];

    assistantContactService.GetAssistantContactNotificationInformationByOrderId = function (OrderId) {
        return $http.get(configurationService.basePath + "GetAssistantContactNotificationInformationByOrderId?OrderId=" + OrderId);
    };

    assistantContactService.UpdateAssistantContact = function (orderId, model) {
        return $http.post(configurationService.basePath + "UpdateAssistantContact?orderId=" + orderId, model);
    };

    return assistantContactService;
});