app.service('AccessReportService', function ($http, configurationService) {
    var accessReportService = [];


    accessReportService.DisplayAccessReportPartsByDate = function (reportName, StartDate, EndDate, CompanyID, CheckNumber, SSNNumber, FirmID, CompanyNo) {
        var reportURL = "DisplayAccessReport" + reportName + "?reportName=";
        // return $http.get(configurationService.basePath + "DisplayAccessReportPartsByDate?reportName=" + reportName + "&StartDate=" + StartDate + "&EndDate=" + EndDate + "&CompanyID=" + CompanyID + "&CheckNumber=" + CheckNumber + "&ssnNumber=" + SSNNumber);
        return $http.get(configurationService.basePath + reportURL + reportName + "&StartDate=" + StartDate + "&EndDate=" + EndDate + "&CompanyID=" + CompanyID + "&CheckNumber=" + CheckNumber + "&ssnNumber=" + SSNNumber + "&FirmID=" + FirmID + "&CompanyNo=" + CompanyNo);
    };

    accessReportService.DownloadAccessReport = function (reportName, StartDate, EndDate, CompanyID, CheckNumber, SSNNumber, FirmID) {
        $http({
            // url: configurationService.basePath + "DownloadInvoice?strHTML=" + JSON.stringify(strHTML),
            url: configurationService.basePath + "DownloadAccessReport?reportName=" + reportName + "&StartDate=" + StartDate + "&EndDate=" + EndDate + "&CompanyID=" + CompanyID + "&CheckNumber=" + CheckNumber + "&ssnNumber=" + SSNNumber + "&FirmID=" + FirmID,
            method: "GET",
            // data: reportName,
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
    };
    accessReportService.DownloadAccessReportAgedAR = function (StartDate, EndDate, CompanyID, FirmID) {
        $http({
            // url: configurationService.basePath + "DownloadInvoice?strHTML=" + JSON.stringify(strHTML),
            url: configurationService.basePath + "DownloadAgedAR?StartDate=" + StartDate + "&EndDate=" + EndDate + "&CompanyID=" + CompanyID + "&FirmID=" + FirmID,
            method: "GET",
            // data: reportName,
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
    };
    return accessReportService;
});