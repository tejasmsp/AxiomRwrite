app.service('CommonServices', function ($http, configurationService) {
    var commonService = [];

    commonService.StateDropdown = function () {
        return $http.get(configurationService.basePath + "StateDropdown");
    };
    commonService.DistrictDropdown = function (StateID) {
        return $http.get(configurationService.basePath + "DistrictDropdown?StateID=" + StateID);
    };
    commonService.CountyDropdown = function (StateID) {
        return $http.get(configurationService.basePath + "CountyDropdown?StateID=" + StateID);
    };
    commonService.CourtDropdown = function (StateID) {
        return $http.get(configurationService.basePath + "CourtDropdown?StateID=" + StateID);
    };
    commonService.DivisionDropdown = function (DistrictId, StateId) {
        return $http.get(configurationService.basePath + "DivisionDropdown?DistirctId=" + DistrictId + "&StateId=" + StateId);
    };
    commonService.DepartmentDropdown = function () {
        return $http.get(configurationService.basePath + "DepartmentDropdown");
    };
    commonService.PartStatusGroupsDropdown = function () {
        return $http.get(configurationService.basePath + "PartStatusGroupsDropdown");
    };
    commonService.GetTermDropDown = function () {
        return $http.get(configurationService.basePath + "GetTermDropDown");
    };
    commonService.GetCompanyDropDown = function () {
        return $http.get(configurationService.basePath + "GetCompanyDropDown");
    };
    commonService.GetAccountDropDown = function () {
        return $http.get(configurationService.basePath + "GetAccountDropDown");
    };
    commonService.RecordTypeDropDown = function () {
        return $http.get(configurationService.basePath + "RecordTypeDropDown");
    };
    commonService.FirmForDropdown = function (search) {
        return $http.get(configurationService.basePath + "FirmForDropdown?search=" + search);
    };
    commonService.AttorneyForDropdown = function (search) {
        return $http.get(configurationService.basePath + "AttorneyForDropdown?search=" + search);
    };
    commonService.MemberDropdown = function () {
        return $http.get(configurationService.basePath + "MemberDropdown");
    };
    commonService.RateRecordTypeDropdown = function (MemberID) {
        return $http.get(configurationService.basePath + "RateRecordTypeDropdown?MemberID=" + MemberID);
    };
    commonService.GetNewSequenceNumber = function (tableName, firstField, secondField) {
        return $http.get(configurationService.basePath + "GetNewSequenceNumber?tableName=" + tableName + "&firstField=" + firstField + "&secondField=" + secondField);
    };
    commonService.GetAttorneyByFirmID = function (firmId) {
        return $http.get(configurationService.basePath + "GetAttorneyByFirmID?FirmId=" + firmId);
    };
    commonService.GetAttorneyByFirmIDForclient = function (firmId, UserId, isShowMore, CompanyNo) {
       
        return $http.get(configurationService.basePath + "GetAttorneyByFirmIDForclient?FirmId=" + firmId + "&UserId=" + UserId + "&isShowMore=" + isShowMore + "&CompNo=" + CompanyNo);
    };
    commonService.GetAttorneyByFirmIDForclientAndAdmin = function (firmId, UserId, isShowMore, CompanyNo) {
        return $http.get(configurationService.basePath + "GetAttorneyByFirmIDForclientAndAdmin?FirmId=" + firmId + "&UserId=" + UserId + "&isShowMore=" + isShowMore + "&CompNo=" + CompanyNo);
    };
    commonService.LocationDepartmentDropDown = function () {
        return $http.get(configurationService.basePath + "LocationDepartmentDropDown");
    };
    commonService.FirmTypeForDropdown = function () {
        return $http.get(configurationService.basePath + "FirmTypeForDropdown");
    };
    commonService.GetSalutationDropDown = function () {
        return $http.get(configurationService.basePath + "GetSalutationDropDown");
    };
    commonService.GetAttorneyDropDown = function () {
        return $http.get(configurationService.basePath + "GetAttorneyDropDown");
    };
    commonService.GetFileTypeDropdown = function () {
        return $http.get(configurationService.basePath + "GetFileTypeDropdown");
    };
    commonService.GetFirmByUserId = function (UserID, CompanyNo) {
        return $http.get(configurationService.basePath + "GetFirmByUserId?UserID=" + UserID + "&CompNo=" + CompanyNo);
    };
    commonService.GetAssociatedFirm = function (UserID, OrderID) {
        return $http.get(configurationService.basePath + "GetAssociatedFirm?UserID=" + UserID + "&OrderID=" + OrderID);
    };
    commonService.GetFirmDetailByFirmID = function (FirmID) {
        return $http.get(configurationService.basePath + "GetFirmDetailByFirmID?FirmID=" + FirmID);
    };
    commonService.GetAttorneyDetailByAttyID = function (AttyID) {
        return $http.get(configurationService.basePath + "GetAttorneyDetailByAttyID?AttyID=" + AttyID);
    };
    commonService.GetAttorneyFormUserId = function (UserID) {
        return $http.get(configurationService.basePath + "GetAttorneyFormUserId?UserID=" + UserID);
    };
    commonService.GetQuickNotesDropDown = function () {
        return $http.get(configurationService.basePath + "GetQuickNotesDropDown");
    };
    commonService.GetInternalStatusDropDown = function () {
        return $http.get(configurationService.basePath + "GetInternalStatusDropDown");
    };
    commonService.GetCanvasRequestMasterDropDown = function () {
        return $http.get(configurationService.basePath + "GetCanvasRequestMasterDropDown");
    };
    commonService.GetAccountRepDropdown = function () {
        return $http.get(configurationService.basePath + "GetAccountRepDropdown");
    };
    commonService.GetInternalStatusesDropdown = function () {
        return $http.get(configurationService.basePath + "GetInternalStatusesDropdown");
    };
    commonService.GetLocationNotes = function (LocId) {
        return $http.get(configurationService.basePath + "GetLocationNotes?LocId=" + LocId);
    }
    commonService.GetFirmNotes = function (FirmId) {
        return $http.get(configurationService.basePath + "GetFirmNotes?FirmId=" + FirmId);
    }
    commonService.GetAttorneyNotes = function (AttyId) {
        return $http.get(configurationService.basePath + "GetAttorneyNotes?AttyId=" + AttyId);
    }
    commonService.GetUserDetails = function () {
        return $http.get(configurationService.basePath + "GetUserDetails");
    }
    commonService.GetScopeForLocation = function (OrderNo, RecType) {
        return $http.get(configurationService.basePath + "GetScopeForLocation?OrderNo=" + OrderNo + "&RecType=" + RecType);
    }

    return commonService;
});