app.controller('BillingRateController', function ($rootScope, $scope, $stateParams, notificationFactory, billingRateService, configurationService, CommonServices, $compile, $filter) {
    $scope.objBillingRate = new Object();

    decodeParams($stateParams);
    $scope.ShowPageNumber = false;

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Billing Rate", "View");
    //------------

    function bindBillingList() {

        if ($.fn.DataTable.isDataTable("#tblCourt")) {
            $('#tblCourt').DataTable().destroy();
        }

        var table = $('#tblCourt').DataTable({
            data: $scope.CourtList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Name",
                    "className": "dt-left",
                    "data": "CourtName",
                    "sorting": "true"
                },
                {
                    "title": "Type",
                    "className": "dt-left",
                    "data": "CourtType",
                    "sorting": "true"
                },
                {
                    "title": "State",
                    "className": "dt-left",
                    "data": "StateName",
                    "sorting": "true"
                },
                {
                    "title": "District",
                    "className": "dt-left",
                    "data": "DistrictName",
                    "sorting": "true"
                },
                {
                    "title": "County",
                    "className": "dt-left",
                    "data": "CountyName",
                    "sorting": "true"
                },
                {
                    "title": "Active",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsActive",
                    "width": "10%",
                    "render": function (data, type, row) {
                        return (row.IsActive) ? "<label class='label bg-success-400'>Active</label>" : "<label class='label bg-danger-400'>InActive</label>";
                    }
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='EditCourt($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteCourt($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblService').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    };

    $scope.isEdit = false;

    $scope.OnMemberChanged = function () {
        GetBillingListByID();
    };

    $scope.EditBillingRate = function (recordtypeid) {
        $scope.GetBillingRateDetail($scope.objBillingRate.MemberID, recordtypeid);
    };

    $scope.FilterRecordTypeList = function (item) {
        return ($scope.isEdit || !item.isExist);
    };

    // ADD OR EDIT RECORD TYPE IN POPUP
    $scope.GetRecordTypeDetail = function () {
        $scope.RecordTypeForm.$setPristine();
        angular.element("#modal_recordtype").modal('show');
        $scope.objRecordTypePopup = new Object();
        $scope.objRecordTypePopup.isOnRatePage = true;
        $scope.objRecordTypePopup.isDisable = false;

        var RecordTypeListForGrid = billingRateService.GetRecordTypeList();
        RecordTypeListForGrid.success(function (response) {
            $scope.RecordList = response.Data;
        });

        RecordTypeListForGrid.error(function (data, statusCode) { });

        //var recordType = CommonServices.RateRecordTypeDropdown(memberid);
        //recordType.success(function (response) {
        //    $scope.recordTypeList = response.Data;
        //    $scope.objBillingRatePopup.Code = response.Data[0].Code;
        //});

    };

    $scope.SaveRecordType = function () {

        var filterData = $filter('filter')($scope.RecordList, { 'Descr': $scope.objRecordTypePopup.Descr }, true);
        if (filterData && filterData.length > 0) {
            notificationFactory.customError("Record Type already exists");
        }
        else {
            if ($scope.RecordTypeForm.$valid) {
                billingRateService.InsertRecordType($scope.objRecordTypePopup);
                notificationFactory.successAdd("Record Type");
                angular.element("#modal_recordtype").modal('hide');
            }
        }
        //angular.forEach($filter('filter')(RecoredList), function (item, key) {
        //    debugger
        //    if (item.Descr == $scope.objRecordTypePopup.Descr) {
        //        notificationFactory.customError("Recored Type Allready Exists");
        //        return;
        //    }
        //});

    };
    $scope.RecordTypeOption =
        [
            { Value: '0', Titile: 'None', IsChecked: true }
            , { Value: '1', Titile: 'Digital', IsChecked: false }
            , { Value: '2', Titile: 'Hard Copy', IsChecked: false }
        ];

    function SetCheckBoxStatus(csvValue, optionsList) {
        $scope.ShowPageNumber = false;
        if (csvValue == null || csvValue == '') {
            csvValue = 'None';
        }
        if (csvValue != null && csvValue != '') {
            angular.forEach($filter('filter')(optionsList), function (item, key) {
                item.IsChecked = (csvValue.indexOf(item.Titile) > -1);
                if (csvValue == 'Digital' || csvValue == 'Hard Copy') {
                    $scope.ShowPageNumber = true;
                }
                else {
                    $scope.ShowPageNumber = false;
                }

            });
        }
    };


    // ADD OR EDIT BILLING RATE IN POPUP
    $scope.GetBillingRateDetail = function (memberid, recordtypeid) {

        $scope.isEdit = false;

        $scope.objBillingRatePopup = new Object();
        var recordType = CommonServices.RateRecordTypeDropdown(memberid);
        recordType.success(function (response) {

            //$scope.recordTypeList = response.Data;
            $scope.FilterBillingRate = [];
            if ($scope.modal_Title === "Add Billing Rate") {
                angular.forEach(response.Data, function (item, key) {
                    var FilterItem = $filter('filter')($scope.BillingList, { 'RecordTypeID': item.Code }, true);
                    if (FilterItem.length == 0) {
                        $scope.FilterBillingRate.push({ 'Code': item.Code, 'CodeGroup': item.CodeGroup, 'Descr': item.Descr, 'GroupDescr': item.GroupDescr, 'isDisable': item.isDisable, 'isExist': item.isExist, 'isOnRatePage': item.isOnRatePage });
                    }
                });
            }
            else {
                $scope.FilterBillingRate = angular.copy(response.Data);
            }


            //$scope.recordTypeList = $scope.FilterBillingRate;
        });

        recordType.error(function (data, statusCode) { });

        $scope.BillingRateForm.$setPristine();
        if (recordtypeid != 0) {
            $scope.modal_Title = "Edit Billing Rate";
            $scope.isEdit = true;
            var promise = billingRateService.GetBillingRateListByID(memberid, recordtypeid);
            promise.success(function (response) {

                $scope.RateEditList = response.Data;
                $scope.objBillingRatePopup.Code = response.Data[0]["RecordTypeID"];
                $scope.objBillingRatePopup.MemberID = $scope.objBillingRate.MemberID;

                $scope.objBillingRatePopup.StartPage = response.Data[0]["StartPage"];
                $scope.objBillingRatePopup.EndPage = response.Data[0]["EndPage"];
                SetCheckBoxStatus(response.Data[0]["BillType"], $scope.RecordTypeOption);
            });
            promise.error(function (data, statusCode) {
            });
            angular.element("#modal_billing").modal('show');
        }
        else {
            $scope.modal_Title = "Add Billing Rate";
            // $scope.recordTypeList = $scope.BillingList
            $scope.objBillingRatePopup.MemberID = $scope.objBillingRate.MemberID;


            $scope.RateEditList =
                [
                    { "ID": 0, "RateMainID": 0, "RateName": "BasicFee", "DcntRate": 0, "RegRate": 0, "RecordTypeID": 0, "RecordTypeName": "", "MemberID": $scope.objBillingRatePopup.MemberID },
                    { "ID": 0, "RateMainID": 0, "RateName": "Subpoena", "DcntRate": 0, "RegRate": 0, "RecordTypeID": 0, "RecordTypeName": "", "MemberID": $scope.objBillingRatePopup.MemberID },
                    { "ID": 0, "RateMainID": 0, "RateName": "Trips", "DcntRate": 0, "RegRate": 0, "RecordTypeID": 0, "RecordTypeName": "", "MemberID": $scope.objBillingRatePopup.MemberID },
                    { "ID": 0, "RateMainID": 0, "RateName": "Copy", "DcntRate": 0, "RegRate": 0, "RecordTypeID": 0, "RecordTypeName": "", "MemberID": $scope.objBillingRatePopup.MemberID },
                    { "ID": 0, "RateMainID": 0, "RateName": "Shipping", "DcntRate": 0, "RegRate": 0, "RecordTypeID": 0, "RecordTypeName": "", "MemberID": $scope.objBillingRatePopup.MemberID }
                ];
            angular.element("#modal_billing").modal('show');
        }
    };

    $scope.RadioCheckChangeType = function (item, optionsList) {

        angular.forEach($filter('filter')(optionsList, { 'IsChecked': true }), function (record, key) {
            record.IsChecked = (item.Value == record.Value);
            if (record.IsChecked) {
                return record.Value;
            }
        });
    };

    $scope.RadioCheckChange = function (item, RecordTypeOption, $event) {

        var ischecked = $($event.target).is(":checked");
        angular.forEach(RecordTypeOption, function (record, key) {
            record.IsChecked = (item.Value == record.Value ? ischecked : false);

            if (item.Titile != "None") {
                $scope.ShowPageNumber = (record.Value > 0);
                $scope.selectedType = 'Other';
            }
            else {
                $scope.ShowPageNumber = false;
                $scope.selectedType = 'None';
            }
        });
    };
    function validateType() {
        var status = true;
        $scope.isStartValidate = $scope.isEndValidate = false;
        if ($scope.selectedType === 'Other' && !$scope.objBillingRatePopup.StartPage) {
            $scope.isStartValidate = true;
            status = false;
        }
        if ($scope.selectedType === 'Other' && !$scope.objBillingRatePopup.EndPage) {
            $scope.isEndValidate = true;
            status = false;
        }
        return status;
    };
    // SAVE BUTTON CLICK
    $scope.AddOrUpdateBillingRate = function (form) {
        if ($scope.BillingRateForm.$valid && validateType()) {
            if (!$scope.isEdit) {
                // debugger;
                var type = $filter('filter')($scope.RecordTypeOption, { 'IsChecked': true });
                if (type != null && type.length > 0) {
                    type = type[0].Titile;
                }

                angular.forEach($scope.RateEditList, function (value, index) {

                    value.RecordTypeID = $scope.objBillingRatePopup.Code;

                    value.BillType = type;

                    value.StartPage = $scope.objBillingRatePopup.StartPage;
                    value.EndPage = $scope.objBillingRatePopup.EndPage;
                });
                // return false;

                var promise = billingRateService.InsertBillingRate($scope.RateEditList);

                promise.success(function (response) {
                    if (response.Success) {
                        notificationFactory.successAdd("Billing Rate");
                        angular.element("#modal_billing").modal('hide');
                        GetBillingListByID();
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else {

                var promiseedit = billingRateService.UpdateBillingRate($scope.RateEditList);
                promiseedit.success(function (response) {
                    if (response.Success) {

                        notificationFactory.successAdd("Billing Rate");
                        angular.element("#modal_billing").modal('hide');
                        GetBillingListByID();
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

    $scope.DeleteCourt = function ($event) {

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

                if (result == true) {
                    var table = $('#tblCourt').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = CourtService.DeleteCourt(row.CourtID);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.successDelete('');
                            GetCourtList();
                            bootbox.hideAll()
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error();
                }
                else {

                }
            }
        });
    };

    function GetBillingListByID() {
        var promise = billingRateService.GetBillingRateList($scope.objBillingRate.MemberID);

        promise.success(function (response) {
            $scope.BillingList = response.Data;
            bindBillingList();
        });
        promise.error(function (data, statusCode) {
        });
    };

    function GetBillingDetail() {

        var member = CommonServices.MemberDropdown();
        member.success(function (response) {
            $scope.objBillingRate.ID = response.Data[0].ID;
            $scope.objBillingRate.MemberID = response.Data[0].MemberID;
            $scope.MemberList = response.Data;
            GetBillingListByID();
        });
        member.error(function (data, statusCode) { });
    };

    function init() {
        GetBillingDetail();
    };

    init();



});