app.controller('AssignUniversalListController', function ($scope, $rootScope, $stateParams, RoleServices, notificationFactory, AssignUniversalListServices, configurationService, CommonServices, $compile, $filter) {
     
    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;

    
    //Page Rights//
     $rootScope.CheckIsPageAccessible('Settings', 'Assign Universal List', 'View');
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
                    "title": "",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsAssignTo",
                    "width": "40px",
                    "render": function (data, type, row) {
                        return (row.IsAssignTo ?
                            '<input type="checkbox" ng-click="UpdateAssignedtoStatus($event)" checked/>' : '<input type="checkbox" ng-click="UpdateAssignedtoStatus($event)"/>');
                    }
                },
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
                }
                , {
                    "title": 'Universal ?',
                    "data": "IsAssignTo",
                    "sClass": "action dt-center",
                    "width": "80px",
                    "sorting": "false",
                    "render": function (data, type, row) { 
                        if (data) {
                            return '<label class="label bg-success-400">Yes</label>';

                            //strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="DeleteMergeLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>'
                            //strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="AddMergeLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>'
                        }
                        else {
                            return '<label class="label bg-danger-400">No</label>';
                        } 
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
        var promise = AssignUniversalListServices.GetUniversalList();
        promise.success(function (response) {
            $scope.Employeelist = response.Data;
            bindEmployeeList();
        });
        promise.error(function (data, statusCode) {
        });
    } 
     
    function init() {
        GetEmployeelist(); 
    }

    $scope.UpdateAssignedtoStatus = function ($event) { 
        var table = $('#tblEmployee').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        var isAssigned = $event.target.checked;
         row.IsAssignTo = isAssigned;
        bootbox.confirm({
            message: "Are you sure you want to " + (isAssigned ? "assign" : "remove") + " this record " + (isAssigned ? "in" : "from") + " the universal assign-to-list?",
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
                    var promise = AssignUniversalListServices.UpdateUniversalStatus(row);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.customSuccess('User has been ' + (isAssigned ? "assigned" : "removed") + ' successfully.');
                            updateAssignToStatusHtml($event);
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function (response) { toastr.error(response.Message[0]); });
                }
                else {
                    $event.target.checked = !needtoMerge;
                    updateAssignToStatusHtml($event);
                }
                bootbox.hideAll();
                
            }
        });
    };
    function updateAssignToStatusHtml($event) {
        var checked = $event.target.checked;
        $($event.target).parents('tr').find('label.label').attr("class", "label " + (checked ? "bg-success-400" : "bg-danger-400"))
        $($event.target).parents('tr').find('label.label').html(checked ? "Yes" : "No");
    };
    init();

});