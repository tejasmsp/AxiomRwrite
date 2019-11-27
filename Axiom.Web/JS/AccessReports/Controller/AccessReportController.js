app.controller('AccessReportController', function ($scope, $rootScope, $stateParams, notificationFactory, AccessReportService, configurationService, CommonServices, $compile, $filter) {
    decodeParams($stateParams);
    $scope.ReportName;
    // $scope.showFromToDate = false;
    $scope.ShowFirm = false;
    $scope.showFromToDate = ($stateParams.type == 'PartsByDate' || $stateParams.type == 'InvoiceByDate' || $stateParams.type == 'ChecksByDate' || $stateParams.type == 'HanoverBilling' || $stateParams.type == 'HanoverBillingFees' || $stateParams.type == 'GrangeBilling' || $stateParams.type == 'GroverBilling' || $stateParams.type == 'AgedAR');
    $scope.showCompany = ($stateParams.type == 'PartsByDate' || $stateParams.type == 'InvoiceByDate' || $stateParams.type == 'ChecksByDate' || $stateParams.type == 'HanoverBilling' || $stateParams.type == 'HanoverBillingFees' || $stateParams.type == 'OrderBySSN' || $stateParams.type == 'NonInvoicedParts' || $stateParams.type == 'GrangeBilling' || $stateParams.type == 'GroverBilling' || $stateParams.type == 'AgedAR');
    $scope.ShowFirm = ($stateParams.type == 'InvoiceByDate' || $stateParams.type == 'HanoverBilling' || $stateParams.type == 'HanoverBillingFees' || $stateParams.type == 'GrangeBilling' || $stateParams.type == 'GroverBilling' || $stateParams.type == 'AgedAR');
    $scope.showCheckNumber = $stateParams.type == 'ChecksByNumber';
    $scope.showSSN = $stateParams.type == 'OrderBySSN';
    $scope.showReportList = [];
    $scope.showReportListAll = [];
    $scope.firmDropdownList = [];
    $scope.showDownloadButton = false;
    $scope.TotalParts = 0;
    $scope.BillAmount = 0;
    $scope.BillBalance = 0;
    $scope.showPartCount = false; $stateParams.type == 'PartsByDate';
    $scope.showSpecial = $stateParams.type == 'AgedAR';
    $scope.UserGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.Days = -1;
    $scope.ShowFilterButton = false;
    $scope.TotalInvoices = 0;
    $scope.showInvoiceCount = false;
    $scope.thirtyTotal = 0;
    $scope.sixtyTotal = 0;
    $scope.nintyTotal = 0;
    $scope.nintyplusTotal = 0;
    $scope.agedarTotal = 0;
    $scope.ShowSummarybutton = false;


    function AgedARBalance() {
        $scope.thirtyTotal = 0;
        $scope.sixtyTotal = 0;
        $scope.nintyTotal = 0;
        $scope.nintyplusTotal = 0;
        $scope.agedarTotal = 0;
        if ($stateParams.type == 'AgedAR') {
            $scope.showReportListAll.filter(function (i, e) {
                $scope.agedarTotal += i.Balance;
                if (i.Days <= 30) {
                    $scope.thirtyTotal = $scope.thirtyTotal + i.Balance;
                }
                else if (i.Days > 30 && i.Days <= 60) {
                    $scope.sixtyTotal = $scope.sixtyTotal + i.Balance;
                }
                else if (i.Days > 60 && i.Days <= 90) {
                    $scope.nintyTotal = $scope.nintyTotal + i.Balance;
                }
                else if (i.Days > 90) {
                    $scope.nintyplusTotal = $scope.nintyplusTotal + i.Balance;
                }
            });
        }
    };

    function showReport() {
        debugger;
        $scope.TotalParts = 0;
        $scope.BillAmount = 0;
        $scope.BillBalance = 0;
        $scope.showPartCount = $stateParams.type == 'PartsByDate';
        $scope.showAmount = ($stateParams.type == 'InvoiceByDate' || $stateParams.type == 'HanoverBilling' || $stateParams.type == 'HanoverBillingFees' || $stateParams.type == 'GrangeBilling' || $stateParams.type == 'GroverBilling');
        $scope.showBalance = ($stateParams.type == 'InvoiceByDate' || $stateParams.type == 'GrangeBilling' || $stateParams.type == 'GroverBilling');
        $scope.showInvoiceCount = $stateParams.type == 'InvoiceByDate';
        $scope.ShowFilterButton = $stateParams.type == 'AgedAR';
        $scope.ShowSummarybutton = $stateParams.type == 'AgedAR';
        if ($.fn.DataTable.isDataTable("#tblReports")) {
            // $('#tblReports').DataTable().fndestroy();
        }


        var columns = []; //["Date", "CountOfPartNo", "CompanyNo", "CompName"];

        if ($stateParams.type == 'PartsByDate') {
            angular.forEach($scope.showReportList, function (value, key) {
                $scope.TotalParts += value.CountOfPartNo;
            });
        }
        if ($stateParams.type == 'InvoiceByDate') {
            $scope.TotalInvoices = $scope.showReportList.length;
        }
        
        if ($stateParams.type == 'GrangeBilling' || $stateParams.type == 'GroverBilling' || $stateParams.type == 'InvoiceByDate') {
            angular.forEach($scope.showReportList, function (value, key) {
                $scope.BillBalance += value.Balance;
            });
        }
        if ($stateParams.type == 'InvoiceByDate' || $stateParams.type == 'HanoverBilling' || $stateParams.type == 'HanoverBillingFees' || $stateParams.type == 'GrangeBilling' || $stateParams.type == 'GroverBilling'); {
            angular.forEach($scope.showReportList, function (value, key) {
                $scope.BillAmount += value.InvoiceAmount;
            });
        }
        if ($stateParams.type == 'AgedAR') {
            if ($scope.Days == 30) {
                $scope.showReportList = $scope.showReportListAll.filter(function (i, e) {                    
                    if (i.Days <= 30) {                        
                        return i.Days;
                    }
                });
            }
            else if ($scope.Days == 60) {
                //$scope.showReportList = $scope.showReportListAll.filter(s => s.Days > 30 && s.Days <= 60);
                $scope.showReportList = $scope.showReportListAll.filter(function (i, e) {
                    if (i.Days > 30 && i.Days <=60) {
                        return i.Days;
                    }
                });
            }
            else if ($scope.Days == 90) {
                // $scope.showReportList = $scope.showReportListAll.filter(s => s.Days > 60 && s.Days <= 90);
                $scope.showReportList = $scope.showReportListAll.filter(function (i, e) {
                    if (i.Days > 60 && i.Days <= 90) {
                        return i.Days;
                    }
                });
            }
            else if ($scope.Days == 0) {
                // $scope.showReportList = $scope.showReportListAll.filter(s => s.Days > 90);
                $scope.showReportList = $scope.showReportListAll.filter(function (i, e) {
                    if (i.Days > 90) {
                        return i.Days;
                    }
                });
            }
            else if ($scope.Days == -1) {
                $scope.showReportList = $scope.showReportListAll;
            }
        };


        var datamodel = new Object();
        if ($scope.showReportList != null && $scope.showReportList.length > 0) {
            datamodel = $scope.showReportList[0];
            Object.keys(datamodel).forEach(function (item, i) {
                columns.push({
                    "title": AddSpace(item),
                    "className": "dt-left",
                    "data": item
                });
            });
        }

        if ($scope.showReportList != null && $scope.showReportList.length > 0) {
            // $('#tblReports').DataTable().destroy();
        }

        $('#tblReports tbody').empty();
        if ($scope.showReportList != null && $scope.showReportList.length > 0) {
            var table = $('#tblReports').DataTable({
                data: $scope.showReportList,
                "bDestroy": true,
                "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
                "aLengthMenu": [10, 20, 50, 100, 200],
                "pageLength": 10,
                "stateSave": false,
                "columns": columns,
                "aaSorting": [[0, 'desc']],
                "initComplete": function () {
                    var dataTable = $('#tblReports').DataTable();
                },
                "fnDrawCallback": function () {
                },
                "fnCreatedRow": function (nRow, aData, iDataIndex) {
                    $compile(angular.element(nRow).contents())($scope);
                }
            });
        }

    };

    function BindCompanyDetail() {
        var companydropdownlist = CommonServices.GetCompanyDropDown();
        companydropdownlist.success(function (response) {
            $scope.companydropdownlist = response.Data;
        });

        companydropdownlist.error(function (response) {
            toastr.error(response.Message[0]);
        });
    };

    $scope.BindFirmDropDown = function () {

        // var firmDropdownList = CommonServices.FirmForDropdown("");
        var firmDropdownList = CommonServices.GetFirmByUserId($scope.UserGuid, $rootScope.CompanyNo);
        firmDropdownList.success(function (response) {

            $scope.firmList = response.Data;
            $('.cls-firm1').selectpicker();
            $('.cls-firm1').selectpicker('refresh');


            var hanoverText = $scope.firmList.filter(function (i, e) {
                if (i.FirmID == 'HANOAA01') {
                    return i.FirmName;
                }
            });

            if ($stateParams.type == 'HanoverBilling' || $stateParams.type == 'HanoverBillingFees') {                
                $scope.AccessReport.FirmID = 'HANOAA01';
                $('.filter-option-inner-inner').text(hanoverText[0].FirmName);
            }

        });

        firmDropdownList.error(function (response) {
            toastr.error(response.Message[0]);
        });
    };

    $scope.FilterRecord = function (days) {
        $scope.Days = days;
        showReport();
    };

    $scope.DisplayReport = function () {
        var showReportList = AccessReportService.DisplayAccessReportPartsByDate($stateParams.type, $scope.AccessReport.StartDate, $scope.AccessReport.EndDate, $rootScope.CompanyNo, $scope.AccessReport.CheckNumber, $scope.AccessReport.SSNNumber, $scope.AccessReport.FirmID);
        showReportList.success(function (response) {
            $scope.showReportList = response.Data;
            $scope.showReportListAll = response.Data;
            $scope.showDownloadButton = true;
            showReport();
            AgedARBalance();
        });
        showReportList.error(function (response) {
            toastr.error(response.Message[0]);
        });
    };

    $scope.DownloadSummaryReport = function () {
        var reportFile = AccessReportService.DownloadAccessReportAgedARSummary($scope.AccessReport.StartDate, $scope.AccessReport.EndDate, $scope.AccessReport.CompanyNo, $scope.AccessReport.FirmID);
    }

    $scope.DownloadReport = function () {
        if ($stateParams.type == 'AgedAR') {
            var reportFile = AccessReportService.DownloadAccessReportAgedAR($scope.AccessReport.StartDate, $scope.AccessReport.EndDate, $scope.AccessReport.CompanyNo, $scope.AccessReport.FirmID);
        }
        else {
            var reportFile = AccessReportService.DownloadAccessReport($stateParams.type, $scope.AccessReport.StartDate, $scope.AccessReport.EndDate, $scope.AccessReport.CompanyNo, $scope.AccessReport.CheckNumber, $scope.AccessReport.SSNNumber, $scope.AccessReport.FirmID);
        }
    };
    function init() {
        $scope.AccessReport = new Object();
        var date = new Date();
        var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
        var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);

        $scope.ReportName = "-" + AddSpace($stateParams.type);
        createDatePicker();
        BindCompanyDetail();

        if ($scope.ShowFirm)
            $scope.BindFirmDropDown();

        $scope.AccessReport.StartDate = $filter('date')(firstDay, 'MM/dd/yyyy');
        $scope.AccessReport.EndDate = $filter('date')(lastDay, 'MM/dd/yyyy');
    };
    init();
});