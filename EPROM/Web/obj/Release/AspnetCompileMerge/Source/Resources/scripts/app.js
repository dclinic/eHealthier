var app = angular.module('app', ['angular.filter', 'ngMaterial', 'ngRoute', 'angularFileUpload', 'ngAnimate', 'ui.bootstrap', 'toaster', 'ngBootbox', 'ngMessages', 'ngCookies', 'ngMessages'])

.directive('fallbackSrc', function () {
    var fallbackSrc = {
        link: function postLink(scope, iElement, iAttrs) {
            iElement.bind('error', function () {
                angular.element(this).attr("src", iAttrs.fallbackSrc);
            });
        }
    }
    return fallbackSrc;
})

 .directive('numbersOnly', function () {
     return {
         require: 'ngModel',
         link: function (scope, element, attr, ngModelCtrl) {
             function fromUser(text) {
                 if (text) {
                     var transformedInput = text.replace(/[^0-9]/g, '');

                     if (transformedInput !== text) {
                         ngModelCtrl.$setViewValue(transformedInput);
                         ngModelCtrl.$render();
                     }
                     return transformedInput;
                 }
                 return undefined;
             }
             ngModelCtrl.$parsers.push(fromUser);
         }
     };
 })

.directive('bootstrapSwitch', [
        function () {
            return {
                restrict: 'A',
                require: '?ngModel',
                link: function (scope, element, attrs, ngModel) {
                    element.bootstrapSwitch();

                    element.on('switchChange.bootstrapSwitch', function (event, state) {
                        if (ngModel) {
                            scope.$apply(function () {
                                ngModel.$setViewValue(state);
                            });
                        }
                    });

                    scope.$watch(attrs.ngModel, function (newValue, oldValue) {
                        if (newValue) {
                            element.bootstrapSwitch('state', true, true);
                        } else {
                            element.bootstrapSwitch('state', false, false);
                        }
                    });
                }
            };
        }
]);

app.filter('reverse', function () {
    return function (items) {
        return items.slice().reverse();
    };
});

app.filter('sum', function () {
    return function (data, key) {
        if (typeof (data) === 'undefined' || typeof (key) === 'undefined') {
            return 0;
        }

        var sum = 0;
        for (var i = data.length - 1; i >= 0; i--) {
            sum += parseInt(data[i][key]);
        }

        return sum;
    };
});

app.filter('capital', function () {
    return function (input) {
        return (!!input) ? input.charAt(0).toUpperCase() + input.substr(1).toLowerCase() : '';
    }
});

app.factory('Utils', function ($q) {
    return {
        isImage: function (src) {

            var deferred = $q.defer();

            var image = new Image();
            image.onerror = function () {
                deferred.resolve(false);
            };
            image.onload = function () {
                deferred.resolve(true);
            };
            image.src = src;
            return deferred.promise;
        }
    };
});

app.directive('validNumber', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }

                var clean = val.replace(/[^-0-9\.]/g, '');
                var negativeCheck = clean.split('-');
                var decimalCheck = clean.split('.');
                if (!angular.isUndefined(negativeCheck[1])) {
                    negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                    clean = negativeCheck[0] + '-' + negativeCheck[1];
                    if (negativeCheck[0].length > 0) {
                        clean = negativeCheck[0];
                    }

                }

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 2);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
        }
    };
});

app.config(['$mdDateLocaleProvider', function ($mdDateLocaleProvider) {
    if ($mdDateLocaleProvider != undefined) {
        $mdDateLocaleProvider.formatDate = function (date) {
            return date ? moment(date).format('D/M/YYYY') : '';
        };
        $mdDateLocaleProvider.parseDate = function (dateString) {
            var m = moment(dateString, 'D/M/YYYY', true);
            return m.isValid() ? m.toDate() : new Date(NaN);
        };
    }
}]);

app.run(['$http', function ($http) {
    $http.defaults.headers.common['timezoneoffset'] = new Date().getTimezoneOffset();
}]);