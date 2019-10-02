app.service('SSNService', function ($http, configurationService) {
    var SSNService = [];

    SSNService.GetSSNSettingList = function () {
        return $http.get(configurationService.basePath + "GetSSNSettingList");
    };

    SSNService.UpdateSSNSetting = function (objssn) {
        return $http.post(configurationService.basePath + "UpdateSSNSetting",objssn);
    };
    return SSNService;
});