app.service('DepartmentService', function ($http, configurationService) {
    var departmentService = [];

    departmentService.GetDepartmentList = function (DepartmentID) {
        return $http.get(configurationService.basePath + "GetDepartmentList?DepartmentID=" + DepartmentID);
    };
    departmentService.InsertDepartment = function (objDepartment) {
        return $http.post(configurationService.basePath + "InsertDepartment", objDepartment);
    };

    departmentService.UpdateDepartment = function (objDepartment) {
        return $http.post(configurationService.basePath + "UpdateDepartment", objDepartment);
    };

    departmentService.DeleteDepartment = function (DepartmentID) {
        return $http.post(configurationService.basePath + "DeleteDepartment?DepartmentID=" + DepartmentID);
    };
    departmentService.UpdateDepartmentSortOrder = function (DepartmentID, Direction) {
        return $http.get(configurationService.basePath + "UpdateDepartmentSortOrder?DepartmentID=" + DepartmentID + "&Direction=" + Direction);
    };
    return departmentService;
});