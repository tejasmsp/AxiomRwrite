app.service('LocationFeesService', function ($http, configurationService) {
    var LocationFeesService = [];

    LocationFeesService.GetProposalFees = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "GetProposalFees?OrderId=" + OrderId + "&PartNo=" + PartNo);
    };

    LocationFeesService.GetLocationFees = function (OrderId, PartNo) {
        return $http.get(configurationService.basePath + "GetLocationFees?OrderNo=" + OrderId + "&PartNo=" + PartNo);
    };
    LocationFeesService.InsertUpdateLocationFeesChecks = function (model) {
        return $http.post(configurationService.basePath + "InsertUpdateLocationFeesChecks", model);
    };
    LocationFeesService.DeleteLocationFeesChecks = function (ChkID) {
        return $http.post(configurationService.basePath + "DeleteLocationFeesChecks?ChkID=" + ChkID);
    };
    LocationFeesService.UpdateLocationFeesVoidChecks = function (ChkID) {
        return $http.post(configurationService.basePath + "UpdateLocationFeesVoidChecks?ChkID=" + ChkID);
    };
    LocationFeesService.GetPrintCheckIIFFiles = function (fromDate, toDate, checkNo, checkId) {
        $http({
            url: configurationService.basePath + "GetPrintCheckIIFFiles?fromDate=" + fromDate + "&toDate=" + toDate + "&checkNo=" + checkNo + "&checkId=" + checkId,
            method: "POST",
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
            if (navigator.appVersion.toString().indexOf('.NET') > 0) 
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
    };

    LocationFeesService.GetGenerateIIFFiles = function (fromDate, toDate, checkNo, checkId) {
        $http({
            url: configurationService.basePath + "GetGenerateIIFFiles?fromDate=" + fromDate + "&toDate=" + toDate + "&checkNo=" + checkNo + "&checkId=" + checkId,
            method: "POST",
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
            if (navigator.appVersion.toString().indexOf('.NET') > 0)
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
    };
    return LocationFeesService;
});