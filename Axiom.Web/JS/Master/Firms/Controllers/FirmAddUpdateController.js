app.controller('FirmAddUpdateController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, EmployeeServices, FirmServices, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);

    $scope.isEdit = false;
    $rootScope.pageTitle = $stateParams.FirmID ? "Edit Firm" : "Add Firm";
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.UserEmployeeID = $rootScope.LoggedInUserDetail.EmpId;
    $scope.UserGUID = $rootScope.LoggedInUserDetail.UserId;

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Admin", "Firms", "View");
    $scope.IsUserCanEditFirm = $rootScope.isSubModuleAccessibleToUser('Admin', 'Firms', 'Edit Firm');
    $scope.IsUserCanAddFirm = $rootScope.isSubModuleAccessibleToUser('Admin', 'Firms', 'Add Firm'); 
   //-----
    $scope.RequestSentOptions =
        [
            { Value: '0', Title: 'None', IsChecked: false }
            , { Value: '4', Title: 'Process Server', IsChecked: false }
            , { Value: '5', Title: 'Certified Mail', IsChecked: false }
        ];

    //#region Events


    $scope.OpenAddForms = function ($event, Type) {
        
        // Type
        // 1 Have Own Authorization
        // 2 Have Own Facesheet
        // 3 Have Own Notice
        // 4 Location Specific Request
        var isRequestForm = 0, isFaceSheet = 0;
        if (Type == 1) {
            isRequestForm = false;
            isFaceSheet = false;
        }
        if (Type == 2) {
            isRequestForm = false;
            isFaceSheet = true;
        }
        

        if ($event.target.checked) {
            $scope.FirmFormObj = { FirmID: $scope.FirmObj.FirmID, Name: ($scope.FirmObj.FirmName), IsRequestForm: isRequestForm, IsFaceSheet: isFaceSheet };
            $scope.ShowFirmForm = true;
        }
    };

    function GetEmployeelist() {
        var promise = EmployeeServices.GetEmployeeList();
        promise.success(function (response) {
            $scope.Employeelist = response.Data;
            bindEmployeeList();
            $("#tblEmployee_filter").find('input[type="search"]').focus();
        });
        promise.error(function (data, statusCode) {
        });
    };

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
    };

    $scope.SelectEmployee = function ($event) {
        var table = $('#tblEmployee').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        if ($scope.EmpTypeSelection == "client") {
            $scope.FirmObj.ClientOf = row.EmpId;
            $scope.FirmObj.ClientOfFirstName = row.FirstName;
            $scope.FirmObj.ClientOfLastName = row.LastName;
        }
        else {
            $scope.FirmObj.SalesRep = row.EmpId;
            $scope.FirmObj.SalesRepFirstName = row.FirstName;
            $scope.FirmObj.SalesRepLastName = row.LastName;
        }
        angular.element("#modal_Employee").modal('hide');
    };

    $scope.RadioCheckChange = function (item, optionsList) {
        angular.forEach($filter('filter')(optionsList, { 'IsChecked': true }), function (record, key) {
            record.IsChecked = (item.Value == record.Value);
        });
    };
    $scope.SaveFirm = function (form) {
        if (form.$valid) {
            $scope.FirmObj.DocProductionPreference = GetCSVForCheckBoxs($scope.DocumentProdctionOptions);
            angular.forEach($filter('filter')($scope.RequestSentOptions, { 'IsChecked': true }), function (item, key) {
                $scope.FirmObj.RequestSent = item.Value;
            });
            if (!$scope.isEdit) {
                $scope.FirmObj.Notes = $('#Notes').val();
                var promise = FirmServices.InsertFirm($scope.FirmObj, $scope.UserEmployeeID, $scope.UserAccessId);
                promise.success(function (response) {
                    if (response.Success) {
                        toastr.success('Firm saved successfully.');
                        $state.transitionTo('Firm');
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else {
                $scope.FirmObj.ChngBy = $scope.UserEmployeeID;
                $scope.FirmObj.Notes = $('#Notes').val();
                var promiseedit = FirmServices.UpdateFirm($scope.FirmObj, $scope.UserEmployeeID, $scope.UserAccessId);
                promiseedit.success(function (response) {
                    if (response.Success) {
                        toastr.success('Firm updated successfully.');
                        $state.transitionTo('Firm');
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

    $scope.GenerateNewFirmCode = function (form) {
        if (form.FirmName.$valid) {

            var promise = CommonServices.GetNewSequenceNumber('Firm', $scope.FirmObj.FirmName, '');
            promise.success(function (response) {
                $scope.FirmObj.FirmID = '';
                if (response.Success) {
                    $scope.FirmObj.FirmID = response.Data[0];
                    $scope.EnableGenerteCodeValidation = false;
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) { });
        }
        else {
            $scope.FirmObj.FirmID = '';
            $scope.EnableGenerteCodeValidation = true;
        }
    };

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

    $scope.ShowAdditionalContacts = function () {
        $scope.BindAdditionalContact();
        angular.element("#modal_AdditionalContacts").modal('show');
        $scope.frmAdditionalContacts.$setPristine();
        $scope.ClearAdditionalContact();
    };

    $scope.BindAdditionalContact = function () {
        var promise = FirmServices.GetAdditionalContacts($scope.FirmObj.FirmID, "Firm");
        promise.success(function (response) {
            if (response.Success) {
                $scope.AdditionalContactsList = response.Data;
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) { });
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
                            toastr.success('Additional Contact deleted successfully.');
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
            $scope.objAdditionalContacts.FirmID = $scope.FirmObj.FirmID;
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
        $scope.objAdditionalContacts.ContactFor = 'Firm';
        $("#FirstName").focus();
    };

    $scope.FirmNameChange = function () {

        if (!$scope.isEdit) {
            $scope.FirmObj.FirmID = '';
        }
    };

    $scope.openFirmNoteClick = function () {
        var promise = CommonServices.GetFirmNotes($scope.FirmObj.FirmID);
        promise.success(function (response) {
            if (response.Success && response.Data.length > 0) {
                $scope.firmNotes = response.Data[0].Notes;
                getNotesList($scope.firmNotes);
            }
            angular.element("#modal_NotesForm").modal('show');

        });
        promise.error(function (data, statusCode) {
        });
    };

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
    $scope.OpenAssociatedFirmPopup = function () {
        $scope.ShowAssociatedFirm = true;
    }

    $scope.ShowEmployeePopup = function (type) {
        $scope.frmAdditionalContacts.$setPristine();
        $scope.EmpTypeSelection = type;
        if (type == "client")
            $scope.EmployeeTitle = 'Select Client Of'
        else
            $scope.EmployeeTitle = 'Select Sales Representative'
        angular.element("#modal_Employee").modal('show');
        GetEmployeelist();

    }
    //#region Methods

    $scope.GetFirmById = function (FirmId) {

        if (FirmId != undefined && FirmId != "") {
            $scope.isEdit = true;
            var promise = FirmServices.GetFirmById(FirmId);
            promise.success(function (response) {
                debugger;
                $scope.FirmObj = response.Data[0];
                if (!isNullOrUndefinedOrEmpty(response.Data[0].LastSalesCall)) {
                    $scope.FirmObj.LastSalesCall = $filter('date')(new Date(response.Data[0].LastSalesCall), $rootScope.GlobalDateFormat);
                }

                SetCheckBoxStatus($scope.FirmObj.DocProductionPreference, $scope.DocumentProdctionOptions);
                $scope.FirmObj.RequestSent = response.Data[0].RequestSent;
                if (!isNullOrUndefinedOrEmpty($scope.FirmObj.RequestSent)) {
                    angular.forEach($filter('filter')($scope.RequestSentOptions, { 'Value': $scope.FirmObj.RequestSent+'' }), function (item, key) {
                        item.IsChecked = true;
                    });
                }
            });
            promise.error(function (data, statusCode) { });
        } else {
            $scope.isEdit = false;
        }
    };

    function SetCheckBoxStatus(csvValue, optionsList) {
        if (csvValue != null && csvValue != '') {

            csvValue = csvValue.toString().split('');
            angular.forEach($filter('filter')(optionsList, { 'IsChecked': false }), function (item, key) {
                item.IsChecked = (csvValue.indexOf(item.Value) > -1);
            });
        }
    }

    function GetCSVForCheckBoxs(optionsList) {
        var selectArray = [];
        angular.forEach($filter('filter')(optionsList, { 'IsChecked': true }), function (item, key) {
            selectArray.push(item.Value);
        });
        return selectArray.join(',');
    }

    function dropdownbind() {

        var statelist = CommonServices.StateDropdown();
        statelist.success(function (response) {
            $scope.Statelist = response.Data;
        });
        // DepartmentD  
        var _departmentlist = CommonServices.DepartmentDropdown();
        _departmentlist.success(function (response) {
            $scope.Departmentlist = response.Data;
        });
        _departmentlist.error(function (data, statusCode) { });



        var _companylist = CommonServices.GetCompanyDropDown();
        _companylist.success(function (response) {
            $scope.Companylist = response.Data;
        });
        _companylist.error(function (data, statusCode) { });


        var _firmTypelist = CommonServices.FirmTypeForDropdown();
        _firmTypelist.success(function (response) {
            $scope.FirmTypelist = response.Data;
        });
        _firmTypelist.error(function (data, statusCode) { });

    }

    function CreateObjects() {

        $scope.FirmObj =
            {
                FirmID: null,
                FirmName: null,
                Rating: null,
                Street1: null,
                Street2: null,
                City: null,
                State: null,
                Zip: null,
                AreaCode1: null,
                PhoneNo: null,
                AreaCode2: null,
                FaxNo: null,
                Email: '',
                Region: null,
                Warning: null,
                MiscCode: null,
                MemberOf: null,
                FirmType: null,
                COD: 0,
                StmtType: 0,
                Detail: 0,
                LateChg: 0,
                ClientOf: null,
                SalesRep: null,
                ConStmt: 0,
                SalesTax: 0,
                Discount: 0,
                FinChg: 0,
                StaffOf: null,
                SendToAdj: 0,
                CrLimit: 0,
                Notes: "",
                Term: "",
                AccRate: 0,
                Collector: null,
                AcctStat: 0,
                CommType: 0,
                Password: null,
                CallBack: null,
                Action: null,
                PmtDate: null,
                PmtAmt: 0,
                CODSent: null,
                DemandSent: null,
                ChngDate: null,
                ChngBy: null,
                EntDate: null,
                EntBy: null,
                TStamp: null,
                DocCapForRecords: 0,
                DocCapForFilms: 0,
                DocProductionPreference: null,
                OwnOrganization: false,
                OwnFacesheet: false,
                MonthlyBilling: false,
                ClientOfFirstName: null,
                ClientOfLastName: null,
                SalesRepFirstName: null,
                SalesRepLastName: null,
                MemberOfName: null,
                LinkRequest: false,
                Contact: "",
                ContactEmail: "",
                ContactName: "",
                LinkedContacts: "",
                LastUsedDate: null,
                UsedRank: null,
                CompanyID: null,
                LEDESBillingCode: null
            };

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

    }

    function init() {
       // $scope.EnableGenerteCodeValidation = false;
        CreateObjects();
        dropdownbind();
        $scope.GetFirmById($stateParams.FirmID);
        getUserDetails();
        
    }
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
    //#endregion

    function GetMemberOfIDList() {
        var objMemberOfIDList = FirmServices.GetMemberOfIDList($scope.FirmObj.FirmID)
        objMemberOfIDList.success(function (response) {
            $scope.memberOfIDList = response.Data;
        });
    }

    $scope.MonthlyBilling = function () {
        if ($scope.FirmObj.MonthlyBilling) {
            angular.element("#modal_MonthyBillingContact").modal('show');
        }
    }

    $scope.SaveMonthlyBillingContact = function (form) {
        if (form.$valid) {            
            if ($scope.FirmObj.FirmID == null || $scope.FirmObj.FirmID == '' ) {
                toastr.error("Please Generate FirmID");
                return;
            }

            var promise = FirmServices.SaveFirmMonthlyBilling($scope.FirmObj.FirmID, $scope.FirmObj.ContactName, $scope.FirmObj.ContactEmail);
            promise.success(function (response) {
                if (response.Success) {
                    //$scope.BindAdditionalContact();
                    //$scope.frmAdditionalContacts.$setPristine();
                    //$scope.ClearAdditionalContact();
                    angular.element("#modal_MonthyBillingContact").modal('hide');
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) { });

        }

    };
    $scope.ManageMember = function () {
        angular.element("#modal_MemberManage").modal('show');
        $scope.objMember = new Object();

        var company = CommonServices.GetCompanyDropDown();
        company.success(function (response) {
            $scope.companyList = response.Data;
        });

        var member = CommonServices.MemberDropdown();
        member.success(function (response) {
            $scope.memberList = response.Data;
        });
        GetMemberOfIDList();
        $scope.formMemberOfID.$setPristine();
    }
    $scope.DeleteMemberOfID = function (BillingRateID) {

        var objDeleteMemberOfID = FirmServices.DeleteMemberOfID(BillingRateID)
        objDeleteMemberOfID.success(function (response) {
            if (response.Success) {
                toastr.success("Company Member information deleted successfully.");
                GetMemberOfIDList();
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
    }

    $scope.InsertMemberOfID = function (form) {
        if (form.$valid) {
            
            var saveMemberOfID = FirmServices.InsertMemberOfID($scope.FirmObj.FirmID, $scope.objMember.CompanyID, $scope.objMember.MemberID)
            saveMemberOfID.success(function (response) {
                if (response.Success) {
                    if (response.str_ResponseData == '-1') {
                        toastr.error("Company Member information already exist.");
                        return false;
                    }
                    else {
                        toastr.success("Company Member information saved Successfully.");
                        GetMemberOfIDList();
                        $scope.objMember = {};
                        $scope.formMemberOfID.$setPristine();
                    }
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            saveMemberOfID.error(function (data, statusCode) {
            });
        }
    }
    $scope.createDatePicker = function () {
        createDatePicker();
    };

    init();
});