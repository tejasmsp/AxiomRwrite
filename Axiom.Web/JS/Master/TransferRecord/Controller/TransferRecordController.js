app.controller('TransferRecordController', function ($rootScope, $scope, $stateParams, notificationFactory, CommonServices, TransferRecordService, $compile, $filter) {

    
    //$rootScope.CheckIsPageAccessible("Settings", "Transfer Record", "View");
    decodeParams($stateParams);

    $scope.objTransferRecord = new Object();
    $scope.objTransferRecord.EnitityTypeId = 2;
    $scope.objTransferRecord.SourceEntityId = null;
    $scope.objTransferRecord.TargetEntityId = null;
    $scope.objTransferRecord.UserId = $rootScope.LoggedInUserDetail.UserAccessId;

    
    $scope.TransferEntityFromLst = [];
   // $scope.TransferEntityToLst = function () { return $filter('filter')($scope.TransferEntityFromLst, { Id:  '!'+ $scope.objTransferRecord.SourceEntityId  }, true ) }; 

    $scope.OnTransferEntityTypeChanged = function () {
        bindDropdown();
    };

    function bindDropdown() {

        $scope.TransferEntityFromLst = [];
        var promise = TransferRecordService.GetEnitityListForTrasferDropdown($scope.objTransferRecord.EnitityTypeId, $rootScope.CompanyNo);


        promise.success(function (response) {
            $scope.TransferEntityFromLst = response.Data;
            setTimeout(function () {
                $('.cls-firm').selectpicker('refresh');
                $('.cls-firm').selectpicker();
            }, 500);
            
        });
        promise.error(function (data, statusCode) { });
    }
    $scope.OnFromListChanged = function () {

        
    };

    $scope.SubmitRecordsToTransfer = function (form) {
        var isvalid = ($scope.objTransferRecord.EnitityTypeId > 0
            && !isNullOrUndefinedOrEmpty($scope.objTransferRecord.SourceEntityId)
            && !isNullOrUndefinedOrEmpty($scope.objTransferRecord.TargetEntityId));
        if (isvalid) {
            var selectedSourceText = $("#ddlSourceEntityId option:selected").text();
            var selectedTargetText = $("#ddlTargetEntityId option:selected").text();
            bootbox.confirm({
                message: "Are you sure you want to transfer all the records from(" + selectedSourceText + ") to (" + selectedTargetText +")?",
                buttons: {
                    cancel: {
                        label: 'No',
                        className: 'btn-danger'
                    },
                    confirm: {
                        label: 'Yes',
                        className: 'btn-success'
                    }
                },
                callback: function (result) {
                    if (result == true) {
                        var promise = TransferRecordService.SubmitRecordsToTransfer($scope.objTransferRecord);
                        promise.success(function (response) {
                            if (response.Success)
                            {
                                var affectedrecords = 0;
                                if (response.Data != null && response.Data.length > 0) {
                                    affectedrecords = response.Data[0].AffectRecordCount;
                                }
                                
                               
                                  
                                    
                                    var selectedRecord = $filter("filter")($scope.TransferEntityFromLst, { Id: $scope.objTransferRecord.SourceEntityId }, true);
                                    if (selectedRecord != null) {
                                        selectedRecord = selectedRecord[0];
                                        $scope.TransferEntityFromLst.splice($scope.TransferEntityFromLst.indexOf(selectedRecord), 1);
                                        var newList = [];
                                        angular.copy($scope.TransferEntityFromLst, newList);
                                        $scope.TransferEntityFromLst = newList;
                                        $scope.objTransferRecord.SourceEntityId = null;
                                    }
                                    $("#ddlSourceEntityId option:selected").remove();
                                    //
                                    //bindDropdown()
                                
                                 toastr.success("Total " + affectedrecords + " records has been transferred successfully (" + selectedSourceText + "-->" + selectedTargetText + " ).");
                            }
                            else {
                                toastr.error(response.Message[0]);
                            }
                        });
                        promise.error(function (data, statusCode) {
                        });
                    }
                    else {
                         
                    }
                    bootbox.hideAll();
                }
            });

            
        }
        else {
            toastr.error("Please select entity type, source & target entity.");
        }

    };
    function init() {

        var EntityEnum = { Location: 1, Attorney: 2, Firm: 3 };
        $scope.TransferEntityList =
            [
                // { Id: EntityEnum.Location, EntityName: 'Location', IsVisible: $rootScope.isSubModuleAccessibleToUser('Settings', 'Transfer Record', 'Location') },
                 { Id: EntityEnum.Attorney, EntityName: 'Attorney', IsVisible: $rootScope.isSubModuleAccessibleToUser('Settings', 'Transfer Record', 'Attorney') },
                 { Id: EntityEnum.Firm, EntityName: 'Firm', IsVisible: $rootScope.isSubModuleAccessibleToUser('Settings', 'Transfer Record', 'Firm') }
            ];
        $scope.OnTransferEntityTypeChanged(); 

    };

    init();

});