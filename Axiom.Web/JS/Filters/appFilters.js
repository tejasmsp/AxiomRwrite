
app.filter('unique', function () {
    return function (collection, keyname) {
        var output = [],
            keys = [];

        angular.forEach(collection, function (item) {
            var key = item[keyname];
            if (keys.indexOf(key) === -1) {
                keys.push(key);
                output.push(item);
            }
        });
        return output;
    };
});


app.filter('sum', function () {
    return function (ListRecords, FieldForSum) {
        var total = 0;
        for (i = 0; i < ListRecords.length; i++) {
            total = total + ListRecords[i][FieldForSum];
        };
        return total;
    };
});

app.filter('sumDecimal', function () {
    return function (ListRecords, FieldForSum) {
        var total = 0;
        if (ListRecords != undefined && ListRecords != null) {
            for (i = 0; i < ListRecords.length; i++) {
                if (ListRecords[i][FieldForSum] != undefined && ListRecords[i][FieldForSum] != null && ListRecords[i][FieldForSum] != "") {
                    total = total + parseFloat(ListRecords[i][FieldForSum]);
                }
            };
        }
        return total;
    };
});

app.filter('avgDecimal', function () {
    return function (listRecords, fieldForSum) {
        if (!isNullOrUndefinedOrEmpty(listRecords)) {
            
            var total = 0;
            var length = listRecords.length;

            if (length == undefined) {
                length = 0;
            }

            for (var i = 0; i < listRecords.length; i++) {
                if (listRecords[i][fieldForSum] != undefined && listRecords[i][fieldForSum] != null && listRecords[i][fieldForSum] != "") {
                    total = total + parseFloat(listRecords[i][fieldForSum]);
                }
            };

            if (length > 0)
                return (total / length);
            else
                return 0;
        }
        return 0;
    };
});

app.filter('avgDecimalExceptBlankElement', function () {
    return function (listRecords, fieldForSum) {
        if (!isNullOrUndefinedOrEmpty(listRecords)) {

            var total = 0;
           

          var listNew =  _.filter(listRecords, function (value) {
                return !isNullOrUndefinedOrEmpty(value.AvgAge) ;
            });

             var length = listNew.length;


            if (length == undefined) {
                length = 0;
            }

            for (var i = 0; i < listRecords.length; i++) {
                if (listRecords[i][fieldForSum] != undefined && listRecords[i][fieldForSum] != null && listRecords[i][fieldForSum] != "") {
                    total = total + parseFloat(listRecords[i][fieldForSum]);
                }
            };

            if (length > 0)
                return (total / length);
            else
                return 0;
        }
        return 0;
    };
});


app.filter('maxDecimal', function () {
    return function (ListRecords, FieldForSum) {
        var total = 0;
        var length = ListRecords.length;
        if (length == undefined || length == null) {
            length = 0;
        }
        if (ListRecords != undefined && ListRecords != null) {
            for (i = 0; i < ListRecords.length; i++) {

                if (ListRecords[i][FieldForSum] != undefined && ListRecords[i][FieldForSum] != null && ListRecords[i][FieldForSum] != "") {
                    if (parseFloat(ListRecords[i][FieldForSum]) > total) {
                        total = parseFloat(ListRecords[i][FieldForSum]);
                    }
                }
            };
        }
        if (length > 0)
            return total;
        else
            return 0;
    };
});

app.filter('minDecimal', function () {
    return function (ListRecords, FieldForSum) {
        var total = 0;
        var length = ListRecords.length;
        if (length == undefined || length == null) {
            length = 0;
        }
        if (ListRecords != undefined && ListRecords != null && ListRecords.length > 0) {
            total = ListRecords[0][FieldForSum];
            for (i = 0; i < ListRecords.length; i++) {

                if (ListRecords[i][FieldForSum] != undefined && ListRecords[i][FieldForSum] != null && ListRecords[i][FieldForSum] != "") {
                    if (total > parseFloat(ListRecords[i][FieldForSum])) {
                        total = parseFloat(ListRecords[i][FieldForSum]);
                    }
                }
            };
        }
        if (length > 0)
            return total;
        else
            return 0;
    };
});

app.filter('removeHTMLTags', function () {
    return function (text) {
        return text ? String(text).replace(/<[^>]+>/gm, '') : '';
    };
});

app.filter('notInArray', function ($filter) {
    return function (list, arrayFilter, element, AcknowledgementQuestionRuleId) {
        if (arrayFilter) {
            return $filter("filter")(list, function (listItem) {
                for (var i = 0; i < arrayFilter.length; i++) {
                    if (arrayFilter[i][element] == listItem[element] && AcknowledgementQuestionRuleId == 0)
                        return false;
                }
                return true;
            });
        }
    };
});


app.filter('toArray', function () {
    return function (obj, addKey) {
        if (!angular.isObject(obj)) return obj;
        if (addKey === false) {
            return Object.keys(obj).map(function (key) {
                return obj[key];
            });
        } else {
            return Object.keys(obj).map(function (key) {
                var value = obj[key];
                return angular.isObject(value) ?
                  Object.defineProperty(value, '$key', { enumerable: false, value: key }) :
          { $key: key, $value: value };
            });
        }
    };
});

app.filter('toInt', function () {
    return function (value) {
        if (isNullOrUndefinedOrEmpty(value)) {
            return 0;
        } else {
            return isNaN(parseInt(value)) ? 0 : parseInt(value);
        }
    };
});


app.filter('makePositive', function () {
    return function (num) { return Math.abs(num); }
});

app.filter('true_false', function () {
    return function (text, length, end) {     
        if (text) {
            return 'Yes';
        }
        return 'No';
    }
});
app.filter('mathPow', function () {
    return function (base, exponent) {
        return Math.pow(base, exponent);
    }
});

app.filter('FindValFromArray', function () {
    return function (List, SearchField, SearchValue, ReturnField) {
        var obj = new Object();
        for (i = 0; i < List.length; i++) {
            if (List[i][SearchField] == SearchValue) {
                obj = List[i];
                return obj[ReturnField];
            }
        };
        return "";
       
    };
});


app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });