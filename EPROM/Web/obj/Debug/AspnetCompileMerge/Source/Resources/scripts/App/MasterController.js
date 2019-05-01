app.controller('MasterController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.Master = {

        Initialize: function () {
        },
        Methods: {
            ClearStorage: function (location) {
                $window.location.href = location;
                $window.localStorage.setItem("from", "");
                $window.localStorage.setItem("restaurantid", "");

                $window.localStorage.setItem("Message", "");
                $window.localStorage.setItem("CustpageObject", "");
                $window.localStorage.setItem("CustpageId", "");

                $window.localStorage.setItem("CatpageObject", "");
                $window.localStorage.setItem("CatpageId", "");

                $window.localStorage.setItem("pageparam", "");
                $window.localStorage.setItem("page", "");

                $window.localStorage.setItem("pageObject", "");
                $window.localStorage.setItem("pageId", "");

                $window.localStorage.setItem("Orderid", "");
                $window.localStorage.setItem("OrderpageObject", "");
                $window.localStorage.setItem("OrderpageId", "");

                $window.localStorage.setItem("SystemFlagpageObject", "");
                $window.localStorage.setItem("SystemFlagpageId", "");

                $window.localStorage.setItem("FlagGrouppageObject", "");
                $window.localStorage.setItem("FlagGrouppageId", "");

                $window.localStorage.setItem("PatientCatpageObject", "");
                $window.localStorage.setItem("PatientCatpageId", "");

                $window.localStorage.setItem("PathwaypageObject", "");
                $window.localStorage.setItem("PathwayId", "");

                $window.localStorage.setItem("3rdPartyAppPageObject", "");
                $window.localStorage.setItem("3rdPartyAppPageId", "");

                $window.localStorage.setItem("IndicatorpageObject", "");
                $window.localStorage.setItem("IndicatorpageId", "");

                $window.localStorage.setItem("EpromspageObject", "");
                $window.localStorage.setItem("EpromspageId", "");
            },
        },

        Services: {
        }
    };

    $scope.Master.Initialize();
}]);