app.service('CompanyServices', function ($http, configurationService) {
    var CompanyService = [];

    CompanyService.GetCompanyList = function () {
        return $http.get(configurationService.basePath + "GetCompanyList");
    }; 

    CompanyService.InsertCompany = function (Companyobj) {
        return $http.post(configurationService.basePath + "InsertCompany", Companyobj);
    };

    CompanyService.UpdateCompanyDetail = function (Companyobj) {
        return $http.post(configurationService.basePath + "UpdateCompanyDetail", Companyobj);
    };

    CompanyService.GetCompanyDetailById = function (CompNo) {
        return $http.get(configurationService.basePath + "GetCompanyDetailById?CompNo="+CompNo);
    };

    CompanyService.DeleteCompany = function (CompNo) {
        return $http.post(configurationService.basePath + "DeleteCompany?CompNo=" + CompNo);
    };
    return CompanyService;
});