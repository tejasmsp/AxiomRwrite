app.controller('Step2Controller', function ($scope, $rootScope, $state, $stateParams, notificationFactory, CommonServices, Step2Service, configurationService, $compile, $filter) {



    decodeParams($stateParams);

    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.validateClaimNo = false;
    $scope.options = [
        { value: true, label: 'Yes' },
        { value: false, label: 'No' }
    ];

    //#region Event
    $scope.BillFirmChangeEvent = function () {
        if ($scope.OrderStep2Obj.BillingFirmId === "SHOWMORE") {
            $scope.OrderStep2Obj.BillingFirmId = "";
            angular.element("#modal_MoreBillSearchFirm").modal('show');
            $scope.SelectFirmForBillingStep2(false);
        }
        else {
            GetSelectedFirmDetail();
            $scope.BindAttorneyByFirmDropdown();
            var selectedFirm = $filter('filter')($scope.Firmlist, { FirmID: $scope.OrderStep2Obj.BillingFirmId }, true);
            if (selectedFirm && selectedFirm.length > 0 && selectedFirm[0].FirmType === "OtherFirm") {
                $scope.validateClaimNo = true;
            }
            else
                $scope.validateClaimNo = false;
        }
    }

    $scope.BindAttorneyByFirmDropdown = function () {

        var isShowMore = false;
        $scope.firmobj = $filter('filter')($scope.Firmlist, { FirmID: $scope.OrderStep2Obj.BillingFirmId }, true);
        if (!isNullOrUndefinedOrEmpty($scope.firmobj) && $scope.firmobj.length == 1) {
            $scope.firmobj = $scope.firmobj[0];
        }
        else {
            isShowMore = true;
        }
        var attry = CommonServices.GetAttorneyByFirmIDForclient($scope.OrderStep2Obj.BillingFirmId, $scope.userGuid, isShowMore);
        attry.success(function (response) {
            $scope.AttorneyListByFirm = response.Data;
            if ($scope.OrderStep2Obj.BillingAttorneyId) {
                $scope.OrderStep2Obj.BillingAttorneyId = $filter('filter')($scope.AttorneyListByFirm, { AttyID: $scope.OrderStep2Obj.BillingAttorneyId }, true)[0];
            }
        });
    };

    function checkFirmType(firmId) {
        var promise = Step2Service.SelectFirmForBillingStep2($scope.OrderStep2Obj);
        promise.success(function (response) {
            if (response.Success) {
                $scope.searchFirmList = angular.copy(response.Data);
                var obj = $filter('filter')($scope.searchFirmList, { FirmID: firmId }, true);
                $scope.validateClaimNo = (obj && obj.length > 0) ? true : false;
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.GetOrderWizardStep2Details = function (OrderId) {
        var promise = Step2Service.GetOrderWizardStep2Details(OrderId);
        promise.success(function (response) {

            $scope.OrderStep2Obj = response.Data[0];
            checkFirmType($scope.OrderStep2Obj.BillingFirmId);
            if (isNullOrUndefinedOrEmpty($scope.OrderStep2Obj.BillToOrderingFirm)) {
                $scope.OrderStep2Obj.BillToOrderingFirm = true;
            }
            if (!isNullOrUndefinedOrEmpty($scope.OrderStep2Obj.BillingFirmId)) {
                $scope.BindAttorneyByFirmDropdown();
            }
            GetSelectedFirmDetail();
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.SubmitStep2 = function (form) {

        //if ($scope.OrderStep2Obj.BillToOrderingFirm == false) {
        if (isNullOrUndefinedOrEmpty($scope.OrderStep2Obj.BillingAttorneyId)) {
            toastr.error('Attorney is required');
            return;
        }
        //}

        if ($scope.validateClaimNo && !$scope.OrderStep2Obj.BillingClaimNo) {
            toastr.error('Billing Claim No is required');
            return;
        }
        form.$submitted = true;


        if (form.$valid) {
            $scope.OrderStep2Obj.EmpId = $scope.EmpId;
            $scope.OrderStep2Obj.UserAccessId = $scope.UserAccessId;
            $scope.OrderStep2Obj.OrderId = $scope.OrderId;
            if ($scope.OrderId > 0) {
                if (angular.isObject($scope.OrderStep2Obj.BillingAttorneyId)) {
                    $scope.OrderStep2Obj.BillingAttorneyId = angular.copy($scope.OrderStep2Obj.BillingAttorneyId.AttyID);
                }
                var promise = Step2Service.InsertOrUpdateOrderWizardStep2($scope.OrderStep2Obj);
                promise.success(function (response) {
                    if (response.Success) {
                        //$state.go("EditOrder", { OrderId: response.lng_InsertedId, Step: $rootScope.Enum.OrderWizardStep.Step3 });
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

    $scope.bindFirmForDropdown = function () {
        var Firm = CommonServices.GetAssociatedFirm($rootScope.LoggedInUserDetail.UserId, $scope.OrderId);
        Firm.success(function (response) {
            $scope.Firmlist = response.Data;
            $scope.Firmlist.push({ CreatedBy: null, FirmID: "SHOWMORE", FirmName: "SHOW MORE", MapID: 0, RecTypeID: 0, RecTypeName: null, Scope: null, ScopeTitle: null })
            $scope.firmobj = new Object();
            $scope.GetOrderWizardStep2Details($scope.OrderId);
        });
    };

    $scope.ChangeBillToOrderingFirm = function () {
        if ($scope.OrderStep2Obj.BillToOrderingFirm) {
            $scope.firmobj = new Object();
            $scope.OrderStep2Obj.BillingAttorneyId = '';
            $scope.OrderStep2Obj.BillingFirmId = '';

        }
    };
    //#endregion

    //#region Method
    function GetSelectedFirmDetail() {
        var objFirm = $filter('filter')($scope.Firmlist, { FirmID: $scope.OrderStep2Obj.BillingFirmId }, true);
        if (objFirm && objFirm.length > 0) {
            var selectedFirmData = objFirm[0];
            $scope.SelectedFirm = selectedFirmData.FirmName;
            $scope.billingFirmAddress = selectedFirmData.Address + "<br><b>" + selectedFirmData.City + "</b> <b>" + selectedFirmData.State + "</b> <b>" + selectedFirmData.Zip + "</b><br>Phone No: " + selectedFirmData.PhoneNo +
                "<br>Fax No: " + selectedFirmData.FaxNo;
        }
        else { //Search
            $scope.SelectFirmForBillingStep2(true);

        }
    };


    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.OrderStep2Obj = new Object();
        // bindStateForDropdown();
        $scope.bindFirmForDropdown();
    };

    $scope.SelectFirmForBillingStep2 = function (isFromLoad) {
        var promise = Step2Service.SelectFirmForBillingStep2($scope.OrderStep2Obj);
        promise.success(function (response) {
            if (response.Success) {
                $scope.searchFirmList = angular.copy(response.Data);

                if (isFromLoad) {
                    var SearchFirmData = $filter('filter')($scope.searchFirmList, { FirmID: $scope.OrderStep2Obj.BillingFirmId }, true);
                    if (SearchFirmData && SearchFirmData.length > 0) {
                        var selectedData = SearchFirmData[0];

                        $scope.billingFirmAddress = selectedData.Address + "<br><b>" + selectedData.City + "</b> <b>" + selectedData.State + "</b> <b>" + selectedData.Zip + "</b><br>Phone No: " +
                            selectedData.Phone + "<br>Fax No: " + selectedData.Fax;
                        $scope.SelectedFirm = angular.copy(selectedData.Company);
                    }

                }
                bindMoreFirm();
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    function bindStateFarmTable()
    {
        if ($.fn.DataTable.isDataTable("#tblStateFarm")) {
            $('#tblStateFarm').DataTable().destroy();
        }
        var table = $('#tblStateFarm').DataTable({
            data: $scope.stateFarmList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Firm ID",
                    "className": "dt-left",
                    "data": "FirmID",
                    "sorting": "false",
                    "visible": false,
                },
                {
                    "title": "Firm",
                    "className": "dt-left",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return row.Company + "</br>" + row.Address + ' <b>' + row.City + '</b> <b>' + row.State + '</b> <b> ' + row.Zip + '</b>';
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblStateFarm').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).attr("ng-dblclick", "FillStateFarm($event)");
                $compile(angular.element(nRow))($scope);
            }
        });
    };


    function bindStateFarm() {
        //tblStateFarm
        //$scope.stateFarmList
        debugger;
        $scope.stateFarmList;
        var promise = Step2Service.GetStateFirmStep2();
        promise.success(function (response) {
            if (response.Success) {
                debugger;
                $scope.stateFarmList = response.Data;
                bindStateFarmTable();
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    function bindMoreFirm() {
        if ($.fn.DataTable.isDataTable("#tblSearchFirm")) {
            $('#tblSearchFirm').DataTable().destroy();
        }
        var table = $('#tblSearchFirm').DataTable({
            data: $scope.searchFirmList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Firm ID",
                    "className": "dt-left",
                    "data": "FirmID",
                    "sorting": "false",
                    "visible": false,
                },
                {
                    "title": "Firm",
                    "className": "dt-left",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return row.Company + "</br>" + row.Address + ' <b>' + row.City + '</b> <b>' + row.State + '</b> <b> ' + row.Zip + '</b>';
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblSearchFirm').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).attr("ng-dblclick", "FillFirm($event)");
                $compile(angular.element(nRow))($scope);
            }
        });

    }

    $scope.FillFirm = function ($event) {
        var tblSearchFirm = $('#tblSearchFirm').DataTable();
        var data = tblSearchFirm.row($($event.target).parents('tr')).data();
        $scope.OrderStep2Obj.BillingFirmId = data.FirmID;

        if ($scope.OrderStep2Obj.BillingFirmId == "STATDA04") {
            bindStateFarm();
            angular.element("#modal_StateFarm").modal('show');
        }

        else {
            var SearchFirmData = $filter('filter')($scope.searchFirmList, { FirmID: data.FirmID }, true);
            if (SearchFirmData) {
                var selectedData = SearchFirmData[0];
                $scope.billingFirmAddress = selectedData.Address + "<br><b>" + selectedData.City + "</b> <b>" + selectedData.State + "</b> <b>" + selectedData.Zip + "</b><br>Phone No: " +
                    selectedData.Phone + "<br>Fax No: " + selectedData.Fax;
                $scope.SelectedFirm = angular.copy(selectedData.Company);
                $scope.validateClaimNo = true;
            }
            else
                $scope.validateClaimNo = false;
        }

        $scope.BindAttorneyByFirmDropdown();
        angular.element("#modal_MoreBillSearchFirm").modal('hide');
    };

    $scope.FillStateFarm = function ($event) {
        debugger;
        var tblSearchFirm = $('#tblStateFarm').DataTable();
        var data = tblSearchFirm.row($($event.target).parents('tr')).data();
        $scope.OrderStep2Obj.BillingFirmId = data.FirmID;

        var SearchFirmData = $filter('filter')($scope.stateFarmList, { FirmID: data.FirmID }, true);
        if (SearchFirmData) {
            var selectedData = SearchFirmData[0];
            $scope.billingFirmAddress = selectedData.Address + "<br><b>" + selectedData.City + "</b> <b>" + selectedData.State + "</b> <b>" + selectedData.Zip + "</b><br>Phone No: " +
                selectedData.Phone + "<br>Fax No: " + selectedData.Fax;
            $scope.SelectedFirm = angular.copy(selectedData.Company);
            $scope.validateClaimNo = true;
        }
        else
            $scope.validateClaimNo = false;


        $scope.BindAttorneyByFirmDropdown();
        angular.element("#modal_StateFarm").modal('hide');
    };
    //#endregion

    init();

});