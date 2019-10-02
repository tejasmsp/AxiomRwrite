app.service('DistrictServices', function ($http, configurationService) {
    var districtService = [];

    districtService.GetDistrictList = function (districtid) {
        return $http.get(configurationService.basePath + "GetDistrictList?districtid=" + districtid);
    };
    districtService.InsertDistrict = function (Districtobj) {
        return $http.post(configurationService.basePath + "InsertDistrict", Districtobj);
    };

    districtService.UpdateDistrict = function (Districtobj) {
        return $http.post(configurationService.basePath + "UpdateDistrict", Districtobj);
    };


    districtService.CheckUniqueDistrict = function (Districtobj) {
        return $http.post(configurationService.basePath + "CheckUniqueDistrict", Districtobj);
    };

    districtService.DeleteDistrict = function (districtid) {
        return $http.post(configurationService.basePath + "DeleteDistrict?districtid=" + districtid);
    };

    return districtService;
});