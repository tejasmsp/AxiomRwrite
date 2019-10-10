app.controller('OrderListController', function ($scope, $state, $stateParams, $rootScope, notificationFactory, OrderListService, OrderPartService, DepartmentService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);

    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.UserId = $rootScope.LoggedInUserDetail.UserId;
    //Page Rights//    
    $rootScope.CheckIsPageAccessible("Order", "Orders", "View");
    $scope.IsUserCanEditOrder = $rootScope.isSubModuleAccessibleToUser('Order', 'Orders', 'Edit Order');
    $scope.IsUserCanViewPart = $rootScope.isSubModuleAccessibleToUser('Order', 'Orders', 'View Parts');
    $scope.IsUserCanAddDocument = $rootScope.isSubModuleAccessibleToUser('Order', 'Orders', 'Add Documents');
    $scope.IsUserCanAddLocation = $rootScope.isSubModuleAccessibleToUser('Order', 'Orders', 'Add Location');
    $scope.IsUserCanArchiveOrder = $rootScope.isSubModuleAccessibleToUser('Order', 'Orders', 'Archive/Restore Order');
    $scope.RoleName = $rootScope.LoggedInUserDetail.RoleName ? $rootScope.LoggedInUserDetail.RoleName[0] : '';
    //-----

    //#region Event

    $scope.EditOrder = function ($event) {
        var table = $('#tblOrder').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        if (row.SubmitStatus) {
            var OrderDetailUrl = $state.href('OrderDetail', { 'OrderId': row.OrderId });
            window.open(OrderDetailUrl, '_blank');
            //$state.transitionTo('OrderDetail', ({ 'OrderId': row.OrderId }));
        }
        else {
            var EditOrderUrl = $state.href('EditOrder', { 'OrderId': row.OrderId, 'Step': row.CurrentStepID });
            window.open(EditOrderUrl, '_blank');
            // $state.transitionTo('EditOrder', ({ 'OrderId': row.OrderId, 'Step': row.CurrentStepID }));
        }



        // $scope.GetDepartmentDetail(row.DepartmentId);
    };

    $scope.ShowPartDetail = function ($event) {
        var table = $('#tblOrder').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.OrderId = row.OrderId;
        $scope.PartNo = 0;
        angular.element("#modal_Part").modal('show');
        $rootScope.$broadcast('PartDetailsOpenedFromClientOrderList', null);

    };

    $scope.AddOrderDocuments = function ($event) {
        var table = $('#tblOrder').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.OpenUploadDocumentPopUp(row.OrderId, 0);
    };

    $scope.OpenUploadDocumentPopUp = function (OrderId, PartNo) {
        $scope.OrderId = OrderId;
        $scope.PartNo = PartNo;
        $scope.IsFromClientOrderList = true;
        $rootScope.$broadcast('OpenedFromClientOrderList', null);
    };

    $scope.ArchiveOrder = function ($event) {
        var table = $('#tblOrder').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        bootbox.confirm({
            message: "Are you sure you want to " + (row.IsArchive ? "restore" : "archive") + " this order?",
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
                    var promise = OrderListService.ArchiveOrder({ EmpId: $rootScope.LoggedInUserDetail.EmpId, UserAccessId: $rootScope.LoggedInUserDetail.UserAccessId, OrderId: row.OrderId });
                    promise.success(function (response) {
                        if (response.Success) {
                            $scope.bindOrderList();
                            toastr.success("The order has been " + (row.IsArchive ? "restored" : "archived") + " successfully.");
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function (data, statusCode) {
                    });
                }
                bootbox.hideAll();
            }
        });

    };
    $scope.AddLocation = function ($event) {


        var table = $('#tblOrder').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $rootScope.$broadcast('AddLocationFromOrderList', row);
        //toastr.error("TODO: Not developed yet");
    };
    //#endregion

    //#region Method

    function bindDropDown() {
        var Attorney = CommonServices.GetAttorneyFormUserId($scope.UserId);
        Attorney.success(function (response) {
            $scope.Attorneylist = response.Data;
        });
    }

    $scope.bindOrderList = function () {
        //$scope.SearchValue = 'tejas';
        if ($.fn.DataTable.isDataTable("#tblOrder")) {
            $('#tblOrder').DataTable().destroy();
        }
        var pagesizeObj = 10;
        $('#tblOrder').DataTable({
            "oLanguage": {
                "sProcessing": '<div class="loader" data-loading><img src="/assets/images/loader2.gif" /></div>',
                "sZeroRecords": "<span class='pull-left'>No records found</span>",
            },
            "stateSave": false,
            "dom": '<"table-responsive"rt><"bottom"lip<"clear">>',
            "bProcessing": true,
            "bServerSide": true,
            "iDisplayStart": 0,
            "iDisplayLength": pagesizeObj,
            "lengthMenu": [10, 20, 30, 40, 50],
            "sAjaxDataProp": "aaData",
            "aaSorting": [[0, 'desc']],
            "sAjaxSource": configurationService.basePath + "GetOrderListForClient",
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                aoData = BindSearchCriteriaCommon(aoData);
                aoData = BindSortingCommon(aoData, oSettings);
                if ($scope.SearchValue == undefined) {
                    $scope.SearchValue = "";
                }
                //aoData.SearchValue = $scope.SearchValue; 
                //aoData.PageIndex = parseInt($('#tblOrder').DataTable().page.info().page) + 1;
                $scope.GridParams = aoData;
                oSettings.jqXHR = $.ajax({
                    'dataSrc': 'aaData',
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource + "?SearchValue=" + $scope.SearchValue + "&PageIndex=" + (parseInt($('#tblOrder').DataTable().page.info().page) + 1)
                        + "&UserAccessId=" + $scope.UserAccessId
                        + "&CompanyNo=" + $rootScope.CompanyNo
                        + "&UserId=" + $scope.UserId
                        + "&OrderID=" + $scope.OrderSearchObj.OrderID
                        + "&AttorneyID=" + ($scope.OrderSearchObj.AttorneyID == null ? "" : $scope.OrderSearchObj.AttorneyID)
                        + "&RecordsOf=" + $scope.OrderSearchObj.RecordsOf
                        + "&Caption=" + $scope.OrderSearchObj.Caption
                        + "&Cause=" + $scope.OrderSearchObj.Cause
                        + "&Claim=" + $scope.OrderSearchObj.Claim
                        + "&ClaimMatterNo=" + $scope.OrderSearchObj.ClaimMatterNo
                        + "&HideArchived=" + $scope.OrderSearchObj.HideArchived,
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });
            },
            "columns": [
                {
                    "title": "Order #",
                    "className": "dt-left",
                    "data": "OrderId",
                    "width": "5%"
                },
                {
                    "title": "Records Of",
                    "className": "dt-left",
                    "data": "RecordsOf",
                    "width": "15%"
                },
                {
                    "title": "Caption",
                    "className": "dt-left",
                    "data": "Caption",
                    "width": "15%"
                },
                {
                    "title": "Cause #",
                    "className": "dt-left",
                    "data": "CauseNo",
                    "width": "10%"
                },
                {
                    "title": "Claim #",
                    "className": "dt-left",
                    "data": "BillingClaimNo",
                    "width": "10%"
                },
                {
                    "title": "Client Matter No #",
                    "className": "dt-left",
                    "data": "ClientMatterNo",
                    "width": "10%"
                },
                {
                    "title": "Status",
                    "className": "dt-center",
                    "data": "SubmitStatus",
                    "width": "8%",
                    "render": function (data, type, row) {
                        //"<a class='label bg-danger-400' title='Click to remove from Rush' ng-click='LocRushClick(" + partNo + ",false);'>
                        return data ? "<label class='label bg-success-400'>Submitted</label>" : "<a class='label bg-danger-400' data-toggle='tooltip' data-placement='bottom' tooltip title='You can delete this order by clicking on Delete button on Action column.' >Draft</a> "
                    }
                },
                {
                    "title": "Order Date",
                    "className": "dt-left",
                    "data": "OrderDateStr",
                    "width": "7%"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "bSortable": false,
                    "width": "7%",
                    "render": function (data, type, row, meta) {
                        var upClass = "";
                        if (meta.row > 4)
                            upClass = "dropup";
                        var strAction = '<div class="dropdown ' + upClass + ' "><a class="dropdown-toggle" data-toggle="dropdown"><i class="icon-three-bars cursor-pointer"></i></a > <ul class="dropdown-menu dropdown-menu-right">';
                        if ($scope.IsUserCanEditOrder) {
                            if (!row.SubmitStatus) {
                                strAction += "<li><a  ng-click='EditOrder($event)' title='Edit' data-toggle='tooltip' data-placement='top' tooltip> <i  class=' icon-pencil4' ></i>Edit Order</a></li> ";
                            }
                            else {
                                if ($scope.RoleName != 'Attorney') {
                                    strAction += "<li><a  ng-click='EditOrder($event)' title='View' data-toggle='tooltip' data-placement='top' tooltip> <i  class='icon-zoomin3' > </i>View Order</a></li> ";
                                }
                            }

                        }
                        if ($scope.IsUserCanViewPart) {
                            strAction += "<li><a ng-click='ShowPartDetail($event)'  title='View Parts' data-toggle='tooltip' data-placement='top' tooltip>  <i  class=' icon-files-empty cursor-pointer'></i> View Parts </a></li>";
                        }
                        if ($scope.IsUserCanAddDocument) {
                            strAction += "<li><a ng-click='AddOrderDocuments($event)'  title='Upload Order Documents' data-toggle='tooltip' data-placement='top' tooltip>  <i  class='icon-file-upload cursor-pointer'></i> Upload Documents</a></li>";
                        }
                        if ($scope.IsUserCanAddLocation && row.CanAddPart) {
                            // strAction += "<li><a  ng-click='AddLocation($event)'  title='Add Location' data-toggle='tooltip' data-placement='top' tooltip>  <i  class=' icon-plus2 cursor-pointer'></i> Add Location</a></li>";
                            strAction += "<li><a href='EditOrder?OrderId=" + row.OrderId + "&Step=6&AddPart=true' target='_blank'   title='Add Location' data-toggle='tooltip' data-placement='top' tooltip>  <i  class=' icon-plus2 cursor-pointer'></i> Add Location</a></li>";
                            // strAction += "<li><a href='EditOrder?OrderId=" + row.OrderId + "&Step=6&AddPart=true' target='_blank'   title='Add Location' data-toggle='tooltip' data-placement='top' tooltip>  <i  class=' icon-plus2 cursor-pointer'></i> Add Location</a></li>";                            

                        }
                        if ($scope.IsUserCanArchiveOrder) {
                            if (!row.IsArchive) {
                                strAction += "<li><a  ng-click='ArchiveOrder($event)'  title='Archive Order' data-toggle='tooltip' data-placement='top' tooltip>  <i  class=' icon-file-minus cursor-pointer'></i> Archive Order </a></li>";
                            }
                            else {
                                strAction += "<li><a  ng-click='ArchiveOrder($event)'  title='Restore Order' data-toggle='tooltip' data-placement='top' tooltip>  <i  class=' icon-file-check cursor-pointer'></i> Restore Order</a></li>";
                            }
                        }
                        if (!row.SubmitStatus) {
                            strAction += "<li><a ng-click='DraftClick(" + row.OrderId + ")'; title = 'Click Here to delete this Draft order.' data-toggle='tooltip' data-placement='top' tooltip> <i class='icon-bin cursor-pointer'></i> Delete Draft</a ></li>";
                        }
                        return strAction + "</li></ul></div>";
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblOrder').DataTable();
                BindCustomerSearchBar($scope, $compile, dataTable);
            },
            "fnDrawCallback": function () {
                //BindToolTip();
                setTimeout(function () { SetAnchorLinks(); }, 500);
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                if (aData.SubmitStatus == false)
                    $(nRow).attr("title", "This is draft order which is not submitted yet, if you want to delete this order please click on Draft button.");
                if (aData.IsArchive) {
                    $(nRow).addClass("archived");
                }

                $compile(angular.element(nRow).contents())($scope);
            }
        });
        // $("#tblOrder").parent().css({ "min-height": "500px !important;" });
        angular.element('#tblOrder').parent().css('min-height', '500px');
    }

    function bindPartByOrderId() {

        if ($.fn.DataTable.isDataTable("#tblPart")) {
            $('#tblPart').DataTable().destroy();
        }

        var table = $('#tblPart').DataTable({
            data: $scope.OrderPartList,
            "bDestroy": true,
            "dom": '<"top pull-left "><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Part  ",
                    "className": "dt-left",
                    "data": "PartNo",
                    "width": "7%",
                    "sorting": "false"
                },
                {
                    "title": "Name1",
                    "className": "dt-left",
                    "data": "Name1",
                    "width": "20%",
                    "sorting": "false"
                },
                {
                    "title": "Name2",
                    "className": "dt-left",
                    "data": "Name2",
                    "width": "10%",
                    "sorting": "false"
                },
                {
                    "title": "City",
                    "className": "dt-left",
                    "data": "City",
                    "width": "10%",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return row.City + ',' + row.State;
                    }
                },
                {
                    "title": "Descr",
                    "className": "dt-left",
                    "data": "Descr",
                    "width": "10%",
                    "sorting": "false"
                },
                {
                    "title": "Status",
                    "className": "dt-left",
                    "data": "PartStatus",
                    "width": "10%",
                    "sorting": "false"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        //strAction = "<a class='ico_btn cursor-pointer' ng-click='EditOrder($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        strAction += "<a class='ico_btn cursor-pointer'   title='Upload Document'>  <i  class='icon-file-upload cursor-pointer'></i> </a>";
                        strAction += "<a class='ico_btn cursor-pointer'   title='Cancel Part'>  <i  class='icon-close2 cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblPart').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }
    function clearSearch() {
        $scope.OrderSearchObj = new Object();
        $scope.OrderSearchObj.OrderID = "";
        $scope.OrderSearchObj.AttorneyID = "";
        $scope.OrderSearchObj.RecordsOf = "";
        $scope.OrderSearchObj.Caption = "";
        $scope.OrderSearchObj.Cause = "";
        $scope.OrderSearchObj.Claim = "";
        $scope.OrderSearchObj.ClaimMatterNo = "";
        $scope.OrderSearchObj.HideArchived = true;
        $scope.SearchValue = "";
    };
    $scope.clearSearchClick = function () { clearSearch(); $scope.bindOrderList(); };




    $scope.DraftClick = function (OrderId) {

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


                    var promise = OrderListService.DeleteDraftOrder(OrderId);
                    promise.success(function (response) {
                        notificationFactory.customSuccess("Order Deleted Successfully.");
                        $scope.bindOrderList();
                    })
                }
                bootbox.hideAll();
            }
        });
    }
    function init() {
        $scope.OpenPartDetailModel = false;
        clearSearch();
        $scope.SearchValue = $stateParams.Search;
        $scope.bindOrderList();
        bindDropDown();
        //bindDropDown();
        //$scope.OrderListobj = new Object();
    }
    //#endregion

    init();
});