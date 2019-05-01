app.controller('surveyMonkeyController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.SurverMonkey = {
        Item: {},
        Items: [],
        Methods: {
            Initialize: function () {
                $scope.Provider.Services.setData();
            },
            setSurverMonkeySettings: function (data) {
                if (data != null) {
                    $scope.SurverMonkey.Item = data;
                }
            },
        },
        Services: {
            setData: function (data) {
                $http.get("/api/SystemFlag/GetEntityById_FlagGroupId?" + data).success($scope.Provider.Methods.setSurverMonkeySettings);
            },
            Update: function (data) {
                
            }
        },
        UI: {
        },
    }

    $scope.Provider.Methods.Initialize();

}]);