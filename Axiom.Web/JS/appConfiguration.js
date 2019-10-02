'use strict';


var app = angular.module("AXIOM", ["ui.router", "LocalStorageModule", "ngSanitize", "angular.filter", "uiSwitch"]);
//app.factory("ShareData", function () {
//    return { value: 0 };
//});

//app.config(function (localStorageServiceProvider) {
//    localStorageServiceProvider
//      .setStorageType('localStorage');

//}); 

app.config(function ($stateProvider, $urlRouterProvider, $locationProvider) {

    $stateProvider.state({ name: 'Home', url: '/Home', templateUrl: '/Templates/Home/index.cshtml', title: 'Home' });
    $stateProvider.state({ name: 'Dashboard', url: '/Dashboard', templateUrl: '/Templates/Home/Dashboard.cshtml', title: 'Dashboard' });

    $stateProvider.state({ name: 'TestRoute', url: '/TestRoute', templateUrl: '/Templates/Home/testRoute.cshtml' });

    $stateProvider.state({ name: 'Location', url: '/Location', templateUrl: '/Templates/Settings/Location.cshtml', title: "Manage Location" });
    $stateProvider.state({ name: 'ManageLocation', url: '/ManageLocation?LocID', templateUrl: '/Templates/Settings/AddOrUpdateLocation.cshtml' });




    //Setting Pages
    $stateProvider.state({ name: 'AttorneyUsers', url: '/AttorneyUsers', templateUrl: '/Templates/Settings/AttorneyUsers.cshtml', title: 'Manage Attorney User' });
    $stateProvider.state({ name: 'Scope', url: '/Scope', templateUrl: '/Templates/Settings/Scope.cshtml', title: 'Manage Scope' });
    $stateProvider.state({ name: 'BillingRate', url: '/BillingRate', templateUrl: '/Templates/Settings/BillingRate.cshtml', title: "Billing Rate" });
    $stateProvider.state({ name: 'County', url: '/County', templateUrl: '/Templates/Settings/County.cshtml', title: "Manage County" });
    $stateProvider.state({ name: 'Court', url: '/Court', templateUrl: '/Templates/Settings/Court.cshtml', title: "Manage Court" });
    $stateProvider.state({ name: 'Department', url: '/Department', templateUrl: '/Templates/Settings/Department.cshtml', title: "Manage Department" });
    $stateProvider.state({ name: 'Employees', url: '/Employees', templateUrl: '/Templates/Settings/Employees.cshtml', title: 'Manage Employee' });
    $stateProvider.state({ name: 'FirmScope', url: '/FirmScope', templateUrl: '/Templates/Settings/FirmScope.cshtml' });
    $stateProvider.state({ name: 'IIFFiles', url: '/IIFFiles', templateUrl: '/Templates/Settings/IIFFiles.cshtml', title: 'IIF Files' });
    $stateProvider.state({ name: 'InvoiceBatch', url: '/InvoiceBatch', templateUrl: '/Templates/Settings/InvoiceBatch.cshtml', title: "Invoice Batch" });
    $stateProvider.state({ name: 'District', url: '/District', templateUrl: '/Templates/Settings/District.cshtml', title: "Manage District" });
    //$stateProvider.state({ name: 'ManageLocation', url: '/ManageLocation', templateUrl: '/Templates/Settings/ManageLocation.cshtml' });
    //$stateProvider.state({ name: 'AddEditLocation', url: '/AddEditLocation?LocationId', templateUrl: '/Templates/Settings/AddOrEditLocation.cshtml' });
    $stateProvider.state({ name: 'ManageLog', url: '/ManageLog', templateUrl: '/Templates/Settings/Log.cshtml', title: 'Log Details' });
    $stateProvider.state({ name: 'Message', url: '/Message', templateUrl: '/Templates/Settings/Message.cshtml', title: 'Manage Custom Message' });
    $stateProvider.state({ name: 'Payment', url: '/Payment', templateUrl: '/Templates/Settings/Payment.cshtml', title: 'Manage Payment' });
    $stateProvider.state({ name: 'QuickNotes', url: '/QuickNotes', templateUrl: '/Templates/Settings/QuickNotes.cshtml', title: 'Manage Quick Notes' });
    $stateProvider.state({ name: 'SSN', url: '/SSN', templateUrl: '/Templates/Settings/SSN.cshtml', title: 'Manage SSN Settings' });
    $stateProvider.state({ name: 'Company', url: '/Company', templateUrl: '/Templates/Settings/Company.cshtml', title: 'Manage Company' });
    $stateProvider.state({ name: 'EditCompany', url: '/EditCompany?CompNo', templateUrl: '/Templates/Settings/AddOrUpdateCompany.cshtml' });
    //Order Detail
    $stateProvider.state({ name: 'EditOrder', url: '/EditOrder?OrderId&Step&PageMode&AddPart', templateUrl: '/Templates/Order/AddOrUpdateOrder.cshtml', title: 'Order' });
    $stateProvider.state({ name: 'OrderDetail', url: '/OrderDetail?OrderId', templateUrl: '/Templates/OrderDetail/OrderDetail.cshtml', title: 'Order Detail' });

    //Part Detail
    $stateProvider.state({ name: 'PartDetail', url: '/PartDetail?OrderId&PartNo', templateUrl: '/Templates/PartDetail/PartDetail.cshtml', title: 'Part Detail' });

    //Roles 
    $stateProvider.state({ name: 'Roles', url: '/Roles', templateUrl: '/Templates/Role/Role.cshtml', title: 'Manage Role' });
    $stateProvider.state({ name: 'RoleConfiguration', url: '/RoleConfiguration?RoleAccessId&RoleName', templateUrl: '/Templates/Role/RoleRightsConfiguration.cshtml' });
    // $stateProvider.state({ name: 'UserProfile', url: '/UserProfile', templateUrl: '/Templates/UserProfile/UserProfile.cshtml' });

    //Admin->firm
    $stateProvider.state({ name: 'Firm', url: '/Firm', templateUrl: '/Templates/Firms/Firm.cshtml', title: 'Manage Firm' });
    $stateProvider.state({ name: 'ManageFirm', url: '/ManageFirm?FirmID', templateUrl: '/Templates/Firms/AddOrUpdateFirm.cshtml' });

    //Admin->Attorney
    $stateProvider.state({ name: 'AttorneyDetail', url: '/AttorneyDetail', templateUrl: '/Templates/Attorney/AttorneyDetail.cshtml', title: 'Attorney Detail' });
    $stateProvider.state({ name: 'ManageAttorney', url: '/ManageAttorney?AttyID', templateUrl: '/Templates/Attorney/AddOrUpdateAttorney.cshtml' });

    //Order List
    $stateProvider.state({ name: 'OrderList', url: '/OrderList?Search', templateUrl: '/Templates/OrderList/OrderList.cshtml', title: 'Order List' });

    //Account Executive
    $stateProvider.state({ name: 'AccountExecutive', url: '/AccountExecutive', templateUrl: '/Templates/Order/AccountExecutive.cshtml' });
    //any url that doesn't exist in routes redirect to '/'

    //Billing
    $stateProvider.state({ name: 'Billing', url: '/Billing', templateUrl: '/Templates/Billing/Billing.cshtml', title: 'Billing' });

    $stateProvider.state({ name: 'PrintInvoice', url: '/PrintInvoice?InvoiceID&OrderId&PartNo&IsPrintAll', templateUrl: '/Templates/PrintInvoice/PrintInvoice.cshtml' });

    $stateProvider.state({ name: 'SearchOrderList', url: '/SearchOrderList?FromDate&EndDate&IsRush&EmpId&IsCallBack&IsFromDailyAnnouncement', templateUrl: '/Templates/SearchOrderList/SearchOrderList.cshtml', title: 'Search Order List' });

    //$stateProvider.state({ name: 'ProposalFeeApproval', url: '/ProposalFeeApproval', templateUrl: '/Templates/PartDetail/ProposalFeeApproval.cshtml', title: 'Proposal Fee Approval' });

    $stateProvider.state({ name: 'EmployeeLog', url: '/EmployeeLog', templateUrl: '/Templates/Settings/Log.cshtml', title: 'Employee Logs' });

    $stateProvider.state({ name: 'Check', url: '/Check', templateUrl: '/Templates/AccountReceivable/AccountReceivable.cshtml', title: 'Check' });
    $stateProvider.state({ name: 'ManageCheck', url: '/ManageCheck?ArID', templateUrl: '/Templates/AccountReceivable/AddOrUpdateAccountReceivable.cshtml', title: 'ManageCheck' });
    $stateProvider.state({ name: 'VoidInvoice', url: '/VoidInvoice', templateUrl: '/Templates/AccountReceivable/VoidInvoices.cshtml', title: 'Void Invoices' });
    $stateProvider.state({ name: 'AccountReceivable', url: '/AccountReceivable', templateUrl: '/Templates/AccountReceivable/AccountReceivable.cshtml', title: 'Account Receivable' });
    $stateProvider.state({ name: 'ManageAccountReceivable', url: '/ManageAccountReceivable', templateUrl: '/Templates/AccountReceivable/AddOrUpdateAccountReceivable.cshtml' });

    $stateProvider.state({ name: 'AccessReports', url: '/AccessReports/:type', params: { type: { squash: true, value: null } }, templateUrl: '/Templates/AccessReports/Reports.cshtml', title: 'Reports' });



    $urlRouterProvider.otherwise('/Home');

    $urlRouterProvider.otherwise('/Home');

    //MiscellaneousCharges
    $stateProvider.state({ name: 'MiscellaneousCharges', url: '/MiscellaneousCharges', templateUrl: '/Templates/Settings/MiscellaneousCharges.cshtml', title: 'MiscellaneousCharges' });

    //$locationProvider.html5Mode({
    //    enabled: true,
    //    requireBase: false
    //});
    $locationProvider.html5Mode(true);
})
    .run(function ($http, $rootScope, $location, $filter, $state, $stateParams, localStorageService, $templateCache, SettingServices) {
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;
        $rootScope.GlobalDateFormat = 'MM/dd/yyyy';

        $rootScope.LoggedInUserDetail = null;
        $rootScope.UserPermissionList = [];
        $rootScope.Enum = null;
        $rootScope.IsAdmin = false;
        $rootScope.employeeLogCount = 0;

        $rootScope.$on('$viewcontentloaded', function () {
            $templatecache.removeall();
        });

        $rootScope.isSubModuleAccessibleToUser = function (module, subModule, func) {            
            var IsAccessible = false;
            if ($rootScope.LoggedInUserDetail.UserAccessId > 0) {
                if ($rootScope.UserPermissionList.length > 0) {
                    var Obj = $filter('filter')($rootScope.UserPermissionList, { ModuleName: module, SubmoduleName: subModule, FunctionName: func }, true).length >= 1;
                    if (Obj) {
                        IsAccessible = true;
                    }
                }
            }
            return IsAccessible;
        };
        $rootScope.getEmployeeCurrentLog = function () {
            var currentDate = $filter('date')(new Date(), $rootScope.GlobalDateFormat);
            var toDate = new Date();
            toDate.setDate(toDate.getDate() + 1);
            toDate = $filter('date')(toDate, $rootScope.GlobalDateFormat);

            var promise = SettingServices.GetLogList($rootScope.LoggedInUserDetail.UserId, currentDate, toDate);
            promise.success(function (response) {

                $rootScope.employeeLogCount = response.Data.length;
            });
            promise.error(function (data, statusCode) {
            });
        }
        $rootScope.CheckIsPageAccessible = function (module, submodule, func) {
            if (!$rootScope.isSubModuleAccessibleToUser(module, submodule, func)) {
                window.location.href = '/accessdenied';
            }
        };

        $rootScope.RedirectsTOAccessDenied = function (NeedToRedirect) {
            if (NeedToRedirect) {
                window.location.href = '/accessdenied';
            }
        };

        $rootScope.CheckValueIsNullEmptyUndefined = function (value) {
            return (value == undefined || value == null || value === '');
        };


        $rootScope.RemoveAllFromLocalStorage_StartWith = function (startWith) {
            var CmpStrLength = 0;

            if (isNullOrUndefinedOrEmpty(startWith)) {
                CmpStrLength = 0;
            } else {
                CmpStrLength = startWith.length;
            }
            if (CmpStrLength > 0) {
                if (localStorage.length > 0) {
                    var arr = []; // Array to hold the keys
                    // Iterate over localStorage and insert the keys that meet the condition into arr
                    for (var i = 0; i < localStorage.length; i++) {
                        if (localStorage.key(i).substring(0, 3) == startWith) {
                            arr.push(localStorage.key(i));
                        }
                    }

                    // Iterate over arr and remove the items by key
                    for (var i = 0; i < arr.length; i++) {
                        localStorage.removeItem(arr[i]);
                    }
                }
            }

        }
        // Redirect to login if route requires auth and you're not logged in
        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            $rootScope.pageTitle = toState.title;
            $state.previous = fromState;
            $state.previousParams = fromParams;
            //if (!(fromParams == toParams)) {
            //    encodeParams(toParams);
            //}
            // to track previous url
            $state.previousHref = $state.href(fromState, fromParams);
            event.targetScope.$watch('$viewContentLoaded', function () {

                //$rootScope.EnableTextSelection();
            })


            if ($rootScope.LoggedInUserDetail.RoleName.contains("Administrator") || $rootScope.LoggedInUserDetail.RoleName.contains("Employee")) {

            }
            else {
                var isAttyUser = $rootScope.LoggedInUserDetail.RoleName.contains("Attorney");
                if (isAttyUser && toState.name === "Home") {
                    $location.path("Dashboard");
                }
            }



        });

    });

