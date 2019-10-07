app.controller('EmployeeController', function ($scope, $rootScope, $stateParams, RoleServices, notificationFactory, EmployeeServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;

    //Page Rights//
    $scope.IsUserCanEditEmployee = $rootScope.isSubModuleAccessibleToUser('Settings', 'Employees', 'Edit Employee');
    //-----

    function bindEmployeeList() {
        if ($.fn.DataTable.isDataTable("#tblEmployee")) {
            $('#tblEmployee').DataTable().destroy();
        }
        var table = $('#tblEmployee').DataTable({
            data: $scope.Employeelist,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Employee Id",
                    "className": "dt-left",
                    "data": "EmpId",
                    "width": "10%",
                    "sorting": "false"
                },
                {
                    "title": "Name",
                    "className": "dt-left",
                    "data": "EmployeeName",
                    "width": "15%",
                    "sorting": "false"
                },
                {
                    "title": "Email",
                    "className": "dt-left",
                    "data": "UserName",
                    "width": "15%",
                    "sorting": "false"
                },
                {
                    "title": "Department",
                    "className": "dt-left",
                    "data": "Department",
                    "sorting": "false"
                },
                {
                    "title": "Status",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsApproved",
                    "width": "5%",
                    "render": function (data, type, row) {
                        return (row.IsApproved) ? "<label class='label bg-success-400'>Active</label>" : "<label class='label bg-danger-400'>InActive</label>";
                    }
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "5%",
                    "visible": $scope.IsUserCanEditEmployee,
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='EditEmployee($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblEmployee').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function GetEmployeelist() {
        var promise = EmployeeServices.GetEmployeeList();
        promise.success(function (response) {
            $scope.Employeelist = response.Data;
            bindEmployeeList();
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.AddEmployee = function () {
        $scope.EmployeeObj = new Object();
        $scope.EmployeeObj.IsApproved = true;
        $scope.isEdit = false;
        $scope.modal_Title = "Add Employee";
        $scope.btnText = "Save";
        $scope.Employeeform.$setPristine();
        bindDropDown();
        $scope.GetRoleList();
        CloseMultiSelectDropDownIfOpen();
        angular.element("#modal_Employee").modal('show');
    };


    function bindDropDown() {
        var promise = CommonServices.DepartmentDropdown();
        promise.success(function (response) {
            $scope.DeptList = response.Data;
        });
        promise.error(function (data, statusCode) {

        });

        var company = CommonServices.GetCompanyDropDown();
        company.success(function (response) {
            $scope.CompanyList = response.Data;
        });
        company.error(function (data, statusCode) {

        });
    }
    $scope.DeleteEmployee = function ($event) {
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
                    var table = $('#tblEmployee').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = EmployeeServices.DeleteEmployee(row.ID);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.customSuccess("Employee Delete Successfully");
                            GetEmployeelist();
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

    $scope.EditEmployee = function ($event) {
        var table = $('#tblEmployee').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetRoleList();
        $scope.GetEmployeeById(row.UserId);
    };

    $scope.GetEmployeeById = function (id) {
        $scope.Employeeform.$setPristine();
        bindDropDown();
        var promise = EmployeeServices.GetEmployeeById(id);
        promise.success(function (response) {
            if (response.Success) {

                $scope.isEdit = true;
                $scope.modal_Title = "Edit Employee";
                $scope.btnText = "Save";
                $scope.EmployeeObj = response.Data[0];
                CloseMultiSelectDropDownIfOpen();
                if (!isNullOrUndefinedOrEmpty(response.Data[0].SelectedRoles)) {
                    $scope.EmployeeObj.SelectedRoles = response.Data[0].SelectedRoles.split(',');
                } else {
                    $scope.EmployeeObj.SelectedRoles = [];
                }
                $scope.SelectedRoles();
                angular.element("#modal_Employee").modal('show');
            }
            else {
                notificationFactory.customError(response.Message[0]);
            }
        });
        promise.error(function () { });
    };

    $scope.AddOrEditEmployee = function (form) {
        if (form.$valid) {
            var selectedRoles = "";
            if ($scope.EmployeeObj.SelectedRoles.length > 0) {
                $scope.EmployeeObj.SelectedRoles = $scope.EmployeeObj.SelectedRoles.join(',');
            }

            $scope.EmployeeObj.CreatedBy = $scope.UserAccessId;
            if (!$scope.isEdit) {  // add mode
                $scope.EmployeeObj.Password = "cem@123";
                $scope.EmployeeObj.CompanyNo = $rootScope.CompanyNo;
                var promise = EmployeeServices.InsertEmployee($scope.EmployeeObj);
                promise.success(function (response) {
                    if (response.Success) {
                        if (response.str_ResponseData == '-1') {
                            toastr.error("User Email already exists.");
                            return false;
                        }
                        else {
                            toastr.success("Employee detail inserted successfully.");
                            angular.element("#modal_Employee").modal('hide');
                            GetEmployeelist();
                        }
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
            else { //Edit Mode
                var promise = EmployeeServices.UpdateEmployee($scope.EmployeeObj);
                promise.success(function (response) {
                    if (response.str_ResponseData == '-1') {
                        toastr.error("User Email already exists.");
                        return false;
                    }
                    else {
                        toastr.success("Employee detail Updated successfully.");
                        angular.element("#modal_Employee").modal('hide');
                        GetEmployeelist();
                    }
                });
                promise.error(function () { });
            }
        }
    };

    $scope.GetRoleList = function () {
        var promise = RoleServices.GetRole(0);
        promise.success(function (response) {
            $scope.RoleList = [];
            $scope.RoleListData = response.Data;
            angular.forEach($scope.RoleListData, function (role, i) {
                if (role.IsActive) {
                    var roleObj = new Object();
                    roleObj.name = role.RoleName;
                    roleObj.id = role.RoleId;
                    $scope.RoleList.push(roleObj);
                }
            });
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.SelectedRoles = function () {
        if (isNullOrUndefinedOrEmpty($scope.EmployeeObj.SelectedRoles)) {
            $scope.EmployeeObj.SelectedRoles = [];
        }
        if ($scope.EmployeeObj.SelectedRoles.length > 0) {
            $scope.RoleNames = $scope.EmployeeObj.SelectedRoles.length + '- Selected ';
        } else {
            $scope.RoleNames = 'None';
        }

        //if (!isNullOrUndefinedOrEmpty($scope.EmployeeObj.SelectedRoles) && !isNullOrUndefinedOrEmpty($scope.RoleList)) {
        //    for (var i = 0; i < $scope.EmployeeObj.SelectedRoles.length; i++) {
        //        var roleobj = $filter('filter')($scope.RoleList, { id: $scope.EmployeeObj.SelectedRoles[i] }, true)[0];
        //        if (!isNullOrUndefinedOrEmpty(roleobj)) {
        //            $scope.RoleNames = $scope.RoleNames + roleobj.name + " , ";
        //        }

        //    }
        //}
    }
    function CloseMultiSelectDropDownIfOpen() {
        if ($('#ddRole div').hasClass('btn-group multiselectDropdown open')) {
            $('#ddRole div').removeClass('open');
        }

        $scope.RoleNames = "";
    }


    function init() {
        $scope.EmployeeObj = new Object();
        GetEmployeelist();
        $scope.EmployeeObj.SelectedRoles = [];
        $scope.RoleNames = [];
    }

    init();

});