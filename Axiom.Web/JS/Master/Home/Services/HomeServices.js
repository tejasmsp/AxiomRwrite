app.service('HomeServices', function ($http, configurationService, $rootScope) {
    var HomeService = [];

    HomeService.GetActionDateStatus = function () {
        return $http.get(configurationService.basePath + "GetActionDateStatus?CompanyNo=" + $rootScope.CompanyNo);
    }; 

    return HomeService;
});