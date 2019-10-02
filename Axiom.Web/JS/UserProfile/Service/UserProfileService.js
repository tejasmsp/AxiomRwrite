app.service('UserProfileService', function ($http, configurationService) {
    var userProfileService = [];

    userProfileService.GetUserMasterList = function (UserAccessID) {
        return $http.get(configurationService.basePath + "GetUserMasterList?UserAccessID=" + UserAccessID);
    };

    userProfileService.UpdateUserMaster = function (objUserProfile, file) {
        return $http.post(configurationService.basePath + "UpdateUserMaster", objUserProfile);
    };

    userProfileService.UploadUserImage = function (fd, UserAccessID) {
          return $.ajax({
              url: configurationService.basePath + "UploadFile?UserAccessID=" + UserAccessID,
              data: fd,
            cache: false,
            contentType: false,
            processData: false,
            method: 'POST',
            type: 'POST', // For jQuery < 1.9
            success: function (data) {
            }
        });
    };
    
    return userProfileService;
});