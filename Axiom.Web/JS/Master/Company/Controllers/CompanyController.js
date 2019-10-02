app.controller('CompanyController', function ($scope, $rootScope, $stateParams, notificationFactory, CompanyServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);

    init();
    //region event

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "Company", "View");
    $scope.IsUserCanEditCompany = $rootScope.isSubModuleAccessibleToUser('Settings', 'Company', 'Edit Company');
    $scope.IsUserCanDeleteCompany = $rootScope.isSubModuleAccessibleToUser('Settings', 'Company', 'Delete Company');
    //------------
   
    $scope.DeleteCompanyDetail = function (CompNo) {
        bootbox.confirm({
            message: "Are you sure you want to delete this record ?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                if (result) {
                    var promise = CompanyServices.DeleteCompany(CompNo);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.customSuccess("Company Delete Successfully");
                            GetCompanyList();
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function () {  });
                }
                bootbox.hideAll();
            }
        });
    };
    $scope.DeleteCompany = function ($event) {
        var table = $('#tblCompany').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.DeleteCompanyDetail(row.CompNo);
    };
    //endregion
    //#region Methods

    function init() {
        GetCompanyList();
        $scope.Companyobj = new Object();
    }
    function GetCompanyList() {

        var promise = CompanyServices.GetCompanyList();
        promise.success(function (response) {
            $scope.CompanyList = response.Data;
            bindGetCompanyList();
        });
        promise.error(function (data, statusCode) {

        });
    }

    function bindGetCompanyList() {

        if ($.fn.DataTable.isDataTable("#tblCompany")) {
            $('#tblCompany').DataTable().destroy();
        }

        var table = $('#tblCompany').DataTable({
            data: $scope.CompanyList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Company No",
                    "className": "dt-left",
                    "data": "CompNo",
                    "sorting": "true"
                },
                {
                    "title": "Company ID",
                    "className": "dt-left",
                    "data": "CompID",
                    "sorting": "true"
                },
                {
                    "title": "Name",
                    "className": "dt-left",
                    "sorting": "true",
                    "data": "CompName"
                },
                {
                    "title": "Email",
                    "className": "dt-left",
                    "sorting": "true",
                    "data": "Email"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "10%",
                    "render": function (data, type, row) {
                        var strAction = "";
                        if ($scope.IsUserCanEditCompany) {
                             strAction = "<a class='ico_btn cursor-pointer' ui-sref='EditCompany({CompNo:" + row.CompNo + "})' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }
                        if ($scope.IsUserCanDeleteCompany) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='DeleteCompany($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        }                                                
                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblCompany').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }


    

    //#endregion 

});