app.controller('ProviderOrganizationController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$cookies', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $cookies) {
    $scope.Organization = {
        PracticeRoles: [],
        Item: {
            User: {},
            Role: 'organization',
            Organization: {}
        },
        Items: [],
        OrganizationList: [],
        issubmitted: false,
        IsLoading: false,
        isPracticeSubmitted: false,
        Methods: {

            Initialize: function () {
                if ($cookies.get("username") != undefined) {
                    $scope.Organization.Item.ToAddress = $cookies.get("username");
                }
                $scope.Organization.Methods.GetProviderIdFromUserId();
                $scope.Organization.Methods.GetOrganizationList();
            },
            SetPractice: function (data) {
                $scope.Organization.Practice = data;
            },
            GetPracticeList: function () {
                $scope.Organization.Services.GetPracticeList($cookies.get("username"));
            },
            GetOrganizationList: function () {
                $scope.Organization.Services.GetOrganizationList();
            },
            CreateProviderOrganization: function () {
                $scope.Organization.issubmitted = true;
                if ($scope.formOrganization.$valid) {
                    $scope.Organization.issubmitted = false;
                    $scope.Organization.Item.Organization.UserName = $cookies.get("username");
                    $scope.Organization.Services.CreateProviderOrganization($scope.Organization.Item.Organization);
                }
            },
            GetProviderOrganizationList: function () {
                $scope.Organization.Services.GetProviderOrganizationList($scope.Organization.ProviderID);
            },
            GetProviderIdFromUserId: function () {
                $scope.Organization.IsLoading = true;
                $scope.Organization.Services.GetProviderIdFromUserId($cookies.get("username"));
            },
            setProviderIdFromUserId: function (ProviderID) {
                $scope.Organization.Services.GetProviderOrganizationList(ProviderID);
            },
            DeleteProviderOrganization: function (Id, OrgId) {
                $scope.Organization.Services.DeleteProviderOrganization(Id, OrgId);
            },
            SetDeleteProviderOrganization: function (response) {
                if (response.indexOf("Successfully") > -1) {
                    toaster.pop('success', '', response);
                }
                else {
                    toaster.pop('info', '', response);
                }
                $scope.Organization.Methods.GetOrganizationList();
                $scope.Organization.Methods.GetProviderOrganizationList();
            },
            CreateProviderPractice: function (id, index) {
                $scope.Organization.isPracticeSubmitted = true;
                if ($scope.formPracticeOrg['practice_' + index].$valid) {
                    $scope.Organization.isPracticeSubmitted = false;
                    $scope.Organization.Services.CreateProviderPractice(
                        {
                            ProviderID: $scope.Organization.ProviderID,
                            PracticeId: $scope.Organization.PracticeId,
                            ProviderOrganizationId: id,
                            CreatedBy: '',
                        });
                }
                else {
                    $scope.formPracticeOrg['practice_' + index].$dirty = true;
                }
            },
            DeleteProviderPractice: function (ProviderPracticeId, PracticeId) {
                $scope.Organization.Services.DeleteProviderPractice(ProviderPracticeId, PracticeId);
            },
            SaveProviderPracticeRole: function (RoleId, Id, ProviderID) {
                $scope.Organization.Services.SaveProviderPracticeRole();
            },
            SaveRole: function (data, ProviderPracticeID) {
                var RoleList = [];
                if (data != null) {
                    for (var i = 0; i < data.length ; i++) {
                        if (data[i].ProviderPracticeID == ProviderPracticeID) {
                            var item = {};
                            item["RoleID"] = data[i].RoleID;
                            item["ProviderPracticeID"] = data[i].ProviderPracticeID;
                            RoleList.push(item);
                        }
                    }
                    $scope.Organization.Services.SaveProviderPracticeRole(RoleList, ProviderPracticeID);
                }
            },
            Exists: function (item, list) {
                if (item.RoleName != undefined && list != undefined) {
                    for (var i = 0; i < list.length; i++) {
                        if (item.RoleID == list[i].RoleID) {
                            return true;
                        }
                    }
                    return false;
                }
            },
            Toggle: function (item, ProviderPracticeID, list) {
                var isSplice = false;
                if (item.RoleName != undefined && list != undefined) {
                    if (list.length > 0) {
                        for (var i = 0; i < list.length; i++) {
                            if (item.RoleID == list[i].RoleID) {
                                list.splice(i, 1);
                                isSplice = true;
                            }
                        }
                        if (isSplice == false) {
                            var roleItem = {};
                            roleItem["RoleID"] = item.RoleID;
                            roleItem["ProviderPracticeID"] = ProviderPracticeID;
                            list.push(roleItem);
                        }
                        return false;
                    }
                    else {
                        item["RoleID"] = item.RoleID;
                        item["ProviderPracticeID"] = ProviderPracticeID;
                        list.push(item);
                    }
                }
            },
            SaveProviderPracticeRole: function (result) {
                if (result) {
                    toaster.pop('success', '', "Role updated successfully.");
                }
                else {
                    toaster.pop('error', '', "There are some issues while updating the entries.");
                }
            },
            setProviderOrganizationList: function (data) {
                $scope.OrganizationList = [];
                $scope.Organization.ProviderOrganizationList = data;
                for (var i = 0; i < $scope.Organization.OrganizationList.length; i++) {
                    for (var j = 0; j < $scope.Organization.ProviderOrganizationList.length; j++) {
                        if ($scope.Organization.OrganizationList[i].ID == $scope.Organization.ProviderOrganizationList[j].OrganizationID) {
                            $scope.Organization.OrganizationList.splice(i, 1);
                        }
                    }
                }
            }
        },
        Services: {
            GetOrganizationList: function () {
                $http.get('/api/Provider/GetOrganizationList').success(function (data) {
                    $scope.Organization.OrganizationList = data;
                });
            },
            CreateProviderOrganization: function (data) {
                $http.post('/api/Provider/CreateProviderOrganization', data).success(function (value) {
                    if (value != null) {
                        $scope.Organization.Item.Organization = {};
                        $scope.formOrganization.$setUntouched();
                        $scope.formOrganization.$setPristine();
                        $scope.Organization.Methods.GetProviderOrganizationList();
                        $scope.Organization.Methods.GetOrganizationList();
                        toaster.pop('success', '', "Organization Details has been Updated.");
                        $window.localStorage.clear();
                    }
                    else {

                        toaster.pop('error', '', "There are some issues while updating the entries.");
                    }
                });
            },
            GetProviderOrganizationList: function (ProviderId) {
                $http.get('/api/Provider/GetProviderOrganizationByProviderID?ProviderId=' + ProviderId).success(function (data) {
                    $scope.Organization.Methods.setProviderOrganizationList(data);
                });
            },
            GetProviderIdFromUserId: function (UserName) {
                $http.get('/api/Provider/GetProviderIdFromUserId?UserName=' + UserName).success(function (data) {
                    $scope.Organization.IsLoading = false;
                    $scope.Organization.ProviderID = data;
                    $scope.Organization.Methods.setProviderIdFromUserId(data);
                });
            },
            DeleteProviderOrganization: function (Id, OrganizationId) {
                $http.delete('/api/Provider/DeleteProviderOrganization?Id=' + Id + "&OrganizationId=" + OrganizationId).success($scope.Organization.Methods.SetDeleteProviderOrganization);
            },
            GetPracticeList: function (UserName) {
                $http.get('/api/Practice/GetPracticeListBy_OrganizationID?UserName=' + UserName + '&OrganizationID=' + '').success(function (data) {
                    $scope.Organization.Methods.SetPractice(data);
                });
            },
            CreateProviderPractice: function (Practice) {
                $http.post('/api/Provider/CreateProviderPractice', Practice).success(function () {
                    $scope.Organization.PracticeId = '';
                    $scope.formPracticeOrg.$setUntouched();
                    $scope.formPracticeOrg.$setPristine();
                    $scope.Organization.Methods.GetProviderOrganizationList();
                    $scope.Organization.Methods.GetOrganizationList();
                    toaster.pop('success', '', "Practice created successfully.");
                });
            },
            DeleteProviderPractice: function (ProviderPracticeId, PracticeId) {
                $http.delete('/api/Provider/DeleteProviderPractice?ProviderPracticeId=' + ProviderPracticeId + "&PracticeId=" + PracticeId).success($scope.Organization.Methods.SetDeleteProviderOrganization);
            },
            SaveProviderPracticeRole: function (ProviderPracticeRoles, ProviderPracticeID) {
                var ProviderPracticeRoles = {
                    ProviderPracticeRoleList: ProviderPracticeRoles,
                    ProviderPracticeId: ProviderPracticeID
                }
                $http.post('/api/Provider/SaveProviderPracticeRole', ProviderPracticeRoles).success($scope.Organization.Methods.SaveProviderPracticeRole);
            }
        },
        UI: {
        },
    }

    $scope.Organization.Methods.Initialize();
}]);


