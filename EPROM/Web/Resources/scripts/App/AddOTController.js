app.controller('AddOTController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddOT = {
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: true,

        Initialize: function () {
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddOT.ID = parseInt(id);
                $scope.AddOT.Methods.GetEditOTDataById(id);
            }
        },
        Methods: {
            RedirecttoList: function () {
                $window.location.href = '/OrganizationType/Index';
            },
            GetEditOTDataById: function (id) {
                return $scope.AddOT.Services.GetEditOTDataById(id);
            },
            Create: function () {                
                $scope.AddOT.issubmitted = true;
                if ($scope.frmAddOT.$valid) {
                    $scope.AddOT.IsLoading = true;
                    $scope.AddOT.Services.Create({
                        OrganizationType1: $scope.AddOT.OrganizationType1,
                        IsActive: $scope.AddOT.IsActive
                    });
                    $window.localStorage.setItem("OTpageObject", "");
                    $window.localStorage.setItem("OTpageId", "");
                    $window.localStorage.setItem("Message", "Successfully Inserted!");
                }
            },
            EditOTData: function (response) {
                var data = JSON.parse(response);
                $scope.AddOT.ID = parseInt(data.ID);
                $scope.AddOT.OrganizationType1 = data.OrganizationType1;
                $scope.AddOT.IsActive = data.IsActive;
            },

            Put: function () {
                $scope.AddOT.issubmitted = true;
                if ($scope.frmAddOT.$valid) {
                    $scope.AddOT.IsLoading = true;
                    $scope.AddOT.Services.Put({
                        ID: $scope.AddOT.ID,
                        OrganizationType1: $scope.AddOT.OrganizationType1,
                        IsActive: $scope.AddOT.IsActive
                    });
                }
            },           
        },

        Services: {          
            Create: function (data) {
                $http.post('/api/OrganizationType/Post', data).success(function () {
                    $scope.AddOT.IsLoading = false;
                    $scope.AddOT.Methods.RedirecttoList();
                });
            },
            GetEditOTDataById: function (id) {
                $http.get('/api/OrganizationType/GetById/' + id).success($scope.AddOT.Methods.EditOTData)
            },
            Put: function (data) {
                $http.post('/api/OrganizationType/Update', data).success(function () {
                    $scope.AddOT.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddOT.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.AddOT.Initialize();
}]);