app.controller('OrderPartController', function ($window, $scope, $rootScope, $timeout, $state, $stateParams, notificationFactory, Step6Service, CommonServices, OrderPartService, FirmScopeServices, configurationService, $compile, $filter) {
    decodeParams($stateParams);
    $scope.$on('PartDetailsOpenedFromClientOrderList', PartDetailsOpenedFromClientOrderList);
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.LocationID = "";
    $scope.IsLocSelect = false;
    $scope.RoleName = $rootScope.LoggedInUserDetail.RoleName ? $rootScope.LoggedInUserDetail.RoleName[0] : '';

    //#region Event
    $scope.OpenPart = function (OrderNo, Partno, $event) {
        $window.open("PartDetail?OrderId=" + OrderNo + "&PartNo=" + Partno, '_blank');
    };

    $scope.AddLocation = function () {
        $window.open("EditOrder?OrderId=" + $scope.OrderId + "&Step=6" + "&AddPart=true", '_blank');

        //$scope.IsFromPart = true;
        //$timeout(function () {
        //    $rootScope.$broadcast('AddLocationFromOrderDetailPart');
        //}, 1000);

        //toastr.error("TODO: Not developed yet");
    };

    $scope.GetPartListByOrderId = function (OrderId) {

        var promise = OrderPartService.GetPartListByOrderId(OrderId, 0);
        promise.success(function (response) {
            $scope.OrderPartList = response.Data;
            //$scope.bindOrderPartList(response.Data);            
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.NewPartPopUp = function () {
        $scope.IsLocSelect = false;
        $scope.OrderPartObj = new Object();
        $scope.OrderPartObj.LocID = "";
        $scope.OrderPartform.$setPristine();
        angular.element("#modal_Add_Part").modal('show');
        bindDropDownList();
    };

    $scope.GetLocationSearch = function () {
        var promise = Step6Service.GetLocationSearch();
        promise.success(function (response) {
            if (response.Success) {
                angular.element("#modal_Search_Location").modal('show');
                $scope.searchLocationList = response.Data;
                bindLocationSerach();
            }
        });
        promise.error(function (data, statusCode) {
        });

    };

    $scope.fillLocation = function ($event) {  //Fill detail to textbox from selected location
        $scope.OrderPartObj = new Object();
        var tblSearchLocation = $('#tblSearchLocation').DataTable();
        var data = tblSearchLocation.row($($event.target).parents('tr')).data();
        $scope.LocationID = data.LocID;
        $scope.OrderPartObj.LocID = data.LocID;
        var promise = OrderPartService.GetLocationByLocIDForPart($scope.LocationID);
        promise.success(function (response) {
            if (response.Success) {
                $scope.IsLocSelect = true;
                $scope.OrderPartObj = response.Data[0];
                angular.element("#modal_Search_Location").modal('hide');
            }
        });
        promise.error(function (data, statusCode) {
        });

    };

    $scope.SaveNewPart = function (form) {
        if (!isNullOrUndefinedOrEmpty($scope.OrderPartObj.LocID)) {
            form.$submitted = true;
            if (form.$valid) {

                $scope.OrderPartObj.EmpId = $scope.EmpId;
                $scope.OrderPartObj.OrderId = $scope.OrderId;
                $scope.OrderPartObj.RoleName = $scope.RoleName;

                var promise = OrderPartService.InsertOrderPart($scope.OrderPartObj);
                promise.success(function (response) {
                    if (response.Success) {

                        angular.element("#modal_Add_Part").modal('hide');
                        toastr.success("Save Successfully");
                        $scope.GetPartListByOrderId($scope.OrderId);

                        $window.location.reload();
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
        }
        else {
            notificationFactory.customError("Select Location");
            $scope.OrderPartform.$setPristine();
        }

    };

    $scope.SelectScope = function () {
        if ($scope.OrderPartObj.RecordTypeId > 0) {
            var promise = FirmScopeServices.GetScopeByRecType($scope.OrderPartObj.RecordTypeId);
            promise.success(function (response) {
                if (response.Success) {
                    $scope.scopelist = response.Data;
                    bindScopelist();
                    angular.element("#modal_selectScope").modal('show');
                }
                else {
                    notificationFactory.customError(response.Message[0]);
                }
            });
            promise.error(function () { });
        }
        else {
            notificationFactory.customError("Select Record Type");
        }
    };

    $scope.AddScopeToScopeList = function ($event) {
        var table = $('#tblScope').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.OrderPartObj.Scope = row.ScopeDesc;
        angular.element("#modal_selectScope").modal('hide');
    };

    $scope.SaveChronology = function (item, $event) {

        $event.stopPropagation();
        var promise = OrderPartService.AddUpdateChronolgy($scope.OrderId, item.PartNo, $rootScope.LoggedInUserDetail.UserId, item.ISChronology);
        promise.success(function (response) {
            if (response.Success) {
                if (response.str_ResponseData == 'True') {
                    toastr.success("Add Chronology Successfully");
                }
                else {
                    toastr.success("Remove Chronology Successfully");
                }
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.AddDocumentPopUp = function (dataitem, $event) {
        if (!isNullOrUndefinedOrEmpty($event)) {
            $event.stopPropagation();
        }
        $scope.PartNo = dataitem.PartNo;
        $scope.$parent.OpenUploadDocumentPopUp(dataitem.OrderId, dataitem.PartNo);
        //$rootScope.$broadcast('OpenedFromPartList', null);
    };

    $scope.OnSelectAll_click = function ($event) {
        angular.element('input.chkchildItem:checkbox').not($event.target).prop('checked', $event.target.checked);
    };
    $scope.OnSelect_click = function ($event) {
        $event.stopPropagation();
        angular.element('#chkAll').prop('checked', angular.element('input.chkchildItem:checked').length == angular.element('input.chkchildItem:checkbox').length);
    };

    $scope.CancelPartSendEmail = function () {

        var arrayPartNo = [];
        angular.element('input.chkchildItem:checked').each(function (index, element) {
            arrayPartNo.push((element.getAttribute("partno")));
        });
        if (arrayPartNo.length == 0) {
            toastr.error("At least one record selection required to cancel part/order.");
            return;
        }

        bootbox.confirm({
            message: "Are you sure you want to cancel this order/part(s)?",
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
                    var promise = OrderPartService.CancelPartSendEmail($scope.OrderId, arrayPartNo.join(','), $rootScope.LoggedInUserDetail.UserId, $rootScope.CompanyNo);
                    promise.success(function (response) {
                        if (response.Success) {
                            toastr.success("Email sent successfully");
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

    //#region Method  

    function bindDropDownList() {
        var promise = CommonServices.LocationDepartmentDropDown();
        promise.success(function (response) {
            if (response.Success) {
                $scope.departmentlist = response.Data;
            }
        });
        promise.error(function (data, statusCode) {
        });

        var state = CommonServices.StateDropdown();
        state.success(function (response) {
            $scope.StateList = response.Data;
        });
        var recordtype = CommonServices.RecordTypeDropDown();
        recordtype.success(function (response) {
            $scope.RecordTypeList = response.Data;
        });
    }

    function bindLocationSerach() {
        if ($.fn.DataTable.isDataTable("#tblSearchLocation")) {
            $('#tblSearchLocation').DataTable().destroy();
        }
        var table = $('#tblSearchLocation').DataTable({
            data: $scope.searchLocationList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Location Id",
                    "className": "dt-left",
                    "data": "LocID",
                    "sorting": "false",
                    "width": "10%"
                },
                {
                    "title": "Name",
                    "className": "dt-left",
                    "data": "Name1",
                    "sorting": "false"
                },
                {
                    "title": "Name2",
                    "className": "dt-left",
                    "data": "Name2",
                    "sorting": "false"
                },
                {
                    "title": "Department",
                    "className": "dt-left",
                    "data": "Dept",
                    "width": "15%",
                    "sorting": "false"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "8%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='fillLocation($event)' data-toggle='tooltip' data-placement='top' tooltip title='Add'> <i  class='icon-add cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblSearchLocation').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function bindScopelist() { //Scope
        if ($.fn.DataTable.isDataTable("#tblScope")) {
            $('#tblScope').DataTable().destroy();
        }

        var table = $('#tblScope').DataTable({
            data: $scope.scopelist,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Scope",
                    "className": "dt-left",
                    "data": "ScopeDesc",
                    "sorting": "false"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='AddScopeToScopeList($event)' data-toggle='tooltip' data-placement='left' tooltip title='Edit'> <i  class='icon-add cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblScope').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    };

    function PartDetailsOpenedFromClientOrderList() {
        $scope.IsFromClientOrderList = true;
        init();
    }
    function init() {
        $scope.IsFromPart = false;
        $scope.OrderPartList = [];
        $scope.OrderId = ($stateParams.OrderId || $scope.$parent.OrderId);
        if (!isNullOrUndefinedOrEmpty($scope.OrderId)) {
            $scope.GetPartListByOrderId($scope.OrderId);
        }
    }

    $scope.bindOrderPartList = function () {

        if ($.fn.DataTable.isDataTable("#tblOrderPart")) {
            $('#tblOrderPart').DataTable().destroy();
        }

        var table = $('#tblOrderPart').DataTable({
            data: $scope.OrderPartList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "PartNo",
                    "className": "dt-left",
                    "data": "PartNo",
                    "width": "5%"
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblOrderPart').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }


    //#endregion

    init();

});