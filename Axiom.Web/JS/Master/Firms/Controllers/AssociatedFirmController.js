app.controller('AssociatedFirmController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, FirmServices, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);
    $scope.ParentFirmID;
    $scope.isEdit = false;

    //#region Events 


    $scope.GetAssociatedFirms = function (id) {
        angular.element("#modal_AssociatedFirm").modal('show');

        $scope.$parent.ShowAssociatedFirm = false;
        bindAssociatedFirm();
    };

    $scope.$parent.$watch('ShowAssociatedFirm', function (newVal, oldVal) {
        if (newVal) {
            if ($scope.$parent.FirmObj.FirmID != undefined && $scope.$parent.FirmObj.FirmID != null && $scope.$parent.FirmObj.FirmID) {
                $scope.ParentFirmID = $scope.$parent.FirmObj.FirmID;
                $scope.ParentFirmName = $scope.$parent.FirmObj.FirmName;
                $scope.FirmSearchObj.ParentFirmID = $scope.ParentFirmID;
                $scope.FirmSearchObj.ParentFirmName = $scope.ParentFirmName;
                $scope.GetAssociatedFirms();
            }
        }

    }, true);


    $scope.UpdateDefaultBillFirm = function ($event) {        
        var table = $('#tblAssociatedFirm').DataTable();
        var row = table.row($($event.target).parents('tr')).data();

        var promise = FirmServices.UpdateAssociatedFirmDefaultBill($scope.ParentFirmID, row.FirmID);
        promise.success(function (response) {
            if (response.Success) {
                bindAssociatedFirm();
                notificationFactory.customSuccess('Default Bill Firm has been updated successfully.');
                // updateDefaultBillHtml($event);
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (response) { toastr.error(response.Message[0]); });
    };

    $scope.UpdateAssociatedFirm = function ($event) {

        var table = $('#tblAssociatedFirm').DataTable();
        var row = table.row($($event.target).parents('tr')).data();

        var needtoMerge = $event.target.checked;
        bootbox.confirm({
            message: "Are you sure you want to " + (needtoMerge ? "add" : "remove") + " this firm as Associate Firm ?",
            buttons: {
                cancel: {
                    label: 'No',
                    className: 'btn-danger  '
                },
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                }
            },
            callback: function (result) {
                if (result == true) {
                    if (needtoMerge) {
                        var promise = FirmServices.InsertAssociatedFirm($scope.ParentFirmID, row.FirmID);
                    }
                    else {
                        var promise = FirmServices.DeleteAssociatedFirm($scope.ParentFirmID, row.FirmID);
                    }



                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.customSuccess('Associated Firm has been ' + (needtoMerge ? "added" : "removed") + ' successfully.');
                            // updateMergeStatusHtml($event);
                            bindAssociatedFirm();
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function (response) { toastr.error(response.Message[0]); });
                }
                else {
                    $event.target.checked = !needtoMerge;
                    // updateMergeStatusHtml($event);
                    bindAssociatedFirm();
                }
                bootbox.hideAll();
            }
        });


    };

    $scope.ClearAssociatedFirmSearch_Click = function () {
        clearFirmSearch();
        bindAssociatedFirm()
    };

    $scope.AssociatedFirmSearch_Click = function () {
        bindAssociatedFirm();
    };

    //#endregion 

    //#region Methods  

    function bindDropDown() {
        //StateDropdown
        var _statelist = CommonServices.StateDropdown();
        _statelist.success(function (response) {
            $scope.Statelist = response.Data;
        });
        _statelist.error(function (data, statusCode) { });

        // DepartmentD  
        var _departmentlist = CommonServices.DepartmentDropdown();
        _departmentlist.success(function (response) {
            $scope.Departmentlist = response.Data;
        });
        _departmentlist.error(function (data, statusCode) { });
    }

    function bindAssociatedFirm() {
        if ($.fn.DataTable.isDataTable("#tblAssociatedFirm")) {
            $('#tblAssociatedFirm').DataTable().clear();
            $('#tblAssociatedFirm').DataTable().destroy();
        }

        if ($.fn.DataTable.isDataTable("#tblAssociatedFirm")) {
            $('#tblAssociatedFirm').DataTable().destroy();
        }

        var pagesizeObj = 5;

        $('#tblAssociatedFirm').DataTable({
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
            "aaSorting": [[5, 'desc']],
            "sAjaxSource": configurationService.basePath + "/GetFirmList",
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
                    "url": sSource + "?SearchValue=" + "" + "&PageIndex=" + (parseInt($('#tblAssociatedFirm').DataTable().page.info().page) + 1)
                        + "&FirmID=" + $scope.FirmSearchObj.FirmID
                        + "&FirmName=" + $scope.FirmSearchObj.FirmName
                        + "&Address=" + $scope.FirmSearchObj.Address
                        + "&City=" + $scope.FirmSearchObj.City
                        + "&State=" + $scope.FirmSearchObj.State
                        + "&ParentFirm=" + $scope.ParentFirmID
                        + "&CompanyNo=" + $rootScope.CompanyNo,
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });


            },
            "columns": [
                {
                    "title": '',
                    "sClass": "action dt-center",
                    "orderable": false,
                    "width": "4%",
                    //"data" : "IsAssociated",
                    "render": function (data, type, row) {

                        var strAction = '';
                        if (row.IsAssociated == 1) {
                            strAction = '<input type="checkbox" ng-click="UpdateAssociatedFirm($event)" checked/>';
                            //row.ReplacedBy
                            //strAction = 'Added';
                            //strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="DeleteMergeLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>'
                            //strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="AddMergeLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>'
                        }
                        else {
                            strAction = '<input type="checkbox" ng-click="UpdateAssociatedFirm($event)"/>';
                        }
                        return strAction;
                    }
                },
                { "title": "Code", "data": "FirmID", "className": "dt-left", "width": "10%" },
                { "data": "FirmName", "title": "Name", "className": "dt-left", "width": "25%" },
                { "data": "Address", "title": "Address", "className": "dt-left" },
                { "data": "City", "title": "City", "className": "dt-left" },
                // { "data": "State", "title": "State", "className": "dt-left" },                
                {

                    "title": 'Status',
                    "data": "IsAssociated",
                    "sClass": "action dt-center",
                    "orderable": true,
                    "width": "10%",
                    "render": function (data, type, row) {
                        if (row.IsAssociated == 1) {
                            return '<label class="label bg-success-400" >Associated</label>';
                        }
                        else {
                            return '<label class="label bg-danger-400" >No</label>';
                        }
                    }
                },
                {

                    "title": 'Default Bill',
                    "sClass": "action dt-center",
                    "orderable": true,
                    "width": "4%",
                    //"data" : "IsAssociated",UpdateDefaultBillFirm
                    "render": function (data, type, row) {                        
                        if (row.IsAssociated == 0) {
                            return '<label class="label bg-grey-400 fc-not-allowed" title="">No</label>';
                        }
                        else {
                            if (row.isDefault == 1) {
                                return '<label class="label bg-success-400 cursor-pointer" ng-click="UpdateDefaultBillFirm($event)" title="Click to remove from default bill." >Yes</label>';
                            }
                            else {
                                return '<label class="label bg-danger-400 cursor-pointer" ng-click="UpdateDefaultBillFirm($event)" title="Click to set as defautl bill." >No</label>';
                            }
                        }


                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblAssociatedFirm').DataTable();
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

    function clearFirmSearch() {
        $scope.FirmSearchObj = new Object();
        $scope.FirmSearchObj.FirmID = "";
        $scope.FirmSearchObj.FirmName = "";
        $scope.FirmSearchObj.Address = "";
        $scope.FirmSearchObj.City = "";
        $scope.FirmSearchObj.State = "";
    }


    function updateMergeStatusHtml($event) {        
        var needtoMerge = $event.target.checked;
        $($event.target).parents('tr').find('label.label:first').attr("class", "label " + (needtoMerge ? "bg-success-400" : "bg-danger-400"))
        $($event.target).parents('tr').find('label.label:first').html(needtoMerge ? "Associated" : "No");
    }


    function init() {
        clearFirmSearch();
        bindDropDown();
        //$scope.GetMergeLocations();
    }

    //#endregion 

    init();
});
