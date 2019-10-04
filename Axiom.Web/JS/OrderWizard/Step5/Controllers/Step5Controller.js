app.controller('Step5Controller', function ($scope, $rootScope, $state, $stateParams, notificationFactory, AttorneyUserService, CommonServices, Step5Service, configurationService, $compile, $filter) {

    decodeParams($stateParams);

    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    
    
    $scope.OrderOppositeAttorneyList = [];
    $scope.AttorneyList = [];
    $scope.searchText = '';
    $scope.searchCriteria = 'a.FirstName';
    $scope.searchCondition = 1;
    $scope.ConditionAry = [{ Id: 1, Name: 'Contains' }, { Id: 2, Name: 'Equal' }];

    $scope.IsNewAttorney = false;
    $scope.IsNewFirm = false;
    $scope.ShowAddNewAttorney = false;
    //#region Event

    $scope.GetOppositeAttorneyForEditMode = function ($event) {
        $scope.mode = 'edit';
        tblOppositeAttorney = $('#tblOppositeAttorney').DataTable();
        var data = tblOppositeAttorney.row($($event.target).parents('tr')).data();
        $scope.AttorneyObj = new Object();
        $scope.AttorneyObj.OrderID = $scope.OrderId;
        $scope.AttorneyObj.OrderFirmAttorneyId = data.OrderFirmAttorneyId;
        $scope.AttorneyObj.Name = data.AttorneyFirstName + ', ' + data.AttorneyLastName;
        $scope.AttorneyObj.AttyID = data.AttyID;
        $scope.AttorneyObj.FirmName = data.FirmName;
        $scope.AttorneyObj.FirmID = data.FirmID;
        $scope.AttorneyObj.IsPatientAttorney = data.IsPatientAttorney;
        $scope.AttorneyObj.OppSide = data.OppSide;
        $scope.AttorneyObj.Represents = data.Represents;
        $scope.AttorneyObj.Notes = data.Notes;
        $scope.popupTitle = 'Edit Additional Attorney';
        angular.element("#modal_form_editoppositeattorney").modal('show');
    };
    $scope.GetOppositeAttorneyForAddModeDirect = function(currentData) {
        
        $scope.mode = 'add';
        
        $scope.AttorneyObj.OrderID = $scope.OrderId;
        $scope.AttorneyObj.OrderFirmAttorneyId = 0;
        $scope.AttorneyObj.Name = currentData.FirstName + ', ' + currentData.LastName;
        $scope.AttorneyObj.OrderFirmAttorneyId = 0;
        $scope.AttorneyObj.AttyID = currentData.AttyID;
        $scope.AttorneyObj.FirmName = currentData.FirmName;
        $scope.AttorneyObj.FirmID = currentData.FirmId;
        $scope.AttorneyObj.IsPatientAttorney = 0;
        $scope.AttorneyObj.OppSide = 0;
        $scope.AttorneyObj.Represents = '';
        $scope.AttorneyObj.Notes = '';
        $scope.popupTitle = 'Add Opposite Attorney';
        angular.element("#modal_form_editoppositeattorney").modal('show');
    };




    $scope.GetOppositeAttorneyForAddMode = function ($event) {
        var data = new Object();
        $scope.CurrentEvent = $event.target;
        //$scope.mode = 'add';

        tblAddOppositeAttorney = $('#tblAddOppositeAttorney').DataTable();
        data = tblAddOppositeAttorney.row($($event.target).parents('tr')).data();


        $scope.GetOppositeAttorneyForAddModeDirect(data);
        //$scope.AttorneyObj = new Object();
        //$scope.AttorneyObj.OrderID = $scope.OrderId;
        //$scope.AttorneyObj.OrderFirmAttorneyId = 0;
        //$scope.AttorneyObj.Name = data.FirstName + ', ' + data.LastName;
        //$scope.AttorneyObj.OrderFirmAttorneyId = 0;
        //$scope.AttorneyObj.AttyID = data.AttyID;
        //$scope.AttorneyObj.FirmName = data.FirmName;
        //$scope.AttorneyObj.FirmID = data.FirmId;
        //$scope.AttorneyObj.IsPatientAttorney = 0;
        //$scope.AttorneyObj.OppSide = 0;
        //$scope.AttorneyObj.Represents = '';
        //$scope.AttorneyObj.Notes = '';
        //$scope.popupTitle = 'Add Opposite Attorney';
        //angular.element("#modal_form_editoppositeattorney").modal('show');
    };


    $scope.UpdateOrderFirmAttorney = function () {
        var promise = Step5Service.UpdateOrderFirmAttorney($scope.AttorneyObj);
        promise.success(function (response) {
            $scope.GetOrderWizardStep5AttorneyRecords($scope.OrderId);
            angular.element("#modal_form_editoppositeattorney").modal('hide');
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.AddOrderFirmAttorney = function () {
        var promise = Step5Service.AddOrderFirmAttorney($scope.AttorneyObj);
        promise.success(function (response) {
            $scope.GetOrderWizardStep5AttorneyRecords($scope.OrderId);
            angular.element("#modal_form_editoppositeattorney").modal('hide');
            angular.element("#modal_form_addoppositeattorney").modal('hide');
            if (!isNullOrUndefinedOrEmpty($scope.CurrentEvent)) {
                var table = $('#tblAddOppositeAttorney').DataTable();
                var data = table.row($($scope.CurrentEvent).parents('tr')).data();
                table.row($($scope.CurrentEvent).parents('tr')).remove();
                table.draw();
            }
        });
        promise.error(function (data, statusCode) {
        });
    };



    $scope.DeleteOppositeAttorney = function ($event) {
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
                    tblOppositeAttorney = $('#tblOppositeAttorney').DataTable();
                    var data = tblOppositeAttorney.row($($event.target).parents('tr')).data();
                    var promise = Step5Service.DeleteOppositeAttorney(data.OrderFirmAttorneyId);
                    promise.success(function (response) {
                        $scope.GetOrderWizardStep5AttorneyRecords($scope.OrderId);
                    });
                    promise.error(function (data, statusCode) {
                    });
                }
                bootbox.hideAll();
            }
        });


    };

    $scope.GetOrderWizardStep5AttorneyRecords = function (OrderId) {
        var promise = Step5Service.GetOrderWizardStep5AttorneyRecords(OrderId);
        promise.success(function (response) {
            $scope.OrderOppositeAttorneyList = response.Data;
            bindtblOppositeAttorney();
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.AddOppositeAttorney = function () {
        $scope.IsNewAttorney = false;
        $scope.searchCondition = 1;
        $scope.step5form.$setPristine();
        $scope.AttorneyList = [];
        // $('tblAddOppositeAttorney').DataTable({ searching: false });
        
        // alert("tt");
        // angular.element("#tblAddOppositeAttorney_filter").hide();        
        binAttorneyBlank();
        $scope.ShowAddNewAttorney = true;
        
        angular.element("#modal_form_addoppositeattorney").find("input[type=text],input[type=search]").val('');        
        angular.element("#modal_form_addoppositeattorney").modal('show');
    };
    

    $scope.BindAttorneyList = function () {
        if (!isNullOrUndefinedOrEmpty($scope.searchText)) {
            var promise = Step5Service.GetAttorneyListWithSearch($scope.searchCriteria, $scope.searchCondition, $scope.searchText, $scope.OrderId, $rootScope.CompanyNo);
            promise.success(function (response) {
                $scope.AttorneyList = response.Data;
                binAttorney();
            });
            promise.error(function (data, statusCode) {
            });
        }
        else {
            toastr.error("Search Text Required");
        }
        $scope.ShowAddNewAttorney = false;
    };

    $scope.SaveAttorneyRecord = function (form) {
        angular.element("#modal_form_addoppositeattorney").modal('hide');
        return true;//Temporary

        if (form.$valid) {
            $scope.OrderStep5Obj.EmpId = $scope.EmpId;
            $scope.OrderStep5Obj.UserAccessId = $scope.UserAccessId;

            if ($scope.OrderId > 0) {
                $scope.OrderStep5Obj.OrderId = $scope.OrderId;
                var promise = Step5Service.UpdateOrderAttorneyRecord($scope.OrderStep5Obj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_AttorneyOfRecord").modal('hide');
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            } else {

                var promise = Step5Service.InsertOrderAttorneyRecord($scope.OrderStep5Obj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_AttorneyOfRecord").modal('hide');
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

    $scope.SubmitStep5 = function (form) {
        if (form.$valid) {
            //$state.go("EditOrder", { OrderId: $scope.OrderId, Step: $rootScope.Enum.OrderWizardStep.Step6 }); 
            $scope.MoveNext();
        }
    };

    $scope.NewAttorney = function () {
        angular.element("#modal_form_addoppositeattorney").modal('hide');
        $scope.IsNewAttorney = true;
        $scope.IsNewFirm = false;
        bindFirmDropDown();
        $scope.NewAttorneyform.$setPristine();
        angular.element("#modal_NewAttorney").modal('show');
        $scope.AttyObj = new Object();
    };

    $scope.AddNewFirm = function (val) {
        if (val == 1) {
            $scope.IsNewFirm = true;
            $scope.AttyObj.FirmID = "Add New Firm";
        }
        else {
            $scope.IsNewFirm = false;
            $scope.AttyObj.FirmID = "";
        }
        $scope.AttyObj.FirmName = "";
        $scope.AttyObj.City = "";

    };

    $scope.SaveAttorney = function () {
        
        $scope.AttyObj.CreatedBy = $rootScope.LoggedInUserDetail.EmpId;
        var attorney = Step5Service.InsertNewAttorneyFromStep5($scope.AttyObj);
        attorney.success(function (response) {
            if (response.Success) {
                debugger;
                $scope.AttyObj.AttyID = response.str_ResponseData;
                $scope.mode = 'add';
                $scope.AttorneyObj = new Object();
                $scope.AttorneyObj.OrderID = $scope.OrderId;
                $scope.AttorneyObj.OrderFirmAttorneyId = 0;
                $scope.AttorneyObj.Name = $scope.AttyObj.FirstName + ', ' + $scope.AttyObj.LastName;
                $scope.AttorneyObj.OrderFirmAttorneyId = 0;
                $scope.AttorneyObj.AttyID = $scope.AttyObj.AttyID;
                $scope.AttorneyObj.FirmName = $scope.AttyObj.FirmName;
                $scope.AttorneyObj.FirmID = $scope.AttyObj.FirmID;
                $scope.AttorneyObj.IsPatientAttorney = 0;
                $scope.AttorneyObj.OppSide = 0;
                $scope.AttorneyObj.Represents = '';
                $scope.AttorneyObj.Notes = '';
                $scope.popupTitle = 'Add Additional Attorney';
                angular.element("#modal_NewAttorney").modal('hide');
                // angular.element("#modal_form_editAdditionalattorney").modal('show');



                angular.element("#modal_form_addoppositeattorney").find("input[type=text],input[type=search]").val('');
                angular.element("#modal_form_addoppositeattorney").modal('show');

                $scope.searchCriteria = "a.AttyID";
                $scope.searchCondition = 1;
                $scope.searchText = $scope.AttorneyObj.AttyID;
                $scope.BindAttorneyList();
            }
        });
        attorney.error(function (data, statusCode) {
        });
    };
    $scope.SaveNewAttorney = function (form) {
        if (form.$valid) {
            if ($scope.IsNewFirm) {
                $scope.AttyObj.EntBy = $rootScope.LoggedInUserDetail.EmpId;
                var promise = Step5Service.InsertNewFirmFromStep5($scope.AttyObj);
                promise.success(function (response) {
                    if (response.Success) {
                        $scope.AttyObj.FirmID = response.str_ResponseData;
                        $scope.SaveAttorney();
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else {
                $scope.SaveAttorney();
            }
        }

    };
    $scope.ChangeFirmDropDown = function () {
        if (!$scope.IsNewFirm && !isNullOrUndefinedOrEmpty($scope.AttyObj.FirmID)) {
            $scope.GetFirmDetailByFirmID($scope.AttyObj.FirmID);
        }
    };

    $scope.GetFirmDetailByFirmID = function (FirmId) {
        var promise = CommonServices.GetFirmDetailByFirmID(FirmId);
        promise.success(function (response) {
            if (response.Success) {
                $scope.AttyObj.FirmID = response.Data[0].FirmID;
                $scope.AttyObj.FirmName = response.Data[0].FirmName;
                $scope.AttyObj.City = response.Data[0].City;
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    //#endregion

    //#region Method

    function bindFirmDropDown() {
        var promise = CommonServices.FirmForDropdown('');
        promise.success(function (response) {
            $scope.ddlFirmList = response.Data;
        });
        promise.error(function (data, statusCode) {
        });
    }

    function bindtblOppositeAttorney() {

        if ($.fn.DataTable.isDataTable("#tblOppositeAttorney")) {
            $('#tblOppositeAttorney').DataTable().destroy();
        }

        var table = $('#tblOppositeAttorney').DataTable({
            data: $scope.OrderOppositeAttorneyList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "paging": false,
            "info": false,
            "searching": false,
            "columns": [
                {
                    "title": "Attorney Name",
                    "className": "dt-left",
                    "sorting": "false",
                    "width": "12%",
                    "render": function (data, type, row) {
                        return row.AttorneyLastName + ' , ' + row.AttorneyFirstName;
                    }
                },
                {
                    "title": "Atty ID",
                    "className": "dt-left",
                    "data": "AttyID",
                    "sorting": "false",
                    "width": "6%"
                },
                {
                    "title": "Firm ID",
                    "className": "dt-left",
                    "data": "FirmID",
                    "sorting": "false",
                    "width": "6%"
                },
                {
                    "title": "Firm Name",
                    "className": "dt-left",
                    "data": "FirmName",
                    "sorting": "false",
                    "width": "12%"
                },
                {
                    "title": "Represents",
                    "className": "dt-left",
                    "data": "Represents",
                    "sorting": "false",
                    "width": "22%"
                },
                {
                    "title": "Patient Attorney",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsPatientAttorney",
                    "width": "6%",
                    "render": function (data, type, row) {
                        return (row.IsPatientAttorney) ? "<label class='label bg-success-400'>Yes</label>" : "<label class='label bg-danger-400'>No</label>";
                    }
                },
                {
                    "title": "OppSide",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "OppSide",
                    "width": "6%",
                    "render": function (data, type, row) {
                        return (row.OppSide) ? "<label class='label bg-success-400'>Yes</label>" : "<label class='label bg-danger-400'>No</label>";
                    }
                },
                {
                    "title": "Notes",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "Notes",
                    "width": "20%"
                }
                , {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer'  ng-click='GetOppositeAttorneyForEditMode($event)' title='Edit' data-toggle='tooltip' data-placement='left' tooltip> <i  class='icon-pencil3' ></i></a> ";

                        strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteOppositeAttorney($event)'  title='Delete' data-toggle='tooltip' data-placement='left' tooltip>  <i  class='icon-bin cursor-pointer'></i> </a>";

                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblOppositeAttorney').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function binAttorneyBlank() {

        if ($.fn.DataTable.isDataTable("#tblAddOppositeAttorney")) {
            $('#tblAddOppositeAttorney').DataTable().destroy();
        }

        var table = $('#tblAddOppositeAttorney').DataTable({
            data: $scope.AttorneyList,
            "bDestroy": true,
            "dom": '<"top pull-left "><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "oLanguage": {
                "sSearch": "Refine Search:"
            },
            "columns": [
                {
                    "title": "Attorney Name",
                    "className": "dt-left",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return row.FirstName + ' ' + row.LastName;
                    }
                },
                {
                    "title": "Atty ID",
                    "className": "dt-left",
                    "data": "AttyID",
                    "sorting": "false"
                },
                {
                    "title": "Firm ID",
                    "className": "dt-left",
                    "data": "FirmId",
                    "sorting": "false"
                },
                {
                    "title": "Firm Name",
                    "className": "dt-left",
                    "data": "FirmName",
                    "sorting": "false"
                }
                , {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer'  ng-click='GetOppositeAttorneyForAddMode($event)' title='Add'> <i  class='icon-add' ></i></a> ";

                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblAddOppositeAttorney').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).attr("ng-dblclick", "GetOppositeAttorneyForAddMode($event)");
                $compile(angular.element(nRow))($scope);
                //$compile(angular.element(nRow).contents())($scope);
            }
        });
    };
    function binAttorney() {

        if ($.fn.DataTable.isDataTable("#tblAddOppositeAttorney")) {
            $('#tblAddOppositeAttorney').DataTable().destroy();
        }

        var table = $('#tblAddOppositeAttorney').DataTable({
            data: $scope.AttorneyList,
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
                {
                    "title": "Attorney Name",
                    "className": "dt-left",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return row.FirstName + ' ' + row.LastName;
                    }
                },
                {
                    "title": "Atty ID",
                    "className": "dt-left",
                    "data": "AttyID",
                    "sorting": "false"
                },
                {
                    "title": "Firm ID",
                    "className": "dt-left",
                    "data": "FirmId",
                    "sorting": "false"
                },
                {
                    "title": "Firm Name",
                    "className": "dt-left",
                    "data": "FirmName",
                    "sorting": "false"
                }
                , {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer'  ng-click='GetOppositeAttorneyForAddMode($event)' title='Add'> <i  class='icon-add' ></i></a> ";

                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblAddOppositeAttorney').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).attr("ng-dblclick", "GetOppositeAttorneyForAddMode($event)");
                $compile(angular.element(nRow))($scope);
                //$compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.OrderStep5Obj = new Object();
        $scope.OrderStep5Obj.OrderId = $scope.OrderId;
        $scope.AttorneyObj = new Object();
    }

    //#endregion

    angular.element(document).ready(function () {
        init();        
    });


});