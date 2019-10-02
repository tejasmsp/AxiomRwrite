app.service('QuickFormService', function ($http, configurationService) {
    var QuickForm = [];

    QuickForm.QuickFormGetPartDetail = function (OrderId) {
        return $http.get(configurationService.basePath + "QuickFormGetPartDetail?OrderNo=" + OrderId);
    };

    QuickForm.QuickFormGetFormList = function () {
        return $http.get(configurationService.basePath + "QuickFormGetFormList");
    };

    QuickForm.QuickFormGetFileList = function () {
        return $http.get(configurationService.basePath + "QuickFormGetFileList");
    };
    QuickForm.QuickFormGetOrderingAttorney = function (OrderId) {
        return $http.get(configurationService.basePath + "QuickFormGetOrderingAttorney?OrderNo=" + OrderId);
    };

    QuickForm.QuickFormGetDocumentListByType = function (type, OrderNo, PartNo) {
        return $http.get(configurationService.basePath + "QuickFormGetDocumentListByType?Type=" + type + "&OrderNo=" + OrderNo + "&PartNo=" + PartNo);
    };

    QuickForm.UpdateOrderPart = function (model) {
        return $http.post(configurationService.basePath + "UpdateOrderPart", model);
    };
    QuickForm.AddUpdateChronolgy = function (OrderId, PartNo, userGuid, IsChronology) {
        return $http.post(configurationService.basePath + "AddUpdateChronolgy?OrderId=" + OrderId + "&PartNo=" + PartNo + "&userGuid=" + userGuid + "&IsChronology=" + IsChronology);
    };

    QuickForm.QuickFormGetAttachFileList = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "QuickFormGetAttachFileList?OrderNo=" + OrderId + "&PartNo=" + PartNo);
    };
    QuickForm.QuickFormInsert = function (objQuickFormList) {
        return $http.post(configurationService.basePath + "QuickFormInsert", objQuickFormList);
    };

    QuickForm.UploadDocumentAttachment = function (fd, CreatedBy, batchId) {

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
    QuickForm.DeleteUploadedDocument = function (batchId, fileName, createdBy) {
        return $http.post(configurationService.basePath + "DeleteUploadedDocument?batchId=" + batchId + "&fileName=" + fileName + "&createdBy=" + createdBy);
    };
    QuickForm.GetFormsAndDirectoryList = function () {
        return $http.get(configurationService.basePath + "GetFormsAndDirectoryList");
    };
    QuickForm.QuickFormGetPdf = function (ObjQuickDocument) {
        debugger;
        ObjQuickDocument = encodeURIComponent(JSON.stringify(ObjQuickDocument));
        window.open(configurationService.basePath + "QuickFormGetPdfNew?jsonObject=" + ObjQuickDocument, "_blank");
        return;

        $http({
            url: configurationService.basePath + "QuickFormGetPdfNew?jsonObject=" + ObjQuickDocument,
            method: "GET",
            data: ObjQuickDocument,
            contentType: "application/json; charset=utf-8",
            responseType: 'arraybuffer'
        }).success(function (data, status, headers, config) {
            var type = headers('Content-Type');
            var disposition = headers('Content-Disposition');
            if (disposition) {
                var match = disposition.match(/.*filename=\"?([^;\"]+)\"?.*/);
                if (match[1])
                    defaultFileName = match[1];
            }
            defaultFileName = defaultFileName.replace(/[<>:"\/\\|?*]+/g, '_');
            var blob = new Blob([data], { type: type });
            if (navigator.appVersion.toString().indexOf('.NET') > 0) // For IE 
                window.navigator.msSaveBlob(blob, defaultFileName);
            else {
                var objectUrl = URL.createObjectURL(blob);
                var downloadLink = document.createElement("a");
                downloadLink.href = objectUrl;
                downloadLink.download = defaultFileName;
                document.body.appendChild(downloadLink);
                downloadLink.click();
                document.body.removeChild(downloadLink);
            }
            angular.element("#modal_QuickForm").modal('hide');
        }).error(function (data, status, headers, config) {
        });
    };

    QuickForm.GetDocumentRootPath = function () {
        return $http.get(configurationService.basePath + "GetDocumentRootPath");
    };
    return QuickForm;
});