var app = angular.module('app', ['ngRoute', 'angularFileUpload', 'ngAnimate', 'ui.bootstrap', 'toaster', 'ngBootbox', 'ngMessages', 'ngCookies', 'ngMessages'])

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

//angular.isUndefinedOrNull = function (val) {
//    return angular.isUndefined(val) || val === null
//}