var loginApp = angular.module('loginApp', ['ngMaterial', 'ngRoute', 'ngAnimate', 'toaster'])

loginApp.run(['$http', function ($http) {
    $http.defaults.headers.common['timezoneoffset'] = new Date().getTimezoneOffset();
}]);