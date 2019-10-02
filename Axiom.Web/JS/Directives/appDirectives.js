app.directive('loading', ['$http', function ($http) {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs) {

            scope.isLoading = function () {
                return $http.pendingRequests.length > 0;
            };

            scope.$watch(scope.isLoading, function (v) {
                if (v) {
                    elm.show();
                } else {
                    elm.hide();
                }
            });
        }
    };

}]);

app.directive('myEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.myEnter);
                });
                event.preventDefault();
            }
        });
    };
});

app.directive('closeNotification', function () {
    return function (scope, element) {
        element.bind('click', function () {
            angular.element(element).parent('.alert').hide();
        });
    };
});

app.directive('customOnChange', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var onChangeHandler = scope.$eval(attrs.customOnChange);
            element.bind('change', onChangeHandler);
        }
    };
});

app.directive('validNumber', function ($parse) {

    return function (scope, element, attrs) {

        element.bind("keypress", function (event) {

            scope.$apply(function () {
                if (isNaN(element.val())) {
                    element.val(null);
                    event.preventDefault();
                }
            });
        });
    }
});

app.directive('dropdownMultiselect', function () {
    return {
        restrict: 'E',
        scope: {
            model: '=',
            options: '=',
            selected: '='
        },
        template:
                "<div class='btn-group multiselectDropdown' data-ng-class='{open: open}'>" +
                    "<button class='btn btn-small' data-ng-click='openDropdown()' type='button'>Select...</button>" +
                    "<button class='btn btn-smalldropdown-toggle'  type='button' data-ng-click='openDropdown()'><span class='caret'></span></button>" +
    "<ul class='dropdown-menu' aria-labelledby='dropdownMenu' style='max-height: 300px;overflow: scroll;'>" + "<li><a data-ng-click='selectAll()'><span class='glyphicon glyphicon-ok green' aria-hidden='true'></span> Check All</a></li>" +
    "<li><a data-ng-click='deselectAll();'><span class='glyphicon glyphicon-remove red' aria-hidden='true'></span> Uncheck All</a></li>" +
    "<li class='divider'></li>" + "<li data-ng-repeat='option in options'><a data-ng-click='toggleSelectItem(option)'><span data-ng-class='getClassName(option)' aria-hidden='true'></span> {{option.name}}</a></li>" +
    "</ul>",
//"</div><strong class='margin-left-5'>{{selected}} Selected</strong>",
        controller: function ($scope) {
            $scope.openDropdown = function () {
                $scope.open = !$scope.open;
            };

            $scope.selectAll = function () {
                $scope.model = [];
                angular.forEach($scope.options, function (item, index) {
                    $scope.model.push(item.id);
                });
            };

            $scope.deselectAll = function () {
                $scope.model = [];
            };

            $scope.toggleSelectItem = function (option) {
                if ($scope.model == undefined || $scope.model == '') {
                    $scope.model = [];
                }
                var intIndex = -1;
                angular.forEach($scope.model, function (item, index) {
                    if (item == option.id) {
                        intIndex = index;
                    }
                });

                if (intIndex >= 0) {
                    $scope.model.splice(intIndex, 1);
                }
                else {
                    $scope.model.push(option.id);
                }
            };

            $scope.getClassName = function (option) {

                var varClassName = 'glyphicon glyphicon-remove';
                angular.forEach($scope.model, function (item, index) {
                    if (item == option.id) {
                        varClassName = 'glyphicon glyphicon-ok green';
                    }
                });
                return (varClassName);
            };
        }
    }
});

app.directive('noDirty', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            ngModelCtrl.$setDirty = angular.noop;
        }
    }
});

app.directive('ngJsonExportExcel', function () {
    return {
        restrict: 'AE',
        scope: {
            data: '=',
            filename: '=?',
            reportFields: '=',
            separator: '@'
        },
        link: function (scope, element) {
            scope.filename = !!scope.filename ? scope.filename : 'export-excel';

            var fields = [];
            var header = [];
            var separator = scope.separator || ';';

            angular.forEach(scope.reportFields, function (field, key) {
                if (!field || !key) {
                    throw new Error('error json report fields');
                }

                fields.push(key);
                header.push(field);
            });

            element.bind('click', function () {
                var bodyData = _bodyData();
                var strData = _convertToExcel(bodyData);

                var blob = new Blob([strData], { type: "text/plain;charset=utf-8" });

                return saveAs(blob, [scope.filename + '.csv']);
            });

            function _bodyData() {
                var data = scope.data;
                var body = "";
                angular.forEach(data, function (dataItem) {
                    var rowItems = [];

                    angular.forEach(fields, function (field) {
                        if (field.indexOf('.')) {
                            field = field.split(".");
                            var curItem = dataItem;

                            // deep access to obect property
                            angular.forEach(field, function (prop) {
                                if (curItem !== null && curItem !== undefined) {
                                    curItem = curItem[prop];
                                }
                            });

                            data = curItem;
                        }
                        else {
                            data = dataItem[field];
                        }

                        var fieldValue = data !== null ? data : ' ';

                        if (fieldValue !== undefined && angular.isObject(fieldValue)) {
                            fieldValue = _objectToString(fieldValue);
                        }

                        if (typeof fieldValue == 'string') {
                            rowItems.push('"' + fieldValue.replace(/"/g, '""') + '"');
                        } else {
                            rowItems.push(fieldValue);
                        }
                    });

                    body += rowItems.join(separator) + '\n';
                });

                return body;
            }

            function _convertToExcel(body) {
                return header.join(separator) + '\n' + body;
            }

            function _objectToString(object) {
                var output = '';
                angular.forEach(object, function (value, key) {
                    output += key + ':' + value + ' ';
                });

                return '"' + output + '"';
            }
        }
    };
});

app.directive('date', function (dateFilter) {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {

            var dateFormat = attrs['date'] || 'MM/dd/yyyy';

            ctrl.$formatters.unshift(function (modelValue) {
                return dateFilter(modelValue, dateFormat);
            });
        }
    };
});

app.directive('fixedHeader', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        scope: {
            tableHeight: '@'
        },
        link: function ($scope, $elem, $attrs, $ctrl) {
            function isVisible(el) {
                var style = window.getComputedStyle(el);
                return (style.display != 'none' && el.offsetWidth != 0);
            }

            function isTableReady() {
                return isVisible(elem.querySelector("tbody")) && elem.querySelector('tbody tr:first-child') != null;
            }

            var elem = $elem[0];
            // wait for content to load into table and to have at least one row, tdElems could be empty at the time of execution if td are created asynchronously (eg ng-repeat with promise)
            var unbindWatch = $scope.$watch(isTableReady,
                function (newValue, oldValue) {
                    if (newValue === true) {
                        // reset display styles so column widths are correct when measured below
                        angular.element(elem.querySelectorAll('thead, tbody, tfoot')).css('display', '');

                        // wrap in $timeout to give table a chance to finish rendering
                        $timeout(function () {
                            // set widths of columns
                            angular.forEach(elem.querySelectorAll('tr:first-child th'), function (thElem, i) {

                                var tdElems = elem.querySelector('tbody tr:first-child td:nth-child(' + (i + 1) + ')');
                                var tfElems = elem.querySelector('tfoot tr:first-child td:nth-child(' + (i + 1) + ')');


                                var columnWidth = tdElems ? tdElems.offsetWidth : thElem.offsetWidth;
                                if (tdElems) {
                                    tdElems.style.width = columnWidth + 'px';
                                }
                                if (thElem) {
                                    thElem.style.width = columnWidth + 'px';
                                }
                                if (tfElems) {
                                    tfElems.style.width = columnWidth + 'px';
                                }
                            });

                            // set css styles on thead and tbody
                            angular.element(elem.querySelectorAll('thead, tfoot')).css('display', 'block');

                            angular.element(elem.querySelectorAll('tbody')).css({
                                'display': 'block',
                                'height': $scope.tableHeight || 'inherit',
                                'overflow': 'auto'
                            });


                            // reduce width of last column by width of scrollbar
                            var tbody = elem.querySelector('tbody');
                            var scrollBarWidth = tbody.offsetWidth - tbody.clientWidth;
                            if (scrollBarWidth > 0) {
                                // for some reason trimming the width by 2px lines everything up better
                                scrollBarWidth -= 2;
                                var lastColumn = elem.querySelector('tbody tr:first-child td:last-child');
                                lastColumn.style.width = (lastColumn.offsetWidth - scrollBarWidth) + 'px';
                            }
                        });

                        //we only need to watch once
                        unbindWatch();
                    }
                });
        }
    };
}]);

app.directive('staticInclude', function ($http, $templateCache, $compile) {
    return function (scope, element, attrs) {
        var templatePath = attrs.staticInclude;
        //$http.get(templatePath + "?v=" + ApplicationVersion, { cache: $templateCache }).success(function (response) {
        $http.get(templatePath + "?v=" + ApplicationVersion, { cache: false }).success(function (response) {
            var contents = element.html(response).contents();
            $compile(contents)(scope);
        });
    };
});

app.directive('validateEmail', function () {
    //var EMAIL_REGEXP = /^[_a-z0-9]+(\.[_a-z0-9]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/;
    var EMAIL_REGEXP = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;

    return {
        require: 'ngModel',
        restrict: '',
        link: function (scope, elm, attrs, ctrl) {
            // only apply the validator if ngModel is present and Angular has added the email validator
            if (ctrl && ctrl.$validators.email) {

                // this will overwrite the default Angular email validator
                ctrl.$validators.email = function (modelValue) {
                    return ctrl.$isEmpty(modelValue) || EMAIL_REGEXP.test(modelValue);
                };
            }
        }
    };
});

app.directive('noDirty', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            // override the $setDirty method on ngModelController
            ngModelCtrl.$setDirty = angular.noop;
        }
    }
});

app.directive('decimalOnly', function (localStorageService) {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            elm.on('keydown', function (event, attrs) {
                if (event.key == "Decimal") {
                    event.key = '.';
                }

                var IsValidKeyPress = false;
                var $input = $(this);
                var value = $input.val();
                var collection = $input.attr("decimal-only").split(' ');
                var controlType = collection[0];
                var decimalPlaces = 0;
                if (collection.length > 1) {
                    decimalPlaces = parseFloat(collection[1]);
                }
                value = value.replace(/[^0-9\.]/g, '');
                var findsDot = new RegExp(/\./g);

                var containsDot = value.match(findsDot)
                if (containsDot != null && ([110, 190].indexOf(event.which) > -1)) {
                    event.preventDefault();
                    return false;
                }

                if ([8, 13, 27, 36, 37, 38, 39, 40, 46, 110].indexOf(event.which) > -1) {
                    // backspace, enter, escape, arrows  
                    IsValidKeyPress = true;
                }

                $input.val(value);
                if (event.which == 64 || event.which == 16) {
                    // numbers  
                    IsValidKeyPress = false;
                } if ([8, 13, 27, 36, 37, 38, 39, 40, 110, 9, 17, 46].indexOf(event.which) > -1) {
                    // backspace, enter, escape, arrows , t ab , ctrl , delete 
                    IsValidKeyPress = true;
                } else if (event.which >= 48 && event.which <= 57) {
                    // numbers  
                    IsValidKeyPress = true;
                } else if (event.which >= 65 && event.which <= 90) {
                    // a to z letters  
                    IsValidKeyPress = false;
                } else if (event.which >= 96 && event.which <= 105) {
                    // numpad number  
                    IsValidKeyPress = true;
                } else if ([110, 190].indexOf(event.which) > -1) {
                    // dot and numpad dot  
                    IsValidKeyPress = true;
                }
                else {
                    IsValidKeyPress = false;
                }

                //if ((value.substring(value.indexOf('.')).length > 4) && (value.indexOf('.') >= -1) && event.which != 110) {
                //    if (isAlphaNumericKeyPressed(event.which)) {
                //        event.preventDefault();
                //    }
                //}

                var value1 = value;
                if (event.keyCode != 190 && event.key != undefined && event.key.length >= 1) {
                    value1 = value + event.key;
                }

                valueCollection = value1.split('.');

                if ((valueCollection.length > 1 && valueCollection[1].length > decimalPlaces) && event.which != 110) {
                    if (isAlphaNumericKeyPressed(event.which)) {
                        //if ((value.substring(this.selectionStart, this.selectionEnd)).length > 0) {
                        //    $input.val("");
                        //}
                        if ($input.is(':focus')) {
                            $input.val("");
                        }
                        else {
                            event.preventDefault();
                        }

                    }
                }
                if (controlType == "temp") {
                    if ((localStorageService.get("LengthUnitId") == 1 && valueCollection[0].length > 5) || (localStorageService.get("LengthUnitId") == 2 && valueCollection[0].length > 4)) {
                        if (isAlphaNumericKeyPressed(event.which)) {
                            event.preventDefault();
                        }
                    }
                }
                else if (controlType == "number") {
                    if ((localStorageService.get("LengthUnitId") == 1 && valueCollection[0].length > 8) || (localStorageService.get("LengthUnitId") == 2 && valueCollection[0].length > 10)) {
                        if (isAlphaNumericKeyPressed(event.which)) {
                            event.preventDefault();
                        }
                    }
                }

                if (!IsValidKeyPress) {
                    event.preventDefault();
                }
            });

            function isAlphaNumericKeyPressed(keyCode) {
                return ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 96 && keyCode <= 105) || [110, 190].indexOf(keyCode) > -1)

            }

        }
    }
});

app.directive('integerOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    var transformedInput = text.replace(/[^0-9-]/g, '');
                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return "";
            }
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
});

app.directive("treeModel", function ($compile) {
    return {
        restrict: "A", link: function (b, h, c) {
            var a = c.treeId,
                g = c.treeModel,
                e = c.nodeLabel || "label",
                d = c.nodeChildren || "children",
                ex = c.nodeExpanded || "isExpanded",
                e = '<ul><li data-ng-repeat="node in ' + g + '"><i class="collapsed" data-ng-show="node.' + d + '.length && node.collapsed" data-ng-click="' + a + '.selectNodeHead(node)"></i><i class="expanded" data-ng-show="node.' + d + '.length && !node.collapsed" data-ng-click="' + a + '.selectNodeHead(node)"></i><i class="normal" data-ng-hide="node.' +
                    d + '.length"></i> <span data-ng-class="node.selected" data-ng-click="' + a + '.selectNodeLabel(node); ' + a + '.selectNodeHead(node)">{{node.' + e + '}}</span><div data-ng-hide="node.collapsed" data-ng-init="node.collapsed=!node.' + ex + '" data-tree-id="' + a + '" data-tree-model="node.' + d + '" data-node-id=' + (c.nodeId || "id") + " data-node-label=" + e + " data-node-children=" + d + "></div></li></ul>";
            a && g && (c.angularTreeview && (b[a] = b[a] || {}, b[a].selectNodeHead = b[a].selectNodeHead || function (a) {
                a.collapsed = !a.collapsed
            }, b[a].selectNodeLabel = b[a].selectNodeLabel || function (c) {
                b[a].currentNode && b[a].currentNode.selected &&
                    (b[a].currentNode.selected = void 0);
                c.selected = "selected";
                b[a].currentNode = c
            }), h.html('').append($compile(e)(b)))
        }
    }
});

app.directive('deleteIfEmpty', function () {
    return {
        restrict: 'A',
        scope: {
            ngModel: '='
        },
        link: function (scope, element, attrs) {
            scope.$watch("ngModel", function (newValue, oldValue) {
                if (typeof scope.ngModel !== 'undefined' && scope.ngModel.length === 0) {
                    delete scope.ngModel;
                }
            });
        }
    };
});


app.directive('ngFileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.ngFileModel);
            var isMultiple = attrs.multiple;
            var modelSetter = model.assign;
            element.bind('change', function () {
                var values = [];
                angular.forEach(element[0].files, function (item) {
                    var value = {
                        // File Name 
                        name: item.name,
                        //File Size 
                        size: item.size,
                        //File URL to view 
                        url: URL.createObjectURL(item),
                        // File Input Value 
                        _file: item
                    };
                    values.push(value);
                });
                scope.$apply(function () {
                    if (isMultiple) {
                        modelSetter(scope, values);
                    } else {
                        modelSetter(scope, values[0]);
                    }
                });
            });
        }
    };
}]);

app.directive('tooltip', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.hover(function () {
                // on mouseenter
                element.tooltip('show');
            }, function () {
                // on mouseleave
                element.tooltip('hide');
            });
        }
    };
});

app.value('customSelectDefaults', {
    displayText: '-- Select --',
    emptyListText: 'There are no items to display',
    emptySearchResultText: 'No results match "$0"',
    addText: 'Add',
    searchDelay: 100
});

app.directive('customSelect', ['$parse', '$compile', '$timeout', '$q', 'customSelectDefaults', function ($parse, $compile, $timeout, $q, baseOptions) {
    var CS_OPTIONS_REGEXP = /^\s*(.*?)(?:\s+as\s+(.*?))?\s+for\s+(?:([\$\w][\$\w\d]*))\s+in\s+([\s\S]+?)(?:\s+track\s+by\s+([\s\S]+?))?$/;

    return {
        restrict: 'AE',
        require: 'ngModel',
        link: function (scope, elem, attrs, controller) {
            var customSelect = attrs.customSelect;
            if (!customSelect) {
                throw new Error('Expected custom-select attribute value.');
            }
            var match = customSelect.match(CS_OPTIONS_REGEXP);

            if (!match) {
                throw new Error("Expected expression in form of " +
                    "'_select_ (as _label_)? for _value_ in _collection_[ track by _id_]'" +
                    " but got '" + customSelect + "'.");
            }

            elem.addClass('dropdown custom-ddl-select');

            // Ng-Options break down
            
            var displayFn = $parse(match[2] || match[1]),
                valueName = match[3],
                valueFn = $parse(match[2] ? match[1] : valueName),
                values = match[4],
                valuesFn = $parse(values),
                track = match[5],
                trackByExpr = track ? " track by " + track : "",
                dependsOn = attrs.csDependsOn;

            var options = getOptions(),
                timeoutHandle,
                lastSearch = '',
                focusedIndex = -1,
                matchMap = {};

            var itemTemplate = elem.html().trim() || '{{' + (match[2] || match[1]) + '}}',

                dropdownTemplate =
                    '<a class="dropdown-toggle " data-toggle="dropdown" href ng-class="{ disabled: disabled }">' +
                    '<i class="icon-arrow-down5" id="ddlSelectarrow" style="position: absolute;top: 8px;right: 8px;"></i>'+
                    '<span>{{displayText}}</span>' +
                    '<b></b>' +
                    '</a>' +
                    '<div class="dropdown-menu">' +
                    '<div stop-propagation="click" class="custom-ddl-select-search">' +
                    '<i class="glyphicon glyphicon-search" style="position: absolute;top: 8px;right: 8px;"></i>' +
                    '<input class="form-control" type="text" autocomplete="off" ng-model="searchTerm" />' +
                    '</div>' +
                    '<ul role="menu">' +
                    '<li role="presentation" ng-repeat="' + valueName + ' in matches' + trackByExpr + '">' +
                    '<a role="menuitem" tabindex="-1" href ng-click="select(' + valueName + ')">' +
                    itemTemplate +
                    '</a>' +
                    '</li>' +
                    '<li ng-hide="matches.length" class="empty-result" stop-propagation="click">' +
                    '<em class="muted">' +
                    '<span ng-hide="searchTerm">{{emptyListText}}</span>' +
                    '<span class="word-break" ng-show="searchTerm">{{ format(emptySearchResultText, searchTerm) }}</span>' +
                    '</em>' +
                    '</li>' +
                    '</ul>' +
                    '<div class="custom-ddl-select-action">' +
                    (typeof options.onAdd === "function" ?
                        '<button type="button" class="btn btn-primary btn-block add-button" ng-click="add()">{{addText}}</button>' : '') +
                    '</div>' +
                    '</div>';

            // Clear element contents
            elem.empty();

            // Create dropdown element
            var dropdownElement = angular.element(dropdownTemplate),
                anchorElement = dropdownElement.eq(0).dropdown(),
                inputElement = dropdownElement.eq(1).find(':text'),
                ulElement = dropdownElement.eq(1).find('ul');

            // Create child scope for input and dropdown
            var childScope = scope.$new(true);
            configChildScope();

            // Click event handler to set initial values and focus when the dropdown is shown
            anchorElement.on('click', function (event) {
                if (childScope.disabled) {
                    return;
                }
                childScope.$apply(function () {
                    lastSearch = '';
                    childScope.searchTerm = '';
                });

                focusedIndex = -1;
                inputElement.focus();

                // If filter is not async, perform search in case model changed
                if (!options.async) {
                    getMatches('');
                }
            });

            if (dependsOn) {
                scope.$watch(dependsOn, function (newVal, oldVal) {
                    if (newVal !== oldVal) {
                        childScope.matches = [];
                        childScope.select(undefined);
                    }
                });
            }

            // Event handler for key press (when the user types a character while focus is on the anchor element)
            anchorElement.on('keypress', function (event) {
                if (!(event.altKey || event.ctrlKey)) {
                    anchorElement.click();
                }
            });

            // Event handler for Esc, Enter, Tab and Down keys on input search
            inputElement.on('keydown', function (event) {
                if (!/(13|27|40|^9$)/.test(event.keyCode)) return;
                event.preventDefault();
                event.stopPropagation();

                switch (event.keyCode) {
                    case 27: // Esc
                        anchorElement.dropdown('toggle');
                        break;
                    case 13: // Enter
                        selectFromInput();
                        break;
                    case 40: // Down
                        focusFirst();
                        break;
                    case 9:// Tab
                        anchorElement.dropdown('toggle');
                        break;
                }
            });

            // Event handler for Up and Down keys on dropdown menu
            ulElement.on('keydown', function (event) {
                if (!/(38|40)/.test(event.keyCode)) return;
                event.preventDefault();
                event.stopPropagation();

                var items = ulElement.find('li > a');

                if (!items.length) return;
                if (event.keyCode == 38) focusedIndex--;                                    // up
                if (event.keyCode == 40 && focusedIndex < items.length - 1) focusedIndex++; // down
                //if (!~focusedIndex) focusedIndex = 0;

                if (focusedIndex >= 0) {
                    items.eq(focusedIndex)
                        .focus();
                } else {
                    focusedIndex = -1;
                    inputElement.focus();
                }
            });

            resetMatches();

            // Compile template against child scope
            $compile(dropdownElement)(childScope);
            elem.append(dropdownElement);

            // When model changes outside of the control, update the display text
            controller.$render = function () {
                setDisplayText();
            };

            // Watch for changes in the default display text
            childScope.$watch(getDisplayText, setDisplayText);

            childScope.$watch(function () { return elem.attr('disabled'); }, function (value) {
                childScope.disabled = value;
            });

            childScope.$watch('searchTerm', function (newValue) {
                if (timeoutHandle) {
                    $timeout.cancel(timeoutHandle);
                }

                var term = (newValue || '').trim();
                timeoutHandle = $timeout(function () {
                    getMatches(term);
                },
                    // If empty string, do not delay
                    (term && options.searchDelay) || 0);
            });

            // Support for autofocus
            if ('autofocus' in attrs) {
                anchorElement.focus();
            }

            var needsDisplayText;
            function setDisplayText() {
                var locals = {};
                locals[valueName] = controller.$modelValue;
                var text = displayFn(scope, locals);

                if (text === undefined) {
                    var map = matchMap[hashKey(controller.$modelValue)];
                    if (map) {
                        text = map.label;
                    }
                }

                needsDisplayText = !text;
                childScope.displayText = text || options.displayText;
            }

            function getOptions() {
                return angular.extend({}, baseOptions, scope.$eval(attrs.customSelectOptions));
            }

            function getDisplayText() {
                options = getOptions();
                return options.displayText;
            }

            function focusFirst() {
                var opts = ulElement.find('li > a');
                if (opts.length > 0) {
                    focusedIndex = 0;
                    opts.eq(0).focus();
                }
            }

            // Selects the first element on the list when the user presses Enter inside the search input
            function selectFromInput() {
                var opts = ulElement.find('li > a');
                if (opts.length > 0) {
                    var ngRepeatItem = opts.eq(0).scope();
                    var item = ngRepeatItem[valueName];
                    childScope.$apply(function () {
                        childScope.select(item);
                    });
                    anchorElement.dropdown('toggle');
                }
            }

            function getMatches(searchTerm) {
                var locals = { $searchTerm: searchTerm }
                $q.when(valuesFn(scope, locals)).then(function (matches) {
                    if (!matches) return;

                    if (searchTerm === inputElement.val().trim()/* && hasFocus*/) {
                        matchMap = {};
                        childScope.matches.length = 0;
                        for (var i = 0; i < matches.length; i++) {
                            locals[valueName] = matches[i];
                            var value = valueFn(scope, locals),
                                label = displayFn(scope, locals);

                            matchMap[hashKey(value)] = {
                                value: value,
                                label: label/*,
									model: matches[i]*/
                            };

                            childScope.matches.push(matches[i]);
                        }
                        //childScope.matches = matches;
                    }

                    if (needsDisplayText) setDisplayText();
                }, function () {
                    resetMatches();
                });
            }

            function resetMatches() {
                childScope.matches = [];
                focusedIndex = -1;
            };

            function configChildScope() {
                childScope.addText = options.addText;
                childScope.emptySearchResultText = options.emptySearchResultText;
                childScope.emptyListText = options.emptyListText;

                childScope.select = function (item) {
                    var locals = {};
                    locals[valueName] = item;
                    var value = valueFn(childScope, locals);
                    //setDisplayText(displayFn(scope, locals));
                    childScope.displayText = displayFn(childScope, locals) || options.displayText;
                    controller.$setViewValue(value);

                    anchorElement.focus();

                    typeof options.onSelect === "function" && options.onSelect(item);
                };

                childScope.add = function () {
                    $q.when(options.onAdd(), function (item) {
                        if (!item) return;

                        var locals = {};
                        locals[valueName] = item;
                        var value = valueFn(scope, locals),
                            label = displayFn(scope, locals);

                        matchMap[hashKey(value)] = {
                            value: value,
                            label: label/*,
									model: matches[i]*/
                        };

                        childScope.matches.push(item);
                        childScope.select(item);
                    });
                };

                childScope.format = format;

                setDisplayText();
            }

            var current = 0;
            function hashKey(obj) {
                if (obj === undefined) return 'undefined';

                var objType = typeof obj,
                    key;

                if (objType == 'object' && obj !== null) {
                    if (typeof (key = obj.$$hashKey) == 'function') {
                        // must invoke on object to keep the right this
                        key = obj.$$hashKey();
                    } else if (key === undefined) {
                        key = obj.$$hashKey = 'cs-' + (current++);
                    }
                } else {
                    key = obj;
                }

                return objType + ':' + key;
            }
        }
    };
}]);

function format(value, replace) {
    if (!value) {
        return value;
    }
    var target = value.toString();
    if (replace === undefined) {
        return target;
    }
    if (!angular.isArray(replace) && !angular.isObject(replace)) {
        return target.split('$0').join(replace);
    }
    var token = angular.isArray(replace) && '$' || ':';

    angular.forEach(replace, function (value, key) {
        target = target.split(token + key).join(value);
    });
    return target;
}
