app.controller('ChangePasswordController', function ($scope, $rootScope, $stateParams, notificationFactory, ChangePasswordService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;

    $scope.ChangePassword = function (form) {
        if (form.$valid) {
            $scope.objUserProfile.UserAccessId = $scope.UserAccessId;
            if ($scope.objUserProfile.Password == $scope.objUserProfile.ConfirmPassword) {
                var promise = ChangePasswordService.UserMasterUpdatePassword($scope.objUserProfile)
                promise.success(function (response) {
                    if (response.Success) {
                        notificationFactory.customSuccess("UserProfile Update Successfully");
                        angular.element("#modal_MyProfileChangePassword").modal('hide');
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else {
                toastr.error("Password and Confirm Password must match !");
            }
        }
    }


    function init() {

    }
    init();
});