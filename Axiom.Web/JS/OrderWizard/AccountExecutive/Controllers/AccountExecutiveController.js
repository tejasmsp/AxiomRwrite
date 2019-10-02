app.controller('AccountExecutiveController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, CommonServices, AccountExecutive, configurationService, $compile, $filter) {
    decodeParams($stateParams);


    function init() {
        $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
        $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
        $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
        $scope.AccountExecutiveObj = new Object();
        $scope.GetClientAccountExecutive($scope.userGuid);
    }


    $scope.GetClientAccountExecutive = function (userGuid) {
        var promise = AccountExecutive.GetClientAccountExecutive(userGuid);
        promise.success(function (response) {
            if (response.Data && response.Data.length > 0) {
                $scope.AccountExecutiveObj = response.Data[0];
            }
        });
        promise.error(function (data, statusCode) {
        });
    }


    init();

});