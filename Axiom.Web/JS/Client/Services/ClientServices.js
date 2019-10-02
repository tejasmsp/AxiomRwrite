app.service('ClientServices', function ($http, configurationService) {
    var ClientServices = [];

    ClientServices.GetClientPartStatus = function (UserID) {
        return $http.get(configurationService.basePath + "GetClientPartStatus?UserID=" + UserID);
    };
    ClientServices.GetClientDashboardOrders = function (UserID) {
        return $http.get(configurationService.basePath + "GetClientDashboardOrders?UserID=" + UserID);
    };
    ClientServices.GetClientDashboardParts = function (OrderId, PartStatusGroupId) {
        return $http.get(configurationService.basePath + "GetClientDashboardParts?OrderId=" + OrderId + "&PartStatusGroupId=" + PartStatusGroupId);
    };

    ClientServices.ClientPartReport = function (UserID,ReportName) {
        // strHTML = "url: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePath";

        $http({
            // url: configurationService.basePath + "DownloadInvoice?strHTML=" + JSON.stringify(strHTML),
            url: configurationService.basePath + "ClientPartReport?UserID=" + UserID + "&ReportName=" + ReportName,
            method: "GET",            
            // data: "strHTML=" + strHTML,
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
                //  window.open(objectUrl);
            }
        }).error(function (data, status, headers, config) {
        });

        // return $http.get(configurationService.basePath + "DownloadInvoice?strHTML=" + strHTML);
    };
    ClientServices.DownloadClientRecords = function (strHTML) {
        // strHTML = "url: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePath";

        $http({
            // url: configurationService.basePath + "DownloadInvoice?strHTML=" + JSON.stringify(strHTML),
            url: configurationService.basePath + "DownloadClientRecords",
            method: "POST",
            data: strHTML,
            // data: "strHTML=" + strHTML,
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
                //  window.open(objectUrl);
            }
        }).error(function (data, status, headers, config) {
        });

        // return $http.get(configurationService.basePath + "DownloadInvoice?strHTML=" + strHTML);
    };

    ClientServices.DownloadFileMultiple = function (objFile) {
        return $http({
            method: 'POST',
            url: configurationService.basePath + "DownloadFileMultiple",
            data: objFile,
            responseType: 'arraybuffer'
        });
    };
    ClientServices.DownloadFile = function (fileDiskName, fileName, orderNo, partNo) {
        return $http({
            method: 'GET',
            url: configurationService.basePath + "DownloadFile?fileDiskName=" + fileDiskName + "&fileName=" + fileName + "&orderNo=" + orderNo + "&partNo=" + partNo,
            responseType: 'arraybuffer'
        });
    };
    ClientServices.GetClientFileByFileType = function (OrderId, PartNo, fileTypeId) {
        return $http.get(configurationService.basePath + "GetClientFileByFileType?OrderId=" + OrderId + "&PartNo=" + PartNo + "&FileTypeId=" + fileTypeId);
    };
    ClientServices.ClientDashboardRemovePartFromList = function (OrderNo, PartNo) {
        return $http.post(configurationService.basePath + "ClientDashboardRemovePartFromList?OrderNo=" + OrderNo + "&PartNo=" + PartNo);
    };
    return ClientServices;
});