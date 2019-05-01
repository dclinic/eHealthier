app.controller('AddPatientCategoryController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddPatientCategory = {
        Category: [],
        list_category: [],
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: true,

        Initialize: function () {
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddPatientCategory.ID = parseInt(id);
                $scope.AddPatientCategory.Methods.GetEditCategoryDataById(id);
            }
        },
        Methods: {
            RedirecttoList: function () {
                $window.location.href = '/PatientCategory/Index';
            },
            GetEditCategoryDataById: function (id) {
                return $scope.AddPatientCategory.Services.GetEditCategoryDataById(id);
            },
            Create: function () {
                $scope.AddPatientCategory.issubmitted = true;
                if ($scope.frmCategory.$valid) {
                    $scope.AddPatientCategory.IsLoading = true;
                    $scope.AddPatientCategory.Services.Create({
                        PatientCategoryName: $scope.AddPatientCategory.PatientCategoryName,
                        Description: $scope.AddPatientCategory.Description,
                        IsActive: $scope.AddPatientCategory.IsActive
                    });
                    $window.localStorage.setItem("PatientCatpageObject", "");
                    $window.localStorage.setItem("PatientCatpageId", "");
                    $window.localStorage.setItem("Message", "Successfully Inserted!");
                }
            },
            EditCategoryData: function (response) {
                var data = JSON.parse(response);
                $scope.AddPatientCategory.ID = parseInt(data.ID);
                $scope.AddPatientCategory.PatientCategoryName = data.PatientCategoryName;
                $scope.AddPatientCategory.Description = data.Description;
                $scope.AddPatientCategory.IsActive = data.IsActive;
            },

            Put: function () {
                $scope.AddPatientCategory.issubmitted = true;
                if ($scope.frmCategory.$valid) {
                    $scope.AddPatientCategory.IsLoading = true;
                    $scope.AddPatientCategory.Services.Put({
                        ID: $scope.AddPatientCategory.ID,
                        PatientCategoryName: $scope.AddPatientCategory.PatientCategoryName,
                        Description: $scope.AddPatientCategory.Description,
                        IsActive: $scope.AddPatientCategory.IsActive
                    });
                }
            },
        },

        Services: {
            Create: function (data) {                
                $http.post('/api/PatientCategory/Post', data).success(function () {
                    $scope.AddPatientCategory.IsLoading = false;
                    $scope.AddPatientCategory.Methods.RedirecttoList();
                });
            },
            GetEditCategoryDataById: function (id) {
                $http.get('/api/PatientCategory/GetById/' + id).success($scope.AddPatientCategory.Methods.EditCategoryData)
            },
            Put: function (data) {
                $http.post('/api/PatientCategory/Update', data).success(function () {
                    $scope.AddPatientCategory.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddPatientCategory.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.AddPatientCategory.Initialize();
}]);