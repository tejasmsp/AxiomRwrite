app.controller('MergeLocationController', function ($scope, $rootScope, $stateParams, $state, notificationFactory, LocationServices, CommonServices, configurationService, $compile, $filter) {

    decodeParams($stateParams);

    $scope.isEdit = false;

    //#region Events 
    

    $scope.GetMergeLocations = function (id)
    { 
        $scope.ParentLocationId = id;// $scope.$parent.MergeLocationParentId;   
         
        angular.element("#modal_LocationMergedLocation").modal('show');
        clearMergedLocationSearch();
        bindMergeLocationReplacedByID();
        $scope.$parent.ShowMergeLocation = false;
    };

    $scope.$parent.$watch('ShowMergeLocation', function (newVal, oldVal) {
        
        if (newVal) {
            if ($scope.$parent.ObjOpenMergeLocation.LocID != undefined && $scope.$parent.ObjOpenMergeLocation.LocID != null && $scope.$parent.ObjOpenMergeLocation.LocID) {
                $scope.parentLocationObj = $scope.$parent.ObjOpenMergeLocation;
                
                $scope.GetMergeLocations();
            }
        }
        
    }, true);
  
    $scope.UpdateMergeLocation = function ($event) {

         
        var table = $('#tblMergedLocation').DataTable();
        var row = table.row($($event.target).parents('tr')).data();
        var needtoMerge = $event.target.checked;
        bootbox.confirm({
            message: "Are you sure you want to " + (needtoMerge ? "merge" : "unmerge") + " this record ?",
            buttons: {
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                },
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                }
            },
            callback: function (result) {
                if (result == true) {
                    var promise = LocationServices.UpdateMergeLocation(row.LocID, $scope.parentLocationObj.LocID, needtoMerge);
                    promise.success(function (response) {
                        if (response.Success) {
                            notificationFactory.customSuccess('Location has been ' + (needtoMerge ? "merged" : "unmerged") + ' successfully.');
                            success($event);
                        }
                        else {
                            toastr.error(response.Message[0]);
                        }
                    });
                    promise.error(function (response) { toastr.error(response.Message[0]); });
                }
                else {
                    $event.target.checked = !needtoMerge;
                    updateMergeStatusHtml($event);
                }
                bootbox.hideAll();
            }
        });
    };

    $scope.ClearMergedLocationSearch_Click = function ()
    {
        clearMergedLocationSearch();
        bindMergeLocationReplacedByID()
    };

    $scope.MergedLocationSearch_Click = function () {
        bindMergeLocationReplacedByID();
    };
     
    //#endregion 

    //#region Methods  

    function bindDropDown() {
        //StateDropdown
        var _statelist = CommonServices.StateDropdown();
        _statelist.success(function (response) {
            $scope.Statelist = response.Data;
        });
        _statelist.error(function (data, statusCode) { });

        // DepartmentD  
        var _departmentlist = CommonServices.DepartmentDropdown();
        _departmentlist.success(function (response) {
            $scope.Departmentlist = response.Data;
        });
        _departmentlist.error(function (data, statusCode) { });
    };

    function bindMergeLocationReplacedByID() {
        if ($.fn.DataTable.isDataTable("#tblMergedLocation")) {
            $('#tblMergedLocation').DataTable().clear();
            $('#tblMergedLocation').DataTable().destroy();
        }
        var pagesizeObj = 5;
        $('#tblMergedLocation').DataTable({
            stateSave: false,
            "oLanguage": {
                "sProcessing": '',
                "sZeroRecords": "<span class='pull-left'>No records found</span>",
            },
            "searching": false,
            "dom": '<"table-responsive"frt><"bottom"lip<"clear">>',
            "bProcessing": true,
            "bServerSide": true,
            "iDisplayStart": 0,
            "iDisplayLength": pagesizeObj,
            "lengthMenu": [5, 10, 20, 30, 40, 50],
            "sAjaxDataProp": "aaData",
            "aaSorting": [[4, 'desc']],
            "sAjaxSource": configurationService.basePath + "/GetLocationList",
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                aoData = BindSearchCriteriaCommon(aoData);
                aoData = BindSortingCommon(aoData, oSettings);
                if ($scope.SearchValue == undefined) {
                    $scope.SearchValue = "";
                }
               
                var aa = $scope.MergedLocationSearchObj;
                $scope.GridParams = aoData;
                oSettings.jqXHR = $.ajax({
                    'dataSrc': 'aaData',
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource + "?SearchValue=" + "" + "&PageIndex=" + (parseInt($('#tblMergedLocation').DataTable().page.info().page) + 1)
                        + "&ReplacedBy=" + $scope.parentLocationObj.LocID
                        + "&LocID=" + $scope.MergedLocationSearchObj.LocID 
                        + "&Name=" + $scope.MergedLocationSearchObj.Name
                        + "&PhoneNo=" + $scope.MergedLocationSearchObj.PhoneNo
                        + "&Department=" + ($scope.MergedLocationSearchObj.Department == null ? "" : $scope.MergedLocationSearchObj.Department) 
                        + "&Address=" + $scope.MergedLocationSearchObj.Address
                        + "&City=" + $scope.MergedLocationSearchObj.City
                        + "&State=" + ($scope.MergedLocationSearchObj.State == null ? "" : $scope.MergedLocationSearchObj.State),
                    
                    "data": aoData,
                    "success": fnCallback,
                    "error": function (data, statusCode) { }
                });


            },
            "columns": [
                {
                    "title": '',
                    "sClass": "action dt-center",
                    "orderable": false,
                    "width": "4%",
                    "render": function (data, type, row) {
                        var strAction = '';
                        if (row.ReplacedBy != '') {
                            strAction = '<input type="checkbox" ng-click="UpdateMergeLocation($event)" checked/>';
                            //row.ReplacedBy
                            //strAction = 'Added';
                            //strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="DeleteMergeLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>'
                            //strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="AddMergeLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>'
                        }
                        else {
                            strAction = '<input type="checkbox" ng-click="UpdateMergeLocation($event)"/>';
                        }


                        return strAction;
                    }
                },
                { "title": "Location Code", "data": "LocID", "className": "dt-left", "orderable": false, "width": "14%" },
                { "data": "Name", "title": "Name", "className": "dt-left", "orderable": false },
                { "data": "Department", "title": "Department", "className": "dt-left", "orderable": false,"width": "20%" },
                {
                    "title": 'Marged Status',
                    "data": "ReplacedBy",
                    "sClass": "action dt-center",
                    "orderable": true,
                    "width": "16%",
                    "render": function (data, type, row) {

                        if (data != '') {
                            return '<label class="label bg-success-400">Marged</label>';

                            //strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="DeleteMergeLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>'
                            //strAction = strAction + '<a class="ico_btn cursor-pointer" ng-click="AddMergeLocation($event)" title="Delete">  <i class="icon-trash cursor-pointer"></i> </a>'
                        }
                        else {
                            return '<label class="label bg-danger-400">Not</label>';
                        }


                    }
                }

            ],
            "initComplete": function () {
                var dataTable = $('#tblMergedLocation').DataTable();
                //BindCustomerSearchBar($scope, $compile, dataTable);
            },
            "fnDrawCallback": function () {
                //BindToolTip();
                //setTimeout(function () { SetAnchorLinks(); }, 500);
            },
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                $compile(angular.element(nRow).contents())($scope);
            }
        }); 
    };

    function clearMergedLocationSearch() {
        $scope.MergedLocationSearchObj = new Object();
        $scope.MergedLocationSearchObj.LocID = "";
        $scope.MergedLocationSearchObj.Name = "";
        $scope.MergedLocationSearchObj.PhoneNo = "";
        $scope.MergedLocationSearchObj.Department = "";
        $scope.MergedLocationSearchObj.Address = "";
        $scope.MergedLocationSearchObj.City = "";
        $scope.MergedLocationSearchObj.State = "";
    };

    function updateMergeStatusHtml($event) {
        var needtoMerge = $event.target.checked;
        $($event.target).parents('tr').find('label.label').attr("class", "label " + (needtoMerge ? "bg-success-400" : "bg-danger-400"))
        $($event.target).parents('tr').find('label.label').html(needtoMerge ? "Marged" : "Not");
    };

    function init()
    {  
       
        clearMergedLocationSearch(); 
        bindDropDown();
        //$scope.GetMergeLocations();
    };
   
    //#endregion 

    init();
});