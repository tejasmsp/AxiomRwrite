app.controller('AccountReceivableController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, AccountReceivableService, configurationService, CommonServices, $compile, $filter) {

   

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
     $scope.objAttorney = new Object();    
     $scope.SearchCheck();
    }
    $scope.SearchCheck = function () {
        debugger;
        $scope.objAttorney.FirmID = isNullOrUndefinedOrEmpty($scope.objAttorney.FirmID) ? "" : $scope.objAttorney.FirmID;
        $scope.objAttorney.FirmName = isNullOrUndefinedOrEmpty($scope.objAttorney.FirmName) ? "" : $scope.objAttorney.FirmName;
        $scope.objAttorney.CheckType = isNullOrUndefinedOrEmpty($scope.objAttorney.CheckType) ? "" : $scope.objAttorney.CheckType;
        $scope.objAttorney.CheckNo = isNullOrUndefinedOrEmpty($scope.objAttorney.CheckNo) ? "" : $scope.objAttorney.CheckNo;
        $scope.objAttorney.InvoiceNo = isNullOrUndefinedOrEmpty($scope.objAttorney.InvoiceNo) ? "" : $scope.objAttorney.InvoiceNo;
        var promise = AccountReceivableService.GetAccountReceivableListBySearch($scope.objAttorney.FirmID, $scope.objAttorney.FirmName, $scope.objAttorney.CheckType,
                                                $scope.objAttorney.CheckNo, $scope.objAttorney.InvoiceNo, $scope.UserAccessId);
        promise.success(function (response) {
            if (response.Success) {               
                $scope.AccountReceivableList = response.Data;
                bindAccountReceivable();
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function () { });
    }
    //$scope.GetAccountReceivableListBySearch = function () {
    //    var promise = AccountReceivableService.GetAccountReceivableListBySearch();
    //    promise.success(function (response) {
    //        if (response.Success) {                
    //            $scope.AccountReceivableList = response.Data;
    //            bindAccountReceivable();
    //        }
    //        else {
    //            toastr.error(response.Message[0]);
    //        }
    //    });
    //    promise.error(function () { });
    //}

    function bindAccountReceivable() {

        if ($.fn.DataTable.isDataTable("#tblAccountReceivable")) {
            $('#tblAccountReceivable').DataTable().destroy();
        }

        var table = $('#tblAccountReceivable').DataTable({
            data: $scope.AccountReceivableList,
            "bDestroy": true,
            "dom": '<"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                //{
                //    "title": "FirmID",
                //    "className": "dt-left",
                //    "data": "FirmID",
                //    "sorting": "true"
                //},
                {
                    "title": "Firm Name",
                    "className": "dt-left",
                    "data": "FirmName",
                    "sorting": "true"
                },
                {
                    "title": "Check Number",
                    "className": "dt-left",
                    "data": "CheckNumber",
                    "sorting": "true"
                },
                {
                    "title": "Check Amount",
                    "className": "dt-left",
                    "data": "CheckAmount",
                    "sorting": "true"
                },
                  {
                      "title": "Remaining Amount",
                      "className": "dt-left",
                      "data": "RemainingAmount",
                      "sorting": "true"
                },
                {
                    "title": "Invoices",
                    "className": "dt-left",
                    "data": "InvoicesWithPayment",
                    "sorting": "true"
                },
                {
                    "title": "CheckType",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "CheckType",
                    "width": "10%",
                    "render": function (data, type, row) {                        
                        if (row.CheckType == 'N') {
                            return "<label class='label bg-success-400'>Normal Check</label>";
                        }
                        else if (row.CheckType == 'D') {
                            return "<label class='label bg-grey-400'>Debit</label>";
                        }
                        else if (row.CheckType == 'C') {
                            return "<label class='label bg-indigo-400'>Credit</label>";
                        }                       
                        else if (row.CheckType == 'B') {
                            return "<label class='label bg-brown-400'>Bounce Check</label>";
                        }
                        else if (row.CheckType == 'L') {
                            return "<label class='label bg-danger-400'>Cancel Check</label>";
                        }
                            else {
                            return "";
                        }
                        
                    }
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='EditAccountReceivable($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        //if (row.CheckType != 'L') {
                           
                        //}
                      
                        //strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteCourt($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblAccountReceivable').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {  
                debugger;
                if ((aData.CheckAmount > aData.RemainingAmount) && aData.RemainingAmount > 0 && (aData.CheckType != "L" && aData.CheckType != "B")) {
                    $('td',nRow).css('background-color', '#beeec0');
                }
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    };
    $scope.EditAccountReceivable = function ($event) {
        var table = $('#tblAccountReceivable').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $state.transitionTo('ManageCheck', { 'ArID': row.ArID });
    }
 $scope.ClearSearch = function () {
     $scope.objAttorney.FirmID = "";
     $scope.objAttorney.FirmName = "";
     $scope.objAttorney.CheckType = "";
     $scope.objAttorney.CheckNo = "";
     $scope.objAttorney.InvoiceNo = "";
    $scope.SearchCheck();
 }
 $scope.SearchAttorney = function () {
     $scope.objAttorney.FirmID = "";
     if ($scope.objAttorney.FirmID == null || $scope.objAttorney.FirmID == "") {
         $scope.objAttorney.FirmID = $scope.objAttorney.FirmIDSearch;
     }
   
 }

 $scope.AddFirm = function () {
     $scope.FirmObj = new Object();
     $scope.isEdit = false;
     $scope.modal_Title = "Add Firm";
     $scope.btnText = "Save";
     $scope.Firmform.$setPristine();
     // bindDropDown();
     angular.element("#modal_Firm").modal('show');
 };

 $scope.FirmPopup = function () {
     $scope.ShowSearchFirm = true;
 }



 init();

    });
