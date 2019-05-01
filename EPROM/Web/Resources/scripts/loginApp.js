var loginApp = angular.module('loginApp', ['ngMaterial', 'ngRoute', 'ngAnimate', 'toaster'])

loginApp.constant("appSetting", {
    apiBaseUrl: "http://localhost:4777/"
});

loginApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('AuthInterceptor');
}]);

//loginApp.config(['$qProvider', function ($qProvider) {
//    $qProvider.errorOnUnhandledRejections(false);
//}]);

loginApp.run(['$http', function ($http) {
    $http.defaults.headers.common['timezoneoffset'] = new Date().getTimezoneOffset();
}]);