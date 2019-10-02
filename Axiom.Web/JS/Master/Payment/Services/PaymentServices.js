app.service('PaymentServices', function ($http, configurationService) {
    var paymentService = [];

    paymentService.GetPaymentList = function (pmtDate, PmtNo) {
        return $http.get(configurationService.basePath + "GetPaymentList?pmtDate=" + pmtDate + "&PmtNo=" + PmtNo);
    };


    //paymentService.InsertCounty = function (Countyobj) {
    //    return $http.post(configurationService.basePath + "InsertCounty", Countyobj);
    //};

    //paymentService.UpdateCounty = function (Countyobj) {
    //    return $http.post(configurationService.basePath + "UpdateCounty", Countyobj);
    //};

    paymentService.DeletePayment = function (PmtNo) {
        return $http.post(configurationService.basePath + "DeletePayment?PmtNo=" + PmtNo);
    };

    //paymentService.CheckUniqueCounty = function (Countyobj) {
    //    return $http.post(configurationService.basePath + "CheckUniqueCounty", Countyobj);
    //};

    return paymentService;
});