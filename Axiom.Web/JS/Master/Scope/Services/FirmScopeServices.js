app.service('FirmScopeServices', function ($http, configurationService) {
    var FirmScopeService = [];

    FirmScopeService.GetFirmScopeList = function (FirmID, RecTypeID) {
        return $http.get(configurationService.basePath + "GetFirmScopeList?FirmID=" + FirmID + "&RecTypeID=" + RecTypeID);
    };

    FirmScopeService.GetScopeByRecType = function (RecType) {
        return $http.get(configurationService.basePath + "GetScopeByRecType?RecType=" + RecType);
    };

    FirmScopeService.InsertFirmDefaultScope = function (model) {
        return $http.post(configurationService.basePath + "InsertFirmDefaultScope", model);
    };

    FirmScopeService.UpdateFirmDefaultScope = function (model) {
        return $http.post(configurationService.basePath + "UpdateFirmDefaultScope", model);
    };

    FirmScopeService.GetFirmScopeById = function (mapId) {
        return $http.get(configurationService.basePath + "GetFirmScopeById?mapId=" + mapId);
    };
    FirmScopeService.DeleteFirmScope = function (mapId) {
        return $http.post(configurationService.basePath + "DeleteFirmScope?mapId=" + mapId);
    };

    return FirmScopeService;
});