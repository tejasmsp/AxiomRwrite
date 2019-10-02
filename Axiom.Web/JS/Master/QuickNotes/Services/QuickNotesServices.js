app.service('QuickNotesServices', function ($http, configurationService) {
    var QuickNotesService = [];

    QuickNotesService.GetQuickNotesList = function (partstatusid) {
        return $http.get(configurationService.basePath + "GetQuickNotesList?partstatusid=" + partstatusid);
    }; 

    QuickNotesService.InsertQuickNotes = function (QuickNotesobj) {
        return $http.post(configurationService.basePath + "InsertQuickNotes", QuickNotesobj);
    };

    QuickNotesService.UpdateQuickNotes = function (QuickNotesobj) {
        return $http.post(configurationService.basePath + "UpdateQuickNotes", QuickNotesobj);
    };
    return QuickNotesService;
});