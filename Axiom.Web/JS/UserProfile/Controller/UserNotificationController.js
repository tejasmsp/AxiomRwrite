app.controller('UserNotificationController', function ($scope, $rootScope, $stateParams, notificationFactory, MessageServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);

    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;



    $scope.HideNotification = function () {
        var promise = MessageServices.InsertNotificationReadByUser($scope.userGuid);
        promise.success(function (response) {
            $scope.objHideNotification = response.Data;
            $rootScope.NotificationCount = '';
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.GetNotification = function () {
        var promise = MessageServices.GetCustomMessageForClient();
        promise.success(function (response) {
            $scope.objCustomNotification = response.Data;
            $rootScope.NotificationCount = $scope.objCustomNotification.length;
        });
        promise.error(function (data, statusCode) {
        });
    };
  
    $scope.HaveUserReadNotification = function () {
        var promise = MessageServices.GetNotificationReadByUser($scope.userGuid);
        promise.success(function (response) {            
            if (response.Data.length > 0) {
                $rootScope.NotificationCount = '';
            }
            
        });
        promise.error(function (data, statusCode) {
        });
    }

    function init() {
        $scope.GetNotification();
        $scope.HaveUserReadNotification();
        $rootScope.getEmployeeCurrentLog();
    }
    init();
})