app.controller('Step6Controller', function ($scope, $rootScope, $state, $stateParams, notificationFactory, AttorneyUserService, CommonServices, Step6Service, configurationService, $compile, $filter, LocationServices) {

    decodeParams($stateParams);

    $scope.CurrentState = $state.current.name;

    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    $scope.RushAllStatus = false;
    $scope.IsEdit = false;
    $scope.LocationID = "";
    $scope.IsAuthorizationAry = [{ Id: 0, Name: 'Select' }, { Id: true, Name: 'Authorization' }, { Id: false, Name: 'Subpoena Only' }];
    $scope.RequestMeansAry = [{ Id: 0, Name: 'Select' }, { Id: 1, Name: 'Create' }, { Id: 2, Name: 'Upload' }, { Id: 3, Name: 'Upload Batch' }];
    $scope.ConditionAry = [{ Id: 1, Name: 'Contains' }, { Id: 2, Name: 'Equal' }];

    $scope.PkgType = [{ Id: 1, Name: 'Basic' }, { Id: 2, Name: 'Advance (Choose three options)' }, { Id: 3, Name: 'Complete (Choose five options)' }];
    $scope.checkedCount = 0;
    $scope.maxlimit = 0;

    $scope.searchText = '';
    $scope.searchCriteria = 'Both';
    $scope.searchCondition = 1;
    $scope.ConditionAry = [{ Id: 1, Name: 'Contains' }, { Id: 2, Name: 'Equal' }];
    //$scope.InfomrationText = "Authorization will be created and sent to Patients Attorney.";
    $scope.UploadFileList = [];

    $scope.ErrorLocation = false;
    $scope.ErrorFileUpload = false;
    $scope.ErrorRecordSelect = false;
    $scope.ShowAddNewLocation = false;
    $scope.RoleName = $rootScope.LoggedInUserDetail.RoleName ? $rootScope.LoggedInUserDetail.RoleName[0] : '';    
    //------Add Location From Order List//----
    $scope.OrderDetailFromOrderList = new Object();
    $scope.$on('AddLocationFromOrderList', AddLocationFromOrderList);

    function AddLocationFromOrderList($event, orderData) {

        $scope.OrderDetailFromOrderList = orderData;

        if ($.fn.DataTable.isDataTable("#tblSearchLocationGrid")) {
            $('#tblSearchLocationGrid').DataTable().destroy();
        }
        InitFromOrderList($scope.OrderDetailFromOrderList);
        $scope.AddLocationPopup();
    }

    function InitFromOrderList(OrderDetail) {
        $scope.OrderId = OrderDetail.OrderId;
        //$scope.OrderStep6Obj = new Object();
        //$scope.LocationStep1 = true;
        //$scope.LocationStep2 = false;
        //$scope.LocationStep3 = false;
        //$scope.LocationStep4 = false;
        //$scope.NextLocatinStep = true;
        //$scope.PriviousLocatinStep = false;
        //$scope.StepCount = 1;
        GetOrderWizardStep3DOB($scope.OrderId);

    }

    function GetOrderWizardStep3DOB(OrderId) {
        var promise = Step6Service.GetOrderWizardStep3Details(OrderId);
        promise.success(function (response) {
            if (response && response.Data) {
                var objData = response.Data[0];
                if (!isNullOrUndefinedOrEmpty(objData.DateOfBirth)) {
                    $scope.scopeStDate = angular.copy($filter('date')(new Date(objData.DateOfBirth), $rootScope.GlobalDateFormat));
                }
            }
        });
        promise.error(function (data, statusCode) {
        });
    };
    //------Add Location From Order List//----

    $scope.UploadFile = function (e) {
        $scope.UserGUID = $rootScope.LoggedInUserDetail.UserId;
        var allfiles = $("#myfile")[0].files;
        angular.forEach(allfiles, function (item, index) {
            var fd = new FormData();
            fd.append("file", item);
            var fileupload = Step6Service.UploadNewOrderDocument(fd, $scope.UserGUID);
            fileupload.success(function (response) {
                if (response && response.Data) {
                    $scope.$apply(function () {
                        $scope.UploadFileList.push({ OrderNo: $scope.OrderId, PartNo: 0, LocID: '', CreatedBy: $scope.userGuid, RecordTypeId: 0, FileId: 0, BatchId: response.Data[0].batchId, FileName: response.Data[0].name, IsAuthSub: $scope.OrderStep6Obj.IsAuthorization });
                        if ($scope.UploadFileList.length === allfiles.length) {
                            $("#myfile").val('');
                            notificationFactory.customSuccess("Document Uploaded Successfully");
                        }
                    });
                }
            });
        });
    };


    $scope.deleteUploadedDocument = function (index, batchId, fileName, fileId, PartNo) {
        if (index > -1) {
            if (fileId > 0) {
                DeleteFileFromDB($scope.OrderId, PartNo, fileId);
            }
            var deleteuploadDocument = Step6Service.DeleteNewOrderDocument(batchId, fileName, $scope.UserGUID);
            deleteuploadDocument.success(function (response) {
                if (response.Success) {
                    $scope.UploadFileList.splice(index, 1);
                    notificationFactory.customSuccess("Document deleted Successfully");
                }
            });
            deleteuploadDocument.error(function (data, statusCode) {
            });
        }
    };
    $scope.UploadCanvasFile = function () {
        $("#canvasfile").click();
    }
    $scope.FileUploadClick = function () {
        $("#myfile").click();
    }
    function clearData() {
        $scope.UploadFileList = [];
    }

    $scope.AuthorizationChange = function () {
        //console.log($scope.OrderStep6Obj.IsAuthorization);
        //console.log($scope.OrderStep6Obj.RequestMeansId);

        if ($scope.OrderStep6Obj.IsAuthorization == true) { // AUTHORIZATION
            if ($scope.OrderStep6Obj.RequestMeansId == 1) // CREATE
                $scope.InfomrationText = "Authorization will be created and sent to Patients Attorney.";
            else if ($scope.OrderStep6Obj.RequestMeansId == 2) // UPLOAD
                $scope.InfomrationText = "Upload Signed Authorization to send with request.";
            else // UPLOAD BATCH
                $scope.InfomrationText = "Please upload on Documents tab. By selecting this option, you may incur additional cost.";

        }
        else {// SUBPOENA
            if ($scope.OrderStep6Obj.RequestMeansId == 1) // CREATE
            {
                $scope.InfomrationText = "No Authorization Needed.";
            }
            else if ($scope.OrderStep6Obj.RequestMeansId == 2) // UPLOAD
            {
                $scope.InfomrationText = "No Authorization Needed.";
            }
            else // UPLOAD BATCH
                $scope.InfomrationText = "No Authorization Needed. (Please upload on Documents tab.)";

        }

        if ($scope.OrderStep6Obj.IsAuthorization !== 0)
            $scope.ErrorAuthSubSelection = false;


        if ($scope.IsEdit == false) {
            if ($scope.OrderStep6Obj.IsAuthorization !== 0)
                $scope.showcreateUpload = true;
            else
                $scope.showcreateUpload = false;
        }
        

        if ($scope.OrderStep6Obj.RequestMeansId !== 0)
            $scope.ErrorCreateUploadSelection = false;


    };


    //#region Event
    $scope.$on('AddLocationFromOrderDetailPart', AddLocationFromOrderDetailPart);
    function AddLocationFromOrderDetailPart() {        
        $scope.AddLocationPopup();
    }
    $scope.AddLocationPopup = function () {

        $scope.IsEdit = false;
        $scope.OrderStep6Obj = new Object();
        $scope.OrderStep6Obj.RequestMeansId = 1;
        $scope.OrderStep6Obj.IsAuthorization = true;
        $("#LocatonStep4").removeClass('active');
        $("#LocatonStep3").removeClass('active');
        $("#LocatonStep2").removeClass('active');
        $("#LocatonStep1").addClass('active');
        $scope.PriviousLocatinStep = false;
        $scope.NextLocatinStep = true;
        $scope.LocationStep1 = true;
        $scope.LocationStep2 = false;
        $scope.LocationStep3 = false;
        $scope.LocationStep4 = false;
        if ($scope.CurrentState != "OrderList") {
            $scope.step6form.$setPristine();
        }
        $scope.Name1 = $scope.Name2 = "";
        $scope.showcreateUpload = false;
        clearData();
        angular.element("#modal_Add_Location").modal('show');
        $scope.OrderStep6Obj.IsAuthorization = 0;
        $scope.OrderStep6Obj.RequestMeansId = 0;
        var recordtype = CommonServices.RecordTypeDropDown();
        recordtype.success(function (response) {
            $scope.RecordTypeList = response.Data;
        });
        // bindDropDownList();
        $scope.LocationSearchPopup();
    };

    $scope.AddNewLocation = function () { //New Location Select
        $scope.LocationID = "";
        $scope.NewLocationObj = new Object();
        bindDropDownList();
        angular.element("#modal_New_Location").modal('show');
    };

    $scope.SaveNewLocation = function (form) {

        if (form.$valid) {

            var promise = Step6Service.InsertNewLocation($scope.NewLocationObj);
            promise.success(function (response) {
                if (response.Success) {
                    $scope.ResultArray = response.str_ResponseData.split(',');
                    $scope.LocationID = $scope.ResultArray[0];
                    $scope.Name1 = $scope.ResultArray[1];
                    $scope.Name2 = $scope.ResultArray[2];
                    // $scope.LocationID = response.str_ResponseData.split(',');
                }
            });
            promise.error(function (data, statusCode) {
            });
            angular.element("#modal_New_Location").modal('hide');
            angular.element("#modal_Location_Search").modal('hide');
        }
    };

    $scope.EditLocationPopup = function ($event) { //edit mode

        var tblStep6 = $('#tblStep6').DataTable();
        var data = tblStep6.row($($event.target).parents('tr')).data();
        $scope.IsEdit = true;
        $scope.showcreateUpload = true;
        bindDropDownList();
        var promise = Step6Service.GetOrderLocationById(data.PartNo, data.OrderId);
        promise.success(function (response) {           
            if (response.Success) {                
                $scope.step6form.$setPristine();
                $scope.OrderStep6Obj = response.Data[0];                
                $scope.LocationID = $scope.OrderStep6Obj.LocID;
                $scope.RecordType = $scope.OrderStep6Obj.RecordTypeId;
                if (!isNullOrUndefinedOrEmpty($scope.OrderStep6Obj.ScopeStartDate)) {
                    $scope.OrderStep6Obj.ScopeStartDate = $filter('date')(new Date($scope.OrderStep6Obj.ScopeStartDate), $rootScope.GlobalDateFormat);
                }
                if (!isNullOrUndefinedOrEmpty($scope.OrderStep6Obj.ScopeEndDate)) {
                    $scope.OrderStep6Obj.ScopeEndDate = $filter('date')(new Date($scope.OrderStep6Obj.ScopeEndDate), $rootScope.GlobalDateFormat);
                }
                $scope.Name1 = angular.copy($scope.OrderStep6Obj.Name1);
                $scope.Name2 = angular.copy($scope.OrderStep6Obj.Name2);

                clearData();
                $scope.OrderStep6Obj.PartNo = data.PartNo;
                $scope.OrderStep6Obj.IsOtherChecked = ($filter('filter')($scope.RecordTypeList, { 'IsLocationPageView': 1, 'Code': $scope.OrderStep6Obj.RecordTypeId }, true).length == 0);
                //$scope.OrderStep6Obj.RecordTypeId = 138;
                //$scope.OrderStep6Obj.IsOtherChecked = true;

                $scope.GetUploadedFileList($scope.OrderId, $scope.OrderStep6Obj.PartNo, $scope.OrderStep6Obj.LocID, $scope.OrderStep6Obj.RecordTypeId);
                angular.element("#modal_Add_Location").modal('show');
                LoadLocationTab();
            }
        });
        promise.error(function (data, statusCode) {
        });
    };
    function LoadLocationTab() {
        $("#LocatonStep2").removeClass('active');
        $("#LocatonStep3").removeClass('active');
        $("#LocatonStep4").removeClass('active');
        $("#LocatonStep1").addClass('active');
        $scope.ShowLocationSteps(1);
    }
    $scope.ShowLocationSteps = function (stepCount) {           
        if (stepCount == 1) {
            $scope.LocationStep1 = true;
            $scope.LocationStep2 = false;
            $scope.LocationStep3 = false;
            $scope.LocationStep4 = false;
            $scope.PriviousLocatinStep = false;
            $scope.NextLocatinStep = true;
        }
        else if (stepCount == 2) {
            if (step1Validation()) {
                $scope.LocationStep2 = true;
                $scope.LocationStep1 = false;
                $scope.LocationStep3 = false;
                $scope.LocationStep4 = false;
                $scope.StepCount = 1;
                $scope.PriviousLocatinStep = true;
                $scope.NextLocatinStep = true;
                setTimeout(function () { createDatePicker(); }, 500);
            } else {
                setTimeout(function () { $scope.PriviousLocationStep("Step2") }, 50);
            }
        }
        else if (stepCount == 3) {
            if (step1Validation() && step2Validation()) {
                $scope.LocationStep3 = true;
                $scope.LocationStep1 = false;
                $scope.LocationStep2 = false;
                $scope.LocationStep4 = false;
                $scope.NextLocatinStep = true;
                $scope.PriviousLocatinStep = true;
                $scope.StepCount = 2;
                CreateScope();

            } else {
                if (!step1Validation() && !step2Validation()) {
                    setTimeout(function () { $scope.PriviousLocationStep("Step2") }, 50);
                    return;
                }
                if (!step2Validation()) {
                    setTimeout(function () { $scope.PriviousLocationStep("Step3") }, 50);
                }
               
            }

        }
        else if (stepCount == 4) {
            var validStep1 = step1Validation() ? true : false;
            var validStep3 = (validStep1 && step2Validation()) ? true : false;
            if (validStep1 && validStep3) {
                CreateScope();
                $scope.LocationStep4 = true;
                $scope.LocationStep3 = false;
                $scope.LocationStep1 = false;
                $scope.LocationStep2 = false;
                $scope.NextLocatinStep = false;
                $scope.PriviousLocatinStep = true;
                $scope.StepCount = 3;
            }
            else if (!validStep1) {
                setTimeout(function () { $scope.PriviousLocationStep("Step2") }, 50);
            } else if (!validStep3) {
                setTimeout(function () { $scope.PriviousLocationStep("Step4") }, 50);
            }
        }
        $scope.StepCount = stepCount;

    }

    function CreateScope() {
        var promise = CommonServices.GetScopeForLocation($scope.OrderId.toString(), $scope.OrderStep6Obj.RecordTypeId.toString());
        promise.success(function (response) {
            if (response.Success) {                  
                if (isNullOrUndefinedOrEmpty($scope.OrderStep6Obj.Scope) || isNullOrUndefinedOrEmpty($scope.RecordType)) {
                    $scope.OrderStep6Obj.Scope = response.Data;
                }
                if ($scope.OrderStep6Obj.RecordTypeId!= $scope.RecordType){
                    $scope.OrderStep6Obj.Scope = response.Data;
                    $scope.RecordType = $scope.OrderStep6Obj.RecordTypeId;
                }
               
            }
        });
        promise.error(function (data, statusCode) {
        });
    }



    $scope.GetLocationSearch = function () {
        if (!isNullOrUndefinedOrEmpty($scope.searchText)) {
            if ($scope.searchText.length < 2) {
                toastr.error("Minimun two charater Required");
                return;
            }
            var promise = Step6Service.GetLocationListWithSearch($scope.searchCriteria, $scope.searchCondition, $scope.searchText, $scope.OrderId);
            promise.success(function (response) {
                $scope.searchLocationList = response.Data;
                bindLocationSerach();
            });
            promise.error(function (data, statusCode) {
            });
        }
        else {
            toastr.error("Search Text Required");
        }
        $scope.ShowAddNewLocation = true;
    };

    $scope.LocationSearchPopup = function () {
        $("#divLocationSearchGrid .top.pull-left").remove();
        $("#divLocationSearchGrid .bottom").remove();
        $('#tblSearchLocationGrid tbody').empty();
        angular.element("#modal_Location_Search").modal('show');
        $("#SearchText").focus();
        angular.element("#modal_Location_Search").find("input[type=text],input[type=search]").val('');
    };

    $scope.FillLocation = function ($event) {  //Fill detail to textbox from selected location
        debugger;
        var tblSearchLocation = $('#tblSearchLocationGrid').DataTable();
        var data = tblSearchLocation.row($($event.target).parents('tr')).data();
        $scope.LocationID = data.LocID;
        $scope.Name1 = data.Name1;
        $scope.Name2 = data.Name2;
        if (!isNullOrUndefinedOrEmpty(data.ReplacedBy)) {
            var promise = LocationServices.GetLocationById(data.ReplacedBy);
            promise.success(function (response) {                
                if (response.Success) {
                    debugger;
                    $scope.LocationID = response.Data[0].LocID;
                    $scope.Name1 = response.Data[0].Name1;
                    $scope.Name2 = response.Data[0].Name2;
                }
            });
            promise.error(function (statusCode) {
                $scope.LocationID = data.LocID;
                $scope.Name1 = data.Name1;
                $scope.Name2 = data.Name2;
            });
        }
        
        angular.element("#modal_Location_Search").modal('hide');
    };

    $scope.SubmitStep6 = function (form) {

        if (!isNullOrUndefinedOrEmpty($scope.LocationID)) {
            form.$submitted = true;

            if (form.$valid) {

                if (isNullOrUndefinedOrEmpty($scope.OrderStep6Obj.RecordTypeId) && (isNullOrUndefinedOrEmpty($scope.OrderStep6Obj.IsOtherChecked) || $scope.OrderStep6Obj.IsOtherChecked == false)) {
                    toastr.error("Select Record Type");
                    return;
                }
                else if (!isNullOrUndefinedOrEmpty($scope.OrderStep6Obj.IsOtherChecked)) {
                    if (isNullOrUndefinedOrEmpty($scope.OrderStep6Obj.RecordTypeId)) {
                        toastr.error("Select Record Type");
                        return;
                    }
                }

                $scope.OrderStep6Obj.EmpId = $scope.EmpId;
                $scope.OrderStep6Obj.UserAccessId = $scope.UserAccessId;
                $scope.OrderStep6Obj.LoggedInUserEmail = $rootScope.LoggedInUserDetail.Email;                
                $scope.OrderStep6Obj.OrderId = $scope.OrderId;
                $scope.OrderStep6Obj.LocID = $scope.LocationID;
                $scope.OrderStep6Obj.DocumentFileList = angular.copy($filter('filter')($scope.UploadFileList, { 'FileId': 0 }, true));
                $scope.OrderStep6Obj.CreatedBy = $scope.userGuid;

                $scope.OrderStep6Obj.RoleName = $scope.RoleName;

                $scope.OrderStep6Obj.Scope = $("#divScope").html()
                // $scope.OrderStep6Obj.Scope = $scope.OrderStep6Obj.Scope;
                $scope.UploadFileList = angular.copy($filter('filter')($scope.UploadFileList, { 'FileId': 0 }, true));
                angular.forEach($scope.UploadFileList, function (item, key) {
                    item.LocID = angular.copy($scope.OrderStep6Obj.LocID);
                });

                var promise = Step6Service.InsertOrUpdateOrderWizardStep6($scope.OrderStep6Obj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_Add_Location").modal('hide');
                        toastr.success("Location Save Successfully");
                        //GetDBDeletedFile();
                        if ($scope.CurrentState != "OrderList") {
                            $scope.GetOrderWizardStep6Location($scope.OrderId);
                        }
                        if ($scope.CurrentState == "OrderDetail") {
                            $scope.GetPartListByOrderId($scope.OrderId);
                        }

                    }
                });
                promise.error(function (data, statusCode) {
                });
                $scope.LocationID = "";
                $scope.NewLocationObj = new Object();
                $scope.newLocationform.$setPristine();
                $scope.Name1 = "";
                $scope.Name2 = "";
            }
        }
        else {
            toastr.error("Select Location");
        }
    };

    $scope.GetOrderWizardStep6Location = function (orderId) {
        var hideOldPart = false;

        if ($stateParams.AddPart) {
            hideOldPart = true;
            if ($rootScope.LoggedInUserDetail.RoleName.contains("Administrator") || $rootScope.LoggedInUserDetail.RoleName.contains("Employee")) {
                hideOldPart = false;    
            }
        }
        


        

        
        var promise = Step6Service.GetOrderWizardStep6Location(orderId, hideOldPart);
        promise.success(function (response) {
            if (response.Success) {
                $scope.LocationList = response.Data;
                var RushLocData = $filter('filter')(response.Data, { 'Rush': true }, true);
                if (RushLocData && RushLocData.length > 0 && RushLocData.length === response.Data.length) {
                    $scope.RushFound = true;
                }
                else
                    $scope.RushFound = false;

                setTimeout(function () {
                    $scope.$apply(function () {
                        if ($scope.RushFound)
                            $(".table-grid th.btn-custom-link").text("RUSH NONE");
                        else
                            $(".table-grid th.btn-custom-link").text("RUSH ALL");
                    });
                }, 500);
                bindOrderWazardStep6Location();
            }
        });
        promise.error(function (data, statusCode) {
        });

    };

    $scope.DeleteLocation = function ($event) {
        var tblStep6 = $('#tblStep6').DataTable();
        var data = tblStep6.row($($event.target).parents('tr')).data();
        var promise = Step6Service.DeleteOrderLocation(data.PartNo, data.OrderId, $scope.UserAccessId);
        promise.success(function (response) {
            if (response.Success) {
                tblStep6.row($($event.target).parents('tr')).remove();
                tblStep6.draw();
                toastr.success("Location Deleted Successfully");
            }
            else {
                toastr.success("TRY Again");
            }
        });
    };

    $scope.AddCanvasPopup = function () {
        $scope.files = [];
        $scope.Step6CanvasObj = new Object();
        $scope.step6Canvasform.$setPristine();
        angular.element("#modal_Canvas").modal('show');
        bindCanvasRequestMasterDropDown();
        $scope.Step6CanvasObj.PkgType = 1;

    };

    $scope.SaveCanvas = function (form) {
        if ($scope.files.length === 0) {
            $scope.canvasuploadError = true;
            return;
        }
        if (form.$valid) {
            if ($scope.Step6CanvasObj.PkgType == $rootScope.Enum.packgeType.Advance || $scope.Step6CanvasObj.PkgType == $rootScope.Enum.packgeType.Complete) {
                $scope.Step6CanvasObj.PkgVal = "";
                angular.forEach($scope.CanvasRequestMasterList, function (obj, arrayIndex) {
                    if (obj.Selected) {
                        $scope.Step6CanvasObj.PkgVal += obj.ID + ',';
                    }
                });
            }
            $scope.Step6CanvasObj.OrderId = $scope.OrderId;
            $scope.Step6CanvasObj.FileCount = $scope.files.length;
            $scope.Step6CanvasObj.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
            $scope.Step6CanvasObj.userGuid = $rootScope.LoggedInUserDetail.UserId;
            //var formData = new FormData();
            //formData.append("model", angular.toJson($scope.Step6CanvasObj));
            //for (var i = 0; i < $scope.files.length; i++) {
            //    formData.append("file" + i, $scope.files[i]);
            //}
            $scope.Step6CanvasObj.CanvasFileList = angular.copy($filter('filter')($scope.files, { 'FileId': 0 }, true));

            var promise = Step6Service.SaveOrderCanvasRequest($scope.Step6CanvasObj);
            promise.success(function (response) {
                if (response.Success) {
                    toastr.success("Canvas Save Successfully");
                    angular.element("#file").val('');
                    angular.element("#modal_Canvas").modal('hide');
                    $scope.files = [];
                    $scope.GetCanvasList();
                }
            });
        }
    };

    $scope.GetFileDetails = function (e) {
        $scope.UserGUID = $rootScope.LoggedInUserDetail.UserId;
        var allfiles = $("#canvasfile")[0].files;
        angular.forEach(allfiles, function (item, index) {
            var fd = new FormData();
            fd.append("file", item);
            var canvasfileupload = Step6Service.UploadNewOrderDocument(fd, $scope.UserGUID);
            canvasfileupload.success(function (response) {
                if (response && response.Data) {
                    $scope.$apply(function () {
                        $scope.files.push({ FileId: 0, BatchId: response.Data[0].batchId, FileName: response.Data[0].name });
                        if ($scope.files.length === allfiles.length) {
                            $("#canvasfile").val('');
                            notificationFactory.customSuccess("Document Uploaded Successfully");
                        }
                    });
                }
            });
        });
        $scope.canvasuploadError = false;
        //$scope.$apply(function () {
        //    for (var i = 0; i < e.files.length; i++) {
        //        $scope.files.push(e.files[i]);
        //    }
        //    $scope.canvasuploadError = false;
        //});
    };

    $scope.RemoveFile = function (index, batchId, fileName) {
        if (index > -1) {
            var deletecanvasuploadDocument = Step6Service.DeleteNewOrderDocument(batchId, fileName, $scope.UserGUID);
            deletecanvasuploadDocument.success(function (response) {
                if (response.Success) {
                    $scope.files.splice(index, 1);
                    notificationFactory.customSuccess("Document deleted Successfully");
                }
            });
            deletecanvasuploadDocument.error(function (data, statusCode) {
            });
        }
        //var index = $scope.files.indexOf(fileobj);
        //$scope.files.splice(index, 1);
    };

    $scope.GetCanvasList = function () {
        var promise = Step6Service.GetCanvasList($scope.OrderId);
        promise.success(function (response) {
            if (response.Success) {
                $scope.CanvasList = response.Data;
                bindCanvas();
            }
        });
    };

    $scope.DeleteCanvas = function ($event) {
        var tblCanvas = $('#tblCanvas').DataTable();
        var data = tblCanvas.row($($event.target).parents('tr')).data();
        var promise = Step6Service.DeleteCanvas(data.ID);
        promise.success(function (response) {
            if (response.Success) {
                tblCanvas.row($($event.target).parents('tr')).remove();
                tblCanvas.draw();
                toastr.success("Canvas Delete Successfully");
            }
            else {
                toastr.success("TRY Again");
            }
        });
    };

    $scope.ChangePkgType = function () {
        $scope.IsDisblePkgTyped = false;
        $scope.checkedCount = 0;
        angular.forEach($scope.CanvasRequestMasterList, function (item, key) {
            item.Selected = false;
        });
        //for (var i = 0; i < $scope.CanvasRequestMasterList.length; i++) {
        //    $scope.CanvasRequestMasterList[i].Selected = false;
        //}
        if ($scope.Step6CanvasObj.PkgType == $rootScope.Enum.packgeType.Advance) {
            $scope.maxlimit = 3;
        }
        if ($scope.Step6CanvasObj.PkgType == $rootScope.Enum.packgeType.Complete) {
            $scope.maxlimit = 5;
        }
    };

    $scope.SelectPkgType = function (item) {
        if (item.Selected) {
            $scope.checkedCount++;
            item.Selected = true;
        } else {
            $scope.checkedCount--;
            item.Selected = false;
        }
    };
    //#endregion

    //#region Methodd

    function bindDropDownList() {
        var promise = CommonServices.LocationDepartmentDropDown();
        promise.success(function (response) {
            if (response.Success) {
                $scope.departmentlist = response.Data;
            }
        });
        promise.error(function (data, statusCode) {
        });

        var state = CommonServices.StateDropdown();
        state.success(function (response) {
            $scope.StateList = response.Data;
        });
        var recordtype = CommonServices.RecordTypeDropDown();
        recordtype.success(function (response) {
            $scope.RecordTypeList = response.Data;
        });
    }

    function bindCanvasRequestMasterDropDown() {
        var _canvas = CommonServices.GetCanvasRequestMasterDropDown();
        _canvas.success(function (response) {
            $scope.CanvasRequestMasterList = response.Data;
            for (var i = 0; i < $scope.CanvasRequestMasterList.length; i++) {
                $scope.CanvasRequestMasterList[i].Selected = false;
            }
        });
    }

    function bindLocationSerach() {
        if ($.fn.DataTable.isDataTable("#tblSearchLocationGrid")) {
            $('#tblSearchLocationGrid').DataTable().destroy();
        }
        var table = $('#tblSearchLocationGrid').DataTable({
            data: $scope.searchLocationList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "oLanguage": {
                "sSearch": "Refine Search:"
            },
            "columns": [
                //{
                //    "title": "Location Id",
                //    "className": "dt-left",
                //    "data": "LocID",
                //    "sorting": "false",
                //    "width": "10%"
                //},
                {
                    "title": "Location",
                    "className": "dt-left",
                    //"data": "Name1",                    
                    "sorting": "false",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = row.Name1 + "</br>" + row.Street1 + ' ' + row.Street2 + ' <b>' + row.City + '</b> ' + '<b>' + row.State + '</b>';
                        return strAction;
                    }
                },
                {
                    "title": "Doctor",
                    "className": "dt-left",
                    "data": "Name2",
                    "sorting": "false"
                },
                {
                    "title": "Department",
                    "className": "dt-left",
                    "data": "Dept",
                    "width": "20%",
                    "sorting": "false"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "8%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='FillLocation($event)' title='Add' data-toggle='tooltip' data-placement='left' tooltip> <i  class='icon-add cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblSearchLocationGrid').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $(nRow).attr("ng-dblclick", "FillLocation($event)");
                $compile(angular.element(nRow))($scope);
                // $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function bindOrderWazardStep6Location() {

        RushCallCount = 0;
        if ($.fn.DataTable.isDataTable("#tblStep6")) {
            $('#tblStep6').DataTable().destroy();
        }
        var table = $('#tblStep6').DataTable({
            data: $scope.LocationList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            //            "aaSorting": false,
            "aaSorting": [[0, 'desc']],
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Location Id",
                    "className": "dt-left",
                    "data": "LocID",
                    "sorting": "false",
                    "width": "10%"
                },
                {
                    "title": "Name",
                    "className": "dt-left",
                    "data": "Name1",
                    "sorting": "false"
                },
                {
                    "title": "Doctor Name",
                    "className": "dt-left",
                    "data": "Name2",
                    "sorting": "false"
                },
                {
                    "title": "Department",
                    "className": "dt-left",
                    "data": "Dept",
                    "width": "15%",
                    "sorting": "false"
                },
                {
                    "title": "RUSH ALL",
                    "className": "dt-center btn-custom-link",
                    "data": "Rush",
                    "width": "7%",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        var partNo = row.PartNo;
                        return (row.Rush) ? "<a class='label bg-danger-400' data-toggle='tooltip' data-placement='left' tooltip title='Click to remove from Rush' ng-click='LocRushClick(" + partNo + ",false);'>Yes</a>" : "<a class='label bg-success-400' title='Click to add this Location Rush' ng-click='LocRushClick(" + partNo + ",true);'>No</a>";
                    }

                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "8%",
                    "render": function (data, type, row) {
                        
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='EditLocationPopup($event)' data-toggle='tooltip' data-placement='left' tooltip title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        if (row.AllowDelete) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteLocation($event)' data-toggle='tooltip' data-placement='left' tooltip  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        }                        
                        return strAction;
                    }
                }
            ],
            "fnInitComplete": function () {
                DefaultInitialization();
            },
            "fnDrawCallback": function () {

            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {

                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    var RushCallCount = 0;
    function DefaultInitialization() {
        $('#tblStep6').on('click', 'th', function () {

            RushCallCount++;
            if (RushCallCount == 1 && $(this).text() == "RUSH ALL") {

                $scope.RushAllStatus = true;
                $scope.AllRush();
            }
            else if (RushCallCount == 1 && $(this).text() == "RUSH NONE") {

                //$scope.RushAllStatus = $scope.RushAllStatus ? false : true;
                $scope.RushAllStatus = false;
                $scope.AllRush();
                //$(this).text('Rush All');


            }
        });
    }




    function bindCanvas() {
        if ($.fn.DataTable.isDataTable("#tblCanvas")) {
            $('#tblCanvas').DataTable().destroy();
        }
        var table = $('#tblCanvas').DataTable({
            data: $scope.CanvasList,
            "bDestroy": true,
            "dom": '<"top pull-left "><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [

                {
                    "title": "Package",
                    "className": "dt-left",
                    "data": "PkgName",
                    "sorting": "false"
                },
                {
                    "title": "Description",
                    "className": "dt-left",
                    "data": "PkgDescription",
                    "sorting": "false"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "8%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='DeleteCanvas($event)' title='Add' data-toggle='tooltip' data-placement='left' tooltip> <i  class='icon-bin cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblCanvas').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);

            }
        });

    }


    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.OrderStep6Obj = new Object();
        $scope.LocationStep1 = true;
        $scope.LocationStep2 = false;
        $scope.LocationStep3 = false;
        $scope.LocationStep4 = false;
        $scope.NextLocatinStep = true;
        $scope.PriviousLocatinStep = false;
        $scope.StepCount = 1;
        if (!isNullOrUndefinedOrEmpty($scope.OrderId)) {
            GetOrderWizardStep3DOB($scope.OrderId);
        }
        //  $scope.GetOrderWizardStep6Location($scope.OrderId);  Sometimes, grid is not destroyed automatically so call the function on grid ng-init
    }

    $scope.GetUploadedFileList = function (orderId, partNo, LocId, RecTypeId) {
        var promise = Step6Service.GetLocationTempFiles(orderId, partNo);
        promise.success(function (response) {
            //response.Data = $filter('filter')(response.Data, { 'FileTypeId': 3 }, true);
            angular.forEach(response.Data, function (item, index) {
                $scope.UploadFileList.push({ OrderNo: $scope.OrderId, PartNo: partNo, LocID: LocId, CreatedBy: $scope.userGuid, RecordTypeId: RecTypeId, FileId: item.ID, BatchId: item.BatchId, FileName: item.FileName, IsAuthSub: item.IsAuthSub });
            });
            $scope.StoredFileList = angular.copy($scope.UploadFileList);
        });
        promise.error(function (data, statusCode) {
        });
    };
    //#endregion

    //function GetDBDeletedFile() {
    //    angular.forEach($scope.StoredFileList, function (storeditem, index) {
    //        var obj = $filter('filter')($scope.UploadFileList, { 'FileId': storeditem.FileId }, true);
    //        if (obj && obj.length == 0) {
    //            DeleteFileFromDB(storeditem.FileId);
    //        }
    //    });
    //}
    function DeleteFileFromDB(OrderNo, PartNo, FileId) {
        var deleteuploadDocument = Step6Service.DeleteDBUploadedFile(OrderNo, PartNo, FileId);
        deleteuploadDocument.success(function (response) {
        });
        deleteuploadDocument.error(function (data, statusCode) {
        });
    }

    $scope.LocRushClick = function (partNo, isRush) {
        var updateRush = Step6Service.UpdateOrderWizardStep6LocRush($scope.OrderId, partNo, isRush);
        updateRush.success(function (response) {
            if (response.Success) {
                $scope.GetOrderWizardStep6Location($scope.OrderId);
            }
        });
        updateRush.error(function (data, statusCode) {
        });
    }

    $scope.PriviousLocationStep = function (currentStep) {
        if (currentStep == "Step4") {
            $("#LocatonStep4").removeClass('active');
            $("#LocatonStep1").removeClass('active');
            $("#LocatonStep2").removeClass('active');
            $("#LocatonStep3").addClass('active');
            $scope.LocationStep2 = false;
            $scope.LocationStep1 = false;
            $scope.LocationStep4 = false;
            $scope.LocationStep3 = true;
            $scope.NextLocatinStep = true;
            $scope.PriviousLocatinStep = true;
        }
        else if (currentStep == "Step3") {
            $("#LocatonStep4").removeClass('active');
            $("#LocatonStep3").removeClass('active');
            $("#LocatonStep1").removeClass('active');
            $("#LocatonStep2").addClass('active');
            $scope.LocationStep2 = true;
            $scope.LocationStep1 = false;
            $scope.LocationStep3 = false;
            $scope.LocationStep4 = false;
            $scope.NextLocatinStep = true;
            $scope.PriviousLocatinStep = true;
            setTimeout(function () { createDatePicker(); }, 500);
        }
        else if (currentStep == "Step2") {
            $("#LocatonStep4").removeClass('active');
            $("#LocatonStep2").removeClass('active');
            $("#LocatonStep3").removeClass('active');
            $("#LocatonStep1").addClass('active');
            $scope.LocationStep1 = true;
            $scope.LocationStep2 = false;
            $scope.LocationStep3 = false;
            $scope.PriviousLocatinStep = false;
            $scope.NextLocatinStep = true;
        }

    }
    function step1Validation() {
        $scope.ErrorLocation = false;
        $scope.ErrorFileUpload = false;
        if (!$scope.Name1 && !$scope.Name2) {
            $scope.ErrorLocation = true;
            // toastr.error('Please Select Location.');
            return false;
        }
        else if ($scope.OrderStep6Obj.RequestMeansId === 2 && $scope.UploadFileList.length == 0) {
            $scope.ErrorFileUpload = true;
            // toastr.error('Please Upload atleast one file.');
            return false;
        }
        else if ($scope.OrderStep6Obj.IsAuthorization === 0) {
            $scope.ErrorAuthSubSelection = true;
            return false;
        }
        else if ($scope.OrderStep6Obj.RequestMeansId === 0) {
            $scope.ErrorCreateUploadSelection = true;
            return false;
        }
        return true;
    }
    //function step3Validation() {
    function step2Validation() {
        $scope.ErrorRecordSelect = false;
        $scope.filterRecordTypeList = $filter('filter')($scope.RecordTypeList, { 'IsLocationPageView': 1 }, true);
        var isMatched = false;
        angular.forEach($scope.filterRecordTypeList, function (item) {
            if (item.Code === $scope.OrderStep6Obj.RecordTypeId) {
                isMatched = true;
            }
        });
        if (!$scope.OrderStep6Obj.IsOtherChecked && !isMatched) {
            $scope.OrderStep6Obj.RecordTypeId = "";
        }
        if (!$scope.OrderStep6Obj.RecordTypeId && !$scope.OrderStep6Obj.IsOtherChecked) {
            // toastr.error('Please Select Record Type.');
            $scope.ErrorRecordSelect = true;
            return false;
        }
        else if ($scope.OrderStep6Obj.IsOtherChecked && !$scope.OrderStep6Obj.RecordTypeId) {
            $scope.ErrorRecordSelect = true;
            // toastr.error('Please Select Other Record Type.');
            return false;
        }
        return true;
    }
    $scope.NextLocationStep = function (currentStep) {

        if (currentStep === "Step1" && step1Validation()) {
            $("#LocatonStep1").removeClass('active');
            $("#LocatonStep3").removeClass('active');
            $("#LocatonStep4").removeClass('active');
            $("#LocatonStep2").addClass('active');
            $scope.LocationStep2 = true;
            $scope.LocationStep1 = false;
            $scope.LocationStep3 = false;
            $scope.LocationStep4 = false;
            $scope.PriviousLocatinStep = true;
            setTimeout(function () { createDatePicker(); }, 500);
            $scope.OrderStep6Obj.ScopeStartDate = $scope.scopeStDate;
            $scope.OrderStep6Obj.ScopeEndDate = angular.copy($filter('date')(new Date(), $rootScope.GlobalDateFormat));
        }
        else if (currentStep === "Step2" && step2Validation()) {
            if (!isNullOrUndefinedOrEmpty($scope.OrderStep6Obj.ScopeStartDate) && !isNullOrUndefinedOrEmpty($scope.OrderStep6Obj.ScopeEndDate)) {
                var ScopeStartDate = new Date($scope.OrderStep6Obj.ScopeStartDate);
                var ScopeEndDate = new Date($scope.OrderStep6Obj.ScopeEndDate);
                if (ScopeStartDate > ScopeEndDate) {
                    toastr.error('Scope End Date must be greater than Scope Start Date.');
                    return;
                }
            }
            $("#LocatonStep1").removeClass('active');
            $("#LocatonStep2").removeClass('active');
            $("#LocatonStep4").removeClass('active');
            $("#LocatonStep3").addClass('active');
            $scope.PriviousLocatinStep = true;
            $scope.LocationStep3 = true;
            $scope.LocationStep1 = false;
            $scope.LocationStep2 = false;
            $scope.LocationStep4 = false;
            $scope.NextLocatinStep = true;
            $scope.PriviousLocatinStep = true;
            CreateScope();
        }
        else if (currentStep === "Step3") {

            $("#LocatonStep3").removeClass('active');
            $("#LocatonStep2").removeClass('active');
            $("#LocatonStep1").removeClass('active');
            $("#LocatonStep4").addClass('active');
            $scope.PriviousLocatinStep = true;
            $scope.LocationStep4 = true;
            $scope.LocationStep3 = false;
            $scope.LocationStep1 = false;
            $scope.LocationStep2 = false;
            $scope.NextLocatinStep = false;
            $scope.PriviousLocatinStep = true;

        }

    }




    $scope.AllLocRushClick = function ($event) {

    }

    $(".btnRushAllClass").on('click', function (event) {
        alert(1);
    });
    $scope.AllRush = function () {
        var promise = Step6Service.RushAllLocation($scope.OrderId, $scope.RushAllStatus);
        promise.success(function (response) {
            if (response.Success) {

                var lblName = $scope.RushAllStatus ? "RUSH NONE" : "RUSH ALL";
                setTimeout(function () {
                    $scope.$apply(function () {
                        $(".table-grid th.btn-custom-link").text(lblName);
                    });
                }, 500);
                $scope.GetOrderWizardStep6Location($scope.OrderId);
                notificationFactory.customSuccess("Rush Updated Successfully");
            }
        });
        promise.error(function (data, statusCode) {
        });
    };
    if ($scope.CurrentState != "OrderList") {
        init();
    }


});