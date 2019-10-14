app.controller('FirmController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, FirmServices, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);

    $scope.isEdit = false;

    //#region Events

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Admin", "Firms", "View");
    $scope.IsUserCanEditFirm = $rootScope.isSubModuleAccessibleToUser('Admin', 'Firms', 'Edit Firm');
    $scope.IsUserCanDeleteFirm = $rootScope.isSubModuleAccessibleToUser('Admin', 'Firms', 'Delete Firm');    
    //-----

    $scope.EditFirm = function ($event) {
        var table = $('#tblFirm').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        //$scope.ManageFirmById(row.FirmID);
        $state.transitionTo('ManageFirm', { 'FirmID': row.FirmID });
    };

    $scope.DeleteFirm = function ($event) {
        bootbox.confirm({
            message: "Are you sure you want to delete this record ?",
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
                    var table = $('#tblFirm').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = FirmServices.DeleteFirm(row.FirmID);
                    promise.success(function (response) {
                        if (response.Success) {
                            bootbox.hideAll()
                            notificationFactory.successDelete('');
                            $scope.bindFirmList()
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error();
                }
                bootbox.hideAll()
            }
        });
    }; 

    $scope.ClearFirmSearch_Click = function () {
        clearFirmSearch();
        $scope.bindFirmList();
    };

    $scope.FirmSearch_Click = function () {
        $scope.bindFirmList();
    };

    //#endregion 

    //#region Methods 

    function init() { 
        clearFirmSearch(); 
        $scope.bindFirmList();
        $scope.FirmObj = new Object();
        bindDropDown();
        
    }

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
    }
     
    function clearFirmSearch() {
        $scope.FirmSearchObj = new Object();
        $scope.FirmSearchObj.FirmID = "";
        $scope.FirmSearchObj.FirmName = "";
        $scope.FirmSearchObj.Address = "";
        $scope.FirmSearchObj.City = "";
        $scope.FirmSearchObj.State = "";
    } 

    $scope.bindFirmList = function () {
        debugger;
        if ($.fn.DataTable.isDataTable("#tblFirm")) {
            $('#tblFirm').DataTable().destroy();
        }
        var pagesizeObj = 10;

        $('#tblFirm').DataTable({
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
            "iDisplayLength": pagesizeObj,
            "lengthMenu": [10, 20, 30, 40, 50],
            "sAjaxDataProp": "aaData",
            "aaSorting": [[0, 'asc']],
            "sAjaxSource": configurationService.basePath + "/GetFirmList",
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                aoData = BindSearchCriteriaCommon(aoData);
                aoData = BindSortingCommon(aoData, oSettings);
                if ($scope.SearchValue == undefined) {
                    $scope.SearchValue = "";
                }
                //aoData.SearchValue = $scope.SearchValue; 
                //aoData.PageIndex = parseInt($('#tblFirm').DataTable().page.info().page) + 1;


                $scope.GridParams = aoData;
                oSettings.jqXHR = $.ajax({
                    'dataSrc': 'aaData',
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource + "?SearchValue=" + ""
                        + "&PageIndex=" + (parseInt($('#tblFirm').DataTable().page.info().page) + 1)
                        + "&CompanyNo=" + $rootScope.CompanyNo
                        + "&FirmID=" + $scope.FirmSearchObj.FirmID
                        + "&FirmName=" + $scope.FirmSearchObj.FirmName 
                        + "&Address=" + $scope.FirmSearchObj.Address
                        + "&City=" + $scope.FirmSearchObj.City
                        + "&State=" + $scope.FirmSearchObj.State
                        + "&ParentFirm=",
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });


            },
            "columns": [
                { "title": "Firm Code", "data": "FirmID", "className": "dt-left", "width": "14%" },
                { "data": "FirmName", "title": "Name", "className": "dt-left" },
                { "data": "City", "title": "City", "className": "dt-left" },
                { "data": "State", "title": "State", "className": "dt-left" },
                { "data": "Address", "title": "Address", "className": "dt-left" },
                {
                    "title": 'Action',
                    "sClass": "action dt-center",
                    "orderable": false,
                    "width":"14%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if ($scope.IsUserCanEditFirm) {
                            strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="EditFirm($event)" title="Edit"> <i class="icon-pencil3"></i></a>';
                        }
                        if ($scope.IsUserCanDeleteFirm) {
                            strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="DeleteFirm($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>';
                        }                                                
                        return strAction;
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblFirm').DataTable();
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

    }

    //#endregion 


    

    init();
});