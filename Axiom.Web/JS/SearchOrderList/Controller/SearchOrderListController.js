app.controller('SearchOrderListController', function ($scope, $state, $stateParams, $rootScope, EmployeeServices, notificationFactory, SearchOrderListService, OrderPartService, DepartmentService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.IsOrderPartSelected = false;
    //#region Event
    $scope.OrderSearchObj = new Object();

    $scope.OnCheckAll_click = function ($event) {
        angular.element('input.chkchildItem:checkbox').not($event.target).prop('checked', $event.target.checked);
        HighlightSelectedRow();
    };
    $scope.OnCheck_click = function ($event) {
        angular.element('#chkAll').prop('checked', angular.element('input.chkchildItem:checked').length == angular.element('input.chkchildItem:checkbox').length);
        HighlightSelectedRow();
    };

    function HighlightSelectedRow() {
        $('input.chkchildItem').each(function () {
            if (this.checked == true) {
                $(this).parent().parent().attr('style', 'background-color:#f9f1db !important');
            }
            else {
                $('#chkAll').prop("checked", false);
                $(this).parent().parent().attr('style', '');
            }
        });
    }

    $scope.ViewOrderDetail = function ($event) {
        var table = $('#tblOrder').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        var url = $state.href('OrderDetail', { 'OrderId': row.OrderNo });
        window.open(url, '_blank');
    };
    $scope.ViewPartDetail = function ($event) {
        var table = $('#tblOrder').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        //$state.transitionTo('PartDetail', ({ 'OrderId': row.OrderNo, 'PartNo': row.PartNo }));
        var url = $state.href('PartDetail', { 'OrderId': row.OrderNo, 'PartNo': row.PartNo });
        window.open(url, '_blank');

    };
    $scope.ViewLocation = function ($event) {
        var table = $('#tblOrder').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        //$state.transitionTo('ManageLocation', ({ 'LocID': row.LocId }));
        var url = $state.href('ManageLocation', { 'LocID': row.LocId });
        window.open(url, '_blank');

    };
    $scope.EmailToPlaintiffAtty = function ($event) {
        var table = $('#tblOrder').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        //$state.transitionTo('ManageLocation', ({ 'LocID': row.LocId }));
        var url = $state.href('ManageLocation', { 'LocID': row.LocId });
        window.open(url, '_blank');
        //<a href="mailto:ttakala@morganmeyers.com?subject=Kristel Oaks a.k.a. Kristel Bien a.k.a. Kristel Connelly (32270-15)">Timothy M. Takala</a>
    };

    //#endregion

    //#region Method
    function getEmployeeList() {
        var emp = EmployeeServices.GetEmployeeList($rootScope.CompanyNo);
        emp.success(function (response) {
            $scope.EmployeeList = angular.copy(response.Data);
        });
        emp.error(function (data, statusCode) {
        });
    }

    function bindDropDown() {
        var Attorney = CommonServices.AttorneyForDropdown('', $rootScope.CompanyNo);
        Attorney.success(function (response) {
            $scope.Attorneylist = response.Data;
        });
        var recordType = CommonServices.RecordTypeDropDown();
        recordType.success(function (response) {
            $scope.RecordTypeList = response.Data;
        });

        var stateType = CommonServices.StateDropdown();
        stateType.success(function (response) {
            $scope.StateList = response.Data;
        });

        var company = CommonServices.GetCompanyDropDown();
        company.success(function (response) {
            $scope.Companylist = response.Data;
        });

        var accountrep = CommonServices.GetAccountRepDropdown();
        accountrep.success(function (response) {
            $scope.AccountReplist = response.Data;
        });

        var internalStatus = CommonServices.GetInternalStatusesDropdown();
        internalStatus.success(function (response) {
            $scope.InternalStatuslist = response.Data;
        });
        getEmployeeList();
    }


    $scope.bindOrderList = function () {

        if ($.fn.DataTable.isDataTable("#tblOrder")) {
            $('#tblOrder').DataTable().destroy();
        }
        $('.table-responsive').hide();
        var pagesizeObj = 10;
        $('#tblOrder').DataTable({
            stateSave: false,
            "oLanguage": {
                "sProcessing": '<div class="loader" data-loading><img src="/assets/images/loader2.gif" /></div>',
                "sZeroRecords": "<span class='pull-left'>No records found</span>",
            },
            columnDefs: [{
                orderable: false,
                className: 'select-checkbox',
                targets: 0
            }],
            select: {
                style: 'os',
                selector: 'td:first-child'
            },
            "searching": false,
            "dom": '<"table-responsive"frt><"bottom"lip<"clear">>',
            "bProcessing": true,
            "bServerSide": true,
            "iDisplayStart": 0,
            "iDisplayLength": pagesizeObj,
            "lengthMenu": [[10, 20, 30, 40, 50, 100, -1], [10, 20, 30, 40, 50, 100, 'All']],
            "sAjaxDataProp": "aaData",
            "aaSorting": [[1, 'desc']],
            "sAjaxSource": configurationService.basePath + "GetOrderListForAdmin",
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                aoData = BindSearchCriteriaCommon(aoData);
                aoData = BindSortingCommon(aoData, oSettings);
                if ($scope.SearchValue == undefined) {
                    $scope.SearchValue = "";
                }
                //aoData.SearchValue = $scope.SearchValue; 
                //aoData.PageIndex = parseInt($('#tblOrder').DataTable().page.info().page) + 1;
                $scope.GridParams = aoData;
                var url = sSource + "?PageIndex=" + (parseInt($('#tblOrder').DataTable().page.info().page) + 1)
                    + "&SearchValue=" + ""
                    + "&OrderNo=" + $scope.OrderSearchObj.OrderNo
                    + "&PartNo=" + $scope.OrderSearchObj.PartNo
                    + "&PatientName=" + $scope.OrderSearchObj.RecordsofName // $scope.OrderSearchObj.RecordsofName
                    + "&LocationName=" + $scope.OrderSearchObj.Location
                    + "&LocationState=" + ($scope.OrderSearchObj.LocationState == null ? "" : $scope.OrderSearchObj.LocationState)
                    + "&PlaintiffAtty1=" + ($scope.OrderSearchObj.PatientAttorney == null ? "" : $scope.OrderSearchObj.PatientAttorney)
                    + "&PlaintiffAtty2=" + ""
                    + "&PlaintiffAtty3=" + ""
                    + "&PlaintiffAtty4=" + ""
                    + "&PlaintiffAtty5=" + ""
                    + "&OrderAtty=" + ($scope.OrderSearchObj.OrderingAttorney == null ? "" : $scope.OrderSearchObj.OrderingAttorney)
                    + "&OrderFirm=" + ($scope.OrderSearchObj.OrderingFirm == null ? "" : $scope.OrderSearchObj.OrderingFirm)
                    + "&RecordTypeId=" + ($scope.OrderSearchObj.RecordTypeId == null ? "" : $scope.OrderSearchObj.RecordTypeId)
                    + "&AccountRep=" + ($scope.OrderSearchObj.AccountRep == null ? "" : $scope.OrderSearchObj.AccountRep)
                    + "&Processor=" + ($scope.OrderSearchObj.Processor == null ? "" : $scope.OrderSearchObj.Processor)
                    + "&AssignedTo=" + ($scope.OrderSearchObj.AssignedTo == null ? "" : $scope.OrderSearchObj.AssignedTo)
                    + "&InternalStatus=" + ($scope.OrderSearchObj.InternalStatus == null ? "" : $scope.OrderSearchObj.InternalStatus)
                    + "&Rush=" + $("input:radio[name='Rush']:checked").val()
                    + "&CallbackOnly=" + $scope.OrderSearchObj.IsOnlyCallbacks
                    + "&DueDateFrom=" + $scope.OrderSearchObj.FromDueDate
                    + "&DueDateTo=" + $scope.OrderSearchObj.ToDueDate
                    + "&ActionDateFrom=" + $scope.OrderSearchObj.FromActionDate
                    + "&ActionDateTo=" + $scope.OrderSearchObj.ToActionDate
                    + "&CallbackDateFrom=" + $scope.OrderSearchObj.FromCallbackDate
                    + "&CallbackDateTo=" + $scope.OrderSearchObj.ToCallbackDate
                    + "&CompNo=" + ($scope.OrderSearchObj.CompanyID == null ? "" : $scope.OrderSearchObj.CompanyID)
                    + "&AreaCode=" + $scope.OrderSearchObj.AreaCode
                    + "&FaxNo=" + $scope.OrderSearchObj.FaxNo
                    + "&ShowPartDetailWithOrder=" + ($scope.OrderSearchObj.ShowPartDetailWithOrder == null ? "" : $scope.OrderSearchObj.ShowPartDetailWithOrder)
                    + "&CompanyNo=" + $rootScope.CompanyNo

                oSettings.jqXHR = $.ajax({
                    'dataSrc': 'aaData',
                    "dataType": 'json',
                    "type": "POST",
                    "url": url,
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });
            },
            "columns": [
                {
                    "title": '<input type="checkbox" class="cursor-pointer" id="chkAll"  />', //ng-click="OnCheckAll_click($event)"
                    "data": "OrderNo",
                    "className": "action dt-center dt-select-all",
                    orderable: false,
                    visible: $scope.OrderSearchObj.ShowPartDetailWithOrder,
                    "render": function (data, type, row) {
                        var strAction = '<input type="checkbox"  class="cursor - pointer chkchildItem" ng-click="OnCheck_click($event)" OrderNo="' + row.OrderNo + '"  PartNo="' + row.PartNo + '"/>';

                        return strAction;
                    }
                },
                {
                    "title": 'Order',
                    "data": "OrderNo",
                    "className": "dt-left",
                    orderable: true,
                    "render": function (data, type, row) {
                        var strHtml = '<a ng-click="ViewOrderDetail($event)" title="Click here to order detail">' + row.OrderNo + '</a>';
                        return strHtml;
                    }
                },
                {
                    "title": 'Part',
                    "data": "PartNo",
                    "className": "dt-left",
                    orderable: true,
                    visible: $scope.OrderSearchObj.ShowPartDetailWithOrder,
                    "render": function (data, type, row) {
                        var strHtml = '<a ng-click="ViewPartDetail($event)" title="Click here to open part detail">' + row.PartNo + '<i class="icon-square-up-right" style="padding-left:5px;padding-top:2px;"></i></a>';
                        return strHtml;
                    }
                },
                {
                    "title": 'Location',
                    "data": "Name1",
                    "className": "dt-left",
                    orderable: true,
                    visible: $scope.OrderSearchObj.ShowPartDetailWithOrder,
                    "render": function (data, type, row) {
                        var strHtml = '<a ng-click="ViewLocation($event)" title="Click here to edit location">' + row.Name1 + '</a>';
                        return strHtml;
                    }
                },
                {
                    "title": "Patient's Attorney",
                    "data": "PlaintiffAtty",
                    "className": "dt-left",
                    orderable: true,
                    visible: $scope.OrderSearchObj.ShowPartDetailWithOrder,
                    "render": function (data, type, row) {
                        if (row.PlaintiffEmail != null && row.PlaintiffEmail != "") {
                            var strHtml = '<a href="mailto:' + row.PlaintiffEmail + '?subject=' + row.Patient + ' (' + row.OrderNo + '-' + row.PartNo + ')">' + row.PlaintiffAtty + '</a>';
                            return strHtml;
                        }
                        else {
                            return row.PlaintiffAtty;
                        }
                    }
                },
                {
                    "title": "Records of",
                    "className": "dt-left",
                    "data": "Patient",
                },
                {
                    "title": "Assigned To",
                    "data": "AssignedTo",
                    "className": "dt-left",
                    orderable: true,
                    visible: $scope.OrderSearchObj.ShowPartDetailWithOrder,
                    "render": function (data, type, row) {
                        if (row.AssignedToEmail != null && row.AssignedToEmail != "") {
                            var strHtml = '<a href="mailto:' + row.AssignedToEmail + '?subject=' + row.OrderNo + '-' + row.PartNo + '">' + row.AssignedTo + '</a>';
                            return strHtml;
                        }
                        else {
                            return row.AssignedTo;
                        }
                    }
                },
                {
                    "title": "Internal Status",
                    "className": "dt-left",
                    "data": 'InternalStatus',
                    visible: $scope.OrderSearchObj.ShowPartDetailWithOrder,
                    "render": function (data, type, row) {
                        var strStatus = '<div class="orderstatus"><div class="' + row.StatusClass + '"></div><span class="' + row.StatusClass + '">' + row.InternalStatus + '</span></div>';
                        return strStatus;
                    }
                },
                {
                    "title": "Status",
                    "className": "dt-left",
                    visible: $scope.OrderSearchObj.ShowPartDetailWithOrder,
                    "data": 'Status'
                },
                {
                    "title": "Action",
                    "className": "dt-left",
                    visible: $scope.OrderSearchObj.ShowPartDetailWithOrder,
                    "data": "ActionDate", //AssignedToEmail
                }


            ],
            "initComplete": function () {
                var dataTable = $('#tblOrder').DataTable();
                BindCustomerSearchBar($scope, $compile, dataTable);
                // $compile($('#tblOrder'))($scope);
            },
            "fnDrawCallback": function () {
                //BindToolTip();
                angular.element('#chkAll').prop('checked', false);
                //setTimeout(function () { SetAnchorLinks(); }, 500);
                $('.table-responsive').show();
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    $scope.DefaultInitialization = function () {
        $('#tblOrder').on('click', 'th', function () {
            if ($(this).index() == 0 && $(this).text() == "") {
                angular.element('input.chkchildItem:checkbox').prop('checked', $(".dt-select-all #chkAll").is(":checked"));
                if ($(".dt-select-all #chkAll").is(":checked")) {
                    angular.element('input.chkchildItem:checkbox').parent().parent().attr('style', 'background-color:#f9f1db !important');
                }
                else {
                    angular.element('input.chkchildItem:checkbox').parent().parent().attr('style', '');
                }

                $scope.GetSelectedOrders();
            }
        });
        $('#tblOrder').on('click', 'input.chkchildItem', function () {
            $scope.GetSelectedOrders();
        });
    };

    $scope.GetSelectedOrders = function () {
        $scope.SelectedOrdersWithPart = [];
        $("input.chkchildItem").each(function () {
            if ($(this).is(":checked")) {
                var obj = new Object();
                obj.OrderId = $(this).attr('OrderNo');
                obj.PartNo = $(this).attr('PartNo');
                $scope.SelectedOrdersWithPart.push(obj);
            }
        });
        if (!isNullOrUndefinedOrEmpty($scope.SelectedOrdersWithPart) && $scope.SelectedOrdersWithPart.length > 0) {
            $("#btnAddNotesToPart").show();
            $("#btnGenerateInovice").show();

        } else {
            $("#btnAddNotesToPart").hide();
            $scope.IsNoteSectionEnable = 0;
            $("#btnGenerateInovice").hide();
        }
    };

    $scope.AddPartNotes = function () {
        $scope.IsNoteSectionEnable = 1;
        $rootScope.$broadcast('resetPartNotesPopupDetails', null);
        angular.element('#modal_OrderPartNote').modal('show');
    };
    $scope.GenerateInvoice = function () {
        console.log($scope.SelectedOrdersWithPart);
        var generateInvoiceMultiple = SearchOrderListService.GenerateInvoiceMultiple($scope.SelectedOrdersWithPart, $rootScope.CompanyNo);
        generateInvoiceMultiple.success(function (response) {

            toastr.success('Invoice generate process executed successfully.');
        });
        generateInvoiceMultiple.error(function (data, statusCode) {

        });

    };

    function clearSearch() {

        $scope.OrderSearchObj = new Object();
        $scope.OrderSearchObj.ShowPartDetailWithOrder = true;
        $scope.OrderSearchObj.OrderNo = "";
        $scope.OrderSearchObj.PartNo = "";
        $scope.OrderSearchObj.RecordsofName = "";
        $scope.OrderSearchObj.Location = "";
        $scope.OrderSearchObj.LocationState = "";
        $scope.OrderSearchObj.AreaCode = "";
        $scope.OrderSearchObj.FaxNo = "";
        $scope.OrderSearchObj.PatientAttorney = "";
        $scope.OrderSearchObj.OrderingAttorney = "";
        $scope.OrderSearchObj.OrderingFirm = "";
        $scope.OrderSearchObj.RecordTypeId = "";
        $scope.OrderSearchObj.AccountRep = "";
        $scope.OrderSearchObj.Processor = "";
        $scope.OrderSearchObj.AssignedTo = "";// $rootScope.LoggedInUserDetail.EmpId;
        $scope.OrderSearchObj.CompanyID = "";
        $scope.OrderSearchObj.InternalStatus = "";
        $scope.OrderSearchObj.Rush = "";
        $('input[name=Rush][value=""]').prop('checked', true);
        $scope.OrderSearchObj.IsOnlyCallbacks = false;
        $scope.OrderSearchObj.FromDueDate = null;
        $scope.OrderSearchObj.ToDueDate = null;
        $scope.OrderSearchObj.FromActionDate = null;
        $scope.OrderSearchObj.ToActionDate = null;
        $scope.OrderSearchObj.FromCallbackDate = "";//$filter('date')(new Date(), 'MM/dd/yyyy');
        $scope.OrderSearchObj.ToCallbackDate = "";// $filter('date')(new Date(), 'MM/dd/yyyy');
        //$("input:radio[name='Rush']").val(0);

    };
    $scope.createDatePicker = function () {
        createDatePicker();
    }
    $scope.clearSearchClick = function () {
        clearSearch(); //$scope.bindOrderList();
    };

    $scope.BindSearchParameterFromDailyAnnouncement = function () {
        $scope.OrderSearchObj.ShowPartDetailWithOrder = true;
        $scope.OrderSearchObj.OrderNo = "";
        $scope.OrderSearchObj.PartNo = "";
        $scope.OrderSearchObj.RecordsofName = "";
        $scope.OrderSearchObj.Location = "";
        $scope.OrderSearchObj.LocationState = "";
        $scope.OrderSearchObj.AreaCode = "";
        $scope.OrderSearchObj.FaxNo = "";
        $scope.OrderSearchObj.PatientAttorney = "";
        $scope.OrderSearchObj.OrderingAttorney = "";
        $scope.OrderSearchObj.OrderingFirm = "";
        $scope.OrderSearchObj.RecordTypeId = "";
        $scope.OrderSearchObj.AccountRep = "";
        $scope.OrderSearchObj.Processor = "";
        if (!isNullOrUndefinedOrEmpty($stateParams.EmpId)) {
            $scope.OrderSearchObj.AssignedTo = $stateParams.EmpId;
        } else {
            $scope.OrderSearchObj.AssignedTo = "";
        }

        $scope.OrderSearchObj.CompanyID = "";
        $scope.OrderSearchObj.InternalStatus = "";
        if (!isNullOrUndefinedOrEmpty($stateParams.IsRush)) {
            $scope.OrderSearchObj.Rush = $stateParams.IsRush;
            $('input[name=Rush][value="' + $stateParams.IsRush + '"]').prop('checked', true);
        } else {
            $scope.OrderSearchObj.Rush = "";
            $('input[name=Rush][value=""]').prop('checked', true);
        }

        $scope.OrderSearchObj.IsOnlyCallbacks = true;
        $scope.OrderSearchObj.FromDueDate = null;
        $scope.OrderSearchObj.ToDueDate = null;
        if (!isNullOrUndefinedOrEmpty($stateParams.FromDate)) {
            $scope.OrderSearchObj.FromActionDate = $stateParams.FromDate;
        } else {
            $scope.OrderSearchObj.FromActionDate = null;
        }
        if (!isNullOrUndefinedOrEmpty($stateParams.EndDate)) {
            $scope.OrderSearchObj.ToActionDate = $stateParams.EndDate;
        } else {
            $scope.OrderSearchObj.ToActionDate = null;
        }


        $scope.OrderSearchObj.FromCallbackDate = "";
        $scope.OrderSearchObj.ToCallbackDate = "";
    };

    function init() {
        $scope.DefaultInitialization();
        if ($stateParams.IsFromDailyAnnouncement == 1) {
            $scope.BindSearchParameterFromDailyAnnouncement();
            $state.go("SearchOrderList", { IsFromDailyAnnouncement: 0 }, { notify: false, reload: false, location: 'replace', inherit: false });
        } else {
            clearSearch();

        }
        $scope.bindOrderList();

        bindDropDown();


    }
    //#endregion

    init();
});