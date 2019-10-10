app.service('SearchOrderListService', function ($http, configurationService) {
    var SearchOrderListService = [];

    SearchOrderListService.ArchiveOrder1 = function (orderObj) {
        return $http.post(configurationService.basePath + "ArchiveOrder1", orderObj);
    };
    SearchOrderListService.GenerateInvoiceMultiple = function (OrderPartList, CompanyNo) {
        return $http.post(configurationService.basePath + "GenerateInvoiceMultiple?CompanyNo=" + CompanyNo, OrderPartList);
    };


    return SearchOrderListService;
});