app.controller('AnnouncementController', function ($scope, $rootScope, $stateParams, notificationFactory, HomeServices, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);

    $scope.isEdit = false;

    init();

    //#region Events
    $scope.GetAnnouncementDetail = function () {
        $scope.strAnnouncement ="";
        var promise = AnnouncementServices.GetDailyAnnouncement();
        promise.success(function (response) {
            if (response.Success) {
                $scope.strAnnouncement = response.str_ResponseData;
            }
            angular.element("#modal_Announcement").modal('show');
        });
        promise.error(function (data, statusCode) {

        });
        
    };
    $scope.AddOrUpdateAnnouncement = function (form) {
        if (form.$valid) {
            var promise = AnnouncementServices.InsertDailyAnnouncement($scope.strAnnouncement);
            promise.success(function (response) {
                if (response.Success) {
                    notificationFactory.successAdd("Announcement");
                    angular.element("#modal_Announcement").modal('hide');
                    $scope.announcement = $scope.strAnnouncement;
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

    //#region Methods
    function GetAnnouncement() {
        var promise = AnnouncementServices.GetDailyAnnouncement();
        promise.success(function (response) {
            if (response.Success) {
                $scope.announcement = response.str_ResponseData;
            }
            else {
                $scope.announcement = "";
            }
        });
        promise.error(function (data, statusCode) {

        });
    }
    function init() {
        GetAnnouncement();
    }
    //#endregion 

});