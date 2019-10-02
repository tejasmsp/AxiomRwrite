app.controller('FirmSearchController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, FirmSearchServices, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);

    $scope.isEdit = false;

    //#region Events

    $scope.SelectFirm = function ($event) {
        debugger;
        var table = $('#tblSearchFirm').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        if ($scope.$parent.$state.current.name.toLowerCase() == "attorneydetail") {
            $scope.$parent.objAttorney.FirmID = row.FirmID;
            $scope.$parent.objAttorney.FirmIDSearch = row.FirmID;
            $scope.$parent.objAttorney.FirmName = row.FirmName;
            $scope.$parent.SearchAttorney();
            // $scope.$parent.objEditInvoice.FirmName = row.FirmName;
        }
        else if ($scope.$parent.$state.current.name.toLowerCase() == "manageattorney") {
            $scope.$parent.FillFirmDetail(row.FirmID);
        }
        else if ($scope.$parent.$state.current.name.toLowerCase() == "managelocation") {
            $scope.$parent.FilFirmlLocationDetail(row.FirmID);
        }
        else if ($scope.$parent.$state.current.name.toLowerCase() == "partdetail") {
            $scope.$parent.FillLocationFeesFirmDetail(row.FirmID);
        }
        else if ($scope.$parent.$state.current.name.toLowerCase() == "billing") {
            $scope.$parent.objEditInvoice.FirmID = row.FirmID;
            $scope.$parent.objEditInvoice.FirmName = row.FirmName;
            $scope.$parent.GetAttorneyListByFirmId(row.FirmID,'');

        } else if ($scope.$parent.$state.current.name.toLowerCase() == "managecheck") {
            $scope.$parent.objAccountRec.FirmID = row.FirmID;
            $scope.$parent.objAccountRec.FirmName = row.FirmName;
          
        }
        else if ($scope.$parent.$state.current.name.toLowerCase() == "check") {
            $scope.$parent.objAttorney.FirmID = row.FirmID;   
            $scope.$parent.objAttorney.FirmName = row.FirmName; 
        }
        else if ($scope.$parent.$state.current.name.toLowerCase() == "voidinvoice") {
            $scope.$parent.objVoidInvoice.FirmID = row.FirmID;
            $scope.$parent.objVoidInvoice.FirmName = row.FirmName;
        }
        else if ($scope.$parent.$state.current.name.toLowerCase() == "orderdetail") {
            $scope.$parent.ChangBillToFirmSave(row.FirmID);
            //$scope.$parent.objVoidInvoice.FirmID = row.FirmID;
            //$scope.$parent.objVoidInvoice.FirmName = row.FirmName;
        }
        else {
            $scope.$parent.objEditInvoice.FirmID = row.FirmID;
            $scope.$parent.objEditInvoice.FirmName = row.FirmName;
        }

        angular.element("#modal_FirmSearch").modal('hide');

        //$scope.ManageFirmById(row.FirmID);
        // $state.transitionTo('ManageFirm', { 'FirmID': row.FirmID });
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

    $scope.$parent.$watch('ShowSearchFirm', function (newVal, oldVal) {        
        if (newVal) {
            $scope.ShowSearchFirm();
        }
    }, true);

    $scope.ShowSearchFirm = function () {

        angular.element("#modal_FirmSearch").modal('show');
        clearFirmSearch();
        $scope.$parent.ShowSearchFirm = false;
    }

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
        $scope.objFirmSearch = new Object();
        $scope.objFirmSearch.FirmID = "";
        $scope.objFirmSearch.FirmName = "";
        $scope.objFirmSearch.Address = "";
        $scope.objFirmSearch.City = "";
        $scope.objFirmSearch.State = "";
    }

    $scope.bindFirmList = function () {
        if ($.fn.DataTable.isDataTable("#tblSearchFirm")) {
            $('#tblSearchFirm').DataTable().destroy();
        }
        var pagesizeObj = 10;

        $('#tblSearchFirm').DataTable({
            stateSave: false,
            "oLanguage": {
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
            "sAjaxSource": configurationService.basePath + "/SearchFirmList",
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                aoData = BindSearchCriteriaCommon(aoData);
                aoData = BindSortingCommon(aoData, oSettings);
                if ($scope.SearchValue == undefined) {
                    $scope.SearchValue = "";
                }

                if ($scope.objFirmSearch.Name == undefined) {
                    $scope.objFirmSearch.Name = "";
                }
                //aoData.SearchValue = $scope.SearchValue; 
                //aoData.PageIndex = parseInt($('#tblSearchFirm').DataTable().page.info().page) + 1;


                $scope.GridParams = aoData;
                oSettings.jqXHR = $.ajax({
                    'dataSrc': 'aaData',
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource + "?SearchValue=" + "" + "&PageIndex=" + (parseInt($('#tblSearchFirm').DataTable().page.info().page) + 1)
                        + "&FirmID=" + $scope.objFirmSearch.FirmID
                        + "&FirmName=" + $scope.objFirmSearch.Name
                        + "&Address=" + $scope.objFirmSearch.Address
                        + "&City=" + $scope.objFirmSearch.City
                        + "&State=" + $scope.objFirmSearch.State,
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
                    "width": "14%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="SelectFirm($event)" title="Select"> <i class="icon-checkmark4"></i></a>';
                        return strAction;
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblSearchFirm').DataTable();
                BindCustomerSearchBar($scope, $compile, dataTable);
            },
            "fnDrawCallback": function () {
                //BindToolTip();
                setTimeout(function () { SetAnchorLinks(); }, 500);
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).attr("ng-dblclick", "SelectFirm($event)");
                $compile(angular.element(nRow))($scope);
                //$compile(angular.element(nRow).contents())($scope);
            }
        });
    }
    //#endregion

    

    init();
});