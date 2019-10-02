app.controller('PaymentController', function ($scope, $rootScope, $stateParams, notificationFactory, PaymentServices, configurationService, CommonServices, $compile, $filter) {

    decodeParams($stateParams);
    $scope.isEdit = false;
    $scope.IsUserCanEditPayment = $rootScope.isSubModuleAccessibleToUser('Settings', 'Payment', 'Edit Payment');
    $scope.IsUserCanDeletePayment = $rootScope.isSubModuleAccessibleToUser('Settings', 'Payment', 'Delete Payment');

    function bindPaymentList() {

        if ($.fn.DataTable.isDataTable("#tblPayment")) {
            $('#tblPayment').DataTable().destroy();
        }

        var table = $('#tblPayment').DataTable({
            data: $scope.PaymentList,
            "bDestroy": true,
            "dom": '<"top pull-left "f><"table"rt><"bottom"lip<"clear">>',
            "aaSorting": false,
            "aLengthMenu": [10, 20, 50, 100, 200],
            "pageLength": 10,
            "stateSave": false,
            "autoWidth": false,
            "columns": [
                {
                    "title": "Date",
                    "className": "dt-left",
                    "data": "PmtDate",
                    "width": "15%",
                    "sorting": "false",
                    "render": function (data, type, row) {
                        return $filter('date')(data, $rootScope.GlobalDateFormat);
                    }

                },
                {
                    "title": "Cheque Amount",
                    "className": "dt-left",
                    "data": "ChkAmt",
                    "sorting": "false"
                },

                {
                    "title": "Cheque Number",
                    "className": "dt-left",
                    "sorting": "false",
                    "data": "ChkNo"
                },
                {
                    "title": "Made By",
                    "className": "dt-left",
                    "sorting": "false",
                    "data": "PaidBy"
                },
                {
                    "title": 'Action',
                    "data": null,
                    "className": "action dt-center",
                    "sorting": "false",
                    "width": "7%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if ($scope.IsUserCanEditPayment) {
                            strAction = "<a class='ico_btn cursor-pointer' ng-click='EditPayment($event)' title='Edit'> <i  class='icon-pencil3' ></i></a> ";
                        }
                        if ($scope.IsUserCanDeletePayment) {
                            strAction += "<a class='ico_btn cursor-pointer' ng-click='DeletePayment($event)'  title='Delete'>  <i  class='icon-trash cursor-pointer'></i> </a>";
                        }


                        return strAction;
                    }
                }
            ],
            "initComplete": function () {
                var dataTable = $('#tblPayment').DataTable();
            },
            "fnDrawCallback": function () {
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        });
    }


    $scope.EditPayment = function ($event) {
        var table = $('#tblPayment').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        $scope.GetPaymentDetail(row.PmtNo);
    };

    $scope.GetPaymentDetail = function (PmtNo) {
        $scope.Paymentform.$setPristine();
        if (PmtNo > 0) {
            $scope.modal_Title = "Edit Payment";
            $scope.isEdit = true;
            GetPaymentList(PmtNo);
        } else {
            $scope.modal_Title = "Add Payment";
            $scope.isEdit = false;
            $scope.opjPayment = {};
            $scope.opjPayment.ChkAmt = "";
            $scope.opjPayment.PmtDate = "";
            $scope.opjPayment.PaidBy = "";
            $scope.opjPayment.ChkNo = "";
            angular.element("#modal_Payment").modal('show');
        }

    };

    $scope.createDatePicker = function () {
        createDatePicker();
    };

    $scope.SearchPayment = function () {
        GetPaymentList(0);
    }

    function GetPaymentList(PmtNo) {
        var paymentDate = $filter('date')(new Date($scope.payment.paymentDate), $rootScope.GlobalDateFormat);
        var promise = PaymentServices.GetPaymentList(paymentDate, PmtNo);
        promise.success(function (response) {
            if (PmtNo > 0) {
                $scope.opjPayment = response.Data[0];
                angular.element("#modal_Payment").modal('show');
            }
            else {
                $scope.PaymentList = response.Data;
                bindPaymentList();
            }
        });
        promise.error(function (data, statusCode) {
        });
    }

    $scope.AddOrUpdatePayment = function (form) {
        if (form.$valid) {
            var promise = PaymentServices.InsertUpdatePayment($scope.opjPayment);
            promise.success(function (response) {
                if (response.Success) {
                    toastr.success("Payment saved successfully.");
                    angular.element("#modal_Payment").modal('hide');
                    GetPaymentList(0);
                }
                else {
                    notificationFactory.customError(response.Message[0]);
                }
            });
            promise.error(function (data, statusCode) {
            });
        }
    };

    $scope.DeletePayment = function ($event) {

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
                    var table = $('#tblPayment').DataTable();
                    var row = table.row($($event.target).parents('tr')).data();
                    var promise = PaymentServices.DeletePayment(row.PmtNo);
                    promise.success(function (response) {
                        if (response.Success) {
                            toastr.success('Payment deleted successfully.');
                            GetPaymentList(0);
                        }
                        else {
                            notificationFactory.customError(response.Message[0]);
                        }
                    });
                    promise.error(function () { });
                }
                bootbox.hideAll();
            }
        });



    };
    $scope.ResetPayment = function () {
        init();
    }

    function init() {
        $scope.payment = {
            paymentDate: $filter('date')(new Date(), "MM/dd/yyyy"),
            chequeNumber: ''
        };
        GetPaymentList(0);
    }

    init();
});