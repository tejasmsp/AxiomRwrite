app.controller('QuickFormController', function ($scope, $rootScope, $state, $stateParams, QuickFormService, notificationFactory, Step6Service, CommonServices, configurationService, $compile, $filter) {

    //#region Declarations
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.UserEmployeeID = $rootScope.LoggedInUserDetail.EmpId;
    $scope.UserGUID = $rootScope.LoggedInUserDetail.UserId;

    $scope.declaration = function () {
        $scope.partDetailList = [];
        $scope.OrderNo = $stateParams.OrderId;
        $scope.PartNo = $stateParams.PartNo;
        decodeParams($stateParams);
        $scope.isshowPartSection = true;
        $scope.isshowDocumentSection = false;
        $scope.isshowDocumentAttachmentSection = false;
        $scope.showPrintDocQueue = false;
        $scope.showDocumentTreeNext = false;
        $scope.quickform = { 'OrderNo': $scope.OrderNo };
        $scope.documentQueueList = [];
        $scope.selectedPartList = [];
        $scope.documentQueueAttachmentList = [];
        $scope.SelectedemailByDocNameList = [];
        $scope.storedDcoumentList = [];
        $scope.uploadedFileList = [];
        $scope.showAllDocs = [];
        $scope.displayAllDocList = [];
        $scope.deletedAttachedDocList = [];
        $scope.isshowDocumentTreeSection = false;
        $scope.FormsAndDirectoryList = [];
        $scope.selectedTreeFileList = [];
        $scope.selectedTreeAllFileList = [];
        $scope.selectedChkBoxEmailLst = [];
        $scope.selectedChkBoxFaxLst = [];
        $scope.selectedChkBoxPrintLst = [];
        $scope.attorneyFormList = [];
        $scope.showAttorneyForm = false;
        $scope.showAttorneyFormListSection = false;
        $scope.nextviaAtty = true;
        $scope.selectedAttorneyFormList = [];
        $scope.selectedDocumnetFolder = [];
        $scope.chkGenerateDoc = true;
        $scope.chkdontuploadDoc = true;


    }
    //#endregion


    $scope.$parent.$watch('ShowQuickForm', function (newVal, oldVal) {
        if (newVal) {
            $scope.ShowQuickFormPopup();
        }
    }, true);

    $scope.ShowQuickFormPopup = function () {        
        angular.element("#modal_QuickForm").modal();
        $scope.declaration();
        // clearFirmSearch();
        $scope.QuickFormGetPartDetail();
        $scope.$parent.ShowQuickForm = false;
    };

    $scope.QuickFormGetPartDetail = function () {
        var promise = QuickFormService.QuickFormGetPartDetail($scope.OrderNo);
        promise.success(function (response) {
            if (response.Success) {
                $scope.partDetailList = response.Data;
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    function init() {
        var formsList = QuickFormService.QuickFormGetFormList();
        formsList.success(function (response) {
            if (response.Success) {
                if (response.Data && response.Data.length > 0) {
                    $scope.fromList = angular.copy(response.Data);
                }

            }
        });
        formsList.error(function (data, statusCode) {
        });


        var fileTypeList = QuickFormService.QuickFormGetFileList();
        fileTypeList.success(function (response) {
            if (response.Success) {
                if (response.Data && response.Data.length > 0) {
                    $scope.fileTypeList = angular.copy(response.Data);

                }
            }
        });
        fileTypeList.error(function (data, statusCode) {
        });

    };
    function GetFormsAndDirectoryList() {

        var promise = QuickFormService.GetFormsAndDirectoryList();
        promise.success(function (response) {
            response.Data[0].children = $filter('filter')(response.Data[0].children, { 'breadcrumbs': $scope.quickform.SelectedFormItem }, true);
            $scope.FormsAndDirectoryList = angular.copy(response.Data[0].children);
            $scope.selectedDocumnetFolder = [];
            $scope.selectedTreeFileList = [];
        });
        promise.error(function (data, statusCode) {
        });
    }

    //#endregion

    //#region Events
    $scope.selectedtableRow = function (item) {
        var isselected = $("#chk-" + item.PartNo).is(":checked");
        isselected = !isselected;
        $("#chk-" + item.PartNo).prop('checked', isselected);
        SelectedPartDataList(isselected, item.PartNo);
    }

    $scope.selectAllItems = function (isselectedAll) {
        if ($scope.partDetailList) {
            angular.forEach($scope.partDetailList, function (value, index) {
                $scope.partDetailList[index].isChecked = isselectedAll;
            });
        }

    }
    $scope.SelectAllParts = function ($event) {
        $scope.selectedPartList = [];
        if ($event.target.checked) {
            $('input[rel=quickFormCheckBox]').each(function () {                
                this.checked = true;
                $scope.selectedPartList.push($(this).val());
            });
        }
        else {            
            $('input[rel=quickFormCheckBox]').each(function () {
                
                this.checked = false;
                var index = $scope.selectedPartList.indexOf($(this).val());
                if (index > -1) {
                    $scope.selectedPartList.splice(index, 1);
                }
            });
        }
    }
    $scope.selectedOrder = function (partNo, $event) {
        $event.stopPropagation();
        var isselected = $("#chk-" + partNo).is(":checked");
        SelectedPartDataList(isselected, partNo);

    }
    function SelectedPartDataList(isselected, partNo) {
        if (!isselected) {
            $scope.isselectedAll = false;
            var index = $scope.selectedPartList.indexOf(partNo);
            if (index > -1) {
                $scope.selectedPartList.splice(index, 1);
            }
        }
        else {
            $scope.selectedPartList.push(partNo);
            $scope.selectedPartNo = partNo;
        }
    }
    $scope.nextClick = function () {

        if ($scope.isshowPartSection && (!$scope.selectedPartList || $scope.selectedPartList.length === 0)) {
            $scope.showNextAttchemnt = false;
            toastr.warning("Select atleast one part.");
            return;
        }
        else {
            $scope.isshowPartSection = false;
            $scope.isshowDocumentSection = true;
            $scope.showPrintDocQueue = false;
            $scope.isshowDocumentTreeSection = false;
            $scope.documentQueueList = [];
            init();
            $scope.showNextAttchemnt = true;
            $scope.showDocumentTreeNext = $scope.quickform.SelectedFormItem && $scope.quickform.SelectedFormItem.length > 0 ? true : false;
            $scope.chkNoticeStatus = $scope.chkCWCStatus = $scope.chkConfirmStatus = $scope.chkFaceSheetStatus = $scope.chkRequestStatus =
                $scope.chkAuthorizationStatus = $scope.chkFaxCoverStatus = false;
            if ($scope.quickform.SelectedFormItem && $scope.quickform.SelectedFormItem.toUpperCase() === "ATTORNEY FORMS")
                $scope.showDocumentTreeNext = false;
        }

    }
    $scope.selectFormItemChange = function (item) {
        if (item && item.length > 0) {
            if (item.toUpperCase() === "ATTORNEY FORMS") {
                $scope.showAttorneyFormListSection = true;
                $scope.showDocumentTreeNext = false;
            }
            else {
                $scope.showAttorneyFormListSection = false;
                $scope.showDocumentTreeNext = true;
            }
        }
        else {
            $scope.showAttorneyFormListSection = false;
            $scope.showDocumentTreeNext = false;
        }
    }
    $scope.nextToAttorneyForm = function () {
        $scope.showDocumentTreeNext = true;
        $scope.showAttorneyForm = true;
        $scope.QuickFormGetAttorneyFormDetail();
        $scope.selectedAttorneyFormList = [];
    }
    $scope.nextToDocumentTree = function () {
        if (!$scope.quickform.FileTypeID) {
            toastr.warning("Please Select File Type.");
            return;
        }
        $scope.nextviaAtty = $scope.showAttorneyForm ? true : false;
        if ($scope.nextviaAtty && (!$scope.selectedAttorneyFormList || $scope.selectedAttorneyFormList.length === 0)) {
            toastr.warning("Select atleast one attorney.");
            return;
        }
        $scope.showAttorneyForm = false;
        GetFormsAndDirectoryList();
        $scope.showDocumentTreeNext = false;
        $scope.isshowPartSection = false;
        $scope.isshowDocumentSection = false;
        $scope.isshowDocumentAttachmentSection = false;
        $scope.isshowDocumentTreeSection = true;
    }
    $scope.nexttoAttachment = function () {        
        $scope.documentQueueAttachmentList = angular.copy($scope.documentQueueList);
        $scope.isshowPartSection = false;
        $scope.isshowDocumentSection = false;
        $scope.isshowDocumentTreeSection = false;
        $scope.showDocumentTreeNext = false;
        $scope.isshowDocumentAttachmentSection = true;
        $scope.selectedChkBoxEmailLst = [];
        $scope.selectedChkBoxFaxLst = [];
        $scope.selectedChkBoxPrintLst = [];
        $scope.showAttorneyForm = false;
        $scope.nextviaAtty = false;
    }
    function getDocumentList(docType) {
        debugger;
        var documentList = QuickFormService.QuickFormGetDocumentListByType(docType, $scope.OrderNo, $scope.selectedPartList.toString());
        documentList.success(function (response) {
            if (response.Success) {
                debugger;
                angular.forEach(response.Data, function (item, index) {
                    debugger;
                    if (docType === 'Requests' || docType === 'Authorizations') {
                        var objDocDetail = item.DocumentName.split("_");
                        debugger;
                        if (objDocDetail && objDocDetail.length > 0) {
                            debugger;
                            var partNo = objDocDetail[1];
                            if (partNo) {
                                var objPartDetail = $filter('filter')($scope.partDetailList, { 'PartNo': parseInt(partNo) }, true);
                                if (objPartDetail && objPartDetail.length > 0) {
                                    $scope.documentQueueList.push({ 'DocumentType': docType, 'DocumentPath': item.DocumentPath, 'DocumentName': item.DocumentName, 'FaxNo': '', 'SendRequest': objPartDetail[0].SendRequest, 'Email': objPartDetail[0].Email, 'docAttachList': [], 'Part': partNo, 'AttorneyId': '', 'OrinaldocName': item.DocumentName })
                                }
                            }
                        }
                    }
                    else {
                        debugger;
                        $scope.documentQueueList.push({ 'DocumentType': docType, 'DocumentPath': item.DocumentPath, 'DocumentName': item.DocumentName, 'FaxNo': '', 'SendRequest': "", 'Email': "", 'docAttachList': [], 'Part': partNo, 'AttorneyId': '', 'OrinaldocName': item.DocumentName })
                    }
                });
            }
        });
        documentList.error(function (data, statusCode) {
        });
    }
    $scope.closeClick = function () {
        $scope.isshowPartSection = false;
        $scope.isshowDocumentSection = false;
        $scope.isshowDocumentAttachmentSection = false;
        $scope.isshowDocumentTreeSection = false;
        angular.element("#modal_QuickForm").modal('hide');
    }
    $scope.backtoFormSelectionClick = function () {
        $scope.showAttorneyForm = false;
        $scope.isshowDocumentSection = true;
        $scope.showDocumentTreeNext = false;
        $scope.isshowDocumentTreeSection = false;

    }
    $scope.backtoAttyFormSelectionClick = function () {
        $scope.showAttorneyForm = true;
        $scope.showDocumentTreeNext = true;
        $scope.isshowDocumentTreeSection = false;
    }
    $scope.backToPartClick = function () {
        $scope.isshowPartSection = true;
        $scope.isshowDocumentSection = false;
        $scope.isshowDocumentAttachmentSection = false;
        $scope.isshowDocumentTreeSection = false;
        $scope.showDocumentTreeNext = false;
        $scope.showPrintDocQueue = false;
        $scope.showNextAttchemnt = false;
        $scope.chkConfirmStatus = $scope.chkRequestStatus = $scope.chkAuthorizationStatus = $scope.chkFaxCoverStatus = false;
    }
    $scope.backToDocumentClick = function () {
        $scope.isshowPartSection = false;
        $scope.isshowDocumentSection = true;
        $scope.isshowDocumentAttachmentSection = false;
        $scope.isshowDocumentTreeSection = false;
        $scope.showDocumentTreeNext = $scope.quickform.SelectedFormItem && $scope.quickform.SelectedFormItem.length > 0 ? true : false;
    }


    $scope.checkedDocumentType = function (docType, status) {

        $scope.documentType = docType;
        if (status && (docType === 'Notices' || docType === 'CWC' || docType === 'Facesheets')) {
            $scope.ShowQuickFormDocumentPopup = true;

        }
        if (docType === 'Confirmations' || docType === 'Requests' || docType === 'Authorizations' || docType === 'FaxCoverSheet') {
            $scope.showPrintDocQueue = true;
            if (status) {
                getDocumentList(docType);
            }
            else if (!status) {
                removeItemFromDocumentTable(docType);
            }
        }
        if (!status && (docType === 'Notices' || docType === 'CWC' || docType === 'Facesheets')) {
            removeItemFromDocumentTable(docType);
        }



    }
    function removeItemFromDocumentTable(docType) {
        var newdocList = $scope.documentQueueList.filter(function (x) { return x.DocumentType !== docType; });
        $scope.documentQueueList = angular.copy(newdocList);
        if ($scope.documentQueueList.length === 0) {
            $scope.showPrintDocQueue = false;
        }
    }
    $scope.$on('QuickFormParent', function (event, data) {


        if (data && data.selectedAttorneyList && data.selectedAttorneyList.length > 0) {
            angular.forEach(data.selectedAttorneyList, function (attorneyitem, attorneyindex) {
                var docPath = data.DocumentPath;
                if (data.DocumentType === 'Facesheets') {
                    angular.forEach($scope.selectedPartList, function (selectedPartNo, index) {
                        var documentName = $scope.OrderNo + '_' + selectedPartNo + '_' + attorneyitem.AttorneyId + '_' + data.DocumentName;
                        $scope.documentQueueList.push({ 'DocumentType': data.DocumentType, 'DocumentPath': docPath, 'DocumentName': documentName, 'FaxNo': attorneyitem.FaxNo, 'SendRequest': "", 'Email': "", 'docAttachList': [], 'Part': selectedPartNo, 'AttorneyId': attorneyitem.AttorneyId, 'OrinaldocName': data.DocumentName });
                    });
                }
                else {
                    $scope.selectedPartList = $scope.selectedPartList.sort();
                    var partNos = $scope.selectedPartList.join('-');
                    var documentName = $scope.OrderNo + '_' + partNos + '_' + attorneyitem.AttorneyId + '_' + data.DocumentName;
                    $scope.documentQueueList.push({ 'DocumentType': data.DocumentType, 'DocumentPath': docPath, 'DocumentName': documentName, 'FaxNo': attorneyitem.FaxNo, 'SendRequest': "", 'Email': "", 'docAttachList': [], 'Part': partNos, 'AttorneyId': attorneyitem.AttorneyId, 'OrinaldocName': data.DocumentName });
                }


            });
        }

    });

    $scope.$on('QuickFormDocumentAttchments', function (event, data) {
        if (data) {
            $scope.storedDcoumentList.push.apply($scope.storedDcoumentList, data.storedDcoumentList);
            $scope.uploadedFileList.push.apply($scope.uploadedFileList, data.uploadedFileList);
            $scope.showAllDocument();
        }
    });

    $scope.deleteDocumentQueueClick = function (docType, docName, removeFrom) {
        if (removeFrom === 'documentList') {
            angular.forEach($scope.documentQueueList, function (item, index) {
                if (item.DocumentType === docType && item.DocumentName === docName) {
                    $scope.documentQueueList.splice(index, 1);
                }
            });
        }
        else if (removeFrom === "documentAttachList") {
            angular.forEach($scope.documentQueueAttachmentList, function (item, index) {

                if (item.DocumentType === docType && item.DocumentName === docName) {
                    $scope.documentQueueAttachmentList.splice(index, 1);
                }
            });
        }

    }

    $scope.showAllDocument = function (docName) {
        angular.forEach($scope.documentQueueAttachmentList, function (attchitem, index) {
            angular.forEach($scope.storedDcoumentList, function (storeitem, index) {
                if (attchitem.DocumentName == storeitem.attchmentClickDocumentName) {
                    var doc = $filter('filter')(attchitem.docAttachList, { FileName: storeitem.FileName }, true);
                    var deletedDoc = $filter('filter')($scope.deletedAttachedDocList, { FileName: storeitem.FileName }, true);
                    if (doc.length === 0 && deletedDoc.length === 0) {
                        attchitem.docAttachList.push({ FileName: storeitem.FileName, FileDiskName: storeitem.FileDiskName, BatchId: '', UploadType: 2 });
                    }
                }
            });
            angular.forEach($scope.uploadedFileList, function (uploaditem, index) {
                if (attchitem.DocumentName == uploaditem.attchmentClickDocumentName) {
                    var doc = $filter('filter')(attchitem.docAttachList, { FileName: uploaditem.name }, true);
                    var deletedDoc = $filter('filter')($scope.deletedAttachedDocList, { FileName: uploaditem.FileName }, true);
                    if (doc.length === 0 && deletedDoc.length === 0) {
                        attchitem.docAttachList.push({ FileName: uploaditem.name, FileDiskName: '', BatchId: uploaditem.batchId, UploadType: 3 });
                    }
                }
            });

        });
    }
    $scope.selectNode = function (currentNode) {
        if (currentNode && currentNode.isfolder) {
            $scope.selectedDocumnetFolder.push({ "title": currentNode.title, "fullpath": currentNode.fullpath });
            angular.forEach(currentNode.children, function (item, index) {
                if (item.isfolder === false) {
                    var obj = $filter('filter')($scope.selectedTreeAllFileList, { Title: item.title }, true);
                    if (obj && obj.length == 0) {
                        $scope.selectedTreeAllFileList.push({ Title: item.title, breadcrumbs: currentNode.breadcrumbs });
                    }
                }
            });
            currentNode.children = currentNode.children.filter(function (obj) {
                return obj.isfolder === true
            });

            $scope.selectedTreeFileList = $scope.selectedTreeAllFileList.filter(function (obj) {
                return obj.breadcrumbs === currentNode.breadcrumbs
            });
            $('#treeDoc .normal').addClass('expanded').removeClass('normal');
        }
    }
    $scope.finishTreeClick = function () {
        if (!$scope.quickform.selectedDocFile) {
            toastr.warning("Please select a Document.");
            return;
        }
        $scope.selectedPartSortedList = $scope.selectedPartList.sort(function (a, b) { return a - b })
        if ($scope.quickform.selectedDocFile && $scope.quickform.selectedDocFile.length > 0) {
            $scope.fileName = $scope.quickform.selectedDocFile[0];
        }
        var currentPath = ($scope.selectedDocumnetFolder && $scope.selectedDocumnetFolder.length > 0) ? $scope.selectedDocumnetFolder[0].fullpath : "";
        debugger;
        var objpdf = new Object();
        objpdf.OrderNo = $scope.OrderNo;
        objpdf.Years = null;
        objpdf.Pages = 0;
        objpdf.FolderPath = currentPath;
        objpdf.Fullpath = ($scope.selectedDocumnetFolder && $scope.selectedDocumnetFolder.length > 0) ? $scope.selectedDocumnetFolder[$scope.selectedDocumnetFolder.length - 1].fullpath : "";
        objpdf.FileName = $scope.fileName;
        objpdf.FileTypeId = $scope.quickform.FileTypeID;
        objpdf.RecordTypeId = 0;
        objpdf.isPublic = true;
        objpdf.EnableDocStorage = $scope.chkdontuploadDoc;
        var PartNumbers = $scope.selectedPartSortedList.toString();
        objpdf.PartIds = $scope.selectedPartSortedList.toString();
        delete $scope.ObjPdfList;
        $scope.ObjPdfList = [];
        if ($scope.chkGenerateDoc) {
            if ($scope.selectedAttorneyFormList && $scope.selectedAttorneyFormList.length > 0) {
                if ($scope.nextviaAtty) {
                    angular.forEach($scope.selectedAttorneyFormList, function (attyitem, key) {
                        debugger;
                        objpdf.AttysIds = attyitem.AttId;
                        var NewObject = Object();
                        NewObject = angular.copy(objpdf);
                        $scope.ObjPdfList.push(NewObject);

                    });
                }
                else {
                    angular.forEach($scope.selectedPartSortedList, function (partitem, key) {
                        angular.forEach($scope.selectedAttorneyFormList, function (attyitem, key) {
                            debugger;
                            objpdf.AttysIds = attyitem.AttId;
                            objpdf.PartIds = partitem;
                            var NewObject = Object();
                            NewObject = angular.copy(objpdf);
                            $scope.ObjPdfList.push(NewObject);
                        });
                    });
                }
            }
            else {
                debugger;
                angular.forEach($scope.selectedPartSortedList, function (partItem, key) {
                    debugger;
                    objpdf.PartIds = partItem;
                    objpdf.AttysIds = "";
                    var NewObject = Object();
                    NewObject = angular.copy(objpdf);
                    $scope.ObjPdfList.push(NewObject);

                });

            }
        }
        else {
            objpdf.AttysIds = $scope.selectedAttorneyFormList && $scope.selectedAttorneyFormList.length > 0 ? $scope.selectedAttorneyFormList[0].AttId : "";            
            objpdf.EmpId = $scope.UserEmployeeID;
            objpdf.UserId = $scope.UserGUID;
            var result = QuickFormService.QuickFormGetPdf(objpdf);
        }

        $scope.GetQuickformPdf($scope.ObjPdfList);
    }

    $scope.GetDocumentRootPath = function (item) {
        var currentPath = QuickFormService.GetDocumentRootPath();
        currentPath.success(function (response) {
            $scope.currentPath = response;

            //$scope.selectedPartList;
            var objpdf = new Object();
            objpdf.OrderNo = $scope.OrderNo;
            objpdf.Years = null;
            objpdf.Pages = 0;
            objpdf.FolderPath = $scope.currentPath + '\\' + item.DocumentPath;
            objpdf.Fullpath = $scope.currentPath + '\\' + item.DocumentPath;
            objpdf.FileName = item.OrinaldocName;
            objpdf.FileTypeId = 0;
            objpdf.RecordTypeId = 0;
            objpdf.isPublic = true;
            objpdf.EnableDocStorage = false;
            objpdf.PartIds = !isNullOrUndefinedOrEmpty(item.Part) ? item.Part : 0;
            objpdf.AttysIds = item.AttorneyId;
            // $scope.GetQuickformPdf(objpdf);
            objpdf.EmpId = $scope.UserEmployeeID;
            objpdf.UserId = $scope.UserGUID;
            objpdf.CompNo = $rootScope.CompanyNo;
            var result = QuickFormService.QuickFormGetPdf(objpdf);
        });
        currentPath.error(function (data, statusCode) {
        });
    }
    $scope.PrintOpenDocument = function (item) {
        debugger;
        if (item!=null) {
            item.CompNo = $rootScope.CompanyNo;
            $scope.GetDocumentRootPath(item);
            $scope.ShowQuickForm = true; 
        }
        
    }
    $scope.GetQuickformPdf = function (objpdfList) {
        for (var c = 0; c < objpdfList.length; c++) {            
            objpdfList[c].EmpId = $scope.UserEmployeeID;
            objpdfList[c].UserId = $scope.UserGUID;
            var result = QuickFormService.QuickFormGetPdf(objpdfList[c]);
        }
    }
    $scope.finishClick = function () {
        debugger;
        var saveQuickformList = [];

        var ObjpartDetail = $filter('filter')($scope.partDetailList, { PartNo: $scope.PartNo }, true);

        angular.forEach($scope.documentQueueAttachmentList, function (item, index) {
            debugger;
            var IsSSN = false;
            var fileTypeId = 0;
            if (item.DocumentType === "Notices") {
                IsSSN = $scope.noticeradioStatus;
                fileTypeId = GetFileTypeId().NOTICES;
            }
            else if (item.DocumentType === "CWC") {
                IsSSN = $scope.radioCWCStatus;
                fileTypeId = GetFileTypeId().CWC;
            }
            else if (item.DocumentType === "Confirmations") {
                IsSSN = $scope.radioConfirmStatus;
                fileTypeId = GetFileTypeId().CONFIRMATIONS;
            }
            else if (item.DocumentType === "Facesheets") {
                IsSSN = $scope.radioFaceSheetStatus;
                fileTypeId = GetFileTypeId().FACESHEETS;
            }
            else if (item.DocumentType === "Requests") {
                IsSSN = $scope.radioRequestStatus;
                fileTypeId = GetFileTypeId().REQUESTS;
            }
            else if (item.DocumentType === "Authorizations") {
                IsSSN = $scope.radioAuthorizationStatus;
                fileTypeId = GetFileTypeId().AUTHORIZATIONS;
            }
            else if (item.DocumentType === "FaxCoverSheet") {
                IsSSN = $scope.radioFaxCoverSheetStatus;
                fileTypeId = GetFileTypeId().FAXCOVERSHEET;
            }
            var selectedEmail = '';
            if ($scope.SelectedemailByDocNameList && item.DocumentType !== "Requests") {
                var objSelected = $filter('filter')($scope.SelectedemailByDocNameList, { SelectedDocumentName: item.DocumentName }, true);
                if (objSelected && objSelected.length > 0) {
                    selectedEmail = objSelected[0].SelectedEmail;
                };
            }
            var objSave = new Object();

            //Email
            if ($scope.selectedChkBoxEmailLst && $scope.selectedChkBoxEmailLst.length > 0) {
                var objSelectedEmail = $filter('filter')($scope.selectedChkBoxEmailLst, { DocName: item.DocumentName }, true);
                if (objSelectedEmail && objSelectedEmail.length > 0)
                    objSave.IsEmail = 1;
                else
                    objSave.IsEmail = 0;
            }
            else
                objSave.IsEmail = 0;

            //Fax
            if ($scope.selectedChkBoxFaxLst && $scope.selectedChkBoxFaxLst.length > 0) {
                var objSelectedFax = $filter('filter')($scope.selectedChkBoxFaxLst, { DocName: item.DocumentName }, true);
                if (objSelectedFax && objSelectedFax.length > 0)
                    objSave.IsFax = 1;
                else
                    objSave.IsFax = 0;
            }
            else
                objSave.IsFax = 0;

            //Print
            if ($scope.selectedChkBoxPrintLst && $scope.selectedChkBoxPrintLst.length > 0) {
                var objSelectedPrint = $filter('filter')($scope.selectedChkBoxPrintLst, { DocName: item.DocumentName }, true);
                if (objSelectedPrint && objSelectedPrint.length > 0)
                    objSave.IsPrint = 1;
                else
                    objSave.IsPrint = 0;
            }
            else
                objSave.IsPrint = 0;

            objSave.OrderNo = $scope.OrderNo;
            if (item.DocumentType === "Requests" || item.DocumentType === "Authorizations" ) {
                objSave.PartNo = item.Part;
            }
            else {
                objSave.PartNo = $scope.selectedPartList.toString();
            }

            
            objSave.DocName = item.DocumentName;
            objSave.DocPath = item.DocumentPath;
            objSave.Status = 0;
            objSave.FileTypeID = $scope.quickform.FileTypeID;
            objSave.RecordTypeID = 0
            objSave.Pages = 0
            objSave.IsPublic = 1
            objSave.UserId = $scope.UserGUID;
            objSave.Year1 = null;
            objSave.Year2 = null;
            objSave.Year3 = null;
            objSave.Year4 = null;
            objSave.Year5 = null;
            objSave.Year6 = null;
            objSave.Year7 = null;
            objSave.Year8 = null;
            objSave.printStatus = 0;
            objSave.faxStatus = 0;
            objSave.EmailStatus = 0;
            objSave.Email = item.DocumentType === "Requests" ? item.Email : selectedEmail;
            objSave.FaxNo = item.FaxNo;
            objSave.Name = "";
            objSave.Address = "";
            objSave.IsSSN = IsSSN;
            objSave.IsRevised = $scope.requestOption;
            objSave.isFromClient = 0;
            objSave.CreateDocument = 0;
            objSave.OrdBy = $scope.UserEmployeeID;
            objSave.ClaimNo = (ObjpartDetail && ObjpartDetail.length > 0) ? ObjpartDetail[0].BillingClaimNo : "";
            objSave.BillFirm = (ObjpartDetail && ObjpartDetail.length > 0) ? ObjpartDetail[0].BillingFirmId : "";
            objSave.AttyName = (ObjpartDetail && ObjpartDetail.length > 0) ? ObjpartDetail[0].AttyName : "";
            objSave.documentFileList = item.docAttachList;
            objSave.FileTypeID = fileTypeId;
            saveQuickformList.push(objSave);
        });

        var promise = QuickFormService.QuickFormInsert(saveQuickformList);
        promise.success(function (response) {
            if (response.Success) {
                $scope.closeClick();
                notificationFactory.customSuccess("Quick Form saved successfullly.");
            }
            else {
                toastr.error(response.Message[0]);
            }
        });
        promise.error(function (data, statusCode) {
        });
    }
    $scope.attachmentClick = function (docname) {
        $scope.ShowQuickFormDocAttachment = true;
        $scope.attchmentClickDocumentName = docname;
        angular.element("#modal_QuickdocumentAttachmentForm").modal('show');
    }
    $scope.selectDocAttachmentClick = function (documentType, lblchecked, status, documentName) {

        $scope.ShowQuickFormAttorneyList = false;
        if (documentType != 'Requests' && status) {

            if (lblchecked === "Email")
                $scope.selectedChkBoxEmailLst.push({ 'DocName': documentName });
            if (lblchecked === "Fax")
                $scope.selectedChkBoxFaxLst.push({ 'DocName': documentName });

            $scope.lblchecked = angular.copy(lblchecked);
            $scope.selectedDocumentName = documentName;
            $scope.ShowQuickFormAttorneyList = true;
            angular.element("#modal_QuickAttorneyForm").modal('show');
        }
        else {
            if (!status) {
                if (lblchecked === "Email")
                    $scope.removeItemFromEmailList(documentName);
                if (lblchecked === "Fax")
                    $scope.removeItemFromFaxList(documentName);
            }
        }
    }
    $scope.removeItemFromEmailList = function (item) {
        angular.forEach($scope.selectedChkBoxEmailLst, function (value, key) {
            if (value.DocName === item) {
                $scope.selectedChkBoxEmailLst.splice(key, 1);
            }
        });
    }
    $scope.removeItemFromFaxList = function (item) {
        angular.forEach($scope.selectedChkBoxFaxLst, function (value, key) {
            if (value.DocName === item) {
                $scope.selectedChkBoxFaxLst.splice(key, 1);
            }
        });
    }
    $scope.chkprintStatusClick = function (status, documentName) {
        if (status) {
            $scope.selectedChkBoxPrintLst.push({ 'DocName': documentName });
        }
        else {
            var index = $scope.selectedChkBoxPrintLst.indexOf(documentName);
            if (index > -1) {
                $scope.selectedChkBoxPrintLst.splice(index, 1);
            }
        }
    }
    $scope.deleteAttachedDocuments = function (fileName, index) {

        angular.forEach($scope.documentQueueAttachmentList, function (item, itemIndex) {
            angular.forEach(item.docAttachList, function (docAttachitem, docAttachindex) {
                if (docAttachitem.FileName == fileName && index == docAttachindex) {
                    item.docAttachList.splice(index, 1);
                    $scope.deletedAttachedDocList.push({ FileName: fileName });
                }
            });
        });
    }
    $scope.QuickFormGetAttorneyFormDetail = function () {

        var promise = QuickFormService.QuickFormGetOrderingAttorney($scope.OrderNo);
        promise.success(function (response) {
            if (response.Success && response.Data) {
                $scope.attorneyFormList = [];
                angular.forEach(response.Data, function (item, index) {
                    var attId = item.AttyID.replace(/ +/g, "");
                    $scope.attorneyFormList.push({ attIdwithoutSpace: attId, AttorneyType: item.AttorneyType, AttyID: item.AttyID, Email: item.Email, FaxNo: item.FaxNo, FirstName: item.FirstName, LastName: item.LastName, Represent: item.Represent });
                });

            }
        });
        promise.error(function (data, statusCode) {
        });
    }
    $scope.selectedAttorneyFormRowDocument = function (index) {

        var objAttorneyData = angular.copy($scope.attorneyFormList[index]);
        if (objAttorneyData) {
            var attId = objAttorneyData.AttyID.replace(/ +/g, "");
            var status = $("#chkatty-" + attId).is(":checked");
            status = !status;
            $("#chkatty-" + attId).prop('checked', status);
            SelectedAttorneyformDataList(status, attId, objAttorneyData);
        }
    }
    $scope.selectedAttorneyFormData = function (index, $event) {
        $event.stopPropagation();
        var objAttorneyData = angular.copy($scope.attorneyFormList[index]);
        if (objAttorneyData) {
            var attId = objAttorneyData.AttyID.replace(/ +/g, "");
            var status = $("#chkatty-" + attId).is(":checked");
            SelectedAttorneyformDataList(status, attId, objAttorneyData);
        }
    }
    function SelectedAttorneyformDataList(status, attId, objAttorneyData) {
        if (status) {
            $scope.selectedAttorneyFormList.push({ 'AttId': attId, 'AttorneyId': objAttorneyData.AttyID, 'AttorneyType': objAttorneyData.AttorneyType, 'FirstName': objAttorneyData.FirstName, 'LastName': objAttorneyData.LastName, 'FaxNo': objAttorneyData.FaxNo });
        }
        else {
            $scope.selectedAttorneyFormList = $scope.selectedAttorneyFormList.filter(function (obj) {
                return obj.AttId !== attId
            });
        }
    }
    //#endregion
    $scope.declaration();


});