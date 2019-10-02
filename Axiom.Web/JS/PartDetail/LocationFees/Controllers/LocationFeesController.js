app.controller('LocationFeesController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, LocationFeesService, CommonServices, configurationService, $compile, $filter, LocationFeesService, FirmServices) {
    decodeParams($stateParams);
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.OrderId = $stateParams.OrderId;
    $scope.PartNo = $stateParams.PartNo;
    $scope.SelectChkList = [];
    //#region Event
    $scope.NewLocationFeeCreateClick = function () {
        $scope.objLocFeesModal = {};
        $scope.objLocFeesModal.PayToLocation = true;
        $scope.objLocFeesModal.IssueDate = $filter('date')(new Date(), $rootScope.GlobalDateFormat);
        $scope.OpenLocationFeeCreateUpdate();
    }
    $scope.OpenLocationFeeCreateUpdate = function () {
        setTimeout(function () { createDatePicker(); }, 1500);
        angular.element("#modal_LocationFees_CreateUpdate").modal('show');
    }
    $scope.OpenFirmModalClick = function () {
        $scope.ShowSearchFirm = true;
    }
    $scope.FillLocationFeesFirmDetail = function (FirmID) {
        var promise = FirmServices.GetFirmById(FirmID);
        promise.success(function (response) {
            $scope.objLocFeesModal.FirmID = response.Data[0].FirmID;
            $scope.objLocFeesModal.FirmName = response.Data[0].FirmName;
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.FilterBySelectedDate = function () {
        $scope.filterSelectedLocFeesList = [];
        var fromDate = $filter('date')($scope.LocFromDate, $rootScope.GlobalDateFormat);
        var toDate = $filter('date')($scope.LocToDate, $rootScope.GlobalDateFormat);
        angular.forEach($scope.LocationFeesList, function (item, key) {
            var entDate = $filter('date')(item.EntDate, $rootScope.GlobalDateFormat);
            if (new Date(entDate) >= new Date(fromDate) && new Date(entDate) <= new Date(toDate)) {
                $scope.filterSelectedLocFeesList.push({
                    ChkID: item.ChkID,
                    LocationName: item.LocationName,
                    LocAddress: item.LocAddress,
                    Memo: item.Memo,
                    IssueDate: item.IssueDate,
                    ChkNo: item.ChkNo,
                    Amount: item.Amount,
                    OrderNo: item.OrderNo
                });
            }

        });
        $scope.checkNumber = "";
        $scope.SelectChkList = [];
        angular.element("#modal_LocationFees_Select").modal('show');
    }
    $scope.EditLocationFees = function (chkId) {
        var objLocation = $filter('filter')($scope.LocationFeesList, { ChkID: chkId }, true);
        if (objLocation && objLocation.length > 0) {
            $scope.objLocFeesModal.ChkNo = objLocation[0].ChkNo;
            $scope.objLocFeesModal.FirmID = objLocation[0].FirmID;
            $scope.objLocFeesModal.FirmName = objLocation[0].LocationName;
            $scope.objLocFeesModal.Memo = objLocation[0].Memo;
            $scope.objLocFeesModal.Amount = objLocation[0].Amount;
            $scope.objLocFeesModal.IssueDate = $filter('date')(objLocation[0].IssueDate, $rootScope.GlobalDateFormat);
            $scope.objLocFeesModal.ChkID = objLocation[0].ChkID;
        }
        $scope.objLocFeesModal.PayToLocation = false;
        $scope.OpenLocationFeeCreateUpdate();
    }
    $scope.CloseLocationFeeModal = function () {
        $scope.objLocFeesModal.PayToLocation = true;
        angular.element("#modal_LocationFees_CreateUpdate").modal('hide');
    }
    $scope.SaveUpdateLocationFee = function () {
        $scope.objLocFeesModal.OrderNo = $scope.OrderId;
        $scope.objLocFeesModal.PartNo = $scope.PartNo;
        $scope.objLocFeesModal.ChngBy = $scope.EmpId;
        $scope.objLocFeesModal.ChngBy = $scope.EmpId;
        var promise = LocationFeesService.InsertUpdateLocationFeesChecks($scope.objLocFeesModal);
        promise.success(function (response) {
            if (response.Success) {
                toastr.success("Location Fees Saved Successfully");
                $scope.CloseLocationFeeModal();
                getLocationFeesList();
            }
        });
        promise.error(function (data, statusCode) {
            toastr.error(response.Message[0]);
        });
    }
    $scope.SelectedLocCheck = function (ChkStatus, ChkID) {
        if (ChkStatus)
            $scope.SelectChkList.push(ChkID);
        else {
            var index = $scope.SelectChkList.indexOf(ChkID);
            if (index > -1) {
                $scope.SelectChkList.splice(index, 1);
            }
        }
    }
    $scope.PrintCheckClick = function () {
        if (validateData()) {
            $scope.checkNumber = $scope.checkNumber ? $scope.checkNumber : "";
            LocationFeesService.GetPrintCheckIIFFiles($scope.LocFromDate, $scope.LocToDate, $scope.checkNumber, $scope.SelectChkList.toString());
        }
    }

    $scope.GenerateCheckClick = function () {
        if (validateData()) {
            $scope.checkNumber = $scope.checkNumber ? $scope.checkNumber : "";
            LocationFeesService.GetGenerateIIFFiles($scope.LocFromDate, $scope.LocToDate, $scope.checkNumber, $scope.SelectChkList.toString());
        }
    }

    $scope.DeleteLocationFee = function (ChkID) {
        bootbox.confirm({
            message: "Are you sure you want to delete this Fee?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                if (result) {
                    var promise = LocationFeesService.DeleteLocationFeesChecks(ChkID);
                    promise.success(function (response) {
                        if (response.Success) {
                            toastr.success("Location Fees Deleted Successfully");
                            getLocationFeesList();
                        }
                    });
                    promise.error(function (data, statusCode) {
                        toastr.error(response.Message[0]);
                    });
                }
                bootbox.hideAll();
            }
        });
    }
    $scope.UpdateLocationFeesVoidChecks = function (ChkID) {
        var promise = LocationFeesService.UpdateLocationFeesVoidChecks(ChkID);
        promise.success(function (response) {
            if (response.Success) {
                toastr.success("Location Fees Updated Successfully");
                getLocationFeesList();
            }
        });
        promise.error(function (data, statusCode) {
            toastr.error(response.Message[0]);
        });
    }
    //endregion

    //#region Method
    function validateData() {
        if (($scope.SelectChkList.length === 0) && (!$scope.checkNumber)) {
            toastr.error("Please select atleast one checkbox or enter checknumber");
            return false;
        } else if ($scope.checkNumber && isNaN($scope.checkNumber)) {
            toastr.error("Please Enter only numeric checknumber");
            return false;
        }
        return true;
    }
    function init() {
        $scope.LocationFeesList = [];
        $scope.objLocFeesModal = { PayToLocation: true };
        $scope.LocFromDate = $scope.LocToDate = $scope.CheckDate = $filter('date')(new Date(), $rootScope.GlobalDateFormat);
        getLocationFeesList();
        setTimeout(function () { createDatePicker(); }, 2000);
    }
    function getLocationFeesList() {
        var promise = LocationFeesService.GetLocationFees($scope.OrderId, $scope.PartNo);
        promise.success(function (response) {
            if (response.Success) {
                $scope.LocationFeesList = response.Data;
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    //#endregion

    init();

});