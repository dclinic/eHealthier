app.controller('OrganizationController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$cookies', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $cookies) {
    $scope.Organization = {
        Item: { User: { Gender: 'Male' }, Role: 'organization' },
        Items: [],
        OrganizationList: [],
        issubmitted: false,
        IsLoading: false,

        Methods: {

            Initialize: function () {
                if ($cookies.get("username") != undefined) {
                    $scope.Organization.Item.ToAddress = $cookies.get("username");
                }

                $scope.Organization.Services.GetRole();
                $scope.Organization.Services.States();
                $scope.Organization.Services.Countries();
                $scope.Organization.Methods.GetSalutationList();
                $scope.Organization.Item.User.UserName = $scope.Organization.Item.ToAddress;
                $scope.Organization.Services.GetOrganizationDetails($scope.Organization.Item.ToAddress);

                $scope.Organization.IsStartLoading = false;
            },

            SetRole: function (data) {
                $scope.Organization.Items.RoleList = data;
            },
            UpdateOrganizationDetails: function (data) {
                $scope.Organization.issubmitted = true;
                $scope.Organization.Item.User.UserName = $cookies.get("username");
                if ($scope.formOrganizationDetail.$valid) {
                    $scope.Organization.Item.RoleName = $window.localStorage.getItem("RoleName"); $scope.Organization.Services.UpdateOrganizationDetails($scope.Organization.Item);
                }
            },
            setCountries: function (data) {
                $scope.Organization.Items.Countries = data;
            },
            setStates: function (data) {

                $scope.Organization.Items.States = data;
            },
            SetOrganizationDetails: function (data) {
                if ($scope.Organization.Item.ToAddress != "" && data != null) {
                    $scope.Organization.Item = data;
                    if (data.User.DOB != null && data.User.DOB != "") {
                        $scope.Organization.Item.User.DOB = new Date(data.User.DOB);
                    }
                }
            },

            CreateRole: function () {
                var data = {
                    UserName: $scope.Organization.Item.User.UserName,
                    RoleName: $scope.Organization.Item.RoleName
                }
                $scope.Organization.Services.CreateRole(data);
            },
            GetSalutationList: function () {
                $scope.Organization.Services.GetSalutationList();
            },
        },
        Services: {
            GetRole: function () {
                $http.get('/api/Provider/GetRole').success($scope.Organization.Methods.SetRole);
            },
            States: function (data) {
                $http.get('/api/Provider/GetStates').success($scope.Organization.Methods.setStates);
            },
            Countries: function (data) {
                $http.get('/api/Provider/GetCountries').success($scope.Organization.Methods.setCountries);
            },

            CreateRole: function (data) {
                $http.post('/Provider/CreateRole/', data).success(function (value) {
                    if (value != null && value != "") {
                        toaster.pop('success', '', "Role assigned successfully.");
                    }
                    window.location.href = "/Provider/RegistrationDetails";
                });
            },
            GetOrganizationDetails: function (data) {
                if (data != undefined && data != null) {
                    $http.get('/api/Organization/GetOrganizationDetails/?UserName=' + data).success(
                        $scope.Organization.Methods.SetOrganizationDetails
                    );
                }
            },
            UpdateOrganizationDetails: function (data) {
                $scope.Organization.IsLoading = true;
                $http.post('/api/Organization/UpdateOrganizationDetail/', data).success(function (value) {
                    if (value) {
                        toaster.pop('success', '', "Organization details has been updated.");
                        $scope.Organization.IsLoading = false;
                        setTimeout(function () {
                            window.location.href = "/MyPracticeList";
                        }, 2000)
                    }
                });
            },
            GetSalutationList: function () {
                $http.get('/api/Provider/GetSalutationList').success(function (data) {
                    $scope.Organization.SalutationList = data;
                });
            },
        },
        UI: {
        },
    }

    $scope.Organization.Methods.Initialize();
}]);


