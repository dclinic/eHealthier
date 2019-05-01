app.controller('AddSystemFlagController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddSystemFlag = {
        SystemFlag: [],
        list_FlagGroup: [],
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: false,
        Msg:""  ,
        Initialize: function () {
            $scope.AddSystemFlag.Methods.GetFlagGroup();
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddSystemFlag.ID = parseInt(id);
                $scope.AddSystemFlag.Methods.GetEditSystemFlagDataById(id);
            }
        },

        Methods: {

            RedirecttoList: function () {
                $window.location.href = '/SystemFlag/Index';
            },

            Create: function () {
                $scope.AddSystemFlag.issubmitted = true;
                if ($scope.frmSystemFlag.$valid) {
                    $scope.AddSystemFlag.IsLoading = true;
                    $scope.AddSystemFlag.Services.Create({
                        SystemFlagName: $scope.AddSystemFlag.SystemFlagName,
                        FlagGroupID: $scope.AddSystemFlag.FlagGroupID,
                        DefaultValue: $scope.AddSystemFlag.DefaultValue,
                        Value: $scope.AddSystemFlag.Value,
                        DisplayOrder: $scope.AddSystemFlag.DisplayOrder,
                        IsActive: $scope.AddSystemFlag.IsActive,
                    });
                    $window.localStorage.setItem("SystemFlagpageObject", "");
                    $window.localStorage.setItem("SystemFlagpageId", "");
                    $window.localStorage.setItem("Message", "Successfully Inserted!");
                }
            },

            Put: function () {
                $scope.AddSystemFlag.issubmitted = true;
                if ($scope.frmSystemFlag.$valid) {
                    $scope.AddSystemFlag.IsLoading = true;
                    $scope.AddSystemFlag.Services.Put({
                        ID: $scope.AddSystemFlag.ID,
                        SystemFlagName: $scope.AddSystemFlag.SystemFlagName,
                        FlagGroupID: $scope.AddSystemFlag.FlagGroupID,
                        DefaultValue: $scope.AddSystemFlag.DefaultValue,
                        Value: $scope.AddSystemFlag.Value,
                        DisplayOrder: $scope.AddSystemFlag.DisplayOrder,
                        IsActive: $scope.AddSystemFlag.IsActive
                    });
                }
            },

            CheckDisplayOrderIsExistOrNot: function (DisplayOrder)
            {
                DisplayOrder = $scope.AddSystemFlag.DisplayOrder;
                $scope.AddSystemFlag.Services.CheckDisplayOrderIsExistOrNot(DisplayOrder);
            },

            EditSystemFlagData: function (response) {
                var data = JSON.parse(response);
                $scope.AddSystemFlag.ID = parseInt(data.ID);
                $scope.AddSystemFlag.SystemFlagName = data.SystemFlagName,
                $scope.AddSystemFlag.FlagGroupID = data.FlagGroupID,
                $scope.AddSystemFlag.DefaultValue = data.DefaultValue,
                $scope.AddSystemFlag.Value = data.Value,
                $scope.AddSystemFlag.DisplayOrder = data.DisplayOrder,
                $scope.AddSystemFlag.IsActive = data.IsActive
            },

            GetEditSystemFlagDataById: function (id) {
                return $scope.AddSystemFlag.Services.GetEditSystemFlagDataById(id);
            },

            GetFlagGroup: function () {
                $scope.AddSystemFlag.Services.GetFlagGroup();
            },

            SetFlagGroup: function (data) {
                if (data != null) {
                    var jsondata = JSON.parse(data);
                    $scope.AddSystemFlag.list_FlagGroup = jsondata;
                }
            },
        },

        Services: {
            Create: function (data) {
                $http.post('/api/SystemFlag/Post', data).success(function () {
                    $scope.AddSystemFlag.IsLoading = false;
                    $scope.AddSystemFlag.Methods.RedirecttoList();
                });
            },
            Put: function (data) {
                $http.post('/api/SystemFlag/Put', data).success(function () {
                    $scope.AddSystemFlag.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddSystemFlag.Methods.RedirecttoList();
                });
            },
            GetEditSystemFlagDataById: function (id) {
                $http.get('/api/SystemFlag/GetById/' + id).success($scope.AddSystemFlag.Methods.EditSystemFlagData)
            },

            GetFlagGroup: function () {
                $http.get('/api/FlagGroup/Get').success($scope.AddSystemFlag.Methods.SetFlagGroup)
            },
            CheckDisplayOrderIsExistOrNot: function (DisplayOrder)
            {
                $http.get('/api/SystemFlag/CheckDisplayOrderIsExistOrNot?DisplayOrder=' + DisplayOrder).success(function (response) {
                    if (response != "0") {
                        $scope.frmSystemFlag.$valid = false;
                        $scope.AddSystemFlag.Msg = "Display order is already exist.";
                    }
                    else {
                        $scope.frmSystemFlag.$valid = true;
                        $scope.AddSystemFlag.Msg = "";
                    }
                }).error(function (data, status, headers, config) {
                    console.log(data);
                });
            },
        }
    };

    $scope.AddSystemFlag.Initialize();

}]);