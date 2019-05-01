app.controller('ClearController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$route', '$cookies', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $route, $cookies) {

    $scope.Clear = {
        Initialize: function () {            
            $window.localStorage.setItem("OrganizationId", "");
            $window.localStorage.setItem("PracticeId", "");
            //$window.localStorage.setItem("PatientSurveyID", "");
            //$window.localStorage.setItem("UserpatientID", "");
            //$window.localStorage.setItem("SurveyID", "");
            //$window.localStorage.setItem("ProviderID", "");
            //$window.localStorage.setItem("PatientId", "");
            //$window.localStorage.setItem("surveyList", "");
            //$window.localStorage.setItem("patientDetails", "");
            //$window.localStorage.setItem("ProviderTypeID", "");
            //$window.localStorage.setItem("username", "");            
            //$window.localStorage.setItem("UserId", "");
            //$window.localStorage.setItem("RoleName", "");
            //$window.localStorage.setItem("PracticeUserId", "");

            // -------- Admin---------------
            $window.localStorage.setItem("CatpageObject", "");
            $window.localStorage.setItem("CatpageId", "");
            $window.localStorage.setItem("IndicatorpageObject", "");
            $window.localStorage.setItem("IndicatorpageId", "");
            $window.localStorage.setItem("PatientCatpageObject", "");
            $window.localStorage.setItem("PatientCatpageId", "");
            $window.localStorage.setItem("ProviderpageObject", "");
            $window.localStorage.setItem("ProviderpageId", "");
            $window.localStorage.setItem("SystemFlagpageObject", "");
            $window.localStorage.setItem("SystemFlagpageId", "");
            $window.localStorage.setItem("3rdPartyAppPageObject", "");
            $window.localStorage.setItem("3rdPartyAppPageId", "");
            $window.localStorage.setItem("CustpageObject", "");
            $window.localStorage.setItem("CustpageId", "");
            $window.localStorage.setItem("EpromspageObject", "");
            $window.localStorage.setItem("EpromspageId", "");
            $window.localStorage.setItem("FlagGrouppageObject", "");
            $window.localStorage.setItem("FlagGrouppageId", "");
            $window.localStorage.setItem("IndicatorpageObject", "");
            $window.localStorage.setItem("IndicatorpageId", "");
        },
        Methods: {

        },
        Services: {
        }
    };

    $scope.Clear.Initialize();
}]);
