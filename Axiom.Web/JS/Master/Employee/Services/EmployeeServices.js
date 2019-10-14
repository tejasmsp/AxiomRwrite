app.service('EmployeeServices', function ($http, configurationService) {
    var EmployeeService = [];

    EmployeeService.GetEmployeeList = function (CompanyNo) {
        return $http.get(configurationService.basePath + "GetEmployeeList?CompanyNo=" + CompanyNo);
    };
    EmployeeService.GetEmployeeById = function (UserId) {
        return $http.get(configurationService.basePath + "GetEmployeeById?UserId=" + UserId);
    };

    EmployeeService.InsertEmployee = function (model) {
        return $http.post(configurationService.basePath + "InsertEmployee", model);
    };

    EmployeeService.UpdateEmployee = function (model) {
        return $http.post(configurationService.basePath + "UpdateEmployee", model);
    };


    EmployeeService.DeleteMessage = function (id) {
        return $http.post(configurationService.basePath + "DeleteCustomMessage?id=" + id);
    };

    return EmployeeService;
});