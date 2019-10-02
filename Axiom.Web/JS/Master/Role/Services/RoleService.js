app.service('RoleServices', function ($http, configurationService) {
    var RoleServices = [];

    // Roles Methods
    RoleServices.GetRole = function (roleAccessId) {
        return $http.get(configurationService.basePath + "GetRole?roleAccessId=" + roleAccessId);
    };

    RoleServices.InsertRole = function (roleObj) {
        return $http.post(configurationService.basePath + "InsertRole", roleObj);
    };

    RoleServices.UpdateRole = function (roleObj) {
        return $http.post(configurationService.basePath + "UpdateRole", roleObj);
    };

    // RoleRights Methods
    RoleServices.GetRoleRights = function (RoleAccessId, UserAccessId) {
        return $http.get(configurationService.basePath + "/GetRoleRights?RoleAccessId=" + RoleAccessId + "&UserAccessId=" + UserAccessId);
    }

    RoleServices.AddOrUpdateRoleConfiguration = function (UserAccessId, RoleRightsCollection) {
        return $http.post(configurationService.basePath + "/AddOrUpdateRoleConfiguration?UserAccessId=" + UserAccessId, RoleRightsCollection);
    }

    return RoleServices;
});