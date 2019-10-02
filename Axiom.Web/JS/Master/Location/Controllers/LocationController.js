app.controller('LocationController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, LocationServices, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);

    $scope.isEdit = false;


    //Page Rights//
    $rootScope.CheckIsPageAccessible("Admin", "Location", "View");
    $scope.IsUserCanEditLocation = $rootScope.isSubModuleAccessibleToUser('Admin', 'Location', 'Edit Location');
    $scope.IsUserCanDeleteLocation = $rootScope.isSubModuleAccessibleToUser('Admin', 'Location', 'Delete Location');
    $scope.IsUserCanMergeLocation = $rootScope.isSubModuleAccessibleToUser('Admin', 'Location', 'Merge Location');
    //-----

    //#region Events

    $scope.EditLocation = function ($event) {        
        var table = $('#tblLocation').DataTable();
        var row = table.row($($event.target).parents('tr')).data();        
        //$scope.ManageLocationById(row.LocID);
        $state.transitionTo('ManageLocation', { 'LocID': row.LocID });
    };

    $scope.DeleteLocation = function ($event) {
        bootbox.confirm({
            message: "Are you sure you want to delete this record?",
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
                    var table = $('#tblLocation').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = LocationServices.DeleteLocation(row.LocID);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.customSuccess("Location deleted successfully");
                            //toastr.Success('Location deleted successfully.');
                            $scope.bindLocationList();
                            bootbox.hideAll();
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error();
                }
                bootbox.hideAll();
            }
        });
    };

    $scope.GetMergeLocations = function ($event) {
        var table = $('#tblLocation').DataTable();
        $scope.ObjOpenMergeLocation = table.row($($event.target).parents('tr')).data();
        $scope.ShowMergeLocation = true;

    };

    $scope.ClearLocationSearch_Click = function () {
        clearLocationSearch();
        $scope.bindLocationList();
    };

    $scope.LocationSearch_Click = function () {
        $scope.bindLocationList();
    };

    //#endregion 

    //#region Methods 

    function init() {
        clearLocationSearch();
        $scope.bindLocationList();
        $scope.LocationObj = new Object();
        bindDropDown();
    };

    function bindDropDown() {
        //StateDropdown
        var _statelist = CommonServices.StateDropdown();
        _statelist.success(function (response) {
            $scope.Statelist = response.Data;
        });
        _statelist.error(function (data, statusCode) { });

        // DepartmentD  
        var _departmentlist = CommonServices.DepartmentDropdown();
        _departmentlist.success(function (response) {
            $scope.Departmentlist = response.Data;
        });
        _departmentlist.error(function (data, statusCode) { });
    };

    function clearLocationSearch() {
        $scope.LocationSearchObj = new Object();
        $scope.LocationSearchObj.LocID = "";
        $scope.LocationSearchObj.Name = "";
        $scope.LocationSearchObj.PhoneNo = "";
        $scope.LocationSearchObj.Department = "";
        $scope.LocationSearchObj.Address = "";
        $scope.LocationSearchObj.City = "";
        $scope.LocationSearchObj.State = "";
    };


    $scope.bindLocationList = function () {

        if ($.fn.DataTable.isDataTable("#tblLocation")) {
            $('#tblLocation').DataTable().destroy();
        }
        var pagesizeObj = 10;
        $('#tblLocation').DataTable({
            stateSave: false,
            "oLanguage": {
                "sProcessing": '<div class="loader" data-loading><img src="/assets/images/loader2.gif" /></div>',
                "sZeroRecords": "<span class='pull-left'>No records found</span>",
            },
            "searching": false,
            "dom": '<"table-responsive"frt><"bottom"lip<"clear">>',
            "bProcessing": true,
            "bServerSide": true,
            "iDisplayStart": 0,
            "autoWidth": false,
            "iDisplayLength": pagesizeObj,
            "lengthMenu": [10, 20, 30, 40, 50],
            "sAjaxDataProp": "aaData",
            "aaSorting": [[0, 'asc']],
            "sAjaxSource": configurationService.basePath + "GetLocationList",
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {

                aoData = BindSearchCriteriaCommon(aoData);
                aoData = BindSortingCommon(aoData, oSettings);
                if ($scope.SearchValue == undefined) {
                    $scope.SearchValue = "";
                }
                //aoData.SearchValue = $scope.SearchValue; 
                //aoData.PageIndex = parseInt($('#tblLocation').DataTable().page.info().page) + 1;


                $scope.GridParams = aoData;

                oSettings.jqXHR = $.ajax({
                    'dataSrc': 'aaData',
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource + "?SearchValue=" + "" + "&PageIndex=" + (parseInt($('#tblLocation').DataTable().page.info().page) + 1)
                        + "&LocID=" + $scope.LocationSearchObj.LocID
                        + "&Name=" + $scope.LocationSearchObj.Name
                        + "&PhoneNo=" + $scope.LocationSearchObj.PhoneNo
                        + "&Department=" + ($scope.LocationSearchObj.Department == null ? "" : $scope.LocationSearchObj.Department)
                        + "&Address=" + $scope.LocationSearchObj.Address
                        + "&City=" + $scope.LocationSearchObj.City
                        + "&State=" + ($scope.LocationSearchObj.State == null ? "" : $scope.LocationSearchObj.State),
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });


            },
            "columns": [
                { "title": "Location Code", "data": "LocID", "className": "dt-left", "width": "8%" },
                { "data": "Name", "title": "Name", "className": "dt-left" },
                { "data": "Address", "title": "Address", "className": "dt-left", "width": "15%" },
                { "data": "City", "title": "City", "className": "dt-left", "width": "8%" },
                { "data": "State", "title": "State", "className": "dt-left", "width": "8%" },
                { "data": "PhoneNo1", "title": "Phone No", "className": "dt-left", "width": "8%" },
                { "data": "FaxNo", "title": "FaxNo", "className": "dt-left", "width": "8%" },
                { "data": "Department", "title": "Department", "className": "dt-left", "width": "10%" },
                {
                    "title": 'Action',
                    "sClass": "action dt-center",
                    "orderable": false,
                    "width": "8%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if ($scope.IsUserCanMergeLocation) {
                            strAction = '<a class="ico_btn cursor-pointer" ng-click="GetMergeLocations($event)" title="Merge Location"> <i class="icon-merge"></i></a>';
                        }
                        if ($scope.IsUserCanEditLocation) {
                            strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="EditLocation($event)" title="Edit"> <i class="icon-pencil3"></i></a>';
                        }
                        if ($scope.IsUserCanDeleteLocation) {
                            strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="DeleteLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>';
                        }
                        return strAction;
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblLocation').DataTable();
                BindCustomerSearchBar($scope, $compile, dataTable);
            },
            "fnDrawCallback": function () {
                //BindToolTip();
                setTimeout(function () { SetAnchorLinks(); }, 500);
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    };

    //#endregion 

    init();
});