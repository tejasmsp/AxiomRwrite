app.controller('SSNController', function ($scope, $stateParams, notificationFactory, SSNService, configurationService, CommonServices, $compile, $filter, $rootScope) {

    decodeParams($stateParams);

    //Page Rights//
    $rootScope.CheckIsPageAccessible("Settings", "SSN", "View");
    $scope.IsUserCanEditSSN = $rootScope.isSubModuleAccessibleToUser('Settings', 'SSN', 'Edit SSN');    
    //-----
    
    function bindSSNList() {

        if ($.fn.DataTable.isDataTable("#tblSSN")) {
            $('#tblSSN').DataTable().destroy();
        }

        var table = $('#tblSSN').DataTable({
            data: $scope.SSNList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',            
            "aaSorting": [[0, 'asc']],
            "aLengthMenu": [10, 20],
            "pageLength": 10,
            "stateSave": false,
            "columns": [
                {
                    "title": "Setting",
                    "className": "dt-left",
                    "data": "SettingName"
                },
                {
                    "width": "10%",
                    "title": "Display SSN",
                    "sClass": "action dt-center",
                    //"className": "dt-body-center",
                    "data": "SettingValue",
                    "orderable": false,
                    "render": function (data, type, row) {
                        if ($scope.IsUserCanEditSSN) {
                            if (type === 'display') {
                                // return '<div class="checkbox checkbox-switch"><input type="checkbox" class="switch"></div>';
                                // return '<div class="checkbox checkbox-switch"><label><input type="checkbox" data-off-color="danger" data-on-text="Yes" data-off-text="No" class="switch" checked="checked"></label></div>';
                                return '<switch name="yesNo' + data + '" ng-change="ChangeSSN($event)"   ng-model="SSNList[' + $scope.SSNList.indexOf(row) + '].SettingValue"   on="Yes" off="No" class="wide"></switch>';
                            }
                            else {
                                return "<label>" + (data === true) ? "Yes" : "No" + "</label>";
                            }
                        } else {
                            return "<label>" + (data === true) ? "Yes" : "No" + "</label>";
                        }

                        // return data;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblSSN').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }

    $scope.isEdit = false;

    $scope.ChangeSSN = function ($event) {
        tblSSN = $('#tblSSN').DataTable();
        var data = tblSSN.row($($event.target).parents('tr')).data();
        SSNService.UpdateSSNSetting(data);
    };

    function GetSSNList() {
        var ssn = SSNService.GetSSNSettingList();
        ssn.success(function (response) {
            $scope.SSNList = response.Data;
            bindSSNList();
        });
        ssn.error(function (data, statusCode) { });
    }
    function init() {
        GetSSNList();
    }
    init();
});