app.controller('QuickNotesController', function ($scope, $rootScope, $stateParams, notificationFactory, QuickNotesServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams); 
    $scope.isEdit = false; 
    init();
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    //#region Events

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Quick Notes", "View");
    $scope.IsUserCanEditQuickNotes = $rootScope.isSubModuleAccessibleToUser('Settings', 'Quick Notes', 'Edit Quick Note');    
    //-----
  
    $scope.EditQuickNotes = function ($event) {   
        var table = $('#tblQuickNote').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.ManageQuickNoteById(row.PartStatusId);
    };

    $scope.ManageQuickNoteById = function (quicknotesId) {
        $scope.QuickNotesform.$setPristine();

        var partstatusgroup = CommonServices.PartStatusGroupsDropdown();
        partstatusgroup.success(function (response) {
                    $scope.partstatusgrouplist = response.Data;
        });


        if (quicknotesId > 0 ) {      
            $scope.modal_Title = "Edit Quick Notes";
            $scope.isEdit = true;
            var promise = QuickNotesServices.GetQuickNotesList(quicknotesId);
            promise.success(function (response) {                
                $scope.QuickNotesobj = response.Data[0];                
                $scope.QuickNotesobj.IsHidden = !$scope.QuickNotesobj.IsHidden;
                angular.element("#modal_QuickNotes").modal('show');
            });
            promise.error(function (data, statusCode) {
            });
        } else {
            $scope.isEdit = false;
            $scope.modal_Title = "Add Quick Notes";
            $scope.QuickNotesobj = new Object();
            $scope.QuickNotesobj.PartStatus = "";
            $scope.QuickNotesobj.Note = "";
            $scope.QuickNotesobj.PartStatusGroup = "";
            $scope.QuickNotesobj.PartStatusGroupId = "";
            $scope.QuickNotesobj.IsHidden = true;
            angular.element("#modal_QuickNotes").modal('show');
        }

    };

    $scope.SaveQuickNotes = function (form) {
        
        if (form.$valid) {
            $scope.QuickNotesobj.CreatedBy = $scope.UserAccessId;
            if (!$scope.isEdit) {
                var promise = QuickNotesServices.InsertQuickNotes($scope.QuickNotesobj);
                promise.success(function (response) {
                    if (response.Success) {
                        toastr.success('Note saved successfully.');
                        angular.element("#modal_QuickNotes").modal('hide');
                        GetQuickNotesList();
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else {
                var promiseedit = QuickNotesServices.UpdateQuickNotes($scope.QuickNotesobj);
                promiseedit.success(function (response) {
                    if (response.Success) {
                        toastr.success('Note updated successfully.');
                        angular.element("#modal_QuickNotes").modal('hide');
                        GetQuickNotesList();
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promiseedit.error(function (data, statusCode) {
                });
            }
        }
    };

    //#endregion 

    //#region Methods

    function GetQuickNotesList() {

        var promise = QuickNotesServices.GetQuickNotesList(0);
        promise.success(function (response) { 
            $scope.QuickNotesList = response.Data;
            bindQuickNotesList();
        });
        promise.error(function (data, statusCode) {

        });
    }

    function init() {
        GetQuickNotesList();
        $scope.QuickNotesobj = new Object();
    }

    function bindQuickNotesList() {

        if ($.fn.DataTable.isDataTable("#tblQuickNote")) {
            $('#tblQuickNote').DataTable().destroy();
        }

        var table = $('#tblQuickNote').DataTable({
            data: $scope.QuickNotesList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Part Status Group",
                    "className": "dt-left",
                    "sorting": "true",
                    "data": "PartStatusGroup",
                    "width": "15%"
                },  
                {
                    "title": "Part Status",
                    "className": "dt-left",
                    "data": "PartStatus",
                    "sorting": "true",
                    "width": "20%"
                },
                {
                    "title": "Note",
                    "className": "dt-left",
                    "data": "Note",
                    "sorting": "true"
                },
                
                {
                    "title": "Status",
                    "className": "dt-center",
                    "sorting": "false",
                    "data": "IsHidden",
                    "width": "7%",
                    "render": function (data, type, row) {
                        return (!row.IsHidden) ? "<label class='label bg-success-400'>Active</label>" : "<label class='label bg-danger-400'>InActive</label>";
                    }
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "7%",
                    "visible": $scope.IsUserCanEditQuickNotes,
                    "render": function (data, type, row) { 
                        var strAction = "<a class='ico_btn cursor-pointer' ng-click='EditQuickNotes($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblService').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    //#endregion 

});