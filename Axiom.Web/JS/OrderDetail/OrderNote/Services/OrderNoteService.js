app.service('OrderNoteService', function ($http, configurationService) {
    var OrderNoteService = [];

    OrderNoteService.GetOrderNotes = function (OrderId,PartNo) {
        return $http.get(configurationService.basePath + "GetOrderNotes?OrderId=" + OrderId + "&PartNo=" + PartNo);
    };

    OrderNoteService.InsertOrderNotes = function (modal) {
        return $http.post(configurationService.basePath + "InsertOrderNotes", modal);
    };

    return OrderNoteService;
});