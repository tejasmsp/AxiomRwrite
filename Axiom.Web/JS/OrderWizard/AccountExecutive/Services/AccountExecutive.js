app.service('AccountExecutive', function ($http, configurationService) {
    var AccountExecutive = [];


    AccountExecutive.GetClientAccountExecutive = function (userGuid) {
        return $http.get(configurationService.basePath + "GetClientAccountExecutive?UserId=" + userGuid);
    };

   



    return AccountExecutive;
});