app.controller('OrderNoteController', function ($scope, $rootScope, $http, $state, $stateParams, notificationFactory, CommonServices, OrderNoteService, configurationService, $compile, $filter) {
    decodeParams($stateParams);
    $scope.IsFromClient = 1;  // Temporary
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $scope.IsEditMode = false;
    $scope.userGuid = $rootScope.LoggedInUserDetail.UserId;
    $scope.EmpId = $rootScope.LoggedInUserDetail.EmpId;
    //#region Event

    $scope.GetOrderNotes = function () {
        var promise = OrderNoteService.GetOrderNotes($scope.OrderId, $scope.PartNo);
        promise.success(function (response) {            
            $scope.NoteList = response.Data;            
        });
        promise.error(function (data, statusCode) {
        });
    };
    $scope.AddNotePopUp = function () {
        $scope.NoteObj = new Object();
        $scope.Noteform.$setPristine();
        angular.element('#modal_Note').modal('show');
    };

    $scope.SaveNote = function (form) {

        if (isNullOrUndefinedOrEmpty($scope.NoteObj.NotesInternal) && isNullOrUndefinedOrEmpty($scope.NoteObj.NotesClient)) {
            toastr.warning('Please enter Internal OR Client note.');
            return false;
        }

        if (form.$valid) {
            $scope.NoteObj.OrderId = $scope.OrderId;
            $scope.NoteObj.PartNo = $scope.PartNo;
            $scope.NoteObj.UserId = $rootScope.LoggedInUserDetail.UserId;
            var promise = OrderNoteService.InsertOrderNotes($scope.NoteObj);
            promise.success(function (response) {
                if (response.Success && response.InsertedId == 1) {
                    toastr.success('Note Save Successfully');
                    angular.element('#modal_Note').modal('hide');
                    $scope.GetOrderNotes();
                }
                else {
                    toastr.error(response.Message[0]);

                }
                
            });
            promise.error(function (data, statusCode) {
            });
        }
    };


    //#endregion

    //#region Method    

    function init() {
        $scope.OrderId = $stateParams.OrderId;
        $scope.PartNo = isNullOrUndefinedOrEmpty($stateParams.PartNo) ? 0 : $stateParams.PartNo;
        $scope.NoteObj = new Object();
        $scope.GetOrderNotes();
    }
    //#endregion

    init();

});