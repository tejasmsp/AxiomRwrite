app.controller('InvoiceBatchController', function ($scope, $rootScope, $stateParams, notificationFactory, InvoiceBatchService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.UserGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.showLEDButton = false;
    createDatePicker();
    // $scope.InvoiceBatch = new Object()

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Invoice Batch", "View");
    //-----

    function init() {

        $scope.InvoiceBatch = new Object();

        // $('.cls-firm').selectpicker();
        $scope.BindFirmDropDown();
        // $('.cls-firm').selectpicker('refresh');




        //$scope.BindDropDownSoldAttorney('');
        $scope.InvoiceBatch.Invoice = false;
        $scope.InvoiceBatch.Statement = true;
        $scope.InvoiceBatch.OpenInvoiceOnly = false;
        $scope.InvoiceBatch.IsFirmID = false;
        $scope.InvoiceBatch.IsAttyID = false;
        $scope.InvoiceBatch.FromDate = $filter('date')(new Date(), $rootScope.GlobalDateFormat);
        $scope.InvoiceBatch.ToDate = $filter('date')(new Date(), $rootScope.GlobalDateFormat);


    }


    $scope.BindAttorneyByFirmDropdown = function (FirmID) {

        $('.cls-attorney1').selectpicker();
        if (FirmID != 'undefined') {
            var attry = CommonServices.GetAttorneyByFirmID(FirmID);
            attry.success(function (response) {
                $scope.AttorneyList = response.Data;
            });
        }
        $('.cls-attorney1').selectpicker('refresh');
        $scope.InvoiceBatchCheckLED();
    };

    $("#txtSoldAttorney").keyup(function () {
        if ($(this).val().length > 2) {
            $scope.BindDropDownSoldAttorney($(this).val());
        }
    });


    $scope.BindDropDownSoldAttorney = function (SearchKey) {
        var Attorney = CommonServices.AttorneyForDropdown(SearchKey, $rootScope.CompanyNo);
        Attorney.success(function (response) {
            $scope.SoldAttorneylist = response.Data;
        });
    };


    $scope.BindFirmDropDown = function () {

        var _firm = CommonServices.GetFirmByUserId($scope.UserGuid, $rootScope.CompanyNo);
        _firm.success(function (response) {
            $scope.FirmList = response.Data;
            angular.forEach($scope.FirmList, function (value, key) {
                value.FirmName = value.FirmName + " (" + value.FirmID + ")";
            });

            setTimeout(function () {
                $('.cls-firm1').selectpicker();
                $('.cls-firm1').selectpicker('refresh');
            }, 1000);
        });
        _firm.error(function (data, statusCode) {
        });

    };

    $scope.Search = function () {
        BindInvoiceBatchReportViewer();
    };


    $scope.ResetSearch = function () {
        $scope.InvoiceBatch.FirmID = "";
        $scope.InvoiceBatch.Caption = "";
        $scope.InvoiceBatch.ClaimNo = "";
        $scope.InvoiceBatch.InvoiceNO = "";
        $scope.InvoiceBatch.AttyID = "";
        $scope.InvoiceBatch.SoldAttyID = "";
        $scope.InvoiceBatch.FromDate = "";
        $scope.InvoiceBatch.ToDate = "";
        $scope.InvoiceBatch.Invoice = false;
        $scope.InvoiceBatch.Statement = false;
        $scope.InvoiceBatch.OpenInvoiceOnly = false;
        $scope.InvoiceBatch.IsFirmID = false;
        $scope.InvoiceBatch.IsAttyID = false;
        $scope.showLEDButton = false;
        $scope.AttorneyList = null;
        $scope.FirmList = null;
    };

    $scope.IsFirmIDChange = function () {
        $scope.InvoiceBatchCheckLED();
    }

    $scope.InvoiceBatchCheckLED = function () {
        if ($scope.InvoiceBatch.IsFirmID && $scope.InvoiceBatch.FirmID != '') {
            var promise = InvoiceBatchService.InvoiceBatchCheckLED($scope.InvoiceBatch.FirmID);
            promise.success(function (response) {
                debugger;
                $scope.showLEDButton = response.str_ResponseData;
            });
        }
        else {
            $scope.showLEDButton = false;
        }
    }
    $scope.GenerateLEDESFile = function () {
        debugger;
        if ($scope.InvoiceBatch.IsFirmID) {
            $scope.InvoiceBatch.FirmID = $scope.InvoiceBatch.FirmID;
        }
        if ($scope.InvoiceBatch.IsAttyID) {
            $scope.InvoiceBatch.AttyID = $scope.InvoiceBatch.AttyID;
        }

        $scope.InvoiceBatch.SoldAttyName = $scope.soldAttorney;

        if (isNullOrUndefinedOrEmpty($scope.soldAttorney)) {
            $scope.InvoiceBatch.SoldAttyName = "";
        }

        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.FirmID)) {
            $scope.InvoiceBatch.FirmID = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.Caption)) {
            $scope.InvoiceBatch.Caption = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.ClaimNo)) {
            $scope.InvoiceBatch.ClaimNo = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.InvoiceNO)) {
            $scope.InvoiceBatch.InvoiceNO = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.AttyID)) {
            $scope.InvoiceBatch.AttyID = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.SoldAttyName)) {
            $scope.InvoiceBatch.SoldAttyName = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.FromDate)) {
            $scope.InvoiceBatch.FromDate = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.ToDate)) {
            $scope.InvoiceBatch.ToDate = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.Invoice)) {
            $scope.InvoiceBatch.Invoice = false;
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.Statement)) {
            $scope.InvoiceBatch.Statement = false;
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.OpenInvoiceOnly)) {
            $scope.InvoiceBatch.OpenInvoiceOnly = false;
        }
        // var _led = InvoiceBatchService.GenerateLEDESFile($scope.InvoiceBatch.FirmID, $scope.InvoiceBatch.Caption, $scope.InvoiceBatch.ClaimNo, $scope.InvoiceBatch.AttyID, $scope.InvoiceBatch.FromDate, $scope.InvoiceBatch.ToDate, $scope.InvoiceBatch.InvoiceNO, $scope.InvoiceBatch.SoldAttyName);        

        debugger;
        // TO DOWNLOAD CSF FILE
        var _led = InvoiceBatchService.GenerateLEDESFile($scope.InvoiceBatch.FirmID, $scope.InvoiceBatch.Caption, $scope.InvoiceBatch.ClaimNo, $scope.InvoiceBatch.AttyID, $scope.InvoiceBatch.FromDate, $scope.InvoiceBatch.ToDate, $scope.InvoiceBatch.InvoiceNO, $scope.InvoiceBatch.SoldAttyName);
        _led.success(function (response) {
            debugger;
            var file = new Blob([response], { type: 'text/plain' });

            // var isChrome = !!window.chrome && !!window.chrome.webstore;
            var isChrome = /chrom(e|ium)/.test(navigator.userAgent.toLowerCase());
            var isIE = /*@cc_on!@*/false || !!document.documentMode;
            var isEdge = !isIE && !!window.StyleMedia;


            if (isChrome) {

                var url = window.URL || window.webkitURL;
                $scope.date = new Date();
                var dt = $filter('date')($scope.date, "yyyy-MM-ddHHmmss")

                var downloadLink = angular.element('<a></a>');
                downloadLink.attr('href', url.createObjectURL(file));
                downloadLink.attr('target', '_self');
                downloadLink.attr('download', dt + ".txt");
                downloadLink[0].click();
            }
            else if (isEdge || isIE) {
                debugger;
                $scope.date = new Date();
                var dt = $filter('date')($scope.date, "yyyy-MM-ddHHmmss");
                window.navigator.msSaveOrOpenBlob(file, dt + ".csv");

            }
            else {

            }
        });
    };


    function BindInvoiceBatchReportViewer() {
        debugger;
        if ($scope.InvoiceBatch.IsFirmID) {
            $scope.InvoiceBatch.FirmID = $scope.InvoiceBatch.FirmID;
        }
        if ($scope.InvoiceBatch.IsAttyID) {
            $scope.InvoiceBatch.AttyID = $scope.InvoiceBatch.AttyID;
        }

        $scope.InvoiceBatch.SoldAttyName = $scope.soldAttorney;

        if (isNullOrUndefinedOrEmpty($scope.soldAttorney)) {
            $scope.InvoiceBatch.SoldAttyName = "";
        }

        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.FirmID)) {
            $scope.InvoiceBatch.FirmID = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.Caption)) {
            $scope.InvoiceBatch.Caption = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.ClaimNo)) {
            $scope.InvoiceBatch.ClaimNo = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.InvoiceNO)) {
            $scope.InvoiceBatch.InvoiceNO = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.AttyID)) {
            $scope.InvoiceBatch.AttyID = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.SoldAttyName)) {
            $scope.InvoiceBatch.SoldAttyName = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.FromDate)) {
            $scope.InvoiceBatch.FromDate = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.ToDate)) {
            $scope.InvoiceBatch.ToDate = "";
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.Invoice)) {
            $scope.InvoiceBatch.Invoice = false;
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.Statement)) {
            $scope.InvoiceBatch.Statement = false;
        }
        if (isNullOrUndefinedOrEmpty($scope.InvoiceBatch.OpenInvoiceOnly)) {
            $scope.InvoiceBatch.OpenInvoiceOnly = false;
        }



        var src = '/Reports/InvoiceBatchReport.aspx';
        src = src + "?FirmID=" + $scope.InvoiceBatch.FirmID;
        src = src + "&Caption=" + $scope.InvoiceBatch.Caption;
        src = src + "&ClaimNo=" + $scope.InvoiceBatch.ClaimNo;
        src = src + "&InvoiceNO=" + $scope.InvoiceBatch.InvoiceNO;
        src = src + "&AttyID=" + $scope.InvoiceBatch.AttyID;
        src = src + "&SoldAttyID=" + $scope.InvoiceBatch.SoldAttyID;
        src = src + "&FromDate=" + $scope.InvoiceBatch.FromDate;
        src = src + "&ToDate=" + $scope.InvoiceBatch.ToDate;
        src = src + "&Invoice=" + $scope.InvoiceBatch.Invoice;
        src = src + "&Statement=" + $scope.InvoiceBatch.Statement;
        src = src + "&OpenInvoiceOnly=" + $scope.InvoiceBatch.OpenInvoiceOnly;
        var iframeheight = "880px";
        if ($scope.InvoiceBatch.Invoice && $scope.InvoiceBatch.Statement) {
            iframeheight = "880px";
        }

        $('.loader').show();

        var iframe = '<iframe id="reportFrame" width="100%" onload="InvoiceBatchIframeLoaded()" height="' + iframeheight + '" scrolling="no" frameborder="0" src="' + src + '" allowfullscreen></iframe>';
        $("#divInvoiceBatchReport").html(iframe);
    }

    init();
});