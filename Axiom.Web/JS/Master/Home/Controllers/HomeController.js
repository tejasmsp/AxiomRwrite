app.controller('HomeController', function ($scope, $window, $rootScope, $stateParams, notificationFactory, AnnouncementServices, HomeServices, CommonServices, configurationService, $compile, $filter) {
   
    decodeParams($stateParams);

    $scope.isEdit = false;

    
    //#region Events
    $scope.GetAnnouncementDetail = function () {
        $scope.strAnnouncement ="";
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
                        strAction = "<a class='ico_btn cursor-pointer'  ng-click='GetAttorneyUserForEditMode($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        if (row.IsApproved)
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='ActivateInactiveAttorneyUser($event)'  title='Lock'>  <i  class='icon-lock cursor-pointer'></i> </a>";
                        else
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='ActivateInactiveAttorneyUser($event)'  title='Unlock'>  <i  class='icon-key cursor-pointer'></i> </a>";

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

    function GetActionDateStatuslist()  {
        
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


    
    //#endregion 

    //#region Methods

    function GetAnnouncement() {
        var promise = AnnouncementServices.GetDailyAnnouncement();
        promise.success(function (response) {
            if (response.Success) {
                $scope.announcement = response.str_ResponseData;
            }
            else {
                $scope.announcement = "";
            }
        });
        promise.error(function (data, statusCode) {

        });
    }

    $scope.SearchOrder = function (SearchType, Value, IsRush, Item) {
        var startDate = '';
        var endDate = '';
        var todayDate = new Date();
        var start_Date = "";        
        switch (SearchType) {            
            case $rootScope.Enum.DailyDashboardSearch.AccountExecutive:
                $window.open("SearchOrderList?EmpId=" + Item.EmpId + "&IsCallBack=1&IsFromDailyAnnouncement=1", '_blank');                  
                break;
            case $rootScope.Enum.DailyDashboardSearch.Date:
                startDate = Value;
                $window.open("SearchOrderList?FromDate=" + startDate + "&EndDate=" + startDate + "&IsRush=" + IsRush + "&EmpId=" + Item.EmpId +"&IsCallBack=1&IsFromDailyAnnouncement=1", '_blank');                
                break;
            case $rootScope.Enum.DailyDashboardSearch.Week:
                var days = (7 * Value);                 
                 start_Date = todayDate.setDate(todayDate.getDate() - days);
                 endDate = $filter('date')(start_Date, $rootScope.GlobalDateFormat);                
                if (Value==2) {
                    $window.open("SearchOrderList?EndDate=" + endDate + "&IsRush=" + IsRush + "&EmpId=" + Item.EmpId + "&IsCallBack=1&IsFromDailyAnnouncement=1", '_blank');   
                }
                if (Value == 1) {                    
                    var today = new Date();
                    start_Date = today.setDate(today.getDate() - 13);
                    startDate = $filter('date')(start_Date, $rootScope.GlobalDateFormat);
                    $window.open("SearchOrderList?FromDate=" + startDate + "&EndDate=" + endDate + "&IsRush=" + IsRush + "&EmpId=" + Item.EmpId + "&IsCallBack=1&IsFromDailyAnnouncement=1", '_blank');  
                }                            
                break;
            case $rootScope.Enum.DailyDashboardSearch.Day:                
                if (Value == 2) {
                    start_Date = todayDate.setDate(todayDate.getDate() - 7);
                    startDate = $filter('date')(start_Date, $rootScope.GlobalDateFormat);
                    var today = new Date();
                    var End_Date = today.setDate(today.getDate() - 1);
                    endDate = $filter('date')(End_Date, $rootScope.GlobalDateFormat);
                    $window.open("SearchOrderList?FromDate=" + startDate + "&EndDate=" + endDate + "&IsRush=" + IsRush + "&EmpId=" + Item.EmpId + "&IsCallBack=1&IsFromDailyAnnouncement=1", '_blank');                
                }
                if (Value == 0) {
                    var today = new Date();
                    var start_Date = today.setDate(today.getDate());
                    startDate = $filter('date')(start_Date, $rootScope.GlobalDateFormat);                    
                    $window.open("SearchOrderList?FromDate=" + startDate + "&IsRush=" + IsRush + "&EmpId=" + Item.EmpId + "&IsCallBack=1&IsFromDailyAnnouncement=1", '_blank');                
                }                              
                break;
            case $rootScope.Enum.DailyDashboardSearch.Total:
                $window.open("SearchOrderList?IsRush=" + IsRush + "&EmpId=" + Item.EmpId + "&IsCallBack=1&IsFromDailyAnnouncement=1", '_blank');                
                break;
        }        

    };

    
    function init() {
        
        GetAnnouncement();
        GetActionDateStatuslist();
        
    }
    //#endregion 

    init();
});