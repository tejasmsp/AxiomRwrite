app.controller('AddOrupdateAccountReceivableController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, AccountReceivableService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);

    //Page Rights//    
    $rootScope.CheckIsPageAccessible("Admin", "Attorneys", "View");
    $scope.IsUserCanEditAttorney = $rootScope.isSubModuleAccessibleToUser('Admin', 'Attorneys', 'Edit Attorney');
    //-----


    function init() {

        //GetEmployeelist();

        $scope.isEdit = false;
        $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
        $scope.objAccountRec = new Object();
        $scope.ArID = parseInt($stateParams.ArID);
        $scope.objReceivableInvoice = new Object();
        $scope.modal_Title = "Add Check";
        $scope.CheckType = null;
        $scope.IsCrditORDebitCheck = false;       
        $scope.DebitOrCreditAmount = null;
        $scope.GetAccountReceivable($scope.ArID);
        $scope.TotalPaymentDoneForCheck = 0;
    }

    $scope.GetAccountReceivable = function (ArID) {
        if (ArID > 0) {
            var promise = AccountReceivableService.GetAccountReceivable(ArID);
            promise.success(function (response) {
                if (response.Success) {                   
                    $scope.ShowNote = false;
                    $scope.objAccountRec = response.Data[0];
                    $scope.CheckTypeOld = $scope.objAccountRec.CheckType;
                    $scope.CheckAmount = $scope.objAccountRec.CheckAmount;    
                    $scope.CheckAmountAfterPayment = $scope.CheckAmount;
                    $scope.modal_Title = "Edit Check";
                    if ($scope.ArID > 0) {
                        $scope.ShowInvoiceList = true;
                        $scope.GetAccountReceivableInvoices(ArID,$scope.UserAccessId);
                    }
                    
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function () { });
        }
        else {
            $scope.objAccountRec.ArID = 0;
            $scope.ShowNote = true;
        }

    }
    //Account Receivable
    $scope.AddOrEditAccountReceivable = function (form) {

        if (form.$valid) {            
            $scope.objAccountRec.UserAccessId = $scope.UserAccessId;
            
            if ($scope.ArID > 0) {  // Edit mode	                
                var promise = AccountReceivableService.InsertOrUpdateAccountReceivable($scope.objAccountRec);
                promise.success(function (response) {
                    if (response.Success) {

                        toastr.success("Account Receivable updated successfully.");
                        $scope.CheckAmount = $scope.objAccountRec.CheckAmount;
                        $scope.ShowInvoiceList = true;
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
            else { //Add Mode
                $scope.objAccountRec.ArID = 0;
                var promise = AccountReceivableService.InsertOrUpdateAccountReceivable($scope.objAccountRec);
                promise.success(function (response) {
                    if (response.Success) {
                        toastr.success("Account Receivable inserted successfully.");
                        $scope.CheckAmount = $scope.objAccountRec.CheckAmount;
                        $state.go("ManageCheck", ({ 'ArID': response.ArID }), { reload: true });
                        $scope.ShowInvoiceList = true;
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
        }
    };
    ////////////////////////////////////////////////
    $scope.GetInvoiceDeatiles = function () {
        $scope.InvoiceNo = "";
        $scope.OrderNo = "";
        $scope.BilledAttorney = "";
        $scope.SoldAttorney = "";
        angular.element("#modal_InvoiceDetailList").modal('show');
        $scope.bindInvoices_New();
        
    }
    $scope.GetInvoiceDetailByInvoiceId = function () {
        var promise = AccountReceivableService.GetInvoiceDetailByInvoiceId($scope.objReceivableInvoice.InvNo);
        promise.success(function (response) {
            if (response.Success) {            
                if (response.Data.length > 0) {                    
                    $scope.BounceAmt = "";
                    $scope.DebitAmt = "";
                    $scope.CreditAmt = "";
                    if (response.Data[0].FinChg > 0) {
                        $scope.ShowBounceAmt = true;
                        $scope.BounceAmt = response.Data[0].FinChg;
                    }
                    if (response.Data[0].DebitAmount > 0) {
                        $scope.ShowDebitAmt = true;
                        $scope.DebitAmt = response.Data[0].DebitAmount;
                    }
                    if (response.Data[0].CreditAmount > 0) {
                        $scope.ShowCreditAmt = true;
                        $scope.CreditAmt = response.Data[0].CreditAmount;
                    }
                    $scope.objReceivableInvoice.InvNo = response.Data[0].InvNo;
                    $scope.objReceivableInvoice.InvoicePayableAmount = response.Data[0].InvoiceAmount;
                    $scope.InvoiceAmount = response.Data[0].InvoiceAmount;
                    $scope.SetInvoiceStatus(response.Data[0].Status);
                    $scope.DebitOrCreditAmount = "";
                    if ($scope.objAccountRec.CheckType == 'C' || $scope.objAccountRec.CheckType == 'D') {
                        $scope.DebitOrCreditAmount = ($scope.objReceivableInvoice.InvoicePayableAmount - $scope.CheckAmountAfterPayment) * (-1);
                        $scope.DebitOrCreditAmount = !isNullOrUndefinedOrEmpty($scope.DebitOrCreditAmount) ? $scope.DebitOrCreditAmount.toFixed(2) : "";
                        if ($scope.DebitOrCreditAmount < 0) {
                            $scope.DebitOrCreditAmount = -$scope.DebitOrCreditAmount;
                        }
                    }
                }
                else {
                    $scope.BounceAmt = "";
                    $scope.ShowBounceAmt = false; 
                    $scope.ShowDebitAmt = false;
                    $scope.ShowCreditAmt = false;
                    $scope.objReceivableInvoice.InvoicePayableAmount = "";
                    $scope.InvoiceAmount = "";
                    $scope.DebitOrCreditAmount = "";
                    $scope.InvoiceStatus = "";
                    $scope.CreditAmt = "";
                    $scope.DebitAmt = "";
                    toastr.error("No Invoice Found.");
                }
               
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function () { });
      
     
      
    }
    $scope.ClearSearch = function () {
        $scope.InvoiceNo = '';
        $scope.OrderNo = '';
        $scope.BilledAttorney = '';
        $scope.SoldAttorney = '';
        $scope.FirmID = '';
        $scope.FirmName = '';
        $scope.bindInvoices_New();
    }

    $scope.ViewChangeLogOfCheck = function () {
        var promise = AccountReceivableService.GetChangeLogOfCheck($scope.objAccountRec.ArID);
        promise.success(function (response) {
            if (response.Success) {
                $scope.ChangeLogListOfCheck = response.Data;

                angular.element("#model_ChangeLogOfCheck").modal('show');
                bindChangeLogOfCheck();

            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function () { });
    }

    function bindChangeLogOfCheck() {

        if ($.fn.DataTable.isDataTable("#tblChangeLogOfCheck")) {
            $('#tblChangeLogOfCheck').DataTable().destroy();
        }

        var table = $('#tblChangeLogOfCheck').DataTable({
            data: $scope.ChangeLogListOfCheck,
            "bDestroy": true,
            "dom": '<"top"f><"table"rt><"bottom"lip<"clear">>',
            "aoSorting": true,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": true,
            "columns": [
                {
                    "title": "Field Name",
                    "className": "dt-left",
                    "data": "FieldName"
                },
                {
                    "title": "Previous Value",
                    "className": "dt-left",
                    "data": "OldValue"
                },
                {
                    "title": "Current Value",
                    "className": "dt-left",
                    "data": "NewValue"
                },
                {
                    "title": "Date",
                    "className": "dt-left",
                    "data": "CreatedDate",
                    render: function (data, type, row) {
                        return row.CreatedDate != null ? moment(row.CreatedDate).format("MM/DD/YYYY hh:mm:ss") : "";
                    }

                },



            ],
            "initComplete": function () {
                var dataTable = $('#tblChangeLogOfCheck').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    $scope.bindInvoices_New = function () {
        if ($.fn.DataTable.isDataTable("#tblInvoiceDetailList")) {
            $('#tblInvoiceDetailList').DataTable().destroy();
        }

        var pagesizeObj = 10;
        var table = $('#tblInvoiceDetailList').DataTable({
            stateSave: false,
            "oLanguage": {
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
            "aaSorting": [[0, 'asc']],
            "sAjaxSource": configurationService.basePath + "GetVoidAndAllInVoices",
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                aoData = BindSearchCriteriaCommon(aoData);
                aoData = BindSortingCommon(aoData, oSettings);

                $scope.InvoiceNo = isNullOrUndefinedOrEmpty($scope.InvoiceNo) ? '' : $scope.InvoiceNo;
                $scope.OrderNo = isNullOrUndefinedOrEmpty($scope.OrderNo) ? '' : $scope.OrderNo;
                $scope.BilledAttorney = isNullOrUndefinedOrEmpty($scope.BilledAttorney) ? '' : $scope.BilledAttorney;
                $scope.SoldAttorney = isNullOrUndefinedOrEmpty($scope.SoldAttorney) ? '' : $scope.SoldAttorney;
                $scope.FirmID = '';
                $scope.FirmName = '';
                //if ($scope.objFirmSearch.Name == undefined) {
                //    $scope.objFirmSearch.Name = "";
                //}
                //aoData.SearchValue = $scope.SearchValue; 
                //aoData.PageIndex = parseInt($('#tblSearchFirm').DataTable().page.info().page) + 1;


                $scope.GridParams = aoData;
                oSettings.jqXHR = $.ajax({
                    'dataSrc': 'aaData',
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource + "?PageIndex=" + (parseInt($('#tblInvoiceDetailList').DataTable().page.info().page) + 1)
                        + "&InvoiceNo=" + $scope.InvoiceNo + "&OrderNo=" + $scope.OrderNo + "&UserId=" + $scope.UserAccessId
                        + "&BilledAttorney=" + $scope.BilledAttorney + "&SoldAttorney=" + $scope.SoldAttorney + "&VoidInvoices=" + false + "&AllInvoices=" + false
                        + "&FirmID=" + $scope.FirmID + "&FirmName=" + $scope.FirmName,
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });


            },
            "columns": [
                {
                    "title": "Invoice No",
                    "className": "dt-left",
                    "data": "InvNo",
                    "width": "10%",  
                  
                },
                {
                    "title": "Order No",
                    "className": "dt-left",
                    "data": "OrderNo",
                    "width": "7%", 
                   
                },
                {
                    "title": "Billed Attorney",
                    "className": "dt-left",
                    "data": "BillAtty", 
                    "width": "15%",  
                },
                {
                    "title": "Sold Attorney",
                    "className": "dt-left",
                    "data": "SoldAtty", 
                    "width": "15%",  
                },               
                {
                    "title": "Invoice Payable Amount",
                    "className": "dt-left",
                    "data": "InvAmt",                   
                },
                {
                    "title": "Paid Amount",
                    "className": "dt-left",
                    "data": "PaidAmount",
                    "sorting": "false",  
                },
                {
                    "title": "Bounce Amount",
                    "className": "dt-left",
                    "data": "FinChg",
                    "sorting": "false",  
                },
                {
                    "title": "Credit Amount",
                    "className": "dt-left",
                    "data": "CreditAmount",
                    "sorting": "false",  
                },
                {
                    "title": "Debit Amount",
                    "className": "dt-left",
                    "data": "DebitAmount",
                    "sorting": "false",  
                },
                {
                    "title": "Status",
                    "className": "dt-center",                                   
                    "width": "5%",
                    "data": "StatusDesciption",                    
                    "sorting": "false", 
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "5%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if (row.Status != 4) {
                            strAction = '<a class="ico_btn cursor - pointer" ng-click="AddInvoiceNo($event)" title="Select"> <i class="icon-checkmark4"></i></a>';
                        }                        
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblInvoiceDetailList').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    $scope.AddInvoiceNo = function ($event) {         
        $scope.BounceAmt = "";
        $scope.DebitAmt = "";
        $scope.CreditAmt = "";
        var table = $('#tblInvoiceDetailList').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        if (row.FinChg > 0) {
            $scope.ShowBounceAmt = true;
            $scope.BounceAmt = row.FinChg;
        }
        if (row.DebitAmount > 0) {
            $scope.ShowDebitAmt = true;
            $scope.DebitAmt = row.DebitAmount;
        }
        if (row.CreditAmount > 0) {
            $scope.ShowCreditAmt = true;
            $scope.CreditAmt = row.CreditAmount;
        }
        $scope.objReceivableInvoice.InvNo = row.InvNo;
        $scope.objReceivableInvoice.InvoicePayableAmount = row.InvoicePayableAmount;
        $scope.objReceivableInvoice.InvoicePayableAmount = row.InvoiceAmount;
        $scope.InvoiceAmount = row.InvoiceAmount;
        $scope.SetInvoiceStatus(row.Status);
        $scope.DebitOrCreditAmount = "";
        if ($scope.objAccountRec.CheckType == 'C' || $scope.objAccountRec.CheckType == 'D') {
            $scope.DebitOrCreditAmount = ($scope.objReceivableInvoice.InvoicePayableAmount - $scope.CheckAmountAfterPayment) *(-1);
            $scope.DebitOrCreditAmount = !isNullOrUndefinedOrEmpty($scope.DebitOrCreditAmount) ? $scope.DebitOrCreditAmount.toFixed(2) : "";
            if ($scope.DebitOrCreditAmount < 0) {
                $scope.DebitOrCreditAmount = -$scope.DebitOrCreditAmount;
            }
        }
        
        angular.element("#modal_InvoiceDetailList").modal('hide');
    };

    ////////////////////////////////////////


    $scope.GetAccountReceivableInvoices = function (ArID, UserAccessId) {
        var promise = AccountReceivableService.GetAccountReceivableInvoice(ArID,UserAccessId);
        promise.success(function (response) {
            if (response.Success) {
                $scope.AccountReceivableInvoicesList = response.Data;
                if (response.Data.length > 0) {
                    $scope.TotalPaymentDoneForCheck = response.Data[0].TotalPaymentDoneForCheck;
                    $scope.CheckAmountAfterPayment = ($scope.CheckAmount - $scope.TotalPaymentDoneForCheck).toFixed(2);
                } else {
                    $scope.CheckAmountAfterPayment = ($scope.CheckAmount).toFixed(2);
                }

                BindAccountReceivableInvoices();
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function () { });
    }

    function BindAccountReceivableInvoices() {

        if ($.fn.DataTable.isDataTable("#tblAccountReceivableInvoice")) {
            $('#tblAccountReceivableInvoice').DataTable().destroy();
        }

        var table = $('#tblAccountReceivableInvoice').DataTable({
            data: $scope.AccountReceivableInvoicesList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Invoice No",
                    "className": "dt-left",
                    "data": "InvNo",
                    "sorting": "false"
                },
                {
                    "title": "Invoice Amount",
                    "className": "dt-left",
                    "data": "InvoiceAmount",
                    "sorting": "false"
                },
                {
                    "title": "Note",
                    "className": "dt-left",
                    "data": "Note",
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
                        if ((isNullOrUndefinedOrEmpty(row.AdjustmentType) || row.AdjustmentType < 1) && $scope.objAccountRec.CheckType == 'N') {
                            strAction = "<a class='ico_btn cursor-pointer'  ng-click='EditAccountReceivableInvoice($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }    
                        
                        if ($scope.objAccountRec.CheckType == "N" || $scope.objAccountRec.CheckType == "C" || $scope.objAccountRec.CheckType == "D") {
                            strAction = strAction + "<a class='ico_btn cursor-pointer'  ng-click='DeleteInvoicePayment($event)' title='Delete Invoice Payment'> <i  class='glyphicon glyphicon-trash' ></i></a> ";                      
                        }                     
                        strAction = strAction + "<a class='ico_btn cursor-pointer' ng-click='ViewARInvoiceChangeLogByInvoiceId($event)' title='View Change Log'> <i  class='icon-history' ></i></a> ";
                        //strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteCourt($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblAccountReceivableInvoice').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    };

    $scope.EditAccountReceivableInvoice = function ($event) {
        if ($scope.objAccountRec.CheckType == 'L' || $scope.objAccountRec.CheckType == 'B') {
            return;
        } else {
            var table = $('#tblAccountReceivableInvoice').DataTable();
            var row = table.row($($event.target).parents('tr')).data();
            $scope.GetAccountReceivableInvoicesById(row.CheckInvoiceId);
        }
      

    }
    $scope.DeleteInvoicePayment = function ($event) {       
        var table = $('#tblAccountReceivableInvoice').DataTable();
        var row = table.row($($event.target).parents('tr')).data();       
        var _DeleteInvoicePayment = AccountReceivableService.DeleteInvoicePayment(row.CheckInvoiceId,$scope.UserAccessId);
        _DeleteInvoicePayment.success(function (response) {
            if (response.Success) {
                toastr.success("Invoice Payment updated successfully.");
                $scope.GetAccountReceivableInvoices($scope.ArID,$scope.UserAccessId);
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        _DeleteInvoicePayment.error(function () { });

    }
    $scope.ViewARInvoiceChangeLogByInvoiceId = function ($event) {
        var table = $('#tblAccountReceivableInvoice').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        var ViewARInvoiceChangeLog = AccountReceivableService.GetARInvoiceChangeLogByInvoiceId(row.ArID, row.InvNo);
        ViewARInvoiceChangeLog.success(function (response) {
            if (response.Success) {
                $scope.ChangeLogListOfInvoices = response.Data;
                angular.element("#model_ChangeLogOfInvoices").modal('show');
                bindChangeLoginvoice();
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        ViewARInvoiceChangeLog.error(function () { });
        //$scope.ViewARInvoiceChangeLogByInvoiceId(row.CheckInvoiceId, row.InvNo);

    }
    $scope.GetAccountReceivableInvoicesById = function (InvoiceId) {
        var promise = AccountReceivableService.GetAccountReceivableInvoicesById(InvoiceId);
        promise.success(function (response) {
            if (response.Success) {
               
                $scope.ReceivableInvoiceTitle = "Edit Invoice";
                $scope.objReceivableInvoice = response.Data[0];
                $scope.InvoiceAmount = response.Data[0].InvoiceAmount;

                $scope.SetInvoiceStatus($scope.objReceivableInvoice.Status);
                angular.element("#modal_Invoice").modal('show');
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function () { });
    }
    $scope.SetInvoiceStatus = function (Status) {
        if (Status == 1) {
            $scope.InvoiceStatus = "UnPaid";
        }
        if (Status == 2) {
            $scope.InvoiceStatus = "Partially Paid";
        }
        if (Status == 3) {
            $scope.InvoiceStatus = "FULL Paid";
        }
        if (Status == 4) {
            $scope.InvoiceStatus = "Void";
        }

    }
    $scope.FirmSearch = function (event) {
        $scope.ShowSearchFirm = true;
    };
   
    $scope.AddInvoice = function () {
        $scope.ReceivableInvoiceTitle = "Add Invoice";
        $scope.objReceivableInvoice = new Object();
        $scope.InvoiceStatus = "";
        $scope.InvoiceAmount = "";       
        $scope.DebitOrCreditAmount = "";
        $scope.ShowBounceAmt = false;
        $scope.ShowDebitAmt = false;
        $scope.ShowCreditAmt = false;
        $scope.BounceAmt = "";
        $scope.DebitAmt = "";
        $scope.CreditAmt = "";        
        $scope.objReceivableInvoice.ArID = $scope.ArID;
        $scope.objReceivableInvoice.CheckInvoiceId = 0;        
        $scope.CheckAmountAfterPayment = ($scope.CheckAmount - $scope.TotalPaymentDoneForCheck).toFixed(2);
        if ($scope.objAccountRec.CheckType == 'C' || $scope.objAccountRec.CheckType == 'D') {
            $scope.IsCrditORDebitCheck = true;
        }        
        $scope.ReceivableInvoice.$setPristine();
        angular.element("#modal_Invoice").modal('show');
    }
    // Invoice
    $scope.AddOrEditAccountReceivableInvoice = function (form) {
        if (form.$valid) {
            $scope.objReceivableInvoice.CreatedBy = $scope.UserAccessId;         
           
            if ($scope.objAccountRec.CheckType == 'N' ) {
                if (parseFloat($scope.objReceivableInvoice.InvoicePayableAmount) > $scope.CheckAmountAfterPayment) {
                    toastr.error("Invoice Payable Amount should be less than or eaual to Check Payable Amount");
                    return false;
                }
            }
            
            if ($scope.objAccountRec.CheckType == 'C' ) {
                if (parseFloat($scope.objReceivableInvoice.InvoicePayableAmount) < $scope.CheckAmountAfterPayment) {
                    toastr.error("Invoice Payable Amount should be greater than or eaual to Credit Amount");
                    return false;
                }
            }
            
            //  $scope.objReceivableInvoice.CheckType = $scope.objAccountRec.CheckType;
            if ($scope.objReceivableInvoice.CheckInvoiceId > 0) {
                // Edit mode       
               
                if ($scope.objAccountRec.CheckType == 'C') {
                    var promise = AccountReceivableService.UpdateAccountReceivableInvoiceForCreditCheck($scope.objReceivableInvoice);
                }
                else if ($scope.objAccountRec.CheckType == 'D'){
                    var promise = AccountReceivableService.UpdateCheckInvoicePaymentForDebitCheck($scope.objReceivableInvoice);
                }
                else {
                    var promise = AccountReceivableService.UpdateAccountReceivableInvoice($scope.objReceivableInvoice);
                }
               // var promise = AccountReceivableService.UpdateAccountReceivableInvoice($scope.objReceivableInvoice);
                promise.success(function (response) {
                    if (response.Success) {
                        toastr.success("Account Receivable invoice updated successfully.");
                        angular.element("#modal_Invoice").modal('hide');
                        $scope.GetAccountReceivableInvoices($scope.ArID, $scope.UserAccessId);
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
            else { //Add Mode                    
                if ($scope.objAccountRec.CheckType == 'C') {
                    var promise = AccountReceivableService.InsertAccountReceivableInvoiceForCreditCheck($scope.objReceivableInvoice);
                }
                else if ($scope.objAccountRec.CheckType == 'D') {
                    var promise = AccountReceivableService.InsertCheckInvoicePaymentForDebitCheck($scope.objReceivableInvoice);
                }
                else {
                    var promise = AccountReceivableService.InsertAccountReceivableInvoice($scope.objReceivableInvoice);
                }
                // var promise = AccountReceivableService.InsertAccountReceivableInvoice($scope.objReceivableInvoice);
                promise.success(function (response) {
                    if (response.Success) {                      
                        toastr.success("Account Receivable invoice inserted successfully.");
                        angular.element("#modal_Invoice").modal('hide');
                        $scope.GetAccountReceivableInvoices($scope.ArID, $scope.UserAccessId);
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
        }
    };

    //$scope.CancelBounceCheck = function () {
    //    $("#model_ConfirmBounceCheck").modal('hide');
    //    $scope.objAccountRec.CheckType = $scope.CheckTypeOld;
    //}
    $scope.ConfirmBounceCheck = function () {
        angular.element("#model_ConfirmBounceCheck").modal('show');
    }
    $scope.ConfirmationForCancelCheck = function () {
        angular.element("#model_ConfirmCancelCheck").modal('show');
    }

    $scope.BounceAndCancelCheck = function (triggerId) {
        if (triggerId == 1) {
            $scope.CheckType = "L";
        }
        if (triggerId == 2) {
            $scope.CheckType = "B";
        }
        var promise = AccountReceivableService.BounceAndCancelCheck($scope.objAccountRec.ArID, $scope.UserAccessId, $scope.CheckType);
        promise.success(function (response) {
            if (response.Success) {
                if (triggerId == 1) {
                    $("#model_ConfirmCancelCheck").modal('hide');
                }
                if (triggerId == 2) { $("#model_ConfirmBounceCheck").modal('hide'); }
                $('.modal-backdrop').remove();
                $state.go('Check', { reload: true });
                //$scope.InvoicesList = response.Data;
                //angular.element("#modal_InvoiceDetailList").modal('show');
                //bindInvoices();
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function () { });
    }

    function bindChangeLoginvoice() {

        if ($.fn.DataTable.isDataTable("#tblChangeLogOfInvoices")) {
            $('#tblChangeLogOfInvoices').DataTable().destroy();
        }

        var table = $('#tblChangeLogOfInvoices').DataTable({
            data: $scope.ChangeLogListOfInvoices,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Invoice No",
                    "className": "dt-left",
                    "data": "InvoiceId",
                    "sorting": "false"
                },
                {
                    "title": "Check Number",
                    "className": "dt-left",
                    "data": "CheckNumber",
                    "sorting": "false"
                },
                {
                    "title": "Log Detail",
                    "className": "dt-left",
                    "data": "Description",
                    "sorting": "false"
                },
                {
                    "title": "Previous Payment",
                    "title": "Prev. Inv. Amt.",
                    "className": "dt-left",
                    "data": "InvoiceTotalBeforePayment",
                    "sorting": "false"
                },
                {
                    "title": "Payment",
                    "className": "dt-left",
                    "data": "Payment",
                    "sorting": "false"
                },
                {
                    "title": "Remaining Payment",
                    "title": "Updated Inv. Amt.",
                    "className": "dt-left",
                    "data": "InvoiceRemaingPayment",
                    "sorting": "false"
                },               

                //{
                //    "title": "Status",
                //    "className": "dt-left",
                //    "data": "Description",
                //    "sorting": "false"
                //},
                {
                    "title": "Reason",
                    "title": "Detail",
                    "className": "dt-left",
                    "data": "Reason",
                    "sorting": "false"
                },
                {
                    "title": "Date",
                    "className": "dt-left",
                    "data": "CreatedDate",
                    "sorting": "false",
                    render: function (data, type, row) {
                        return row.CreatedDate != null ? moment(row.CreatedDate).format("MM/DD/YYYY hh:mm:ss") : "";
                    }
                    

                },

            ],
            "initComplete": function () {
                var dataTable = $('#tblChangeLogOfInvoices').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    };

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
                    "url": sSource + "?SearchValue=" + "" + "&PageIndex=" + (parseInt($('#tblAttorney').DataTable().page.info().page) + 1)
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
                { "data": "AttorneyName", "title": "Attorney Name", "className": "dt-left", "width": "15%" },
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

    init();

});