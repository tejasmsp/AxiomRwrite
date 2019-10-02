app.service('AttorneyUserService', function ($http, configurationService) {
    var AttorneyUserService = [];

    AttorneyUserService.GetAttorneyUsers = function (AttorneyUserId) {
        return $http.get(configurationService.basePath + "GetAttorneyUsers?attorneyUserId=" + AttorneyUserId);
    };

    AttorneyUserService.ActivateInactiveAttorneyUser = function (AttorneyUserId) {
        return $http.get(configurationService.basePath + "ActivateInactiveAttorneyUser?attorneyUserId=" + AttorneyUserId);
    };

    AttorneyUserService.InsertAttorneyUser = function (AttorneyUserObj) {
        return $http.post(configurationService.basePath + "InsertAttorneyUser", AttorneyUserObj);
    };

    AttorneyUserService.UpdateAttorneyUser = function (AttorneyUserObj) {
        return $http.post(configurationService.basePath + "UpdateAttorneyUser", AttorneyUserObj);
    };

    AttorneyUserService.DeleteAttorneyUser = function (AttorneyUserId) {
        return $http.get(configurationService.basePath + "DeleteAttorneyUser?attorneyUserId=" + AttorneyUserId);
    };
    
    AttorneyUserService.GetAttorneyListWithSearchCriteria = function (SearchCriteria,SearchCondition,SearchText) {
        return $http.get(configurationService.basePath + "GetAttorneyListWithSearchCriteria?SearchCriteria=" + SearchCriteria + '&SearchCondition=' + SearchCondition + '&SearchText=' + SearchText);
    };   
    
    return AttorneyUserService;
});