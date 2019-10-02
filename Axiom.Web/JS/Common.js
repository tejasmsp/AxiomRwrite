ApplicationVersion = 1.0;

$(function () {
    if (!isNullOrUndefinedOrEmpty($.fn.datepicker))
    {
        var datepicker = $.fn.datepicker.noConflict();
        $.fn.bootstrapDP = datepicker;
        //    $("#dp3").bootstrapDP();
    }
});

function AddSpace(inputString) {
    inputString = inputString
        // insert a space before all caps
        .replace(/([A-Z])/g, ' $1')
        // uppercase the first character
        .replace(/^./, function (str) { return str.toUpperCase(); })
    return inputString;
}


function BindCustomerSearchBar($scope, $compile, dataTable) {
    var searchBar = '<div class="row searcharea"><div id="customSearch" ><div class="col-sm-12"><label class="control-label" translate="Admin_SearchCriteria">Search Criteria:</label>' +
        '<input type="text" id="txtCustomSearch" ng-model="customSearchKeyValue"  my-enter="dataTableSearch()" placeholder="Enter Search Keyword" class="form-control no-margin"></div>' +
        '</div></div>';
    $(".dataTables_filter").html(searchBar);

    $compile(angular.element("#customSearch"))($scope);

    $scope.dataTableSearchKeyUp = function (event) {
        if (event.keyCode == 13) {
            $scope.localStorageService.set($scope.localStorageCustomSearchKey, angular.element("#txtCustomSearch").val());
            dataTable.search(angular.element("#txtCustomSearch").val()).draw();
            setTimeout(function () { SetAnchorLinks() }, 200);
        }
    }
    $scope.dataTableSearch = function (event) {
        if (($scope.localStorageCustomSearchKey != undefined))
            $scope.localStorageService.set($scope.localStorageCustomSearchKey, angular.element("#txtCustomSearch").val());
        dataTable.search(angular.element("#txtCustomSearch").val()).draw();
        setTimeout(function () { SetAnchorLinks() }, 200);
    }


    if ($scope.customSearchKeyValue != undefined && $scope.customSearchKeyValue != '') {
        dataTable.search($scope.customSearchKeyValue).draw();
    }
}

function close_window() {
    if (confirm("Close Window?")) {
        window.open('', '_self', '');
        window.close();
    }
}

function isNullOrUndefinedOrEmpty(value) {
    return (value == undefined || value == null || value === '');
}

function getFloatValue(value) {
    value = parseFloat(value);
    return isNaN(value) ? 0 : value;
}



function ValidateDecimal(value, maxlength, decimalUpto) {

    if (isNaN(parseInt(value))) {
        return null;
    }

    var decimalUpto = decimalUpto > 0 ? parseInt(decimalUpto) : 0;

    var maxlength = parseInt(maxlength);

    if ((value.length > (maxlength - 1)) && decimalUpto > 0) {

        var n = value.lastIndexOf('.');

        var valueAfterDot = value.substring(n);

        if (n > 0 && valueAfterDot.length > decimalUpto) {
            valueAfterDot = valueAfterDot.substring(0, (decimalUpto + 1));
        }
        else {
            valueAfterDot = ".00";
        }
        var valueBeforeDot = n > 0 ? value.substring(0, n) : value.substring(0, (maxlength - 1) - decimalUpto);

        return valueBeforeDot + valueAfterDot;
    }
    return value;

}

///Get color code based on corrent value from ColorCodeScheme
function GetColorFromColorCodeScheme(model, currentvalue, fieldname) {

    var list = _.filter(model, { FieldName: fieldname });
    var data = _.filter(list, function (value) {
        if (value.FromValue <= currentvalue && value.ToValue >= currentvalue) {
            return value;
        }
    });

    if (data != undefined && data.length > 0) {
        return data[0].ColorCode;
    }
    else {
        var datalist = _.filter(list, function (value) {
            return value.FromValue != null && value.ToValue == null;
        });
        var detaultColor = "#32CD32";
        for (var i = 0; i < datalist.length; i++) {
            //  switch(datalist[i].Expression) {
            // case '<' :
            if (datalist[i].Expression.toUpperCase() == 'DEFAULT') {
                detaultColor = datalist[i].ColorCode;
            }
            if (datalist[i].Expression == '=' && currentvalue == datalist[i].FromValue) {
                return datalist[i].ColorCode;
                break;
            }
            else if (datalist[i].Expression == '<' && currentvalue < datalist[i].FromValue) {
                return datalist[i].ColorCode;
                break;
            }
            else if (datalist[i].Expression == '>' && currentvalue > datalist[i].FromValue) {
                return datalist[i].ColorCode;
                break;
            }
            else if (datalist[i].Expression == '>=' && currentvalue >= datalist[i].FromValue) {
                return datalist[i].ColorCode;
                break;
            }

            else if (datalist[i].Expression == '<=' && currentvalue <= datalist[i].FromValue) {
                return datalist[i].ColorCode;
                break;
            }
        }
        return detaultColor;
    }
}

//function makeCORSRequest(type, url, data)
//{
//    $.ajax({

//        // The 'type' property sets the HTTP method.
//        // A value of 'PUT' or 'DELETE' will trigger a preflight request.
//        type: type,

//        // The URL to make the request to.
//        url: url,

//        // The 'contentType' property sets the 'Content-Type' header.
//        // The JQuery default for this property is
//        // 'application/x-www-form-urlencoded; charset=UTF-8', which does not trigger
//        // a preflight. If you set this value to anything other than
//        // application/x-www-form-urlencoded, multipart/form-data, or text/plain,
//        // you will trigger a preflight request.
//     //   contentType: 'text/plain',

//        xhrFields: {
//            // The 'xhrFields' property sets additional fields on the XMLHttpRequest.
//            // This can be used to set the 'withCredentials' property.
//            // Set the value to 'true' if you'd like to pass cookies to the server.
//            // If this is enabled, your server must respond with the header
//            // 'Access-Control-Allow-Credentials: true'.
//            withCredentials: false
//        },

//        headers: {
//            // Set any custom headers here.
//            // If you set any non-simple headers, your server must include these
//            // headers in the 'Access-Control-Allow-Headers' response header.
//        },

//        success: function () {
//            // Here's where you handle a successful response.
//        },

//        error: function () {
//            // Here's where you handle an error response.
//            // Note that if the error was due to a CORS issue,
//            // this function will still fire, but there won't be any additional
//            // information about the error.
//        }
//    });
//}

app.service('configurationService', function () {

    var configService = {};

    configService.basePath = "api/";

    configService.pageSize = 10;

    configService.lengthMenu = [10, 20, 50, 100, 200];

    return configService;

});

toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": false,
    "positionClass": "toast-bottom-right",
    "preventDuplicates": true,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

function createCORSRequest(method, url) {

    var xhr = new XMLHttpRequest();
    if ("withCredentials" in xhr) {
        // XHR for Chrome/Firefox/Opera/Safari.
        xhr.open(method, url, true);
    } else if (typeof XDomainRequest != "undefined") {
        // XDomainRequest for IE.
        xhr = new XDomainRequest();
        xhr.open(method, url);
    } else {
        // CORS not supported.
        xhr = null;
    }

    xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");

    return xhr;
}


//// Make the actual CORS request.
function makeCorsRequest(type, url, data) {

    var xhr = createCORSRequest(type, url);

    if (!xhr) {
        alert('CORS not supported');
        return;
    }

    // Response handlers.
    xhr.onload = function () {

        var text = xhr.responseText;
        // var title = getTitle(text);
        alert('Response from CORS request to ' + url + ': ');
    };

    xhr.onerror = function () {
        alert('Woops, there was an error making the request.');
    };

    xhr.send(data);
}


//Serverside validation error message display.
function showValidationErrors($scope, error) {
    $scope.validationErrors = [];
    for (var key in error) {
        $scope.validationErrors.push(error[key]);
    }
}



var keylength = 4;
var specialCharacter = "~";
function encodeParams(toParams) {

    if (toParams != undefined && toParams != null) {
        $.each(toParams, function (key, element) {

            if (element != undefined && element != null) {
                if (!(element.indexOf(specialCharacter) > -1)) {

                    toParams[key] = Encodestring(element);

                    //makeid(keylength) + "~" + element + "`" + makeid(keylength);
                }
            }
        });
    }
}

function decodeParams($stateParams) {
    if ($stateParams != undefined && $stateParams != null) {
        $.each($stateParams, function (key, element) {
            if (element != undefined && element != null) {

                if (element.indexOf(specialCharacter) > -1) {
                    element = decodeURIComponent(element);
                    var decodedstring = element.substring(keylength + 1, element.length - (keylength + 1));
                    if (decodedstring.length >= 3) {

                        decodedstring = (String.fromCharCode(parseInt(decodedstring.substring(0, 3))) + decodedstring.substring(3, decodedstring.length)).toString();
                    }
                    $stateParams[key] = decodedstring;

                }
                //$stateParams[key] = decodeURIComponent(element).replace(/@/g, "");
            }
        });
    }
    setTimeout(function () { $("form").attr('autocomplete', 'off'); }, 1000);
}

function SetAnchorLinks() {

    $("a[href*='\\?']").each(function () {

        if (this.href.indexOf('?') > 0) {
            var splitUrl1 = this.href.split("?")[0];
            var splitUrl2 = this.href.split("?")[1];
            var newstring = '';
            var strquery = splitUrl2.split("&");

            for (var k = 0; k < strquery.length; k++) {
                var paramName = strquery[k].split("=")[0];
                var value = strquery[k].split("=")[1];

                newstring += paramName + "=";
                newstring += Encodestring(value) + '&';

            }
            $(this).attr("href", '');

            if (newstring != null && newstring.lastIndexOf('&') == (newstring.length - 1)) {

                newstring = newstring.substring(0, newstring.length - 1);
            }
            $(this).attr("href", splitUrl1 + '?' + newstring);

        }
    });
}

function Encodestring(value) {

    if (value != undefined && value != null && value.toString().indexOf(specialCharacter) < 0) {
        value = value.toString().trim().toString();
        var charcode = String(value.charCodeAt(0));
        charcode = paddingLeft(charcode, "000");
        value = encodeURIComponent(getRadomAlphanumeric(keylength / 2) + getRadomSpecialCharacter() + getRadomAlphanumeric(keylength / 2) + charcode + value.substring(1, value.length) + getRadomAlphanumeric(keylength / 2) + getRadomAlphanumeric(keylength - (keylength / 2)) + specialCharacter);
    }
    return value;

}

function getRadomSpecialCharacter() {
    var len = 1
    var text = "";
    //var possible = "~!@#$^*;|+-";
    var possible = "~!#$^*|+-";

    for (var i = 0; i < len; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}

function getRadomAlphanumeric(len) {
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for (var i = 0; i < len; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}

function paddingLeft(stringvalue, paddingValue) {
    return String(paddingValue + stringvalue).slice(-paddingValue.length);
};

function validatedate(dateString) {

    // First check for the pattern
    if (!/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(dateString))
        return false;

    // Parse the date parts to integers
    var parts = dateString.split("/");
    var day = parseInt(parts[1], 10);
    var month = parseInt(parts[0], 10);
    var year = parseInt(parts[2], 10);

    // Check the ranges of month and year
    if (year < 1000 || year > 3000 || month == 0 || month > 12)
        return false;

    var monthLength = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    // Adjust for leap years
    if (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0))
        monthLength[1] = 29;

    // Check the range of the day
    return day > 0 && day <= monthLength[month - 1];
};

function IsDateObject(obj) {
    var re = /^(-?(?:[1-9][0-9]*)?[0-9]{4})-(1[0-2]|0[1-9])-(3[0-1]|0[1-9]|[1-2][0-9])T(2[0-3]|[0-1][0-9]):([0-5][0-9]):([0-5][0-9])(\.[0-9]+)?(Z|[+-](?:2[0-3]|[0-1][0-9]):[0-5][0-9])?$/;
    //var re = /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/;

    if (obj != undefined && obj != null && obj != '') {
        if (re.test(obj)) {
            return true;
        }
    }
    return false;
}
var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
/// dd MMM, yyyy(31 Dec, 2016) 
function GetFormatedDate(date) {
    if (date != undefined && date != null) {
        //date = new Date(date);
        return (paddingLeft(date.getDate().toString(), "00") + " " + (monthNames[date.getMonth()]).substring(0, 3) + ", " + date.getFullYear());
    }
    return date;
}


function enableTextSelect() {
    $('input[type=text]').on('click', function () {
        $(this).select();
        $(this).focus();
    });
}


function createDatePicker(defautvalue) {
    $('.myDatePicker').bootstrapDP({
        format: 'mm/dd/yyyy',
        autoclose: true,
        todayHighlight: true,
        "setDate": defautvalue == undefined ? new Date() : ""
    });

}


function createTimePicker() {

    $('#timepicker2').timepicker();
    //$('.myTimePicker').timepicker({
    //    minuteStep: 1,
    //    template: 'modal',
    //    appendWidgetTo: 'body',
    //    showSeconds: true,
    //    showMeridian: false,
    //    defaultTime: false
    //});
}

function GetCSVFromJsonArray(objectArray, ColumnName) {

    try {
        var valueArray = [];
        objectArray.forEach(function (entry) {
            var value = eval("entry." + ColumnName);
            if (value != null && value != '') {
                valueArray.push(value);
            }

        });

        if (valueArray.length > 0) {
            return valueArray.join(",");
        }

    } catch (e) {

    }

    return "";
}

var intStringMaxLimit = 50;
function SubstringWithMaxStringlength(dataStr, maxLimit) {
    if (dataStr != null && dataStr != "" && dataStr.length > maxLimit) {
        var subString = dataStr.substring(0, maxLimit) + "...";

        return '<span class="help cursor-pointer" data-toggle="tooltip" data-original-title="' + dataStr + '">' + subString + '<span>'
    }
    return dataStr;
}

function GetDateDiff(dt1, dt2) {

    /*
     * setup 'empty' return object
     */
    var ret = { days: 0, months: 0, years: 0 };

    /*
     * If the dates are equal, return the 'empty' object
     */
    if (dt1 == dt2 || dt2 > dt1) return ret;

    /*
     * ensure dt2 > dt1
     */
    if (dt1 > dt2) {
        var dtmp = dt2;
        dt2 = dt1;
        dt1 = dtmp;
    }


    /*
     * First get the number of full years
     */

    var year1 = dt1.getFullYear();
    var year2 = dt2.getFullYear();

    var month1 = dt1.getMonth();
    var month2 = dt2.getMonth();

    var day1 = dt1.getDate();
    var day2 = dt2.getDate();

    /*
     * Set initial values bearing in mind the months or days may be negative
     */

    ret['years'] = year2 - year1;
    ret['months'] = month2 - month1;
    ret['days'] = day2 - day1;

    /*
     * Now we deal with the negatives
     */

    /*
     * First if the day difference is negative
     * eg dt2 = 13 oct, dt1 = 25 sept
     */
    if (ret['days'] < 0) {
        /*
         * Use temporary dates to get the number of days remaining in the month
         */
        var dtmp1 = new Date(dt1.getFullYear(), dt1.getMonth() + 1, 1, 0, 0, -1);

        var numDays = dtmp1.getDate();

        ret['months'] -= 1;
        ret['days'] += numDays;

    }

    /*
     * Now if the month difference is negative
     */
    if (ret['months'] < 0) {
        ret['months'] += 12;
        ret['years'] -= 1;
    }

    return ret;
}

function NumberWithMaxLengthEvents() {
    $(function () {
        setTimeout(function () {
            $("input[type='number']").change(function (e) {
                SetInputValueWithMaxLength(this);
            });
            $("input[type='number']").keypress(function (e) {
                SetInputValueWithMaxLength(this);
            });
        }, 200);
    });
}


function SetInputValueWithMaxLength(object) {
    if (object.value.length > object.maxLength)
        object.value = object.value.slice(0, object.maxLength);
}


function ReplaceCSVWithPrefix(CSV, Replace, ReplaceWith) {


    var regex = new RegExp(',', 'g');
    if (CSV != undefined || CSV != null) {
        return (CSV.replace(regex, ReplaceWith));
    }

}
/*
function encrypt(inputMessage) {
    if (inputMessage != undefined && inputMessage != null) {
        if (inputMessage.length < 44) {
            var encrypted = (CryptoJS.AES.encrypt(inputMessage, encryptionkey));
            return encrypted.ciphertext.toString()
        }


        //return encodeURIComponent(CryptoJS.AES.encrypt(inputMessage, encryptionkey));
    }
    return inputMessage;
}

function decrypt(inputMessage) {
    if (inputMessage != undefined && inputMessage != null) {
        //var decodedParam = decodeURIComponent(inputMessage);
        var decodedParam = inputMessage;
        var decrypted = CryptoJS.AES.decrypt(decodedParam, encryptionkey);
        var inputMessage = decrypted.toString(CryptoJS.enc.Utf8);
        if (inputMessage.length < 44) {
            return inputMessage;
        }
        else if (inputMessage.length >= 44) {
            decrypt(inputMessage);
        }
    }
    return inputMessage;
}
*/

Array.prototype.contains = function (v) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] === v) return true;
    }
    return false;
};

Array.prototype.unique = function () {
    var arr = [];
    for (var i = 0; i < this.length; i++) {
        if (!arr.contains(this[i])) {
            arr.push(this[i]);
        }
    }
    return arr;
}

function BindSearchCriteriaCommon(aoData, searchKey, selectedComparison, searchValue) {

    aoData.push({ 'name': 'searchKey', 'value': searchKey });
    aoData.push({ 'name': 'Operation', 'value': selectedComparison });
    aoData.push({ 'name': 'searchValue', 'value': searchValue });

    return aoData;
}

function BindSortingCommon(aoData, oSettings) {

    angular.forEach(oSettings.aaSorting, function (row, i) {
        var sortObj = new Object();
        sortObj.Column = oSettings.aoColumns[row[0]].mData;
        sortObj.Desc = row[1] == 'desc';
        aoData.push({ 'name': 'SortColumns', 'value': JSON.stringify(sortObj) });
        return;
    });
    return aoData;
}

function genRand() {
    return Math.floor(Math.random() * 89999 + 10000);
}

function checkNullOrNaN(val) {
    if (isNaN(val) || isNullOrUndefinedOrEmpty(val)) {
        return 0;
    }
    else {
        return val;
    }
}

function enablePasswordEye() {
    $(".toggle-password").click(function () {
        $(this).toggleClass("fa-eye fa-eye-slash");
        var input = $($(this).attr("toggle"));
        if (input.attr("type") == "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
    });
}

function GetFileTypeId() {
    return {
        AUTHORIZATIONS: 2,
        REQUESTS: 3,
        FACESHEETS: 18,
        FAXCOVERSHEET: 23,
        CONFIRMATIONS: 28,
        NOTICES: 29,
        CWC: 31
    };
}