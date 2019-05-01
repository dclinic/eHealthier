app.controller('AddUTController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddUT = {
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: true,

        Initialize: function () {
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddUT.ID = parseInt(id);
                $scope.AddUT.Methods.GetEditUTDataById(id);
            }
        },
        Methods: {
            RedirecttoList: function () {
                $window.location.href = '/UserType/Index';
            },
            GetEditUTDataById: function (id) {
                return $scope.AddUT.Services.GetEditUTDataById(id);
            },
            Create: function () {                
                $scope.AddUT.issubmitted = true;
                if ($scope.frmAddUT.$valid) {
                    $scope.AddUT.IsLoading = true;
                    $scope.AddUT.Services.Create({
                        UserType1: $scope.AddUT.UserType1,
                        IsActive: $scope.AddUT.IsActive
                    });
                    $window.localStorage.setItem("UTpageObject", "");
                    $window.localStorage.setItem("UTpageId", "");
                    $window.localStorage.setItem("Message", "Successfully Inserted!");
                }
            },
            EditUTData: function (response) {
                var data = JSON.parse(response);
                $scope.AddUT.ID = parseInt(data.ID);
                $scope.AddUT.UserType1 = data.UserType1;
                $scope.AddUT.IsActive = data.IsActive;
            },

            Put: function () {
                $scope.AddUT.issubmitted = true;
                if ($scope.frmAddUT.$valid) {
                    $scope.AddUT.IsLoading = true;
                    $scope.AddUT.Services.Put({
                        ID: $scope.AddUT.ID,
                        UserType1: $scope.AddUT.UserType1,
                        IsActive: $scope.AddUT.IsActive
                    });
                }
            },           
        },

        Services: {          
            Create: function (data) {
                $http.post('/api/UserType/Post', data).success(function () {
                    $scope.AddUT.IsLoading = false;
                    $scope.AddUT.Methods.RedirecttoList();
                });
            },
            GetEditUTDataById: function (id) {
                $http.get('/api/UserType/GetById/' + id).success($scope.AddUT.Methods.EditUTData)
            },
            Put: function (data) {
                $http.post('/api/UserType/Update', data).success(function () {
                    $scope.AddUT.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddUT.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.AddUT.Initialize();
}]);