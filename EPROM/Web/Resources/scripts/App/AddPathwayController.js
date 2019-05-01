app.controller('AddPathwayController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddPathway = {
        Category: [],
        list_category: [],
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: true,

        Initialize: function () {
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddPathway.ID = id;
                $scope.AddPathway.Methods.GetEditCategoryDataById(id);
            }

            var d = new Date();
            d.setFullYear(d.getFullYear() - 1);
            $scope.AddPathway.FromDate = d;
            $scope.AddPathway.ToDate = new Date();
        },
        Methods: {
            RedirecttoList: function () {
                $window.location.href = '/Pathway/Index';
            },
            GetEditCategoryDataById: function (id) {
                return $scope.AddPathway.Services.GetEditCategoryDataById(id);
            },
            Create: function () {
                $scope.AddPathway.issubmitted = true;
                if ($scope.frmCategory.$valid) {
                    $scope.AddPathway.IsLoading = true;
                    $scope.AddPathway.Services.Create({
                        PathwayName: $scope.AddPathway.PathwayName,
                        Description: $scope.AddPathway.Description,
                        IsActive: $scope.AddPathway.IsActive
                    });
                    $window.localStorage.setItem("PathwaypageObject", "");
                    $window.localStorage.setItem("PathwayId", "");
                    $window.localStorage.setItem("Message", "Successfully Inserted!");
                }
            },
            EditCategoryData: function (response) {
                var data = JSON.parse(response);
                $scope.AddPathway.ID = data.ID;
                $scope.AddPathway.PathwayName = data.PathwayName;
                $scope.AddPathway.Description = data.Description;
                $scope.AddPathway.IsActive = data.IsActive;
            },

            Put: function () {
                $scope.AddPathway.issubmitted = true;
                if ($scope.frmCategory.$valid) {
                    $scope.AddPathway.IsLoading = true;
                    $scope.AddPathway.Services.Put({
                        ID: $scope.AddPathway.ID,
                        PathwayName: $scope.AddPathway.PathwayName,
                        Description: $scope.AddPathway.Description,
                        IsActive: $scope.AddPathway.IsActive
                    });
                }
            },
        },

        Services: {
            Create: function (data) {                
                $http.post('/api/Pathway/Post', data).success(function () {
                    $scope.AddPathway.IsLoading = false;
                    $scope.AddPathway.Methods.RedirecttoList();
                });
            },
            GetEditCategoryDataById: function (id) {
                $http.get('/api/Pathway/GetById/' + id).success($scope.AddPathway.Methods.EditCategoryData)
            },
            Put: function (data) {
                $http.post('/api/Pathway/Update', data).success(function () {
                    $scope.AddPathway.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddPathway.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.AddPathway.Initialize();
}]);