app.controller('DepartmentController', function ($scope, $rootScope, $stateParams, notificationFactory, DepartmentService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Department", "View");
    $scope.IsUserCanEditDepartment = $rootScope.isSubModuleAccessibleToUser('Settings', 'Department', 'Edit Department');
    $scope.IsUserCanDeleteDepartment = $rootScope.isSubModuleAccessibleToUser('Settings', 'Department', 'Delete Department');
    //-----

    function bindDepartmentList() {

        if ($.fn.DataTable.isDataTable("#tblDepartment")) {
            $('#tblDepartment').DataTable().destroy();
        }

        var table = $('#tblDepartment').DataTable({
            data: $scope.DepartmentList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Department",
                    "className": "dt-left",
                    "data": "Department",
                    "sorting": "true"
                },
                {
                    "title": "Active",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsActive",
                    "width": "10%",
                    "render": function (data, type, row) {
                        return (row.isActive) ? "<label class='label bg-success-400'>Active</label>" : "<label class='label bg-danger-400'>InActive</label>";
                    }
                },
                {
                    "title": 'Sort Order',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",
                    "visible": $scope.IsUserCanEditDepartment,
                    "render": function (data, type, row, meta) {
                     
                        var strAction = '';
                        if (!meta.row == 0)
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='UpdateSortOrder($event," + '"UP"' + ")' title='Increase Sort Order'> <i  class='icon-arrow-up7' ></i></a> ";
                        if (meta.row != $scope.DepartmentList.length - 1)
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='UpdateSortOrder($event," + '"DOWN"' + ")'  title='Decrease Sort Order'><i  class='icon-arrow-down7'></i> </a>";
                        return strAction;
                    }
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",                    
                    "render": function (data, type, row) {
                        var strAction = '';
                        if ($scope.IsUserCanEditDepartment) {
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='EditDepartment($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }
                        if ($scope.IsUserCanDeleteDepartment) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteDepartment($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        }                                            
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblDepartment').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    $scope.isEdit = false;

    $scope.UpdateSortOrder = function ($event, direction) {
        var table = $('#tblDepartment').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        var promise = DepartmentService.UpdateDepartmentSortOrder(row.DepartmentId, direction);
        promise.success(function (response) {
            if (response.Success) {
                GetDepartmentList();
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.EditDepartment = function ($event) {
        var table = $('#tblDepartment').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetDepartmentDetail(row.DepartmentId);
    };
    $scope.DeleteDepartment = function ($event) {

        bootbox.confirm({
            message: "Are you sure you want to delete this record ?",
            buttons: {
                confirm: { label: 'Yes', className: 'btn-success' },
                cancel: { label: 'No', className: 'btn-danger' }
            },
            callback: function (result) {
                if (result == true) {
                    var table = $('#tblDepartment').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = DepartmentService.DeleteDepartment(row.DepartmentId);
                    promise.success(function (response) {
                        if (response.Success) {
                            if (response.str_ResponseData == '-1') {
                                toastr.error("You can not delete this department, it is already in use.");
                                bootbox.hideAll()
                                return false;
                            }
                            else {
                                notificationFactory.customSuccess("Department Deleted Successfully");
                                GetDepartmentList()
                                bootbox.hideAll()
                            }
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error();
                }
                else {

                }
            }
        });
    };

    $scope.GetDepartmentDetail = function (DepartmentID) {

        $scope.Departmentform.$setPristine();
        if (DepartmentID > 0) {
            $scope.modal_Title = "Edit Department";
            $scope.isEdit = true;
            var promise = DepartmentService.GetDepartmentList(DepartmentID);
            promise.success(function (response) {
                $scope.objDepartment = response.Data[0];
                console.log($scope.objDepartment);

                angular.element("#modal_Department").modal('show');
            });
            promise.error(function (data, statusCode) {
            });
        }
        else {
            $scope.modal_Title = "Add Department";
            $scope.objDepartment = new Object();
            $scope.objDepartment.DepartmentName = "";
            $scope.objDepartment.isActive = true;

            angular.element("#modal_Department").modal('show');
        }

    };

    $scope.AddOrUpdateDepartment = function (form) {
        if (form.$valid) {
            if (!$scope.isEdit) {
                var promise = DepartmentService.InsertDepartment($scope.objDepartment);
                promise.success(function (response) {
                    if (response.Success) {
                        if (response.str_ResponseData == '-1') {
                            toastr.error("Department Name already exists.");
                            return false;
                        }
                        else {
                            toastr.success("Department saved successfully");
                            angular.element("#modal_Department").modal('hide');
                            GetDepartmentList();
                        }
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else {
                var promiseedit = DepartmentService.UpdateDepartment($scope.objDepartment);
                promiseedit.success(function (response) {
                    if (response.Success) {

                        if (response.str_ResponseData == '-1') {
                            toastr.error("Department Name already exists.");
                            return false;
                        }
                        else {
                            toastr.success("Department updated successfully");
                            angular.element("#modal_Department").modal('hide');
                            GetDepartmentList();
                        }
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promiseedit.error(function (data, statusCode) {
                });
            }
        }
    };


    function GetDepartmentList() {
        var promise = DepartmentService.GetDepartmentList(0);
        promise.success(function (response) {
            $scope.DepartmentList = response.Data;
            bindDepartmentList();
        });
        promise.error(function (data, statusCode) {
        });
    }
    function init() {
        GetDepartmentList();
    }
    init();
});