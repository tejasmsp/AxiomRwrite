app.controller('CompanyAddUpdateController', function ($scope, $state, $rootScope, $stateParams, notificationFactory, CompanyServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.UserAccessId = $rootScope.LoggedInUserDetail.UserAccessId;
    $rootScope.pageTitle = parseInt($stateParams.CompNo) > 0 ? "Edit Company" : "Add Company";

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Company", "View");
    $scope.IsUserCanEditCompany = $rootScope.isSubModuleAccessibleToUser('Settings', 'Company', 'Edit Company');    
    //------------

    //#region Events

    $scope.UpdateUserProfile = function (form) {

        if (form.$valid) { 
            var fd = new FormData();
            fd.append("file", $("#fileLogo")[0].files[0]);
            var fileupload = CompanyServices.UploadCompanyLogo(fd, $scope.Companyobj.CompNo);
            fileupload.success(function (response) {
                if (response != null && response.Message != null && response.Message.length > 0) {
                    toastr.error(response.Message[0]);
                    return;
                }
                if (!$scope.isEdit)
                {
                    toastr.success('Company saved successfully.'); 
                }
                else
                {
                    toastr.success('Company updated successfully.');
                }
                $state.transitionTo('Company');
             
            });
        }

    };

    $scope.SaveCompany = function (form) {
        
        if (form.$valid) {
            $scope.Companyobj.CreatedBy = $scope.UserAccessId;
            if (!$scope.isEdit) {
                
                var promise = CompanyServices.InsertCompany($scope.Companyobj);
                promise.success(function (response) {
                    if (response.Success) { 
                        $scope.Companyobj.CompNo = response.InsertedId;
                        $scope.UpdateUserProfile(form);
                        //toastr.success('Company saved successfully.');
                        //$state.transitionTo('Company');
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promise.error(function (data, statusCode) {
                });
            }
            else { 
                var promiseedit = CompanyServices.UpdateCompanyDetail($scope.Companyobj);
                promiseedit.success(function (response) {
                    
                    if (response.Success) {
                        //toastr.success('Company updated successfully.');
                        //$state.transitionTo('Company');
                        $scope.UpdateUserProfile(form);
                    }
                    else {
                        toastr.error(response.Message[0]);
                    }
                });
                promiseedit.error(function (data, statusCode) {
                });
            }
        }
    };
    //#endregion 

    //#region Methods
    $scope.GetCompany = function (ComapanyId) {
        if (ComapanyId > 0) {
            $scope.isEdit = true;
            var promise = CompanyServices.GetCompanyDetailById(ComapanyId);
            promise.success(function (response) {                
                $scope.Companyobj = response.Data[0];
            });
            promise.error(function (data, statusCode) {
            });
        } else {
            $scope.isEdit = false;
        }
    };

    function dropdownbind() {

        var statelist = CommonServices.StateDropdown();
        statelist.success(function (response) {
            $scope.Statelist = response.Data;
        });
        var Termlist = CommonServices.GetTermDropDown();
        Termlist.success(function (response) {
            $scope.Termlist = response.Data;
        });
        Termlist.error(function (response) {
            toastr.error(response.Message[0]);
        });
        var companydropdownlist = CommonServices.GetCompanyDropDown();
        companydropdownlist.success(function (response) {
            $scope.companydropdownlist = response.Data;
        });
        companydropdownlist.error(function (response) {
            toastr.error(response.Message[0]);
        });


        var Accountlist = CommonServices.GetAccountDropDown();
        Accountlist.success(function (response) {
            $scope.Accountlist = response.Data;
        });
        Accountlist.error(function (response) {
            toastr.error(response.Message[0]);
        });

    }

    function init() {
        dropdownbind();
        $scope.Companyobj = new Object();
        //$scope.Companyobj.CompID = "";
        //$scope.Companyobj.CompName = "";
        //$scope.Companyobj.Street1 = "";
        //$scope.Companyobj.Street2 = "";
        //$scope.Companyobj.State = "";
        //$scope.Companyobj.City = "";
        //$scope.Companyobj.Zip = "";
        //$scope.Companyobj.Email = "";
        //$scope.Companyobj.AreaCode1 = "";
        //$scope.Companyobj.PhoneNo = "";
        //$scope.Companyobj.AreaCode2 = "";
        //$scope.Companyobj.FaxNo = "";
        //$scope.Companyobj.TaxID = "";
        //$scope.Companyobj.Term = "";
        //$scope.Companyobj.RemitNo = 0;
        //$scope.Companyobj.Notes = "";
        //$scope.Companyobj.LateDays = 0;
        //$scope.Companyobj.DueDays = 0;
        //$scope.Companyobj.OnInv = false;
        //$scope.Companyobj.OnStmt = false;
        //$scope.Companyobj.ShowPage = false;
        //$scope.Companyobj.PreprtInv = false;
        //$scope.Companyobj.PreprtStmt = false;
        //$scope.Companyobj.ChkNo = 0;
        //$scope.Companyobj.RefundNo = 0;
        //$scope.Companyobj.FCNo = 0;
        //$scope.Companyobj.DcntNo = 0;
        //$scope.Companyobj.DebtNo = 0;
        //$scope.Companyobj.PayrollNo = 0;        
        $scope.GetCompany(parseInt($stateParams.CompNo));
    }

    //#endregion 
    init();
});