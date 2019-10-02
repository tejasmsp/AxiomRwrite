app.controller('CourtController', function ($scope, $rootScope, $stateParams, notificationFactory, CourtService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Court", "View");
    $scope.IsUserCanEditCourt = $rootScope.isSubModuleAccessibleToUser('Settings', 'Court', 'Edit Court');
    $scope.IsUserCanDeleteCourt = $rootScope.isSubModuleAccessibleToUser('Settings', 'Court', 'Delete Court');
    //-----

    function bindCourtList() {

        if ($.fn.DataTable.isDataTable("#tblCourt")) {
            $('#tblCourt').DataTable().destroy();
        }

        var table = $('#tblCourt').DataTable({
            data: $scope.CourtList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Name",
                    "className": "dt-left",
                    "data": "CourtName",
                    "sorting": "true",
                    "width": "15%"
                },                
                {
                    "title": "State",
                    "className": "dt-left",
                    "data": "StateName",
                    "sorting": "true",
                    "width": "15%"
                },
                {
                    "title": "District",
                    "className": "dt-left",
                    "data": "DistrictName",
                    "sorting": "true",
                    "width": "15%"
                },
                {
                    "title": "County",
                    "className": "dt-left",
                    "data": "CountyName",
                    "sorting": "true"
                },
                {
                    "title": "Type",
                    "className": "dt-center",
                    "data": "CourtType",
                    "sorting": "true",
                    "width": "7%"
                },
                {
                    "title": "Status",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsActive",
                    "width": "7%",
                    "render": function (data, type, row) {
                        return (row.IsActive) ? "<label class='label bg-success-400'>Active</label>" : "<label class='label bg-danger-400'>InActive</label>";
                    }
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "7%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if ($scope.IsUserCanEditCourt) {
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='EditCourt($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }
                        if ($scope.IsUserCanDeleteCourt) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteCourt($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        }                                              
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblService').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    $scope.isEdit = false;


    $scope.EditCourt = function ($event) {
        var table = $('#tblCourt').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetCourtDetail(row.CourtID);
    };

    $scope.GetCourtDetail = function (CourtID) {

        // STATE LIST FOR DROPDOWN
        var statelist = CommonServices.StateDropdown();
        statelist.success(function (response) {
            $scope.Statelist = response.Data;
        });
        statelist.error(function (data, statusCode) { });

        // DISTRICT LIST FOR DROPDOWN
        var districtlist = CommonServices.DistrictDropdown('');
        districtlist.success(function (response) {
            $scope.DistrictList = response.Data;
        });
        districtlist.error(function (data, statusCode) { });

        // COUNTY LIST FOR DROPDOWN
        var countylist = CommonServices.CountyDropdown('');
        countylist.success(function (response) {
            $scope.CountyList = response.Data;
        });
        countylist.error(function (data, statusCode) { });
        $scope.Courtform.$setPristine();
        if (CourtID > 0) {
            $scope.modal_Title = "Edit Court";
            $scope.isEdit = true;
            var promise = CourtService.GetCourtList(CourtID);
            promise.success(function (response) {
                $scope.objCourt = response.Data[0];

                angular.element("#modal_Court").modal('show');
            });
            promise.error(function (data, statusCode) {
            });
        } else {
            $scope.modal_Title = "Add Court";
            $scope.objCourt = new Object();
            $scope.objCourt.CourtName = "";
            $scope.objCourt.CourtType = "";

            $scope.objCourt.StateID = "";
            $scope.objCourt.StateName = "";

            $scope.objCourt.DistrictID = "";
            $scope.objCourt.DistrictName = "";

            $scope.objCourt.CountyID = "";
            $scope.objCourt.CountyName = "";

            $scope.objCourt.IsActive = 0;
            angular.element("#modal_Court").modal('show');
        }

    };

    $scope.AddOrUpdateCourt = function (form) {

        if (form.$valid) {
            if (!$scope.isEdit) {
                $scope.objCourt.CreatedBy = $rootScope.LoggedInUserDetail.UserId;
                var promise = CourtService.InsertCourt($scope.objCourt);
                promise.success(function (response) {
                    if (response.Success) {
                        toastr.success('Court saved successfully');
                        angular.element("#modal_Court").modal('hide');
                        GetCourtList();
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else {
                var promiseedit = CourtService.UpdateCourt($scope.objCourt);
                promiseedit.success(function (response) {
                    if (response.Success) {
                        toastr.success('Court updated successfully');
                        angular.element("#modal_Court").modal('hide');
                        GetCourtList();
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

    $scope.DeleteCourt = function ($event) {

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
                
                if (result == true) {
                    var table = $('#tblCourt').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = CourtService.DeleteCourt(row.CourtID);
                    promise.success(function (response) {
                        if (response.Success) {
                            toastr.success('Court deleted successfully.');
                            GetCourtList();
                            bootbox.hideAll()
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

    function GetCourtList() {
        var promise = CourtService.GetCourtList(0);
        promise.success(function (response) {
            $scope.CourtList = response.Data;
            bindCourtList();
        });
        promise.error(function (data, statusCode) {
        });
    }
    function init() {
        GetCourtList();
    }
    init();
});