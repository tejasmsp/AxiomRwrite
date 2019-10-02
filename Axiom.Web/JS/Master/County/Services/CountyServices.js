app.service('CountyServices', function ($http, configurationService) {
    var countyService = [];

    countyService.GetCountyList = function (countyId) {
        return $http.get(configurationService.basePath + "GetCountyList?countyId=" + countyId);
    };

   
    countyService.InsertCounty = function (Countyobj) {
        return $http.post(configurationService.basePath + "InsertCounty", Countyobj);
    };

    countyService.UpdateCounty = function (Countyobj) {
        return $http.post(configurationService.basePath + "UpdateCounty", Countyobj);
    };

    countyService.DeleteCounty = function (countyId) {
        return $http.post(configurationService.basePath + "DeleteCounty?countyId=" + countyId);
    };

    countyService.CheckUniqueCounty = function (Countyobj) {
        return $http.post(configurationService.basePath + "CheckUniqueCounty", Countyobj);
    };
    
    return countyService;
});