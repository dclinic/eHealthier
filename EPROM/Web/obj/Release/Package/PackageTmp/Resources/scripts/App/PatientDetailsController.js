
app.controller('patientDetailsController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$cookies', '$filter', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $cookies, $filter) {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.PatientDetails = {
        OrganizationList: [],
        PracticeList: [],
        isMedicureExist: false,
        isIHINumber: false,
        isactivePatientdetail: false,
        IsDateValid: true,
        PatientUsername: '',
        IndicatorItem: { PatientID: '', PatientIndicatorList: [] },
        Item: {
            SecretQuestionID: '1', Answer: '', User: { Gender: 'Male' }, PracticeId: '',
            OrganizationId: ''
        },
        Items: [],
        States: [],
        Countries: [],
        issubmitted: false,
        isIndicatorsubmitted: false,
        IstermsAccept: false,
        fromlink: false,
        acceptTerms: '',
        IspasswordMatch: false,
        tabSelectedIndex: 0,
        isEmailVerified: false,
        isUserExist: false,
        SecretQuestionlist: [],
        RegisteredSince: new Date(),
        Methods: {

            Initialize: function () {

                if ($cookies.get("username") != undefined && $cookies.get("username") != null && $cookies.get("username") != "") {
                    $scope.PatientDetails.Item.ToAddress = $cookies.get("username");
                }
                else if ($window.localStorage.getItem("username") != null && $window.localStorage.getItem("username") != "") {
                    $scope.PatientDetails.Item.ToAddress = $window.localStorage.getItem("username");
                }

                var OrganizationId = $window.localStorage.getItem("OrganizationId");

                if (OrganizationId != undefined && OrganizationId != null && OrganizationId != "") {
                    $scope.PatientDetails.Item.OrganizationID = OrganizationId;

                    var PracticeId = $window.localStorage.getItem("PracticeId");

                    if (PracticeId != undefined && PracticeId != null && PracticeId != "")
                        $scope.PatientDetails.Item.PracticeID = PracticeId;
                }

                if ($window.localStorage.getItem("UserpatientID") != undefined && $window.localStorage.getItem("UserpatientID") != null && $window.localStorage.getItem("UserpatientID") != "") {
                    $scope.PatientDetails.IndicatorItem.PatientID = $window.localStorage.getItem("UserpatientID");
                }

                $scope.PatientDetails.Services.ProviderTypes();
                $scope.PatientDetails.Services.States();
                $scope.PatientDetails.Services.Countries();
                $scope.PatientDetails.Methods.GetSecretQuestion();
                $scope.PatientDetails.Methods.GetOrganizationList();

                if (window.location.pathname.indexOf("/patient/MyDetails") > -1) {
                    $scope.PatientDetails.Methods.GetIndicators();
                    $scope.PatientDetails.Methods.GetPatientIndicatorListByPatientId();
                    $scope.PatientDetails.PatientUsername = getParameterByName("PatientUsername");
                }

                $scope.PatientDetails.Item.User.UserName = $scope.PatientDetails.Item.ToAddress;
                if ($scope.PatientDetails.PatientUsername != undefined && $scope.PatientDetails.PatientUsername != null && $scope.PatientDetails.PatientUsername != "") {
                    $scope.PatientDetails.Services.GetPatientDetailsByUserName($scope.PatientDetails.PatientUsername);
                }
                else
                    $scope.PatientDetails.Services.GetPatientDetailsByUserName($scope.PatientDetails.Item.User.UserName);
            },
            RedirectToPatientpage: function () {
                $window.location.href = "/Patient/Index";
            },
            RedirecttoMyEproms: function () {
                $window.location.href = "/Patient/PatientEprom";
            },
            Register: function () {
                $scope.PatientDetails.issubmitted = true;
                if (window.location.href.indexOf("patient/MyDetails") > -1) {
                    if ($scope.FormMypatientDetails.$valid && (!$scope.PatientDetails.isUserExist) && (!$scope.PatientDetails.isMedicureExist) && (!$scope.PatientDetails.isIHINumber)) {
                        if ($scope.PatientDetails.Item.Password != null && $scope.PatientDetails.Item.Password != "" && (!$scope.PatientDetails.IspasswordMatch)) {
                            $scope.PatientDetails.Methods.ChangePassword();
                        }
                        $scope.PatientDetails.Services.Register($scope.PatientDetails.Item);
                    }
                }
                else {
                    if ($scope.formPatientDetails.$valid && (!$scope.PatientDetails.isUserExist) && (!$scope.PatientDetails.isMedicureExist) && (!$scope.PatientDetails.isIHINumber)) {
                        if ($scope.PatientDetails.Item.Password != null && $scope.PatientDetails.Item.Password != "" && (!$scope.PatientDetails.IspasswordMatch)) {
                            $scope.PatientDetails.Methods.ChangePassword();
                        }
                        $scope.PatientDetails.Services.Register($scope.PatientDetails.Item);
                    }
                }
            },
            SendVerificationMail: function () {
                var email = $scope.PatientDetails.Item.User.UserName;
                $scope.PatientDetails.Services.SendVerificationMail(email);
                $scope.PatientDetails.Services.GetEmailVerificationStatus(email);
            },
            setProviderTypes: function (data) {
                $scope.PatientDetails.Items.ProviderTypes = data;
            },
            CreateProvider: function (data) {
                $scope.PatientDetails.issubmitted = true;
                if ($scope.frmRegistration.$valid) {
                    if ($scope.PatientDetails.Item.ProviderTypeID != null || $scope.PatientDetails.Item.ProviderTypeID != undefined) {
                        $window.localStorage.setItem("ProviderTypeID", $scope.PatientDetails.Item.ProviderTypeID);
                    }

                    $scope.PatientDetails.Services.CreateProvider($scope.PatientDetails.Item);
                }
            },
            UpdateProviderDetails: function (data) {
                $scope.PatientDetails.issubmitted = true;
                $scope.PatientDetails.Item.User.UserName = $cookies.get("username");
                if ($scope.formRegistrationDetail.$valid) {
                    $scope.PatientDetails.IstermsAccept = $scope.PatientDetails.Methods.getLocalStorage("isTermsAccepted");

                    $scope.PatientDetails.Item.ProviderTypeID = $window.localStorage.getItem("ProviderTypeID");  // remove from registration detail
                    $scope.PatientDetails.Services.UpdateProviderDetails($scope.PatientDetails.Item);
                }
            },
            UpdateContactDetails: function (index, data) {
                $scope.PatientDetails.Services.UpdateContactDetails(index, $scope.PatientDetails.Item);
            },
            setCountries: function (data) {
                $scope.PatientDetails.Items.Countries = data;
            },
            setStates: function (data) {
                $scope.PatientDetails.Items.States = data;
            },
            setProvider: function (data) {
                if ($scope.PatientDetails.Item.ToAddress != "" && data != null) {
                    $scope.PatientDetails.Item = data;
                }
            },
            fromTermsLink: function () {
                $scope.PatientDetails.fromlink = true;
            },
            comparePassword: function () {
                if ($scope.PatientDetails.Item.ConfirmPassword != undefined && $scope.PatientDetails.Item.ConfirmPassword != "") {
                    if ($scope.PatientDetails.Item.Password != $scope.PatientDetails.Item.ConfirmPassword)
                        $scope.PatientDetails.IspasswordMatch = true;
                    else
                        $scope.PatientDetails.IspasswordMatch = false;
                }
            },
            GetEmailVerificationStatus: function (UserName) {
                $scope.PatientDetails.Services.GetEmailVerificationStatus(UserName);
            },
            CheckExistingUser: function (UserName) {
                $scope.PatientDetails.Services.CheckExistingUser(UserName);
            },
            GetSecretQuestion: function () {
                $scope.PatientDetails.Services.GetSecretQuestion();
            },
            redirectToDashboard: function () {
                window.location.href = "/Provider/Dashboard";
            },
            setLocalStorage: function (key, value) {
                $window.localStorage.setItem(key, value);
            },
            getLocalStorage: function (key, value) {
                return $window.localStorage.getItem(key);
            },
            ChangePassword: function () {
                $scope.PatientDetails.Services.ChangePassword($scope.PatientDetails.Item.User.UserName, $scope.PatientDetails.Item.Password);
            },
            SetPatientDetails: function (data) {
                if (data != undefined && data != null) {
                    $scope.PatientDetails.Item = data;
                    $window.localStorage.setItem("PatientId", data.ID);
                    $scope.PatientDetails.isActiveDeactive = $scope.PatientDetails.Item.IsActive;

                    var OrganizationId = $window.localStorage.getItem("OrganizationId");
                    if (OrganizationId != undefined && OrganizationId != null && OrganizationId != "") {
                        $scope.PatientDetails.Item.OrganizationID = OrganizationId;

                        $scope.PatientDetails.Methods.GetPracticeListByOrgID(OrganizationId)

                        var PracticeId = $window.localStorage.getItem("PracticeId");

                        if (PracticeId != undefined && PracticeId != null && PracticeId != "")
                            $scope.PatientDetails.Item.PracticeID = PracticeId;
                    }

                    //Question Hetal 
                    //$scope.PatientDetails.Methods.GetPracticeListByOrgID(data.OrganizationID)
                    //
                    $scope.PatientDetails.Item.User.DOB = new Date(data.User.DOB);
                }
            },
            GetIndicators: function () {
                $scope.PatientDetails.Services.GetIndicators();
            },
            SetIndicators: function (data) {
                $scope.PatientDetails.Items.Indicators = JSON.parse(data);
            },
            CheckDateValidation: function (startdate, enddate) {
                $scope.errMessage = '';
                var curDate = new Date();
                if (enddate != null && enddate != undefined && enddate != '') {
                    if (new Date(startdate) > new Date(enddate)) {
                        toaster.pop('warning', '', "End Date should be greater than the Start Date");
                        $scope.PatientDetails.IsDateValid = false;
                        return false;
                    }
                    else {
                        $scope.PatientDetails.IsDateValid = true;
                    }
                }
            },
            GenerateIndicatorRow: function () {
                $scope.PatientDetails.isIndicatorsubmitted = true;

                if ($scope.FormIndicator.$valid) {
                    $scope.PatientDetails.isIndicatorsubmitted = false;
                    var newRow = {
                        IndicatorID: '',
                        StartDate: '',
                        Frequency: '',
                        Unit: '',
                        Goal: '',
                        Comments: '',
                        EndDate: '',
                        IsActive: true,
                        Status: 'Yes'
                    };
                    $scope.PatientDetails.IndicatorItem.PatientIndicatorList.push(newRow);
                }
            },
            CreatePatientIndicators: function () {
                $scope.PatientDetails.isIndicatorsubmitted = true;
                if ($scope.PatientDetails.IsDateValid) {
                    if ($scope.FormIndicator.$valid) {
                        $scope.PatientDetails.Services.CreatePatientIndicators($scope.PatientDetails.IndicatorItem);
                    }
                }
                else {
                    toaster.pop('warning', '', "End Date should be greater than the Start Date");
                }
            },
            GetPatientIndicatorListByPatientId: function () {
                $scope.PatientDetails.IsLoading = true;
                $scope.PatientDetails.Services.GetPatientIndicatorListByPatientId($scope.PatientDetails.IndicatorItem.PatientID);
            },
            SetPatientIndicatorsData: function (data) {
                $scope.PatientDetails.IsLoading = false;
                $scope.PatientDetails.IndicatorItem.PatientIndicatorList = JSON.parse(data);
                if ($scope.PatientDetails.IndicatorItem.PatientIndicatorList.length == 0) {
                    var newRow = {
                        IndicatorID: '',
                        StartDate: '',
                        Frequency: '',
                        Unit: '',
                        Goal: '',
                        Comments: '',
                        EndDate: '',
                        IsActive: true,
                        Status: 'Yes'
                    };
                    $scope.PatientDetails.IndicatorItem.PatientIndicatorList.push(newRow);
                }
                else {
                    for (var i = 0; i < $scope.PatientDetails.IndicatorItem.PatientIndicatorList.length; i++) {
                        $scope.PatientDetails.IndicatorItem.PatientIndicatorList[i].StartDate = new Date($scope.PatientDetails.IndicatorItem.PatientIndicatorList[i].StartDate);
                        $scope.PatientDetails.IndicatorItem.PatientIndicatorList[i].EndDate = new Date($scope.PatientDetails.IndicatorItem.PatientIndicatorList[i].EndDate);
                    }
                }
            },
            DeleteIndicatorRow: function (rowNo, Id) {
                $scope.PatientDetails.IndicatorItem.PatientIndicatorList.splice(rowNo, 1);
                if (Id != null && Id != undefined)
                    $scope.PatientDetails.Services.DeletePatientIndicators(Id)
            },
            ActiveDeactivePatient: function (status, role) {
                var id = $scope.PatientDetails.Item.ID;
                $scope.PatientDetails.Services.ActiveDeactivePatient(id, status, role);
            },
            OpenConfirmDialog: function () {
                $("#ConfirmDialog").modal('show');
            },
            SetDeactivePatient: function (role) {
                if (role == 'provider') {
                    if ($scope.PatientDetails.Item.IsActive == false) {
                        $scope.PatientDetails.Item.IsActive = true;
                    } else {
                        $scope.PatientDetails.Item.IsActive = false;
                    }
                } else {
                    $scope.PatientDetails.isactivePatientdetail = false;
                }
            },
            CheckExistingMedicure: function (MedicareNumber) {
                $scope.PatientDetails.Services.CheckExistingMedicure(MedicareNumber);
            },
            CheckExistingIHINumber: function (IHINumber) {
                $scope.PatientDetails.Services.CheckExistingIHINumber(IHINumber);
            },
            GetOrganizationList: function () {
                $scope.PatientDetails.Services.GetOrganizationList($cookies.get("username"));
            },
            GetPracticeListByOrgID: function (OrganizationId) {
                if (OrganizationId != null && OrganizationId != '')
                    $scope.PatientDetails.Services.GetPracticeListByOrgID($cookies.get("username"), OrganizationId);
            },
            SetPractice: function (data) {
                $scope.PatientDetails.PracticeList = data;
            },
        },
        Services: {
            Register: function (data) {
                $http.post('/api/Patient/UpdateUserData/', data).success(function (value) {
                    if (value == true) {
                        toaster.pop('success', '', "Patient Detail Updated Successfully.");
                    } else {
                        toaster.pop('warning', '', "There is some issue. Please try again later.");
                    }
                });
            },
            ChangePassword: function (email, newpassword) {
                $http.post('/Provider/ChangePassword?Email=' + email + "&NewPassword=" + newpassword).success(function (value) {
                    if (value != null) {
                        $scope.PatientDetails.tabSelectedIndex = 0;
                        toaster.pop('success', '', "Password Changed successfully.");
                        $scope.PatientDetails.Item.ConfirmPassword = "";
                        $scope.PatientDetails.Item.Password = "";
                    }
                });
            },
            SendVerificationMail: function (data) {
                $http.post('/api/Email/SendVerificationMail/?ToAddress=' + data).success(function (value) {
                    if (value != null) {
                        if (value == "1") {
                            $scope.PatientDetails.isEmailVerified = true;
                            $scope.PatientDetails.emailNotification = "Please verify your account through email notification link sent on your register email address"
                            toaster.pop('success', '', "Mail has been sent.");
                        }
                        else {
                            $scope.PatientDetails.isEmailVerified = false;
                            toaster.pop('success', '', "Mail has been sent already.");
                        }
                    }
                });
            },
            ProviderTypes: function (data) {
                $http.get('/api/Provider/GetProviderType').success($scope.PatientDetails.Methods.setProviderTypes);
            },
            States: function (data) {
                $http.get('/api/Provider/GetStates').success($scope.PatientDetails.Methods.setStates);
            },
            Countries: function (data) {
                $http.get('/api/Provider/GetCountries').success($scope.PatientDetails.Methods.setCountries);
            },
            GetPatientDetailsByUserName: function (data) {
                $http.get('/api/Patient/GetPatientDetailsByUserName/?UserName=' + data).success($scope.PatientDetails.Methods.SetPatientDetails);
            },
            CreateProvider: function (data) {
                $http.post('/api/Provider/Post/', data).success(function (value) {
                    if (value != null) {
                        toaster.pop('success', '', "Provider details have been updated.");
                    }
                    window.location.href = "/Provider/RegistrationDetails";
                });
            },
            Providers: function (data) {
                if (data != undefined && data != null) {
                    $http.get('/api/Provider/Get/?UserName=' + data).success(
                        $scope.PatientDetails.Methods.setProvider
                    );
                }
            },
            UpdateProviderDetails: function (data) {
                $http.put('/api/Provider/Put/', data).success(function (value) {
                    if (value != null) {
                        toaster.pop('success', '', "Provider details have been updated.");
                        window.location.href = "/Provider/ContactDetails";
                    }
                });
            },
            UpdateContactDetails: function (index, data) {
                $http.put('/api/Provider/Put/', data).success(function (value) {
                    if (value != null) {
                        $scope.PatientDetails.tabSelectedIndex = index;
                        toaster.pop('success', '', "Contact details have been updated.");
                    }
                    else {
                        toaster.pop('error', '', "There are some issues while updating the entries.");
                    }
                });
            },
            GetEmailVerificationStatus: function (UserName) {
                $http.get('/api/Provider/GetEmailVerificationStatus?UserName=' + UserName).success(function (data) {
                    $scope.PatientDetails.isEmailVerified = data;
                });
            },
            CheckExistingUser: function (UserName) {
                $http.get('/api/Provider/CheckExistingUser?UserName=' + UserName).success(function (data) {
                    $scope.PatientDetails.isUserExist = data;
                });
            },
            GetSecretQuestion: function () {
                $http.get('/api/Provider/GetSecretQuestion').success(function (data) {
                    $scope.PatientDetails.SecretQuestionlist = data;
                });
            },
            GetIndicators: function (id) {
                $http.get('/api/Indicators/Get').success($scope.PatientDetails.Methods.SetIndicators)
            },
            CreatePatientIndicators: function (data) {
                $http.post('/api/patient/AddPatientIndicators/', data).success(function (response) {
                    if (response != "") {
                        $scope.PatientDetails.IsLoading = false;
                        toaster.pop('success', '', response);
                        $scope.PatientDetails.isIndicatorsubmitted = false;
                    }
                });
            },
            GetPatientIndicatorListByPatientId: function (id) {
                $http.get('/api/patient/GetPatientIndicatorsByPatientID?patientid=' + id).success($scope.PatientDetails.Methods.SetPatientIndicatorsData);
            },
            DeletePatientIndicators: function (Id) {
                $http.delete('/api/patient/DeletePatientIndicators/' + Id).success(function (response) {
                    if (response != "") {
                        $scope.PatientDetails.IsLoading = false;
                        toaster.pop('success', '', "Indicator Deleted successfully.");
                    }
                });
            },
            ActiveDeactivePatient: function (data, status, role) {
                $http.get('/api/patient/ActiveDeactivePatient/?PatientID=' + data + '&status=' + status).success(function (value) {
                    if (value != null) {
                        if (role == "patient") {
                            $window.localStorage.clear();
                            window.location.href = "/login?deactive=" + true;
                        } else {
                            if (status == true) {
                                toaster.pop('success', '', "Patient is active.");
                            } else {
                                toaster.pop('success', '', "Patient is inactive.");
                            }

                        }
                    } else {
                        toaster.pop('error', '', "There are some issues while updating the entries.");
                    }
                });
            },
            CheckExistingMedicure: function (MedicareNumber) {
                $http.get('/api/Provider/CheckExistingMedicure?MedicareNumber=' + MedicareNumber).success(function (data) {
                    $scope.PatientDetails.isMedicureExist = data;
                });
            },
            CheckExistingIHINumber: function (IHINumber) {
                $http.get('/api/Provider/CheckExistingIHINumber?IHINumber=' + IHINumber).success(function (isExist) {
                    $scope.PatientDetails.isIHINumber = isExist;
                });
            },
            GetOrganizationList: function (UserName) {
                $http.get('/api/Organization/GetOrganizationByProviderId?UserName=' + UserName).success(function (data) {
                    $scope.PatientDetails.OrganizationList = data;
                });
            },
            GetPracticeListByOrgID: function (UserName, OrgId) {
                $http.get('/api/Practice/GetOrganizationPracticeByProviderId?UserName=' + UserName + '&OrganizationId=' + OrgId).success(function (data) {
                    $scope.PatientDetails.Methods.SetPractice(data);
                });
            },
        },
        UI: {
        },
    }

    $scope.PatientDetails.Methods.Initialize();
}]);


