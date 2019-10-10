app.controller('AttorneyUserController', function ($scope, $rootScope, $stateParams, CommonServices, notificationFactory, AttorneyUserService, AttorneyEmployeeTypeService, UserAttorneyMappingService, configurationService, $compile, $filter) {

    decodeParams($stateParams);
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.AccessAttorneyList = [];

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Attorney Users", "View");
    $scope.IsUserCanEditAttorneyUser = $rootScope.isSubModuleAccessibleToUser('Settings', 'Attorney Users', 'Edit Attorney User');
    $scope.IsUserCanDeleteAttorneyAccess = $rootScope.isSubModuleAccessibleToUser('Settings', 'Attorney Users', 'Delete Attorney Acess');
    //------------

    function bindAttorneyUserList() {

        if ($.fn.DataTable.isDataTable("#tblAttorneyUser")) {
            $('#tblAttorneyUser').DataTable().destroy();
        }

        var table = $('#tblAttorneyUser').DataTable({
            data: $scope.AttorneyUserList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": [[0, 'asc']],
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Name",
                    "className": "dt-left",
                    "width": "20%",
                    "render": function (data, type, row) {
                        return row.LastName + ' , ' + row.FirstName;
                    }
                },
                {
                    "title": "Email",
                    "className": "dt-left",
                    "width": "20%",
                    "data": "Email"
                },
                {
                    "title": "Firm ID",
                    "className": "dt-left",
                    "width": "20%",
                    "data": "FirmId"
                },
                {
                    "title": "Firm Name",
                    "className": "dt-left",
                    "data": "FirmName"
                },

                {
                    "title": "Status",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsActive",
                    "width": "6%",
                    "render": function (data, type, row) {
                        return row.IsApproved ? "<label class='label bg-success-400'>Active</label>" : "<label class='label bg-danger-400'>InActive</label>";
                    }
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "7%",
                    "visible": $scope.IsUserCanEditAttorneyUser,
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer'  ng-click='GetAttorneyUserForEditMode($event)' data-toggle='tooltip' data-placement='left' tooltip title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        if (row.IsApproved)
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='ActivateInactiveAttorneyUser($event)' data-toggle='tooltip' data-placement='left' tooltip title='Lock'>  <i class='icon-lock cursor-pointer'></i> </a>";
                        else
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='ActivateInactiveAttorneyUser($event)' data-toggle='tooltip' data-placement='left' tooltip title='Unlock'>  <i class='icon-unlocked2 cursor-pointer'></i> </a>";

                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblAttorneyUser').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    };

    function bindAccessAttorneyList() {

        if ($.fn.DataTable.isDataTable("#tblAttorneyWhoCanAccessUser")) {
            $('#tblAttorneyWhoCanAccessUser').DataTable().destroy();
        }
        var table = $('#tblAttorneyWhoCanAccessUser').DataTable({
            data: $scope.AccessAttorneyList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [5, 10, 20, 50, 100, 200],
            "pageLength": 5,
            "autoWidth": false,
            "stateSave": false,
            "columns": [
                {
                    "title": "Attorney Id",
                    "className": "dt-left",
                    "width": "15%",
                    "data": "Attyid"
                },
                {
                    "title": "Attorney Name",
                    "className": "dt-left",
                    "data": "Name"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "width": "10%",
                    "sorting": "false",
                    "visible": $scope.IsUserCanDeleteAttorneyAccess,
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer deletemapping' ng-click='DeleteAttorneyUserMapping($event)'  data-toggle='tooltip' data-placement='left' tooltip title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblAttorneyWhoCanAccessUser').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    };

    function bindAttorneyWhoNeedToAccessUserList() {
        if ($.fn.DataTable.isDataTable("#tblAttorneyWhoNeedToAccessUser")) {
            $('#tblAttorneyWhoNeedToAccessUser').DataTable().destroy();
        }
        var table = $('#tblAttorneyWhoNeedToAccessUser').DataTable({
            data: $scope.AccessAttorneyList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10],
            "pageLength": 10,
            "autoWidth": false,
            "stateSave": false,
            "columns": [
                {
                    "title": "Attorney Id",
                    "className": "dt-left",
                    "width": "15%",
                    "data": "Attyid"
                },
                {
                    "title": "Attorney Name",
                    "className": "dt-left",
                    "data": "Name"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "width": "10%",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='InsertAttorneyUserMapping($event)'  data-toggle='tooltip' data-placement='left' tooltip title='Add Access Attorney'>  <i  class='icon-add cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblAttorneyWhoNeedToAccessUser').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    };

    $scope.AddOrUpdateAttorneyUser = function (form) {
        debugger;
        if ($scope.IsEditMode == true) {
            if (!($scope.AccessAttorneyLength > 0)) {
                return false;
            }
        }

        if (form.$valid) {
            $scope.AttorneyUserObj.CreatedBy = $scope.UserAccessId;
            $scope.AttorneyUserObj.CompanyNo = $rootScope.CompanyNo;

            if (!$scope.IsEditMode) {  //add mode  
                $scope.AttorneyUserObj.Password = "cem@123";
                $scope.AttorneyUserObj.CompanyNo = $rootScope.CompanyNo
                var promise = AttorneyUserService.InsertAttorneyUser($scope.AttorneyUserObj);
                promise.success(function (response) {
                    if (response.Success) {
                        if (response.str_ResponseData == '-1') {
                            toastr.error("User Email already exists.");
                            return false;
                        }
                        else {
                            toastr.success("Attorney user detail inserted successfully.");
                            $scope.currentattorneyuserId = response.str_ResponseData;
                            $scope.AttorneyUserObj.AttorneyUserId = $scope.currentattorneyuserId;
                            angular.element("#modal_form_addattorneyaccess").modal('show');
                            if (!$scope.IsEditMode) {
                                $scope.GetAttorneyListWhoNeedToAccessUser();
                            }
                        }
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });

            }
            else {  //edit mode
                $scope.AttorneyUserObj.AttorneyEmployeeTypeId = $scope.AttorneyUserObj.AttorneyEmployeeTypeId;
                var promise = AttorneyUserService.UpdateAttorneyUser($scope.AttorneyUserObj);
                promise.success(function (response) {
                    if (response.Success) {
                        if (response.str_ResponseData == '-1') {
                            toastr.error("User Email already exists.");
                            return false;
                        }
                        else {
                            toastr.success("Attorney user detail updated successfully.");
                            angular.element("#modal_form_addattorneyuser").modal('hide');
                            $scope.GetAttorneyUsersList();
                        }
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
        }
    };

    $scope.CloseAttorneyListPopup = function () {
        $scope.IsEditMode = true;
        $scope.GetUserAttorneyMappingList($scope.currentattorneyuserId, true);
    };

    $scope.GetAttorneyUsersList = function () {
        var promise = AttorneyUserService.GetAttorneyUsers('');
        promise.success(function (response) {
            $scope.AttorneyUserList = response.Data;
            bindAttorneyUserList();
            $scope.addupdateform.$setPristine();
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.GetAttorneyEmployeeType = function () {
        var promise = AttorneyEmployeeTypeService.GetAttorneyEmployeeType();
        promise.success(function (response) {
            $scope.AttorneyEmployeeTypeList = response.Data;
        });
        promise.error(function (data, statusCode) {
        });

        var company = CommonServices.GetCompanyDropDown();
        company.success(function (response) {
            $scope.CompanyList = response.Data;
        });
        company.error(function (data, statusCode) {

        });
    };

    $scope.GetUserAttorneyMappingList = function (attorneyuserId, selectOnlyCurrentAccessAttorney) {
        var promise = UserAttorneyMappingService.GetUserAttorneyMapping(attorneyuserId, selectOnlyCurrentAccessAttorney);
        promise.success(function (response) {
            $scope.AccessAttorneyList = response.Data;

            if (response.Data != undefined && response.Data != null && response.Data.length > 0) {
                $scope.AccessAttorneyLength = $scope.AccessAttorneyList.length;
            } else {
                $scope.AccessAttorneyList = [];
                $scope.AccessAttorneyLength = null;
            }

            if (selectOnlyCurrentAccessAttorney == true) {

                bindAccessAttorneyList();
                if ($scope.AccessAttorneyLength == 1) {
                    $(".deletemapping").hide();
                }
            }
            else {
                bindAttorneyWhoNeedToAccessUserList();
            }

        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.DeleteAttorneyUserMapping = function ($event) {
        tblAttorneyWhoCanAccessUser = $('#tblAttorneyWhoCanAccessUser').DataTable();
        var data = tblAttorneyWhoCanAccessUser.row($($event.target).parents('tr')).data();

        var promise = UserAttorneyMappingService.DeleteAttorneyUserMapping($scope.currentattorneyuserId, data.Attyid);

        promise.success(function (response) {
            $scope.GetUserAttorneyMappingList($scope.currentattorneyuserId, true);
        });
        promise.error(function (data, statusCode) {
        });
    };
    $scope.DeleteAttorneyUser = function () {

        if ($scope.AccessAttorneyLength > 0 || $scope.currentattorneyuserId == 0) {
            angular.element("#modal_form_addattorneyuser").modal('hide');
        }
        else {
            if (confirm('Are you sure to delete the attorney user?')) {
                var promise = AttorneyUserService.DeleteAttorneyUser($scope.currentattorneyuserId);
                promise.success(function (response) {
                    toastr.success("Attorney user deleted successfully.");
                    angular.element("#modal_form_addattorneyuser").modal('hide');
                });
                promise.error(function (data, statusCode) {
                });
            }
        }

    };


    $scope.ActivateInactiveAttorneyUser = function ($event) {
        tblAttorneyUser = $('#tblAttorneyUser').DataTable();
        var data = tblAttorneyUser.row($($event.target).parents('tr')).data();

        var promise = AttorneyUserService.ActivateInactiveAttorneyUser(data.AttorneyUserId);

        promise.success(function (response) {
            $scope.GetAttorneyUsersList();
        });
        promise.error(function (data, statusCode) {
        });
    };



    $scope.InsertAttorneyUserMapping = function ($event) {

        tblAttorneyWhoCanAccessUser = $('#tblAttorneyWhoNeedToAccessUser').DataTable();
        var data = tblAttorneyWhoCanAccessUser.row($($event.target).parents('tr')).data();
        tblAttorneyWhoCanAccessUser.row($($event.target).parents('tr')).remove();
        tblAttorneyWhoCanAccessUser.draw();
        UserAttorneyMappingService.InsertAttorneyUserMapping($scope.currentattorneyuserId, data.Attyid);
    };


    $scope.GetAttorneyUserForEditMode = function ($event) {

        $scope.addupdateform.$setPristine();
        tblAttorneyUser = $('#tblAttorneyUser').DataTable();
        var data = tblAttorneyUser.row($($event.target).parents('tr')).data();
        $scope.IsEditMode = true;
        //$scope.AccessAttorneyLength = null;
        $scope.currentattorneyuserId = data.AttorneyUserId;

        var promise = AttorneyUserService.GetAttorneyUsers(data.AttorneyUserId);

        promise.success(function (response) {
            $scope.modal_Title = "Edit Attorney User";
            $scope.AttorneyUserObj = response.Data[0];
            $scope.SelectedAttorneyEmployeeTypeId = $scope.AttorneyUserObj.AttorneyEmployeeTypeId;
            angular.element("#modal_form_addattorneyuser").modal('show');
        });
        promise.error(function (data, statusCode) {
        });

        $scope.GetUserAttorneyMappingList(data.AttorneyUserId, true);

    };

    $scope.GetAttorneyListWhoNeedToAccessUser = function () {
        $scope.GetUserAttorneyMappingList($scope.currentattorneyuserId, false);
    };


    $scope.ClearScreenForNewAttorneyUser = function () {
        $scope.AttorneyUserObj = new Object();
        $scope.IsEditMode = false;
        $scope.currentattorneyuserId = 0;
        $scope.SelectedAttorneyEmployeeTypeId = '';
        $scope.AttorneyUserObj.Email = '';
        $scope.AccessAttorneyList = new Object();
        bindAccessAttorneyList();
        $scope.addupdateform.$setPristine();
        angular.element("#modal_form_addattorneyuser").modal('show');
        $scope.modal_Title = "Add Attorney User";
        $scope.AccessAttorneyLength = null;
    };


    function init() {



        $scope.GetAttorneyUsersList();
        $scope.GetAttorneyEmployeeType();
    };

    init();
});