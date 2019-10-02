app.service('AnnouncementServices', function ($http, configurationService) {
    var AnnouncementService = [];

    AnnouncementService.GetDailyAnnouncement = function () {
        return $http.get(configurationService.basePath + "GetDailyAnnouncement");
    }; 

    AnnouncementService.InsertDailyAnnouncement = function (strAnnouncement) {
        return $http.post(configurationService.basePath + "InsertDailyAnnouncement?strAnnouncement=" + strAnnouncement);
    };
    return AnnouncementService;
});