/*
 * Autor: Alex Figueiredo
 * Name: datepicker
 *
 * Descrição: 
 * Adiciona um calendário a um input.
 *
 * Exemplo:
 * <input datepicker />
 *
*/

'use strict';

angular.module('directives').directive('datepicker', ['$timeout', 'dateFilter', function ($timeout, $dateFilter) {

    // Private
    function isASPDate(date) {
        return /^\/?Date\((\-?\d+)/i.exec(date);
    }

    function isValidDate(date) {

        if (date && typeof (date) === "string")
            return date.match(/^\d{2}([\/])\d{2}\1\d{4}$/) !== null;
        else
            return false;
    }
    
    function processDate(date) {
        if (isASPDate(date))
            return new Date(parseInt(date.substr(6)));
        else
            return date;
    }

    function reverseDate(date)
    {
        if (date)
            return date.split("/").reverse().join("/");
    }

    return {
        restrict: 'A',
        require: '?ngModel',
        link: function ($scope, elem, attrs, ngModel) {

            // $.fn.datepicker options
            var options = angular.extend({ autoclose: true });
            angular.forEach(['format', 'weekStart', 'calendarWeeks', 'startDate', 'endDate', 'daysOfWeekDisabled', 'autoclose', 'startView', 'minViewMode', 'todayBtn', 'todayHighlight', 'keyboardNavigation', 'language', 'forceParse'], function (key) {
                if (angular.isDefined(attrs[key]))
                {
                    options[key] = attrs[key];
                }
            });

            // Parsers e Formatters
            if (ngModel)
            {

                /* Array of functions to execute, as a pipeline, whenever the model value changes.
                 * Each function is called, in turn, passing the value through to the next.
                 * Used to format/convert values for display in the control and validation. */
                ngModel.$formatters.unshift(function (modelValue) {
                    if (modelValue)
                    {
                        return $dateFilter(isASPDate(modelValue) ? processDate(modelValue) : modelValue, 'dd/MM/yyyy');
                    }
                });

                /* Array of functions to execute, as a pipeline, whenever the control reads value from the DOM.
                 * Each function is called, in turn, passing the value through to the next. Used to sanitize / convert the value as well as validation.
                 * For validation, the parsers should update the validity state using $setValidity(), and return undefined for invalid values. */
                ngModel.$parsers.unshift(function (viewValue) {
                    
                    if (viewValue && isValidDate(viewValue))
                    {
                        ngModel.$setValidity('date', true);
                        return $dateFilter(processDate(reverseDate(viewValue)), 'yyyy/MM/dd');
                    }
                    else
                    {
                        ngModel.$setValidity('date', false);

                        if (!viewValue)
                            return undefined;
                        return 'Invalid Date';

                    }
                });

                /* Called when the view needs to be updated.
                 * It is expected that the user of the ng-model directive will implement this method. */
                ngModel.$render = function () {
                    if (ngModel.$viewValue)
                    {
                        var viewValue = isValidDate(ngModel.$viewValue) ? ngModel.$viewValue : reverseDate(ngModel.$viewValue);
                        return elem.val(viewValue);
                    }
                    else
                    {
                        elem.val('');
                        return false;
                    }
                };
            }

            // Binds
            if (ngModel)
            {
                // Trata o valor inicial
                ngModel.$viewValue = processDate(ngModel.$viewValue);
                var init_value = $scope.$watch(function () { return ngModel.$modelValue; }, function (modelValue) {
                    if (modelValue)
                    {
                        if (isASPDate(modelValue))
                        {
                            ngModel.$setViewValue($dateFilter(processDate(modelValue), 'dd/MM/yyyy'));
                            init_value(); // Remove o $watch
                        }
                    }
                });

                elem.on('changeDate', function () {
                        ngModel.$setViewValue(elem.val());
                });
            }


            elem.blur(function () {
                if (!isValidDate(ngModel.$viewValue)) {
                    elem.val('')
                    ngModel.$setViewValue(undefined);
                    ngModel.$viewValue = undefined;
                }
                    
            });

            // Cria o datepicker
            elem.datepicker(
                angular.extend(options, {
                    format: "dd/mm/yyyy",
                    language: "pt-BR"
                })).attr("maxlength", 10);                

            // Coleta de lixo
            $scope.$on('$destroy', function () {
                var datepicker = elem.data('datepicker');
                if (datepicker)
                {
                    datepicker.picker.remove();
                    elem.data('datepicker', null);
                }
            });

            // Altera o start-date quando a propriedade modificar
            attrs.$observe('startDate', function (value) {
                elem.datepicker('setStartDate', value);
            });

            // Altera o end-date quando a propriedade modificar
            attrs.$observe('endDate', function (value) {
                elem.datepicker('setEndDate', value);
            });

        }
    };
}]);