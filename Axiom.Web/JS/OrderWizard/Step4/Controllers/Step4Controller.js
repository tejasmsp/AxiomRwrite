app.controller('Step4Controller', function ($scope, $rootScope, $state, $stateParams, notificationFactory, CommonServices, CourtService, Step1Service, Step4Service, configurationService, $compile, $filter) {
    decodeParams($stateParams);

    $scope.CurrentState = $state.current.name;
    $scope.createDatePicker = function () {
        createDatePicker();
    };
    $scope.BindDropDown = function (StateId) {
        $scope.CountyList = null;
        $scope.CourtList = null;
        $scope.DistrictList = null;
        $scope.DivisionList = null;
        var countylist = CommonServices.CountyDropdown(StateId);
        countylist.success(function (response) {
            $scope.CountyList = response.Data;
        });
        countylist.error(function (data, statusCode) { });

        var promise = CommonServices.CourtDropdown(StateId);
        promise.success(function (response) {
            $scope.CourtList = response.Data;

        });
        promise.error(function (data, statusCode) {
        });

        var districtlist = CommonServices.DistrictDropdown(StateId);
        districtlist.success(function (response) {
            $scope.DistrictList = response.Data;
        });
        districtlist.error(function (data, statusCode) { });
    }

    $scope.BindDropDownByDistrict = function (DistrictId, StateId) {

        $scope.DivisionList = null;

        var divisionlist = CommonServices.DivisionDropdown(DistrictId, StateId);
        divisionlist.success(function (response) {
            $scope.DivisionList = response.Data;
        });
        divisionlist.error(function (data, statusCode) { });

    }

    function bindDropdownList() {
        var state = CommonServices.StateDropdown();
        state.success(function (response) {
            $scope.StateList = response.Data;
            $scope.StateList.splice(0, 0, { SateID: "", StateName: "-- Select --" });
        });

    }
    function bindDropdownListState() {

        //county
        var countylist = CommonServices.CountyDropdown($scope.OrderStep4Obj.State);
        countylist.success(function (response) {
            $scope.CountyList = response.Data;
        });
        countylist.error(function (data, statusCode) { });

        //Court

        var promise = CommonServices.CourtDropdown($scope.OrderStep4Obj.State);
        promise.success(function (response) {
            $scope.CourtList = response.Data;
        });
        promise.error(function (data, statusCode) {

        });

    }
    function bindDropdownListFederals() {

        //District
        var districtlist = CommonServices.DistrictDropdown($scope.OrderStep4Obj.State);
        districtlist.success(function (response) {
            $scope.DistrictList = response.Data;
        });
        districtlist.error(function (data, statusCode) { });

    }
    //function bindDropdownListState() {

    //    //county
    //    var countylist = CommonServices.CountyDropdown('');
    //    countylist.success(function (response) {
    //        $scope.CountyList = response.Data;
    //    });
    //    countylist.error(function (data, statusCode) { });

    //    //Court

    //    var promise = CourtService.GetCourtList(0);
    //    promise.success(function (response) {
    //        $scope.CourtList = response.Data;

    //    });
    //    promise.error(function (data, statusCode) {
    //    });

    //}
    //function bindDropdownListFederals() {

    //    //District
    //    var districtlist = CommonServices.DistrictDropdown('');
    //    districtlist.success(function (response) {            
    //        $scope.DistrictList = response.Data;
    //    });
    //    districtlist.error(function (data, statusCode) { });

    //}

    $scope.GetOrderWizardStep4Details = function (OrderId) {
        var promise = Step4Service.GetOrderWizardStep4Details(OrderId);
        promise.success(function (response) {
            $scope.OrderStep4Obj = response.Data[0];

            if (!isNullOrUndefinedOrEmpty($scope.OrderStep4Obj.BillingDateOfLoss)) {
                $scope.OrderStep4Obj.BillingDateOfLoss = $filter('date')(new Date($scope.OrderStep4Obj.BillingDateOfLoss), $rootScope.GlobalDateFormat);
            }


            if (isNullOrUndefinedOrEmpty($scope.OrderStep4Obj.CaseTypeId)) {
                // $scope.OrderStep4Obj.TrialDate = $filter('date')(new Date($scope.OrderStep4Obj.TrialDate), $rootScope.GlobalDateFormat);
                $scope.OrderStep4Obj.CaseTypeId = 2;
            }

            $scope.BindDropDown($scope.OrderStep4Obj.State);
            $scope.BindDropDownByDistrict($scope.OrderStep4Obj.District, $scope.OrderStep4Obj.State);
            if (!isNullOrUndefinedOrEmpty($scope.OrderStep4Obj.TrialDate)) {
                $scope.OrderStep4Obj.TrialDate = $filter('date')(new Date($scope.OrderStep4Obj.TrialDate), $rootScope.GlobalDateFormat);
            }


        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.SubmitStep4 = function (form) {
        if (form.$valid) {

            if ($scope.OrderStep4Obj.IsStateOrFedral == 1) {
                $scope.OrderStep4Obj.District = null;
                $scope.OrderStep4Obj.Division = null;
            }
            if ($scope.OrderStep4Obj.IsStateOrFedral == 2) {
                $scope.OrderStep4Obj.County = null;
                $scope.OrderStep4Obj.Court = null;
            }
            if ($scope.OrderStep4Obj.CaseTypeId == 2) {
                $scope.OrderStep4Obj.District = null;
                $scope.OrderStep4Obj.Division = null;
                $scope.OrderStep4Obj.County = null;
                $scope.OrderStep4Obj.Court = null;
                $scope.OrderStep4Obj.Caption1 = null;
                $scope.OrderStep4Obj.VsText1 = null;
                $scope.OrderStep4Obj.Caption2 = null;
                $scope.OrderStep4Obj.VsText2 = null;
                $scope.OrderStep4Obj.Caption3 = null;
                $scope.OrderStep4Obj.TrialDate = null;
                $scope.OrderStep4Obj.VsText3 = null;
                $scope.OrderStep4Obj.CauseNo = null;
                $scope.OrderStep4Obj.TrialDate = null;
                $scope.OrderStep4Obj.State = null;
                //$scope.OrderStep4Obj.Rush = null;
                $scope.OrderStep4Obj.IsStateOrFedral = null;

            }
            $scope.OrderStep4Obj.EmpId = $scope.EmpId;
            $scope.OrderStep4Obj.UserAccessId = $scope.UserAccessId;
            $scope.OrderStep4Obj.OrderId = $scope.OrderId;

            if ($scope.OrderId > 0) {
                var promise = Step4Service.InsertOrUpdateOrderWizardStep4($scope.OrderStep4Obj);
                promise.success(function (response) {
                    if (response.Success) {
                        //$state.go("EditOrder", { OrderId: response.lng_InsertedId, Step: $rootScope.Enum.OrderWizardStep.Step5 });
                        $scope.OrderId = response.lng_InsertedId;
                        if ($scope.CurrentState == "OrderDetail" || $scope.CurrentState == "PartDetail") {
                            toastr.success("Case Information successfully saved.");
                            angular.element("#modal_Case").modal('hide');
                            $('.modal-backdrop').remove();
                            $scope.$parent.GetCaseInformation($scope.OrderId);

                        } else {
                            $scope.MoveNext();
                        }

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

    $scope.showHideSates = function () {
        bindDropdownListState();
        $scope.DistrictList = null;
        $scope.DistrictList = null;
    };

    $scope.showHideFederals = function () {
        bindDropdownListFederals();
        $scope.CountyList = null;
        $scope.CourtList = null;
    };
    function init() {
        $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
        $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
        $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
        $scope.OrderId = $stateParams.OrderId;
        $scope.OrderStep4Obj = new Object();
        $scope.currentStateId = '';
        bindDropdownList();
        $scope.OrderStep4Obj.CaseTypeId = 2;
        // bindDropdownListState();
        // bindDropdownListFederals();
        // $scope.OrderStep4Obj.IsStateOrFedral = 1;


        $scope.GetOrderWizardStep4Details($scope.OrderId);

    }

    init();

});