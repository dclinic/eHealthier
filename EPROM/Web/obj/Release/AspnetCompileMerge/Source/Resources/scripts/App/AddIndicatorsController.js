app.controller('AddIndicatorsController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddIndicators = {
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: true,

        Initialize: function () {
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddIndicators.ID = parseInt(id);
                $scope.AddIndicators.Methods.GetEditIndicatorDataById(id);
            }
        },
        Methods: {
            RedirecttoList: function () {
                $window.location.href = '/Indicators/Index';
            },
            GetEditIndicatorDataById: function (id) {
                return $scope.AddIndicators.Services.GetEditIndicatorDataById(id);
            },
            Create: function () {                
                $scope.AddIndicators.issubmitted = true;
                if ($scope.frmAddIndicators.$valid) {
                    $scope.AddIndicators.IsLoading = true;
                    $scope.AddIndicators.Services.Create({
                        IndicatorName: $scope.AddIndicators.IndicatorName,
                        Description: $scope.AddIndicators.Description,
                        IsActive: $scope.AddIndicators.IsActive
                    });
                    $window.localStorage.setItem("IndicatorpageObject", "");
                    $window.localStorage.setItem("IndicatorpageId", "");
                    $window.localStorage.setItem("Message", "Successfully Inserted!");
                }
            },
            EditIndicatorData: function (response) {
                var data = JSON.parse(response);
                $scope.AddIndicators.ID = parseInt(data.ID);
                $scope.AddIndicators.IndicatorName = data.IndicatorName;
                $scope.AddIndicators.Description = data.Description;
                $scope.AddIndicators.IsActive = data.IsActive;
            },

            Put: function () {
                $scope.AddIndicators.issubmitted = true;
                if ($scope.frmAddIndicators.$valid) {
                    $scope.AddIndicators.IsLoading = true;
                    $scope.AddIndicators.Services.Put({
                        ID: $scope.AddIndicators.ID,
                        IndicatorName: $scope.AddIndicators.IndicatorName,
                        Description: $scope.AddIndicators.Description,
                        IsActive: $scope.AddIndicators.IsActive
                    });
                }
            },           
        },

        Services: {          
            Create: function (data) {
                $http.post('/api/Indicators/Post', data).success(function () {
                    $scope.AddIndicators.IsLoading = false;
                    $scope.AddIndicators.Methods.RedirecttoList();
                });
            },
            GetEditIndicatorDataById: function (id) {
                $http.get('/api/Indicators/GetById/' + id).success($scope.AddIndicators.Methods.EditIndicatorData)
            },
            Put: function (data) {
                $http.post('/api/Indicators/Update', data).success(function () {
                    $scope.AddIndicators.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddIndicators.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.AddIndicators.Initialize();
}]);