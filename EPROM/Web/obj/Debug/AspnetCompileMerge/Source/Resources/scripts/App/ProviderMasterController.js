app.controller('ProviderMasterController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$route', '$cookies', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $route, $cookies) {

    $scope.ProviderMaster = {
        IsLoading: false,
        UserName: "",
        OrganizationList: [],
        PracticeList: [],
        OrganizationID: "",
        PracticeID: "",
        isCurrentProviderOrg: false,

        Initialize: function () {
            if ($cookies.get("username") != undefined) {
                $scope.ProviderMaster.UserName = $cookies.get("username");
            }

            $scope.ProviderMaster.Methods.GetProviderOrganizationList();
        },
        Methods: {
            Logout: function () {
                $cookies.remove("tmpData");
                $scope.ProviderMaster.Services.Logout();
            },
            GetProviderOrganizationList: function () {
                $scope.ProviderMaster.isCurrentProviderOrg == false;
                $scope.ProviderMaster.Services.GetProviderOrganizationList($scope.ProviderMaster.UserName);
            },
            GetPracticeListByOrgID: function (OrganizationId) {
                if (OrganizationId != null && OrganizationId != "") {
                    $scope.ProviderMaster.PracticeID = "";
                    $window.localStorage.setItem("PracticeId", "");
                    $window.localStorage.setItem("OrganizationId", OrganizationId);
                    $scope.ProviderMaster.Services.GetPracticeListByOrgID($scope.ProviderMaster.UserName, OrganizationId, "reload");
                }
            },
            SetPracticeList: function (data, isreload) {
                $scope.ProviderMaster.PracticeList = data;
                if (data != null) {
                    if (data.length > 0 && $scope.ProviderMaster.isCurrentProviderOrg == false) {
                        $window.localStorage.setItem("PracticeId", data[0].ID);
                        $scope.ProviderMaster.PracticeID = data[0].ID;
                    }
                }

                if (isreload == "reload") {
                    $window.location.reload();
                }
            },
            SetPracticeID: function (Id) {
                $window.localStorage.setItem("PracticeId", Id);
                $window.location.reload();
            },
        },

        Services: {
            Logout: function () {
                $http.post('/Provider/LogOff/').success(function () {
                    $window.location.href = "/Provider/Login";
                });
            },
            GetProviderOrganizationList: function (UserName) {
                $scope.ProviderMaster.IsLoading = true;
                $http.get('/api/Organization/GetOrganizationByProviderId?UserName=' + UserName).success(function (data) {
                    $scope.ProviderMaster.OrganizationList = data;
                    if (data != null) {
                        $scope.ProviderMaster.IsLoading = false;
                        if (data.length > 0) {
                            var OrganizationId = $window.localStorage.getItem("OrganizationId");

                            if (OrganizationId != undefined && OrganizationId != null && OrganizationId != "") {
                                $scope.ProviderMaster.OrganizationID = OrganizationId;
                                for (var i = 0; i < data.length; i++) {
                                    if (data[i].ID == OrganizationId) {
                                        $scope.ProviderMaster.isCurrentProviderOrg = true;
                                    }
                                }

                                var PracticeId = $window.localStorage.getItem("PracticeId");
                                if (PracticeId != undefined && PracticeId != null && PracticeId != "")
                                    $scope.ProviderMaster.PracticeID = PracticeId;
                            }

                            if (!$scope.ProviderMaster.isCurrentProviderOrg) {
                                OrganizationId = $scope.ProviderMaster.OrganizationList[0].ID;
                                $scope.ProviderMaster.OrganizationID = OrganizationId;
                                $window.localStorage.setItem("OrganizationId", OrganizationId)
                            }
                            $scope.ProviderMaster.Services.GetPracticeListByOrgID(UserName, OrganizationId, "");
                        }
                    }
                    else
                        $scope.ProviderMaster.IsLoading = false;
                });
            },
            GetPracticeListByOrgID: function (UserName, OrgId, isreload) {
                $http.get('/api/Practice/GetOrganizationPracticeByProviderId?UserName=' + UserName + '&OrganizationId=' + OrgId).success(function (data) {
                    $scope.ProviderMaster.IsLoading = false;
                    $scope.ProviderMaster.Methods.SetPracticeList(data, isreload);
                });
            },
        }
    };

    $scope.ProviderMaster.Initialize();
}]);
