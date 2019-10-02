app.service('PrintInvoiceService', function ($http, configurationService) {
    var PrintInvoiceService = [];

    PrintInvoiceService.PrintInvoice = function (InvoiceNumber, OrderId, PartNo, PrintAll) {
        return $http.get(configurationService.basePath + "PrintInvoice?InvoiceNumber=" + InvoiceNumber + "&OrderId=" + OrderId + "&PartNo=" + PartNo + "&PrintAll=" + PrintAll);
    };

    PrintInvoiceService.PrintInvoiceDetail = function (InvoiceNumber) {
        return $http.get(configurationService.basePath + "PrintInvoiceDetail?InvoiceNumber=" + InvoiceNumber);
    };

    PrintInvoiceService.DownloadInvoice = function (strHTML) {
        // strHTML = "url: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePathurl: configurationService.basePath";
        
        $http({
            // url: configurationService.basePath + "DownloadInvoice?strHTML=" + JSON.stringify(strHTML),
            url: configurationService.basePath + "DownloadInvoice",
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
    
    return PrintInvoiceService;
});