app.service('Step7Service', function ($http, configurationService) {
    var Step7Service = [];


    Step7Service.GetDocumentList = function (OrderID,PartNo) {
        return $http.get(configurationService.basePath + "GetDocumentList?OrderID=" + OrderID + "&PartNo=" + PartNo);
    };

    Step7Service.UploadOrderDocument = function (fd,OrderId,CreatedBy,UserAccessId) {

        return $.ajax({
            url: configurationService.basePath + "UploadOrderDocument?OrderId=" + OrderId + "&CreatedBy=" + CreatedBy + "&UserAccessId=" + UserAccessId,
            data: fd,
            cache: false,
            contentType: false,
            processData: false,
            method: 'POST',
            type: 'POST', // For jQuery < 1.9
            success: function (data) {
            }
        });
    };

    Step7Service.DeleteOrderDocument = function (OrderDocumentId) {
        return $http.post(configurationService.basePath + "DeleteOrderDocument?OrderDocumentId=" + OrderDocumentId);
    };

    return Step7Service;
});