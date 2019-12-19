app.controller('OrderPartNoteController', function ($scope, $rootScope, $http, $state, $stateParams, notificationFactory, CommonServices, EmployeeServices, OrderNoteService, OrderPartService, OrderPartNoteService, configurationService, $compile, $filter) {
    decodeParams($stateParams);

    $scope.CurrentState = $state.current.name;

    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.RoleName = $rootScope.LoggedInUserDetail.RoleName ? $rootScope.LoggedInUserDetail.RoleName[0] : '';
    $scope.IsCallBack = false;
    $scope.$on('resetPartNotesPopupDetails', resetPartNotesDetailPopup);
    //#region Event


    $scope.PrintNote = function () {
        console.log($scope.PartNoteList);
        var printContents;
        printContents = "<div style='font-family: monospace;'>"
        printContents += "Notes for Order No : " + $scope.OrderId + "-" + $scope.PartNo;
        printContents += "</br></br>";
        angular.forEach($scope.PartNoteList, function (value, key) {
            debugger;
            printContents = printContents + value.Note + "</br>";
            printContents = printContents + "<b>" + $filter('date')(value.DtsInserted, "MMM dd yyyy hh:mm:ss a")  + " - " + value.FirstName + " " + value.LastName + "</b></br>";
            printContents = printContents + "</br>";
        });
        printContents = printContents + "</div>";
        var popupWin = window.open('', '_blank', 'width=600,height=500');
        popupWin.document.open();
        popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();
    };
    $scope.GetOrderPartNotes = function () {

        var promise = OrderNoteService.GetOrderNotes($scope.OrderId, $scope.PartNo);
        promise.success(function (response) {
            if (response.Data != null) {
                var result1 = [];
                for (var i = 0; i < response.Data.length; i++) {
                    if (response.Data[i].Note == "This Part Needs Chronology") {
                        result1.push(response.Data[i]);
                        response.Data.splice(i, 1);
                    }
                    if (response.Data[i] != null) {
                        if (response.Data[i].Note == "This Order has canvas request for below ZipCode") {
                            result1.push(response.Data[i]);
                            response.Data.splice(i, 1);
                        }
                    }
                }
                for (var i = 0; i < response.Data.length; i++) {
                    result1.push(response.Data[i]);
                }
            }
            if ($scope.RoleName === 'Attorney') {
                result1 = $filter('filter')(result1, { IsPublic: true }, true);
            }
            else if ($scope.RoleName === 'Administrator') {
                result1 = result1;//$filter('filter')(result1, { IsPublic: false }, true);
            }

            $scope.PartNoteList = result1;

        });
        promise.error(function (data, statusCode) {
        });
    };
    $scope.AddPartNotePopUp = function () {
        $scope.PartNoteObj = new Object();
        $scope.PartNoteform.$setPristine();
        angular.element('#modal_PartNote').modal('show');
        bindDropdown();
        $scope.GetPartDetail();
    };

    $scope.AddClientPartNotePopUp = function () {
        $scope.ClientPartNoteObj = new Object();
        $scope.ClientPartNoteform.$setPristine();
        angular.element('#modal_ClientPartNote').modal('show');
    };
    $scope.SaveClientPartNote = function (form) {
        if (form.$valid) {

            $scope.ClientPartNoteObj.OrderId = $scope.OrderId;
            $scope.ClientPartNoteObj.PartNo = $scope.PartNo;
            $scope.ClientPartNoteObj.UserId = $rootScope.LoggedInUserDetail.UserId;
            $scope.ClientPartNoteObj.RoleName = $scope.RoleName;
            var promise = OrderPartNoteService.InsertOrderPartNotesByClient($scope.ClientPartNoteObj);
            promise.success(function (response) {
                if (response.Success && response.InsertedId == 1) {
                    toastr.success('Client Note Save Successfully');
                    angular.element('#modal_ClientPartNote').modal('hide');
                    $scope.GetOrderPartNotes();
                    $scope.getInternalStatus();
                }
                else {
                    toastr.error(response.Message[0]);
                }

            });
            promise.error(function (data, statusCode) {
            });
        }
    };


    $scope.SavePartNote = function (form) {
        //if (isNullOrUndefinedOrEmpty($scope.PartNoteObj.NotesInternal) && isNullOrUndefinedOrEmpty($scope.PartNoteObj.NotesClient) && $scope.CurrentState != "SearchOrderList") {
        //    toastr.warning('Please enter Internal OR Client note.');
        //    return false;
        //}
        if (form.$valid || $scope.CurrentState == "SearchOrderList") {
            if (!isNullOrUndefinedOrEmpty($scope.PartNoteObj.InternalStatusId)) {
                var InternalStatusID = $scope.PartNoteObj.InternalStatusId;
                var InternalStatusText = $.grep($scope.InternalStatusList, function (status) {
                    return status.InternalStatusId == InternalStatusID;
                })[0].InternalStatus;
                $scope.PartNoteObj.InternalStatusText = InternalStatusText;
            }

            $scope.PartNoteObj.OrderId = $scope.OrderId;
            $scope.PartNoteObj.PartNo = $scope.PartNo;
            $scope.PartNoteObj.UserId = $rootScope.LoggedInUserDetail.UserId;
            $scope.PartNoteObj.RoleName = $scope.RoleName;
            $scope.PartNoteObj.AssgnTo = angular.copy($scope.PartNoteAssgnTo);
            if ($scope.CurrentState == "SearchOrderList") {
                $scope.PartNoteObj.PageFrom = "SearchOrderList";
                $scope.PartNoteObj.OrderIdPartIdList = $scope.$parent.SelectedOrdersWithPart;
            }
            $scope.RemoveCallBack();
            var promise = OrderPartNoteService.InsertOrderPartNotes($scope.PartNoteObj);
            promise.success(function (response) {
                if (response.Success && response.InsertedId == 1) {
                    toastr.success('Note Save Successfully');
                    if ($scope.CurrentState == "SearchOrderList") {
                        angular.element('#modal_OrderPartNote').modal('hide');
                    } else {
                        angular.element('#modal_PartNote').modal('hide');
                        $scope.GetOrderPartNotes();
                        $scope.getInternalStatus();
                    }


                }
                else {
                    toastr.error(response.Message[0]);
                }

            });
            promise.error(function (data, statusCode) {
            });
        }
    };

    $scope.RemoveCallBackFromNote = function (form) {
        if ($scope.CurrentState == "SearchOrderList") {
            $scope.PartNoteObj.OrderIdPartIdList = $scope.$parent.SelectedOrdersWithPart;
            $scope.PartNoteObj.UserId = $scope.UserAccessId;
            var promise = OrderPartNoteService.RemoveCallBackFromNote($scope.PartNoteObj);
            promise.success(function (response) {
                if (response.Success) {
                    toastr.success('Call Back Date set blank Successfully');
                    angular.element('#modal_OrderPartNote').modal('hide');
                }
                else {
                    toastr.error(response.Message[0]);
                }

            });
            promise.error(function (data, statusCode) {
            });
        }
    };
    function fn_ConvertDate(dt) {
        return !isNullOrUndefinedOrEmpty(dt) ? $filter('date')(new Date(dt), $rootScope.GlobalDateFormat) : dt;
    }
    $scope.GetPartDetail = function () {
        var promise = OrderPartService.GetPartListByOrderId($scope.OrderId, $scope.PartNo);
        promise.success(function (response) {
            debugger;
            if (response.Data != null && response.Data.length > 0) {
                $scope.PartNoteObj.CallBack = fn_ConvertDate(response.Data[0].CallBack);
                $scope.PartNoteObj.CanDate = fn_ConvertDate(response.Data[0].CanDate);
                $scope.PartNoteObj.DueDate = fn_ConvertDate(response.Data[0].DueDate);
                $scope.PartNoteObj.EntDate = fn_ConvertDate(response.Data[0].EntDate);
                $scope.PartNoteObj.FirstCall = fn_ConvertDate(response.Data[0].FirstCall);
                $scope.PartNoteObj.HoldDate = fn_ConvertDate(response.Data[0].HoldDate);
                $scope.PartNoteObj.NRDate = fn_ConvertDate(response.Data[0].NRDate);
                $scope.PartNoteObj.OrdDate = fn_ConvertDate(response.Data[0].OrdDate);
                $scope.PartNoteObj.RequestSendDate = fn_ConvertDate(response.Data[0].RequestSendDate);
                $scope.PartNoteObj.InternalStatusId = response.Data[0].InternalStatusId;
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.createDatePicker = function () {
        createDatePicker();
    };
    $scope.FillNote = function () {
        $scope.QuickNoteobj = $filter('filter')($scope.QuickNoteList, { PartStatusId: $scope.PartNoteObj.PartStatusId }, true);
        if (!isNullOrUndefinedOrEmpty($scope.QuickNoteobj)) {
            $scope.PartNoteObj.NotesInternal = $scope.QuickNoteobj[0].Note;
            $scope.PartNoteObj.NotesClient = $scope.QuickNoteobj[0].Note;
        }
    };

    $scope.RemoveCallBack = function () {
        if ($scope.IsCallBack) {
            $scope.PartNoteObj.CallBack = null;
        }
    }

    //#endregion

    //#region Method    
    function bindDropdown() {
        var _quickNote = CommonServices.GetQuickNotesDropDown();
        _quickNote.success(function (response) {
            $scope.QuickNoteList = response.Data;
        });
        _quickNote.error(function (data, statusCode) {
        });
        var _internalStatus = CommonServices.GetInternalStatusDropDown();
        _internalStatus.success(function (response) {
            $scope.InternalStatusList = response.Data;
            debugger;
            if (!isNullOrUndefinedOrEmpty($scope.InternalStatusList) && $scope.InternalStatusList.length > 0 && $scope.CurrentState != "SearchOrderList") {
                // $scope.PartNoteObj.InternalStatusId = $scope.InternalStatusList[0].InternalStatusId;
            }
        });
        _internalStatus.error(function (data, statusCode) {
        });
        //var _employee = EmployeeServices.GetEmployeeList();
        //_employee.success(function (response) {
        //    $scope.EmployeeList = angular.copy(response.Data);
        //    //if (!isNullOrUndefinedOrEmpty($scope.EmployeeList) && $scope.EmployeeList.length > 0) {
        //    //    $scope.PartNoteObj.AssgnTo = $scope.EmployeeList[2].EmpId;
        //    //}

        //    $scope.PartNoteObj.AssgnTo = angular.copy($scope.AsgnTo);


        //});
        //_employee.error(function (data, statusCode) {
        //});
    }

    function getEmployeeList() {
        var emp = CommonServices.AssignToDropDown($rootScope.CompanyNo);
        emp.success(function (response) {
            $scope.EmployeeList = angular.copy(response.Data);
            $scope.PartNoteAssgnTo = "";
        });
        emp.error(function (data, statusCode) {
        });
    }

    function resetPartNotesDetailPopup() {
        $scope.PartNoteObj = new Object();
        $scope.PartNoteform.$setPristine();
        $scope.PartNoteAssgnTo = "";
        $('form').trigger("reset");
    }
    $scope.PartNoteObj = new Object();
    function init() {
        if ($scope.CurrentState == "SearchOrderList") {
            bindDropdown();
            getEmployeeList();

        } else {
            $scope.OrderId = $stateParams.OrderId;
            $scope.PartNo = isNullOrUndefinedOrEmpty($stateParams.PartNo) ? 0 : $stateParams.PartNo;
            $scope.PartNoteObj = new Object();
            $scope.GetOrderPartNotes();
        }

    }
    //#endregion

    init();

});