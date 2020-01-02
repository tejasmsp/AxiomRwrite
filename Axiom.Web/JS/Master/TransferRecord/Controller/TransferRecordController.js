app.controller('TransferRecordController', function ($rootScope, $scope, $stateParams, notificationFactory, CommonServices, TransferRecordService, $compile, $filter) {


    //$rootScope.CheckIsPageAccessible("Settings", "Transfer Record", "View");
    decodeParams($stateParams);

    $scope.objTransferRecord = new Object();
    $scope.objTransferRecord.EnitityTypeId = 2;
    $scope.objTransferRecord.SourceEntityId = null;
    $scope.objTransferRecord.TargetEntityId = null;
    $scope.objTransferRecord.UserId = $rootScope.LoggedInUserDetail.UserAccessId;


    $scope.TransferEntityFromLst = [];
    $scope.TransferEntityToLst = []; //function () { }; 

    $scope.OnTransferEntityTypeChanged = function () {
        bindDropdown();
    };

    function bindDropdown() {

        $scope.TransferEntityFromLst = [];
        var promise = TransferRecordService.GetEntityListForTrasferDropdown($scope.objTransferRecord.EnitityTypeId, $rootScope.CompanyNo);


        promise.success(function (response) {
            $scope.TransferEntityFromLst = response.Data;
            setTimeout(function () {
                $('.cls-firm').selectpicker('refresh');
                $('.cls-firm').selectpicker();
            }, 500);

        });
        promise.error(function (data, statusCode) { });
    }
    $scope.OnFromSelectionChanged = function () {
        $scope.TransferEntityToLst = [];
        $('#ddlTargetEntityId').parent().find('.filter-option-inner-inner').text('-- Select --');
        if (!isNullOrUndefinedOrEmpty($scope.objTransferRecord.SourceEntityId)) {
            $scope.TransferEntityToLst = $filter('filter')($scope.TransferEntityFromLst, { Id: '!' + $scope.objTransferRecord.SourceEntityId }, true);
            setTimeout(function () {
                $('.cls-firm').selectpicker('refresh');
                $('.cls-firm').selectpicker();
            }, 500);
        }
    };

    $scope.SubmitRecordsToTransfer = function (form) {
        var isvalid = ($scope.objTransferRecord.EnitityTypeId > 0
            && !isNullOrUndefinedOrEmpty($scope.objTransferRecord.SourceEntityId)
            && !isNullOrUndefinedOrEmpty($scope.objTransferRecord.TargetEntityId));
        if (isvalid) {
            var selectedSourceText = $("#ddlSourceEntityId option:selected").text();
            var selectedTargetText = $("#ddlTargetEntityId option:selected").text();
            bootbox.confirm({
                message: "Are you sure you want to transfer all the records from <b class='badge bg-info'>" + selectedSourceText + "</b> to <b class='badge bg-success'>" + selectedTargetText + "</b>?</br></br><b>Note:This transaction cannot be reversed.<b>",
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
                            if (response.Success) {
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
                                    $scope.objTransferRecord.SourceEntityId = '';
                                    $('#ddlSourceEntityId').parent().find('.filter-option-inner-inner').text('-- Select --');
                                    setTimeout(function () {
                                        $('.cls-firm').selectpicker('refresh');
                                        $('.cls-firm').selectpicker();
                                    }, 500);
                                    $scope.OnFromSelectionChanged();
                                    form.$submitted = false;
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
    $scope.RadioCheckChange = function (item, optionsList) {
        
        angular.forEach(optionsList, function (record, key) {
            record.IsChecked = (item.Id == record.Id); 
        });
        $scope.objTransferRecord.EnitityTypeId = $filter('filter')(optionsList, { 'IsChecked': true })[0].Id;
        bindDropdown();
    };
    function init() {

        var EntityEnum = { Location: 1, Attorney: 2, Firm: 3 };
        $scope.TransferEntityList =
            [
                // { Id: EntityEnum.Location, EntityName: 'Location', IsVisible: $rootScope.isSubModuleAccessibleToUser('Settings', 'Transfer Record', 'Location') },
            { Id: EntityEnum.Attorney, EntityName: 'Attorney', IsChecked: true },
            { Id: EntityEnum.Firm, EntityName: 'Firm', IsChecked: false }
            ];
        $scope.OnTransferEntityTypeChanged();

    };

    init();

});