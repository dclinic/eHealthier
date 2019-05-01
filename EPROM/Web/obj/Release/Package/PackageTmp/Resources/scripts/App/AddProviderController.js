app.controller('AddProviderController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddProvider = {
        Provider: [],
        list_FlagGroup: [],
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: false,
        Msg:""  ,
        Initialize: function () {
            $scope.AddProvider.Methods.GetFlagGroup();
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddProvider.ID = parseInt(id);
                $scope.AddProvider.Methods.GetEditProviderDataById(id);
            }
        },

        Methods: {

            RedirecttoList: function () {
                $window.location.href = '/Provider/Index';
            },

            Create: function () {
                $scope.AddProvider.issubmitted = true;
                if ($scope.frmProvider.$valid) {
                    $scope.AddProvider.IsLoading = true;
                    $scope.AddProvider.Services.Create({
                        ProviderName: $scope.AddProvider.ProviderName,
                        FlagGroupID: $scope.AddProvider.FlagGroupID,
                        DefaultValue: $scope.AddProvider.DefaultValue,
                        Value: $scope.AddProvider.Value,
                        DisplayOrder: $scope.AddProvider.DisplayOrder,
                        IsActive: $scope.AddProvider.IsActive,
                    });
                    $window.localStorage.setItem("ProviderpageObject", "");
                    $window.localStorage.setItem("ProviderpageId", "");
                    $window.localStorage.setItem("Message", "Successfully Inserted!");
                }
            },

            Put: function () {
                $scope.AddProvider.issubmitted = true;
                if ($scope.frmProvider.$valid) {
                    $scope.AddProvider.IsLoading = true;
                    $scope.AddProvider.Services.Put({
                        ID: $scope.AddProvider.ID,
                        ProviderName: $scope.AddProvider.ProviderName,
                        FlagGroupID: $scope.AddProvider.FlagGroupID,
                        DefaultValue: $scope.AddProvider.DefaultValue,
                        Value: $scope.AddProvider.Value,
                        DisplayOrder: $scope.AddProvider.DisplayOrder,
                        IsActive: $scope.AddProvider.IsActive
                    });
                }
            },

            CheckDisplayOrderIsExistOrNot: function (DisplayOrder)
            {
                DisplayOrder = $scope.AddProvider.DisplayOrder;
                $scope.AddProvider.Services.CheckDisplayOrderIsExistOrNot(DisplayOrder);
            },

            EditProviderData: function (response) {
                var data = JSON.parse(response);
                $scope.AddProvider.ID = parseInt(data.ID);
                $scope.AddProvider.ProviderName = data.ProviderName,
                $scope.AddProvider.FlagGroupID = data.FlagGroupID,
                $scope.AddProvider.DefaultValue = data.DefaultValue,
                $scope.AddProvider.Value = data.Value,
                $scope.AddProvider.DisplayOrder = data.DisplayOrder,
                $scope.AddProvider.IsActive = data.IsActive
            },

            GetEditProviderDataById: function (id) {
                return $scope.AddProvider.Services.GetEditProviderDataById(id);
            },

            GetFlagGroup: function () {
                $scope.AddProvider.Services.GetFlagGroup();
            },

            SetFlagGroup: function (data) {
                if (data != null) {
                    var jsondata = JSON.parse(data);
                    $scope.AddProvider.list_FlagGroup = jsondata;
                }
            },
        },

        Services: {
            Create: function (data) {
                $http.post('/api/Provider/Post', data).success(function () {
                    $scope.AddProvider.IsLoading = false;
                    $scope.AddProvider.Methods.RedirecttoList();
                });
            },
            Put: function (data) {
                $http.post('/api/Provider/Put', data).success(function () {
                    $scope.AddProvider.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddProvider.Methods.RedirecttoList();
                });
            },
            GetEditProviderDataById: function (id) {
                $http.get('/api/Provider/GetById/' + id).success($scope.AddProvider.Methods.EditProviderData)
            },

            GetFlagGroup: function () {
                $http.get('/api/FlagGroup/Get').success($scope.AddProvider.Methods.SetFlagGroup)
            },
            CheckDisplayOrderIsExistOrNot: function (DisplayOrder)
            {
                $http.get('/api/Provider/CheckDisplayOrderIsExistOrNot?DisplayOrder=' + DisplayOrder).success(function (response) {
                    if (response != "0") {
                        $scope.frmProvider.$valid = false;
                        $scope.AddProvider.Msg = "Display order is already exist.";
                    }
                    else {
                        $scope.frmProvider.$valid = true;
                        $scope.AddProvider.Msg = "";
                    }
                }).error(function (data, status, headers, config) {
                    console.log(data);
                });
            },
        }
    };

    $scope.AddProvider.Initialize();

}]);