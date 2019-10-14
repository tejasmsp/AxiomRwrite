app.controller('FirmScopeController', function ($scope, $rootScope, $stateParams, notificationFactory, FirmScopeServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;

    function bindFirmScopeList() {
        if ($.fn.DataTable.isDataTable("#tblFirmScope")) {
            $('#tblFirmScope').DataTable().destroy();
        }

        var table = $('#tblFirmScope').DataTable({
            data: $scope.FirmScopelist,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "FirmID",
                    "className": "dt-left",
                    "data": "FirmID",
                    "sorting": "false"
                },
                {
                    "title": "Firm Name",
                    "className": "dt-left",
                    "data": "FirmName",
                    "sorting": "false"
                },
                {
                    "title": "Record Type",
                    "className": "dt-left",
                    "data": "RecTypeName",
                    "sorting": "false"
                },

                {
                    "title": "Scope",
                    "className": "dt-left",
                    "data": "ScopeTitle",
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
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='EditFirmScope($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteFirmScope($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblFirmScope').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function GetFirmScopelist() {
        var promise = FirmScopeServices.GetFirmScopeList("", 0);
        promise.success(function (response) {
            $scope.FirmScopelist = response.Data;
            bindFirmScopeList();
        });
        promise.error(function (data, statusCode) {
        });
    }

    function bindDropDown() {
        var Firm = CommonServices.FirmForDropdown('', $rootScope.CompanyNo);
        Firm.success(function (response) {
            $scope.Firmlist = response.Data;
        });

        var RecordType = CommonServices.RecordTypeDropDown();
        RecordType.success(function (response) {
            $scope.RecordTypelist = response.Data;
        });
    }

    $scope.AddFirmScope = function () {
        $scope.FirmScopeObj = new Object();
        $scope.isEdit = false;
        $scope.modal_Title = "Add Firm Scope";
        $scope.btnText = "Add";
        $scope.FirmScopeform.$setPristine();
        angular.element("#modal_FirmScope").modal('show');
        bindDropDown();
    };

    $scope.DeleteFirmScope = function ($event) {
        bootbox.confirm({
            message: "Are you sure you want to delete this record ?",
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
                    var table = $('#tblFirmScope').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = FirmScopeServices.DeleteFirmScope(row.MapID);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.customSuccess("Firm  Delete Successfully");
                            GetFirmScopelist();
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function () { });
                }
                bootbox.hideAll();
            }
        });
    };

    $scope.EditFirmScope = function ($event) {
        var table = $('#tblFirmScope').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetFirmScopeById(row.MapID);
    };

    $scope.GetFirmScopeById = function (mapid) {
        var promise = FirmScopeServices.GetFirmScopeById(mapid);
        promise.success(function (response) {
            if (response.Success) {
                bindDropDown();
                $scope.isEdit = true;
                $scope.modal_Title = "Edit Firm Scope";
                $scope.btnText = "Edit";
                $scope.FirmScopeObj = response.Data[0];
                $scope.FirmScopeform.$setPristine();
                angular.element("#modal_FirmScope").modal('show');
            }
            else {
                notificationFactory.customError(response.Message[0]);
            }
        });
        promise.error(function () { });
    };

    $scope.AddOrEditFirmScope = function (form) {
        if (form.$valid) {
            $scope.FirmScopeObj.RecTypeName = $.grep($scope.RecordTypelist, function (r) {
                return r.Code == $scope.FirmScopeObj.RecTypeID;
            })[0].Descr;

            $scope.FirmScopeObj.CreatedBy = $scope.UserAccessId;
            if (!$scope.isEdit) {  // add mode
                var promise = FirmScopeServices.InsertFirmDefaultScope($scope.FirmScopeObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_FirmScope").modal('hide');
                        notificationFactory.customSuccess("Firm scope save successfully");
                        GetFirmScopelist();
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
            else { //Edit Mode
                var promise = FirmScopeServices.UpdateFirmDefaultScope($scope.FirmScopeObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_FirmScope").modal('hide');
                        notificationFactory.customSuccess("Firm scope Update successfully");
                        GetFirmScopelist();
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
        }
    };

    $scope.SelectScope = function () {

        if ($scope.FirmScopeObj.RecTypeID > 0) {
            var promise = FirmScopeServices.GetScopeByRecType($scope.FirmScopeObj.RecTypeID);
            promise.success(function (response) {
                if (response.Success) {
                    $scope.scopelist = response.Data;
                    $scope.bindScopelist();
                    angular.element("#modal_FirmselectScope").modal('show');
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

    $scope.bindScopelist = function () {
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
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='AddScopeToScopeList($event)' title='Edit'> <i  class='icon-add cursor-pointer'></i> </a>";
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

    $scope.AddScopeToScopeList = function ($event) {
        var table = $('#tblScope').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.FirmScopeObj.Scope = row.ScopeDesc;
        angular.element("#modal_FirmselectScope").modal('hide');
    };

    function init() {
        GetFirmScopelist();
        $scope.FirmScopeObj = new Object();
    }

    init();

});