app.service('SearchOrderListService', function ($http, configurationService) {
    var SearchOrderListService = []; 

    SearchOrderListService.ArchiveOrder1 = function (orderObj) {
        return $http.post(configurationService.basePath + "ArchiveOrder1", orderObj);
    };
    SearchOrderListService.GenerateInvoiceMultiple = function (OrderPartList) {
        return $http.post(configurationService.basePath + "GenerateInvoiceMultiple", OrderPartList);
    };

  
    return SearchOrderListService;
});