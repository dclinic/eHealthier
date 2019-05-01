app.controller('PatientMasterController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$route', '$cookies', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $route, $cookies) {

    $scope.PatientMaster = {
        isExistCINT: "NO",
        isLoadCINTLayout: false,
        errormessage: "",

        Initialize: function () {
            if ($cookies.get("isExistCINT") != undefined && $cookies.get("isExistCINT") != "") {
                $scope.PatientMaster.isExistCINT = $cookies.get("isExistCINT");
            }

            if ($scope.PatientMaster.isExistCINT == "YES") {
                $scope.PatientMaster.isLoadCINTLayout = true;
            }
        },

        DisableDate: function () {
            $http.get('/api/patient/CheckInvalidDate?PatientSurveyId=' + $window.localStorage.getItem("PSID")).success(function (response) {
                $scope.PatientMaster.errormessage = response;
            });
        }
    };

    $scope.PatientMaster.Initialize();
}]);
