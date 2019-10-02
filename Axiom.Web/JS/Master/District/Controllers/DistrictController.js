app.controller('DistrictController', function ($scope, $rootScope, $stateParams, notificationFactory, DistrictServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "District", "View");
    $scope.IsUserCanEditDistrict = $rootScope.isSubModuleAccessibleToUser('Settings', 'District', 'Edit District');
    $scope.IsUserCanDeleteDistrict = $rootScope.isSubModuleAccessibleToUser('Settings', 'District', 'Delete District');
    //-----

    function bindDistrictList() {

        if ($.fn.DataTable.isDataTable("#tblDistrict")) {
            $('#tblDistrict').DataTable().destroy();
        }

        var table = $('#tblDistrict').DataTable({
            data: $scope.DistrictList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "District",
                    "className": "dt-left",
                    "data": "DistrictName",
                    "sorting": "false",
                    "width": "15%"
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
                        if ($scope.IsUserCanEditDistrict) {
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='EditDistrict($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }
                        if ($scope.IsUserCanDeleteDistrict) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteDistrict($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        }                                             
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblDistrict').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function GetDistrictList() {
        var promise = DistrictServices.GetDistrictList(0);
        promise.success(function (response) {
            $scope.DistrictList = response.Data;
            bindDistrictList();
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.EditDistrict = function ($event) {
        var table = $('#tblDistrict').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetDistrictDetail(row.DistrictID);
    };
    $scope.GetDistrictDetail = function (districtid) {

        var statelist = CommonServices.StateDropdown();
        statelist.success(function (response) {
            $scope.Statelist = response.Data;
        });
        statelist.error(function (data, statusCode) { });
        $scope.Districtform.$setPristine();
        if (districtid > 0) {
            $scope.modal_Title = "Edit District";
            $scope.isEdit = true;
            var promise = DistrictServices.GetDistrictList(districtid);
            promise.success(function (response) {
                $scope.DistrictObj = response.Data[0];
                angular.element("#modal_District").modal('show');
            });
            promise.error(function (data, statusCode) {
            });
        } else {
            $scope.modal_Title = "Add District";
            $scope.isEdit = false;
            $scope.DistrictObj = new Object();
            $scope.DistrictObj.DistrictName = "";
            $scope.DistrictObj.StateId = "";
            $scope.DistrictObj.IsActive = 0;
            angular.element("#modal_District").modal('show');
        }
    };

    $scope.AddOrUpdateDistrict = function (form) {

        if (form.$valid) {
            var checkDistrict = DistrictServices.CheckUniqueDistrict($scope.DistrictObj);
            checkDistrict.success(function (response) {
                if (response.Success) {
                    notificationFactory.customError("District Name must be unique.Please enter another district name");
                }
                else {
                    $scope.DistrictObj.CreatedBy = $scope.UserAccessId;
                    if (!$scope.isEdit) {
                        var promise = DistrictServices.InsertDistrict($scope.DistrictObj);
                        promise.success(function (response) {
                            if (response.Success) {
                                notificationFactory.successAdd("District");
                                angular.element("#modal_District").modal('hide');
                                GetDistrictList();
                            }
                            else {
                                notificationFactory.customError(response.Message[0]);
                            }
                        });
                        promise.error(function (data, statusCode) {
                        });
                    }
                    else {
                        var promiseedit = DistrictServices.UpdateDistrict($scope.DistrictObj);
                        promiseedit.success(function (response) {
                            if (response.Success) {
                                notificationFactory.customSuccess("District Update Successfully");
                                angular.element("#modal_District").modal('hide');
                                GetDistrictList();
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

    $scope.DeleteDistrict = function ($event) {
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
                    var table = $('#tblDistrict').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = DistrictServices.DeleteDistrict(row.DistrictID);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.successDelete('');
                            GetDistrictList();
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

    function init() {
        GetDistrictList();
    }
    init();

});