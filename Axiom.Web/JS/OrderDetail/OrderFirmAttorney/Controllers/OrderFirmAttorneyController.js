app.controller('OrderFirmAttorneyController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, OrderFirmAttorneyService, CommonServices, configurationService, $compile, $filter) {
    decodeParams($stateParams);
    $scope.IsFromClient = 1;  // Temporary
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEdit = false;
    //#region Event

    $scope.GetOrderFirmAttorneyByOrderId = function () {
        var promise = OrderFirmAttorneyService.GetOrderFirmAttorneyByOrderId($scope.OrderId);
        promise.success(function (response) {
            $scope.OrderFirmAttorneyList = response.Data;
        });
        promise.error(function (data, statusCode) {
        });
    };

    // #region Waiver

    $scope.WaiverPopUp = function (OrderFirmAttorneyId) {
        var promise = OrderFirmAttorneyService.GetWaiverDetailByOrderFirmAttorneyId(OrderFirmAttorneyId, $scope.OrderId);
        promise.success(function (response) {
            $scope.waiverList = response.Data;
            angular.element("#modal_waiver").modal('show');
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.SaveWaiver = function (form) {
        if (form.$valid) {
            var promise = OrderFirmAttorneyService.SaveWaiver($scope.OrderId, $scope.waiverList);
            promise.success(function (response) {
                if (response.Success) {
                    if (response.lng_InsertedId == "1") {
                        angular.element("#modal_waiver").modal('hide');
                        toastr.success("Save successfully");
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                }
            });
        }
    };

    $scope.SetValue = function (item, flag) {
        $("#radWaived").attr('checked', false);
        $("#radCopy").attr('checked', false);
        if (flag == "1") {
            item.Copy = false;
            item.Waiver = true;
        }
        else {
            item.Copy = true;
            item.Waiver = false;
        }
    };

    $scope.ChangeAllWaived_Copy = function (flag) {
        if (flag == 1) {
            angular.forEach($filter('filter')($scope.waiverList), function (item, key) {
                item.Waiver = true;
                item.Copy = false;
            });
            $("#radWaived").attr('checked', true);

        }
        else if (flag == 2) {
            angular.forEach($filter('filter')($scope.waiverList), function (item, key) {
                item.Waiver = false;
                item.Copy = true;
            });
            $("#radCopy").attr('checked', true);
        }
    }
    //#endregion

    //#region Firm

    $scope.ClearFirmSearch_Click = function () {
        clearFirmSearch();
        $scope.bindFirmList();
    };

    $scope.FirmSearch_Click = function () {
        $scope.bindFirmList();
    };

    $scope.bindFirmList = function () {

        if ($.fn.DataTable.isDataTable("#tblFirm")) {
            $('#tblFirm').DataTable().destroy();
        }
        var pagesizeObj = 10;

        $('#tblFirm').DataTable({
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
            "aaSorting": [[0, 'asc']],
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
                    "url": sSource + "?SearchValue=" + "" + "&PageIndex=" + (parseInt($('#tblFirm').DataTable().page.info().page) + 1)
                        + "&FirmID=" + ($scope.FirmSearchObj.FirmID == null ? "" : $scope.FirmSearchObj.FirmID)
                        + "&FirmName=" + ($scope.FirmSearchObj.FirmName == null ? "" : $scope.FirmSearchObj.FirmName)
                        + "&Address=" + ($scope.FirmSearchObj.Address == null ? "" : $scope.FirmSearchObj.Address)
                        + "&City=" + ($scope.FirmSearchObj.City == null ? "" : $scope.FirmSearchObj.City)
                        + "&State=" + ($scope.FirmSearchObj.State == null ? "" : $scope.FirmSearchObj.State),
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });


            },
            "columns": [
                { "title": "Firm Code", "data": "FirmID", "className": "dt-left", "width": "14%" },
                { "data": "FirmName", "title": "Name", "className": "dt-left" },
                { "data": "City", "title": "City", "className": "dt-left" },
                { "data": "State", "title": "State", "className": "dt-left" },
                { "data": "Address", "title": "Address", "className": "dt-left" },
                {
                    "title": 'Action',
                    "sClass": "action dt-center",
                    "orderable": false,
                    "width": "14%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='FillFirm($event)' title='Add'> <i  class='icon-add cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblFirm').DataTable();
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

    };

    //#endregion

    $scope.NewRqstAttorneyPopup = function () {
        BindDropDown();
        $scope.IsEdit = false;
        $scope.FirmAttorneyObj = new Object();
        $scope.FirmAttorneyform.$setPristine();
        $scope.FirmAttorneyObj.OrderFirmAttorneyId = 0;
        $scope.FirmAttorneyObj.IsPatientAttorney = 0;
        $scope.FirmAttorneyObj.OppSide = 0;
        angular.element("#modal_FirmAttorney").modal('show');
    };
    $scope.EditFirmAttorneyPopup = function (OrderFirmAttorneyId) {
        BindDropDown();
        $scope.IsEdit = true;
        $scope.FirmAttorneyObj = new Object();
        var promise = OrderFirmAttorneyService.GetOrderFirmAttorneyByOrderFirmAttorneyId(OrderFirmAttorneyId);
        promise.success(function (response) {
            if (response.Success) {
                $scope.FirmAttorneyform.$setPristine();
                $scope.FirmAttorneyObj = response.Data[0];
                angular.element("#modal_FirmAttorney").modal('show');
                $scope.BindAttorneyByFirmDropdown(); 
            }
        });
    };


    $scope.SelectFirmPopup = function () {
        clearFirmSearch();
        $scope.bindFirmList();
        angular.element("#modal_FirmSearchForAttorneyContacts").modal('show');
    };

    $scope.FillFirm = function ($event) {
        var tblFirm = $('#tblFirm').DataTable();
        var data = tblFirm.row($($event.target).parents('tr')).data();
        $scope.FirmAttorneyObj.FirmID = data.FirmID;
        $scope.FirmAttorneyObj.FirmName = data.FirmName;
        angular.element("#modal_FirmSearchForAttorneyContacts").modal('hide');
        $scope.BindAttorneyByFirmDropdown();
    };

    $scope.BindAttorneyByFirmDropdown = function () {

        var attry = CommonServices.GetAttorneyByFirmID($scope.FirmAttorneyObj.FirmID);
        attry.success(function (response) {
            $scope.AttorneyListByFirm = response.Data;
        });
    };

    $scope.GetAttorneyByAttyId = function () {
        if (!isNullOrUndefinedOrEmpty($scope.FirmAttorneyObj.AttyId)) {
            var attry = OrderFirmAttorneyService.GetAttorneyByAttyId($scope.FirmAttorneyObj.AttyId);
            attry.success(function (response) {
                if (response.Success) {
                    $scope.FirmAttorneyObj.AttyId = response.Data[0].AttyId;
                    $scope.FirmAttorneyObj.FirstName = response.Data[0].FirstName;
                    $scope.FirmAttorneyObj.LastName = response.Data[0].LastName;
                    $scope.FirmAttorneyObj.Salutation = response.Data[0].Salutation;
                    $scope.FirmAttorneyObj.Street1 = response.Data[0].Street1;
                    $scope.FirmAttorneyObj.Street2 = response.Data[0].Street2;
                    $scope.FirmAttorneyObj.City = response.Data[0].City;
                    $scope.FirmAttorneyObj.State = response.Data[0].State;
                    $scope.FirmAttorneyObj.Zip = response.Data[0].Zip;
                    $scope.FirmAttorneyObj.AreaCode1 = response.Data[0].AreaCode1;
                    $scope.FirmAttorneyObj.PhoneNo = response.Data[0].PhoneNo;
                    $scope.FirmAttorneyObj.AreaCode2 = response.Data[0].AreaCode2;
                    $scope.FirmAttorneyObj.FaxNo = response.Data[0].FaxNo;
                    $scope.FirmAttorneyObj.StateBarNo = response.Data[0].StateBarNo;
                    $scope.FirmAttorneyObj.FirmName = response.Data[0].FirmName;

                }
            });
        }
    };

    $scope.SaveNewRqstAttorney = function (form) {
        if (!isNullOrUndefinedOrEmpty($scope.FirmAttorneyObj.FirmID)) {
            form.$submitted = true;
            if (form.$valid) {
                $scope.FirmAttorneyObj.EmpId = $rootScope.LoggedInUserDetail.EmpId;
                $scope.FirmAttorneyObj.OrderId = $scope.OrderId;
                $scope.FirmAttorneyObj.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
                var promise = OrderFirmAttorneyService.InsertUpdateOrderFirmAttorney($scope.FirmAttorneyObj);
                promise.success(function (response) {
                    if (response.Success) {
                        if (response.lng_InsertedId == "-1") {
                            toastr.error("Record Already Exist");
                        }
                        else {
                            angular.element("#modal_FirmAttorney").modal('hide');
                            $scope.GetOrderFirmAttorneyByOrderId();
                            if ($scope.FirmAttorneyObj.OrderFirmAttorneyId == 0) {
                                toastr.success("Record Save Successfully");
                            }
                            else {
                                toastr.success("Record Update Successfully");
                            }
                        }

                    }
                });

            }
        }
        else {
            notificationFactory.customError("Select Firm");
            $scope.FirmAttorneyform.$setPristine();
        }
    };
    //#endregion

    //#region Method  

    function clearFirmSearch() {
        $scope.FirmSearchObj = new Object();
        $scope.FirmSearchObj.FirmID = "";
        $scope.FirmSearchObj.FirmName = "";
        $scope.FirmSearchObj.Address = "";
        $scope.FirmSearchObj.City = "";
        $scope.FirmSearchObj.State = "";
    }

    function BindDropDown() {
        var _statelist = CommonServices.StateDropdown();
        _statelist.success(function (response) {
            $scope.Statelist = response.Data;
        });
        _statelist.error(function (data, statusCode) { });

        var _salutation = CommonServices.GetSalutationDropDown();
        _salutation.success(function (response) {
            $scope.SalutationList = response.Data;
        });
        _salutation.error(function (data, statusCode) { });

        var _attorney = CommonServices.GetAttorneyDropDown();
        _attorney.success(function (response) {
            $scope.AttorneyList = response.Data;
        });
        _attorney.error(function (data, statusCode) { });

    }


    $scope.ChangeOrderAttorneyStatus = function (item) {        
        var IsDisabled = true;
        if (item.IsOrderAttorneyDisabled) {
             IsDisabled = false;
        }
            
        var promisetoupdatestatus = OrderFirmAttorneyService.ChangeOrderAttorneyStatus($scope.OrderId, item.AttyId, IsDisabled, $scope.UserAccessId);
        promisetoupdatestatus.success(function (response) {
            $scope.GetOrderFirmAttorneyByOrderId();
        });
        promisetoupdatestatus.error(function (data, statusCode) { });
        
    };

    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.PartNo = isNullOrUndefinedOrEmpty($stateParams.PartNo) ? 0 : $stateParams.PartNo;
        $scope.GetOrderFirmAttorneyByOrderId();
    }

    //#endregion


    init();

});