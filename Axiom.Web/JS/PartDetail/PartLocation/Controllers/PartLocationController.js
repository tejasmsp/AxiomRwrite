app.controller('PartLocationController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, Step6Service, CommonServices, OrderPartService, configurationService, $compile, $filter) {
    decodeParams($stateParams);
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.LocationID = "";
    $scope.IsLocSelect = false;
    $scope.RoleName = $rootScope.LoggedInUserDetail.RoleName ? $rootScope.LoggedInUserDetail.RoleName[0] : '';
    $scope.searchCriteria = 'Both';
    $scope.searchCondition = 1;
    //#region Event

    $scope.GetPartListByOrderId = function () {
        var promise = OrderPartService.GetPartListByOrderId($scope.OrderId, $scope.PartNo);
        promise.success(function (response) {
            $scope.OrderPartObj = response.Data[0];
            $scope.$parent.MasterLocId = angular.copy($scope.OrderPartObj.LocID);
            // bindOrderPartList();
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.EditPartPopUp = function () {                
        $scope.IsLocSelect = true;
        $scope.OrderPartLocationform.$setPristine();
        $scope.GetPartListByOrderId();
        //$scope.OrderPartLocationform = {};
        
        angular.element("#modal_Edit_Part").modal('show');

    };

    $scope.GetLocationSearch = function () {        
        angular.element("#modal_Location_Search").modal('show');
        $("#SearchText").focus();
        angular.element("#modal_Location_Search").find("input[type=text],input[type=search]").val('');
    };

    $scope.SearchLocation = function () {
        
        if (!isNullOrUndefinedOrEmpty($scope.searchText)) {
            if ($scope.searchText.length < 2) {
                toastr.error("Minimun two charater Required");
                return;
            }
            var promise = Step6Service.GetLocationSearchFromPart($scope.searchCriteria, $scope.searchCondition, $scope.searchText);
            promise.success(function (response) {
                
                $scope.searchLocationList = response.Data;
                bindLocationSerach();
            });
            promise.error(function (data, statusCode) {
            });
        }
        else {
            toastr.error("Search Text Required");
        }
        $scope.ShowAddNewLocation = true;
    };

    function bindLocationSerach() {
        if ($.fn.DataTable.isDataTable("#tblSearchLocationGrid")) {
            $('#tblSearchLocationGrid').DataTable().destroy();
        }
        var table = $('#tblSearchLocationGrid').DataTable({
            data: $scope.searchLocationList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "oLanguage": {
                "sSearch": "Refine Search:"
            },
            "columns": [
                //{
                //    "title": "Location Id",
                //    "className": "dt-left",
                //    "data": "LocID",
                //    "sorting": "false",
                //    "width": "10%"
                //},
                {
                    "title": "Location",
                    "className": "dt-left",
                    //"data": "Name1",                    
                    "sorting": "false",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = row.Name1 + "</br>" + row.Street1 + ' ' + row.Street2 + ' <b>' + row.City + '</b> ' + '<b>' + row.State + '</b>';
                        return strAction;
                    }
                },
                {
                    "title": "Doctor",
                    "className": "dt-left",
                    "data": "Name2",
                    "sorting": "false"
                },
                {
                    "title": "Department",
                    "className": "dt-left",
                    "data": "Dept",
                    "width": "20%",
                    "sorting": "false"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "8%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='UpdateLocation($event)' title='Add' data-toggle='tooltip' data-placement='left' tooltip> <i  class='icon-add cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblSearchLocationGrid').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {                
                $(nRow).attr("ng-dblclick", "UpdateLocation($event)");
                $compile(angular.element(nRow))($scope);
                // $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    //$scope.GetLocationSearch = function () {
    //    var promise = Step6Service.GetLocationSearch();
    //    promise.success(function (response) {
    //        if (response.Success) {
    //            angular.element("#modal_Search_Location").modal('show');
    //            $scope.searchLocationList = response.Data;
    //            bindLocationSerach();
    //        }
    //    });
    //    promise.error(function (data, statusCode) {
    //    });

    //};

    $scope.UpdateLocation = function ($event) {        
        var tblSearchLocation = $('#tblSearchLocationGrid').DataTable();
        var data = tblSearchLocation.row($($event.target).parents('tr')).data();
        $scope.OrderPartObj.LocID = data.LocID;
        $scope.OrderPartObj.LocationName = data.Name1;
        angular.element("#modal_Location_Search").modal('hide');
    };


    $scope.UpdatePart = function (form) {        
        if (!isNullOrUndefinedOrEmpty($scope.OrderPartObj.LocID)) {
            form.$submitted = true;
            if (form.$valid) {
                $scope.OrderPartObj.EmpId = $scope.EmpId;
                $scope.OrderPartObj.OrderId = $scope.OrderId;
                $scope.OrderPartObj.Scope = $("#divScope").html()
                
                var promise = OrderPartService.UpdateOrderPart($scope.OrderPartObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_Edit_Part").modal('hide');
                        toastr.success("Records Updated Successfully");
                        $scope.GetPartListByOrderId();
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
        }
        else {
            notificationFactory.customError("Select Location");
            $scope.OrderPartLocationform.$setPristine();
        }

    };

    //#endregion

    //#region Method    

    function bindDropDownList() {
        var promise = CommonServices.LocationDepartmentDropDown();
        promise.success(function (response) {
            if (response.Success) {
                $scope.departmentlist = response.Data;
            }
        });
        promise.error(function (data, statusCode) {
        });

        var state = CommonServices.StateDropdown();
        state.success(function (response) {
            $scope.StateList = response.Data;
        });
        var recordtype = CommonServices.RecordTypeDropDown();
        recordtype.success(function (response) {
            $scope.RecordTypeList = response.Data;            
        });
    }

    //function bindLocationSerach() {
    //    if ($.fn.DataTable.isDataTable("#tblSearchLocation")) {
    //        $('#tblSearchLocation').DataTable().destroy();
    //    }
    //    var table = $('#tblSearchLocation').DataTable({
    //        data: $scope.searchLocationList,
    //        "bDestroy": true,
    //        "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
    //        "aaSorting": [[0, 'desc']],
    //        "aLengthMenu": [10, 20, 50, 100, 200],
    //        "pageLength": 10,
    //        "stateSave": false,
    //        "columns": [
    //            {
    //                "title": "Location Id",
    //                "className": "dt-left",
    //                "data": "LocID",
    //                "sorting": "false",
    //                "width": "10%"
    //            },
    //            {
    //                "title": "Name",
    //                "className": "dt-left",
    //                "data": "Name1",
    //                "sorting": "false"
    //            },
    //            {
    //                "title": "Doctor Name",
    //                "className": "dt-left",
    //                "data": "Name2",
    //                "sorting": "false"
    //            },
    //            {
    //                "title": "Department",
    //                "className": "dt-left",
    //                "data": "Dept",
    //                "width": "15%",
    //                "sorting": "false"
    //            },
    //            {
    //                "title": 'Action',
    //                "data": null,
    //                "className": "action dt-center",
    //                "sorting": "false",
    //                "width": "8%",
    //                "render": function (data, type, row) {
    //                    var strAction = '';
    //                    strAction = "<a class='ico_btn cursor-pointer' ng-click='UpdateLocation($event)' title='Add'> <i  class='icon-add cursor-pointer'></i> </a>";
    //                    return strAction;
    //                }
    //            }
    //        ],
    //        "initComplete": function () {
    //            var dataTable = $('#tblSearchLocation').DataTable();
    //        },
    //        "fnDrawCallback": function () {
    //        },
    //        "fnCreatedRow": function (nRow, aData, iDataIndex) {
    //            $compile(angular.element(nRow).contents())($scope);
    //        }
    //    });

    //}

    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.PartNo = $stateParams.PartNo;
        $scope.GetPartListByOrderId();
        bindDropDownList();
    }

    //#endregion


    init();

});