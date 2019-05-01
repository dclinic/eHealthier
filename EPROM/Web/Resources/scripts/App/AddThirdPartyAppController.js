app.controller('AddThirdPartyAppController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddThirdPartyApp = {
        EMAIL_REGEXP: /^[a-z0-9!#$%&'*+/=?^_`{|}~.-]+@[a-z0-9-]+(\.[a-z0-9-]+)*$/i,
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: true,
        list_SurvayCategory: [],
        list_SubCategory: [],

        Initialize: function () {
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddThirdPartyApp.ID = parseInt(id);
                $scope.AddThirdPartyApp.Methods.GetEditAppDataById(id);
            }
            $scope.AddThirdPartyApp.Methods.GetSurveyCategory();
        },
        Methods: {
            RedirecttoList: function () {
                $window.location.href = '/ThirdPartyApp/Index';
            },
            GetEditAppDataById: function (id) {
                return $scope.AddThirdPartyApp.Services.GetEditAppDataById(id);
            },
            Create: function () {
                $scope.AddThirdPartyApp.issubmitted = true;
                if ($scope.formThirdPartyApp.$valid) {
                    $scope.AddThirdPartyApp.IsLoading = true;
                    $scope.AddThirdPartyApp.Services.Create({
                        SurveyCategoryID: $scope.AddThirdPartyApp.SurveyCategoryID,
                        SurveySubCategoryID: $scope.AddThirdPartyApp.SurveySubCategoryID,
                        AppName: $scope.AddThirdPartyApp.AppName,
                        URL: $scope.AddThirdPartyApp.URL,
                        Address: $scope.AddThirdPartyApp.Address,
                        Email: $scope.AddThirdPartyApp.Email,
                        MobileNo: $scope.AddThirdPartyApp.MobileNo,
                        PhoneNo: $scope.AddThirdPartyApp.PhoneNo,
                        IsActive: $scope.AddThirdPartyApp.IsActive
                    });
                    $window.localStorage.setItem("3rdPartyAppPageObject", "");
                    $window.localStorage.setItem("3rdPartyAppPageId", "");
                    $window.localStorage.setItem("Message", "Successfully Inserted!");
                }
            },
            EditAppData: function (response) {
                var data = JSON.parse(response);
                $scope.AddThirdPartyApp.SurveyCategoryID = data.SurveyCategoryID;

                $scope.AddThirdPartyApp.Methods.GetSubCategoryById($scope.AddThirdPartyApp.SurveyCategoryID)

                $scope.AddThirdPartyApp.SurveySubCategoryID = data.SurveySubCategoryID;
                $scope.AddThirdPartyApp.ID = parseInt(data.ID);
                $scope.AddThirdPartyApp.AppName = data.AppName;
                $scope.AddThirdPartyApp.URL = data.URL;
                $scope.AddThirdPartyApp.Address = data.Address;
                $scope.AddThirdPartyApp.Email = data.Email;
                $scope.AddThirdPartyApp.MobileNo = data.MobileNo;
                $scope.AddThirdPartyApp.PhoneNo = data.PhoneNo;
                $scope.AddThirdPartyApp.IsActive = data.IsActive;
            },

            Put: function () {
                $scope.AddThirdPartyApp.issubmitted = true;
                if ($scope.formThirdPartyApp.$valid) {
                    $scope.AddThirdPartyApp.IsLoading = true;
                    $scope.AddThirdPartyApp.Services.Put({
                        ID: $scope.AddThirdPartyApp.ID,
                        AppName: $scope.AddThirdPartyApp.AppName,
                        URL: $scope.AddThirdPartyApp.URL,
                        SurveyCategoryID: $scope.AddThirdPartyApp.SurveyCategoryID,
                        SurveySubCategoryID: $scope.AddThirdPartyApp.SurveySubCategoryID,
                        Address: $scope.AddThirdPartyApp.Address,
                        Email: $scope.AddThirdPartyApp.Email,
                        MobileNo: $scope.AddThirdPartyApp.MobileNo,
                        PhoneNo: $scope.AddThirdPartyApp.PhoneNo,
                        IsActive: $scope.AddThirdPartyApp.IsActive
                    });
                }
            },
            GetSurveyCategory: function () {
                $scope.AddThirdPartyApp.Services.GetSurveyCategory();
            },
            SetSurveyCategory: function (data) {
                $scope.AddThirdPartyApp.list_SurvayCategory = JSON.parse(data);
            },
            GetSubCategoryById: function (Id) {
                if (Id == undefined) {
                    $scope.AddThirdPartyApp.list_SubCategory = [];
                }
                else
                    $scope.AddThirdPartyApp.Services.GetSubCategoryById(Id);
            },
            SetSubCategoryData: function (data) {
                $scope.AddThirdPartyApp.list_SubCategory = JSON.parse(data);
            },
        },

        Services: {
            Create: function (data) {
                $http.post('/api/ThirdPartyApp/Post', data).success(function () {
                    $scope.AddThirdPartyApp.IsLoading = false;
                    $scope.AddThirdPartyApp.Methods.RedirecttoList();
                });
            },
            GetEditAppDataById: function (id) {
                $http.get('/api/ThirdPartyApp/GetById/' + id).success($scope.AddThirdPartyApp.Methods.EditAppData)
            },
            Put: function (data) {
                $http.post('/api/ThirdPartyApp/Update', data).success(function () {
                    $scope.AddThirdPartyApp.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddThirdPartyApp.Methods.RedirecttoList();
                });
            },
            GetSurveyCategory: function () {
                $http.get('/api/category/Get').success($scope.AddThirdPartyApp.Methods.SetSurveyCategory)
            },
            GetSubCategoryById: function (id) {
                $http.get('/api/category/GetSubCategoryById/' + id).success($scope.AddThirdPartyApp.Methods.SetSubCategoryData)
            },
        }
    };

    $scope.AddThirdPartyApp.Initialize();
}]);