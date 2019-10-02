app.service('MessageServices', function ($http, configurationService) {
    var MessageService = [];

    MessageService.GetCustomMessageList = function () {
        return $http.get(configurationService.basePath + "GetCustomMessageList");
    };

    MessageService.InsertCustomMessage = function (model) {
        return $http.post(configurationService.basePath + "InsertCustomMessage", model);
    };

    MessageService.UpdateCustomMessage = function (model) {
        return $http.post(configurationService.basePath + "UpdateCustomMessage", model);
    };

    MessageService.GetCustomMessageById = function (id) {
        return $http.get(configurationService.basePath + "GetCustomMessageById?id=" + id);
    };

    MessageService.DeleteMessage = function (id) {
        return $http.post(configurationService.basePath + "DeleteCustomMessage?id=" + id);
    };

    MessageService.GetCustomMessageForClient = function () {
        return $http.get(configurationService.basePath + "GetCustomMessageForClient");
    };

    MessageService.GetUserNotification = function (UserAccessId) {
        return $http.get(configurationService.basePath + "GetUserNotification?UserAccessId=" + UserAccessId);
    };

    MessageService.UpdateUserNotification = function (UserAccessId, ID) {
        return $http.get(configurationService.basePath + "UpdateUserNotification?UserAccessId=" + UserAccessId + "&ID=" + ID);
    };

    MessageService.InsertNotificationReadByUser = function (UserID) {
        return $http.get(configurationService.basePath + "InsertNotificationReadByUser?UserID=" + UserID);
    };
    MessageService.GetNotificationReadByUser = function (UserID) {
        return $http.get(configurationService.basePath + "GetNotificationReadByUser?UserID=" + UserID);
    };
    
    return MessageService;
});