app.service('LocationServices', function ($http, $rootScope, configurationService) {
    var LocationService = [];

    LocationService.GetLocationList = function (locationId) {
        return $http.get(configurationService.basePath + "GetLocationList?LocationId=" + locationId);
    };

    LocationService.InsertLocation = function (Locationobj, EmpID) {
        return $http.post(configurationService.basePath + "InsertLocation?LoginEmpId=" + EmpID, Locationobj);
    };

    LocationService.UpdateLocation = function (Locationobj, EmpID) {
        return $http.post(configurationService.basePath + "UpdateLocation?LoginEmpId=" + EmpID, Locationobj);
    };

    LocationService.DeleteLocation = function (locationId) {
        return $http.post(configurationService.basePath + "DeleteLocation?LocationId=" + locationId);
    };

    LocationService.UpdateMergeLocation = function (locID, parentLocationId, needtoMerge) {
        return $http.post(configurationService.basePath + "UpdateMergeLocation?locID=" + locID + "&parentLocationId=" + parentLocationId + "&needtoMerge=" + needtoMerge);
    };

    LocationService.GetLocationById = function (locationId) {
        return $http.get(configurationService.basePath + "GetLocationById?LocationId=" + locationId);
    };

    LocationService.GetLocationFormsList = function (locationId, IsRequestForm) {
        return $http.get(configurationService.basePath + "GetLocationFormsList?LocationId=" + locationId + "&IsRequestForm=" + IsRequestForm);
    };
    LocationService.GetFormsAndDirectoryList = function () {
        return $http.get(configurationService.basePath + "GetFormsAndDirectoryList");
    };

    LocationService.InsertLocationForm = function (model) {
        return $http.post(configurationService.basePath + "InsertLocationForm", model);
    };

    LocationService.DeleteLocationForm = function (LocformID) {
        return $http.post(configurationService.basePath + "DeleteLocationForm?LocformID=" + LocformID);
    };

    LocationService.GetLocationListWithSearchFromLocation = function (SearchCriteria, SearchCondition, SearchText) {
        return $http.get(configurationService.basePath + "GetLocationListWithSearch?SearchCriteria=" + SearchCriteria + '&SearchCondition=' + SearchCondition + '&SearchText=' + SearchText);
    };
    return LocationService;
});