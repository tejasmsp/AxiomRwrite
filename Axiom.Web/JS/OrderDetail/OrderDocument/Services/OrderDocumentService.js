app.service('OrderDocumentService', function ($http, configurationService) {
    var OrderDocumentService = [];

    OrderDocumentService.GetFileList = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "GetFile?OrderId=" + OrderId + "&PartNo=" + PartNo);
    };

    OrderDocumentService.UploadDocument = function (formData) {
        return $.ajax({
            url: configurationService.basePath + "UploadDocument",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            method: 'POST',
            type: 'POST', // For jQuery < 1.9
            success: function (data) {
            }
        });
    };
    OrderDocumentService.DownloadFile = function (fileDiskName, fileName, orderNo, partNo) {
        return $http({
            method: 'GET',
            url: configurationService.basePath + "DownloadFile?fileDiskName=" + fileDiskName + "&fileName=" + fileName + "&orderNo=" + orderNo + "&partNo=" + partNo,
            responseType: 'arraybuffer'
        });
    };
    OrderDocumentService.UpdateFileStatus = function (fileId, isPublic) {
        return $http.post(configurationService.basePath + "UpdateFileStatus?FileId=" + fileId + "&isPublic=" + isPublic);
    };
    OrderDocumentService.CheckFileDownloadAccess = function (OrderId, PartNo, userGuid) {
        return $http.get(configurationService.basePath + "CheckFileDownloadAccess?OrderNo=" + OrderId + "&PartNo=" + PartNo + "&UserID=" + userGuid);
    };
    return OrderDocumentService;
});