app.controller('LoginController', ['$scope', '$http', '$window', function ($scope, $http, $window) {
    $scope.Login = {
        Initialize: function () {
            $window.localStorage.setItem("Message", "");

            $window.localStorage.setItem("CustpageObject", "");
            $window.localStorage.setItem("CustpageId", "");

            $window.localStorage.setItem("CatpageObject", "");
            $window.localStorage.setItem("CatpageId", "");

            $window.localStorage.setItem("from", "");
            $window.localStorage.setItem("pageparam", "");
            $window.localStorage.setItem("page", "");

            $window.localStorage.setItem("pageObject", "");
            $window.localStorage.setItem("pageId", "");

            $window.localStorage.setItem("Orderid", "");
            $window.localStorage.setItem("OrderpageObject", "");
            $window.localStorage.setItem("OrderpageId", "");

            $window.localStorage.setItem("PatientCatpageObject", "");
            $window.localStorage.setItem("PatientCatpageId", "");

            $window.localStorage.setItem("IndicatorpageObject", "");
            $window.localStorage.setItem("IndicatorpageId", "");

            $window.localStorage.setItem("EpromspageObject", "");
            $window.localStorage.setItem("EpromspageId", "");
        },
        Methods: {
        },

        Services: {
        }
    };

    $scope.Login.Initialize();
}]);