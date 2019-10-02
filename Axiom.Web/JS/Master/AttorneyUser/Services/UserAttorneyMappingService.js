app.service('UserAttorneyMappingService', function ($http, configurationService) {
    var UserAttorneyMappingService = [];
    UserAttorneyMappingService.GetUserAttorneyMapping = function (userId, selectOnlyCurrentAccessAttorney) {
        return $http.get(configurationService.basePath + "GetUserAttorneyMapping?userId=" + userId + "&selectOnlyCurrentAccessAttorney=" + selectOnlyCurrentAccessAttorney);
    };

    UserAttorneyMappingService.DeleteAttorneyUserMapping = function (attorneyUserId, attorneyId) {
        
        return $http.get(configurationService.basePath + "DeleteAttorneyUserMapping?AttorneyUserId=" + attorneyUserId + "&attorneyId=" + attorneyId );
    };

    UserAttorneyMappingService.InsertAttorneyUserMapping = function (attorneyUserId, attorneyId) {

        return $http.get(configurationService.basePath + "InsertAttorneyUserMapping?AttorneyUserId=" + attorneyUserId + "&attorneyId=" + attorneyId);
    };

   
    return UserAttorneyMappingService;
});