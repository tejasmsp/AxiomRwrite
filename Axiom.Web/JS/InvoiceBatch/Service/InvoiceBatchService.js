app.service('InvoiceBatchService', function ($http, configurationService) {
    var InvoiceBatchService = [];

    InvoiceBatchService.InvoiceBatchCheckLED = function (FirmID) {
        return $http.get(configurationService.basePath + "InvoiceBatchCheckLED?FirmID=" + FirmID);
    };
    InvoiceBatchService.GenerateLEDESFile = function (FirmID, Caption, ClaimNo, AttyID, FromDate, ToDate, InvNo, SoldAtty) {        
        return $http.get(configurationService.basePath + "GenerateLEDESFile?FirmID=" + FirmID + "&Caption=" + Caption + "&ClaimNo=" + ClaimNo + "&AttyID=" + AttyID + "&FromDate=" + FromDate + "&ToDate=" + ToDate + "&InvNo=" + InvNo + "&SoldAtty=" + SoldAtty);
    };

    return InvoiceBatchService;
});