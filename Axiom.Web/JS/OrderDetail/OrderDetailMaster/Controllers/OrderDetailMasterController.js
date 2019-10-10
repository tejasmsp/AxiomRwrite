app.controller('OrderDetailMasterController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, OrderListService, OrderDetailMasterService, CommonServices, Step4Service, configurationService, $compile, $filter) {
    decodeParams($stateParams);
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.ArchiveButtonTitle = "Archive Order";
    $scope.OrderBasicDetail = new Object();
    $scope.CurrentOrderStep = 1;
    $rootScope.IsAttyUser = $rootScope.LoggedInUserDetail.RoleName.indexOf('Attorney') == -1 ? false : true; // $rootScope.LoggedInUserDetail.RoleName.includes("Attorney");
    //#region ---- QUICKFORM ----

    $rootScope.pageTitle = $rootScope.pageTitle + ' - ' + $stateParams.OrderId;
    $scope.OpenQuickForm = function (event) {
        $scope.ShowQuickForm = true;
    };
    $scope.objBillTo = new Object();
    //#endregion

    //#region Event

    // CHANGE BILLTO FIRM

    $scope.UpdateBillToAttorney = function (AttyID) {
        var promiseOrderDetail = OrderDetailMasterService.UpdateBillToAttorney($scope.OrderId, AttyID);
        promiseOrderDetail.success(function (response) {
            if (response && response.Success) {
                $scope.GetOrderDetailByOrderId($scope.OrderId);
                toastr.success("Billing Attorney updated successfully.");
                angular.element("#modal_BillToAttorney").modal('hide');
            }
        });
        promiseOrderDetail.error(function (data, statusCode) {

        });
    };


    $scope.ChangBillToFirm = function () {
        $scope.ShowSearchFirm = true;
    };
    $scope.ChangBillToAttorney = function () {
        
        var attorneyList = CommonServices.GetAttorneyByFirmID($scope.OrderBasicDetail.BillingFirmId);
        attorneyList.success(function (response) {
            debugger;
            $scope.lstAttorneyList = response.Data;
            if (response.Data.length > 0) {
                // $scope.objBillTo.AttyID = response.Data[0].AttyID;
            }
            $('.cls-firm1').selectpicker();
            $('.cls-firm1').selectpicker('refresh');
            
        });
        attorneyList.error(function (data, statusCode) { });
        angular.element("#modal_BillToAttorney").modal('show');
    };

    $scope.ChangBillToFirmSave = function (FirmID) {
        var promiseOrderDetail = OrderDetailMasterService.UpdateBillToFirm($scope.OrderId, FirmID);
        promiseOrderDetail.success(function (response) {
            if (response && response.Success) {
                $scope.GetOrderDetailByOrderId($scope.OrderId);
                toastr.success("Billing Firm updated successfully.");
            }
        });
        promiseOrderDetail.error(function (data, statusCode) {

        });

    };

    $scope.GetOrderDetailByOrderId = function (OrderId) {
        var promiseOrderDetail = OrderDetailMasterService.GetOrderDetailByOrderId(OrderId);
        promiseOrderDetail.success(function (response) {
            if (!isNullOrUndefinedOrEmpty(response.Data)) {
                debugger;
                $scope.OrderBasicDetail = response.Data[0];
                if (!isNullOrUndefinedOrEmpty($scope.OrderBasicDetail)) {
                    $scope.CurrentOrderStep = $scope.OrderBasicDetail.CurrentStepID;
                    if ($scope.OrderBasicDetail.IsArchive) {
                        $scope.ArchiveButtonTitle = "Restore Order";
                    } else {
                        $scope.ArchiveButtonTitle = "Archive Order";
                    }
                }
            }
        });
        promiseOrderDetail.error(function (data, statusCode) {
        });
    };

    $scope.GetCompanyDetails = function (OrderId) { //Header Detail
        var promise = OrderDetailMasterService.GetOrderCompanyDetail(OrderId);
        promise.success(function (response) {
            if (response.Data.length > 0) {
                $scope.CompanyDetailobj = response.Data[0];
                if ($scope.CompanyDetailobj.CompanyNo != $rootScope.CompanyNo) {
                    alert("item not found.")
                    $state.go("OrderList");
                }
            }
            else {
                alert("item not found.")
                $state.go("OrderList");
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.GetCaseInformation = function (OrderId) { //Case Information
        var promise = Step4Service.GetOrderWizardStep4Details(OrderId);
        promise.success(function (response) {
            if (response) {
                $scope.CaseInfoobj = response.Data[0];
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.GetClientInformation = function (OrderId) { //Client Information
        var promise = OrderDetailMasterService.GetClientInformation(OrderId);
        promise.success(function (response) {
            if (response && response.Data) {
                $scope.ClientInfoobj = response.Data[0];

                //setTimeout(function () {
                //    var ObjselectedCmpny = $filter('filter')($scope.companydropdownlist, { 'CompNo': $scope.ClientInfoobj.CompanyNo }, true);
                //    if (ObjselectedCmpny && ObjselectedCmpny.length > 0) {
                //        $("#ddlCmpny").val(ObjselectedCmpny[0].CompName);
                //    }
                //}, 100);

            }
        });
        promise.error(function (data, statusCode) {
        });
    };
    //endregion

    //#region Method
    function init() {
        $scope.OrderId = $stateParams.OrderId;
        if (isNullOrUndefinedOrEmpty($scope.OrderId)) {
            $state.transitionTo('OrderDetail', ({ 'OrderId': 0 }));
        }
        $scope.GetOrderDetailByOrderId($scope.OrderId);
        $scope.GetCompanyDetails($scope.OrderId);
        $scope.GetCaseInformation($scope.OrderId); //case Information
        $scope.GetClientInformation($scope.OrderId); //Client Information

        bindDropDownList();
    }

    function bindDropDownList() {
        //var companydropdownlist = CommonServices.GetCompanyDropDown();
        //companydropdownlist.success(function (response) {
        //    $scope.companydropdownlist = response.Data;

        //});
        //companydropdownlist.error(function (response) {
        //    toastr.error(response.Message[0]);
        //});
    };
    //#endregion

    //#region Edit Case
    $scope.EditCaseDetail = function () {
        angular.element("#modal_Case").modal('show');
    }
    //#endregion

    //#regionArchiveOrder

    $scope.EditOrderInformation = function () {

        var promise = OrderDetailMasterService.GetOrderInformation($scope.OrderId);
        promise.success(function (response) {

            if (response && response.Data) {
                $scope.objEditOrderInfo = response.Data[0];

                var comp = CommonServices.GetCompanyDropDown($scope.OrderId);
                comp.success(function (response) {
                    debugger;
                    $scope.companydropdownlist = response.Data;
                });
                comp.error(function (data, statusCode) {

                });
                angular.element("#modalEditOrderInformation").modal('show');
            }
        });
        promise.error(function (data, statusCode) {

        });
    };

    $scope.UpdateOrderBasicInformation = function (form) {
        if (form.$valid) {
            modalEditOrderInformation
            var promise = OrderDetailMasterService.UpdateBasicOrderIformation($scope.objEditOrderInfo);
            promise.success(function (response) {

                if (response && response.Success) {
                    angular.element("#modalEditOrderInformation").modal('hide');
                    toastr.success("Order Information updated successfully.");
                    $scope.GetCompanyDetails($scope.OrderId);
                    $scope.GetClientInformation($scope.OrderId);
                }
            });
            promise.error(function (data, statusCode) {

            });
        }
    };

    $scope.ArchiveOrder = function () {
        var row = $scope.OrderBasicDetail;
        bootbox.confirm({
            message: "Are you sure you want to " + (row.IsArchive ? "restore" : "archive") + " this order?",
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
                    var promise = OrderListService.ArchiveOrder({ EmpId: $rootScope.LoggedInUserDetail.EmpId, UserAccessId: $rootScope.LoggedInUserDetail.UserAccessId, OrderId: $scope.OrderId });
                    promise.success(function (response) {
                        if (response.Success) {
                            toastr.success("The order has been " + (row.IsArchive ? "restored" : "archived") + " successfully.");
                            $scope.GetOrderDetailByOrderId($scope.OrderId);
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function (data, statusCode) {
                    });
                }
                bootbox.hideAll();
            }
        });

    };

    //#endregion

    init();

});