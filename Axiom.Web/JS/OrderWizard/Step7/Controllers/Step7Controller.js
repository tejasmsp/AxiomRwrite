app.controller('Step7Controller', function ($scope, $rootScope, $state, $stateParams, notificationFactory, Step7Service, CommonServices, CourtService, Step1Service, Step4Service, configurationService, $compile, $filter) {
    decodeParams($stateParams);
    
    //#region Event
    $scope.GetDocumentList = function () {
        var promise = Step7Service.GetDocumentList($scope.OrderId,0);
        promise.success(function (response) {
            $scope.DocumentList = response.Data;
            bindDocumentList();
        });
        promise.error(function (data, statusCode) {
        });
    };

    $scope.DeleteDocument = function (OrderDocumentId) {
        bootbox.confirm({
            message: "Are you sure you want to delete this record ?",
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
                    var promise = Step7Service.DeleteOrderDocument(OrderDocumentId);
                    promise.success(function (response) {
                        $scope.DocumentList = null;
                        $scope.GetDocumentList();
                        bindDocumentList();
                        toastr.success("Document deleted successfully.");
                    });
                    promise.error(function (data, statusCode) {
                    });
                }
                bootbox.hideAll();
            }
        });

      
    };


    $scope.AddOrUpdateDocument = function (form) {
        debugger;
        if ($scope.step7form.$valid) {
            var fd = new FormData();
            if ($("#OrderDocument")[0].files[0] == undefined)
            {
                notificationFactory.customError("Please Select Document");
                return true;
            }           
            fd.append("file", $("#OrderDocument")[0].files[0]);
            var fileupload = Step7Service.UploadOrderDocument(fd, $scope.OrderId, $scope.EmpId, $scope.UserAccessId);
            fileupload.success(function (response) {
                $scope.GetDocumentList();
                $("#OrderDocument").val('');
                notificationFactory.customSuccess("Document Uploaded Successfully");
                bindDocumentList()
            });
        }
    };
    //#endregion

    //#region Method

    function bindDocumentList() {

        if ($.fn.DataTable.isDataTable("#tblDocument")) {
            $('#tblDocument').DataTable().destroy();
        }

        var table = $('#tblDocument').DataTable({
            data: $scope.DocumentList,
            "bDestroy": true,
            "dom": '<"top pull-left "><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "NO",
                    "className": "action dt-center",
                    "data": "rowNumber",
                    "sorting": "false",
                    "width": "5%"
                },
                {
                    "title": "Title",
                    "className": "dt-left",
                    "data": "Title",
                    "sorting": "false"
                },        
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        strAction = "<a class='ico_btn cursor-pointer' ng-click='DeleteDocument(" + row.OrderDocumentId +")' data-toggle='tooltip' data-placement='top' tooltip title='Delete'><i class='icon-trash cursor-pointer'></i> </a>";
                        strAction += "<a class='ico_btn cursor-pointer' target='_blank' href='" + row.FilePath +"' title='Download' data-toggle='tooltip' data-placement='top' tooltip><i class='icon-download cursor-pointer'></i> </a>";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblDocument').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    function init() {
        $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
        $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
        $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
        $scope.OrderId = $stateParams.OrderId;
        $scope.OrderStep7Obj = new Object();
        $scope.OrderStep7Obj.OrderId = $scope.OrderId;
        $scope.GetDocumentList();

    }
    //#endregion

    init();

});