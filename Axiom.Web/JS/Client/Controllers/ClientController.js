app.controller('ClientController', function ($window, $scope, $state, $rootScope, $stateParams, notificationFactory, ClientServices, CommonServices, configurationService, $compile, $filter) {


    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.UserGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.RoleName = $rootScope.LoggedInUserDetail.RoleName;


    decodeParams($stateParams);

    $scope.isEdit = false;

    $scope.ShowOrderPage = function (OrderNo) {

        $state.go("OrderDetail", { OrderId: OrderNo });
    };

    //#region Events
    $scope.GetAnnouncementDetail = function () {
        $scope.strAnnouncement = "";
        var promise = AnnouncementServices.GetDailyAnnouncement();
        promise.success(function (response) {
            if (response.Success) {
                $scope.strAnnouncement = response.str_ResponseData;
            }
            angular.element("#modal_Announcement").modal('show');
        });
        promise.error(function (data, statusCode) {

        });

    };

    $scope.OrderSearch = function () {
        if (!isNullOrUndefinedOrEmpty($scope.SearchParameter)) {
            $window.open("OrderList?Search=" + $scope.SearchParameter);
        }
    };
    $scope.AddOrUpdateAnnouncement = function (form) {
        if (form.$valid) {
            var promise = AnnouncementServices.InsertDailyAnnouncement($scope.strAnnouncement);
            promise.success(function (response) {
                if (response.Success) {
                    notificationFactory.successAdd("Announcement");
                    angular.element("#modal_Announcement").modal('hide');
                    $scope.announcement = $scope.strAnnouncement;
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) {
            });
        }
    };

    function bindActionDateStatusList() {

        if ($.fn.DataTable.isDataTable("#tblActionStatus")) {
            $('#tblActionStatus').DataTable().destroy();
        }

        var table = $('#tblActionStatus').DataTable({
            data: $scope.DataList,
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
                    "data": "Department"
                },
                {
                    "title": "Name",
                    "className": "dt-left",
                    "render": function (data, type, row) {
                        return row.LastName + ' , ' + row.FirstName;
                    }
                },
                {
                    "title": "EmpId",
                    "className": "dt-left",
                    "data": "EmpId"
                },
                {
                    "title": "OldestDaysOld",
                    "className": "dt-left",
                    "data": "OldestDaysOld"
                },
                {
                    "title": "OldestDate",
                    "className": "dt-left",
                    "data": "OldestDate"
                },
                {
                    "title": "TwoWeeksRush",
                    "className": "dt-left",
                    "data": "TwoWeeksRush"
                },
                {
                    "title": "TwoWeeks",
                    "className": "dt-left",
                    "data": "TwoWeeks"
                },
                {
                    "title": "OneWeekRush",
                    "className": "dt-left",
                    "data": "OneWeekRush"
                },
                {
                    "title": "OneWeek",
                    "className": "dt-left",
                    "data": "OneWeek"
                },

                {
                    "title": "TwoDaysRush",
                    "className": "dt-left",
                    "data": "TwoDaysRush"
                },
                {
                    "title": "TwoDays",
                    "className": "dt-left",
                    "data": "TwoDays"
                },
                {
                    "title": "CurrentRush",
                    "className": "dt-left",
                    "data": "CurrentRush"
                },
                {
                    "title": "Current",
                    "className": "dt-left",
                    "data": "Current"
                },

                {
                    "title": "TotalRush",
                    "className": "dt-left",
                    "data": "TotalRush"
                },
                {
                    "title": "Total",
                    "className": "dt-left",
                    "data": "Total"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer'  ng-click='GetAttorneyUserForEditMode($event)' data-toggle='tooltip' data-placement='top' tooltip title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        if (row.IsApproved)
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='ActivateInactiveAttorneyUser($event)'  data-toggle='tooltip' data-placement='top' tooltip title='Lock'>  <i  class='icon-lock cursor-pointer'></i> </a>";
                        else
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='ActivateInactiveAttorneyUser($event)'  data-toggle='tooltip' data-placement='top' tooltip title='Unlock'>  <i  class='icon-key cursor-pointer'></i> </a>";

                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblActionStatus').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    function GetActionDateStatuslist() {

        var promise = HomeServices.GetActionDateStatus();
        promise.success(function (response) {
            if (response.Success) {
                $scope.DataList = response.Data;
                // bindActionDateStatusList();
                //$scope.addupdateform.$setPristine();
            }
            else {
                $scope.announcement = "";
            }
        });
        promise.error(function (data, statusCode) { });
    };

    $scope.DownloadRecords = function (type) {

        //var strHTML = String(angular.element("#invoiceTable").html()).replace(/[&<>"'`=\/]/g, function (s) {
        //    return entityMap[s];
        //});
        $scope.PostData = new Object();
        var strHTML;
        if (type == "Authorization") {
            var clientReport = ClientServices.ClientPartReport($scope.UserGuid,"authorization");
            //$scope.AuthorizationNeeded = removeDuplicateValue($scope.AuthorizationNeeded);
            //$scope.PostData = $scope.AuthorizationNeeded;
        }
        else if (type == "InProgress") {
            var clientReport = ClientServices.ClientPartReport($scope.UserGuid, "inprogress");

            //$scope.InProgress = removeDuplicateValue($scope.InProgress);
            //$scope.PostData = $scope.InProgress;
        }
        else if (type == "NewRecords") {
            var clientReport = ClientServices.ClientPartReport($scope.UserGuid, "newrecords");
            //$scope.NewRecords = removeDuplicateValue($scope.NewRecords)
            //$scope.PostData = $scope.NewRecords;
        }
        else if (type == "MoreInformation") {
            var clientReport = ClientServices.ClientPartReport($scope.UserGuid, "moreinformation");
            // $scope.MoreInformation = removeDuplicateValue($scope.MoreInformation);
            // $scope.PostData = $scope.MoreInformation
        }
        // var invoice = ClientServices.DownloadClientRecords($scope.PostData);
    }

    //#endregion 

    //#region Methods
    function removeDuplicateValue(myArray) {
        var newArray = [];

        angular.forEach(myArray, function (value, key) {
            var exists = false;
            angular.forEach(newArray, function (val2, key) {
                if (angular.equals(value.OrderNo, val2.OrderNo) && angular.equals(value.PartStatusGroupId, val2.PartStatusGroupId)) { exists = true };
            });
            if (exists == false) { newArray.push(value); }
        });

        return newArray;
    }

    function GetClientDashboardOrders() {
        var promise = ClientServices.GetClientDashboardOrders($scope.UserGuid);
        promise.success(function (response) {
            if (response.Success) {
                $scope.ClientOrders = response.Data;
                response.Data = removeDuplicateValue(response.Data);

                $scope.AuthorizationNeeded = $filter('filter')(response.Data, { 'PartStatusGroupId': 2 }, true);
                $scope.InProgress = $filter('filter')(response.Data, { 'PartStatusGroupId': 1 }, true);
                $scope.NewRecords = $filter('filter')(response.Data, { 'PartStatusGroupId': 5 }, true);
                $scope.MoreInformation = $filter('filter')(response.Data, { 'PartStatusGroupId': 4 }, true);
                bindTblAuthorization();
                bindTblInProgress();
                bindTblNewRecord();
                bindTblMoreInfo();
            }
            else {
                $scope.ClientOrders = [];
            }
        });
        promise.error(function (data, statusCode) {
        });
    }
    function GetClientParts() {
        var promise = ClientServices.GetClientPartStatus($scope.UserGuid);
        promise.success(function (response) {
            if (response.Success) {
                $scope.ClientPart = response.Data;
                $scope.AuthorizationNeeded = $filter('filter')(response.Data, { 'PartStatusGroupId': 2 }, true);
                $scope.InProgress = $filter('filter')(response.Data, { 'PartStatusGroupId': 1 }, true);
                $scope.NewRecords = $filter('filter')(response.Data, { 'PartStatusGroupId': 5 }, true);
                $scope.MoreInformation = $filter('filter')(response.Data, { 'PartStatusGroupId': 4 }, true);
            }
            else {
                $scope.ClientPart = [];
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.FilterRecordsByDays = function (PartStatusGroupId, day) {        
        $scope.Temp = $filter('filter')($scope.ClientOrders, { 'PartStatusGroupId': PartStatusGroupId }, true);
        var FiteredRecord = [];
        if (day == 0) {
            $scope.Temp = removeDuplicateValue($scope.Temp);            
            FiteredRecord = angular.copy($scope.Temp);
        }
        else if (day == 30) {
            FiteredRecord = [];
            angular.forEach($scope.Temp, function (value, key) {
                if (value.OrderDays >= 30 && value.OrderDays <= 60) {
                    FiteredRecord.push($scope.Temp[key]);
                }
            });
        }
        else if (day == 60) {
            FiteredRecord = [];
            angular.forEach($scope.Temp, function (value, key) {
                if (value.OrderDays >= 60 && value.OrderDays <= 90) {
                    FiteredRecord.push($scope.Temp[key]);

                }
            });
        }
        else if (day == 90) {
            FiteredRecord= [];
            angular.forEach($scope.Temp, function (value, key) {
                if (value.OrderDays >= 90) {
                    FiteredRecord.push($scope.Temp[key]);
                }
            });
        }

        if (PartStatusGroupId == 2) {
            $scope.AuthorizationNeeded = FiteredRecord;
            bindTblAuthorization();
        }
        else if (PartStatusGroupId == 1) {
            $scope.InProgress = FiteredRecord;
            bindTblInProgress();
        }
        else if (PartStatusGroupId == 5) {
            $scope.NewRecords = FiteredRecord;
            bindTblNewRecord();
        }
        else if (PartStatusGroupId == 4) {
            $scope.MoreInformation = FiteredRecord;
            bindTblMoreInfo();
        }
    }

    //$scope.FilterRecordsAuthorization = function (day) {
    //    $scope.Temp = $filter('filter')($scope.ClientOrders, { 'PartStatusGroupId': 2 }, true);
    //    if (day == 0) {
    //        $scope.Temp = removeDuplicateValue($scope.Temp);
    //        $scope.AuthorizationNeeded = angular.copy($scope.Temp);
    //    }
    //    else if (day == 30) {
    //        $scope.AuthorizationNeeded = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays >= 30 && value.OrderDays <= 60) {
    //                $scope.AuthorizationNeeded.push($scope.Temp[key]);
    //            }
    //        });
    //    }
    //    else if (day == 60) {
    //        $scope.AuthorizationNeeded = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays >= 60 && value.OrderDays <= 90) {
    //                $scope.AuthorizationNeeded.push($scope.Temp[key]);

    //            }
    //        });
    //    }
    //    else if (day == 90) {
    //        $scope.AuthorizationNeeded = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays >= 90) {
    //                $scope.AuthorizationNeeded.push($scope.Temp[key]);
    //            }
    //        });
    //    }
    //    bindTblAuthorization();
    //};

    //$scope.FilterRecordsInPorgress = function (day) {
    //    $scope.Temp = $filter('filter')($scope.ClientOrders, { 'PartStatusGroupId': 1 }, true);
    //    if (day == 0) {
    //        $scope.Temp = removeDuplicateValue($scope.Temp);
    //        $scope.InProgress = angular.copy($scope.Temp);
    //    }
    //    else if (day == 30) {
    //        $scope.InProgress = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays > 30 && value.OrderDays <= 60) {
    //                $scope.InProgress.push($scope.Temp[key]);
    //            }
    //        });
    //    } else if (day == 60) {
    //        $scope.InProgress = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays > 60 && value.OrderDays <= 90) {
    //                $scope.InProgress.push($scope.Temp[key]);
    //            }
    //        });
    //    } else if (day == 90) {
    //        $scope.InProgress = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays > 90) {
    //                $scope.InProgress.push($scope.Temp[key]);
    //            }
    //        });
    //    }
    //    bindTblInProgress();
    //};

    //$scope.FilterNewRecords = function (day) {
    //    $scope.Temp = $filter('filter')($scope.ClientOrders, { 'PartStatusGroupId': 5 }, true);
    //    if (day == 0) {
    //        $scope.Temp = removeDuplicateValue($scope.Temp);
    //        $scope.NewRecords = angular.copy($scope.Temp);
    //    }
    //    else if (day == 30) {
    //        $scope.NewRecords = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays > 30 && value.OrderDays <= 60) {
    //                $scope.NewRecords.push($scope.Temp[key]);
    //            }
    //        });
    //    }
    //    else if (day == 60) {
    //        $scope.NewRecords = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays > 60 && value.OrderDays <= 90) {
    //                $scope.NewRecords.push($scope.Temp[key]);
    //            }
    //        });
    //    }
    //    else if (day == 90) {
    //        $scope.NewRecords = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays > 90) {
    //                $scope.NewRecords.push($scope.Temp[key]);
    //            }
    //        });
    //    }
    //    bindTblNewRecord();
    //}

    //$scope.FilterMoreInfomation = function (day) {
    //    $scope.Temp = $filter('filter')($scope.ClientOrders, { 'PartStatusGroupId': 4 }, true);
    //    if (day == 0) {
    //        $scope.Temp = removeDuplicateValue($scope.Temp);
    //        $scope.MoreInformation = angular.copy($scope.Temp);
    //    }
    //    else if (day == 30) {
    //        $scope.MoreInformation = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays > 30 && value.OrderDays <= 60) {
    //                $scope.MoreInformation.push($scope.Temp[key]);
    //            }
    //        });
    //    }
    //    else if (day == 60) {
    //        $scope.MoreInformation = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays > 60 && value.OrderDays <= 90) {
    //                $scope.MoreInformation.push($scope.Temp[key]);
    //            }
    //        });
    //    }
    //    else if (day == 90) {
    //        $scope.MoreInformation = [];
    //        angular.forEach($scope.Temp, function (value, key) {
    //            if (value.OrderDays > 90) {
    //                $scope.MoreInformation.push($scope.Temp[key]);
    //            }
    //        });
    //    }
    //    bindTblMoreInfo();
    //}

    $scope.selectedAuthtableRow = function ($event) {
        var table = $('#tblAuthorization').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        OpenPartModal(row.OrderNo, row.PartStatusGroupId);

    }
    $scope.selectedInProgresstableRow = function ($event) {
        var table = $('#tblInProgress').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        OpenPartModal(row.OrderNo, row.PartStatusGroupId);

    }
    $scope.selectedNewRectableRow = function ($event) {
        var table = $('#tblNewRecords').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        OpenPartModal(row.OrderNo, row.PartStatusGroupId);

    }
    $scope.selectedMoreInfotableRow = function ($event) {
        var table = $('#tblMoreInformation').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        OpenPartModal(row.OrderNo, row.PartStatusGroupId);

    }
    function OpenPartModal(OrderNo, PartStatusGroupId) {
        $scope.SelectedOrderNo = OrderNo;
        $scope.ShowOrderPartList = true;
        $scope.PartStatusGroupId = PartStatusGroupId;
        angular.element("#modal_DashboardOrderPartForm").modal('show');
    }

    function bindTblAuthorization() {
        if ($.fn.DataTable.isDataTable("#tblAuthorization")) {
            $('#tblAuthorization').DataTable().destroy();
        }

        var table = $('#tblAuthorization').DataTable({
            data: $scope.AuthorizationNeeded,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "searching": false,
            "bSortable": true,
            "autoWidth": false,
            "aaSorting": [[0, 'desc']],
            "bPaginate": false,
            "bInfo": false,
            "columns": [
                {
                    "title": "Order No",
                    "className": "dt-left",
                    "data": "OrderNo",
                    "bSortable": true,
                    "width": "15%",
                    "render": function (data, type, row) {
                        return '<span  data-placement="right" tooltip title="Click to show Part Detail" ng-click="selectedAuthtableRow($event)">' + row.OrderNo + '</span>'
                    }
                },
                {
                    "title": "Matter#",
                    "className": "dt-left",
                    "data": "CliMatNo",
                    "bSortable": true,
                    "width": "20%",
                    "render": function (data, type, row) {
                        return '<span title="Click to show Part Detail" ng-click="selectedAuthtableRow($event)">' + row.CliMatNo + '</span>';
                    }
                },
                {
                    "title": "Records Of",
                    "className": "dt-left",
                    "data": "RecordsOf",
                    "bSortable": true,
                    "render": function (data, type, row) {
                        return '<span class="cursor-pointer" data-toggle="tooltip" data-placement="top" tooltip title="Click to show Part Detail" ng-click="selectedAuthtableRow($event)">' + row.RecordsOf + '</span>';
                    }
                },
                {
                    "title": "Claim No",
                    "className": "dt-left",
                    "data": "BillingClaimNo",
                    "bSortable": true,
                    "render": function (data, type, row) {
                        return '<span class="cursor-pointer" data-toggle="tooltip" data-placement="top" tooltip title="Click to show Part Detail" ng-click="selectedAuthtableRow($event)">' + row.BillingClaimNo + '</span>';
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblAuthorization').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).find("td:not(:first-child)").attr("ng-click", "selectedAuthtableRow($event)");
                $compile(angular.element(nRow))($scope);

            }
        });

    }

    function bindTblInProgress() {
        if ($.fn.DataTable.isDataTable("#tblInProgress")) {
            $('#tblInProgress').DataTable().destroy();
        }

        var table = $('#tblInProgress').DataTable({
            data: $scope.InProgress,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "searching": false,
            "bSortable": true,
            "autoWidth": false,
            "aaSorting": [[0, 'desc']],
            "bPaginate": false,
            "bInfo": false,
            "columns": [
                {
                    "title": "Order No",
                    "className": "dt-left",
                    "data": "OrderNo",
                    "bSortable": true,
                    "width": "15%",
                    "render": function (data, type, row) {
                        return '<span   data-placement="right" tooltip title="Click to show Part Detail" ng-click="selectedInProgresstableRow($event)">' + row.OrderNo + '</span>'
                    }
                },
                {
                    "title": "Matter#",
                    "className": "dt-left",
                    "data": "CliMatNo",
                    "bSortable": true,
                    "width": "20%",
                    "render": function (data, type, row) {
                        return '<span data-toggle="tooltip" data-placement="top" tooltip title="Click to show Part Detail" ng-click="selectedInProgresstableRow($event)">' + row.CliMatNo + '</span>';
                    }
                },
                {
                    "title": "Records Of",
                    "className": "dt-left",
                    "data": "RecordsOf",
                    "bSortable": true,
                    "render": function (data, type, row) {
                        return '<span title="Click to show Part Detail" ng-click="selectedInProgresstableRow($event)">' + row.RecordsOf + '</span>';
                    }
                },
                {
                    "title": "Claim No",
                    "className": "dt-left",
                    "data": "BillingClaimNo",
                    "bSortable": true,
                    "render": function (data, type, row) {
                        return '<span class="cursor-pointer" data-toggle="tooltip" data-placement="top" tooltip title="Click to show Part Detail" ng-click="selectedInProgresstableRow($event)">' + row.BillingClaimNo + '</span>';
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblInProgress').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).find("td:not(:first-child)").attr("ng-click", "selectedInProgresstableRow($event)");
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function bindTblNewRecord() {
        if ($.fn.DataTable.isDataTable("#tblNewRecords")) {
            $('#tblNewRecords').DataTable().destroy();
        }

        var table = $('#tblNewRecords').DataTable({
            data: $scope.NewRecords,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "searching": false,
            "bSortable": true,
            "autoWidth": false,
            "aaSorting": [[0, 'desc']],
            "bPaginate": false,
            "bInfo": false,
            "columns": [
                {
                    "title": "Order No",
                    "className": "dt-left",
                    "data": "OrderNo",
                    "bSortable": true,
                    "width": "15%",
                    "render": function (data, type, row) {
                        return "<a data-toggle='tooltip' data-placement='right' tooltip title='Click to open Order Detail page' href='OrderDetail?OrderId=" + row.OrderNo + "' target='_blank'>" + row.OrderNo + "</a>"
                    }
                },
                {
                    "title": "Matter#",
                    "className": "dt-left",
                    "data": "CliMatNo",
                    "bSortable": true,
                    "width": "20%",
                    "render": function (data, type, row) {
                        return '<span title="Click to show Part Detail" ng-click="selectedNewRectableRow($event)">' + row.CliMatNo + '</span>';
                    }
                },
                {
                    "title": "Records Of",
                    "className": "dt-left",
                    "data": "RecordsOf",
                    "bSortable": true,
                    "render": function (data, type, row) {
                        return '<span title="Click to show Part Detail" ng-click="selectedNewRectableRow($event)">' + row.RecordsOf + '</span>';
                    }
                },
                {
                    "title": "Claim No",
                    "className": "dt-left",
                    "data": "BillingClaimNo",
                    "bSortable": true,
                    "render": function (data, type, row) {
                        return '<span class="cursor-pointer" data-toggle="tooltip" data-placement="top" tooltip title="Click to show Part Detail" ng-click="selectedNewRectableRow($event)">' + row.BillingClaimNo + '</span>';
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblNewRecords').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).find("td:not(:first-child)").attr("ng-click", "selectedNewRectableRow($event)");
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function bindTblMoreInfo() {
        if ($.fn.DataTable.isDataTable("#tblMoreInformation")) {
            $('#tblMoreInformation').DataTable().destroy();
        }

        var table = $('#tblMoreInformation').DataTable({
            data: $scope.MoreInformation,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "searching": false,
            "bSortable": true,
            "autoWidth": false,
            "aaSorting": [[0, 'desc']],
            "bPaginate": false,
            "bInfo": false,
            "columns": [
                {
                    "title": "Order No",
                    "className": "dt-left",
                    "data": "OrderNo",
                    "bSortable": true,
                    "width": "15%",
                    "render": function (data, type, row) {
                        return "<a data-toggle='tooltip' data-placement='right' tooltip title='Click to open Order Detail page' href='OrderDetail?OrderId=" + row.OrderNo + "' target='_blank'>" + row.OrderNo + "</a>"
                    }
                },
                {
                    "title": "Matter#",
                    "className": "dt-left",
                    "data": "CliMatNo",
                    "bSortable": true,
                    "width": "20%",
                    "render": function (data, type, row) {
                        return '<span title="Click to show Part Detail" ng-click="selectedMoreInfotableRow($event)">' + row.CliMatNo + '</span>';
                    }
                },
                {
                    "title": "Records Of",
                    "className": "dt-left",
                    "data": "RecordsOf",
                    "bSortable": true,
                    "render": function (data, type, row) {
                        return '<span title="Click to show Part Detail" ng-click="selectedMoreInfotableRow($event)">' + row.RecordsOf + '</span>';
                    }
                },
                {
                    "title": "Claim No",
                    "className": "dt-left",
                    "data": "BillingClaimNo",
                    "bSortable": true,
                    "render": function (data, type, row) {
                        return '<span class="cursor-pointer" data-toggle="tooltip" data-placement="top" tooltip title="Click to show Part Detail" ng-click="selectedMoreInfotableRow($event)">' + row.BillingClaimNo + '</span>';
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblMoreInformation').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).find("td:not(:first-child)").attr("ng-click", "selectedMoreInfotableRow($event)");
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }
    function init() {


        GetClientDashboardOrders();
        //GetClientParts();
        // GetAnnouncement();
        // GetActionDateStatuslist();
        //alert($scope.UserAccessId);
        //alert($scope.UserGuid);
        //alert($scope.EmpId);
        //alert($scope.RoleName);


    }
    //#endregion 

    init();
});