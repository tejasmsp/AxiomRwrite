app.controller('ManageFormAndRequestedFormController', function ($scope, $rootScope, $stateParams, AttorneyService, FirmServices, $state, notificationFactory, LocationServices, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);
    $scope.title = '';
    $scope.parentForm = "";

    //#region Events

    $scope.DeleteLocationForm = function ($event) {

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
                    var table = $('#tblLocationForm').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    if ($scope.parentForm == "Location") {
                        var promise = LocationServices.DeleteLocationForm(row.LocformID);
                    }
                    else if ($scope.parentForm == "Attorney") {
                        var promise = AttorneyService.DeleteAttorneyForm(row.AttyformID);
                    }
                    else if ($scope.parentForm == "Firms") {
                        var promise = FirmServices.DeleteFirmForm(row.FirmformID);
                    }
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.successDelete('');
                            if ($scope.parentForm == "Attorney") {
                                GetAttorneyFormsList();
                            }
                            else if ($scope.parentForm == "Location") {
                                GetLocationFormsList();
                            }
                            else if ($scope.parentForm == "Firms") {
                                GetFirmFormsList();
                            }

                            bootbox.hideAll();
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function (response) { toastr.error(response.Message[0]); });
                }
                else {

                }
            }
        });


    };


    // THIS WILL OPEN FORM FOR Firms
    $scope.$parent.$watch('ShowFirmForm', function (newVal, oldVal) {
        $scope.parentForm = "Firms";
        if (newVal) {
            if ($scope.$parent.FirmFormObj.FirmID != undefined && $scope.$parent.FirmFormObj.FirmID != null && $scope.$parent.FirmFormObj.FirmID) {
                $scope.objFirmForm = $scope.$parent.FirmFormObj;
                $scope.title = $scope.objFirmForm.IsRequestForm ? 'Manage Request Form' : 'Mange Form';
                OpenFirmForm();
            }
        }
    }, true);

    // THIS WILL OPEN FORM FOR Attorney
    $scope.$parent.$watch('ShowAttorneyForm', function (newVal, oldVal) {
        $scope.parentForm = "Attorney";

        if (newVal) {
            if ($scope.$parent.objAttorney.AttyID != undefined && $scope.$parent.objAttorney.AttyID != null && $scope.$parent.objAttorney.AttyID) {
                $scope.objAttorneyForm = $scope.$parent.AttorneyFormobj;
                $scope.title = $scope.objAttorneyForm.IsRequestForm ? 'Manage Request Form' : 'Mange Form';

                OpenAttorneyForm();
            }
        }
    }, true);

    // THIS WILL OPEN FORM FOR Location
    $scope.$parent.$watch('ShowLocationForm', function (newVal, oldVal) {
        $scope.parentForm = "Location";
        if (newVal) {
            if ($scope.$parent.ObjOpenMergeLocation.LocID != undefined && $scope.$parent.ObjOpenMergeLocation.LocID != null && $scope.$parent.ObjOpenMergeLocation.LocID) {
                $scope.parentLocationObj = $scope.$parent.ObjOpenMergeLocation;
                $scope.title = $scope.parentLocationObj.IsRequestForm ? 'Manage Request Form' : 'Mange Form';
                $scope.OpenLocationForm();
            }
        }

    }, true);

    $scope.$watch('mytree.currentNode', function (newObj, oldObj) {
        if ($scope.mytree && angular.isObject($scope.mytree.currentNode) && !$scope.mytree.currentNode.isfolder && newObj != oldObj) {
            bootbox.confirm({
                message: "Are you sure you want to add this document?",
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
                        if ($scope.parentForm == "Attorney") {
                            var model = {
                                AttyformID: 0,
                                FormType: $scope.objAttorneyForm.FormType,
                                IsRequestForm: $scope.objAttorneyForm.IsRequestForm,
                                AttyID: $scope.objAttorneyForm.AttyID,
                                FormID: 0,
                                FolderPath: $scope.mytree.currentNode.breadcrumbs,
                                // CreatedBy: $rootScope.LoggedInUserDetail.UserAccessId,
                                CreatedBy: $rootScope.LoggedInUserDetail.EmpId,
                                FolderName: $scope.mytree.currentNode.breadcrumbs.split('>')[0],
                                DocFileName: $scope.mytree.currentNode.title
                            };
                        }
                        else if ($scope.parentForm == "Location") {
                            var model = {
                                LocformID: 0,
                                IsRequestForm: $scope.parentLocationObj.IsRequestForm,
                                LocID: $scope.parentLocationObj.LocID,
                                FormID: 0,
                                FolderPath: $scope.mytree.currentNode.breadcrumbs,
                                // CreatedBy: $rootScope.LoggedInUserDetail.UserAccessId,
                                CreatedBy: $rootScope.LoggedInUserDetail.EmpId,
                                FolderName: $scope.mytree.currentNode.breadcrumbs.split('>')[0],
                                DocFileName: $scope.mytree.currentNode.title
                            };
                        }
                        else if ($scope.parentForm == "Firms") {
                            var model = {
                                FirmformID: 0,
                                IsRequestForm: $scope.objFirmForm.IsRequestForm,
                                IsFaceSheet: $scope.objFirmForm.IsFaceSheet,
                                FirmID: $scope.objFirmForm.FirmID,
                                FormID: 0,
                                FolderPath: $scope.mytree.currentNode.breadcrumbs,
                                // CreatedBy: $rootScope.LoggedInUserDetail.UserAccessId,
                                CreatedBy: $rootScope.LoggedInUserDetail.EmpId,
                                FolderName: $scope.mytree.currentNode.breadcrumbs.split('>')[0],
                                DocFileName: $scope.mytree.currentNode.title
                            };
                        }

                        if ($scope.parentForm == "Attorney") {
                            var promise = AttorneyService.InsertAttorneyForm(model);
                        }
                        else if ($scope.parentForm == "Location") {
                            var promise = LocationServices.InsertLocationForm(model);
                        }
                        else if ($scope.parentForm == "Firms") {
                            var promise = FirmServices.InsertFirmForm(model);
                        }


                        promise.success(function (response) {
                            if (response.Success) {
                                toastr.success('The document added successfully.');

                                if ($scope.parentForm == "Attorney") {
                                    GetAttorneyFormsList();
                                }
                                else if ($scope.parentForm == "Location") {
                                    GetLocationFormsList();
                                }
                                else if ($scope.parentForm == "Firms") {
                                    GetFirmFormsList();
                                }

                                bootbox.hideAll();
                            }
                            else {
                                toastr.error(response.Message[0]);
                                bootbox.hideAll();
                            }
                        });
                        promise.error(function (response) {
                            toastr.error(response.Message[0]);
                            bootbox.hideAll();
                        });

                    }
                    else {
                        bootbox.hideAll();
                    }
                }
            });
        }
    }, false);


    $scope.OpenLocationForm = function () {
        angular.element("#modal_LocationFormList").modal('show');
        GetLocationFormsList();
        GetFormsAndDirectoryList();
        $scope.$parent.ShowLocationForm = false;
    };

    function OpenAttorneyForm() {
        angular.element("#modal_LocationFormList").modal('show');
        GetAttorneyFormsList();
        GetFormsAndDirectoryList();
        $scope.$parent.ShowAttorneyForm = false;
    };

    function OpenFirmForm() {
        angular.element("#modal_LocationFormList").modal('show');
        GetFormsAndDirectoryList();
        GetFirmFormsList();
        $scope.$parent.ShowFirmForm = false;
    };

    //#endregion 

    //#region Methods

    function GetFormsAndDirectoryList() {

        var promise = LocationServices.GetFormsAndDirectoryList();
        promise.success(function (response) {
            $scope.FormsAndDirectoryList = response.Data;
        });
        promise.error(function (data, statusCode) {
        });
    };

    function GetLocationFormsList() {

        var promise = LocationServices.GetLocationFormsList($scope.parentLocationObj.LocID, $scope.parentLocationObj.IsRequestForm);
        promise.success(function (response) {

            $scope.LocationFormsList = response.Data;
            bindLocationFormsList();
        });
        promise.error(function (data, statusCode) {
        });
    };

    function GetAttorneyFormsList() {


        var promise = AttorneyService.GetAttorneyFormsList($scope.objAttorneyForm.AttyID, $scope.objAttorneyForm.FormType);
        promise.success(function (response) {

            $scope.LocationFormsList = response.Data;
            bindLocationFormsList();
        });
        promise.error(function (data, statusCode) {
        });
    };

    function GetFirmFormsList() {
        var promise = FirmServices.GetFirmFormsList($scope.objFirmForm.FirmID,  $scope.objFirmForm.IsFaceSheet,$scope.objFirmForm.IsRequestForm);
        promise.success(function (response) {

            $scope.LocationFormsList = response.Data;
            bindLocationFormsList();
        });
        promise.error(function (data, statusCode) {
        });
    };

    function bindLocationFormsList() {

        if ($.fn.DataTable.isDataTable("#tblLocationForm")) {
            $('#tblLocationForm').DataTable().destroy();
        }

        var table = $('#tblLocationForm').DataTable({
            data: $scope.LocationFormsList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [5, 10, 20, 50, 100, 200],
            "pageLength": 5,
            "stateSave": false,
            "columns": [
                {
                    "title": "Folder Name",
                    "className": "dt-left",
                    "data": "FolderName",
                    "sorting": "true"
                },
                {
                    "title": "File",
                    "className": "dt-left",
                    "data": "DocFileName",
                    "sorting": "true"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteLocationForm($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                //var dataTable = $('#tblLocationForm').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    //#endregion    

    function init() {

    };

    init();


});