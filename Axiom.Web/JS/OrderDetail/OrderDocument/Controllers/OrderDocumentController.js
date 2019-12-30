app.controller('OrderDocumentController', function ($scope, $rootScope, $http, $state, $stateParams, notificationFactory, CommonServices, OrderDocumentService, configurationService, $compile, $filter, LocationServices) {
    decodeParams($stateParams);
    $scope.$on('OpenedFromClientOrderList', OpenedFromClientOrderList);
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.LocationID = "";
    $scope.IsLocSelect = false;
    $scope.RoleName = $rootScope.LoggedInUserDetail.RoleName ? $rootScope.LoggedInUserDetail.RoleName[0] : '';
    $scope.IsAttorneyLogin = ($scope.RoleName === 'Attorney');
    $scope.validateFileType = false;

    //#region Event

    $scope.GetFileList = function () {
        var promise = OrderDocumentService.GetFileList($scope.OrderId, $scope.PartNo);
        promise.success(function (response) {
            if ($scope.IsAttorneyLogin) {
                $scope.FileList = $filter('filter')(response.Data, { IsPublic: true }, true);
            }
            else {
                $scope.FileList = response.Data;
            }
            //bindFileList();
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.GetOtherFileList = function () {
        if ($scope.PartNo > 0) {
            var promise = OrderDocumentService.GetFileList($scope.OrderId, 0);
            promise.success(function (response) {
                $scope.OtherFileList = response.Data;
                bindFileDocument();
            });
            promise.error(function (data, statusCode) {
            });
        }
    };

    $scope.AddDocumentPopUp = function () {
        $scope.files = [];
        $scope.DocumentObj = new Object();
        $scope.DocumentObj.OrderId = parseInt($scope.OrderId);
        $scope.DocumentObj.PartNo = parseInt($scope.PartNo);
        $scope.DocumentObj.IsPublic = true;
        bindDropDown();
        $scope.Documentform.$setPristine();
        angular.element("#modal_Document").modal('show');
    };

    $scope.GetFileDetails = function (e) {
        $scope.$apply(function () {
            for (var i = 0; i < e.files.length; i++) {
                $scope.files.push(e.files[i]);
            }
        });
        angular.element("#file").val("");

    };
    $scope.RemoveFile = function (fileobj) {
        var index = $scope.files.indexOf(fileobj);
        $scope.files.splice(index, 1);
    };
    $scope.onFileTypeChange = function (FileTypeId) {
        if (FileTypeId) {
            $scope.validateFileType = false;
        } else {
            $scope.validateFileType = true;
        }
    }
    $scope.UploadDocument = function (form) {
        if (!$scope.DocumentObj.FileTypeId) {
            $scope.validateFileType = true;
            return;
        }

        if (form.$valid) {

            //if ($scope.DocumentObj.PartNo == 0 ) {
            //    if ($scope.files.length == 0) {
            //        toastr.error('Please Select File');
            //        return false;
            //    }
            //}
            //else {
            //    if ($scope.DocumentObj.FileTypeId != 11 && $scope.files.length == 0) {
            //        toastr.error('Please Select File');
            //        return false;
            //    }
            //}

            if ($scope.DocumentObj.PartNo > 0 && $scope.DocumentObj.FileTypeId == 11) {
                if (isNullOrUndefinedOrEmpty($scope.DocumentObj.PageNo) && $scope.DocumentObj.PageNo <= 0) {
                    toastr.error('Pages count should be greater than zero.');
                    return false;
                }
                if (isNullOrUndefinedOrEmpty($scope.DocumentObj.RecordTypeId) && $scope.DocumentObj.RecordTypeId == 0) {
                    toastr.error('Please Select RecordType.');
                    return false;
                }
            }
            $scope.FileTypeObj = $filter('filter')($scope.FileTypeList, { FileTypeId: $scope.DocumentObj.FileTypeId }, true);
            if ($scope.FileTypeObj[0].FileType.toLowerCase() == 'authorization' || $scope.FileTypeObj[0].FileType.toLowerCase() == 'return of service') {
                $scope.DocumentObj.IsPublic = true;
            }
            if (isNullOrUndefinedOrEmpty($scope.DocumentObj.PageNo)) {
                $scope.DocumentObj.PageNo = 0;
            }

            if ($scope.DocumentObj.FileTypeId == 11) {
                if (isNullOrUndefinedOrEmpty($scope.DocumentObj.RecordTypeId)) {
                    toastr.error("Select Record Type");
                    return false;
                }
            }
            else {
                $scope.DocumentObj.RecordTypeId = 0;
            }
            $scope.saveCheckTwoIsPublic($scope.FileTypeObj[0].FileType.toLowerCase());

            $scope.RecTypeObj = $filter('filter')($scope.RecTypeList, { Code: $scope.DocumentObj.RecordTypeId }, true);
            if ($scope.RecTypeObj && $scope.RecTypeObj.length > 0)
                $scope.DocumentObj.RecTypeName = $scope.RecTypeObj[0].Descr;

            $scope.DocumentObj.EmpId = $rootScope.LoggedInUserDetail.EmpId;
            $scope.DocumentObj.CreatedBy = $rootScope.LoggedInUserDetail.UserId;
            $scope.DocumentObj.UserAccessId = parseInt($rootScope.LoggedInUserDetail.UserAccessId);
            $scope.DocumentObj.IsAttorneyLogin = $scope.IsAttorneyLogin;
            var formData = new FormData();
            formData.append("model", angular.toJson($scope.DocumentObj));
            for (var i = 0; i < $scope.files.length; i++) {
                formData.append("file" + i, $scope.files[i]);
            }

            var fileupload = OrderDocumentService.UploadDocument(formData, $rootScope.CompanyNo);
            fileupload.success(function (response) {
                if (response.Success) {
                    toastr.success("Document Uploaded Successfully");
                    angular.element("#file").val('');
                    angular.element("#modal_Document").modal('hide');
                    $scope.files = [];
                    $scope.GetFileList();
                }
            });
        }
    };

    $scope.saveCheckTwoIsPublic = function (FileType) {
        if (!$scope.DocumentObj.IsPublic && FileType == 'adtnl') {
            bootbox.confirm({
                message: "This File Type is not normally made public.  Are you sure you want the Clients to see these files?",
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-success'
                    },
                    cancel: {
                        label: 'No',
                        className: 'btn-danger'
                    }
                },
                callback: function (result) {
                    return true;
                }
            });
        }
    };

    $scope.selectedtableRowDocument = function (fileDiskName, fileName) {
        if ($rootScope.LoggedInUserDetail.RoleName.contains("Administrator") || $rootScope.LoggedInUserDetail.RoleName.contains("Employee")
            || $rootScope.LoggedInUserDetail.RoleName.contains("DocumentAdmin")) {
            DownLoadFile(fileDiskName, fileName, $scope.OrderId, $scope.PartNo);
        }
        else {
            var isDownloadAccess = true;
            var chedkFileDownload = OrderDocumentService.CheckFileDownloadAccess($scope.OrderId, $scope.PartNo, $scope.userGuid);
            chedkFileDownload.success(function (data, status, headers) {
                debugger;
                if (data.str_ResponseData === "false") {
                    debugger;
                    isDownloadAccess = false;
                    toastr.error('You dont have enough rights to download documents of this part.');
                    return false;
                }
                else {
                    DownLoadFile(fileDiskName, fileName, $scope.OrderId, $scope.PartNo);
                }
            });
        }


    }
    function DownLoadFile(fileDiskName, fileName, OrderId, PartNo) {
        var promise = OrderDocumentService.DownloadFile(fileDiskName, fileName, OrderId, PartNo);
        promise.success(function (data, status, headers) {
            headers = headers();

            var filename = headers['x-filename'];
            var contentType = headers['content-type'];

            var linkElement = document.createElement('a');
            try {
                var blob = new Blob([data], { type: contentType });
                if (navigator.appVersion.toString().indexOf('.NET') > 0) {

                    window.navigator.msSaveBlob(blob, filename);
                }
                else {

                    var url = window.URL.createObjectURL(blob);

                    linkElement.setAttribute('href', url);
                    linkElement.setAttribute("download", filename);

                    var clickEvent = new MouseEvent("click", {
                        "view": window,
                        "bubbles": true,
                        "cancelable": false
                    });
                    linkElement.dispatchEvent(clickEvent);
                }



            } catch (ex) {
                console.log(ex);
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.FileStatusClick = function (currentStatus, fileId, $event) {
        bootbox.confirm({
            message: currentStatus ? "Are you sure you want to prevent clients from downloading this file?" : "Are you sure you want to allow clients to download this file?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                if (result) {
                    var promise = OrderDocumentService.UpdateFileStatus(fileId, !currentStatus);
                    promise.success(function (response) {
                        $scope.GetFileList();
                    });
                    promise.error(function (data, statusCode) {
                    });
                }
                bootbox.hideAll();
            }
        });
        $event.stopPropagation();
    }
    //#endregion

    //#region Method 

    function bindDropDown() {
        var _fileType = CommonServices.GetFileTypeDropdown();
        _fileType.success(function (response) {
            $scope.FileTypeList = response.Data;
        });
        _fileType.error(function (data, statusCode) {
        });
        var _RecType = CommonServices.RecordTypeDropDown();
        _RecType.success(function (response) {
            $scope.RecTypeList = response.Data;
        });
        _RecType.error(function (data, statusCode) {
        });
    }

    //function bindFileList() {

    //    if ($.fn.DataTable.isDataTable("#tblfile")) {
    //        $('#tblfile').DataTable().destroy();
    //    }

    //    var table = $('#tblfile').DataTable({
    //        data: $scope.FileList,
    //        "bDestroy": true,
    //        "dom": '<"top pull-left "><"table"rt><"bottom"lip<"clear">>',
    //        "aaSorting": false,
    //        "aLengthMenu": [10, 20, 50, 100, 200],
    //        "pageLength": 10,
    //        "stateSave": false,
    //        "autoWidth": false,
    //        "columns": [
    //            {
    //                "title": "FileName",
    //                "className": "dt-left",
    //                "data": "FileName",
    //                "width": "30%",
    //                "sorting": "false"
    //            },
    //            {
    //                "title": "FileType",
    //                "className": "dt-left",
    //                "data": "FileType",
    //                "width": "20%",
    //                "sorting": "false"
    //            },
    //            {
    //                "title": "Created Date",
    //                "data": "DtsCreated",
    //                "sorting": "false",
    //                "width": "10%",
    //                "render": function (data, type, row) {
    //                    return !isNullOrUndefinedOrEmpty(row.DtsCreated) ? $filter('date')(new Date(row.DtsCreated), $rootScope.GlobalDateFormat) : "";
    //                }
    //            },

    //            {
    //                "title": "Status",
    //                "className": "dt-center",
    //                "sorting": "false",
    //                "data": "IsPublic",
    //                "width": "7%",
    //                "render": function (data, type, row) {
    //                    //<a ng-click='ChangePrivacy(item.OrderDocumentId)' class='btn-link' title='Change'></a>
    //                    return (row.IsPublic) ? "<label class='label bg-success-400'>Private</label>" : "<label class='label bg-danger-400'>Public</label>";
    //                }
    //            }
    //            //{
    //            //    "title": 'Action',
    //            //    "data": null,
    //            //    "className": "action dt-center",
    //            //    "sorting": "false",
    //            //    "width": "7%",
    //            //    "render": function (data, type, row) {
    //            //        var strAction = '';
    //            //       // strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteDocument($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
    //            //        return strAction;
    //            //    }
    //            //}
    //        ],
    //        "initComplete": function () {
    //            var dataTable = $('#tblfile').DataTable();
    //        },
    //        "fnDrawCallback": function () {
    //        },
    //        "fnCreatedRow": function (nRow, aData, iDataIndex) {
    //            $compile(angular.element(nRow).contents())($scope);
    //        }
    //    });
    //}

    function bindFileDocument() {

        if ($.fn.DataTable.isDataTable("#tblOtherFile")) {
            $('#tblOtherFile').DataTable().destroy();
        }

        var table = $('#tblOtherFile').DataTable({
            data: $scope.OtherFileList,
            "bDestroy": true,
            "dom": '<"top pull-left "><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": [[0, 'desc']],
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "autoWidth": false,
            "columns": [
                {
                    "title": "FileName",
                    "className": "dt-left",
                    "data": "FileName",
                    "width": "30%",
                    "sorting": "false"
                },
                {
                    "title": "FileType",
                    "className": "dt-left",
                    "data": "FileType",
                    "width": "10%",
                    "sorting": "false"
                },
                {
                    "title": "Created Date",
                    "data": "DtsCreated",
                    "sorting": "false",
                    "width": "7%",
                    "render": function (data, type, row) {
                        return !isNullOrUndefinedOrEmpty(row.DtsCreated) ? $filter('date')(new Date(row.DtsCreated), $rootScope.GlobalDateFormat) : "";
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblOtherFile').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    function init() {
        $scope.OrderId = $stateParams.OrderId || $scope.$parent.OrderId;
        $scope.PartNo = (isNullOrUndefinedOrEmpty($stateParams.PartNo) ? 0 : $stateParams.PartNo) || $scope.$parent.PartNo || 0;
        if (!isNullOrUndefinedOrEmpty($scope.OrderId)) {
            $scope.GetFileList();
        }
        $scope.DocumentObj = new Object();
        $scope.DocumentObj.OrderId = parseInt($scope.OrderId);
        $scope.DocumentObj.PartNo = parseInt($scope.PartNo);
        $scope.DocumentObj.IsPublic = true;
        $scope.files = [];
    }

    function OpenedFromClientOrderList() {
        init();
        $scope.IsFromClientOrderList = true;
        $scope.AddDocumentPopUp();
    }


    //#endregion

    init();

});