app.service('AttorneyService', function ($http, configurationService) {
    var AttorneyService = [];

    AttorneyService.InsertAttorney = function (LoginEmpId, UserID, AttorneyUserObj) {
        return $http.post(configurationService.basePath + "InsertAttorney?LoginEmpId=" + LoginEmpId + "&UserID=" + UserID, AttorneyUserObj);
    };
    AttorneyService.UpdateAttorney = function (LoginEmpId, UserID, AttorneyUserObj) {
        return $http.post(configurationService.basePath + "UpdateAttorney?LoginEmpId=" + LoginEmpId + "&UserID=" + UserID, AttorneyUserObj);
    };

    AttorneyService.GetAttorneyByAttyIdForAttorney = function (AttyID) {
        return $http.get(configurationService.basePath + "GetAttorneyByAttyIdForAttorney?AttyID=" + AttyID);
    };

    AttorneyService.GetAttorneyFormsList = function (AttyID, FormType) {
        return $http.get(configurationService.basePath + "GetAttorneyFormsList?AttyID=" + AttyID + "&FormType=" + FormType);
    };

    AttorneyService.InsertAttorneyForm = function (model) {
        return $http.post(configurationService.basePath + "InsertAttorneyForm", model);
    };

    AttorneyService.DeleteAttorneyForm = function (AttyFormID) {
        return $http.post(configurationService.basePath + "DeleteAttorneyForm?AttyFormID=" + AttyFormID);
    };

    AttorneyService.GetAttorneyAssistantContactList = function (UserAccessId, AttyID) {        
        return $http.get(configurationService.basePath + "GetAttorneyAssistantContactList?AttyID=" + AttyID + "&UserAccessId=" + UserAccessId);
    };


    return AttorneyService;
});