app.service('AttorneyScopeServices', function ($http, configurationService) {
    var AttorneyScopeService = [];

    AttorneyScopeService.GetAttorneyScopeList = function () {
        return $http.get(configurationService.basePath + "GetAttorneyScopeList");
    };
    AttorneyScopeService.InsertAttorneyScope = function (model) {
        return $http.post(configurationService.basePath + "InsertAttorneyScope", model);
    };

    AttorneyScopeService.UpdateAttorneyScope = function (model) {
        return $http.post(configurationService.basePath + "UpdateAttorneyScope", model);
    };

    AttorneyScopeService.GetAttorneyScopeById = function (mapId) {
        return $http.get(configurationService.basePath + "GetAttorneyScopeById?mapId=" + mapId);
    };
    AttorneyScopeService.DeleteAttorneyScope = function (mapId) {
        return $http.post(configurationService.basePath + "DeleteAttorneyScope?mapId=" + mapId);
    };
    AttorneyScopeService.GetDefaultScope = function () {
        return $http.get(configurationService.basePath + "GetDefaultScope");
    };
    AttorneyScopeService.GetDefaultScopeById = function (ScopeID) {        
        return $http.get(configurationService.basePath + "GetDefaultScopeById?ScopeID=" + ScopeID);
    };
    AttorneyScopeService.UpdateDefaultScope = function (model) {
        return $http.post(configurationService.basePath + "UpdateDefaultScope", model);
    };


    return AttorneyScopeService;
});