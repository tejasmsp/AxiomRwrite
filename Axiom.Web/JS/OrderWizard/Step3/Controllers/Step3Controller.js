app.controller('Step3Controller', function ($scope, $rootScope, $state, $stateParams, notificationFactory, CommonServices, Step1Service, Step3Service, configurationService, $compile, $filter) {
    

    decodeParams($stateParams);

    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;

    function bindStateForDropdown() {
        
        var state = CommonServices.StateDropdown();
        state.success(function (response) {            
            $scope.StateList = response.Data;
            $scope.StateList.splice(0, 0, { SateID: "", StateName: "-- Select --" });
            
        });
    };
    

    $scope.GetOrderWizardStep3Details = function (OrderId) {
        var promise = Step3Service.GetOrderWizardStep3Details(OrderId);
        promise.success(function (response) {
            $scope.OrderStep3Obj = response.Data[0];
            if (!isNullOrUndefinedOrEmpty($scope.OrderStep3Obj.DateOfBirth)) {
                $scope.OrderStep3Obj.DateOfBirth = $filter('date')(new Date($scope.OrderStep3Obj.DateOfBirth), $rootScope.GlobalDateFormat);
            }
            if (!isNullOrUndefinedOrEmpty($scope.OrderStep3Obj.DateOfDeath)) {
                $scope.OrderStep3Obj.DateOfDeath = $filter('date')(new Date($scope.OrderStep3Obj.DateOfDeath), $rootScope.GlobalDateFormat);
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.SubmitStep3 = function (form) {
        if (form.$valid) {
            if (!isNullOrUndefinedOrEmpty($scope.OrderStep3Obj.DateOfBirth)) {
                var TodayDate = new Date();
                var DOB = new Date($scope.OrderStep3Obj.DateOfBirth);
                if (TodayDate < DOB) {
                    toastr.error('Date of Birth must be lower than Today date.');
                    return;
                }
            }
            if (!isNullOrUndefinedOrEmpty($scope.OrderStep3Obj.DateOfDeath)) {
                var DOB = new Date($scope.OrderStep3Obj.DateOfBirth);
                var DOD = new Date($scope.OrderStep3Obj.DateOfDeath);
                if (DOB > DOD) {
                    toastr.error('Date of Death must be greater than Date of Birth');
                    return;
                }
            }
            $scope.OrderStep3Obj.EmpId = $scope.EmpId;
            $scope.OrderStep3Obj.UserAccessId = $scope.UserAccessId;
            $scope.OrderStep3Obj.OrderId = $scope.OrderId;
            if ($scope.OrderId > 0) {
                var promise = Step3Service.InsertOrUpdateOrderWizardStep3($scope.OrderStep3Obj);
                promise.success(function (response) {
                    if (response.Success) {
                        //$state.go("EditOrder", { OrderId: response.lng_InsertedId, Step: $rootScope.Enum.OrderWizardStep.Step4 });
                        $scope.OrderId = response.lng_InsertedId;
                        $scope.MoveNext();
                    }
                    else if (response.lng_InsertedId == -1) {
                        toastr.error("Order No: " + $scope.OrderId + " not found.");
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
        }
    };


    function init() {
      
        $scope.OrderId = $stateParams.OrderId;
        $scope.OrderStep3Obj = new Object();
        $scope.currentStateId = '';
        bindStateForDropdown();        
        $scope.GetOrderWizardStep3Details($scope.OrderId);

    }
    init();

});