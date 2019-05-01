app.controller('AddCategoryController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddCategory = {
        Category: [],
        list_category: [],
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: true,

        Initialize: function () {
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddCategory.ID = parseInt(id);
                $scope.AddCategory.Methods.GetCategory($scope.AddCategory.ID);
                $scope.AddCategory.Methods.GetEditCategoryDataById(id);
            }
            else
                $scope.AddCategory.Methods.GetCategory();
        },
        Methods: {
            RedirecttoList: function () {
                $window.location.href = '/Category/Index';
            },
            GetEditCategoryDataById: function (id) {
                return $scope.AddCategory.Services.GetEditCategoryDataById(id);
            },
            Create: function () {
                $scope.AddCategory.issubmitted = true;
                if ($scope.frmCategory.$valid) {
                    $scope.AddCategory.IsLoading = true;
                    $scope.AddCategory.Services.Create({
                        SurvayCategoryName: $scope.AddCategory.SurvayCategoryName,
                        Description: $scope.AddCategory.Description,
                        ParentSurveyCategoryID: $scope.AddCategory.ParentSurveyCategoryID,
                        IsActive: $scope.AddCategory.IsActive
                    });
                    $window.localStorage.setItem("CatpageObject", "");
                    $window.localStorage.setItem("CatpageId", "");
                    $window.localStorage.setItem("Message", "Successfully Inserted!");
                }
            },
            EditCategoryData: function (response) {               
                var data = JSON.parse(response);
                $scope.AddCategory.ID = parseInt(data.ID);
                $scope.AddCategory.SurvayCategoryName = data.SurvayCategoryName;
                $scope.AddCategory.Description = data.Description;
                $scope.AddCategory.ParentSurveyCategoryID = data.ParentSurveyCategoryID != null ? parseInt(data.ParentSurveyCategoryID) : null;
                $scope.AddCategory.IsActive = data.IsActive;
            },

            Put: function () {
                $scope.AddCategory.issubmitted = true;
                if ($scope.frmCategory.$valid) {
                    $scope.AddCategory.IsLoading = true;
                    $scope.AddCategory.Services.Put({
                        ID: $scope.AddCategory.ID,
                        SurvayCategoryName: $scope.AddCategory.SurvayCategoryName,
                        Description: $scope.AddCategory.Description,
                        ParentSurveyCategoryID: $scope.AddCategory.ParentSurveyCategoryID,
                        IsActive: $scope.AddCategory.IsActive
                    });
                }
            },
            GetCategory: function (Id) {
                return $scope.AddCategory.Services.Get(Id);
            },
            SetCategory: function (data) {
                return $scope.AddCategory.list_category = JSON.parse(data);
            },
        },

        Services: {
            Get: function (id) {
                $http.get('/api/category/GetList/' + id).success($scope.AddCategory.Methods.SetCategory)
            },
            Create: function (data) {
                $http.post('/api/category/Post', data).success(function () {
                    $scope.AddCategory.IsLoading = false;
                    $scope.AddCategory.Methods.RedirecttoList();
                });
            },
            GetEditCategoryDataById: function (id) {
                $http.get('/api/category/GetById/' + id).success($scope.AddCategory.Methods.EditCategoryData)
            },
            Put: function (data) {
                $http.post('/api/category/Put', data).success(function () {
                    $scope.AddCategory.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddCategory.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.AddCategory.Initialize();
}]);