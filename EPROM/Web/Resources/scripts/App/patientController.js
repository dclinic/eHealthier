app.controller('patientController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$cookies', '$filter', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $cookies, $filter) {
    $scope.Patient = {
        RegisterData: {},
        isSearch: false,
        ischooseExist: true,
        Choose: 'exist',
        Search: {},
        OrganizationList: [],
        Item: { PatientUserId: 0, isPatient: false, Role: "patient", Salutation: '', User: { Gender: 'Male' }, OrganizationID: '', PracticeID: '', Address: { Line1: '' }, Contact: { Mobile: '' }, IHINumber: '', MedicareNumber: '' },
        Items: [],
        IsLoading: false,
        issubmitted: false,
        SecretQuestionlist: [],
        isUserExist: false,
        IspasswordMatch: false,
        isMedicureExist: false,
        isIHINumber: false,
        RegisteredSince: new Date(),
        files: [],
        CSVData: [],
        PracticeList: [],
        patientname: null,
        isLoadOneTime: true,
        IHINo: '',
        IsNotAllow: false,
        IsNotRegister: false,
        IsEmpty: false,
        days: [],
        months: [],
        years: [],
        day: "",
        month: "",
        year: "",
        MessageData: "Loading …",
        Methods: {
            Initialize: function () {
                if ($cookies.get("username") != undefined) {
                    $scope.Patient.Item.User.UserName = $cookies.get("username");
                }

                $scope.Patient.Methods.GetProviderIDByUserName();

                $scope.Patient.days = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31];
                $scope.Patient.months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
                for (var i = new Date().getFullYear() ; i > 1900; i--)
                    $scope.Patient.years.push(i);

                $scope.Patient.day = 1;
                $scope.Patient.month = "JAN";
                $scope.Patient.year = 2000;

                setTimeout(function () {
                    var OrganizationId = $window.localStorage.getItem("OrganizationId");

                    if (OrganizationId != undefined && OrganizationId != null && OrganizationId != "") {
                        $scope.Patient.Item.OrganizationID = OrganizationId;
                        var PracticeId = $window.localStorage.getItem("PracticeId");
                        if (PracticeId != undefined && PracticeId != null && PracticeId != "")
                            $scope.Patient.Item.PracticeID = PracticeId;
                    }

                    if (window.location.pathname.indexOf("Provider/Dashboard") > -1) {
                        var d = new Date();
                        d.setFullYear(d.getFullYear() - 1);
                        $scope.Patient.FromDate = d;
                    }

                    if (window.location.href.indexOf("PatientId") > -1) {
                        $scope.Patient.Item.PatientID = getParameterByName("PatientId", window.location.href);
                    }

                    if (window.location.pathname.split("/").pop().toLowerCase() == "Index".toLowerCase()) {
                        $scope.Patient.Methods.GetProviderPatientDetailByUserName();
                    }
                    else {
                        $scope.Patient.Methods.GetPatientforDashboard();
                    }

                    if (window.location.pathname.indexOf("SearchPatient") == -1 && window.location.pathname.indexOf("epromAllocation") == -1) {
                        $scope.Patient.Methods.getStates();
                        $scope.Patient.Methods.getCountries();
                    }

                    $scope.Patient.Methods.GetOrganizationList();
                }, 4000);
            },

            CollectIdFromSelectedEprom: function (PatientSurveyId, SurveyId, ProviderId, OrganizationId, PracticeId) {
                if (PatientSurveyId != undefined) {
                    $scope.Patient.PatientSurveyID = PatientSurveyId;
                    $window.localStorage.setItem("PatientSurveyID", PatientSurveyId);
                }

                if (SurveyId != undefined) {
                    $window.localStorage.setItem("SurveyID", SurveyId);
                }
                if (ProviderId != undefined) {
                    $window.localStorage.setItem("ProviderID", ProviderId);
                }
                if (OrganizationId != undefined) {
                    $window.localStorage.setItem("OrganizationId", OrganizationId);
                }
                if (PracticeId != undefined) {
                    $window.localStorage.setItem("PracticeId", PracticeId);
                }
            },

            RedirecttoMyPatientDashboard: function () {
                if ($scope.Patient.PatientSurveyID != null && $scope.Patient.PatientSurveyID != "" && $scope.Patient.FromDate != null && $scope.Patient.FromDate != "" && $scope.Patient.FromDate != undefined) {
                    $window.localStorage.setItem("PatientId", $scope.Patient.Item.PatientID);
                    $window.localStorage.setItem("PatientSurveyID", $scope.Patient.PatientSurveyID);

                    var fromDate = $filter('date')($scope.Patient.FromDate, "MM/dd/yyyy");
                    $window.location.href = "/Dashboard/MyPatientDashaboard?FromDate=" + fromDate;
                }
                else if ($scope.Patient.PatientSurveyID == null || $scope.Patient.PatientSurveyID == "") {
                    toaster.pop('warning', '', "Please select Eproms!");
                }
                else if ($scope.Patient.FromDate == null || $scope.Patient.FromDate == "" || $scope.Patient.FromDate == undefined) {
                    toaster.pop('warning', '', "Please select from date!");
                }
            },
            RedirecttoMyPatientDetails: function (userpatientID, patientUserName) {
                $window.localStorage.setItem("UserpatientID", userpatientID);
                window.location.href = "/patient/MyDetails?PatientUsername=" + patientUserName;
            },
            GetCSVFile: function (e) {
                if ($("#UploadCsvFile")[0].files.length > 0) {
                    for (var i = 0; i < $("#UploadCsvFile")[0].files.length; i++) {
                        $scope.Patient.files = [];
                        $scope.Patient.files.push($("#UploadCsvFile")[0].files[0])
                    }
                    $scope.Patient.Services.UploadCsvFileAndGetData($scope.Patient.files);
                }
                else {
                    toaster.pop('warning', '', "Please choose a file!");
                }
            },
            UploadCsv: function (e) {
                if ($scope.Patient.files.length > 0) {
                    $scope.Patient.Services.UploadCsvFile($scope.Patient.files);
                }
            },
            GetCsvFileData: function (data) {
                $scope.Patient.CSVData = data;
                if ($.fn.DataTable.isDataTable('.csvfiledetails')) {
                    $(".csvfiledetails").dataTable().fnDestroy()
                }
                setTimeout(function () {
                    var table = $(".csvfiledetails").dataTable({
                        "paging": true,
                        "ordering": true,
                        "searching": false,
                        "autoFill": true,
                        "dom": 'lftip',
                        "pagingType": "simple_numbers"
                    });
                    $("#csvdata").append($("#DataTables_Table_1_info"));
                    $("#csvdata").append($("#DataTables_Table_1_paginate"));
                }, 200);
            },
            ViewPatientEprom: function (providerID, patientID, fname, lname, dob, medicareNo, IHINo) {
                var data = {
                    FirstName: fname,
                    LastName: lname,
                    DOB: dob,
                    MedicareNo: medicareNo,
                    IHINo: IHINo,
                }

                $window.localStorage.setItem("surveyList", []);
                $window.localStorage.setItem("patientDetails", JSON.stringify(data));
                $window.localStorage.setItem("ProviderID", providerID);
                $window.localStorage.setItem("PatientId", patientID);

                if (window.location.pathname.indexOf("Patient/epromAllocation") > -1) {
                    window.location.href = "../patienteprom/Index?from=allocate";
                }
                else {
                    window.location.href = "../patienteprom/Index";
                }
            },

            CreatePatient: function (data) {
                $scope.Patient.issubmitted = true;
                if ($scope.formAddPatient.$valid) {
                    $scope.Patient.Services.CreatePatient($scope.Patient.Item);
                }
            },
            Register: function (data) {
                $scope.Patient.issubmitted = true;
                if ($scope.formAddPatient.$valid && (!$scope.Patient.isMedicureExist) && (!$scope.Patient.isIHINumber) && (!$scope.Patient.isUserExist)) {
                    $scope.Patient.IsLoading = true;
                    $scope.Patient.RegisterData.isPatient = true;
                    $scope.Patient.RegisterData.Role = 'patient';
                    $scope.Patient.RegisterData.Email = $scope.Patient.Item.Contact.EmailPersonal;
                    $scope.Patient.RegisterData.Password = "123456";
                    $scope.Patient.RegisterData.ConfirmPassword = "123456";
                    $scope.Patient.RegisterData.SecretQuestionID = 0;
                    $scope.Patient.Services.Register($scope.Patient.RegisterData);
                }
            },
            getCountries: function () {
                $scope.Patient.Services.Countries();
            },
            getStates: function () {
                $scope.Patient.Services.States();
            },
            setCountries: function (data) {
                $scope.Patient.Item.Countries = data;
            },
            setStates: function (data) {
                $scope.Patient.Item.States = data;
            },
            GetSecretQuestion: function () {
                $scope.Patient.Services.GetSecretQuestion();
            },
            CheckExistingUser: function (UserName) {
                $scope.Patient.Services.CheckExistingUser(UserName);
            },
            comparePassword: function () {
                if ($scope.Patient.Item.ConfirmPassword != undefined && $scope.Patient.Item.ConfirmPassword != "") {
                    if ($scope.Patient.Item.Password != $scope.Patient.Item.ConfirmPassword)
                        $scope.Patient.IspasswordMatch = true;
                    else
                        $scope.Patient.IspasswordMatch = false;
                }
            },
            GetPatientDetails: function () {
                $scope.Patient.IsLoading = true;
                $scope.Patient.Methods.GetProviderPatientDetailByUserName();
            },
            SetPatientDetails: function (data) {
                $scope.Patient.Items = data;
                $scope.Patient.IsLoading = false;

                if ($scope.Patient.Items.length == 0) {
                    $scope.Patient.MessageData = "There are no patients";
                }
                else {
                    $scope.Patient.MessageData = "";
                }

                if ($.fn.DataTable.isDataTable('.sample_1')) {
                    $(".sample_1").dataTable().fnDestroy();
                }

                setTimeout(function () {
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

            GetPatientforDashboard: function () {
                $scope.Patient.IsLoading = true;
                $scope.Patient.Services.GetPatientforDashboard($scope.Patient.Item.User.UserName, $scope.Patient.Item.OrganizationID, $scope.Patient.Item.PracticeID);
            },
            SetPatientsNameList: function (data) {
                $scope.Patient.Items = data;
                $scope.Patient.IsLoading = false;

                if ($scope.Patient.Items.length == 0) {
                    $scope.Patient.MessageData = "There are no patients";
                }
                else {
                    $scope.Patient.MessageData = "";
                }
            },
            SetDashboardPatientList: function (data) {
                $scope.Patient.Items = data;
                $scope.Patient.IsLoading = false;

                if ($scope.Patient.Items.length == 0) {
                    $scope.Patient.MessageData = "There are no patients";
                }
                else {
                    $scope.Patient.MessageData = "";
                }

                if ($scope.Patient.isLoadOneTime == true) {
                    setTimeout(function () {
                        $scope.table = $(".sample_1").dataTable({
                            "paging": true,
                            "ordering": true,
                            "searching": true,
                            "autoFill": true,
                            "dom": 'ftip',
                            "pagingType": "simple_numbers"
                        });
                    }, 300);
                }

                if (data.length > 0) {
                    if ($scope.Patient.Item.PatientID != undefined && $scope.Patient.Item.PatientID != "") {
                        $scope.Patient.Item.PatientID = $scope.Patient.Item.PatientID;
                        $scope.Patient.Item.ProviderID = $scope.Patient.Item.ProviderID;
                    }
                    else {
                        $scope.Patient.Item.PatientID = data[0].PatientID;
                        $scope.Patient.Item.ProviderID = data[0].ProviderID;
                    }

                    if (window.location.pathname.toLowerCase().indexOf("patient/epromallocation") == -1 || window.location.pathname.toLowerCase().indexOf("patient/index") == -1) {
                        $scope.Patient.Methods.GetSurveyByPatient_Provider_Org_Practice_IDs();
                    }
                    $window.localStorage.setItem("ProviderID", $scope.Patient.Item.ProviderID);
                }
            },
            GetSurveyByPatient_Provider_Org_Practice_IDs: function () {
                $scope.Patient.IsLoading = true;
                $scope.Patient.Services.GetSurveyByPatient_Provider_Org_Practice_IDs($scope.Patient.Item.PatientID, $scope.Patient.Item.ProviderID, $scope.Patient.Item.OrganizationID, $scope.Patient.Item.PracticeID);
            },
            SetSurveyListByPatientId: function (data) {
                $scope.Patient.IsLoading = false;
                $scope.Patient.Item.surveyList = JSON.parse(data);
                if ($scope.Patient.Item.surveyList != null && $scope.Patient.Item.surveyList.length > 0) {
                    $scope.Patient.Methods.CollectIdFromSelectedEprom($scope.Patient.Item.surveyList[0].ID, $scope.Patient.Item.surveyList[0].SurveyID, $scope.Patient.Item.surveyList[0].ProviderID, $scope.Patient.Item.surveyList[0].OrganizationID, $scope.Patient.Item.surveyList[0].PracticeID)
                }
            },

            SendVerificationMail: function () {
                $scope.Patient.Services.SendVerificationMail($scope.Patient.Item.PatientID, $scope.Patient.Item.ProviderID, $scope.Patient.Item.OrganizationID, $scope.Patient.Item.PracticeID, $scope.Patient.Item.Contact.EmailPersonal, $scope.Patient.RegisterData.Password, $scope.Patient.Item.PatientUserId);
                $scope.Patient.Services.GetEmailVerificationStatus($scope.Patient.Item.Contact.EmailPersonal);
            },
            GetEmailVerificationStatus: function (UserName) {
                $scope.Patient.Services.GetEmailVerificationStatus(UserName);
            },
            CheckExistingMedicure: function (MedicareNumber) {
                $scope.Patient.Services.CheckExistingMedicure(MedicareNumber);
            },
            CheckExistingIHINumber: function (IHINumber) {
                $scope.Patient.Services.CheckExistingIHINumber(IHINumber);
            },
            GetOrganizationList: function () {
                $scope.Patient.Services.GetOrganizationList($cookies.get("username"));
                if ($scope.Patient.Item.OrganizationID != null && $scope.Patient.Item.OrganizationID != '')
                    $scope.Patient.Methods.GetPracticeListByOrgID($scope.Patient.Item.OrganizationID);
            },
            GetPracticeListByOrgID: function (OrganizationId) {
                $scope.Patient.Services.GetPracticeListByOrgID($cookies.get("username"), OrganizationId);
            },
            SetPractice: function (data) {
                if (data != null && data.length > 0) {
                    if ($scope.Patient.Item.PracticeID == null || $scope.Patient.Item.PracticeID == "")
                        $scope.Patient.Item.PracticeID = data[0].ID;
                }
                else {
                    $scope.Patient.Item.PracticeID = '';
                }
                $scope.Patient.PracticeList = data;
            },
            GetProviderPatientDetailByUserName: function () {
                $scope.Patient.IsLoading = true;
                $scope.Patient.Services.GetProviderPatientDetailByUserName($scope.Patient.Item.User.UserName, $scope.Patient.Item.OrganizationID, $scope.Patient.Item.PracticeID);
            },
            PatientSearch: function (selection) {
                $scope.Patient.isSearch = false;
                $scope.Patient.issubmitted = false;
                $scope.frmSearchPatient.$setUntouched();
                $scope.frmSearchPatient.$setPristine();
                $scope.Patient.Item.IHINumber = '';
                $scope.Patient.Item.MedicareNumber = '';

                if (selection == "new")
                    $scope.Patient.ischooseExist = false;
                else
                    $scope.Patient.ischooseExist = true;
            },
            SearchPatientDetail: function (IHINumber, MedicareNumber) {
                if (IHINumber != "" || MedicareNumber != "") {
                    $scope.Patient.isSearch = true;
                    $scope.Patient.IsLoading = true;
                    $scope.Patient.Services.SearchPatientDetail(IHINumber, MedicareNumber, $scope.Patient.Item.ProviderID, $scope.Patient.Item.OrganizationID, $scope.Patient.Item.PracticeID)
                }
                else {
                    toaster.pop('warning', '', "Enter atleast one value.");
                }
            },
            SetPatientDetailsForSearch: function (data) {
                $scope.Patient.IsLoading = false;
                $scope.Patient.Search = data;

                if (data != null) {
                    $scope.Patient.Item.PatientID = data.ID;
                }
            },
            ClearSearch: function () {
                $scope.Patient.isSearch = false;
                $scope.Patient.issubmitted = false;
                $scope.frmSearchPatient.$setUntouched();
                $scope.frmSearchPatient.$setPristine();
                $scope.Patient.Item.IHINumber = '';
                $scope.Patient.Item.MedicareNumber = '';
            },
            GetProviderIDByUserName: function () {
                $scope.Patient.IsLoading = true;
                $scope.Patient.Services.GetProviderIDByUserName($scope.Patient.Item.User.UserName);
            },
            SetProviderIDByUserName: function (providerid) {
                $scope.Patient.Item.ProviderID = providerid;
                $scope.Patient.Item.Address.CountryID = 12;
            },
            CreatePatientProvider: function (patientId) {
                $scope.Patient.IsLoading = true;
                var data = {
                    ProviderID: $scope.Patient.Item.ProviderID,
                    PatientID: patientId,
                    OrganizationID: $scope.Patient.Item.OrganizationID,
                    PracticeID: $scope.Patient.Item.PracticeID
                }
                $scope.Patient.Services.CreatePatientProvider(data);
            },
            SendPatientAssociateWithProviderMail: function () {
                $scope.Patient.Services.SendPatientAssociateWithProviderMail($scope.Patient.Item.PatientID, $scope.Patient.Item.OrganizationID, $scope.Patient.Item.PracticeID, $scope.Patient.Item.ProviderID, $scope.Patient.Search.Contact.EmailPersonal);
            },
            RedirectToDashboard: function (Id) {
                window.location.href = "/Provider/Dashboard?PatientId=" + Id;
            },
            getPatientDetail: function () {
                if ($scope.Patient.IHINo.length == 0) {
                    $scope.Patient.IsEmpty = true;
                }
                else {
                    $scope.Patient.IsEmpty = false;
                    if ($scope.Patient.IHINo.length == 16) {
                        $scope.Patient.IsNotAllow = false;
                        $scope.Patient.IsLoading = true;
                        $scope.Patient.Services.getPatientDetail($scope.Patient.IHINo);
                    }
                    else {
                        $scope.Patient.IsNotAllow = true;
                    }
                }
            },
            SetPatientDetail: function (data) {
                if (data != null && data != undefined && data.length != 0) {
                    $scope.Patient.Item.Salutation = '';
                    $scope.Patient.Item.MedicareNumber = '';

                    $scope.Patient.Item.User.FirstName = data.Firstname;
                    $scope.Patient.Item.User.MiddleName = data.Middlename;
                    $scope.Patient.Item.User.LastName = data.Lastname;
                    $scope.Patient.Item.User.DOB = new Date(data.DOB);
                    $scope.Patient.Item.User.Gender = data.Gender;
                    $scope.Patient.Item.Address.Line1 = data.Line1;
                    $scope.Patient.Item.Address.Line2 = data.Line2;
                    $scope.Patient.Item.Address.Suburb = data.Suburb;
                    $scope.Patient.Item.Address.ZipCode = data.Pincode;
                    $scope.Patient.Item.Address.StateID = data.SID;
                    $scope.Patient.Item.Address.CountryID = '';
                    $scope.Patient.Item.Contact.Mobile = data.telecom;
                    $scope.Patient.Item.Contact.EmailPersonal = data.Email;
                    $scope.Patient.Item.IHINumber = data.IHI;

                    $scope.Patient.IsLoading = false;
                    $scope.Patient.IsNotRegister = false;
                }
                else {
                    $scope.Patient.IsLoading = false;
                    $scope.Patient.IsNotRegister = true;
                }
            }
        },
        Services: {
            getPatientDetail: function (IHINo) {
                $http.get('/api/Patient/getConsumerDetails?IHINo=' + IHINo).success($scope.Patient.Methods.SetPatientDetail);
            },
            GetProviderIDByUserName: function (UserName) {
                $http.get('/api/Provider/GetProviderIDByUserName?UserName=' + UserName).success($scope.Patient.Methods.SetProviderIDByUserName);
            },
            GetEmailVerificationStatus: function (UserName) {
                $http.get('/api/Provider/GetEmailVerificationStatus?UserName=' + UserName).success(function (data) {
                    $scope.Patient.isEmailVerified = data;
                });
            },
            SendVerificationMail: function (patientId, providerId, organizationId, practiceId, userName, password, userId) {
                $http.post('/api/Email/SendPatientRegistrationMail/?PatientId=' + patientId + '&ProviderId=' + providerId + '&OrganizationId=' + organizationId + '&PracticeId=' + practiceId + '&UserName=' + userName + "&Password=" + password + "&userId=" + userId).success(function (value) {
                    if (value != null) {
                        if (value == "1") {
                            toaster.pop('success', '', "Mail has been sent.");
                        }
                        else {
                            toaster.pop('success', '', "Mail has been sent already.");
                        }
                        window.location.href = "/patient/Index";
                    }
                });
            },
            UploadCsvFile: function (data) {
                $scope.Patient.IsLoading = true;
                var fd = new FormData();
                var xfile = null;
                for (var i = 0; i < data.length; i++) {
                    fd.append('file' + [i + 1], data[i]);
                    xfile = data[i];
                }
                $upload.upload({
                    url: '/Patient/UploadCsvFileAndInsertData',
                    file: xfile,
                    data: { "userName": $scope.Patient.Item.User.UserName, "ProviderId": $scope.Patient.Item.ProviderID, "OrganizationId": $scope.Patient.Item.OrganizationID, "PracticeId": $scope.Patient.Item.PracticeID },
                    progress: function (e) { }
                }).then(function (data, status, headers, config) {
                    var tbldata = $scope.Patient.CSVData;

                    if (data.data.status == 1) {
                        $scope.CurrentStackCSVRecords = data.data.TotalCsvRecords;
                        if (data.data.patientList.length != 0) {
                            $http.post('/api/Patient/InsertPatientData', data.data.patientList, {
                            }).success(function (data) {
                                $scope.Patient.IsLoading = false;
                                if (data.status == 1) {
                                    $scope.Patient.Items = data.dataList;
                                    var TotalCsvRecords = data.TotalCsvRecords;
                                    var TotalAddedPatient = data.TotalAddedPatient;
                                    var TotalNotAddedPatient = data.TotalNotAddedPatient;
                                    var Message = TotalAddedPatient + " of " + $scope.CurrentStackCSVRecords + " Patients uploaded successfully";
                                    if (TotalNotAddedPatient != "0") {
                                        Message += " and " + TotalNotAddedPatient + " Patients are not imported.";
                                    }
                                    toaster.pop('success', '', Message);
                                    $scope.Patient.Methods.GetProviderPatientDetailByUserName();
                                }
                                else if (data.status == 2) {
                                    toaster.pop('warning', '', "There is some issue. Please try again.");
                                }
                            });
                        }
                        else {
                            $scope.Patient.IsLoading = false;
                        }
                    }
                    else if (data.data.status == 2) {
                        if (data.data.isPatientAssociate) {
                            $scope.Patient.Methods.GetProviderPatientDetailByUserName();
                            toaster.pop('success', '', "Patient associated successfully.");
                        }
                        $scope.Patient.IsLoading = false;
                    }
                    else {
                        $scope.Patient.IsLoading = false;
                    }
                });
                return;
                fd.append('UserName', $scope.Patient.Item.User.UserName);
                fd.append('ProviderId', $scope.Patient.Item.ProviderID);
                fd.append('OrganizationId', $scope.Patient.Item.OrganizationID);
                fd.append('PracticeId', $scope.Patient.Item.PracticeID);

                $http.post('/Patient/UploadCsvFileAndInsertData', fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }

                }).success(function (data) {

                }).success(function (data) {

                    var objArray = data.split("#$%^");
                    if (objArray[0] != "") {
                        if (objArray[0] == "1") {
                            var TotalCsvRecords = objArray[1];
                            var TotalAddedPatient = objArray[2];
                            var TotalNotAddedPatient = objArray[3];
                            var Message = TotalAddedPatient + " of " + TotalCsvRecords + " Patients uploaded successfully";
                            if (TotalNotAddedPatient != "0") {
                                Message += " and " + TotalNotAddedPatient + " Patients are not uploaded.";
                            }

                            setTimeout(function () { window.location.reload() }, 2000);
                        }
                        else if (objArray[0] == "2") {
                            toaster.pop('info', '', "Data not available.");
                        }
                    }
                    else {
                        toaster.pop('warning', '', "There is some issue. Please try again.");
                    }
                }).error(function (error) {
                });
            },
            UploadCsvFileAndGetData: function (data) {
                var fd = new FormData();
                for (var i = 0; i < data.length; i++) {
                    fd.append('file' + [i + 1], data[i]);
                    fd.append('ProviderId', $scope.Patient.Item.ProviderID);
                    fd.append('OrganizationId', $scope.Patient.Item.OrganizationID);
                    fd.append('PracticeId', $scope.Patient.Item.PracticeID);
                }

                $http.post('/Patient/UploadCsvFileAndGetData', fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                }).success($scope.Patient.Methods.GetCsvFileData).error(function (error) {
                });
            },
            GetProviderPatientDetailByUserName: function (UserName, OrganizationId, PracticeId) {
                $http.get('/api/Patient/GetProviderPatientDetailByUserName?UserName=' + UserName + '&OrganizationId=' + OrganizationId + '&PracticeId=' + PracticeId).success($scope.Patient.Methods.SetPatientDetails);
            },
            GetPatientforDashboard: function (UserName, OrganizationId, PracticeId) {
                $http.get('/api/Patient/GetProviderPatientDetailByUserName?UserName=' + UserName + '&OrganizationId=' + OrganizationId + '&PracticeId=' + PracticeId).success($scope.Patient.Methods.SetDashboardPatientList);
            },
            Register: function (data) {
                $http.post('/Provider/Register/', data).success(function (value) {
                    if (value != null && value != "") {
                        $scope.Patient.Item.PatientUserId = value.UserId;
                        var m = retValueFromName($scope.Patient.month);
                        $scope.Patient.Item.User.DOB = new Date($scope.Patient.year, m - 1, $scope.Patient.day);
                        $scope.Patient.Methods.CreatePatient(data);
                    }
                });
            },
            CreatePatient: function (data) {
                $http.post('/api/Patient/CreatePatient/', data).success(function (response) {
                    if (response != "") {
                        $scope.Patient.Item.PatientID = response;
                        $scope.Patient.Methods.SendVerificationMail();
                        $scope.Patient.IsLoading = false;
                        toaster.pop('success', '', "Patient record created successfully.");
                    }
                });
            },
            States: function (data) {
                $http.get('/api/Provider/GetStates').success($scope.Patient.Methods.setStates);
            },
            Countries: function (data) {
                $http.get('/api/Provider/GetCountries').success($scope.Patient.Methods.setCountries);
            },
            GetSecretQuestion: function () {
                $http.get('/api/Provider/GetSecretQuestion').success(function (data) {
                    $scope.Patient.SecretQuestionlist = data;
                    if (data.length > 0) {
                        $scope.Patient.Item.SecretQuestionID = data[0].ID;
                    }
                });
            },
            CheckExistingUser: function (UserName) {
                $http.get('/api/Provider/CheckExistingUser?UserName=' + UserName).success(function (data) {
                    $scope.Patient.isUserExist = data;
                });
            },

            GetSurveyByPatient_Provider_Org_Practice_IDs: function (PatientId, ProviderId, OrganizationId, PracticeId) {
                $http.get('/api/eproms/GetSurveyByPatient_Provider_Org_Practice_IDs?PatientId=' + PatientId + "&ProviderId=" + ProviderId + "&OrganizationId=" + OrganizationId + "&PracticeId=" + PracticeId + "&isAllPatient=false&isCompleted=true").success($scope.Patient.Methods.SetSurveyListByPatientId);
            },
            CheckExistingMedicure: function (MedicareNumber) {
                $http.get('/api/Provider/CheckExistingMedicure?MedicareNumber=' + MedicareNumber).success(function (data) {
                    $scope.Patient.isMedicureExist = data;
                });
            },
            CheckExistingIHINumber: function (IHINumber) {
                $http.get('/api/Provider/CheckExistingIHINumber?IHINumber=' + IHINumber).success(function (isExist) {
                    $scope.Patient.isIHINumber = isExist;
                });
            },
            GetOrganizationList: function (UserName) {
                $http.get('/api/Organization/GetOrganizationByProviderId?UserName=' + UserName).success(function (data) {
                    $scope.Patient.OrganizationList = data;
                    if (window.location.pathname.indexOf("patient/create") > -1 || window.location.pathname.indexOf("Patient/Index") > -1) {
                        if (data == null || data.length == 0) {
                            toaster.pop('info', '', "First, choose organization and practice.");
                            setTimeout(function () {
                                $window.location.href = '/Provider/ProviderOrganization';
                            }, 1000);
                        }
                    }
                });
            },
            GetPracticeListByOrgID: function (UserName, OrgId) {
                $http.get('/api/Practice/GetOrganizationPracticeByProviderId?UserName=' + UserName + '&OrganizationId=' + OrgId).success(function (data) {
                    $scope.Patient.Methods.SetPractice(data);
                });
            },
            SearchPatientDetail: function (IHINumber, MedicareNumber, ProviderId, OrganizationId, PracticeId) {
                $http.get('/api/Provider/SearchPatientDetail?IHINumber=' + IHINumber + "&MedicareNumber=" + MedicareNumber + "&ProviderId=" + ProviderId + "&OrganizationId=" + OrganizationId + "&PracticeId=" + PracticeId).success($scope.Patient.Methods.SetPatientDetailsForSearch);
            },
            CreatePatientProvider: function (data) {
                $http.post('/api/Provider/CreatePatientProvider/', data).success(function (response) {
                    if (response != "") {
                        $scope.Patient.Search.IsPatientExist = true;
                        $scope.Patient.Methods.SendPatientAssociateWithProviderMail();
                    }
                    else {
                        $scope.Patient.IsLoading = false;
                    }
                });
            },
            SendPatientAssociateWithProviderMail: function (PatientId, OrganizationId, PracticeId, ProviderId, UserName) {
                $http.post('/api/Email/SendPatientAssociateToProvider/?PatientId=' + PatientId + "&OrganizationId=" + OrganizationId + "&PracticeId=" + PracticeId + "&ProviderId=" + ProviderId + "&UserName=" + UserName).success(function (value) {
                    if (value != null) {
                        $scope.Patient.IsLoading = false;
                        if (value == "1") {
                            toaster.pop('success', '', "Mail has been sent.");
                        }
                        else {
                            toaster.pop('success', '', "Mail has been sent already.");
                        }
                    }
                    else {
                        $scope.Patient.IsLoading = false;
                    }
                });
            },
        }, UI: {
        }
    }

    $scope.Patient.Methods.Initialize();

    function getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }

    function retValueFromName(name) {
        if (name == "JAN")
            return 1;
        else if (name == "FEB")
            return 2;
        else if (name == "MAR")
            return 3;
        else if (name == "APR")
            return 4;
        else if (name == "MAY")
            return 5;
        else if (name == "JUN")
            return 6;
        else if (name == "JUL")
            return 7;
        else if (name == "AUG")
            return 8;
        else if (name == "SEP")
            return 9;
        else if (name == "OCT")
            return 10;
        else if (name == "NOV")
            return 11;
        else if (name == "DEC")
            return 12;
    }

    function retNameFromValue(val) {
        if (val == 1)
            return "JAN";
        else if (val == 2)
            return "FEB";
        else if (val == 3)
            return "MAR";
        else if (val == 4)
            return "APR";
        else if (val == 5)
            return "MAY";
        else if (val == 6)
            return "JUN";
        else if (val == 7)
            return "JUL";
        else if (val == 8)
            return "AUG";
        else if (val == 9)
            return "SEP";
        else if (val == 10)
            return "OCT";
        else if (val == 11)
            return "NOV";
        else if (val == 12)
            return "DEC";
    }

}]);