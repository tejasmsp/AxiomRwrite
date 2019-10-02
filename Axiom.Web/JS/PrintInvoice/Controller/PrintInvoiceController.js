app.controller('PrintInvoiceController', function ($scope, $rootScope, $stateParams, notificationFactory, PrintInvoiceService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $rootScope.pageTitle = "Print Invoice";

    $scope.DownloadInvoice = function () {

        //var strHTML = String(angular.element("#invoiceTable").html()).replace(/[&<>"'`=\/]/g, function (s) {
        //    return entityMap[s];
        //});
        $scope.PostData = new Object();
        // var strHTML = "tejas j;alksdf;aksdf; lakdsf;lakdsf;adslkf";
        var strHTML = angular.element("#invoiceTable").html();
        $scope.PostData.htmlString = strHTML;
        
        var invoice = PrintInvoiceService.DownloadInvoice($scope.PostData);
    };
    
    $scope.PrintInvoice = function () {
        var promise = PrintInvoiceService.PrintInvoice($scope.InvoiceID, $scope.OrderId, $scope.PartNo, $scope.PrintAll);
        promise.success(function (response) {
            if (response.Success) {
                promise.success(function (response) {                    
                    if (!isNullOrUndefinedOrEmpty(response.Data)) {
                        $scope.InvoiceDetailList = [];
                        $scope.InvoiceDetailList = response.Data;
                        for (var i = 0; i < $scope.InvoiceDetailList.length; i++) {
                           GetInvoiceFeesDetail($scope.InvoiceDetailList[i],i);                            
                        }
                    }                    
                  
                    //$scope.InvoiceDetail = new Object();
                    //$scope.InvoiceDetail.MiscChrg = response.Data[0].MiscChrg;
                    //$scope.InvoiceDetail.CompName = response.Data[0].CompName;
                    //$scope.InvoiceDetail.TaxID = response.Data[0].TaxID;
                    //$scope.InvoiceDetail.CompStreet = response.Data[0].CompStreet;
                    //$scope.InvoiceDetail.CompCity = response.Data[0].CompCity;
                    //$scope.InvoiceDetail.CompState = response.Data[0].CompState;
                    //$scope.InvoiceDetail.CompZip = response.Data[0].CompZip;
                    //$scope.InvoiceDetail.CompanyPhoneNumber = response.Data[0].CompanyPhoneNumber;
                    //$scope.InvoiceDetail.CompanyFaxNumber = response.Data[0].CompanyFaxNumber;
                    //$scope.InvoiceDetail.RemitNo = response.Data[0].RemitNo;
                    //$scope.InvoiceDetail.BillPhoneNumber = response.Data[0].BillPhoneNumber;
                    //$scope.InvoiceDetail.BillFaxNumber = response.Data[0].BillFaxNumber;
                    //$scope.InvoiceDetail.BillFirmName = response.Data[0].BillFirmName;
                    //$scope.InvoiceDetail.BillAttorneyName = response.Data[0].BillAttorneyName;
                    //$scope.InvoiceDetail.BillAddress = response.Data[0].BillAddress;
                    //$scope.InvoiceDetail.BillCity = response.Data[0].BillCity;
                    //$scope.InvoiceDetail.BillState = response.Data[0].BillState;
                    //$scope.InvoiceDetail.BillZip = response.Data[0].BillZip;
                    //$scope.InvoiceDetail.SoldPhoneNumber = response.Data[0].SoldPhoneNumber;
                    //$scope.InvoiceDetail.SoldFaxNumber = response.Data[0].SoldFaxNumber;
                    //$scope.InvoiceDetail.SoldFirmName = response.Data[0].SoldFirmName;
                    //$scope.InvoiceDetail.SoldAttorneyName = response.Data[0].SoldAttorneyName;
                    //$scope.InvoiceDetail.SoldAddress = response.Data[0].SoldAddress;
                    //$scope.InvoiceDetail.SoldCity = response.Data[0].SoldCity;
                    //$scope.InvoiceDetail.SoldState = response.Data[0].SoldState;
                    //$scope.InvoiceDetail.SoldZip = response.Data[0].SoldZip;
                    //$scope.InvoiceDetail.InvNo = response.Data[0].InvNo;
                    //$scope.InvoiceDetail.InvDate = response.Data[0].InvDate;
                    //$scope.InvoiceDetail.InvAmt = response.Data[0].InvAmt;
                    //$scope.InvoiceDetail.Message = response.Data[0].Message;
                    //$scope.InvoiceDetail.OrderNumber = response.Data[0].OrderNumber;
                    //$scope.InvoiceDetail.OrderDate = response.Data[0].OrderDate;
                    //$scope.InvoiceDetail.CauseNo = response.Data[0].CauseNo;
                    //$scope.InvoiceDetail.Caption = response.Data[0].Caption;
                    //$scope.InvoiceDetail.Name = response.Data[0].Name;
                    //$scope.InvoiceDetail.SSN = response.Data[0].SSN;
                    //$scope.InvoiceDetail.DateOfBirth = response.Data[0].DateOfBirth;
                    //$scope.InvoiceDetail.OrderLocationName = response.Data[0].OrderLocationName;
                    //$scope.InvoiceDetail.Dept = response.Data[0].Dept;
                    //$scope.InvoiceDetail.OrderLocationAddress = response.Data[0].OrderLocationAddress;
                    //$scope.InvoiceDetail.OrderLocationCity = response.Data[0].OrderLocationCity;
                    //$scope.InvoiceDetail.OrderLocationState = response.Data[0].OrderLocationState;
                    //$scope.InvoiceDetail.OrderLocationZip = response.Data[0].OrderLocationZip;
                    //$scope.InvoiceDetail.InvHdr = response.Data[0].InvHdr;
                    //$scope.InvoiceDetail.Pages = response.Data[0].Pages;
                    //$scope.InvoiceDetail.Original = response.Data[0].Original;
                    //$scope.InvoiceDetail.Copies = response.Data[0].Copies;
                    //$scope.InvoiceDetail.CopyRate = response.Data[0].CopyRate;
                    //$scope.InvoiceDetail.StdFee1 = response.Data[0].StdFee1;
                    //$scope.InvoiceDetail.StdFee2 = response.Data[0].StdFee2;
                    //$scope.InvoiceDetail.StdFee3 = response.Data[0].StdFee3;
                    //$scope.InvoiceDetail.StdFee4 = response.Data[0].StdFee4;
                    //$scope.InvoiceDetail.StdFee5 = response.Data[0].StdFee5;
                    //$scope.InvoiceDetail.StdFee6 = response.Data[0].StdFee6;
                    //$scope.InvoiceDetail.StdFee7 = response.Data[0].StdFee7;
                    //$scope.InvoiceDetail.StdFee8 = response.Data[0].StdFee8;
                    //$scope.InvoiceDetail.OrigRate = response.Data[0].OrigRate;


                });
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    };


    function GetInvoiceFeesDetail(invoiceData,index) {        
        var prom = PrintInvoiceService.PrintInvoiceDetail(invoiceData.InvNo);
        prom.success(function (response) {
            if (response.Success) {                
                $scope.InvoiceFeesDetail = response.Data;
                $scope.InvoiceFeesHeaderRow = [];
                for (var i = 0; i < $scope.InvoiceFeesDetail.length; i++) {
                    var re = $filter('filter')($scope.InvoiceFeesHeaderRow, { ItemNo: $scope.InvoiceFeesDetail[i].ItemNo });
                    if (re.length == 0) {
                        $scope.InvoiceFeesHeaderRow.push({
                            ItemNo: $scope.InvoiceFeesDetail[i].ItemNo,
                            InvHdr: $scope.InvoiceFeesDetail[i].InvHdr,
                            CopyRate: $scope.InvoiceFeesDetail[i].CopyRate,
                            OrigRate: $scope.InvoiceFeesDetail[i].OrigRate,
                            Pages: $scope.InvoiceFeesDetail[i].Pages,
                            FeeValue: $scope.InvoiceFeesDetail[i].FeeValue
                        });
                    }
                }                
                $scope.InvoiceDetailList[index].InvoiceFeesHeaderRow = $scope.InvoiceFeesHeaderRow;
                $scope.InvoiceDetailList[index].InvoiceFeesDetail = $scope.InvoiceFeesDetail;                
            }
            else {
                toastr.error(response.Message[0]);
            }

        });
        prom.error(function (data, statusCode) {
        });
    }

    function init() {        
        $scope.InvoiceID = $stateParams.InvoiceID;
        if (isNullOrUndefinedOrEmpty($scope.InvoiceID)) {
            $scope.InvoiceID = "";
        }
        $scope.OrderId = $stateParams.OrderId;
        $scope.PartNo = $stateParams.PartNo;
        $scope.PrintAll = $stateParams.IsPrintAll;
        if (isNullOrUndefinedOrEmpty($scope.PrintAll)) {
            $scope.PrintAll = false;
        }        
        $scope.PrintInvoice();
    }

    init();
});