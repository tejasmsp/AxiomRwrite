app.service('AttorneyEmployeeTypeService', function ($http, configurationService) {
    var AttorneyEmployeeTypeService = [];
    AttorneyEmployeeTypeService.GetAttorneyEmployeeType = function () {
        return $http.get(configurationService.basePath + "GetAttorneyEmployeeType");
    };

    return AttorneyEmployeeTypeService;
});