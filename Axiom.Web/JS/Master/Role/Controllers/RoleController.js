app.controller('RoleController', function ($scope,$rootScope, $stateParams,$state, notificationFactory,RoleServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;

    //Page Rights//    
    $rootScope.CheckIsPageAccessible("Admin", "Roles", "View");    
    $scope.IsUserCanEditRole = $rootScope.isSubModuleAccessibleToUser('Admin', 'Roles', 'Edit Role');
    $scope.IsUserCanAddEditRoleRightsConfiguration = $rootScope.isSubModuleAccessibleToUser('Admin', 'Roles', 'Add/Edit Role Rights Configuration');
    //-----

    function bindRoleList() {
        if ($.fn.DataTable.isDataTable("#tblRole")) {
            $('#tblRole').DataTable().destroy();
        }        

        var table = $('#tblRole').DataTable({
            data: $scope.RoleList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',            
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Role Name",
                    "className": "dt-left",
                    "data": "RoleName"                                        

                },
                  {
                      "title": "Description",
                      "className": "dt-left",
                      "data": "Description"

                  },
                 {
                     "title": "Active",
                     "className": "dt-center",                     
                     "data": "IsActive",
                     "width": "10%",
                     "render": function (data, type, row) {
                         return (row.IsActive) ? "<label class='label bg-success-400'>Active</label>" : "<label class='label bg-danger-400'>InActive</label>";
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
                        if ($scope.IsUserCanEditRole) {
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='EditRole($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }
                        if ($scope.IsUserCanAddEditRoleRightsConfiguration) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='GoToRoleConfiguration($event)' title='Role Rights Configuration'> <i  class='icon-cog52' ></i></a> ";
                        }
                        
                        
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblRole').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    $scope.GoToRoleConfiguration = function ($event) {        
        var table = $('#tblRole').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $state.go("RoleConfiguration", { RoleAccessId: row.RoleAccessId, RoleName: row.RoleName });
    }

    function GetRole(roleAccessId) {
        if (roleAccessId>0)
        {
            var promise = RoleServices.GetRole(roleAccessId);
            promise.success(function (response) {                
                $scope.ResetForm();
                $scope.modal_Title = "Edit Role";
                $scope.btnText = "Edit";
                $scope.RoleObj = response.Data[0];
                angular.element("#modal_Role").modal('show');
            });
            promise.error(function (data, statusCode) {
            });

        }
        else {
            var promise = RoleServices.GetRole(0);
            promise.success(function (response) {                
                $scope.RoleList = response.Data;
                bindRoleList();
            });
            promise.error(function (data, statusCode) {
            });
        }      
    }

    $scope.ResetForm = function () {
        $scope.Roleform.$setPristine();
    }


    $scope.AddRole = function () {
        $scope.ResetForm();
        $scope.RoleObj = new Object();
        $scope.RoleObj.IsActive = true;
        $scope.modal_Title = "Add Role";        
        angular.element("#modal_Role").modal('show');
    };


    $scope.EditRole = function ($event) {      
        var table = $('#tblRole').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        GetRole(row.RoleAccessId);
    };
    $scope.AddOrEditRole = function (form) {
        $scope.RoleObj.CreatedBy = $scope.UserAccessId;
        if (form.$valid) {           
            if (!($scope.RoleObj.RoleAccessId>0)) {  // add mode
                var promise = RoleServices.InsertRole($scope.RoleObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_Role").modal('hide');                        
                        notificationFactory.customSuccess("Role Saved Successfully");
                        GetRole(0);
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
            else { //Edit Mode
                var promise = RoleServices.UpdateRole($scope.RoleObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_Role").modal('hide');
                        notificationFactory.customSuccess("Role Saved Successfully");
                        GetRole(0);
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
        }
    };


    function init() {        
        GetRole(0);
        $scope.RoleObj = new Object();
    }

    init();

});