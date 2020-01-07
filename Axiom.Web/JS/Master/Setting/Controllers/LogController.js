app.controller('LogController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, SettingServices, EmployeeServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.hasAccessToViewOthersLog = $scope.isSubModuleAccessibleToUser('EmployeeLog', 'EmployeeLog', 'View Other User Log');

    
    $scope.log = new Object();
     
    $scope.currentDate = $filter('date')(new Date(), $rootScope.GlobalDateFormat);
    $scope.nextDate = $filter('date')(new Date(), $rootScope.GlobalDateFormat);
    
    //$scope.currentDate = $filter('date')(new Date("2018-11-12"), $rootScope.GlobalDateFormat);
    //$scope.nextDate = $filter('date')(new Date(), $rootScope.GlobalDateFormat);
    $scope.log = { fromDate: $scope.currentDate, toDate: $scope.nextDate, UserId: "", DepartmentId: null };

    //Page Rights//

    if ($state.$current.name != "EmployeeLog") {
        $rootScope.CheckIsPageAccessible("Settings", "Log", "View");
    }
    else {
        $rootScope.CheckIsPageAccessible("EmployeeLog", "EmployeeLog", "View");
    }

    //------------



    //#region Binding
    function bindLogListTotable() {
     
        if ($.fn.DataTable.isDataTable("#tblEmployeeLog")) {
            $('#tblEmployeeLog').DataTable().destroy();
        }

        var table = $('#tblEmployeeLog').DataTable({
            data: $scope.logList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "autoWidth": false,
            "columns": [
                {
                    "title": "Employee",
                    "className": "dt-left",
                    "data": "EmployeeName",
                    "sorting": "false",
                    "width": "150px",
                    //"visible":false
                },
                {
                    "title": "Status",
                    "className": "dt-left",
                    "data": "InternalStatus",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return '<div class="orderstatus"><div class="' + row.StatusClass + '"></div><span class="' + row.StatusClass + '">' + row.StatusClass + ' </span></div>';
                    }

                },
                {
                    "title": "Note Date",
                    "className": "dt-left",
                    "data": "NoteDate",
                    "sorting": "false"
                },
                {
                    "title": "Order No",
                    "className": "dt-left",
                    "data": "OrderNo",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return '<a href="javascript:void(0)" title="Order No" class="cursor-pointer" ng-click="openOrderDetail(' + row.OrderNo + ')">' + row.OrderNo + '</a>';
                    }

                },
                {
                    "title": "Part No",
                    "className": "dt-left",
                    "data": "PartNo",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return '<a href="javascript:void(0)" title="Part No" class="cursor-pointer" ng-click="openPartDetail(' + row.PartNo + ',' + row.OrderNo + ')">' + row.PartNo + '</a>';
                    }
                },
                {
                    "title": "Records Of",
                    "className": "dt-left",
                    "data": "RecordOf",
                    "sorting": "false",
                },
                {
                    "title": "Location",
                    "className": "dt-left",
                    "data": "Location",
                    "sorting": "false",
                },
                {
                    "title": "Assigned To",
                    "className": "dt-left",
                    "data": "AssignedTo",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        if (row.AssignedTo) {
                            var strHtml = '<a href="mailto:' + row.AssignedTo + '?subject=' + row.OrderNo + '-' + row.PartNo + '">' + row.AssignedTo + '</a>';
                            return strHtml;
                        }
                        else {
                            return row.AssignedTo;
                        }
                    }
                },
                {
                    "title": "Status",
                    "className": "dt-left",
                    "data": "InternalStatus",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return '<div class="orderstatus"><span class="' + row.StatusClass + '">' + row.InternalStatus + '</span></div>';
                    }
                },
                {
                    "title": "Note",
                    "className": "dt-left",
                    "data": "Note",
                    "sorting": "false"
                },

                {
                    "title": "Action Date",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "ActionDate",

                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblEmployeeLog').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    function getLogList() {
         
        var toDate = new Date($scope.log.toDate);
        toDate.setDate(toDate.getDate() + 1);
        toDate = $filter('date')(toDate, $rootScope.GlobalDateFormat);
        var promise = SettingServices.GetLogList($scope.log.UserId, $scope.log.fromDate, toDate, $rootScope.CompanyNo);
        promise.success(function (response) {
            $scope.logList = response.Data;
            bindLogListTotable(); 
        });
        promise.error(function (data, statusCode)
        {
        });
    }

    $scope.showEmployeeLogList = function ($event) {
        var table = $('#tblEmployeeLogCount').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.log.UserId = row.UserId;

        $scope.showEmployeeLogCount = false;
        $scope.showOnlyLoginUserLog = true;
        getLogList(); 

    }

    function getDepartmentEmployeeList(userId) {

        //$scope.DepartmentList = [];
        $scope.DepartmentEmployeeList = [];
        var toDate = new Date($scope.log.toDate);
        toDate.setDate(toDate.getDate() + 1);
        toDate = $filter('date')(toDate, $rootScope.GlobalDateFormat);
        var emp = SettingServices.GetDepartmentEmpoyeeLog(userId, $scope.log.DepartmentId, $scope.log.fromDate, toDate, $rootScope.CompanyNo);
        emp.success(function (response) {

            
                $scope.DepartmentEmployeeList = response.Data;

                //var Obj = $filter('filter')($scope.DepartmentEmployeeList, { IsSelected: true }, true);
                //var DepartmentList = $filter('distinctArray')($scope.DepartmentEmployeeList, "DepartmentId,Department", null, null, true);

                setTimeout(function () { $('.cls-Department').selectpicker('refresh'); $('.cls-Department').selectpicker(); }, 500);

                BindEmployeeLogCountGrid();

                //setTimeout(function () {
                //    alert($scope.log.UserId);
                //    $("#ddl_assignedTo .dropdown-toggle").trigger('click');
                //    $("#ddl_assignedTo .dropdown-toggle").trigger('click');
                //    //$("#ddl_DepartmentId .dropdown-toggle").trigger('click');
                //    //$("#ddl_DepartmentId .dropdown-toggle").trigger('click');



                //}, 100);
                getLogList();
            
        });
        emp.error(function (data, statusCode) {
        });
    }

    $scope.Department_Change = function () {
        $scope.log.UserId = "";
        getDepartmentEmployeeList(null);
        $scope.showEmployeeLogCount = $scope.hasAccessToViewOthersLog;
        $scope.showOnlyLoginUserLog = !$scope.showEmployeeLogCount;
    }

    function BindEmployeeLogCountGrid() {

        lst = $filter('distinctArray')($scope.DepartmentEmployeeList, "UserId,EmployeeName,DepartmentId,LogCount", "DepartmentId", $scope.log.DepartmentId, true);
        setTimeout(function () {
            $('.cls-assignedTo').selectpicker('refresh');
            $('.cls-assignedTo').selectpicker();
        }, 500);  

        if ($.fn.DataTable.isDataTable("#tblEmployeeLogCount")) {
            $('#tblEmployeeLogCount').DataTable().destroy();
        }

        var table = $('#tblEmployeeLogCount').DataTable({
            data: lst,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "autoWidth": false,
            "columns": [
                
                {
                    "title": "Employee",
                    "className": "dt-left",
                    //"data": "EmployeeName",
                    "sorting": "false"
                    ,"render": function (data, type, row) {
                        var strHtml = '<a style="cursor: pointer;" ng-click="showEmployeeLogList($event)">' + row.EmployeeName + '</a>';
                        return strHtml;
                    } 

                },
                {
                    "title": "Logs",
                    "className": "dt-left",
                    "data": "LogCount",
                    "sorting": "false"
                    , "width":"50px"
                }  
            ],
            "initComplete": function () {
                var dataTable = $('#tblEmployeeLogCount').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    function BindDepartmentDropdown()
    {
        // DepartmentD  
        var _departmentlist = CommonServices.DepartmentDropdown();
        _departmentlist.success(function (response) { 
            $scope.DepartmentList = response.Data;
            if ($scope.DepartmentList != null && $scope.DepartmentList.length > 0)
            {
                $scope.log.DepartmentId = $scope.DepartmentList[0].DepartmentId;
                getDepartmentEmployeeList(null);
            }
        });
        _departmentlist.error(function (data, statusCode) { });
    }

    function init() { 
        $scope.showEmployeeLogCount = $scope.hasAccessToViewOthersLog;
        $scope.showOnlyLoginUserLog = !$scope.showEmployeeLogCount;

        $scope.EmployeeList = [];
        $scope.log = { fromDate: $scope.currentDate, toDate: $scope.currentDate, UserId: $rootScope.LoggedInUserDetail.UserId, DepartmentId: 0 };
        $scope.logList = [];
        
        if ($scope.showOnlyLoginUserLog) { 
            $scope.log.UserId = $rootScope.LoggedInUserDetail.UserId;
            getLogList();
        }
        else {
            BindDepartmentDropdown();
        } 
    }

    //#endregion


    //#region Events

    $scope.searchLogClick = function () {

        $scope.showEmployeeLogCount = $scope.hasAccessToViewOthersLog;
        $scope.showOnlyLoginUserLog = !$scope.showEmployeeLogCount;
        if ($scope.hasAccessToViewOthersLog) {
            getDepartmentEmployeeList(null)
        }
        else {
            getLogList();
        }
         
    }

    $scope.createDatePicker = function () {
        createDatePicker();
    };

    $scope.resetDataClick = function () {
        init();
    }

    $scope.openPartDetail = function (partNo, orderId) {
        var url = $state.href('PartDetail', { OrderId: orderId, PartNo: partNo });
        window.open(url, '_blank');
    }

    $scope.openOrderDetail = function (orderId) {
        var url = $state.href('OrderDetail', { OrderId: orderId });
        window.open(url, '_blank');
    }

    //#endregion

    init();
});