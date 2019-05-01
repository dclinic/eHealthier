app.controller('AddFlagGroupController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddFlagGroup = {
        FlagGroup: [],
        list_FlagGroup: [],
        ID: 0,
        issubmitted: false,
        IsLoading: false,
        IsActive: false,
        IsDisplayOrderExist: false,
        Msg: "",
        CurrentDisplayOrder: 0,
        Initialize: function () {
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddFlagGroup.ID = parseInt(id);
                $scope.AddFlagGroup.Methods.GetEditFlagGroupDataById(id);
            }
        },
        Methods: {
            RedirecttoList: function () {
                $window.location.href = '/FlagGroup/Index';
            },

            Create: function () {
                $scope.AddFlagGroup.issubmitted = true;
                if ($scope.AddFlagGroup.IsDisplayOrderExist == false) {
                    if ($scope.frmFlagGroup.$valid) {
                        $scope.AddFlagGroup.IsLoading = true;
                        $scope.AddFlagGroup.Services.Create({
                            FlagGroupName: $scope.AddFlagGroup.FlagGroupName,
                            Description: $scope.AddFlagGroup.Description,
                            DisplayOrder: $scope.AddFlagGroup.DisplayOrder,
                            IsDelete: $scope.AddFlagGroup.IsDelete,
                            IsActive: $scope.AddFlagGroup.IsActive,
                        });
                        $window.localStorage.setItem("FlagGrouppageObject", "");
                        $window.localStorage.setItem("FlagGrouppageId", "");
                        $window.localStorage.setItem("Message", "Successfully Inserted!");
                    }
                }

            },

            Put: function () {
                $scope.AddFlagGroup.issubmitted = true;
                if ($scope.AddFlagGroup.IsDisplayOrderExist == false) {
                    if ($scope.frmFlagGroup.$valid) {
                        $scope.AddFlagGroup.IsLoading = true;
                        $scope.AddFlagGroup.Services.Put({
                            ID: $scope.AddFlagGroup.ID,
                            FlagGroupName: $scope.AddFlagGroup.FlagGroupName,
                            Description: $scope.AddFlagGroup.Description,
                            DisplayOrder: $scope.AddFlagGroup.DisplayOrder,
                            IsDelete: $scope.AddFlagGroup.IsDelete,
                            IsActive: $scope.AddFlagGroup.IsActive
                        });
                    }
                }
            },

            GetEditFlagGroupDataById: function (id) {
                return $scope.AddFlagGroup.Services.GetEditFlagGroupDataById(id);
            },

            EditFlagGroupData: function (response) {
                var data = JSON.parse(response);
                $scope.AddFlagGroup.CurrentDisplayOrder = data.DisplayOrder;
                $scope.AddFlagGroup.ID = parseInt(data.ID);
                $scope.AddFlagGroup.FlagGroupName = data.FlagGroupName,
                $scope.AddFlagGroup.Description = data.Description,
                $scope.AddFlagGroup.DisplayOrder = data.DisplayOrder,
                $scope.AddFlagGroup.IsDelete = data.IsDelete,
                $scope.AddFlagGroup.IsActive = data.IsActive
            },

            CheckDisplayOrderIsExistOrNot: function (DisplayOrder) {
                DisplayOrder = $scope.AddFlagGroup.DisplayOrder;
                $scope.AddFlagGroup.Services.CheckDisplayOrderIsExistOrNot(DisplayOrder);
            },

        },

        Services: {
            Create: function (data) {
                $http.post('/api/FlagGroup/Post', data).success(function () {
                    $scope.AddFlagGroup.IsLoading = false;
                    $scope.AddFlagGroup.Methods.RedirecttoList();
                });
            },

            GetEditFlagGroupDataById: function (id) {
                $http.get('/api/FlagGroup/GetById/' + id).success($scope.AddFlagGroup.Methods.EditFlagGroupData)
            },

            Put: function (data) {
                $http.post('/api/FlagGroup/Put', data).success(function () {
                    $scope.AddFlagGroup.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddFlagGroup.Methods.RedirecttoList();
                });
            },

            CheckDisplayOrderIsExistOrNot: function (DisplayOrder) {
                $http.get('/api/FlagGroup/CheckDisplayOrderIsExistOrNot?DisplayOrder=' + DisplayOrder).success(function (response) {
                    if (response != "0") {
                        if (DisplayOrder == $scope.AddFlagGroup.CurrentDisplayOrder) {
                            $scope.AddFlagGroup.IsDisplayOrderExist = false;
                            $scope.AddFlagGroup.Msg = "";
                        }
                        else {
                            $scope.AddFlagGroup.IsDisplayOrderExist = true;
                            $scope.AddFlagGroup.Msg = "Display order is already exist.";
                        }
                    }
                    else {
                        $scope.AddFlagGroup.IsDisplayOrderExist = false;
                        $scope.AddFlagGroup.Msg = "";
                    }
                }).error(function (data, status, headers, config) {
                    console.log(data);
                });
            },

        },
    };

    $scope.AddFlagGroup.Initialize();

}]);