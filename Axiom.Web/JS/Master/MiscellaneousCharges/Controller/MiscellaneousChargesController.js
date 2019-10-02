app.controller('MiscellaneousChargesController', function ($rootScope, $scope, $stateParams, MiscellaneousChargesServiceService, notificationFactory, billingRateService, configurationService, CommonServices, $compile, $filter) {
    $scope.objMiscCharges = new Object();

    decodeParams($stateParams);
    $scope.ShowPageNumber = false;

    //Page Rights//
   // $rootScope.CheckIsPageAccessible("Settings", "Billing Rate", "View");
    //------------
    function init() {
        //GetMemberList();
        //GetMiscellaneousCharges(0);       
        $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;	
        $scope.objMiscCharges = new Object();
        $scope.TempDesc = new Object();
        //GetMemberList();
        $scope.MiscDesc = null;
        $scope.MemberID = null;
        $scope.MiscDescList = [];
        GetMiscellaneousCharges(0);
            

    };
      
    // Get Member Dropdown
    function GetMemberList() {
        var member = CommonServices.MemberDropdown();
        member.success(function (response) {             
            if (isNullOrUndefinedOrEmpty($scope.TempDesc.ProgID) && isNullOrUndefinedOrEmpty($scope.objMiscCharges.FeeNo)) {                
                $scope.objMiscCharges.MemberID = response.Data[0].MemberID;                
            }  
            
            if (isNullOrUndefinedOrEmpty($scope.objMiscCharges.MemberID)) {
                $scope.MemberID = response.Data[0].MemberID;  
                $scope.objMiscCharges.MemberID = $scope.MemberID;
            }           
            $scope.MemberList = response.Data;              
            $scope.GetMiscDescById($scope.objMiscCharges.MemberID);
        });
        member.error(function (data, statusCode) { });
    };

    // Open Misc charges Popup
    $scope.AddMiscellaneousCharges = function (MiscId) {
        $scope.Model_MiscTitle ="Add Miscellaneous Charges"
       $scope.MiscChargesForm.$setPristine();                      
       // $scope.objMiscCharges.MiscDesc = null;
        $scope.objMiscCharges.RegularFee = null;
        $scope.objMiscCharges.DiscountFee = null;
        GetMemberList();
      
        $scope.objMiscCharges.FeeNo = 0;  
        if (!isNullOrUndefinedOrEmpty($scope.TempDesc.ProgID)) {
            setTimeout(function () {
                $('.memberid option').last().prop('selected', true);
            }, 100);
          
        }
    
        angular.element("#modal_MiscCharges").modal('show');
      
    }
    // Close Misc Charges Popup
    $scope.closeMiscChargesForm = function () {
        if (isNullOrUndefinedOrEmpty($scope.objMiscCharges.FeeNo == 0)) {
            $scope.objMiscCharges.FeeNo = 0;
            //$scope.objMiscCharges.MemberID = null;
            $scope.objMiscCharges.MiscDesc = null;
            $scope.objMiscCharges.RegularFee = null;
            $scope.objMiscCharges.DiscountFee = null;
            $scope.MiscDescList = [];
            $scope.TempDesc.ProgID = null;
            $scope.TempDesc.MiscDesc = null;
        }      
        angular.element("#modal_MiscCharges").modal('hide');
    }

    //Get Misc Charges by Id
    $scope.GetMiscDescById=function(MemberID) {       
        var MiscDesc = MiscellaneousChargesServiceService.GetMiscDescById(MemberID);
        MiscDesc.success(function (response) {             
            $scope.MiscDescList = [];
            $scope.MemberID = $scope.objMiscCharges.MemberID;
            $scope.MiscDescList = response.Data;            
            if (!isNullOrUndefinedOrEmpty($scope.TempDesc.ProgID)) {
                $scope.MiscDescList.push($scope.TempDesc);   
                
                //$('.memberid option').prop('selected', false)[0];                             
            }
          
        });
        MiscDesc.error(function (data, statusCode) { });
    }

    //Get Misc Charges list
    function GetMiscellaneousCharges(MiscId) {
        var MiscDesc = MiscellaneousChargesServiceService.GetMiscellaneousCharges(MiscId);
        MiscDesc.success(function (response) {
            $scope.MiscDescList = response.Data;
            BindMisc();    
        });
        MiscDesc.error(function (data, statusCode) { });
    }

   // Insert Or Update Misc charges
    $scope.InsertOrUpdateMiscCharges = function (form) {
        if (form.$valid) {            
            $scope.objMiscCharges.CreatedBy = $scope.EmpId;
            if ($scope.objMiscCharges.FeeNo > 0) {
                var promise = MiscellaneousChargesServiceService.UpdateMiscCharges($scope.objMiscCharges);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_MiscCharges").modal('hide');
                        notificationFactory.customSuccess("Miscellaneous Charges updated successfully");
                        GetMiscellaneousCharges(0);
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            } else {
                $scope.objMiscCharges.FeeNo = 0;
                var promise = MiscellaneousChargesServiceService.InsertMiscCharges($scope.objMiscCharges);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_MiscCharges").modal('hide');
                        notificationFactory.customSuccess("Miscellaneous Charges saved successfully");
                      GetMiscellaneousCharges(0);
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
          

        }
    }

    //open Misc Description popup
    $scope.GetMiscPopUp = function () {      
        $scope.MiscDesc = null;
        $scope.MiscDescForm.$setPristine();
        angular.element("#modal_MiscDesc").modal('show');

    }

    // Add new Misc description
    $scope.AddMiscDesc = function (form) {        
        if (form.$valid) {            
            $scope.TempDesc.ProgID = $scope.MemberID;
            $scope.TempDesc.MiscDesc = $scope.MiscDesc;
            $scope.objMiscCharges.MemberID = $scope.TempDesc.ProgID;
           // $scope.MiscDescList.push($scope.TempDesc);
            angular.element("#modal_MiscDesc").modal('hide');
        }       
    }
    // bind misc list
   function BindMisc(){
       if ($.fn.DataTable.isDataTable("#tblMiscChargesList")) {
           $('#tblMiscChargesList').DataTable().destroy();
       }

       var table = $('#tblMiscChargesList').DataTable({
           data: $scope.MiscDescList,
           "bDestroy": true,
           "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
           "aaSorting": false,
           "aLengthMenu": [10, 20, 50, 100, 200],
           "pageLength": 10,
           "stateSave": false,
           "columns": [

               {
                   "title": "Member #",
                   "className": "dt-left",
                   "data": "MemberID",
                   "width": "8%",
                   "sorting": "false"
               },
               {
                   "title": "Misc Description",
                   "className": "dt-left",
                   "data": "MiscDesc",
                   "sorting": "false"
               },

               {
                   "title": "Regular Fees",
                   "className": "dt-left",
                   "data": "RegularFee",
                   "sorting": "false"

               },
               {
                   "title": "Discount Fees",
                   "className": "dt-left",
                   "data": "DiscountFee",
                   "sorting": "false"

               },
               {
                   "title": 'Action',
                   "data": null,
                   "className": "action dt-center",
                   "sorting": "false",
                   "width": "6%",
                   "render": function (data, type, row) {
                       var strAction = '';
                       strAction = "<a  class='ico_btn cursor-pointer' ng-click='EditMiscCharges($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                       //if ($scope.IsUserCanEditScope) {
                          
                       //}
                       //if ($scope.IsUserCanDeletScope) {
                       //    strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteAttorneyScope($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                       //}


                       return strAction;
                   }
               }
           ],
           "initComplete": function () {
               var dataTable = $('#tblMiscChargesList').DataTable();
           },
           "fnDrawCallback": function () {
           },
           "fnCreatedRow": function (nRow, aData, iDataIndex) {
               $compile(angular.element(nRow).contents())($scope);
           }
       });
   }

    //Edit Misc charges
    $scope.EditMiscCharges = function ($event) {        
        var table = $('#tblMiscChargesList').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.objMiscCharges.FeeNo = row.FeeNo;       
        $scope.objMiscCharges.MiscDesc = row.MiscDesc;
        $scope.objMiscCharges.RegularFee = row.RegularFee;
        $scope.objMiscCharges.DiscountFee = row.DiscountFee;
        var member = CommonServices.MemberDropdown();
        member.success(function (response) {          
            $scope.MemberList = response.Data;
            $scope.objMiscCharges.MemberID = row.MemberID;
            $scope.GetMiscDescById($scope.objMiscCharges.MemberID);
        });
        var MiscDesc = MiscellaneousChargesServiceService.GetMiscDescById($scope.objMiscCharges.MemberID);
        MiscDesc.success(function (response) {  
            if (isNullOrUndefinedOrEmpty($scope.MiscDescList)) {
                $scope.MiscDescList = response.Data;  
            }
                     
        });
        member.error(function (data, statusCode) { });
        $scope.Model_MiscTitle = "Edit Miscellaneous Charges"
        angular.element("#modal_MiscCharges").modal('show');
    }
    init();



});