app.controller('PartDetailMasterController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, OrderDetailMasterService, PartDetailMasterService, CommonServices, Step4Service, configurationService, $compile, $filter, EmployeeServices) {


    decodeParams($stateParams);
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;    
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.MasterLocId = "";
    $scope.RoleName = $rootScope.LoggedInUserDetail.RoleName ? $rootScope.LoggedInUserDetail.RoleName[0] : '';
    $scope.CurrentOrderStep = 1;
    $rootScope.pageTitle = $rootScope.pageTitle + ' ' + $stateParams.OrderId + '-' + $stateParams.PartNo;
    //$scope.internalStatusClass = "Films";
    //#region Event

    $scope.GetCompanyDetails = function (OrderId) { //Header Detail
        
        var promise = OrderDetailMasterService.GetOrderCompanyDetail(OrderId);
        promise.success(function (response) {
            if (response && response.Data.length > 0) {
                $scope.CompanyDetailobj = response.Data[0];
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
            if (response && response.Data.length > 0) {
                $scope.ClientInfoobj = response.Data[0];
            }
        });
        promise.error(function (data, statusCode) {
        });
    };
    $scope.OpenQuickForm = function (event) {
        $scope.ShowQuickForm = true;
    };
    function getNotesList(inputstr) {
        var span = document.createElement('span');
        var lines = inputstr.split('\n');
        var firstlineDisplay = false;
        for (var i = 0; i < lines.length; i++) {

            var isfound = lines[i].match(/:/g);
            var firstCharacter = lines[i].indexOf('[');

            if (isfound && isfound.length == 2 && firstCharacter === 0) {
                var timeFound = lines[i].search(' PM - ');
                if (timeFound == -1) {
                    timeFound = lines[i].search(' AM - ');
                }
                if (timeFound > -1) {

                    var lastCharcter = lines[i].lastIndexOf(']');
                    var strLastIndex = lines[i] ? lines[i].trim().length - 1 : -1;
                    if ((firstCharacter === 0) && (lastCharcter === strLastIndex)) {
                        var objData = lines[i].split("-");
                        if (objData && objData.length > 0) {
                            var empId = lines[i].split("-")[1].replace(']', '').trim();
                            var objuserDetail = $filter('filter')($scope.userDetailList, { 'EmpId': empId }, true);
                            if (objuserDetail && objuserDetail.length > 0) {
                                lines[i] = lines[i].replace(empId, objuserDetail[0].FirstName + ' ' + objuserDetail[0].LastName);
                            }
                        }

                        if (firstlineDisplay) {
                            lines[i] = "$line$".concat(lines[i]);
                        }
                        else {
                            lines[i] = "$firstline$".concat(lines[i]);
                        }
                    }
                    firstlineDisplay = true;
                }

            }
            span.innerText = lines[i];
            span.textContent = lines[i];
            lines[i] = span.innerHTML;
        }
        lines.join('<br />');

        $scope.notesList = angular.copy(lines);
    }
    $scope.openNotesClick = function () {
        var promise = CommonServices.GetLocationNotes($scope.MasterLocId);
        promise.success(function (response) {
            if (response.Success && response.Data.length > 0) {
                $scope.locationNotes = response.Data[0].Notes;
                getNotesList($scope.locationNotes);

            }

            angular.element("#modal_NotesForm").modal('show');

        });
        promise.error(function (data, statusCode) {
        });

    }
    //endregion
    //#region Edit Case
    $scope.EditCaseDetail = function () {
        angular.element("#modal_Case").modal('show');
    }
    //#endregion

    //#region Method
    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.PartNo = $stateParams.PartNo;
        if (isNullOrUndefinedOrEmpty($scope.OrderId)) {
            $state.transitionTo('OrderDetail', ({ 'OrderId': 0 }));
        }
        $scope.GetOrderDetailByOrderId($scope.OrderId);
        $scope.GetCompanyDetails($scope.OrderId);
        $scope.GetCaseInformation($scope.OrderId); //case Information
        $scope.GetClientInformation($scope.OrderId); //Client Information
        bindDropDownList();
        getUserDetails();
        $scope.getInternalStatus();
        getEmployeeList();

    }
    function getEmployeeList() {
        var emp = EmployeeServices.GetEmployeeList();
        emp.success(function (response) {
            $scope.EmployeeList = angular.copy(response.Data);
            $scope.PartNoteAssgnTo = angular.copy($scope.AsgnTo);
        });
        emp.error(function (data, statusCode) {
        });
    }
    function getUserDetails() {
        var promise = CommonServices.GetUserDetails();
        promise.success(function (response) {
            if (response.Success) {
                $scope.userDetailList = response.Data;
            }
        });
        promise.error(function (data, statusCode) {
        });
    }
    $scope.getInternalStatus = function () {
        var promise = OrderDetailMasterService.GetPartInternalStatus($scope.OrderId, $scope.PartNo);
        promise.success(function (response) {
            if (response.Success) {
                $scope.internalStatusClass = response.Data[0].IStatus;
                $scope.AsgnToName = response.Data[0].EmployeeName;
                $scope.AsgnTo = response.Data[0].AsgnTo;
                $scope.AcctRep = response.Data[0].AcctRep;
                $scope.AccountRepresentative = response.Data[0].AccountRepresentative;
            }
        });
        promise.error(function (data, statusCode) {
        });
    }
    function bindDropDownList() {
        var companydropdownlist = CommonServices.GetCompanyDropDown();
        companydropdownlist.success(function (response) {
            $scope.companydropdownlist = response.Data;
        });
        companydropdownlist.error(function (response) {
            toastr.error(response.Message[0]);
        });
    }
    //#endregion


    $scope.GetOrderDetailByOrderId = function (OrderId) {
        var promiseOrderDetail = OrderDetailMasterService.GetOrderDetailByOrderId(OrderId);
        promiseOrderDetail.success(function (response) {
            $scope.OrderBasicDetail = response.Data[0];
            if (!isNullOrUndefinedOrEmpty($scope.OrderBasicDetail)) {
                $scope.CurrentOrderStep = $scope.OrderBasicDetail.CurrentStepID;
                if ($scope.OrderBasicDetail.IsArchive) {
                    $scope.ArchiveButtonTitle = "Restore Order";
                } else {
                    $scope.ArchiveButtonTitle = "Archive Order";
                }
            }
        });
        promiseOrderDetail.error(function (data, statusCode) {
        });
    };


    init();

});