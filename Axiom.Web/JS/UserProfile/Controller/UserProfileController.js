app.controller('UserProfileController', function ($scope, $rootScope, $stateParams, notificationFactory, UserProfileService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.isEdit = false;

    $scope.UpdateUserProfile = function (form) {
        
        if (form.$valid) {

            var promise = UserProfileService.UpdateUserMaster($scope.objUserProfile);
            promise.success(function (response) {
                if (response.Success) {
                    var fd = new FormData();
                    fd.append("file", $("#UserImage")[0].files[0]);
                    var fileupload = UserProfileService.UploadUserImage(fd, $scope.UserAccessId);
                    fileupload.success(function (response) {                        
                        notificationFactory.customSuccess("UserProfile Update Successfully");
                        angular.element("#modal_MyProfile").modal('hide');
                    });                    
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) {
            });
        }

    };
    

    $scope.GetUserProfileDetail = function () {
        //$scope.UserProfileForm.$setPristine();
        var promise = UserProfileService.GetUserMasterList($scope.UserAccessId);
        promise.success(function (response) {
            $scope.objUserProfile = response.Data[0];
            // bindCourtList();
        });
        promise.error(function (data, statusCode) {
        });
    }

    function init() {
        $scope.GetUserProfileDetail();
    }
    init();
});