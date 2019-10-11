app.controller('LogController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, SettingServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.UserGUID = $rootScope.LoggedInUserDetail.UserId;

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Log", "View");
    //------------



    //#region Binding
    function bindLogListTotable() {

        if ($.fn.DataTable.isDataTable("#tblLog")) {
            $('#tblLog').DataTable().destroy();
        }

        var table = $('#tblLog').DataTable({
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
                var dataTable = $('#tblLog').DataTable();
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
        $scope.nextDate = $filter('date')(toDate, $rootScope.GlobalDateFormat);
        var promise = SettingServices.GetLogList($scope.UserGUID, $scope.log.fromDate, $scope.nextDate, $rootScope.CompanyNo);
        promise.success(function (response) {
            $scope.logList = response.Data;
            bindLogListTotable();

        });
        promise.error(function (data, statusCode) {
        });
    }

    function init() {
        $scope.currentDate = $filter('date')(new Date(), $rootScope.GlobalDateFormat);
        $scope.log = { fromDate: $scope.currentDate, toDate: $scope.currentDate };
        $scope.logList = [];
        getLogList();
    }
    //#endregion


    //#region Events
    $scope.searchLogClick = function () {
        getLogList();
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