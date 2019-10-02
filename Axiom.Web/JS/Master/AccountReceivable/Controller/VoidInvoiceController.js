app.controller('VoidInvoicesController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, AccountReceivableService, configurationService, CommonServices, $compile, $filter) {

   

 decodeParams($stateParams);

 //Page Rights//    
 $rootScope.CheckIsPageAccessible("Admin", "Attorneys", "View");
 $scope.IsUserCanEditAttorney = $rootScope.isSubModuleAccessibleToUser('Admin', 'Attorneys', 'Edit Attorney');
 //-----


 function init() {

     //GetEmployeelist();
     $scope.isEdit = false;
     $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
     $scope.FirmObj = new Object();
     $scope.objVoidInvoice = new Object();
     $('input[name="AllInvoices"]').attr('checked', true);    
     $scope.objVoidInvoice.VoidInvoices = false;
     $scope.objVoidInvoice.AllInvoices = true;
     $scope.bindVoidInvoices();
   
 }
  
    $('input[name="AllInvoices"]').click(function () {    
        $scope.objVoidInvoice.VoidInvoices = false;
        $scope.objVoidInvoice.AllInvoices = true;
        $('input[name="VoidInvoices"]').attr('checked', false);
       
    });
    $('input[name="VoidInvoices"]').click(function () {
        $scope.objVoidInvoice.VoidInvoices = true;
        $scope.objVoidInvoice.AllInvoices = false;
        $('input[name="AllInvoices"]').attr('checked', false);
    });
   
    $scope.bindVoidInvoices = function () {
        if ($.fn.DataTable.isDataTable("#tblVoidInvoices")) {
            $('#tblVoidInvoices').DataTable().destroy();
        }

        var pagesizeObj = 10;
        var table = $('#tblVoidInvoices').DataTable({
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

                $scope.objVoidInvoice.InvoiceNo = isNullOrUndefinedOrEmpty($scope.objVoidInvoice.InvoiceNo) ? '' : $scope.objVoidInvoice.InvoiceNo;
                $scope.objVoidInvoice.OrderNo = isNullOrUndefinedOrEmpty($scope.objVoidInvoice.OrderNo) ? '' : $scope.objVoidInvoice.OrderNo;
                $scope.objVoidInvoice.BilledAttorney = isNullOrUndefinedOrEmpty($scope.objVoidInvoice.BilledAttorney) ? '' : $scope.objVoidInvoice.BilledAttorney;
                $scope.objVoidInvoice.SoldAttorney = isNullOrUndefinedOrEmpty($scope.objVoidInvoice.SoldAttorney) ? '' : $scope.objVoidInvoice.SoldAttorney;
                $scope.objVoidInvoice.FirmID = isNullOrUndefinedOrEmpty($scope.objVoidInvoice.FirmID) ? '' : $scope.objVoidInvoice.FirmID;;
                $scope.objVoidInvoice.FirmName = isNullOrUndefinedOrEmpty($scope.objVoidInvoice.FirmName) ? '' : $scope.objVoidInvoice.FirmName;;
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
                    "url": sSource + "?PageIndex=" + (parseInt($('#tblVoidInvoices').DataTable().page.info().page) + 1)
                        + "&InvoiceNo=" + $scope.objVoidInvoice.InvoiceNo + "&OrderNo=" + $scope.objVoidInvoice.OrderNo + "&UserId=" + $scope.UserAccessId
                        + "&BilledAttorney=" + $scope.objVoidInvoice.BilledAttorney + "&SoldAttorney=" + $scope.objVoidInvoice.SoldAttorney + "&VoidInvoices=" + $scope.objVoidInvoice.VoidInvoices + "&AllInvoices=" + $scope.objVoidInvoice.AllInvoices
                        + "&FirmID=" + $scope.objVoidInvoice.FirmID + "&FirmName=" + $scope.objVoidInvoice.FirmName,
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
                    "sorting": "true"
                },
                {
                    "title": "Order No",
                    "className": "dt-left",
                    "data": "OrderNo",
                    "sorting": "true"
                },
                {
                    "title": "Invoice Amount",
                    "className": "dt-left",
                    "data": "InvoiceAmount",
                    "sorting": "true"
                },
                {
                    "title": "Billed Attorney",
                    "className": "dt-left",
                    "data": "BillAttyName",
                    "sorting": "true"
                },  
                {
                    "title": "Sold Attorney",
                    "className": "dt-left",
                    "data": "SoldAttyName",
                    "sorting": "true"
                },
                {
                    "title": "Paid Amount",
                    "className": "dt-left",
                    "data": "PaidAmount",
                    "width":"5%",
                    "sorting": "false"
                },
                {
                    "title": "Status",
                    "className": "dt-center",
                    "sorting": "false",
                    "width": "5%",
                    "data": "StatusDesciption",
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "5%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if ($scope.objVoidInvoice.AllInvoices) {
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='EditInvoiceStatus($event)' title='Make void' data-toggle='tooltip' data-placement='top' tooltip> <i  class='icon-eye-blocked2' ></i></a> ";
                        } else {
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='EditInvoiceStatus($event)' title='Make UnVoid' data-toggle='tooltip' data-placement='top' tooltip> <i  class='icon-eye2' ></i></a> ";
                        }                       
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblVoidInvoices').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }
    $scope.EditInvoiceStatus = function ($event) {
        var table = $('#tblVoidInvoices').DataTable();
        var row = table.row($($event.target).parents('tr')).data();      
        var invoicestatus = null;
        if (row.Status == 4) {
            invoicestatus = 1;
        }else
        {
            invoicestatus = 4;
        }
        var promise = AccountReceivableService.SetInvoiceStatus(row.InvNo, invoicestatus);
        promise.success(function (response) {
            if (response.Success) {
                $scope.bindVoidInvoices();
               
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function () { });
    }
  
  
 $scope.ClearSearch = function () {
     $scope.objVoidInvoice.FirmID = "";
     $scope.objVoidInvoice.FirmName = "";
     $scope.objVoidInvoice.VoidInvoices =false;
     $scope.objVoidInvoice.AllInvoices = true;
     $scope.objVoidInvoice.CheckNo = "";
     $scope.objVoidInvoice.InvoiceNo = "";
     $scope.objVoidInvoice.BilledAttorney = "";
     $scope.objVoidInvoice.SoldAttorney = "";
     $scope.objVoidInvoice.OrderNo = "";
     $('input[name="VoidInvoices"]').prop('checked', false);        
     $scope.bindVoidInvoices();
     setTimeout(function () {
         $('input[name="AllInvoices"]').attr('checked', true);   
     }, 200);
 }



 $scope.FirmPopup = function () {
     $scope.ShowSearchFirm = true;
 }



 init();

    });
