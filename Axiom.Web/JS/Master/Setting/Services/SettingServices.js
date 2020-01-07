app.service('SettingServices', function ($http, configurationService) {
    var settingService = [];

    settingService.GetLogList = function (userId, startDate, endDate, CompanyNo) {
        return $http.get(configurationService.basePath + "GetLogList?userId=" + userId + "&startDate=" + startDate + "&endDate=" + endDate + "&CompanyNo=" + CompanyNo);
    };

    settingService.GetDepartmentEmpoyeeLog = function (userId, departmentId, startDate, endDate, CompanyNo) {
        return $http.get(configurationService.basePath + "GetDepartmentEmpoyeeLog?userId=" + userId + "&departmentId=" + departmentId +  "&startDate=" + startDate + "&endDate=" + endDate + "&CompanyNo=" + CompanyNo);
    };
    return settingService;
});