app.controller('ScopeController', function ($scope, $rootScope, $stateParams, notificationFactory, AttorneyScopeServices, FirmScopeServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);

    $scope.isEdit = false;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.SelectedScope = "1";

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Scope", "View");
    $scope.IsUserCanEditScope = $rootScope.isSubModuleAccessibleToUser('Settings', 'Scope', 'Edit Scope');
    $scope.IsUserCanDeletScope = $rootScope.isSubModuleAccessibleToUser('Settings', 'Scope', 'Delete Scope');
    //-----


    $scope.fn_ChangeScope = function () {
        if ($scope.SelectedScope == "1") {
            GetAttorneyScopelist();
        }
        else if ($scope.SelectedScope == "2") {
            GetFirmScopelist();
        }
        else if ($scope.SelectedScope == "3") {
            GetDefaultScopelist();
        }
    };

    //#region Attornry

    function bindAttorneyScopeList() {
        if ($.fn.DataTable.isDataTable("#tblAttorneyScope")) {
            $('#tblAttorneyScope').DataTable().destroy();
        }

        var table = $('#tblAttorneyScope').DataTable({
            data: $scope.AttorneyScopelist,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Attorney Id",
                    "className": "dt-left",
                    "data": "AttyID",
                    "width": "6%",
                    "sorting": "false",

                },
                {
                    "title": "Attorney Name",
                    "className": "dt-left",
                    "data": "AttorneyName",
                    "width": "8%",
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
                    "width": "6%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if ($scope.IsUserCanEditScope) {
                            strAction = "<a  class='ico_btn cursor-pointer' ng-click='EditAttorneyScope($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }
                        if ($scope.IsUserCanDeletScope) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteAttorneyScope($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        }


                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblAttorneyScope').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function GetAttorneyScopelist() {
        var promise = AttorneyScopeServices.GetAttorneyScopeList();
        promise.success(function (response) {
            $scope.AttorneyScopelist = response.Data;
            bindAttorneyScopeList();
        });
        promise.error(function (data, statusCode) {
        });
    }

    //Default scope
    function GetDefaultScopelist() {
        var promise = AttorneyScopeServices.GetDefaultScope();
        promise.success(function (response) {
            $scope.DefaultScopelist = response.Data;
            bindDefaultScopeList();
        });
        promise.error(function (data, statusCode) {
        });
    }
    function bindDefaultScopeList() {
        if ($.fn.DataTable.isDataTable("#tblDefaultScope")) {
            $('#tblDefaultScope').DataTable().destroy();
        }
        var table = $('#tblDefaultScope').DataTable({
            data: $scope.DefaultScopelist,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "ScopeID",
                    "className": "dt-left",
                    "data": "ScopeID",
                    "width": "6%",


                },
                {
                    "title": "Record Type",
                    "className": "dt-left",
                    "data": "Descr",

                },

                {
                    "title": "Scope",
                    "className": "dt-left",
                    "data": "ScopeDesc",


                },
                {
                    "title": "Is Default",
                    "className": "dt-center",
                    "data": null,
                    "width": "6%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if (row.IsDefault == 1) {
                            strAction += "<a class='label  bg-success-400'>Yes</a>";
                        }
                        if (row.IsDefault == 0 || row.IsDefault == null) {
                            strAction += '<lable class="label bg-danger-400">No</lable>';
                        }

                        return strAction;
                    }

                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "6%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if ($scope.IsUserCanEditScope) {
                            strAction = "<a  class='ico_btn cursor-pointer' ng-click='EditDefaultScope($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }

                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblDefaultScope').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    $scope.EditDefaultScope = function ($event) {
        var table = $('#tblDefaultScope').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetDefaultScopeById(row.ScopeID);
    };
    $scope.GetDefaultScopeById = function (ScopeID) {
        var promise = AttorneyScopeServices.GetDefaultScopeById(ScopeID);
        promise.success(function (response) {
            if (response.Success) {
                bindDropDown();
                $scope.isEdit = true;
                $scope.modal_Title = "Edit Default Scope";
                $scope.btnText = "Save";
                $scope.DefaultScopeObj = response.Data[0];
                angular.element("#modal_DefaultScope").modal('show');
            }
            else {
                notificationFactory.customError(response.Message[0]);
            }
        });
        promise.error(function () { });
    }
    $scope.AddOrEditDefaultScope = function (form) {
        if (form.$valid) {
            { //Edit Mode
                var promise = AttorneyScopeServices.UpdateDefaultScope($scope.DefaultScopeObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_DefaultScope").modal('hide');
                        notificationFactory.customSuccess("Default scope Update successfully");
                        GetDefaultScopelist();
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
        }
    };
    //Default End




    $scope.AddOrEditAttorneyScope = function (form) {
        if (form.$valid) {
            $scope.AttorneyScopeObj.RecTypeName = $.grep($scope.RecordTypelist, function (r) {
                return r.Code == $scope.AttorneyScopeObj.RecTypeID;
            })[0].Descr;
            $scope.AttorneyScopeObj.CreatedBy = $scope.UserAccessId;
            if (!$scope.isEdit) {  // add mode
                var promise = AttorneyScopeServices.InsertAttorneyScope($scope.AttorneyScopeObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_AttorneyScope").modal('hide');
                        notificationFactory.customSuccess("Attorney scope save successfully");
                        GetAttorneyScopelist();
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
            else { //Edit Mode
                var promise = AttorneyScopeServices.UpdateAttorneyScope($scope.AttorneyScopeObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_AttorneyScope").modal('hide');
                        notificationFactory.customSuccess("Attorney scope Update successfully");
                        GetAttorneyScopelist();
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
        }
    };

    $scope.DeleteAttorneyScope = function ($event) {
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
                    var table = $('#tblAttorneyScope').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = AttorneyScopeServices.DeleteAttorneyScope(row.MapID);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.customSuccess("Attorney scope delete successfully.");
                            GetAttorneyScopelist();
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

    $scope.GetAttorneyScopeById = function (mapid) {
        var promise = AttorneyScopeServices.GetAttorneyScopeById(mapid);
        promise.success(function (response) {
            if (response.Success) {
                bindDropDown();
                $scope.isEdit = true;
                $scope.modal_Title = "Edit Attorney Scope";
                $scope.btnText = "Save";
                $scope.AttorneyScopeObj = response.Data[0];
                angular.element("#modal_AttorneyScope").modal('show');
            }
            else {
                notificationFactory.customError(response.Message[0]);
            }
        });
        promise.error(function () { });
    };

    $scope.EditAttorneyScope = function ($event) {
        var table = $('#tblAttorneyScope').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetAttorneyScopeById(row.MapID);
    };

    // #endregion

    //#region firm Scope

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
                    "title": "Firm Id",
                    "className": "dt-left",
                    "data": "FirmID",
                    "width": "6%",
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
                    "width": "6%",
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

    $scope.DeleteFirmScope = function ($event) {
        bootbox.confirm({
            message: "Are you sure you want to delete this record?",
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
                            notificationFactory.customSuccess("Firm scope delete successfully.");
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

    $scope.GetFirmScopeById = function (mapid) {
        var promise = FirmScopeServices.GetFirmScopeById(mapid);
        promise.success(function (response) {
            if (response.Success) {
                bindDropDown();
                $scope.isEdit = true;
                $scope.modal_Title = "Edit Firm Scope";
                $scope.btnText = "Save";
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

    $scope.EditFirmScope = function ($event) {
        var table = $('#tblFirmScope').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetFirmScopeById(row.MapID);
    };

    //#endregion 

    //#region common
    function bindDropDown() {
        if ($scope.SelectedScope == "1") {
            var Attorney = CommonServices.AttorneyForDropdown('', $rootScope.CompanyNo);
            Attorney.success(function (response) {
                $scope.Attorneylist = response.Data;
            });
        }
        else if ($scope.SelectedScope == "2") {
            var Firm = CommonServices.FirmForDropdown('', $rootScope.CompanyNo);
            Firm.success(function (response) {
                $scope.Firmlist = response.Data;
            });
        }
        var RecordType = CommonServices.RecordTypeDropDown();
        RecordType.success(function (response) {
            $scope.RecordTypelist = response.Data;
        });

    }

    $scope.AddSelectedScope = function () {
        bindDropDown();
        if ($scope.SelectedScope == "1") {
            $scope.AttorneyScopeObj = new Object();
            $scope.AttorneyScopeObj.RecTypeID = 0;
            $scope.isEdit = false;
            $scope.modal_Title = "Add Attorney Scope";
            $scope.btnText = "Save";
            $scope.AttorneyScopeform.$setPristine();
            angular.element("#modal_AttorneyScope").modal('show');
        }
        else {
            $scope.FirmScopeObj = new Object();
            $scope.FirmScopeObj.RecTypeID = 0;
            $scope.isEdit = false;
            $scope.modal_Title = "Add Firm Scope";
            $scope.btnText = "Save";
            $scope.FirmScopeform.$setPristine();
            angular.element("#modal_FirmScope").modal('show');
        }

    };
    //#endregion

    //#region Select from scope

    $scope.SelectScope = function () {
        if ($scope.SelectedScope == "1") {
            if ($scope.AttorneyScopeObj.RecTypeID > 0) {
                var promise = FirmScopeServices.GetScopeByRecType($scope.AttorneyScopeObj.RecTypeID);
                promise.success(function (response) {
                    if (response.Success) {
                        $scope.scopelist = response.Data;
                        $scope.bindScopelist();
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
        }
        else if ($scope.SelectedScope == "2") {
            if ($scope.FirmScopeObj.RecTypeID > 0) {
                var promise = FirmScopeServices.GetScopeByRecType($scope.FirmScopeObj.RecTypeID);
                promise.success(function (response) {
                    if (response.Success) {
                        $scope.scopelist = response.Data;
                        $scope.bindScopelist();
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
            "aLengthMenu": [5],
            "pageLength": 5,
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
        if ($scope.SelectedScope == "1") {
            $scope.AttorneyScopeObj.Scope = row.ScopeDesc;
        } else if ($scope.SelectedScope == "2") {
            $scope.FirmScopeObj.Scope = row.ScopeDesc;
        }
        angular.element("#modal_selectScope").modal('hide');


    };
    //#endregion 


    function init() {
        GetAttorneyScopelist();
        $scope.AttorneyScopeObj = new Object();
        $scope.DefaultScopeObj = new Object();
    }

    init();

});