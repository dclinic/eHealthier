app.controller('UserMasterController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.UserMaster = {
        MenuList: [],
        IsLoading: false,
        Items: [],

        Initialize: function () {
            $scope.UserMaster.Methods.GetMenu();
        },
        Methods: {
            GetItemsByCategoryId: function (categoryid) {
                $window.location.href = '/Items/Index?categoryid=' + categoryid;
            },
            GetItemsByRestaurantItemId: function (restaurantitemid) {
                $window.location.href = '/Items/Index?itemid=' + restaurantitemid;
            },
            GetMenu: function () {
                return $scope.UserMaster.Services.GetMenu();
            },
            SetMenu: function (response) {
                var data = JSON.parse(response);
                $scope.UserMaster.MenuList = data;
            },
        },

        Services: {
            GetMenu: function () {
                $http.get('/api/User/GetMenu').success($scope.UserMaster.Methods.SetMenu)
            },
        }
    };

    $scope.UserMaster.Initialize();
}]);