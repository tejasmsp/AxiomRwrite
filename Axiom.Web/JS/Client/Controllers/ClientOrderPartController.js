app.controller('ClientOrderPartController', function ($scope, $window, $rootScope, $stateParams, notificationFactory, ClientServices, CommonServices, configurationService, $compile, $filter) {

    $scope.OrderPart = {};
    $scope.PartList = [];
    $scope.SelectedPart = [];
    var objFileLst = [];
    $scope.$parent.$watch('ShowOrderPartList', function (newVal, oldVal) {
        if (newVal) {
            $scope.OrderPart = {};
            $scope.PartList = [];
            $scope.SelectedPart = [];
            $scope.SelectedOrder = angular.copy($scope.$parent.SelectedOrderNo);
            $scope.SelectedPartGroup = angular.copy($scope.$parent.PartStatusGroupId);
            $scope.GetOrderParts($scope.SelectedOrder, $scope.SelectedPartGroup);
        }
    }, true);

    //#region Events
    $scope.OpenPartDetailPage = function (OrderNo, PartNo) {
        var downloadLink = document.createElement("a");
        downloadLink.href = 'PartDetail?OrderId=' + OrderNo + '&PartNo=' + PartNo;
        downloadLink.target = '_blank';
        // document.body.appendChild(downloadLink);
        downloadLink.click();

    }

    $scope.GetOrderParts = function (OrderNo, SelectedPartGroup) {
        var promise = ClientServices.GetClientDashboardParts(OrderNo, SelectedPartGroup);
        promise.success(function (response) {
            if (response.Success) {
                $scope.PartList = angular.copy(response.Data);
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.selectAllPart = function (isSelected) {
        if (!isSelected) {
            $scope.SelectedPart = [];
        }

        angular.forEach($scope.PartList, function (item, key) {
            if (isSelected) {
                var index = $scope.SelectedPart.indexOf(item.PartNo);
                if (index < 0)
                    $scope.SelectedPart.push(item.PartNo);
            }
            item.isChecked = $scope.OrderPart.allItemsSelected;
        });
    };

    $scope.SelectedOrderPart = function (isSelected, partNo) {
        if (isSelected)
            $scope.SelectedPart.push(partNo);
        else {
            var index = $scope.SelectedPart.indexOf(partNo);
            if (index > -1) {
                $scope.SelectedPart.splice(index, 1);
            }
        }
        for (var i = 0; i < $scope.PartList.length; i++) {
            if (!$scope.PartList[i].isChecked) {
                $scope.OrderPart.allItemsSelected = false;
                return;
            }
        }
        $scope.OrderPart.allItemsSelected = true;

    }

    DownloadDoc = function (fileTypeId) {
        debugger;
        if ($scope.SelectedPart.length === 0) {
            toastr.error("Please select Part to Download.");
        }
        else {
            angular.forEach($scope.SelectedPart, function (partNo, key) {
                objFileLst.push({ "OrderNo": $scope.SelectedOrder, "PartNo": partNo, "FileTypeID": fileTypeId });

                //var promise = ClientServices.GetClientFileByFileType($scope.SelectedOrder, partNo, fileTypeId);
                //promise.success(function (response) {
                //    if (response.Success && response.Data && response.Data.length > 0) {
                //        angular.forEach(response.Data, function (item, key) {
                //            objFileLst.push({ "FileDiskName": item.FileDiskName, "FileName": item.FileName, "OrderNo": $scope.SelectedOrder, "PartNo": partNo });
                //            // DownloadFile(item.FileDiskName, item.FileName, partNo);
                //        });
                //    }
                //});
                //promise.error(function (data, statusCode) {
                //});
            });
            DownloadFileList(objFileLst);
        }
    }

    $scope.DownloadDocuments = function (fileTypeId) {
        objFileLst = [];
        if ($scope.SelectedPart.length === 0) {
            toastr.error("Please select Part to Download.");
        }
        else {
            angular.forEach($scope.SelectedPart, function (partNo, key) {
                objFileLst.push({ "OrderNo": $scope.SelectedOrder, "PartNo": partNo, "FileTypeID": fileTypeId });
            });
            DownloadFileList(objFileLst);
        }
    }


    function DownloadFileList(objFileList) {
        
        var promise = ClientServices.DownloadFileMultiple(objFileList);
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

    function DownloadFile(fileDiskName, fileName, partNo) {
        var promise = ClientServices.DownloadFile(fileDiskName, fileName, $scope.SelectedOrder, partNo);
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
    $scope.ClientDashboardRemovePartFromList = function (partNo) {
        var promise = ClientServices.ClientDashboardRemovePartFromList($scope.SelectedOrder, partNo);
        promise.success(function (response) {
            if (response.Success) {
                $scope.GetOrderParts($scope.SelectedOrder, $scope.SelectedPartGroup);
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.CloseModal = function () {
        $scope.$parent.ShowOrderPartList = false;
        angular.element("#modal_DashboardOrderPartForm").modal('hide');
    }
    //#endregion
});