app.service('IIFFilesService', function ($http, configurationService) {
    var IIFFilesService = [];

    IIFFilesService.GetIIFFileForDay = function (date, ToBePrint) {
        return $http.get(configurationService.basePath + "GetIIFFileForDay?Date=" + date + "&ToBePrint=" + ToBePrint);
    };
    IIFFilesService.GetIIFFileForDayCSV = function (date, ToBePrint) {
        return $http.get(configurationService.basePath + "GetIIFFileForDayCSV?Date=" + date + "&ToBePrint=" + ToBePrint);
    };

    IIFFilesService.IIFGenerateCheckList = function (fromDate, toDate) {
        return $http.post(configurationService.basePath + "IIFGenerateCheckList?fromdate="+ fromDate + "&todate=" + toDate);
    };
    IIFFilesService.PrintCheckIIFFiles = function (fromDate, toDate, checkID, checkNo) {
       //  return $http.post(configurationService.basePath + "PrintCheckIIFFiles?fromdate=" + fromDate + "&todate=" + toDate + "&checkID=" + checkID + "&checkNo=" + checkNo );

        $http({
            url: configurationService.basePath + "PrintCheckIIFFiles?fromdate=" + fromDate + "&todate=" + toDate + "&checkID=" + checkID + "&checkNo=" + checkNo,
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
    return IIFFilesService;
});