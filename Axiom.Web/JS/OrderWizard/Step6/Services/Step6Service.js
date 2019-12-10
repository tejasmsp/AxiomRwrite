app.service('Step6Service', function ($http, configurationService) {
    var Step6Service = [];


    Step6Service.GetOrderWizardStep5AttorneyRecords = function (orderId) {
        return $http.get(configurationService.basePath + "GetOrderWizardStep5AttorneyRecords?orderId=" + orderId);
    };

    Step6Service.GetLocationSearch = function () {
        return $http.get(configurationService.basePath + "GetLocationSearch");
    };

    //Step6Service.GetLocationByLocID = function (LocationId) {
    //    return $http.get(configurationService.basePath + "GetLocationByLocID?LocationId=" + LocationId);
    //};

    Step6Service.GetOrderLocationById = function (PartNo, OrderNo) {
        return $http.get(configurationService.basePath + "GetOrderLocationById?PartNo=" + PartNo + "&OrderNo=" + OrderNo);
    };

    Step6Service.InsertOrUpdateOrderWizardStep6 = function (model) {
        return $http.post(configurationService.basePath + "InsertOrUpdateOrderWizardStep6", model);
    };

    Step6Service.InsertNewLocation = function (model) {
        return $http.post(configurationService.basePath + "InsertNewLocation", model);
    };
    Step6Service.GetOrderWizardStep6Location = function (orderId, hideOldPart) {
        return $http.get(configurationService.basePath + "GetOrderWizardStep6Location?orderId=" + orderId + "&hideOldPart=" + hideOldPart);
    };

    Step6Service.GetLocationListWithSearch = function (SearchCriteria, SearchCondition, SearchText, OrderId) {
        return $http.get(configurationService.basePath + "GetLocationListWithSearch?SearchCriteria=" + SearchCriteria + '&SearchCondition=' + SearchCondition + '&SearchText=' + SearchText + "&OrderId=" + OrderId);
    };
    Step6Service.GetCanvasList = function (OrderId) {
        return $http.get(configurationService.basePath + "GetCanvasList?OrderId=" + OrderId);
    };
    Step6Service.GetLocationTempFiles = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "GetLocationTempFiles?OrderNo=" + OrderId + "&PartNo=" + PartNo);
    };
    Step6Service.SaveOrderCanvasRequest = function (model) {
        return $http.post(configurationService.basePath + "SaveOrderCanvasRequest", model);

        //return $.ajax({
        //    url: configurationService.basePath + "SaveOrderCanvasRequest",
        //    data: formData,
        //    cache: false,
        //    contentType: false,
        //    processData: false,
        //    method: 'POST',
        //    type: 'POST', // For jQuery < 1.9
        //    success: function (data) {
        //    }
        //});
    };

    Step6Service.DeleteCanvas = function (ID) {
        return $http.post(configurationService.basePath + "DeleteCanvas?ID=" + ID);
    };

    Step6Service.DeleteOrderLocation = function (PartNo, OrderNo,UserAccessID) {
        return $http.post(configurationService.basePath + "DeleteOrderLocation?PartNo=" + PartNo + "&OrderNo=" + OrderNo + "&UserAccessID=" + UserAccessID);
    };


    Step6Service.UploadNewOrderDocument = function (fd, CreatedBy, batchId) {

        return $.ajax({
            url: configurationService.basePath + "UploadDocumentAttachment?CreatedBy=" + CreatedBy,
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

    Step6Service.DeleteNewOrderDocument = function (batchId, fileName, createdBy) {
        return $http.post(configurationService.basePath + "DeleteUploadedDocument?batchId=" + batchId + "&fileName=" + fileName + "&createdBy=" + createdBy);
    };
    Step6Service.GetFileList = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "GetFile?OrderId=" + OrderId + "&PartNo=" + PartNo);
    };
    Step6Service.DeleteDBUploadedFile = function (fileId) {
        return $http.post(configurationService.basePath + "DeleteDBUploadedFile?FileId=" + fileId);
    };
    Step6Service.GetOrderWizardStep3Details = function (orderId) {
        return $http.get(configurationService.basePath + "GetOrderWizardStep3Details?orderId=" + orderId);
    };
    Step6Service.UpdateOrderWizardStep6LocRush = function (OrderNo, PartNo, Rush) {
        return $http.post(configurationService.basePath + "UpdateOrderWizardStep6LocRush?OrderNo=" + OrderNo + "&PartNo=" + PartNo + "&Rush=" + Rush);
    };
    Step6Service.RushAllLocation = function (orderId, Value) {
        return $http.post(configurationService.basePath + "RushAllLocation?OrderID=" + orderId + "&Status=" + Value);
    };
    Step6Service.GetLocationSearchFromPart = function (SearchCriteria, SearchCondition, SearchText) {
        return $http.get(configurationService.basePath + "GetLocationSearchFromPart?SearchCriteria=" + SearchCriteria + '&SearchCondition=' + SearchCondition + '&SearchText=' + SearchText);
    };

    return Step6Service;
});