app.controller('ProposalFeesController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, ProposalFeesService, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;

    //#region Event

    $scope.GetProposalFees = function () {
        var promise = ProposalFeesService.GetProposalFees($scope.OrderId, $scope.PartNo);
        promise.success(function (response) {
            if (response) {
                $scope.ProposalFeesList = response.Data;
            }
        });
        promise.error(function (data, statusCode) {
        });
    };
    $scope.onRecordTypeDDChange = function (RecordTypeId) {
        if (RecordTypeId) {
            $scope.showRecordTypeValidate = false;
        } else {
            $scope.showRecordTypeValidate = true;
        }
    }
    $scope.NewProposalFeesPopUp = function () {

        $scope.ProposalFees = {
            OrderId: parseInt($scope.OrderId),
            PartNo: parseInt($scope.PartNo)
        };

        bindDropDown();
        $scope.OrderPartform.$setPristine();
        angular.element("#modal_Add_ProposalFees").modal('show');
    };

    function bindDropDown() {
        var _fileType = CommonServices.GetFileTypeDropdown();
        _fileType.success(function (response) {
            $scope.FileTypeDetailList = angular.copy(response.Data);
            $scope.TotalFee = "";
            $scope.DocCap = "";
            $scope.validateCal = $scope.showFileTypeValidate = $scope.showRecordTypeValidate = false;
            $scope.showPageNoValidate = true;
            $scope.ProposalFees.FileTypeId = 11;
            //setTimeout(function () {
            //    $scope.$apply(function () {
            //        $scope.ProposalFees.FileTypeId = 11;
            //    });
            //}, 400);
            //setTimeout(function () {
            //    $("#ddlSelectarrow").click();
            //    $("#ddlSelectarrow").click();
            //    //$scope.disbaledFileType = true;
            //}, 600);

        });
        _fileType.error(function (data, statusCode) {
        });
        var _RecType = CommonServices.RecordTypeDropDown();
        _RecType.success(function (response) {
            $scope.RecTypeDetailList = response.Data;
        });
        _RecType.error(function (data, statusCode) {
        });
    }
    function validateCalculate() {
        $scope.validateCal = false;
        if (!$scope.ProposalFees.FileTypeId) {
            $scope.validateCal = true;
            $scope.showFileTypeValidate = true;
        }
        if (!$scope.ProposalFees.RecordTypeId) {
            $scope.validateCal = true;
            $scope.showRecordTypeValidate = true;
        }
        if (!($scope.ProposalFees.PageNo) || !($scope.ProposalFees.PageNo > 0)) {
            $scope.validateCal = true;
            $scope.showPageNoValidate = true;
        }
        if ($scope.validateCal) {
            return false;
        }
        return true;
    }

    $scope.CalculationProposalFees = function () {
        if (validateCalculate()) {
            var promise = ProposalFeesService.GetProposalFeesCalculation($scope.OrderId, $scope.PartNo, $scope.ProposalFees.RecordTypeId, $scope.ProposalFees.PageNo);
            promise.success(function (response) {
                if (response.Data && response.Data.length > 0) {

                    $scope.TotalFee = angular.copy(response.Data[0].TotalFees.toFixed(2));
                    $scope.DocCap = angular.copy(response.Data[0].DocCap);

                }
            });
        }

    }
    $scope.SaveProposalFee = function () {
        var totalFee = parseFloat($scope.TotalFee);
        var docCap = parseFloat($scope.DocCap);
        if (totalFee > docCap) {
            bootbox.confirm({
                message: "This is over the Client's doc cap for records limit. Would you like to send Fee Approvals ?",
                buttons: {
                    cancel: {
                        label: 'No',
                        className: 'btn-danger'
                    },
                    confirm: {
                        label: 'Yes',
                        className: 'btn-success'
                    }
                },
                callback: function (result) {
                    if (result == true) {
                        var objSave = {
                            OrderNo: $scope.OrderId,
                            PartNo: $scope.PartNo,
                            Descr: $scope.ProposalFees.Description,
                            Pages: $scope.ProposalFees.PageNo,
                            Amount: totalFee,
                            EntBy: $scope.EmpId
                        };
                        var promise = ProposalFeesService.SaveProposalFees(objSave);
                        promise.success(function (response) {
                            if (response.Success) {
                                toastr.success('Verification Email Sent Successfully.');
                                angular.element("#modal_Add_ProposalFees").modal('hide');
                                $scope.GetProposalFees();
                            }
                            else {
                                toastr.error(response.Message[0]);
                            }

                        });
                    }
                    bootbox.hideAll();
                }
            });
        }
        else {
            var obj = {
                OrderNo: $scope.OrderId,
                PartNo: $scope.PartNo,
                Descr: $scope.ProposalFees.Description,
                Pages: $scope.ProposalFees.PageNo,
                Amount: totalFee,
                EntBy: $scope.EmpId
            };

            var promise = ProposalFeesService.VerifyProposalFees(obj);
            promise.success(function (response) {
                if (response.Success) {
                    toastr.success('Proposal Fees saved successfully.');
                    angular.element("#modal_Add_ProposalFees").modal('hide');
                    $scope.GetProposalFees();
                }
            });
        }


    };
    $scope.onFileTypeDDChange = function (FileTypeId) {

        if (FileTypeId) {
            $scope.validateFileType = false;
        } else {
            $scope.validateFileType = true;
        }
    }
    //endregion

    //#region Method
    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.PartNo = $stateParams.PartNo;
        if (isNullOrUndefinedOrEmpty($scope.OrderId)) {
            $state.transitionTo('PartDetail', ({ 'OrderId': 0, 'PartNo': 0 }));
        }
        $scope.GetProposalFees();
    }
    //#endregion

    init();

});