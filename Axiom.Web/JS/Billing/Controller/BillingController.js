app.controller('BillingController', function ($scope, $state, $rootScope, $stateParams, notificationFactory, BillingService, AccountReceivableService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.UserGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.isEdit = false;
    $scope.SoldToAttorneyList;
    $scope.BillToAttorneyList;
    $scope.BillToDetails;
    $scope.EditInvoiceList;
    $scope.RecordTypeList;
    $scope.EditItemList;
    $scope.InvoiceDetaiItem;

    $rootScope.pageTitle = "Billing";

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Billing", "Billing", "View");
    //------------

    $scope.DeleteInvoice = function (invoiceNumber, itemNumber) {
        bootbox.confirm({
            message: "Are you sure you want to delete this record ?",
            buttons: {
                confirm: { label: 'Yes', className: 'btn-success' },
                cancel: { label: 'No', className: 'btn-danger' }
            },
            callback: function (result) {
                if (result == true) {

                    var promise = BillingService.DeleteInvoice(invoiceNumber, itemNumber);
                    promise.success(function (response) {
                        if (response.Success) {

                            notificationFactory.customSuccess("Invoice deleted successfully");
                            bootbox.hideAll();
                            $scope.EditInvoice(invoiceNumber);
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function () {

                    });
                }
            }
        });
    };

    $scope.SendEmail = function (AttyID, invoiceNumber) {
        // AttyID, InvoiceNumber, OrderNumber, Location, Locationname, Patient
        //AttyID,invoiceNumber,$scope.objBilling.OrderNo,$scope.objBilling.LocID,$scope.objBilling.LocationName,$scope.objBilling.RecordsOf

        var promise = BillingService.SendEmailForBill(AttyID, invoiceNumber, $scope.objBilling.OrderNo, $scope.objBilling.PartNo, $scope.objBilling.LocID, $scope.objBilling.LocationName, $scope.objBilling.RecordsOf, $scope.UserGuid, $rootScope.CompanyNo);

        promise.success(function (response) {
            if (response.Success) {
                var res = response.Data;
            }
        });
    };

    $scope.PrintInvoice = function (invoiceNumber) {

        OpenInvoiceReportViewer(invoiceNumber);
        //Old code
        if (false) {
            var url = $state.href('PrintInvoice', { 'InvoiceID': invoiceNumber, 'OrderId': $scope.objBilling.OrderNo, 'PartNo': $scope.objBilling.PartNo, 'IsPrintAll': $scope.IsPrintAll });
            window.open(url, '_blank');
        } 
    };

    $scope.FirmSearch = function (event) {
        $scope.ShowSearchFirm = true;
    };

    $scope.GenerateInvoice = function () {

        $scope.objSoldAtty = [];
        angular.forEach($scope.SoldToAttorneyList, function (value, index) {
            $scope.objSoldAtty.push({ AttyId: value.AttyId, AttyType: "Ordering" });
        });

        var promise = BillingService.GenerateInvoice($scope.objBilling.OrderNo, $scope.objBilling.PartNo, $scope.objBilling.BillToAttorney, $scope.objSoldAtty,$rootScope.CompanyNo);
        promise.success(function (response) {
            if (response.Success) {
                $scope.GetInvoiceList();
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
    };

    $scope.UpdateInvoice = function (form) {
        if (form.$valid) {
            $scope.UpdateInvoiceInfo.OrigRate = $scope.objEditInvoice.OrigRate;
            $scope.UpdateInvoiceInfo.CopyRate = $scope.objEditInvoice.CopyRate;
            $scope.UpdateInvoiceInfo.Pages = $scope.objEditInvoice.Pages;
            $scope.UpdateInvoiceInfo.StdFee1 = $scope.objEditInvoice.BasicFee;
            $scope.UpdateInvoiceInfo.StdFee2 = $scope.objEditInvoice.Subpoena;
            $scope.UpdateInvoiceInfo.StdFee4 = $scope.objEditInvoice.CustodianFees;
            $scope.UpdateInvoiceInfo.StdFee5 = $scope.objEditInvoice.Binding;
            $scope.UpdateInvoiceInfo.StdFee6 = $scope.objEditInvoice.Shipping;
            $scope.UpdateInvoiceInfo.MiscChrge = $scope.objEditInvoice.MiscCharge;

            if ($scope.objEditInvoice.Attorney && typeof $scope.objEditInvoice.Attorney === 'object')
                $scope.UpdateInvoiceInfo.BillAtty = $scope.objEditInvoice.Attorney.AttyId;
            else
                $scope.UpdateInvoiceInfo.BillAtty = $scope.objEditInvoice.Attorney;


            var promise = BillingService.UpdateInvoice($scope.UpdateInvoiceInfo);

            promise.success(function (response) {
                if (response.Success) {
                    promise.success(function (response) {
                        notificationFactory.customSuccess("Invoice updated Successfully");
                        $scope.GetInvoiceList();
                        angular.element("#modal_EditInvoice").modal('hide');
                    });
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
        }
    };

    $scope.EditInvoiceByItemNumber = function (itemNumber) {
        var item = $filter('filter')($scope.EditItemList, { ItemNo: itemNumber }, true);

        $scope.objEditInvoice = new Object();
        $scope.objEditInvoice.RcvdDate = item[0].RcvdDate;
        $scope.objEditInvoice.InvHdr = item[0].InvHdr;



        $scope.objEditInvoice.OrigRate = item[0].OrigRate;
        $scope.objEditInvoice.CopyRate = item[0].CopyRate;
        $scope.objEditInvoice.Pages = item[0].Pages;
        $scope.objEditInvoice.Copies = item[0].Copies;
        $scope.objEditInvoice.CustodianFees = item[0].StdFee4;
        $scope.objEditInvoice.Shipping = item[0].StdFee6;
        $scope.objEditInvoice.Subpoena = item[0].StdFee2;
        $scope.objEditInvoice.MiscCharge = item[0].MiscChrge;
        $scope.objEditInvoice.BasicFee = item[0].StdFee1;
        $scope.objEditInvoice.Binding = item[0].StdFee5;
        $scope.objEditInvoice.FirmID = item[0].FirmID;
        $scope.objEditInvoice.FirmName = response.Data[0].FirmName;
        $scope.objEditInvoice.Original = response.Data[0].Original;
        $scope.objEditInvoice.Attorney = response.Data[0].BillAtty;


        $scope.UpdateInvoiceInfo = new Object();
        $scope.UpdateInvoiceInfo.InvoiceNumber = item[0].InvoiceNumber;
        $scope.UpdateInvoiceInfo.RcvdID = item[0].RcvdID;
        $scope.UpdateInvoiceInfo.ItemNo = item[0].ItemNo;

    }

    $scope.EditInvoice = function (invoiceNumber) {
        angular.element("#modal_EditInvoice").modal('show');

        var promise = BillingService.EditInvoice(invoiceNumber);
        promise.success(function (response) {
            if (response.Success) {
                promise.success(function (response) {
                    if (response.Data.length > 0) {

                        $scope.objEditInvoice = new Object();
                        $scope.GetRecordTypeListForBilling(response.Data[0].InvHdr);
                        $scope.GetAttorneyListByFirmId(response.Data[0].FirmID, response.Data[0].BillAtty);
                        //notificationFactory.customSuccess("UserProfile Update Successfully");
                        //angular.element("#modal_MyProfile").modal('hide');
                        // GetBilltoDetails();

                        $scope.objEditInvoice.RcvdDate = response.Data[0].RcvdDate;
                        $scope.objEditInvoice.OrigRate = response.Data[0].OrigRate;
                        $scope.objEditInvoice.CopyRate = response.Data[0].CopyRate;
                        $scope.objEditInvoice.Pages = response.Data[0].Pages;
                        $scope.objEditInvoice.Copies = response.Data[0].Copies;
                        $scope.objEditInvoice.CustodianFees = response.Data[0].StdFee4;
                        $scope.objEditInvoice.Shipping = response.Data[0].StdFee6;
                        $scope.objEditInvoice.Subpoena = response.Data[0].StdFee2;
                        $scope.objEditInvoice.MiscCharge = response.Data[0].MiscChrge;
                        $scope.objEditInvoice.BasicFee = response.Data[0].StdFee1;
                        $scope.objEditInvoice.Binding = response.Data[0].StdFee5;
                        $scope.objEditInvoice.FirmID = response.Data[0].FirmID;
                        $scope.objEditInvoice.FirmName = response.Data[0].FirmName;
                        $scope.objEditInvoice.Original = response.Data[0].Original;
                        $scope.objEditInvoice.Attorney = response.Data[0].BillAtty;
                        $scope.objEditInvoice.MemberID = response.Data[0].MemberOf;


                        $scope.UpdateInvoiceInfo = new Object();
                        $scope.UpdateInvoiceInfo.InvoiceNumber = invoiceNumber;
                        $scope.UpdateInvoiceInfo.RcvdID = response.Data[0].RcvdID;
                        $scope.UpdateInvoiceInfo.ItemNo = response.Data[0].ItemNo;
                        $scope.EditItemList = response.Data;
                        $scope.InvoiceDetaiItem = response.Data;


                    }
                    else {
                        angular.element("#modal_EditInvoice").modal('hide');
                        $scope.SelectInvoiceDetail($scope.frmOrderSearch);
                    }
                });
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    };


    $scope.GetRecordTypeListForBilling = function (InvHdr) {

        var promise = BillingService.GerRecordTypeListForBilling();
        promise.success(function (response) {
            if (response.Success) {
                promise.success(function (response) {
                    $scope.RecordTypeList = response.Data;
                    if (InvHdr) {
                        var objInv = $filter('filter')($scope.RecordTypeList, { Descr: InvHdr }, true);
                        if (objInv && objInv.length > 0) {
                            $scope.objEditInvoice.InvHdr = objInv[0];
                            $scope.objEditInvoice.InvHdrdesc = InvHdr;
                        }
                    }
                });
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.GetInvoiceList = function () {

        var promise = BillingService.GetAllInvoiceByOrderIdAndPartId($scope.objBilling.OrderNo, $scope.objBilling.PartNo)
        promise.success(function (response) {
            if (response.Success) {
                promise.success(function (response) {
                    $scope.InvoiceDetailList = response.Data;
                });
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.SelectInvoiceDetail = function (form) {
        if (form.$valid) {
            var promise = BillingService.GetAllInvoiceByOrderIdAndPartId($scope.objBilling.OrderNo, $scope.objBilling.PartNo)
            promise.success(function (response) {
                if (response.Success) {
                    promise.success(function (response) {
                        $scope.InvoiceDetailList = response.Data;
                        //notificationFactory.customSuccess("UserProfile Update Successfully");
                        //angular.element("#modal_MyProfile").modal('hide');
                        GetBilltoDetails();
                        GetSoldtoDetails();
                    });
                }
                else {
                    toastr.error(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) {
            });
        }
    };

    $scope.PrintSelectedInvoice = function () {
        var SelectedInvoiceIds = [];
        var SelectedInvoiceList = [];
        if (!$scope.IsPrintAll) {
            SelectedInvoiceList = $filter('filter')($scope.InvoiceDetailList, { IsPrint: true });
        }
        else {
            SelectedInvoiceList = $scope.InvoiceDetailList;
        } 
        for (var i = 0; i < SelectedInvoiceList.length; i++) {
            SelectedInvoiceIds.push(SelectedInvoiceList[i].InvoiceNumber);
        }

        if (SelectedInvoiceIds == null || SelectedInvoiceIds.length == 0) {
            toastr.warning("Please selected invoice");
        } else {
            OpenInvoiceReportViewer(SelectedInvoiceIds.join(','));
        }

        //Old code
        if (false) {
            var SelectedInvoiceIds = "";
            if (!$scope.IsPrintAll) {
                var SelectedInvoiceList = $filter('filter')($scope.InvoiceDetailList, { IsPrint: true });
                for (var i = 0; i < SelectedInvoiceList.length; i++) {
                    SelectedInvoiceIds = SelectedInvoiceIds + SelectedInvoiceList[i].InvoiceNumber + ",";
                }
            }
            if (!($scope.IsPrintAll) && isNullOrUndefinedOrEmpty(SelectedInvoiceIds)) {
                toastr.warning("Please selected invoice");
            } else { 
                var url = $state.href('PrintInvoice', { 'InvoiceID': SelectedInvoiceIds, 'OrderId': $scope.objBilling.OrderNo, 'PartNo': $scope.objBilling.PartNo, 'IsPrintAll': $scope.IsPrintAll });
                window.open(url, '_blank');
            }
        }
    };

    function GetSoldtoDetails() {
        var promise = BillingService.GetSoldToAttorneyDetailsByOrderId($scope.objBilling.OrderNo, $scope.objBilling.PartNo);
        promise.success(function (response) {
            if (response.Success) {
                promise.success(function (response) {
                    $scope.SoldToDetails = response.Data;
                    $scope.objBilling.SoldToFirmName = $scope.SoldToDetails[0].FirmName;
                    $scope.objBilling.SoldToFirmID = $scope.SoldToDetails[0].FirmID;
                    $scope.objBilling.RecordsOf = $scope.SoldToDetails[0].PatientName;
                    //$scope.objBilling.BillToFirmID = $scope.BillToDetails[0].BillingFirmID;
                    //$scope.objBilling.BillToFirmName = $scope.BillToDetails[0].FirmName;
                    //$scope.objBilling.RecordsOf = $scope.BillToDetails[0].PatientName;
                    //$scope.objBilling.LocID = $scope.BillToDetails[0].LocID;
                    //$scope.objBilling.LocationName = $scope.BillToDetails[0].LocationName;

                    //notificationFactory.customSuccess("UserProfile Update Successfully");
                    //angular.element("#modal_MyProfile").modal('hide');
                    GetSoldToAttorneyList();
                });
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    function GetBilltoDetails() {
        var promise = BillingService.GetBillToAttorneyDetailsByOrderId($scope.objBilling.OrderNo, $scope.objBilling.PartNo)
        promise.success(function (response) {
            if (response.Success) {
                promise.success(function (response) {
                    $scope.BillToDetails = response.Data;
                    $scope.objBilling.BillToFirmID = $scope.BillToDetails[0].BillingFirmID;
                    $scope.objBilling.BillToFirmName = $scope.BillToDetails[0].FirmName;
                    $scope.objBilling.LocID = $scope.BillToDetails[0].LocID;
                    $scope.objBilling.LocationName = $scope.BillToDetails[0].LocationName;

                    //notificationFactory.customSuccess("UserProfile Update Successfully");
                    //angular.element("#modal_MyProfile").modal('hide');
                    GetBilltoAttorneyList();
                });
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    function GetSoldToAttorneyList() {

        var promise = BillingService.GetSoldToAttorneyByOrderNo($scope.objBilling.OrderNo, $scope.objBilling.PartNo)
        promise.success(function (response) {
            if (response.Success) {
                promise.success(function (response) {

                    if (response.Data.length > 0) {
                        $scope.SoldToAttorneyList = response.Data;
                        $scope.objBilling.SoldToAttorney = $scope.SoldToAttorneyList[0].AttyId;
                    }
                });
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    }
    $scope.GetAttorneyListByFirmId = function (firmId, attyId) {
        var promise = BillingService.GetBillToAttorneyByFirmId(firmId);
        promise.success(function (response) {
            if (response.Success) {
                promise.success(function (response) {
                    $scope.AttorneyList = response.Data;
                    $scope.objEditInvoice.Attorney = '';
                    if (attyId) {
                        var objAtty = $filter('filter')($scope.AttorneyList, { AttyId: attyId }, true);
                        if (objAtty && objAtty.length > 0) {
                            $scope.objEditInvoice.Attorney = objAtty[0];
                        }
                    }

                });
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    function GetBilltoAttorneyList() {

        var promise = BillingService.GetBillToAttorneyByFirmId($scope.objBilling.BillToFirmID);
        promise.success(function (response) {
            if (response.Success) {
                promise.success(function (response) {
                    $scope.BillToAttorneyList = response.Data;
                    $scope.AttorneyList = angular.copy(response.Data);
                    $scope.objBilling.BillToAttorney = $scope.BillToDetails[0].BillingAttorneyID;
                });
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.ChangePrintAll = function (IsPrintAll) {
        if (IsPrintAll) {
            $('.tblInvoiceDeatil .chk-print-invoice').prop('checked', true);
        } else {
            $('.tblInvoiceDeatil .chk-print-invoice').prop('checked', false);
        }
    };

    $scope.OpenInvMessagePopup = function () {
        angular.element("#invMsgModel").modal('show');
    };

    $scope.GetInvMsg = function () {
        var promise = BillingService.GetInvMsg();
        promise.success(function (response) {
            if (response.Success) {
                $scope.InvMsgList = response.Data;
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
    };
    $scope.RecordTypeChange = function (InvHdr) {
        if (InvHdr) {
            $scope.objEditInvoice.InvHdrdesc = InvHdr;
        }
    }
    $scope.ViewARInvoiceChangeLogByInvoiceId = function (ArID, InvNo) {
        var ViewARInvoiceChangeLog = AccountReceivableService.GetARInvoiceChangeLogByInvoiceId(ArID, InvNo);
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
                    "title": "Description",
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

                {
                    "title": "Status",
                    "className": "dt-left",
                    "data": "Description",
                    "sorting": "false"
                },
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

    function init() {
        $scope.GetInvMsg();
    }

    init();

    function OpenInvoiceReportViewer(InvoiceNO) { 
        var src = '/Reports/InvoiceBatchReport.aspx'; 
        src = src + "?OnlyFilterByInvoice=true&InvoiceNO=" + InvoiceNO; 
        window.open(src, '_blank');
    }

});