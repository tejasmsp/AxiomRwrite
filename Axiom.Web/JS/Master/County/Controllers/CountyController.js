app.controller('CountyController', function ($scope, $rootScope, $stateParams, notificationFactory, CountyServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "County", "View");
    $scope.IsUserCanEditCounty = $rootScope.isSubModuleAccessibleToUser('Settings', 'County', 'Edit County');
    $scope.IsUserCanDeleteCounty = $rootScope.isSubModuleAccessibleToUser('Settings', 'County', 'Delete County');
    //-----

    function bindCountyList() {

        if ($.fn.DataTable.isDataTable("#tblCounty")) {
            $('#tblCounty').DataTable().destroy();
        }

        var table = $('#tblCounty').DataTable({
            data: $scope.CountyList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "autoWidth": false,
            "columns": [
                {
                    "title": "County",
                    "className": "dt-left",
                    "data": "CountyName",
                    "width": "15%",
                    "sorting": "false"
                },
                {
                    "title": "State",
                    "className": "dt-left",
                    "data": "StateName",
                    "sorting": "false"
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
                        if ($scope.IsUserCanEditCounty) {
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='EditCounty($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }
                        if ($scope.IsUserCanDeleteCounty) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteCounty($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        }
                  
                       
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblCounty').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    $scope.EditCounty = function ($event) {
        var table = $('#tblCounty').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetCountyDetail(row.CountyId);
    };

    $scope.GetCountyDetail = function (CountyId) {

        var statelist = CommonServices.StateDropdown();
        statelist.success(function (response) {
            $scope.Statelist = response.Data;
        });
        statelist.error(function (data, statusCode) { });
        $scope.Countyform.$setPristine();
        if (CountyId > 0) {
            $scope.modal_Title = "Edit County";
            $scope.isEdit = true;
            var promise = CountyServices.GetCountyList(CountyId);
            promise.success(function (response) {
                $scope.CountyObj = response.Data[0];

                angular.element("#modal_County").modal('show');
            });
            promise.error(function (data, statusCode) {
            });
        } else {
            $scope.modal_Title = "Add County";
            $scope.isEdit = false;
            $scope.CountyObj = new Object();
            $scope.CountyObj.CountyName = "";
            $scope.CountyObj.StateId = "";
            $scope.CountyObj.IsActive = 0;
            angular.element("#modal_County").modal('show');
        }

    };

    $scope.AddOrUpdateCounty = function (form) {
        if (form.$valid) {
            var checkCounty = CountyServices.CheckUniqueCounty($scope.CountyObj);
            checkCounty.success(function (response) {
                if (response.Success) {
                    notificationFactory.customError("County Name must be unique.Please enter another county name");
                }
                else {
                    $scope.CountyObj.CreatedBy = $scope.UserAccessId;
                    if (!$scope.isEdit) {
                        var promise = CountyServices.InsertCounty($scope.CountyObj);
                        promise.success(function (response) {
                            if (response.Success) {
                                toastr.success('County saved successfully.');
                                angular.element("#modal_County").modal('hide');
                                GetCountyList();
                            }
                            else {
                                notificationFactory.customError(response.Message[0]);
                            }
                        });
                        promise.error(function (data, statusCode) {
                        });
                    }
                    else {
                        var promiseedit = CountyServices.UpdateCounty($scope.CountyObj);
                        promiseedit.success(function (response) {
                            if (response.Success) {
                                toastr.success("County updated successfully.");
                                angular.element("#modal_County").modal('hide');
                                GetCountyList();
                            }
                            else {
                                notificationFactory.customError(response.Message[0]);
                            }
                        });
                        promiseedit.error(function (data, statusCode) {
                        });
                    }
                }
            });

        }
    };

    $scope.DeleteCounty = function ($event) {

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
                    var table = $('#tblCounty').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = CountyServices.DeleteCounty(row.CountyId);
                    promise.success(function (response) {
                        if (response.Success) {
                            toastr.success('County deleted successfully.');
                            GetCountyList();
                        }
                        else {
                            notificationFactory.customError(response.Message[0]);
                        }
                    });
                    promise.error(function () { });
                }
                bootbox.hideAll();
            }
        });



    };

    function GetCountyList() {
        var promise = CountyServices.GetCountyList(0);
        promise.success(function (response) {
            $scope.CountyList = response.Data;
            bindCountyList();
        });
        promise.error(function (data, statusCode) {
        });
    }

    function init() {
        GetCountyList();
    }

    init();
});