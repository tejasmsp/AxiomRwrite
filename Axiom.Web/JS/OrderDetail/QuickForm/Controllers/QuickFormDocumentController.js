app.controller('QuickFormDocumentController', function ($scope, $rootScope, $state, $stateParams, QuickFormService, notificationFactory, Step6Service, CommonServices, configurationService, $compile, $filter) {

    //#region Declarations

    $scope.OrderNo = $stateParams.OrderId;
    $scope.documentList = [];
    $scope.attorneyList = [];
    decodeParams($stateParams);
    $scope.selectedAttorneyList = [];
    $scope.quickFormDoc = {};
    //#endregion

    //#region DOM Binding
    $scope.$parent.$watch('chkNoticeStatus', function (newVal, oldVal) {
        if (newVal) {
            $scope.GetParentData($scope.$parent.documentType, $scope.$parent.selectedPartNo);
        }

    }, true);
    $scope.$parent.$watch('chkCWCStatus', function (newVal, oldVal) {
        if (newVal) {
            $scope.GetParentData($scope.$parent.documentType, $scope.$parent.selectedPartNo);
        }
    }, true);
    $scope.$parent.$watch('chkFaceSheetStatus', function (newVal, oldVal) {
        if (newVal) {
            $scope.GetParentData($scope.$parent.documentType, $scope.$parent.selectedPartNo);
        }
    }, true);
    $scope.GetParentData = function (documentType, PartNo) {
        
        $scope.ShowQuickFormPopup();
        $scope.DocumetType = angular.copy(documentType);
        $scope.PartNo = angular.copy(PartNo);
        $scope.documentdataList = [];
        $scope.bindDataList();
        $scope.QuickFormGetAttorneyDetail();
        $scope.quickFormDoc = {};
        $scope.selectedAttorneyList = [];
    }


    $scope.ShowQuickFormPopup = function () {
        angular.element("#modal_QuickdocumentForm").modal('show');
        $scope.$parent.ShowQuickForm = false;
        $scope.$parent.ShowQuickFormDocumentPopup = false;
    };

    $scope.bindDataList = function () {
        if ($scope.PartNo) {
            var documentList = QuickFormService.QuickFormGetDocumentListByType($scope.DocumetType, $scope.OrderNo, $scope.PartNo);
            documentList.success(function (response) {
                if (response.Success) {
                    $scope.documentdataList = response.Data;

                }
            });
            documentList.error(function (data, statusCode) {
            });
        }
    }
    $scope.QuickFormGetAttorneyDetail = function () {
        var promise = QuickFormService.QuickFormGetOrderingAttorney($scope.OrderNo);
        promise.success(function (response) {
            if (response.Success && response.Data) {
                $scope.attorneyList = [];
                angular.forEach(response.Data, function (item, index) {
                    var attId = item.AttyID.replace(/ +/g, "");
                    $scope.attorneyList.push({ attIdwithoutSpace: attId, AttorneyType: item.AttorneyType, AttyID: item.AttyID, Email: item.Email, FaxNo: item.FaxNo, FirstName: item.FirstName, LastName: item.LastName, Represent: item.Represent });
                });
                
            }
        });
        promise.error(function (data, statusCode) {
        });
    }
    
    //#endregion

    //#region Events
    $scope.selectedAttorneyRowDocument = function (index) {

        var objAttorneyData = angular.copy($scope.attorneyList[index]);
        if (objAttorneyData) {
            var attId = objAttorneyData.AttyID.replace(/ +/g, "");
            var status = $("#chk-" + attId).is(":checked");
            status = !status;
            $("#chk-" + attId).prop('checked', status);
            SelectedAttorneyDataList(status, attId, objAttorneyData);
        }
    }
    $scope.selectedAttorneyData = function (index, $event) {
        $event.stopPropagation();
        var objAttorneyData = angular.copy($scope.attorneyList[index]);
        if (objAttorneyData) {
            var attId = objAttorneyData.AttyID.replace(/ +/g, "");
            var status = $("#chk-" + attId).is(":checked");
            SelectedAttorneyDataList(status, attId, objAttorneyData);
        }
    }
    function SelectedAttorneyDataList(status, attId, objAttorneyData) {
        if (status) {
            $scope.selectedAttorneyList.push({ 'AttId': attId, 'AttorneyId': objAttorneyData.AttyID, 'AttorneyType': objAttorneyData.AttorneyType, 'FirstName': objAttorneyData.FirstName, 'LastName': objAttorneyData.LastName, 'FaxNo': objAttorneyData.FaxNo });
        }
        else {
            $scope.selectedAttorneyList = $scope.selectedAttorneyList.filter(function (obj) {
                return obj.AttId !== attId
            });
        }
    }


    $scope.closeClick = function () {
        $scope.$parent.chkNoticeStatus = false;
        $scope.$parent.chkCWCStatus = false;
        $scope.$parent.chkFaceSheetStatus = false;
        angular.element("#modal_QuickdocumentForm").modal('hide');

    }
    $scope.saveClick = function () {
        
        if ($scope.DocumetType != 'Facesheets' && (!$scope.quickFormDoc.documentName || $scope.quickFormDoc.documentName.length === 0)) {
            alert("Select a document");
            return;
        }
        else if (!$scope.selectedAttorneyList || $scope.selectedAttorneyList.length === 0) {
            toastr.warning("Select atleast one attorney.");                
            return;
        }
        else {
            var objDatatoPass = new Object();
            if ($scope.DocumetType === 'Facesheets' && $scope.documentdataList && $scope.documentdataList.length > 0) {
                objDatatoPass.DocumentName = $scope.documentdataList[0].DocumentName;
                objDatatoPass.DocumentPath = $scope.documentdataList[0].DocumentPath;
            }
            else if ($scope.DocumetType !== 'Facesheets' && $scope.quickFormDoc.documentName && $scope.quickFormDoc.documentName.length > 0) {
                objDatatoPass.DocumentName = $scope.quickFormDoc.documentName[0];
                var objDoc = $filter('filter')($scope.documentdataList, { DocumentName: $scope.quickFormDoc.documentName[0] }, true);
                if (objDoc && objDoc.length > 0) {
                    objDatatoPass.DocumentPath = objDoc[0].DocumentPath;
                }
            }

            objDatatoPass.DocumentType = $scope.DocumetType;
            objDatatoPass.selectedAttorneyList = angular.copy($scope.selectedAttorneyList);
            
            $scope.$emit('QuickFormParent', objDatatoPass);
            angular.element("#modal_QuickdocumentForm").modal('hide');
            $scope.$parent.showPrintDocQueue = true;
        }
    }

    //#endregion
});