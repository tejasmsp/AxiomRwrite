app.controller('QuickFormAttorneyListController', function ($scope, $rootScope, $state, $stateParams, QuickFormService, notificationFactory, Step6Service, CommonServices, configurationService, $compile, $filter) {

    //#region Declarations
    $scope.OrderNo = $stateParams.OrderId;
    $scope.PartNo = $stateParams.PartNo;
    decodeParams($stateParams);
    $scope.attorneyDataList = [];
    $scope.attorney = {};
    //#endregion

    //#region DOM Binding
    $scope.bindData = function () {
        var promise = QuickFormService.QuickFormGetOrderingAttorney($scope.OrderNo);
        promise.success(function (response) {
            if (response.Success) {
                $scope.attorneyDataList = response.Data;
            }
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.$parent.$watch('ShowQuickFormAttorneyList', function (newVal, oldVal) {
        if (newVal) {
            $scope.lblchecked = angular.copy($scope.$parent.lblchecked);
            $scope.selectedDocumentName = angular.copy($scope.$parent.selectedDocumentName);
            $scope.attorney = {};
            $scope.bindData();
        }

    }, true);

    //#endregion

    //#region Events
    $scope.addAttorneyClick = function () {
        if ($scope.lblchecked === 'Email' && (!$scope.attorney.selectedAttorneyEmail || $scope.attorney.selectedAttorneyEmail.length === 0)) {
            toastr.warning("Please select atleast one attorney.");              
            return;
        }
        else {
            //var objtoPass = new Object();
            //if ($scope.lblchecked === 'Email') {
            //    objtoPass.attorneyEmail = $scope.attorney.selectedAttorneyEmail[0];
            //}
            //else if ($scope.lblchecked === 'Fax') {
            //    objtoPass.attorneyFaxNo = $scope.attorney.selectedAttorneyFaxNo;
            //}
            
            $scope.$parent.SelectedemailByDocNameList.push({ 'SelectedDocumentName': $scope.selectedDocumentName, 'SelectedEmail': $scope.attorney.selectedAttorneyEmail? $scope.attorney.selectedAttorneyEmail[0]:"", 'SelectedFaxNo': $scope.attorney.selectedAttorneyFaxNo })

            $scope.closeClick();
        }

    }
    $scope.closeClick = function () {
        $scope.$parent.ShowQuickFormAttorneyList = false;
        angular.element("#modal_QuickAttorneyForm").modal('hide');
    }
    //#endregion
});