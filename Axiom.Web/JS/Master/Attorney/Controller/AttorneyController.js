app.controller('AttorneyController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, EmployeeServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);

    //Page Rights//    
    $rootScope.CheckIsPageAccessible("Admin", "Attorneys", "View");
    $scope.IsUserCanEditAttorney = $rootScope.isSubModuleAccessibleToUser('Admin', 'Attorneys', 'Edit Attorney');    
    //-----


    function init() {
        
        //GetEmployeelist();
        $scope.isEdit = false;
        $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
        $scope.FirmObj = new Object();
        $scope.objAttorney = new Object();
        $scope.SearchAttorney();
    }

    $scope.EditAttorney = function ($event) {
        var table = $('#tblAttorney').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $state.transitionTo('ManageAttorney', { 'AttyID': row.AttyID });
    }

    $scope.ClearSearch = function () {
        $scope.objAttorney.FirmID = "";
        $scope.objAttorney.FirmIDSearch = "";
        $scope.objAttorney.FirmName = "";
        $scope.objAttorney.AttorneyFirstName = "";
        $scope.objAttorney.AttorneyLastName = "";
        bindAttorneyList();
    }
    $scope.SearchAttorney = function () {        
        $scope.objAttorney.FirmID = "";
        if ($scope.objAttorney.FirmID == null || $scope.objAttorney.FirmID == "" ) {
            $scope.objAttorney.FirmID = $scope.objAttorney.FirmIDSearch;
        }
        
        bindAttorneyList();
    }
    function bindAttorneyList() {

        if ($.fn.DataTable.isDataTable("#tblAttorney")) {
            $('#tblAttorney').DataTable().destroy();
        }
        var pagesizeObj = 10;
        $('#tblAttorney').DataTable({
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
            "aaSorting": [[1, 'asc']],
            "sAjaxSource": configurationService.basePath + "/AttorneySearch",
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
                    "url": sSource + "?SearchValue=" + ""
                        + "&PageIndex=" + (parseInt($('#tblAttorney').DataTable().page.info().page) + 1)
                        + "&CompanyNo=" + $rootScope.CompanyNo
                        + "&FirmID=" + ($scope.objAttorney.FirmID == null ? "" : $scope.objAttorney.FirmID)
                        + "&FirmName=" + ($scope.objAttorney.FirmName == null ? "" : $scope.objAttorney.FirmName)
                        + "&AttorneyFirstName=" + ($scope.objAttorney.AttorneyFirstName == null ? "" : $scope.objAttorney.AttorneyFirstName)
                        + "&AttorneyLastName=" + ($scope.objAttorney.AttorneyLastName == null ? "" : $scope.objAttorney.AttorneyLastName),
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });


            },
            "columns": [
                { "title": "Attorney ID", "data": "AttyID", "className": "dt-left", "width": "8%" },
                { "data": "AttorneyName", "title": "Attorney Name", "className": "dt-left","width" : "15%" },
                { "title": "FirmID", "data": "FirmID", "className": "dt-left", "width": "8%" },
                { "data": "FirmName", "title": "Firm Name", "className": "dt-left", "width": "20%" },
                {
                    "title": 'Action',
                    "sClass": "action dt-center",
                    "orderable": false,
                    "width": "8%",
                    "visible": $scope.IsUserCanEditAttorney,
                    "render": function (data, type, row) {
                        var strAction = '';                             
                        strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="EditAttorney($event)" title="Edit"> <i class="icon-pencil3"></i></a>';
                        // strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="DeleteLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>';
                        return strAction;
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblAttorney').DataTable();
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

    //function GetFirmlist() {
    //    var promise = EmployeeServices.GetEmployeeList();
    //    promise.success(function (response) {
    //        $scope.Firmlist = response.Data;
    //        //bindFirmList();
    //    });
    //    promise.error(function (data, statusCode) {
    //    });
    //}

    $scope.AddFirm = function () {
        $scope.FirmObj = new Object();
        $scope.isEdit = false;
        $scope.modal_Title = "Add Firm";
        $scope.btnText = "Save";
        $scope.Firmform.$setPristine();
       // bindDropDown();
        angular.element("#modal_Firm").modal('show');
    };

    $scope.FirmPopup = function () {
        debugger;
        $scope.ShowSearchFirm = true;
    }

    //function bindDropDown() {
    //    var promise = CommonServices.DepartmentDropdown();
    //    promise.success(function (response) {
    //        $scope.DeptList = response.Data;
    //    });
    //    promise.error(function (data, statusCode) {
    //    });
    //}
    //$scope.DeleteEmployee = function ($event) {
    //    bootbox.confirm({
    //        message: "Are you sure you want to delete this record ?",
    //        buttons: {
    //            confirm: {
    //                label: 'Yes',
    //                className: 'btn-success'
    //            },
    //            cancel: {
    //                label: 'No',
    //                className: 'btn-danger'
    //            }
    //        },
    //        callback: function (result) {
    //            if (result) {
    //                var table = $('#tblEmployee').DataTable();
    //                var row = table.row($($event.target).parents('tr')).data();
    //                var promise = EmployeeServices.DeleteEmployee(row.ID);
    //                promise.success(function (response) {
    //                    if (response.Success) {
    //                        notificationFactory.customSuccess("Employee Delete Successfully");
    //                        GetEmployeelist();
    //                    }
    //                    else {
    //                        toastr.error(response.Message[0]);
    //                    }
    //                });
    //                promise.error(function () { });
    //            }
    //            bootbox.hideAll();
    //        }
    //    });
    //};

    //$scope.EditEmployee = function ($event) {
    //    var table = $('#tblEmployee').DataTable();
    //    var row = table.row($($event.target).parents('tr')).data();
    //    $scope.GetEmployeeById(row.UserId);
    //};

    //$scope.GetEmployeeById = function (id) {
    //    $scope.Employeeform.$setPristine();
    //    bindDropDown();
    //    var promise = EmployeeServices.GetEmployeeById(id);
    //    promise.success(function (response) {
    //        if (response.Success) {
    //            $scope.isEdit = true;
    //            $scope.modal_Title = "Edit Employee";
    //            $scope.btnText = "Edit";
                 
    //            $scope.EmployeeObj = response.Data[0];
    //            angular.element("#modal_Employee").modal('show');
    //        }
    //        else {
    //            notificationFactory.customError(response.Message[0]);
    //        }
    //    });
    //    promise.error(function () { });
    //};

    //$scope.AddOrEditEmployee = function (form) {
    //    if (form.$valid) {
    //        $scope.EmployeeObj.CreatedBy = $scope.UserAccessId;
    //        if (!$scope.isEdit) {  // add mode
    //            $scope.EmployeeObj.Password = "cem@123";
    //            var promise = EmployeeServices.InsertEmployee($scope.EmployeeObj);
    //            promise.success(function (response) {
    //                if (response.Success) {
    //                    if (response.str_ResponseData == '-1') {
    //                        toastr.error("User Email already exists.");
    //                        return false;
    //                    }
    //                    else {
    //                        toastr.success("Employee detail inserted successfully.");
    //                        angular.element("#modal_Employee").modal('hide');
    //                        GetEmployeelist();
    //                    }
    //                }
    //                else {
    //                    toastr.error(response.Message[0]);
    //                }
    //            });
    //            promise.error(function () { });
    //        }
    //        else { //Edit Mode
    //            var promise = EmployeeServices.UpdateEmployee($scope.EmployeeObj);
    //            promise.success(function (response) {
    //                if (response.str_ResponseData == '-1') {
    //                    toastr.error("User Email already exists.");
    //                    return false;
    //                }
    //                else {
    //                    toastr.success("Employee detail Updated successfully.");
    //                    angular.element("#modal_Employee").modal('hide');
    //                    GetEmployeelist();
    //                }
    //            });
    //            promise.error(function () { });
    //        }
    //    }
    //};


    init();

});