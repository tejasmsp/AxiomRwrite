app.service('AssignUniversalListServices', function ($http, configurationService) {
    var AssignUniversalListServices = [];
    AssignUniversalListServices.GetUniversalList = function () {
        return $http.get(configurationService.basePath + "GetUniversalList");
    }; 
    AssignUniversalListServices.UpdateUniversalStatus = function (model) {
        return $http.post(configurationService.basePath + "UpdateUniversalStatus", model);
    }; 
    return AssignUniversalListServices;
});