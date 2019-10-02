app.service('HomeServices', function ($http, configurationService) {
    var HomeService = [];

    HomeService.GetActionDateStatus = function () {
        return $http.get(configurationService.basePath + "GetActionDateStatus");
    }; 

    return HomeService;
});