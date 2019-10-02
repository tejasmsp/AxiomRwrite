app.controller('MessageController', function ($scope, $rootScope, $stateParams, notificationFactory, MessageServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Message", "View");
    $scope.IsUserCanEditMessage = $rootScope.isSubModuleAccessibleToUser('Settings', 'Message', 'Edit Message');
    $scope.IsUserCanDeleteMessage = $rootScope.isSubModuleAccessibleToUser('Settings', 'Message', 'Delete Message');
    //------------

    function bindMessageList() {
        if ($.fn.DataTable.isDataTable("#tblMessage")) {
            $('#tblMessage').DataTable().destroy();
        }

        var table = $('#tblMessage').DataTable({
            data: $scope.Messagelist,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Name",
                    "className": "dt-left",
                    "data": "Name",
                    "sorting": "false",
                    "width": "15%"
                },
                {
                    "title": "Custom Message",
                    "className": "dt-left",
                    "data": "CustomMessage",
                    "sorting": "false"
                },
                {
                    "title": "Status",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsActive",
                    "width": "7%",
                    "render": function (data, type, row) {
                        return (row.IsActive) ? "<label class='label bg-success-400'>Active</label>" : "<label class='label bg-danger-400'>InActive</label>";
                    }
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "7%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if ($scope.IsUserCanEditMessage) {
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='EditMessage($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }
                        if ($scope.IsUserCanDeleteMessage ) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteMessage($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        }                                              
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblMessage').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });

    }

    function GetMessagelist() {
        var promise = MessageServices.GetCustomMessageList();
        promise.success(function (response) {
            $scope.Messagelist = response.Data;
            bindMessageList();
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.AddMessage = function () {
        $scope.MessageObj = new Object();
        $scope.isEdit = false;
        $scope.modal_Title = "Add Message";
        $scope.btnText = "Save";
        $scope.Messageform.$setPristine();
        angular.element("#modal_Message").modal('show');
    };

    $scope.DeleteMessage = function ($event) {
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
                    var table = $('#tblMessage').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = MessageServices.DeleteMessage(row.ID);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.customSuccess("Message Delete Successfully");
                            GetMessagelist();
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function () { });
                }
                bootbox.hideAll();
            }
        });
    };

    $scope.EditMessage = function ($event) {
        var table = $('#tblMessage').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetMessageById(row.ID); 
    };

    $scope.GetMessageById = function (id) {
        $scope.Messageform.$setPristine();
        var promise = MessageServices.GetCustomMessageById(id);
        promise.success(function (response) {
            if (response.Success) {
                $scope.isEdit = true;
                $scope.modal_Title = "Edit Message";
                $scope.btnText = "Save";
                $scope.MessageObj = response.Data[0];
                angular.element("#modal_Message").modal('show');
            }
            else {
                notificationFactory.customError(response.Message[0]);
            }
        });
        promise.error(function () { });
    };

    $scope.AddOrEditMessage = function (form) {
        if (form.$valid) {
            $scope.MessageObj.CreatedBy = $scope.UserAccessId;
            if (!$scope.isEdit) {  // add mode
                var promise = MessageServices.InsertCustomMessage($scope.MessageObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_Message").modal('hide');
                        notificationFactory.customSuccess("Message save successfully");
                        GetMessagelist();
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
            else { //Edit Mode
                var promise = MessageServices.UpdateCustomMessage($scope.MessageObj);
                promise.success(function (response) {
                    if (response.Success) {
                        angular.element("#modal_Message").modal('hide');
                        notificationFactory.customSuccess("Message Update successfully");
                        GetMessagelist();
                    }
                    else {
                        notificationFactory.customError(response.Message[0]);
                    }
                });
                promise.error(function () { });
            }
        }
    };

    function init() {
        GetMessagelist();
        $scope.MessageObj = new Object();
    }

    init();

});