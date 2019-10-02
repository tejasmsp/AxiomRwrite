app.controller('AssistantContactController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, AssistantContactService, configurationService, $compile, $filter) {
    decodeParams($stateParams);
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;


    //#region Event

    $scope.GetAssistantContactNotificationInformationByOrderId = function (OrderId) {
        var promise = AssistantContactService.GetAssistantContactNotificationInformationByOrderId(OrderId);
        promise.success(function (response) {
            $scope.AssistantContactNotificationInfoList = response.Data;
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.SaveDetail = function (form) {
        if (form.$valid) {
            var promise = AssistantContactService.UpdateAssistantContact($scope.OrderId,$scope.AssistantContactNotificationInfoList);
            promise.success(function (response) {
                if (response.Success) {
                    if (response.lng_InsertedId == "1") {
                        $scope.GetAssistantContactNotificationInformationByOrderId($scope.OrderId);
                        toastr.success("Save successfully");
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                }
            });
            promise.error(function (data, statusCode) {
            });
        }
    };
    //#endregion

    //#region Method    

    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.PartNo = isNullOrUndefinedOrEmpty($stateParams.PartNo) ? 0 : $stateParams.PartNo;
        $scope.AssistantContactObj = new Object();
        $scope.GetAssistantContactNotificationInformationByOrderId($scope.OrderId);
    }

    //#endregion
    
    
    init();

});