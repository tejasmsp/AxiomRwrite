app.service('ChangePasswordService', function ($http, configurationService) {
    var changePasswordService = [];

    changePasswordService.UserMasterUpdatePassword = function (objUserProfile) {
        return $http.post(configurationService.basePath + "UserMasterUpdatePassword", objUserProfile);
    };

    return changePasswordService;
});