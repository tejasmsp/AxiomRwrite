app.controller('QuickFormDocumentAttachmentController', function ($scope, $rootScope, $state, $stateParams, QuickFormService, notificationFactory, Step6Service, CommonServices, configurationService, $compile, $filter) {

    //#region Declarations

    $scope.declaration = function () {
        $scope.OrderNo = $stateParams.OrderId;
        $scope.PartNo = $stateParams.PartNo;
        decodeParams($stateParams);
        $scope.docAttachList = [];
        $scope.files = [];
        $scope.showUploadbtn = false;
        $scope.uploadedFileList = [];
        $scope.selectedDocumentList = [];
    }

    //#endregion

    //#region DOM Binding
    $scope.bindData = function () {       
        var documentAttachmentList = QuickFormService.QuickFormGetAttachFileList($scope.OrderNo, 0);
        documentAttachmentList.success(function (response) {
            if (response.Success) {
                $scope.docAttachList = angular.copy(response.Data);
            }
        });
        documentAttachmentList.error(function (data, statusCode) {
        });
    }

    $scope.$parent.$watch('ShowQuickFormDocAttachment', function (newVal, oldVal) {
        if (newVal) {
            $scope.declaration();
            $scope.bindData();
            $scope.docName = angular.copy($scope.$parent.attchmentClickDocumentName);
        }
    }, true);

    //#endregion

    //#region Events

    $scope.selectedtableRowDocument = function (item) {
        var isselected = $("#chk-" + item.FileId).is(":checked");
        isselected = !isselected;
        $("#chk-" + item.FileId).prop('checked', isselected);
        SelectedDocumentDataList(isselected, item.FileId);
    }
    $scope.selectedDocument = function (fileId, $event) {
        $event.stopPropagation();
        var isselected = $("#chk-" + fileId).is(":checked");
        SelectedDocumentDataList(isselected, fileId);
    }
    function SelectedDocumentDataList(isselected, fileId) {
        if (!isselected) {
            $scope.isselectedAll = false;
            var index = $filter('filter')($scope.selectedDocumentList, { 'FileId': parseInt(fileId) }, true);// $scope.selectedDocumentList.map(e => e.FileId).indexOf(fileId);
            if (index) {
                $scope.selectedDocumentList.splice(index, 1);
            }
        }
        else {
            var objDocData = $filter('filter')($scope.docAttachList, { 'FileId': parseInt(fileId) }, true);
            if (objDocData) {
                $scope.selectedDocumentList.push({ attchmentClickDocumentName: $scope.docName, FileName: objDocData[0].FileName, FileId: objDocData[0].FileId, FileDiskName: objDocData[0].FileDiskName });
            }
        }
    }
    $scope.closeClick = function () {
        $scope.$parent.ShowQuickFormDocAttachment = false;
        angular.element("#modal_QuickdocumentAttachmentForm").modal('hide');
    }

    $scope.UploadFile = function () {
        $("#OrderDocumentQuickForm").click();
    }
    $scope.FileUploadChange = function (files) {
        $scope.UserGUID = $rootScope.LoggedInUserDetail.UserId;
        var allfiles = $("#OrderDocumentQuickForm")[0].files;
        angular.forEach(allfiles, function (item, index) {
            var fd = new FormData();
            fd.append("file", item);
            var fileupload = QuickFormService.UploadDocumentAttachment(fd, $scope.UserGUID);
            fileupload.success(function (response) {
                if (response && response.Data) {
                    $scope.$apply(function () {
                        $scope.uploadedFileList.push({ attchmentClickDocumentName: $scope.docName, batchId: response.Data[0].batchId, name: response.Data[0].name });
                        if ($scope.uploadedFileList.length === allfiles.length) {
                            $("#OrderDocumentQuickForm").val('');
                            notificationFactory.customSuccess("Document Uploaded Successfully");
                        }
                    });
                }
            });
        });
    };

    $scope.deleteUploadedDocument = function (index, batchId, fileName) {
        if (index > -1) {
            var deleteuploadDocument = QuickFormService.DeleteUploadedDocument(batchId, fileName, $scope.UserGUID);
            deleteuploadDocument.success(function (response) {
                if (response.Success) {
                    $scope.uploadedFileList.splice(index, 1);
                    notificationFactory.customSuccess("Document deleted Successfully");
                }
            });
            deleteuploadDocument.error(function (data, statusCode) {
            });
        }
    }

    $scope.saveDocumentAttachment = function () {
        var objDocumentAttchment = new Object();
        objDocumentAttchment.storedDcoumentList = angular.copy($scope.selectedDocumentList);
        objDocumentAttchment.uploadedFileList = angular.copy($scope.uploadedFileList);
        $scope.$parent.attchmentClickDocumentName = '';
        $scope.$emit('QuickFormDocumentAttchments', objDocumentAttchment);
        $scope.closeClick();
    }



    //#endregion
    $scope.declaration();
});