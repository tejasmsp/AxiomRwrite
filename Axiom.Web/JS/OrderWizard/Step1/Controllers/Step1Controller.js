app.controller('Step1Controller', function ($scope, $rootScope, $state, $stateParams, notificationFactory, Step1Service, CommonServices, configurationService, $compile, $filter) {
    decodeParams($stateParams);
    $scope.PageMode = $stateParams.PageMode;
    $scope.isEdit = false;
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.RoleName = $rootScope.LoggedInUserDetail.RoleName ? $rootScope.LoggedInUserDetail.RoleName[0] : '';
    $scope.onFirmChange = function () {
        BindAttorneyByFirmDropdown($scope.OrderStep1Obj.OrderingFirmID);
    };

    //#region Event

    $scope.DownloadForm = function () {
        angular.element("#modal_DownloadForms").modal('show');
    };

    $scope.GetAttorneyForCode = function () {
        var promiseGetAttorneyForCode = Step1Service.GetAttorneyForCode();
        promiseGetAttorneyForCode.success(function (response) {
            $scope.AttorneyForCodeList = response.Data;
        });
        promiseGetAttorneyForCode.error(function (data, statusCode) {
        });
    };


    function BindAttorneyByFirmDropdown(FirmID) {
        debugger;
        if (FirmID) {
            if (FirmID == undefined) {
                FirmID = "";
            }

            var attry = CommonServices.GetAttorneyByFirmIDForclientAndAdmin(FirmID, $scope.userGuid, false, $rootScope.CompanyNo);
            attry.success(function (response) {
                $scope.OrderingAttorneyList = response.Data;
                setTimeout(function () {
                    $('.cls-attorney').selectpicker('refresh');
                    $('.cls-attorney').selectpicker();
                }, 500);
            });

        }
        else {
            if ($scope.RoleName == 'Administrator') {
                var attry = CommonServices.GetAttorneyByFirmIDForclientAndAdmin(FirmID, $scope.userGuid, true, $rootScope.CompanyNo);
                attry.success(function (response) {
                    $scope.OrderingAttorneyList = response.Data;
                    setTimeout(function () {
                        $('.cls-attorney').selectpicker('refresh');
                        $('.cls-attorney').selectpicker();
                    }, 500);
                });
            }
        }

    };


    $scope.createDatePicker = function () {
        createDatePicker();
    };

    $scope.GetNotificationList = function (selectedAttrney) {
        var promise = Step1Service.GetNotificationList(selectedAttrney, $scope.OrderId);
        promise.success(function (response) {
            $scope.NotificationList = response.Data;
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.RefreshNotificationList = function () {
        $scope.GetNotificationList($scope.OrderStep1Obj.OrderingAttorney);
        if ($scope.OrderStep1Obj.OrderingAttorney) {
            var obj = $filter('filter')($scope.OrderingAttorneyList, { 'AttyID': $scope.OrderStep1Obj.OrderingAttorney })
            $scope.OrderStep1Obj.OrderingFirmID = obj[0].FirmID;
            $('.cls-firm').selectpicker('refresh');
            $('.cls-firm').selectpicker();
            var obj1 = $filter('filter')($scope.OrderFirmList, { 'FirmID': $scope.OrderStep1Obj.OrderingFirmID })
            $('.cls-firm .filter-option-inner-inner').text(obj1[0].FirmName);
        }
    };

    $scope.GetOrderWizardStep1Details = function (OrderId) {
        if (OrderId > 0) {
            $scope.isEdit = true;
            var promise = Step1Service.GetOrderWizardStep1Details(OrderId);
            promise.success(function (response) {
                if (response && response.Data.length > 0) {
                    BindAttorneyByFirmDropdown(response.Data[0].OrderingFirmID);
                    $scope.OrderStep1Obj = response.Data[0];
                    $scope.OrderStep1Obj.AttorneyFor = parseInt(response.Data[0].AttorneyFor);
                    $scope.selectedAttrney = $scope.OrderStep1Obj.OrderingAttorney;
                    $('.cls-firm option').attr('selected', false)[0];
                    $('.cls-firm').selectpicker('refresh');
                    $('.cls-attorney option').attr('selected', false)[0];
                    $('.cls-attorney').selectpicker('refresh');
                    $scope.GetNotificationList($scope.selectedAttrney);
                }

            });
            promise.error(function (data, statusCode) {
            });
        }
    };

    $scope.SubmitStep1 = function (form) {
        if (form.$valid) {

            $scope.OrderStep1Obj.EmpId = $scope.EmpId;
            $scope.OrderStep1Obj.UserAccessId = $scope.UserAccessId;
            $scope.OrderStep1Obj.IsFromClient = $scope.IsFromClient;
            $scope.OrderStep1Obj.NotificationEmail = $scope.NotificationList;
            $scope.OrderStep1Obj.CompanyNo = $rootScope.CompanyNo;
            if ($scope.OrderId > 0) {
                $scope.OrderStep1Obj.OrderId = $scope.OrderId;
                var promise = Step1Service.UpdateOrderWizardStep1($scope.OrderStep1Obj);
                promise.success(function (response) {
                    if (response.Success) {

                        //$state.go("EditOrder", { OrderId: response.lng_InsertedId, Step: $rootScope.Enum.OrderWizardStep.Step2 });
                        $scope.OrderId = response.lng_InsertedId;
                        $scope.MoveNext();
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            } else {

                $scope.OrderStep1Obj.OrderId = 0;
                var promiseOrderWizardStep1 = Step1Service.InsertOrderWizardStep1($scope.OrderStep1Obj);
                promiseOrderWizardStep1.success(function (response) {
                    if (response.Success) {
                        //$state.go("EditOrder", { OrderId: response.lng_InsertedId, Step: $rootScope.Enum.OrderWizardStep.Step2 });
                        $scope.OrderId = response.lng_InsertedId;
                        $scope.MoveNext();
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }

                });
                promiseOrderWizardStep1.error(function (data, statusCode) {
                });
            }
        }
    };

    $scope.SubmitOrder = function () {
        debugger;
        $scope.OrderStep1Obj.EmpId = $scope.EmpId;
        $scope.OrderStep1Obj.UserAccessId = $scope.UserAccessId;
        $scope.OrderStep1Obj.IsFromClient = $scope.IsFromClient;
        //$scope.OrderStep1Obj.NotificationEmail = $scope.NotificationList;
        if ($scope.OrderId > 0) {
            $scope.OrderStep1Obj.OrderId = $scope.OrderId;
            $scope.OrderStep1Obj.UserEmail = $rootScope.LoggedInUserDetail.UserName;
            $scope.OrderStep1Obj.LocDocumentList = $scope.LocDocumentList;
            $scope.OrderStep1Obj.CompanyNo = $rootScope.CompanyNo;

            var promise = Step1Service.SubmitOrder($scope.OrderStep1Obj);
            promise.success(function (response) {
                if (response.Success) {
                    // alert("TODO:Need to remove rights for this order to be edited.");
                    $state.go("OrderList");
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) {
            });
        };
        //bootbox.confirm({
        //    message: "Are you sure you want to sumbit this order?",
        //    buttons: {
        //        cancel: {
        //            label: 'No',
        //            className: 'btn-danger'
        //        },
        //        confirm: {
        //            label: 'Yes',
        //            className: 'btn-success'
        //        }
        //    },
        //    callback: function (result) {
        //        if (result == true) {

        //        }
        //        bootbox.hideAll();
        //    }
        //});
    };

    $scope.Back = function () {
        $state.go("EditOrder", { OrderId: $scope.OrderId, Step: (parseInt($scope.CurrentStep) - 1), PageMode: $scope.PageMode });
    };
    $scope.MoveNext = function () {
        $state.go("EditOrder", { OrderId: $scope.OrderId, Step: (parseInt($scope.CurrentStep) + 1), PageMode: $scope.PageMode });
    };
    $scope.MakeCurrentStepActive = function (IsBack) {
        $scope.CurrentStep = $stateParams.Step;
        if (IsBack) {
            $scope.CurrentStep = $scope.CurrentStep - 1;
        }
        $(".stepy-step").hide();
        $("[title=" + $scope.CurrentStep + "]").show();
        $(".stepy-header *[id*=head]").removeClass("stepy-active");
        $(".stepy-header li:nth-child(" + $scope.CurrentStep + ")").addClass("stepy-active");
    };

    $scope.InitMethodsForStep1 = function () {
        $scope.BindOrderingFirmDropDown();
        BindAttorneyByFirmDropdown($scope.OrderStep1Obj.OrderingFirmID);
        $scope.chkOrderConfirmation = false;
        $scope.GetAttorneyForCode();
        $scope.GetOrderWizardStep1Details($scope.OrderId);
    };

    $scope.fnChangeCol = function (val) {
        for (var i = 0; i < $scope.NotificationList.length; i++) {
            switch (val) {
                case 1:
                    $scope.NotificationList[i].OrderConfirmation = $('[name="OrderConfirmation"]').is(":checked");
                    break;
                case 2:
                    $scope.NotificationList[i].FeeApproval = $('[name="FeeApproval"]').is(":checked");
                    break;
                case 3:
                    $scope.NotificationList[i].AuthNotice = $('[name="AuthNotice"]').is(":checked");
                    break;
                case 4:
                    $scope.NotificationList[i].NewRecordAvailable = $('[name="NewRecordAvailable"]').is(":checked");
                    break;
            }
        }

    };

    $scope.fnChangeRow = function (index) {
        $scope.NotificationList[index].OrderConfirmation = $('#chkrow_' + index).is(":checked");
        $scope.NotificationList[index].FeeApproval = $('#chkrow_' + index).is(":checked");
        $scope.NotificationList[index].AuthNotice = $('#chkrow_' + index).is(":checked");
        $scope.NotificationList[index].NewRecordAvailable = $('#chkrow_' + index).is(":checked");
    };

    //#endregion

    //#region Method

    //$scope.BindOrderingFirmDropDown = function () {
    //    //var _firm = CommonServices.GetFirmByUserId($rootScope.LoggedInUserDetail.UserId);
    //    //_firm.success(function (response) {
    //    //    $scope.FirmList = angular.copy(response.Data);
    //    //    if (!isNullOrUndefinedOrEmpty($scope.FirmList) && $scope.FirmList.length >= 1 && $scope.OrderId == 0) {
    //    //        $scope.OrderStep1Obj.OrderingFirmID = $scope.FirmList[0].FirmID;
    //    //        $scope.onFirmChange();
    //    //    }
    //    //});
    //    //_firm.error(function (data, statusCode) {
    //    //});

    //    //BindAttorneyByFirmDropdown($scope.OrderStep1Obj.OrderingFirmID);
    //};

    $scope.BindOrderingFirmDropDown = function () {
        var _firm = CommonServices.GetFirmByUserId($rootScope.LoggedInUserDetail.UserId, $rootScope.CompanyNo);
        _firm.success(function (response) {
            $rootScope.OrderFirmList = angular.copy(response.Data);
            setTimeout(function () {
                $('.cls-firm').selectpicker('refresh');
                $('.cls-firm').selectpicker();
            }, 500);

        });
        _firm.error(function (data, statusCode) {
        });
    };


    $('#Wizard-head-0').click(function () {
        $scope.ChangeWizardStepByStepNumber(1);
    });
    $('#Wizard-head-1').click(function () {
        $scope.ChangeWizardStepByStepNumber(2);
    });

    $('#Wizard-head-2').click(function () {
        $scope.ChangeWizardStepByStepNumber(3);
    });

    $('#Wizard-head-3').click(function () {
        $scope.ChangeWizardStepByStepNumber(4);
    });

    $('#Wizard-head-4').click(function () {
        $scope.ChangeWizardStepByStepNumber(5);
    });

    $('#Wizard-head-5').click(function () {
        $scope.ChangeWizardStepByStepNumber(6);
    });

    $('#Wizard-head-6').click(function () {
        $scope.ChangeWizardStepByStepNumber(7);
    });

    $('#Wizard-head-7').click(function () {
        $scope.ChangeWizardStepByStepNumber(8);
    });

    $scope.ChangeWizardStepByStepNumber = function (GotoStepNumber) {
        if (GotoStepNumber > 0 && GotoStepNumber < $scope.CurrentStep) {
            $state.go("EditOrder", { OrderId: $scope.OrderId, Step: GotoStepNumber, PageMode: $scope.PageMode });
        }
    };


    function init() {
        $scope.CurrentStep = $stateParams.Step;
        $scope.OrderId = $stateParams.OrderId;
        $scope.MakeCurrentStepActive();
        if (isNullOrUndefinedOrEmpty($scope.OrderId)) {
            $scope.OrderId = 0;
        }
        $scope.OrderStep1Obj = new Object();
        $scope.chkOrderConfirmation = false;
        //$scope.onFirmChange();

        //setTimeout(function () {
        //    debugger;
        //    if (!isNullOrUndefinedOrEmpty($rootScope.OrderFirmList) && $rootScope.OrderFirmList.length >= 1 && $scope.OrderId == 0) {
        //        $scope.OrderStep1Obj.OrderingFirmID = angular.copy($rootScope.OrderFirmList[0].FirmID);

        //    }
        //    $scope.onFirmChange();

        //}, 500);
        //setTimeout(function () {
        //    $("#ddlSelectarrow").click();
        //    $("#ddlSelectarrow").click();
        //}, 600);
        ////-----

        //setTimeout(function () {
        //    $("fieldset").removeAttr("title");
        //}, 200);
    }

    //#endregion

    init();

});