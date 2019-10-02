app.service('CourtService', function ($http, configurationService) {
    var courtService = [];

    courtService.GetCourtList = function (CourtId) {
        return $http.get(configurationService.basePath + "GetCourtList?CourtId=" + CourtId);
    };

    courtService.GetStateList = function () {
        return $http.get(configurationService.basePath + "GetStateList");
    };
    courtService.InsertCourt = function (objCourt) {
        return $http.post(configurationService.basePath + "InsertCourt", objCourt);
    };

    courtService.UpdateCourt = function (objCourt) {
        return $http.post(configurationService.basePath + "UpdateCourt", objCourt);
    };

    courtService.DeleteCourt = function (CourtID) {
        return $http.post(configurationService.basePath + "DeleteCourt?CourtID=" + CourtID);
    };
    return courtService;
});