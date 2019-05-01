app.controller('PracticeController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$cookies', '$filter', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $cookies, $filter) {

    $scope.Practice = {
        SalutationList: [],
        OrganizationList: [],
        isPracticeCreated: false,
        PracticeId: '',
        OrganizationId: '',
        isRoleExist: false,
        RoleList: [],
        PracticeRole: { RoleId: 0, RoleName: '' },
        UserId: '',
        UserName: '',
        duplicates_Medicare: [],
        duplicate: [],
        Item: { Practice: {}, User: { Gender: 'Male' }, Role: "practice", ID: '', isPractice: false, },
        Items: [],
        IsLoading: false,
        issubmitted: false,
        isRoleSubmitted: false,
        SecretQuestionlist: [],
        isUserExist: false,
        IspasswordMatch: false,
        isMedicureExist: false,
        RegisteredSince: new Date(),
        files: [],        
        Methods: {
            Initialize: function () {
                if ($cookies.get("username") != undefined) {
                    $scope.Practice.UserName = $cookies.get("username");
                    $scope.Practice.Methods.GetUserIdByUserName($scope.Practice.UserName);
                }

                var UserId = $window.localStorage.getItem("UserId")
                if (UserId != null && UserId != "") {
                    $scope.Practice.UserId = UserId;
                }

                var OrganizationId = $window.localStorage.getItem("OrganizationId");

                if (OrganizationId != undefined && OrganizationId != null && OrganizationId != "") {
                    $scope.Practice.Item.OrganizationID = OrganizationId;

                    var PracticeId = $window.localStorage.getItem("PracticeId");

                    if (PracticeId != undefined && PracticeId != null && PracticeId != "")
                        $scope.Practice.PracticeId = PracticeId;
                }
                else {
                    $scope.Practice.Methods.GetOrganizationDetailsByUserName();
                }

                if (window.location.href.indexOf("/Practice/Create") > -1 || window.location.href.indexOf("/MyPracticeDetail") > -1 || window.location.href.indexOf("/Practice/MyDetails") > -1) {
                    $scope.Practice.Methods.getStates();
                    $scope.Practice.Methods.getCountries();
                    //$scope.Practice.Methods.GetSecretQuestion();
                    $scope.Practice.Methods.GetOrganizationList();
                    $scope.Practice.Methods.GetSalutationList();
                }

                if (window.location.href.indexOf("/MyPracticeDetail") > -1) {
                    var PracticeUserId = $window.localStorage.getItem("PracticeUserId");
                    if (PracticeUserId != null && PracticeUserId != "") {
                        $scope.Practice.Methods.GetPracticeDetail(PracticeUserId);
                    }
                }

                if (window.location.href.indexOf("/MyPracticeList") > -1) {
                    $window.localStorage.setItem("PracticeUserId", "")
                    $scope.Practice.Methods.GetPracticeList();
                }
            },
            getCountries: function () {
                $scope.Practice.Services.Countries();
            },
            getStates: function () {
                $scope.Practice.Services.States();
            },
            setCountries: function (data) {
                $scope.Practice.Items.Countries = data;
            },
            setStates: function (data) {
                $scope.Practice.Items.States = data;
            },
            GetSecretQuestion: function () {
                $scope.Practice.Services.GetSecretQuestion();
            },
            CheckExistingUser: function (UserName) {
                $scope.Practice.Services.CheckExistingUser(UserName);
            },
            comparePassword: function () {
                if ($scope.Practice.Item.ConfirmPassword != undefined && $scope.Practice.Item.ConfirmPassword != "") {
                    if ($scope.Practice.Item.Password != $scope.Practice.Item.ConfirmPassword)
                        $scope.Practice.IspasswordMatch = true;
                    else
                        $scope.Practice.IspasswordMatch = false;
                }
            },

            SendVerificationMail: function () {             
                $scope.Practice.Services.SendVerificationMail($scope.Practice.Item.Email, $scope.Practice.Item.Password, $scope.Practice.Item.Practice.UserId);
                $scope.Practice.Services.GetEmailVerificationStatus($scope.Practice.Item.Email);
            },
            GetEmailVerificationStatus: function (UserName) {
                $scope.Practice.Services.GetEmailVerificationStatus(UserName);
            },
            Register: function (data) {
                $scope.Practice.issubmitted = true;
                if ($scope.formAddPractice.$valid && (!$scope.Practice.isMedicureExist)) {
                    $scope.Practice.IsLoading = true;
                    $scope.Practice.Item.isPractice = true;
                    $scope.Practice.Item.Role = 'Practice';
                    $scope.Practice.Item.Email = $scope.Practice.Item.Contact.EmailPersonal;
                    $scope.Practice.Item.Password = "123456";
                    $scope.Practice.Item.ConfirmPassword = "123456";
                    $scope.Practice.Item.SecretQuestionID = 0;
                    $scope.Practice.Services.Register($scope.Practice.Item);
                }
            },
            CreatePractice: function (data) {
                $scope.Practice.issubmitted = true;
                if ($scope.formAddPractice.$valid) {
                    $scope.Practice.Services.CreatePractice($scope.Practice.Item);
                }
            },
            SetPractice: function (data) {
                $scope.Practice.Items = data;
                if ($.fn.DataTable.isDataTable('.sample_1')) {
                    $(".sample_1").dataTable().fnDestroy();
                }
                setTimeout(function () {
                    $scope.Practice.IsLoading = false;
                    $scope.table = $(".sample_1").dataTable({
                        "paging": true,
                        "ordering": true,
                        "searching": true,
                        "autoFill": true,
                        "dom": 'ftip',
                        "pagingType": "simple_numbers"
                    });
                }, 300);
            },
            GetPracticeList: function () {
                $scope.Practice.Services.GetPracticeList($scope.Practice.UserName);
            },
            GetPracticeDetail: function (UserId) {
                $scope.Practice.Services.GetPracticeDetail(UserId);
            },
            SetPracticeDetail: function (data) {
                $scope.Practice.Item = data;
                if (data != null) {
                    $scope.Practice.Item.ID = data.ID;

                    if (window.location.href.indexOf("/MyPracticeDetail") > -1 || window.location.href.indexOf("/Practice/MyDetails") > -1) {
                        $scope.Practice.Methods.GetPracticeRoleList();
                    }
                }
            },
            RedirectToPracticeDetail: function (userId) {
                $window.localStorage.setItem("PracticeUserId", userId);
                $window.location.href = "/MyPracticeDetail";
            },
            GetUserIdByUserName: function (UserName) {
                $scope.Practice.Services.GetUserIdByUserName(UserName)
            },
            ManagePracticeRole: function () {
                var item = {
                    RoleId: $scope.Practice.PracticeRole.RoleId,
                    RoleName: $scope.Practice.PracticeRole.RoleName,
                    PracticeId: $scope.Practice.Item.ID,
                    UserId: $scope.Practice.UserId,
                    OrganizationId: $scope.Practice.Item.OrganizationID
                }
                $scope.Practice.isRoleSubmitted = true;
                if ($scope.formPracticeRole.$valid && (!$scope.Practice.isRoleExist)) {
                    $scope.Practice.isRoleSubmitted = false;
                    $scope.Practice.Services.ManagePracticeRole(item);
                }
            },
            SetEditPracticeRole: function (RoleName, RoleId) {
                $scope.Practice.PracticeRole.RoleName = RoleName;
                $scope.Practice.PracticeRole.RoleId = RoleId;
            },
            GetPracticeRoleList: function () {
                $scope.Practice.Services.GetPracticeRoleList($scope.Practice.Item.OrganizationID, $scope.Practice.UserId, $scope.Practice.Item.ID);
            },
            DeletePracticeRole: function (RoleId) {
                $scope.Practice.Services.DeletePracticeRole(RoleId);
            },
            CheckPracticeRoleExist: function (RoleName) {
                $scope.Practice.Services.CheckPracticeRoleExist($scope.Practice.Item.ID, RoleName, $scope.Practice.PracticeRole.RoleId, $scope.Practice.UserId, $scope.Practice.Item.OrganizationID);
            },
            GetProviderPatientDetailByUserName: function () {
                $scope.Practice.Services.GetProviderPatientDetailByUserName($scope.Practice.UserName, $scope.Practice.Item.OrganizationID, $scope.Practice.PracticeId);
            },
            GetOrganizationList: function () {
                $scope.Practice.Services.GetOrganizationList();
            },
            CancelRole: function () {
                $scope.Practice.isRoleSubmitted = false;
                $scope.Practice.PracticeRole.RoleName = '';
                $scope.Practice.PracticeRole.RoleId = '';
            },
            GetSalutationList: function () {
                $scope.Practice.Services.GetSalutationList();
            },
            GetOrganizationDetailsByUserName: function () {
                $scope.Practice.Services.GetOrganizationDetailsByUserName($scope.Practice.UserName);
            },
        },
        Services: {
            GetEmailVerificationStatus: function (UserName) {
                $http.get('/api/Provider/GetEmailVerificationStatus?UserName=' + UserName).success(function (data) {
                    $scope.Practice.isEmailVerified = data;
                });
            },
            SendVerificationMail: function (userName, password, userId) {
                $http.post('/api/Email/SendPracticeRegistrationMail/?ToAddress=' + userName + "&Password=" + password + "&userId=" + userId ).success(function (value) {
                    if (value != null) {
                        if (value == "1") {
                            toaster.pop('success', '', "Mail has been sent.");
                        }
                        else {
                            toaster.pop('success', '', "Mail has been sent already.");
                        }
                        window.location.href = "/MyPracticeList";
                    }
                });
            },
          
            Register: function (data) {
                $http.post('/Provider/Register/', data).success(function (value) {
                    if (value != null) {
                        $scope.Practice.Item.Practice.UserId = value.UserId;
                        $scope.Practice.Item.CurrentUserName = $scope.Practice.UserName;
                        $scope.Practice.Item.User.UserName = value.Email;
                        $scope.Practice.Methods.CreatePractice(data);
                    }
                });
            },
            CreatePractice: function (data) {
                $scope.Practice.IsLoading = true;
                $http.post('/api/Practice/UpdatePracticeDetail/', data).success(function (value) {
                    if (value != null && value != "") {
                        var arr = value.split('_');
                        $scope.Practice.PracticeId = arr[0];
                        $scope.Practice.Item.ID = arr[0];
                        var isCreate = arr[1];
                        if (isCreate == "1") {
                            $scope.Practice.Methods.SendVerificationMail();
                        }
                        $scope.Practice.isPracticeCreated = true;
                        toaster.pop('success', '', "Practice created successfully.");
                        $scope.Practice.IsLoading = false;
                        if (window.location.href.indexOf("/MyPracticeDetail") > -1) {
                            setTimeout(function () {
                                $scope.Practice.Methods.GetPracticeList();
                                window.location.href = "/MyPracticeList";
                            }, 1000);
                        }
                    }
                });
            },
            States: function (data) {
                $http.get('/api/Provider/GetStates').success($scope.Practice.Methods.setStates);
            },
            Countries: function (data) {
                $http.get('/api/Provider/GetCountries').success($scope.Practice.Methods.setCountries);
            },
            GetSecretQuestion: function () {
                $http.get('/api/Provider/GetSecretQuestion').success(function (data) {
                    $scope.Practice.SecretQuestionlist = data;
                    if (data.length > 0) {
                        $scope.Practice.Item.SecretQuestionID = data[0].ID;
                    }
                });
            },
            CheckExistingUser: function (UserName) {
                $http.get('/api/Provider/CheckExistingUser?UserName=' + UserName).success(function (data) {
                    $scope.Practice.isUserExist = data;
                });
            },
            GetPracticeList: function (UserName) {
                $http.get('/api/Practice/GetPracticeListBy_OrganizationID?UserName=' + UserName + '&OrganizationID=' + '').success(function (data) {
                    $scope.Practice.Methods.SetPractice(data);
                });
            },
            GetPracticeDetail: function (UserId) {
                $http.get('/api/Practice/GetPracticeDetail?UserId=' + UserId).success(function (data) {
                    $scope.Practice.Methods.SetPracticeDetail(data);
                });
            },
            GetProviderPatientDetailByUserName: function (UserName, OrganizationId, PracticeId) {
                $http.get('/api/Patient/GetProviderPatientDetailByUserName?UserName=' + UserName + '&OrganizationId=' + OrganizationId + '&PracticeId=' + PracticeId).success($scope.Practice.Methods.SetPatientDetails);
            },
            GetUserIdByUserName: function (UserName) {
                $http.get('/api/Practice/GetUserIdByUserName?UserName=' + UserName).success(function (response) {
                    $scope.Practice.UserId = response;
                    $window.localStorage.setItem("UserId", $scope.Practice.UserId);

                    if (window.location.href.indexOf("/Practice/MyDetails") > -1) {
                        $scope.Practice.Methods.GetPracticeDetail($scope.Practice.UserId);
                    }
                });
            },
            ManagePracticeRole: function (data) {
                $scope.Practice.IsLoading = true;
                $http.post('/api/Practice/ManagePracticeRole/', data).success(function (value) {
                    if (value) {
                        toaster.pop('success', '', "Role created successfully.");
                    }
                    else if (!value) {
                        toaster.pop('success', '', "Role updated successfully.");
                    }
                    $scope.formPracticeRole.$setUntouched();
                    $scope.formPracticeRole.$setPristine();

                    $scope.Practice.PracticeRole.RoleName = '';
                    $scope.Practice.PracticeRole.RoleId = 0;
                    $scope.Practice.Methods.GetPracticeRoleList();
                    $scope.Practice.IsLoading = false;
                });
            },
            GetPracticeRoleList: function (OrgId, UserId, PracticeId) {
                $http.get('/api/Practice/GetPracticeRole?OrganizationId=' + OrgId + '&UserId=' + UserId + '&PracticeId=' + PracticeId).success(function (data) {
                    $scope.Practice.RoleList = data;
                });
            },
            DeletePracticeRole: function (RoleId) {
                $http.delete('/api/Practice/DeletePracticeRole?RoleId=' + RoleId).success(function (response) {
                    if (response) {
                        $scope.Practice.Methods.GetPracticeRoleList();
                        toaster.pop('success', '', "Role deleted successfully.");
                    }
                });
            },
            CheckPracticeRoleExist: function (PracticeId, RoleName, RoleID, UserId, OrgId) {
                $http.get('/api/Practice/CheckPracticeRoleExist?PracticeId=' + PracticeId + '&RoleName=' + RoleName + '&RoleID=' + RoleID + '&UserId=' + UserId + '&OrganizationId=' + OrgId).success(function (response) {
                    $scope.Practice.isRoleExist = response;
                });
            },
            GetOrganizationList: function () {
                $http.get('/api/Provider/GetOrganizationList').success(function (data) {
                    $scope.Practice.OrganizationList = data;
                });
            },
            GetSalutationList: function () {
                $http.get('/api/Provider/GetSalutationList').success(function (data) {
                    $scope.Practice.SalutationList = data;
                });
            },
            GetOrganizationDetailsByUserName: function (username) {
                if (username != undefined && username != null) {
                    $http.get('/api/Organization/GetOrganizationDetails/?UserName=' + username).success(function (response) {
                        if (response != null) {
                            $scope.Practice.Item.OrganizationID = response.ID;
                        }
                    });
                }
            },
        }, UI: {
        }
    }

    $scope.Practice.Methods.Initialize();

}]);