app.controller('AddOrUpdateAttorneyController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, FirmServices, AttorneyService, EmployeeServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;

    //Page Rights//    
    $rootScope.CheckIsPageAccessible("Admin", "Attorneys", "View");
    $scope.IsUserCanAddAttorney = $rootScope.isSubModuleAccessibleToUser('Admin', 'Attorneys', 'Add Attorney');
    $scope.IsUserCanEditAttorney = $rootScope.isSubModuleAccessibleToUser('Admin', 'Attorneys', 'Edit Attorney');
    //-----

    $rootScope.pageTitle = $stateParams.AttyID ? "Edit Attorney" : "Add Attorney";
    function init() {
        $scope.EnableGenerteCodeValidation = false;
        $scope.objAttorney = new Object();
        CreateObjects();
        $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
        $scope.UserEmployeeID = $rootScope.LoggedInUserDetail.EmpId;
        AttorneyById($stateParams.AttyID);
        $scope.GetAttorneyAssistantContactList($stateParams.AttyID);
        getUserDetails();
        // alert($scope.UserAccessId);
        // alert($scope.UserEmployeeID);
    };

    $scope.AddAssistantContact = function () {
        var AssistantContactObj = new Object();
        AssistantContactObj.AdditionalContactsID = 0;
        AssistantContactObj.AssistantName = "";
        AssistantContactObj.AssistantEmail = "";
        AssistantContactObj.isAttorney = false;
        $scope.AttorneyAssistantContactList.push(AssistantContactObj);
    };

    $scope.RemoveAssistantContact = function (AssistantContactObj) {
        var index = $scope.AttorneyAssistantContactList.indexOf(AssistantContactObj);
        if (index > -1) {
            $scope.AttorneyAssistantContactList.splice(index, 1);
        }
    };

    $scope.OpenAddForms = function ($event, Type) {
        // Type
        // 1 Have Own Authorization
        // 2 Have Own Facesheet
        // 3 Have Own Notice
        // 4 Location Specific Request
        if ($event.target.checked) {
            $scope.AttorneyFormobj = { AttyID: $scope.objAttorney.AttyID, Name: ($scope.objAttorney.FirstName + $scope.objAttorney.LastName), IsRequestForm: true, FormType: Type };
            $scope.ShowAttorneyForm = true;
        }
    }

    $scope.GenerateNewAttorneyCode = function (form) {
        if (form.FirstName.$valid && form.LastName.$valid) {
            var promise = CommonServices.GetNewSequenceNumber('Attorney', $scope.objAttorney.FirstName, $scope.objAttorney.LastName);
            promise.success(function (response) {
                $scope.objAttorney.AttyID = '';
                if (response.Success) {
                    $scope.objAttorney.AttyID = response.Data[0];
                    $scope.EnableGenerteCodeValidation = false;
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) { });
        }
        else {
            $scope.objAttorney.AttyID = '';
            $scope.EnableGenerteCodeValidation = true;
        }
    };

    $scope.GetAttorneyAssistantContactList = function (AttyID) {
        var promise = AttorneyService.GetAttorneyAssistantContactList($scope.UserAccessId, AttyID);
        promise.success(function (response) {
            if (response.Success) {
                $scope.AttorneyAssistantContactList = response.Data;
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.FillFirmDetail = function (FirmID) {
        var promise = FirmServices.GetFirmById(FirmID);
        promise.success(function (response) {

            $scope.objAttorney.FirmID = response.Data[0].FirmID;
            $scope.objAttorney.FirmName = response.Data[0].FirmName;
            $scope.objAttorney.Street1 = response.Data[0].Street1;
            $scope.objAttorney.Street2 = response.Data[0].Street2;
            $scope.objAttorney.City = response.Data[0].City;
            $scope.objAttorney.State = response.Data[0].State;
            $scope.objAttorney.Zip = response.Data[0].Zip;
            $scope.objAttorney.FAreaCode1 = response.Data[0].AreaCode1;
            $scope.objAttorney.FPhoneNo = response.Data[0].PhoneNo;
            $scope.objAttorney.FAreaCode2 = response.Data[0].AreaCode2;
            $scope.objAttorney.FFaxNo = response.Data[0].FaxNo;
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.FirmSearch = function (event) {
        $scope.ShowSearchFirm = true;
    };

    function AttorneyById(AttyID) {
        if (AttyID != undefined && AttyID != "") {
            $scope.isEdit = true;
            var promise = AttorneyService.GetAttorneyByAttyIdForAttorney(AttyID);
            promise.success(function (response) {
                if (response.Data != null) {
                    $scope.objAttorney = response.Data[0];
                    SetCheckBoxStatus($scope.objAttorney.DocProductionPreference, $scope.DocumentProdctionOptions);
                    SetCheckBoxStatus($scope.objAttorney.LinkedContacts, $scope.PreferredContactOption);
                    SetCheckBoxStatus($scope.objAttorney.RequestSent, $scope.RequestSentOptions);
                }



                //SetCheckBoxStatus($scope.LocationObj.SendRequest, $scope.SendRequestOptions);                
                //SetCheckBoxStatus($scope.LocationObj.ReccanRequested, $scope.ReccanRequestedOptions);
                //SetCheckBoxStatus($scope.LocationObj.LinkedLocation, $scope.LinkedLocationOptions);

                //GetCSVForCheckBoxs($scope.SendRequestOptions);
                $scope.EnableGenerteCodeValidation = true;
            });
            promise.error(function (data, statusCode) {
            });
        } else {
            $scope.isEdit = false;
        }
    };

    function getUserDetails() {
        var promise = CommonServices.GetUserDetails();
        promise.success(function (response) {
            if (response.Success) {
                $scope.userDetailList = response.Data;
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    function SetCheckBoxStatus(csvValue, optionsList) {
        if (csvValue != null && csvValue != '') {
            csvValue = csvValue.toString().split('');
            angular.forEach($filter('filter')(optionsList, { 'IsChecked': false }), function (item, key) {
                item.IsChecked = (csvValue.indexOf(item.Value) > -1);
            });
        }
    };

    function CreateObjects() {

        $scope.objAttorney =
            {
                'AttyID': null,
                'FirmID': null,
                'FirmName': null,
                'FirstName': null,
                'LastName': null,
                'Street1': null,
                'Street2': null,
                'City': null,
                'State': null,
                'Zip': null,
                'FAreaCode1': null,
                'FPhoneNo': null,
                'FAreaCode2': null,
                'FFaxNo': null,
                'AreaCode1': null,
                'PhoneNo': null,
                'SamePhone': null,
                'AreaCode2': null,
                'FaxNo': null,
                'SameFax': null,
                'StateBarNo': null,
                'Email': null,
                'AttyType': null,
                'Contact': null,
                'ContactAsstName': null,
                'DocCapForRecords': null,
                'DocCapForFilms': null,
                'DocProductionPreference': null,
                'OwnOrganization': null,
                'OwnFacesheet': null,
                'OwnNotice': null,
                'OwnNoticeCertified': null,
                'LinkRequest': null,
                'MonthlyBilling': null,
                'RequestSent': null,
                'LinkedContacts': null,
                'ContactEmail': null,
                'ContactName': null,
                'isDisable': null,
                'ClientOf': null,
                'ClientOfFirstName': null,
                'ClientOfLastName': null,
                'SalesRep': null,
                'SalesRepFirstName': null,
                'SalesRepLastName': null,
                'Warning': null,
                'Notes': null,

                //-------------------------
                'Rating': 0,
                'FedAdmNo': 0,
                'Salutation': 0,
                'CommType': 0,
                'Password': 0,
                'Dormant': 0,
                'FullAccess': 0,
                'Type': 0
            };


        $scope.AttorneyTypeList =
            [
                { Value: 0, Title: 'Attorney' }
                , { Value: 1, Title: 'Paralegal' }
                , { Value: 2, Title: 'Adjuster' }
                , { Value: 3, Title: 'Other' }
            ];

        $scope.SalutationList =
            [
                { Value: 0, Title: 'Select' }
                , { Value: 1, Title: 'Mr.' }
                , { Value: 2, Title: 'Ms.' }
                , { Value: 3, Title: 'Mrs.' }
                , { Value: 4, Title: 'Miss' }
                , { Value: 5, Title: 'Dr.' }
            ];
        $scope.PreferredContactOption =
            [
                { Value: '0', Titile: 'Attorney', IsChecked: false }
                , { Value: '1', Titile: 'Contact', IsChecked: false }
            ];

        $scope.RequestSentOptions =
            [
                { Value: '0', Title: 'None', IsChecked: false }
                , { Value: '4', Title: 'Process Server', IsChecked: false }
                , { Value: '5', Title: 'Certified Mail', IsChecked: false }
            ];
        $scope.DocumentProdctionOptions =
            [
                { Value: '0', Title: '2 Hole', IsChecked: false }
                , { Value: '1', Title: '3 Hole', IsChecked: false }
                , { Value: '2', Title: 'Digital Only', IsChecked: false }
                , { Value: '3', Title: 'CD', IsChecked: false }
                , { Value: '4', Title: '2 Hole Unbound', IsChecked: false }
                , { Value: '5', Title: '3 Hole Unbound', IsChecked: false }
                , { Value: '6', Title: 'No Hole No Bound', IsChecked: false }
            ];

        var _companylist = CommonServices.GetCompanyDropDown();
        _companylist.success(function (response) {
            $scope.Companylist = response.Data;
        });
        _companylist.error(function (data, statusCode) { });
    }

    $scope.SaveAttorney = function (form) {

        if (form.$valid) {
            $scope.objAttorney.DocProductionPreference = GetCSVForCheckBoxs($scope.DocumentProdctionOptions);
            $scope.objAttorney.LinkedContacts = GetCSVForCheckBoxs($scope.PreferredContactOption);
            $scope.objAttorney.RequestSent = GetCSVForCheckBoxs($scope.RequestSentOptions);
            $scope.objAttorney.AttorneyAssistantContactList = [];
            $scope.objAttorney.AttorneyAssistantContactList = $scope.AttorneyAssistantContactList;

            if (!$scope.isEdit) {
                $scope.objAttorney.Notes = $('#Notes').val();
                $scope.objAttorney.CompanyNo = $rootScope.CompanyNo;
                var promise = AttorneyService.InsertAttorney($scope.UserEmployeeID, $scope.UserAccessId, $scope.objAttorney);
                promise.success(function (response) {
                    if (response.Success) {
                        notificationFactory.customSuccess("Attorney saved successfullly.");
                        $state.transitionTo('AttorneyDetail');
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else {
                $scope.objAttorney.Notes = $('#Notes').val();
                var promiseedit = AttorneyService.UpdateAttorney($scope.UserEmployeeID, $scope.UserAccessId, $scope.objAttorney);
                promiseedit.success(function (response) {
                    if (response.Success) {
                        notificationFactory.customSuccess("Attorney updated successfully.");
                        $state.transitionTo('AttorneyDetail');
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promiseedit.error(function (data, statusCode) {
                });
            }
        }
    };

    function GetCSVForCheckBoxs(optionsList) {
        var selectArray = [];
        angular.forEach($filter('filter')(optionsList, { 'IsChecked': true }), function (item, key) {
            selectArray.push(item.Value);
        });
        return selectArray.join(',');
    }

    $scope.RadioCheckChange = function (item, optionsList) {
        angular.forEach($filter('filter')(optionsList, { 'IsChecked': true }), function (record, key) {
            record.IsChecked = (item.Value == record.Value);
            if (record.Title == "Digital Only") {
                if (record.IsChecked) {
                    $scope.ShowAdditionalContacts();
                }
            }
        });
    };

    // #region --- ADDITIONAL CONTACTS ---

    $scope.BindAdditionalContact = function () {
        var promise = FirmServices.GetAdditionalContacts($scope.objAttorney.AttyID, "Attorney");
        promise.success(function (response) {
            if (response.Success) {

                $scope.AdditionalContactsList = response.Data;
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) { });
    }

    $scope.ShowAdditionalContacts = function () {
        $scope.BindAdditionalContact();
        angular.element("#modal_AdditionalContacts").modal('show');
        $scope.frmAdditionalContacts.$setPristine();
        $scope.ClearAdditionalContact();
    };

    $scope.DeleteAdditionalContact = function (ContactID) {

        bootbox.confirm({
            message: "Are you sure you want to delete this record ?",
            buttons: {
                confirm: { label: 'Yes', className: 'btn-success' },
                cancel: { label: 'No', className: 'btn-danger' }
            },
            callback: function (result) {

                if (result == true) {
                    var promise = FirmServices.DeleteAdditionalContact(ContactID);
                    promise.success(function (response) {
                        if (response.Success) {

                            notificationFactory.customSuccess("Record deleted successfully.");
                            $scope.BindAdditionalContact();
                            $scope.frmAdditionalContacts.$setPristine();
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function (data, statusCode) { });
                }
                else {
                }
            }
        });
    };

    $scope.SaveAdditionalContacs = function (form) {
        if (form.$valid) {
            $scope.objAdditionalContacts.FirmID = $scope.objAttorney.AttyID;
            var promise = FirmServices.SaveAdditionalContacts($scope.objAdditionalContacts);
            promise.success(function (response) {
                if (response.Success) {
                    $scope.BindAdditionalContact();
                    $scope.frmAdditionalContacts.$setPristine();
                    $scope.ClearAdditionalContact();
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) { });
        }
    };

    $scope.ClearAdditionalContact = function () {
        $scope.objAdditionalContacts = new Object();
        $scope.objAdditionalContacts.FirstName = '';
        $scope.objAdditionalContacts.LastName = '';
        $scope.objAdditionalContacts.Email = '';
        $scope.objAdditionalContacts.ContactFor = 'Attorney';
        $("#FirstName").focus();
    };
    //#endregion

    //#region --- EMPLOYEE POPUP ---

    function bindEmployeeList() {
        if ($.fn.DataTable.isDataTable("#tblEmployee")) {
            $('#tblEmployee').DataTable().destroy();
        }

        var table = $('#tblEmployee').DataTable({
            data: $scope.Employeelist,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [05, 10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Employee Id",
                    "className": "dt-left",
                    "data": "EmpId",
                    "width": "10%",
                    "sorting": "false"
                },
                {
                    "title": "Name",
                    "className": "dt-left",
                    "data": "EmployeeName",
                    "width": "15%",
                    "sorting": "false"
                },
                {
                    "title": "Email",
                    "className": "dt-left",
                    "data": "UserName",
                    "width": "15%",
                    "sorting": "false"
                },
                {
                    "title": "Status",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsApproved",
                    "width": "5%",
                    "render": function (data, type, row) {
                        return (row.IsApproved) ? "<label class='label bg-success-400'>Active</label>" : "<label class='label bg-danger-400'>InActive</label>";
                    }
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "5%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = '<a class="ico_btn cursor - pointer" ng-click="SelectEmployee($event)" title="Select"> <i class="icon-checkmark4"></i></a>';
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblEmployee').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    $scope.SelectEmployee = function ($event) {
        var table = $('#tblEmployee').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        if ($scope.EmpTypeSelection == "client") {
            $scope.objAttorney.ClientOf = row.EmpId;
            $scope.objAttorney.ClientOfFirstName = row.FirstName;
            $scope.objAttorney.ClientOfLastName = row.LastName;
        }
        else {
            $scope.objAttorney.SalesRep = row.EmpId;
            $scope.objAttorney.SalesRepFirstName = row.FirstName;
            $scope.objAttorney.SalesRepLastName = row.LastName;
        }
        angular.element("#modal_Employee").modal('hide');
    }

    $scope.ShowEmployeePopup = function (type) {

        $scope.EmpTypeSelection = type;
        if (type == "client")
            $scope.EmployeeTitle = 'Select Client Of'
        else
            $scope.EmployeeTitle = 'Select Sales Representative'
        angular.element("#modal_Employee").modal('show');
        GetEmployeelist();
    }

    function GetEmployeelist() {
        var promise = EmployeeServices.GetEmployeeList($rootScope.CompanyNo);
        promise.success(function (response) {
            $scope.Employeelist = response.Data;
            bindEmployeeList();
            $("#tblEmployee_filter").find('input[type="search"]').focus();
        });
        promise.error(function (data, statusCode) {
        });
    }
    $scope.openAttorneyNoteClick = function () {
        var promise = CommonServices.GetAttorneyNotes($scope.objAttorney.AttyID);
        promise.success(function (response) {
            if (response.Success && response.Data.length > 0) {
                $scope.attorneyNotes = response.Data[0].Notes;
                getNotesList($scope.attorneyNotes);
            }
            angular.element("#modal_NotesForm").modal('show');

        });
        promise.error(function (data, statusCode) {
        });
    }

    function getNotesList(inputstr) {
        var span = document.createElement('span');
        var lines = inputstr.split('\n');
        var firstlineDisplay = false;
        for (var i = 0; i < lines.length; i++) {

            var isfound = lines[i].match(/:/g);
            var firstCharacter = lines[i].indexOf('[');

            if (isfound && isfound.length == 2 && firstCharacter === 0) {
                var timeFound = lines[i].search(' PM - ');
                if (timeFound == -1) {
                    timeFound = lines[i].search(' AM - ');
                }
                if (timeFound > -1) {

                    var lastCharcter = lines[i].lastIndexOf(']');
                    var strLastIndex = lines[i] ? lines[i].trim().length - 1 : -1;
                    if ((firstCharacter === 0) && (lastCharcter === strLastIndex)) {
                        var objData = lines[i].split("-");
                        if (objData && objData.length > 0) {
                            var empId = lines[i].split("-")[1].replace(']', '').trim();
                            var objuserDetail = $filter('filter')($scope.userDetailList, { 'EmpId': empId }, true);
                            if (objuserDetail && objuserDetail.length > 0) {
                                lines[i] = lines[i].replace(empId, objuserDetail[0].FirstName + ' ' + objuserDetail[0].LastName);
                            }
                        }

                        if (firstlineDisplay) {
                            lines[i] = "$line$".concat(lines[i]);
                        }
                        else {
                            lines[i] = "$firstline$".concat(lines[i]);
                        }
                    }
                    firstlineDisplay = true;
                }

            }
            span.innerText = lines[i];
            span.textContent = lines[i];
            lines[i] = span.innerHTML;
        }
        lines.join('<br />');

        $scope.notesList = angular.copy(lines);
    }
    //#endregion

    init();

});