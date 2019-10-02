app.service('OrderPartNoteService', function ($http, configurationService) {
    var OrderPartNoteService = [];

    //OrderPartNoteService.GetOrderNotes = function (OrderId,PartNo) {
    //    return $http.get(configurationService.basePath + "GetOrderNotes?OrderId=" + OrderId + "&PartNo=" + PartNo);
    //};

    OrderPartNoteService.InsertOrderPartNotes = function (modal) {
        return $http.post(configurationService.basePath + "InsertOrderPartNotes", modal);
    };

    OrderPartNoteService.InsertOrderPartNotesByClient = function (modal) {
        return $http.post(configurationService.basePath + "InsertOrderPartNotesByClient", modal);
    };

    OrderPartNoteService.RemoveCallBackFromNote = function (modal) {
        return $http.post(configurationService.basePath + "RemoveCallBackFromNote", modal);
    };

    return OrderPartNoteService;
});