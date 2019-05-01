app.controller('providerController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$cookies', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $cookies) {
    $scope.Provider = {
        SalutationList: [],
        ProviderID: '',
        Item: { SecretQuestionID: '1', Answer: '', User: { } },
        Items: [],
        issubmitted: false,
        IstermsAccept: false,
        fromlink: false,
        acceptTerms: '',
        IspasswordMatch: false,
        tabSelectedIndex: 0,
        isEmailVerified: false,
        isUserExist: false,
        SecretQuestionlist: [],
        IsLoading: false,
        IsStartLoading: false,
        RegisterEmail: '',
        Methods: {
            Initialize: function () {
                if ($cookies.get("username") != undefined) {
                    $scope.Provider.Item.ToAddress = $cookies.get("username");
                    $scope.Provider.RegisterEmail = $cookies.get("username");
                }
                if (window.location.href.indexOf("/Register") < 0 || window.location.href.indexOf("Provider/RegistrationDetails") < 0) {
                    $scope.Provider.IsStartLoading = true;
                }
                
                $scope.Provider.Services.GetRole();
                $scope.Provider.Services.States();
                $scope.Provider.Services.Countries();
                $scope.Provider.Methods.GetSecretQuestion();
                $scope.Provider.Methods.GetSalutationList();
                $scope.Provider.Services.GetEmailVerificationStatus($cookies.get("username"));
                $scope.Provider.Item.User.UserName = $scope.Provider.Item.ToAddress;
                $scope.Provider.Services.Providers($scope.Provider.Item.ToAddress);
                $scope.Provider.Methods.GetProviderIDByUserName();

                $scope.Provider.IsStartLoading = false;
            },
            Register: function (data) {
                $scope.Provider.issubmitted = true;
                if ($scope.frmRegister.$valid && (!$scope.Provider.isUserExist) && (!$scope.Provider.IspasswordMatch)) {
                    $scope.Provider.issubmitted = false;
                    $scope.Provider.Services.Register($scope.Provider.Item);
                }
            },
            SendVerificationMail: function () {
                var email = $scope.Provider.RegisterEmail;
                $scope.Provider.Services.SendVerificationMail(email);
                $scope.Provider.Services.GetEmailVerificationStatus(email);
            },
            SetRole: function (data) {
                $scope.Provider.Items.RoleList = data;
            },
            CreateProvider: function (data) {
                $scope.Provider.issubmitted = true;
                if ($scope.frmRegistration.$valid) {
                    if ($scope.Provider.Item.RoleName != null || $scope.Provider.Item.RoleName != undefined) {
                        $window.localStorage.setItem("RoleName", $scope.Provider.Item.RoleName);
                    }
                    $scope.Provider.Services.CreateProvider($scope.Provider.Item);
                }
            },
            UpdateProviderDetails: function (data) {
                $scope.Provider.issubmitted = true;
                $scope.Provider.Item.User.UserName = $cookies.get("username");
                if ($scope.formRegistrationDetail.$valid) {
                    $scope.Provider.Item.RoleName = $window.localStorage.getItem("RoleName");
                    $scope.Provider.Services.UpdateProviderDetails($scope.Provider.Item);
                }
            },
            UpdateContactDetails: function (index, data) {
                $scope.Provider.Services.UpdateContactDetails(index, $scope.Provider.Item);
            },
            UpdateEmailDetails: function (index) {
                if ($scope.formEmail.$valid) {
                    $scope.Provider.Services.UpdateContactDetails(index, $scope.Provider.Item);
                }
            },

            setCountries: function (data) {
                $scope.Provider.Items.Countries = data;
            },
            setStates: function (data) {

                $scope.Provider.Items.States = data;
            },
            setProvider: function (data) {
                $scope.Provider.IsLoading = false;

                $scope.Provider.Item = data;
            },
            comparePassword: function () {
                if ($scope.Provider.Item.confirmPassword != undefined && $scope.Provider.Item.confirmPassword != "") {
                    if ($scope.Provider.Item.Password != $scope.Provider.Item.confirmPassword)
                        $scope.Provider.IspasswordMatch = true;
                    else
                        $scope.Provider.IspasswordMatch = false;
                }
            },
            GetEmailVerificationStatus: function (UserName) {
                $scope.Provider.Services.GetEmailVerificationStatus(UserName);
            },
            CheckExistingUser: function (UserName) {
                $scope.Provider.Services.CheckExistingUser(UserName);
            },
            GetSecretQuestion: function () {
                $scope.Provider.Services.GetSecretQuestion();
            },
            redirectToDashboard: function () {
                window.location.href = "/Provider/Dashboard";
            },
            ChangePassword: function () {
                if (!$scope.Provider.IspasswordMatch) {
                    $scope.Provider.Services.ChangePassword($scope.Provider.Item.User.UserName, $scope.Provider.Item.Password);
                }
            },
            GetProviderIDByUserName: function () {
                $scope.Provider.Services.GetProviderIDByUserName($scope.Provider.Item.User.UserName);
            },
            SetProviderIDByUserName: function (providerid) {
                $scope.Provider.ProviderID = providerid;
            },

            CreateRole: function () {
                var data = {
                    UserName: $scope.Provider.RegisterEmail,
                    RoleName: $scope.Provider.Item.RoleName
                }
                $scope.Provider.issubmitted = true;
                if ($scope.frmRegistration.$valid) {
                    $scope.Provider.Services.CreateRole(data);
                    $scope.Provider.issubmitted = false;
                }
            },
            SetPractice: function (data) {
                $scope.Provider.Items.Practice = data;
            },
            GetSalutationList: function () {
                $scope.Provider.Services.GetSalutationList();
            },
        },
        Services: {
            GetProviderIDByUserName: function (UserName) {
                $http.get('/api/Provider/GetProviderIDByUserName?UserName=' + UserName).success($scope.Provider.Methods.SetProviderIDByUserName);
            },
            Register: function (data) {
                debugger;
                $scope.Provider.IsLoading = true;
                $http.post('/Provider/Register/', data).success(function (value) {
                    if (value != null) {
                        $scope.Provider.RegisterEmail = value.Email;
                        $scope.Provider.Methods.SendVerificationMail();
                        toaster.pop('success', '', "Registered successfully. Please verify through your registered email.");

                        setTimeout(function () {
                            $scope.Provider.IsLoading = false;
                            window.location.href = "/Login";
                        }, 5000)
                    }
                    else {
                        $scope.Provider.IsLoading = true;
                    }
                });
            },
            ChangePassword: function (email, newpassword) {
                $http.post('/Provider/ChangePassword?Email=' + email + "&NewPassword=" + newpassword).success(function (value) {
                    if (value != null) {
                        $scope.Provider.tabSelectedIndex = 0;
                        //$scope.Provider.Item.User.UserName = value.Email;
                        toaster.pop('success', '', "Password Changed successfully.");
                        $scope.Provider.Item.User.UserName = "";
                        $scope.Provider.Item.Password = "";
                    }
                });
            },
            SendVerificationMail: function (data) {
                $http.post('/api/Email/SendVerificationMail/?ToAddress=' + data).success(function (value) {
                    if (value != null) {
                        if (value == "1") {
                            $scope.Provider.isEmailVerified = true;
                            $scope.Provider.emailNotification = "Please verify your account through email notification link sent on your register email address"
                        }
                        else {
                            $scope.Provider.isEmailVerified = false;
                            toaster.pop('success', '', "Mail has been sent already.");
                        }
                    }
                });
            },
            GetRole: function (data) {
                $http.get('/api/Provider/GetRole').success($scope.Provider.Methods.SetRole);
            },
            States: function (data) {
                $http.get('/api/Provider/GetStates').success($scope.Provider.Methods.setStates);
            },
            Countries: function (data) {
                $http.get('/api/Provider/GetCountries').success($scope.Provider.Methods.setCountries);
            },
            CreateProvider: function (data) {
                $http.post('/api/Provider/Post/', data).success(function (value) {
                    if (value != null) {
                        toaster.pop('success', '', "Provider details have been updated.");
                    }
                });
            },
            CreateRole: function (data) {
                $http.post('/Provider/CreateRole/', data).success(function (value) {
                    if (value != null && value != "") {
                        toaster.pop('success', '', "Role assigned successfully.");
                    }

                    if (data.RoleName.indexOf("organization") > -1)
                        window.location.href = "/Organization/OrganizationDetail?from=role";
                    if (data.RoleName.indexOf("provider") > -1)
                        window.location.href = "/Provider/RegistrationDetails?from=role";
                    if (data.RoleName.indexOf("practice") > -1)
                        window.location.href = "/Practice/MyDetails";
                });
            },
            Providers: function (data) {
                $scope.Provider.IsLoading = true;
                //if (data != undefined && data != null) {
                $http.get('/api/Provider/Get/?UserName=' + data).success(
                    $scope.Provider.Methods.setProvider
                );
                //}
            },
            UpdateProviderDetails: function (data) {
                $scope.Provider.IsLoading = true;
                $http.put('/api/Provider/Put/', data).success(function (value) {
                    if (value != null) {
                        toaster.pop('success', '', "Provider details have been updated.");
                        $scope.Provider.IsLoading = false;

                        setTimeout(function () {
                            window.location.href = "/Provider/ProviderOrganization";
                        }, 2000);
                    }
                });
            },
            UpdateContactDetails: function (index, data) {
                $http.put('/api/Provider/Put/', data).success(function (value) {
                    if (value != null) {
                        $scope.Provider.tabSelectedIndex = index;
                        toaster.pop('success', '', "Contact details have been updated.");
                        $window.localStorage.clear();
                    }
                    else {
                        toaster.pop('error', '', "There are some issues while updating the entries.");
                    }
                });
            },
            GetEmailVerificationStatus: function (UserName) {
                $http.get('/api/Provider/GetEmailVerificationStatus?UserName=' + UserName).success(function (data) {
                    $scope.Provider.isEmailVerified = data;
                });
            },
            CheckExistingUser: function (UserName) {
                $http.get('/api/Provider/CheckExistingUser?UserName=' + UserName).success(function (data) {
                    $scope.Provider.isUserExist = data;
                });
            },
            GetSecretQuestion: function () {
                $http.get('/api/Provider/GetSecretQuestion').success(function (data) {
                    $scope.Provider.SecretQuestionlist = data;
                });
            },
            GetSalutationList: function () {
                $http.get('/api/Provider/GetSalutationList').success(function (data) {
                    $scope.Provider.SalutationList = data;
                });
            },
        },
        UI: {
        },
    }

    $scope.Provider.Methods.Initialize();
}]);