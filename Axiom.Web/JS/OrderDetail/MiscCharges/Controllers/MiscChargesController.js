app.controller('MiscChargesController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, MiscChargesService, configurationService, $compile, $filter) {
    decodeParams($stateParams);
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEdit = false;
    //#region Event

    $scope.GetMiscChargesByOrderId = function () {
        var promise = MiscChargesService.GetMiscChargesByOrderId($scope.OrderId, $scope.PartNo);
        promise.success(function (response) {
            $scope.MiscChargesList = response.Data;
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.GetPartListByOrderId = function () {
        var promise = MiscChargesService.GetPartListByOrderId($scope.OrderId);
        promise.success(function (response) {
            $scope.OrderPartList = response.Data;
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.AddMiscCharges = function () { //Add Mode
        $scope.MiscChargesObj = new Object();
        $scope.IsEdit = false;
        $scope.MiscChargesObj.MiscChrgId = 0;
        $scope.MiscChargesform.$setPristine();
        if ($scope.PartNo == 0) {
            $scope.MiscChargesObj.PartNoStr = "";
            $scope.GetPartListByOrderId();
            angular.element("#modal_OrderPart").modal('show');
        }
        else {
            BindDropDown();
            $scope.MiscChargesObj.PartNoStr = "" + $scope.PartNo;            
            $scope.MiscChargesObj.Units = 1;
            angular.element("#modal_MiscCharges").modal('show');
        }
        
    };

    $scope.EditMiscCharges = function (MiscChrgId) { //Edit Mode
        $scope.IsEdit = true;
        $scope.MiscChargesObj = new Object();
        BindDropDown();
        var promise = MiscChargesService.GetMiscChargesByMiscChrgId(MiscChrgId);
        promise.success(function (response) {
            if (response.Success) {
                $scope.MiscChargesObj = response.Data[0];
                $scope.strRegFee = $filter('filter')($scope.MiscChrgsDropDownList, { Descr: $scope.MiscChargesObj.Descr }, true);
                angular.element("#modal_MiscCharges").modal('show');
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.SelectOrderPart = function () {
        for (var i = 0; i < $scope.OrderPartList.length; i++) {
            if ($scope.OrderPartList[i].selected) {
                $scope.MiscChargesObj.PartNoStr += $scope.OrderPartList[i].PartNo + ",";
            }
        }
        if (!isNullOrUndefinedOrEmpty($scope.MiscChargesObj.PartNoStr)) {
            $scope.MiscChargesObj.PartNoStr = $scope.MiscChargesObj.PartNoStr.substr(0, $scope.MiscChargesObj.PartNoStr.length - 1);
        }
        BindDropDown();
        $scope.MiscChargesObj.Units = 1;
        angular.element("#modal_MiscCharges").modal('show');
    };

    $scope.SetMiscChargeDropDown = function () {
        $scope.strRegFee = $filter('filter')($scope.MiscChrgsDropDownList, { Descr: $scope.MiscChargesObj.Descr }, true);
        if (!isNullOrUndefinedOrEmpty($scope.strRegFee)) {
            $scope.MiscChargesObj.RegFee = $scope.strRegFee[0].RegFee;
            $scope.MiscChargesObj.Descr = $scope.strRegFee[0].Descr;
            $scope.SetMiscCharge();
        }
        else {
            $scope.MiscChargesObj.RegFee = 0;
        }

    };

    $scope.SetMiscCharge = function () {
        $scope.MiscChargesObj.Amount = $scope.MiscChargesObj.Units * $scope.MiscChargesObj.RegFee;
    };

    $scope.SaveMiscCharges = function (form) { // ADD-EDIT
        if (form.$valid) {
            $scope.MiscChargesObj.OrderId = $scope.OrderId;
            $scope.MiscChargesObj.EmpId = $rootScope.LoggedInUserDetail.EmpId;

            var promise = MiscChargesService.SaveMiscCharges($scope.MiscChargesObj);
            promise.success(function (response) {
                if (response.Success) {
                    if (response.InsertedId == "1") {
                        toastr.success("Record Save Successfully");
                        angular.element("#modal_MiscCharges").modal('hide');
                        angular.element("#modal_OrderPart").modal('hide');
                        $scope.GetMiscChargesByOrderId();
                    }
                }
            });
            promise.error(function (data, statusCode) {
            });
        }
    };
    $scope.DeleteMiscChargesByMiscChrgId = function (MiscChrgId) { // DELETE 
        var promise = MiscChargesService.DeleteMiscChargesByMiscChrgId(MiscChrgId);
            promise.success(function (response) {
                if (response.Success) {
                    if (response.InsertedId == "1") {
                        toastr.success("Record Deleted Successfully");
                        $scope.GetMiscChargesByOrderId();
                    }
                }
            });
            promise.error(function (data, statusCode) {
            });
       
    };

    //#endregion

    //#region Method    

    function BindDropDown() {
        var attorney = MiscChargesService.GetMiscChargeAttorneyDropDown($scope.OrderId);
        attorney.success(function (response) {
            $scope.MiscChargesAttorneyList = response.Data;
        });
        attorney.error(function (data, statusCode) {
        });
        var _fee = MiscChargesService.GetMiscChrgsDropDown();
        _fee.success(function (response) {
            $scope.MiscChrgsDropDownList = response.Data;



        });
        _fee.error(function (data, statusCode) {
        });
    }

    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.PartNo = isNullOrUndefinedOrEmpty($stateParams.PartNo) ? 0 : $stateParams.PartNo;
        $scope.GetMiscChargesByOrderId();
    }

    //#endregion

    init();

});