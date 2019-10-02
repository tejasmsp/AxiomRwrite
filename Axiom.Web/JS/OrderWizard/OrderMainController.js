app.controller('OrderMainController', function ($scope, $rootScope, $state, $stateParams, notificationFactory, Step1Service, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);
    $scope.OrderStep1Obj = {};
    //#region Event

    $scope.DownloadForm = function () {
        angular.element("#modal_DownloadForms").modal('show');
    };

    //#endregion

    //#region Method

    function init() {
        
    }

    //#endregion

    init();

});