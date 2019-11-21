app.controller('IIFFilesController', function ($scope, $rootScope, $stateParams, notificationFactory, IIFFilesService, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);

    $scope.CheckListArray = [];
    $scope.showCheckResult = false;
    $scope.objIIFFile = new Object();
    $scope.objIIFFile.ToBePrint = false;
    $scope.objCheckDetail = new Object();
    $scope.objCheckDetail.ToBePrint = false;


    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "IIF Files", "View");    
    //------------

    function bindCheckList() {
        
        if ($.fn.DataTable.isDataTable("#tbliifCheckList")) {
            $('#tbliifCheckList').DataTable().clear();
            $('#tbliifCheckList').DataTable().destroy();
        }

        if ($.fn.DataTable.isDataTable("#tbliifCheckList")) {
            $('#tbliifCheckList').DataTable().destroy();
        }

        var pagesizeObj = 10;

        $('#tbliifCheckList').DataTable({
            stateSave: false,
            "oLanguage": {
                "sProcessing": '',
                "sZeroRecords": "<span class='pull-left'>No records found</span>",
            },
            "searching": false,
            "dom": '<"table-responsive"frt><"bottom"lip<"clear">>',
            "bProcessing": true,
            "bServerSide": true,
            "iDisplayStart": 0,
            "iDisplayLength": pagesizeObj,
            "lengthMenu": [05, 10, 20, 30, 40, 50],
            "sAjaxDataProp": "aaData",
            "aaSorting": [[2, 'desc']],
            "sAjaxSource": configurationService.basePath + "/IIFGenerateCheckList",
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                aoData = BindSearchCriteriaCommon(aoData);
                aoData = BindSortingCommon(aoData, oSettings);
                if ($scope.SearchValue == undefined) {
                    $scope.SearchValue = "";
                }
                //aoData.SearchValue = $scope.SearchValue; 
                //aoData.PageIndex = parseInt($('#tblFirm').DataTable().page.info().page) + 1;


                $scope.GridParams = aoData;
                oSettings.jqXHR = $.ajax({
                    'dataSrc': 'aaData',
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource + "?PageIndex=" + (parseInt($('#tbliifCheckList').DataTable().page.info().page) + 1)
                        + "&FromDate=" + $scope.objCheckDetail.FromDate
                        + "&ToDate=" + $scope.objCheckDetail.ToDate + "&ToBePrint=" + $scope.objCheckDetail.ToBePrint + "&CompanyNo=" + $rootScope.CompanyNo,
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });
            },
            "columns": [
                {
                    "title": 'ALL',
                    "data": "",
                    "className": "dt-center btn-custom-link",
                    "sorting": "false",
                    'searchable': false,
                    'orderable': false,
                    "width": "2%",
                    "render": function (data, type, row) {
                        if ($scope.CheckListArray.indexOf(row.CheckID.toString()) > -1) {
                            return '<input type="checkbox" rel="iifCheckBox" ng-click="PushToArray($event,' + "'" + row.CheckID + "'" +'")" checked />';
                        }
                        else {
                            return '<input type="checkbox" rel="iifCheckBox" value="' + row.CheckID + '" ng-click="PushToArray($event,' + "'" + row.CheckID +"'" + ')" />';
                        }


                        //if (data.CheckID) {
                        //    return '<input type="checkbox" ng-click="PushToArray($event,' + data.CheckID +')" checked />';
                        //}
                        //else {
                        //    return '<input type="checkbox"  />';
                        //}
                    }
                },
                { "title": "To", "data": "FirmName", "className": "dt-left", "width": "60%" },
                { "data": "IssueDate", "title": "Issued", "className": "dt-left" },
                { "data": "CheckNo", "title": "Check#", "className": "dt-left" },
                {
                    "data": "Amount", "title": "Amount", "className": "dt-left",
                    "render": function (data, type, row) {
                        return '$' + data;
                    }

                },
                { "data": "OrderNo", "title": "OrderNo", "className": "dt-left" }
                //// { "data": "State", "title": "State", "className": "dt-left" },                
                //{
                //    "title": 'Status',
                //    "data": "IsAssociated",
                //    "sClass": "action dt-center",
                //    "orderable": true,
                //    "width": "16%",
                //    "render": function (data, type, row) {
                //        if (data == 1) {
                //            return '<label class="label bg-success-400">Associated</label>';
                //        }
                //        else {
                //            return '<label class="label bg-danger-400">No</label>';
                //        }
                //    }
                //}

            ],
            "fnInitComplete": function () {
                $scope.DefaultInitialization();
            },
            "initComplete": function () {
                var dataTable = $('#tbliifCheckList').DataTable();
                // BindCustomerSearchBar($scope, $compile, dataTable);
            },
            "fnDrawCallback": function () {
                //BindToolTip();
                // setTimeout(function () { SetAnchorLinks(); }, 500);
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }
    
    $scope.DefaultInitialization = function() {
        $('#tbliifCheckList').on('click', 'th', function () {
            debugger;            
            if ($(this).text() == "ALL") {

                $('input[rel=iifCheckBox]').each(function () {
                    debugger;
                    this.checked = true;
                    $scope.CheckListArray.push($(this).val());
                });
                $(".table-grid th.btn-custom-link").text("NONE");
                
                //$scope.RushAllStatus = true;
                //$scope.AllRush();
            }
            else if ($(this).text() == "NONE") {
                $('input[rel=iifCheckBox]').each(function () {
                    this.checked = false;
                    for (var i = 0; i <= $scope.CheckListArray.length - 1; i++) {
                        if ($scope.CheckListArray[i] === $(this).val()) {
                            $scope.CheckListArray.splice(i, 1);
                            break;
                        }
                    }
                });
                $(".table-grid th.btn-custom-link").text("ALL");
            }
            
        });
    }

    $scope.PushToArray = function ($event, CheckID) {
        if ($($event.target).parent().find("input").prop('checked')) {
            $scope.CheckListArray.push(CheckID);
        }
        else {
            for (var i = 0; i <= $scope.CheckListArray.length -1; i++) {
                if ($scope.CheckListArray[i] === CheckID) {
                    $scope.CheckListArray.splice(i, 1);
                    break;
                }
            }
        }
    }

    $scope.isEdit = false;

    function base64ToArrayBuffer(data) {
        var binaryString = window.atob(data);
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var i = 0; i < binaryLen; i++) {
            var ascii = binaryString.charCodeAt(i);
            bytes[i] = ascii;
        }
        return bytes;
    };

    $scope.PrintCheck = function (form) {
        if (form.$valid) {
            var iif = IIFFilesService.PrintCheckIIFFiles($scope.objCheckDetail.FromDate, $scope.objCheckDetail.ToDate, $scope.CheckListArray.toString(), $scope.objPrinCheck.CheckNo,$rootScope.CompanyNo);
        }
    }
    $scope.SelectCheck = function ($event) {


        if ($($event.target).parent().find("input").prop('checked'))
            $($event.target).parent().find("input").prop('checked', false)
        else
            $($event.target).parent().find("input").prop('checked', true)

    }

    $scope.GetCheckDetails = function (form) {
        if (form.$valid) {
            // angular.element("#modal_checkDetail").modal('show');
            $scope.CheckListArray.length = 0;
            bindCheckList();
            $scope.showCheckResult = true;
            //var promise = IIFFilesService.IIFGenerateCheckList($scope.objCheckDetail.FromDate, $scope.objCheckDetail.ToDate);
            //promise.success(function (response) {

            //    $scope.CheckList = response.Data;

            //});
            //promise.error(function (data, statusCode) {
            //});
        }
    }
    $scope.GetIIFFiles = function (form) {

        if (form.$valid) {
            debugger;
            var iif = IIFFilesService.GetIIFFileForDay($scope.objIIFFile.Date, $scope.objIIFFile.ToBePrint, $rootScope.CompanyNo);
            iif.success(function (response) {

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
                    downloadLink.attr('download', dt + ".iif");
                    downloadLink[0].click();
                }
                else if (isEdge || isIE) {
                    debugger;
                    $scope.date = new Date();
                    var dt = $filter('date')($scope.date, "yyyy-MM-ddHHmmss");
                    window.navigator.msSaveOrOpenBlob(file, dt + ".iif");

                }
                else {

                }
                $scope.GetIIFFilesCSV();
            });
            iif.error(function (data, statusCode) { });



            
         
        }
    }

    $scope.GetIIFFilesCSV = function () {
        debugger;
        // TO DOWNLOAD CSF FILE
        var iifCSV = IIFFilesService.GetIIFFileForDayCSV($scope.objIIFFile.Date, $scope.objIIFFile.ToBePrint,$rootScope.CompanyNo);
        iifCSV.success(function (response) {
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
                downloadLink.attr('download', dt + ".csv");
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
    }

    function init() {
        // GetSSNList();
        createDatePicker();
        // $scope.frmIIFFiles.$setPristine();
        // $scope.frmIIFFilesPrintCheck.$setPristine();
    }
    $scope.createDatePicker = function () {
        createDatePicker();
    }
    function createDatePicker(defautvalue) {
        $('.myDatePicker').bootstrapDP({
            format: 'mm/dd/yyyy',
            autoclose: true,
            "setDate": defautvalue == undefined ? new Date() : ""
        });
        $('#FromDate').bootstrapDP({
            format: 'mm/dd/yyyy',
            autoclose: true,
            "setDate": defautvalue == undefined ? new Date() : ""
        });
        $('#ToDate').bootstrapDP({
            format: 'mm/dd/yyyy',
            autoclose: true,
            "setDate": defautvalue == undefined ? new Date() : ""
        });
    }
    init();
});