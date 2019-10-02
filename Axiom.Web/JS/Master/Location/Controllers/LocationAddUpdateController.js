app.controller('LocationAddUpdateController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, LocationServices, CommonServices, configurationService, FirmServices, $compile, $filter) {

    decodeParams($stateParams);

    $scope.isEdit = false;

    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.UserEmployeeID = $rootScope.LoggedInUserDetail.EmpId;
    $rootScope.pageTitle = $stateParams.LocID ? "Edit Location" : "Add Location";

      $scope.FirmSearch = function (event) {
        $scope.ShowSearchFirm = true;
    };

    $scope.FilFirmlLocationDetail = function (FirmID) {
        
        var promise = FirmServices.GetFirmById(FirmID);
        promise.success(function (response) {
            
            $scope.LocationObj.FirmID = response.Data[0].FirmID;
            $scope.LocationObj.FirmName = response.Data[0].FirmName;
        });
        promise.error(function (data, statusCode) {
        });
    };

    //#region Events

    $scope.AddSearchLocation = function ($event) {
        var table = $('#tblSearchLocation').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        if ($scope.LocationType == "Billing") {
            $scope.LocationObj.BillingLocId = row.LocID;
            $scope.LocationObj.BillingLocName = row.Name;
        }
        else {
            $scope.LocationObj.FilmsLocId = row.LocID;
            $scope.LocationObj.FilmsLocName = row.Name;
        }
        angular.element("#modal_Location_Search").modal('hide');
    }
    $scope.ShowLocationForm = function (type) {
        
        $scope.LocationType = type;
        angular.element("#modal_Location_Search").modal('show');
        clearLocationSearch();
        $scope.bindLocationList();
        dropdownbind();
    }
    function clearLocationSearch() {
        $scope.LocationSearchObj = new Object();
        $scope.LocationSearchObj.LocID = "";
        $scope.LocationSearchObj.Name = "";
        $scope.LocationSearchObj.PhoneNo = "";
        $scope.LocationSearchObj.Department = "";
        $scope.LocationSearchObj.Address = "";
        $scope.LocationSearchObj.City = "";
        $scope.LocationSearchObj.State = "";
    }
    $scope.bindLocationList = function () {

        if ($.fn.DataTable.isDataTable("#tblSearchLocation")) {
            $('#tblSearchLocation').DataTable().destroy();
        }
        var pagesizeObj = 10;
        $('#tblSearchLocation').DataTable({
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
            "sAjaxSource": configurationService.basePath + "/GetLocationList",
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
                    "url": sSource + "?SearchValue=" + "" + "&PageIndex=" + (parseInt($('#tblSearchLocation').DataTable().page.info().page) + 1)
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
                { "title": "Location Code", "data": "LocID", "className": "dt-left", "width": "20%" },
                { "data": "Name", "title": "Name", "className": "dt-left" },
                { "data": "Department", "title": "Department", "className": "dt-left", "width": "20%" },
                {
                    "title": 'Action',
                    "sClass": "action dt-center",
                    "orderable": false,
                    "width": "8%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = '<a class="ico_btn cursor-pointer" ng-click="AddSearchLocation($event)" title="Merge Location"> <i class="icon-plus-circle2"></i></a>';
                                              
                        return strAction;
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblSearchLocation').DataTable();
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

    $scope.ClearLocationSearch_Click = function () {
        clearLocationSearch();
        $scope.bindLocationList();
    };

    $scope.LocationSearch_Click = function () {
        $scope.bindLocationList();
    };
    $scope.SaveLocation = function (form) {
        if (form.$valid) {


            $scope.LocationObj.SendRequest = GetCSVForCheckBoxs($scope.SendRequestOptions);
            $scope.LocationObj.RequestSent = GetCSVForCheckBoxs($scope.RequestSentOptions);
            $scope.LocationObj.ReccanRequested = GetCSVForCheckBoxs($scope.ReccanRequestedOptions);
            $scope.LocationObj.LinkedLocation = GetCSVForCheckBoxs($scope.LinkedLocationOptions);

            //GetCSVForCheckBoxs($scope.SendRequestOptions);


            if (!$scope.isEdit) {
                
                $scope.LocationObj.Notes = $('#Notes').val();
                var promise = LocationServices.InsertLocation($scope.LocationObj, $scope.UserEmployeeID);
                promise.success(function (response) {
                    if (response.Success) {
                        toastr.success('Location saved successfully.');
                        $state.transitionTo('Location');
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else {
                $scope.LocationObj.Notes = $('#Notes').val();
                var promiseedit = LocationServices.UpdateLocation($scope.LocationObj, $scope.UserEmployeeID);
                promiseedit.success(function (response) {
                    if (response.Success) {
                        toastr.success("Location updated successfully.");
                        $state.transitionTo('Location');
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

    $scope.GenerateNewLocationCode = function (form) {
        if (form.Name1.$valid) {
            
            var promise = CommonServices.GetNewSequenceNumber('Location', $scope.LocationObj.Name1, '');
            promise.success(function (response) {
                $scope.LocationObj.LocID = '';
                if (response.Success) {
                    $scope.LocationObj.LocID = response.Data[0];
                    $scope.EnableGenerteCodeValidation = false;
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) { });
        }
        else {
            $scope.LocationObj.LocID = '';
            $scope.EnableGenerteCodeValidation = true;
        }
    };

    $scope.RadioCheckChange = function (item, optionsList) {
        angular.forEach($filter('filter')(optionsList, { 'IsChecked': true }), function (record, key) {
            record.IsChecked = (item.Value == record.Value);
        });
    };

    $scope.OpenMergeLocation = function () {
        $scope.ObjOpenMergeLocation = { LocID: $scope.LocationObj.LocID, Name: ($scope.LocationObj.Name1 + $scope.LocationObj.Name2), Department: $scope.LocationObj.Dept };
        $scope.ShowMergeLocation = true;
    }

    $scope.OpenAddForms = function (IsRequestForm) {
        
        $scope.ObjOpenMergeLocation = { LocID: $scope.LocationObj.LocID, Name: ($scope.LocationObj.Name1 + $scope.LocationObj.Name2), Department: $scope.LocationObj.Dept, IsRequestForm: IsRequestForm };
        $scope.ShowLocationForm = true;
    }
    $scope.openLocationNoteClick = function () {
        var promise = CommonServices.GetLocationNotes($scope.LocationObj.LocID);
        promise.success(function (response) {
            if (response.Success && response.Data.length > 0) {
                $scope.locationNotes = response.Data[0].Notes;
                getNotesList($scope.locationNotes);
            }
            angular.element("#modal_NotesForm").modal('show');

        });
        promise.error(function (data, statusCode) {
        });

    }
    //#endregion 

    //#region Methods
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
    $scope.GetLocationById = function (locationId) {
      
        if (locationId != undefined && locationId != "") {
            $scope.isEdit = true;
            var promise = LocationServices.GetLocationById(locationId);
            promise.success(function (response) {
      
                $scope.LocationObj = response.Data[0];
                SetCheckBoxStatus($scope.LocationObj.SendRequest, $scope.SendRequestOptions);
                SetCheckBoxStatus($scope.LocationObj.RequestSent, $scope.RequestSentOptions);
                SetCheckBoxStatus($scope.LocationObj.ReccanRequested, $scope.ReccanRequestedOptions);
                SetCheckBoxStatus($scope.LocationObj.LinkedLocation, $scope.LinkedLocationOptions);

                //GetCSVForCheckBoxs($scope.SendRequestOptions);
            });
            promise.error(function (data, statusCode) {
            });
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
        var _departmentlist = CommonServices.LocationDepartmentDropDown();
        _departmentlist.success(function (response) {
            $scope.Departmentlist = response.Data;
        });
        _departmentlist.error(function (data, statusCode) { });



        // var Termlist = CommonServices.GetTermDropDown();
        // Termlist.success(function (response) {
        // $scope.Termlist = response.Data;
        // });
        // Termlist.error(function (response) {
        // toastr.error(response.Message[0]);
        // });
        // var Locationdropdownlist = CommonServices.GetLocationDropDown();
        // Locationdropdownlist.success(function (response) {
        // $scope.Locationdropdownlist = response.Data;
        // });
        // Locationdropdownlist.error(function (response) {
        // toastr.error(response.Message[0]);
        // });


        // var Accountlist = CommonServices.GetAccountDropDown();
        // Accountlist.success(function (response) {
        // $scope.Accountlist = response.Data;
        // });
        // Accountlist.error(function (response) {
        // toastr.error(response.Message[0]);
        // });

    }
    $scope.ObjOpenMergeLocation = new Object();
    function CreateObjects() {

        $scope.LocationObj =
            {
                'LocID': null,
                'Name1': null,
                'Name2': null,
                'Dept': null,
                'Street1': null,
                'Street2': null,
                'City': null,
                'State': null,
                'Zip': null,
                'AreaCode1': null,
                'PhoneNo1': null,
                'AreaCode2': null,
                'PhoneNo2': null,
                'AreaCode3': null,
                'FaxNo': null,
                'Email': null,
                'Contact': null,
                'Notes': null,
                'Warning': null,
                'FirstName': null,
                'LastName': null,
                'Title': null,
                'Specialty': null,
                'ServBy': null,
                'HandServer': null,
                'CommType': null,
                'ChngDate': null,
                'ChngBy': null,
                'EntDate': null,
                'EntBy': null,
                'TStamp': null,
                'FirmID': null,
                'FirmName': null,
                'SendRequest': null,
                'ReccanRequested': null,
                'CopyService': null,
                'FeeAmountSendRequest': null,
                'ReqAuthorization': null,
                'AuthorizationDays': null,
                'LinkedLocation': null,
                'FeeAmount': null,
                'LocSpecificAuthorization': null,
                'BillingLocId': null,
                'FilmsLocId': null,
                'BillingLocName': null,
                'FilmsLocName': null,
                'ReplacedBy': null,
                'LinkRequest': null,
                'LastUsedDate': null,
                'UsedRank': null,
                'EnteredByClient': null,
                'IsDisable': null,
                'Comment': null,
                'isCommonLocation': null,
                'RequestSent': null
            };

        $scope.SendRequestOptions =
            [
                { Value: '0', Titile: 'Fax', IsChecked: false }
                , { Value: '1', Titile: 'Mail', IsChecked: false }
                , { Value: '2', Titile: 'Email', IsChecked: false }
                , { Value: '3', Titile: 'Upload', IsChecked: false }
                , { Value: '4', Titile: 'Process Server', IsChecked: false }
                , { Value: '5', Titile: 'Certified Mail', IsChecked: false }
            ];

        $scope.RequestSentOptions =
            [
                { Value: '0', Titile: 'None', IsChecked: false }
                , { Value: '4', Titile: 'Process Server', IsChecked: false }
                , { Value: '5', Titile: 'Certified Mail', IsChecked: false }
            ];
        $scope.ReccanRequestedOptions =
            [
                { Value: '0', Titile: 'Medical', IsChecked: false }
                , { Value: '1', Titile: 'Billing', IsChecked: false }
                , { Value: '2', Titile: 'Films', IsChecked: false }
                , { Value: '3', Titile: 'Pathology', IsChecked: false }
                , { Value: '4', Titile: 'Employment', IsChecked: false }
                , { Value: '5', Titile: 'Academic', IsChecked: false }
            ];

        $scope.LinkedLocationOptions =
            [
                { Value: '0', Titile: 'Billing', IsChecked: false }
                , { Value: '1', Titile: 'Medical', IsChecked: false }
                , { Value: '2', Titile: 'Films', IsChecked: false }

            ];
    }

    function init() {        
        $scope.LocationObj = new Object();
        CreateObjects();
        dropdownbind();
        $scope.GetLocationById($stateParams.LocID);
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

    init();
});