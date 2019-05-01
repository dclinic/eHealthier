app.controller('PatientEpromController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$cookies', '$filter', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $cookies, $filter) {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.PatientEprom = {
        Reassign: false,
        newField: {},
        PatientDetail: {},
        PatientSurveyId: '',
        ProviderId: '',
        PracticeId: '',
        OrganizationId: '',
        TempLists: [],
        PathwayList: [],
        SurveyName: '',
        SurveyResponseMsg: '',
        isPatientId: false,
        TodayDate: new Date(),
        PatientScore: '',
        RespondentId: '',
        surveyEmail: '',
        UniqueID: '',
        QuestionResult: [],
        CollectorId: '',
        ExternalId: '',
        SurveyTitle: '',
        SurveyId: '',
        ContentCode: '',
        Item: { surveyList: [], UserName: '' },
        selectdSurveyList: [],
        Items: [],
        EpromList: [],
        CategoryList: [],
        CategoryID: 0,
        IsDateValid: true,
        IsLoading: false,
        isEpromAlreadySelected: false,
        SelectedEprom: [],
        hideMessage: false,
        list_Survey: [],
        arr_Survey: [],
        tmp_survey: [],
        patientEpromList: [],
        PatientName: '',
        UserEmail: '',
        FromDate: new Date(),
        errorEprom: 0,
        errorSD: 0,
        errorED: 0,
        IsAddEprom: false,
        IsCompleted: true,
        Methods: {
            Initialize: function () {
                if ($cookies.get("username") != undefined) {
                    $scope.PatientEprom.Item.UserName = $cookies.get("username");
                }

                if (getParameterByName("email") != null && getParameterByName("email") != "") {
                    var email = getParameterByName("email");
                    $scope.PatientEprom.surveyEmail = email;
                    $scope.PatientEprom.Item.UserName = email;
                    $window.localStorage.setItem("username", email);
                }

                var OrganizationId = $window.localStorage.getItem("OrganizationId");
                if (OrganizationId != undefined && OrganizationId != null && OrganizationId != "") {
                    $scope.PatientEprom.OrganizationId = OrganizationId;

                    var PracticeId = $window.localStorage.getItem("PracticeId");

                    if (PracticeId != undefined && PracticeId != null && PracticeId != "")
                        $scope.PatientEprom.PracticeId = PracticeId;
                }

                //Set From Date
                var d = new Date();
                d.setFullYear(d.getFullYear() - 1);
                $scope.PatientEprom.FromDate = d;

                if ($window.localStorage.getItem("patientDetails") != null && $window.localStorage.getItem("patientDetails") != "")
                    $scope.PatientEprom.PatientDetail = JSON.parse($window.localStorage.getItem("patientDetails"));

                if ($window.localStorage.getItem("ProviderID") != null && $window.localStorage.getItem("ProviderID") != "")
                    $scope.PatientEprom.ProviderId = $window.localStorage.getItem("ProviderID");

                if ($window.localStorage.getItem("EpromMsg") != undefined && $window.localStorage.getItem("EpromMsg") != null && $window.localStorage.getItem("EpromMsg") != "")
                    $scope.PatientEprom.SurveyResponseMsg = $window.localStorage.getItem("EpromMsg");

                if ($window.localStorage.getItem("userEmail") != undefined && $window.localStorage.getItem("userEmail") != null && $window.localStorage.getItem("userEmail") != "") {
                    $scope.PatientEprom.UserEmail = $window.localStorage.getItem("userEmail");
                }

                if ($window.localStorage.getItem("PatientId") != undefined && $window.localStorage.getItem("PatientId") != null && $window.localStorage.getItem("PatientId") != "") {
                    $scope.PatientEprom.Item.PatientID = $window.localStorage.getItem("PatientId");
                }

                if ($window.localStorage.getItem("PatientName") != undefined && $window.localStorage.getItem("PatientName") != null && $window.localStorage.getItem("PatientName") != "")
                    $scope.PatientEprom.PatientName = $window.localStorage.getItem("PatientName");

                if ($window.localStorage.getItem("SurveyTitle") != undefined && $window.localStorage.getItem("SurveyTitle") != null && $window.localStorage.getItem("SurveyTitle") != "")
                    $scope.PatientEprom.SurveyTitle = $window.localStorage.getItem("SurveyTitle");

                if ($window.localStorage.getItem("ExternalId") != undefined && $window.localStorage.getItem("ExternalId") != null && $window.localStorage.getItem("ExternalId") != "")
                    $scope.PatientEprom.ExternalId = $window.localStorage.getItem("ExternalId");

                if ($window.localStorage.getItem("CollectorID") != undefined && $window.localStorage.getItem("CollectorID") != null && $window.localStorage.getItem("CollectorID") != "")
                    $scope.PatientEprom.CollectorId = $window.localStorage.getItem("CollectorID");

                if ($window.localStorage.getItem("PatientSurveyID") != undefined && $window.localStorage.getItem("PatientSurveyID") != null && $window.localStorage.getItem("PatientSurveyID") != "")
                    $scope.PatientEprom.PatientSurveyId = $window.localStorage.getItem("PatientSurveyID");

                if ($scope.PatientEprom.Item.PatientID == undefined || $scope.PatientEprom.Item.PatientID == null || $scope.PatientEprom.Item.PatientID == "") {
                    $scope.PatientEprom.isPatientId = false;
                    $scope.PatientEprom.Services.GetPatientDetailsByUserName($scope.PatientEprom.Item.UserName);
                }
                else {
                    $scope.PatientEprom.isPatientId = true;
                    if (window.location.pathname.indexOf("Patient/PatientEprom") > -1) {
                        $scope.PatientEprom.Methods.GetSurveyByPatient_Provider_ID();
                    }
                    else {
                        $scope.PatientEprom.Methods.GetSurveyByPatient_Provider_Org_Practice();
                    }
                }

                $scope.PatientEprom.Methods.getSurveyCategory();

                if ($window.localStorage.getItem("SurveyID") != null && $window.localStorage.getItem("SurveyID") != "") {
                    $scope.PatientEprom.SurveyId = $window.localStorage.getItem("SurveyID");
                }

                if (window.location.pathname.indexOf("Patient/CompleteEprom") > -1) {
                    var uniqueId = '';
                    if (getParameterByName("uniqueId") != null && getParameterByName("uniqueId") != "") {
                        uniqueId = getParameterByName("uniqueId");
                        $scope.PatientEprom.UniqueID = uniqueId;
                    }

                    if (getParameterByName("email") != null && getParameterByName("email") != "") {
                        var email = getParameterByName("email");
                        $scope.PatientEprom.surveyEmail = email;
                        $scope.PatientEprom.Item.UserName = email;
                    }
                    $scope.PatientEprom.Methods.CompleteEpromSendEmail();
                }

                if (window.location.pathname.indexOf("Patient/MyEproms") > -1) {

                    if (getParameterByName("patientsurveyid") != null && getParameterByName("patientsurveyid") != "") {
                        var patientsurveyid = getParameterByName("patientsurveyid");
                        $window.localStorage.setItem("PatientSurveyID", patientsurveyid);
                        $scope.PatientEprom.PatientSurveyId = patientsurveyid;
                    }

                    if (getParameterByName("email") != null && getParameterByName("email") != "") {
                        var email = getParameterByName("email");
                        $window.localStorage.setItem("username", email);
                    }

                    if ($scope.PatientEprom.PatientSurveyId != undefined && $scope.PatientEprom.PatientSurveyId != null && $scope.PatientEprom.PatientSurveyId != "") {
                        $scope.PatientEprom.Methods.CheckTodayDate_PatientSurveyStatusExist();
                    }
                }

                if (window.location.pathname.toLowerCase().indexOf("patienteprom/index") > -1) {
                    $scope.PatientEprom.Methods.GetPathwayList();
                }
            },

            GetPathwayList: function () {
                $scope.PatientEprom.Services.GetPathwayList($scope.PatientEprom.ProviderId, $scope.PatientEprom.PracticeID, $scope.PatientEprom.OrganizationId);
            },

            UpdatePatientSurveyStatus: function (data) {
                $scope.PatientEprom.Services.UpdatePatientSurveyStatus(data);
            },

            RedirectToPatient: function () {
                if (getParameterByName("from") != null && getParameterByName("from") != "") {
                    var from = getParameterByName("from");
                    if (from == "allocate") {
                        window.location.href = "/Patient/epromAllocation";
                    }
                }
                else {
                    window.location.href = "/patient/Index";
                }
            },

            RedirectToMyEproms: function (ProviderId, PatientSurveyId, contentcode, surveyID, surveyTitle, PatientId, ExternalId, CollectorID) {
                $window.localStorage.setItem("ProviderID", ProviderId)
                $window.localStorage.setItem("PatientSurveyID", PatientSurveyId)
                $window.localStorage.setItem("ContentCode", contentcode)
                $window.localStorage.setItem("SurveyID", surveyID)
                $window.localStorage.setItem("ExternalId", ExternalId)
                $window.localStorage.setItem("CollectorID", CollectorID)
                $window.localStorage.setItem("SurveyTitle", surveyTitle)
                $window.localStorage.setItem("PatientId", PatientId)
                $scope.PatientEprom.Item.PatientID = PatientId;
                window.location.href = "/Patient/MyEproms";
            },

            RedirectToPatientEprom: function () {
                window.location.href = "/PatientEprom/Index";
            },

            AddEproms: function () {
                window.location.href = "/patientEprom/Add";
            },

            formatDate: function (date) {
                var d = new Date(date),
                    month = '' + (d.getMonth() + 1),
                    day = '' + d.getDate(),
                    year = d.getFullYear();

                if (month.length < 2) month = '0' + month;
                if (day.length < 2) day = '0' + day;

                return [month, day, year].join('/');
            },

            CreatePatientSurvey: function (item, isReassign, rowNO) {
                $scope.PatientEprom.issubmitted = true;
                if ($scope.PatientEprom.IsDateValid) {

                    if (item.SurveyID != "" && item.StartDate != "" && item.EndDate != "") {
                        $scope.PatientEprom.errorEprom = 0;
                        $scope.PatientEprom.errorSD = 0;
                        $scope.PatientEprom.errorED = 0;

                        $scope.PatientEprom.IsLoading = true;

                        $scope.PatientEprom.TempLists = angular.copy(item);
                        $scope.PatientEprom.TempLists.PatientID = $scope.PatientEprom.Item.PatientID;
                        $scope.PatientEprom.TempLists.OrganizationID = $scope.PatientEprom.OrganizationId;
                        $scope.PatientEprom.TempLists.PracticeID = $scope.PatientEprom.PracticeId;
                        $scope.PatientEprom.TempLists.ProviderID = $scope.PatientEprom.ProviderId;
                        $scope.PatientEprom.TempLists.isReassign = isReassign;

                        if ($scope.PatientEprom.TempLists.StartDate != null && $scope.PatientEprom.TempLists.StartDate != "") {
                            $scope.PatientEprom.TempLists.StartDate = $scope.PatientEprom.TempLists.StartDate.format("mm/dd/yyyy");
                        }

                        if ($scope.PatientEprom.TempLists.EndDate != null && $scope.PatientEprom.TempLists.EndDate != "") {
                            $scope.PatientEprom.TempLists.EndDate = $scope.PatientEprom.TempLists.EndDate.format("mm/dd/yyyy");
                        }

                        $scope.PatientEprom.Services.CreatePatientSurvey($scope.PatientEprom.TempLists);
                    }
                    else {
                        $scope.PatientEprom.errorEprom = rowNO;
                        $scope.PatientEprom.errorSD = rowNO;
                        $scope.PatientEprom.errorED = rowNO;
                    }
                }
                else {
                    toaster.pop('warning', '', "End Date should be greater than the Start Date");
                }
            },

            GenerateEpromRow: function (row) {
                $scope.PatientEprom.issubmitted = true;
                if ($scope.frmPatientsEprom != undefined) {
                    if ($scope.frmPatientsEprom.$valid) {
                        $scope.PatientEprom.issubmitted = false;
                        var newRow = {
                            SurveyID: '',
                            StartDate: '',
                            EndDate: '',
                            IsActive: false,
                            isnewRow: true,
                            PatientSurvey_Pathway_PatientSurveyStatus_ID: '',
                            PathwayID: ''
                        };
                        $scope.PatientEprom.Item.surveyList.push(newRow);
                    }
                }
            },

            DeleteEpromRow: function (item, rowNo, Id) {
                if (Id != null && Id != undefined) {
                    $scope.PatientEprom.list_Survey.push(item);
                    $scope.PatientEprom.Services.DeletePatientSurvey(rowNo, Id)
                }
                else {
                    $scope.PatientEprom.Item.surveyList.splice(rowNo, 1);
                }
            },

            CreatePatientSurveyList: function () {
                var ids = '';

                for (var i = 0; i < $scope.PatientEprom.list_Survey.length; i++) {
                    if (i == 0) {
                        ids = $scope.PatientEprom.list_Survey[i].SurveyID;
                    }
                    else {
                        ids = ids + "," + $scope.PatientEprom.list_Survey[i].SurveyID;
                    }

                    $scope.PatientEprom.SelectedEprom.push($scope.PatientEprom.list_Survey[i]);
                }

                $window.localStorage.setItem("surveyList", JSON.stringify($scope.PatientEprom.SelectedEprom));

                var organizationID = $scope.PatientEprom.OrganizationId;
                var practiceID = $scope.PatientEprom.PracticeId;
                var providerID = $scope.PatientEprom.ProviderId;
                var patientID = $scope.PatientEprom.Item.PatientID;

                $scope.PatientEprom.Services.CreatePatientSurveyList(organizationID, practiceID, providerID, patientID, ids);

                window.location.href = "/PatientEprom/Index";
            },

            getSurveyCategory: function () {
                $scope.PatientEprom.Services.getSurveyCategory();
            },

            SetSurveyCategory: function (data) {
                $scope.PatientEprom.Items.CategoryList = JSON.parse(data);

                $scope.PatientEprom.CategoryID = $scope.PatientEprom.Items.CategoryList[0].ID;
                $scope.PatientEprom.Methods.getEpromListByCategoryId($scope.PatientEprom.CategoryID, 0);
            },

            getEpromListByCategoryId: function (id, subid) {
                $scope.PatientEprom.Services.getEpromListByCategoryId(id, subid);
            },

            SetEpromListByCategoryId: function (data) {
                $scope.PatientEprom.Items.EpromList = JSON.parse(data);
            },

            GetSurveyByPatient_Provider_Org_Practice: function () {
                $scope.PatientEprom.Services.GetSurveyByPatient_Provider_Org_Practice($scope.PatientEprom.Item.PatientID, $scope.PatientEprom.ProviderId, $scope.PatientEprom.OrganizationId, $scope.PatientEprom.PracticeId);
            },

            GetSurveyByPatient_Provider_ID: function () {
                if (window.location.pathname.indexOf("Patient/PatientEprom") > -1) {
                    $scope.PatientEprom.IsLoading = true;
                }

                $scope.PatientEprom.Services.GetSurveyByPatient_Provider_ID($scope.PatientEprom.Item.PatientID, $scope.PatientEprom.ProviderId);
            },

            SetSurveyListByPatientId: function (data) {
                $scope.PatientEprom.Item.surveyList = [];
                if (window.location.pathname.indexOf("Patient/PatientEprom") > -1) {
                    $scope.PatientEprom.IsLoading = false;
                }

                $scope.PatientEprom.list_Survey = JSON.parse(data);
                $scope.PatientEprom.arr_Survey = JSON.parse(data);

                if ($cookies.get("tmpData") == "T") {
                    $cookies.put("tmpData", "F");
                    $window.localStorage.setItem("arrList", []);
                }
                else {
                    var tmpData = $window.localStorage.getItem("arrList");

                    if (tmpData != null && tmpData != "") {
                        var surveyData = JSON.parse(tmpData);

                        if (surveyData != null) {
                            for (var j = 0; j < surveyData.length; j++) {
                                var newData = {
                                    SurveyID: surveyData[j].SurveyID,
                                    ExternalTitle: surveyData[j].ExternalTitle
                                };

                                $scope.PatientEprom.arr_Survey.push(newData);
                            }
                        }
                    }
                }

                for (var i = 0; i < $scope.PatientEprom.arr_Survey.length; i++) {
                    if ($scope.PatientEprom.arr_Survey[i].IsSend == true) {
                        $scope.PatientEprom.patientEpromList.push($scope.PatientEprom.arr_Survey[i]);
                        $scope.PatientEprom.Item.surveyList.push($scope.PatientEprom.arr_Survey[i]);
                    }
                }

                for (var i = 0; i < $scope.PatientEprom.Item.surveyList.length; i++) {
                    $scope.PatientEprom.Item.surveyList[i].StartDate = new Date($scope.PatientEprom.Item.surveyList[i].StartDate);
                    $scope.PatientEprom.Item.surveyList[i].EndDate = new Date($scope.PatientEprom.Item.surveyList[i].EndDate);

                    for (var j = 0; j < $scope.PatientEprom.arr_Survey.length; j++) {
                        if ($scope.PatientEprom.Item.surveyList[i].SurveyID == $scope.PatientEprom.arr_Survey[j].SurveyID) {
                            $scope.PatientEprom.arr_Survey.splice(j, 1);
                        }
                    }
                }

                if (window.location.pathname.toLowerCase().indexOf("patienteprom/index") > -1) {
                    $scope.PatientEprom.IsLoading = false;
                }

                if (window.location.pathname.toLowerCase().indexOf("patient/patienteprom") > -1) {
                    $scope.PatientEprom.IsLoading = false;

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
                }

                for (var i = 0; i < $scope.PatientEprom.arr_Survey.length; i++) {
                    var date = new Date();
                    var newdate = new Date(date);
                    newdate.setDate(newdate.getDate() + Number(3));
                    var res = new Date(newdate);

                    var newRow = {
                        SurveyID: $scope.PatientEprom.arr_Survey[i].SurveyID,
                        StartDate: new Date(),
                        EndDate: res,
                        IsActive: true,
                        isnewRow: true,
                        PatientSurvey_Pathway_PatientSurveyStatus_ID: '',
                        PathwayID: ''
                    };

                    $scope.PatientEprom.Item.surveyList.push(newRow);
                }
            },

            AssignNewePROM: function () {
                if ($scope.PatientEprom.IsAddEprom) {
                    $scope.PatientEprom.IsAddEprom = false;
                }
                else {
                    $scope.PatientEprom.IsAddEprom = true;
                }
            },

            CheckDateValidationSD: function (startdate, enddate, index) {
                var newdate = new Date(startdate);
                newdate.setDate(newdate.getDate() + Number(3));
                var res = new Date(newdate);

                $scope.PatientEprom.Item.surveyList[index].EndDate = new Date(newdate);
            },

            CheckDateValidation: function (startdate, enddate) {
                $scope.errMessage = '';

                var curDate = new Date();
                if (enddate != null && enddate != undefined && enddate != '') {
                    if (new Date(startdate) > new Date(enddate)) {
                        toaster.pop('warning', '', "End Date should be greater than the Start Date");
                        $scope.PatientEprom.IsDateValid = false;
                        return false;
                    }
                    else {
                        $scope.PatientEprom.IsDateValid = true;
                    }
                }
            },

            CheckEpromSelectedOrNot: function (item) {
                var index = $scope.PatientEprom.Item.surveyList.indexOf(item)
                if (index > -1) {
                    $scope.PatientEprom.isEpromAlreadySelected = true;
                    toaster.pop('warning', 'Warning', "Eprom is already selected.");
                    return false;
                }
                else {
                    $scope.PatientEprom.isEpromAlreadySelected = false;
                }
            },

            Toggle: function (item, list) {
                var isSplice = false;

                for (var a = 0; a < $scope.PatientEprom.arr_Survey.length; a++) {
                    if ($scope.PatientEprom.arr_Survey[a].SurveyID == item.SurveyID) {
                        $scope.PatientEprom.Item.surveyList.splice(a + 1, 1);
                        $scope.PatientEprom.arr_Survey.splice(a, 1);

                        $window.localStorage.setItem("arrList", JSON.stringify($scope.PatientEprom.arr_Survey));

                        if (item.SurveyID != undefined) {
                            if (list != undefined) {
                                if (list.length > 0) {
                                    for (var i = 0; i < list.length; i++) {
                                        if (item.SurveyID == list[i].SurveyID) {
                                            list.splice(i, 1);
                                            isSplice = true;
                                        }
                                    }
                                    if (isSplice == false)
                                        list.push(item);

                                    return false;
                                }
                                else {
                                    list.push(item);
                                }
                            }
                            else {
                                list = [];
                                list.push(item);
                            }
                        }
                    }
                }

                var newRow = {
                    SurveyID: item.SurveyID,
                    ExternalTitle: item.ExternalTitle
                };

                $scope.PatientEprom.arr_Survey.push(newRow);

                $window.localStorage.setItem("arrList", JSON.stringify($scope.PatientEprom.arr_Survey));

                var date = new Date();
                var newdate = new Date(date);
                newdate.setDate(newdate.getDate() + Number(3));
                var res = new Date(newdate);

                var newRow = {
                    SurveyID: item.SurveyID,
                    StartDate: new Date(),
                    EndDate: res,
                    IsActive: true,
                    isnewRow: true,
                    PatientSurvey_Pathway_PatientSurveyStatus_ID: '',
                    PathwayID: ''
                };

                $scope.PatientEprom.Item.surveyList.push(newRow);

                if (item.SurveyID != undefined) {
                    if (list != undefined) {
                        if (list.length > 0) {
                            for (var i = 0; i < list.length; i++) {
                                if (item.SurveyID == list[i].SurveyID) {
                                    list.splice(i, 1);
                                    isSplice = true;
                                }
                            }
                            if (isSplice == false)
                                list.push(item);

                            return false;
                        }
                        else {
                            list.push(item);
                        }
                    }
                    else {
                        list = [];
                        list.push(item);
                    }
                }
            },

            Exists: function (item, list) {
                if (item.SurveyID != undefined) {
                    if (list != undefined) {
                        for (var i = 0; i < list.length; i++) {
                            if (item.SurveyID == list[i].SurveyID) {
                                return true;
                            }
                        }
                    }
                    if ($scope.PatientEprom.arr_Survey != undefined) {
                        for (var i = 0; i < $scope.PatientEprom.arr_Survey.length; i++) {
                            if (item.SurveyID == $scope.PatientEprom.arr_Survey[i].SurveyID) {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            },

            SetPatientDetails: function (data) {
                if (data != undefined && data != null) {
                    $scope.PatientEprom.Item.PatientID = data.ID;
                    $window.localStorage.setItem("PatientId", data.ID);

                    if (window.location.pathname.indexOf("Patient/PatientEprom") > -1) {
                        $scope.PatientEprom.Methods.GetSurveyByPatient_Provider_ID();
                    }
                    else {
                        $scope.PatientEprom.Methods.GetSurveyByPatient_Provider_Org_Practice();
                    }
                    if ($scope.PatientEprom.isPatientId == false) {
                        $scope.PatientEprom.isPatientId == true
                        if (window.location.pathname.indexOf("Patient/MyEproms") > -1) {
                            $scope.PatientEprom.Methods.CheckTodayDate_PatientSurveyStatusExist();
                        }
                    }
                }
            },

            CompleteEpromSendEmail: function () {
                $scope.PatientEprom.IsLoading = true;
                $scope.PatientEprom.Methods.GetSurveyMonkeyAnalyzeResult();
            },

            GetSurveyMonkeyAnalyzeResult: function () {
                $scope.PatientEprom.Services.GetSurveyMonkeyAnalyzeResult($scope.PatientEprom.ExternalId);
            },

            SetSurveyMonkeyResponseResult: function (response) {
                var isCorrectPatient = false;
                $scope.PatientEprom.AnswerResult = JSON.parse(response);
                var questionList = $scope.PatientEprom.QuestionResult;
                var ansList = {};
                var List = '';
                var response = [];

                for (var i = 0; i < $scope.PatientEprom.AnswerResult.data.length; i++) {
                    List = $scope.PatientEprom.AnswerResult.data[i];
                    if (List.custom_variables.email == $scope.PatientEprom.surveyEmail && List.custom_variables.uniqueId == $scope.PatientEprom.UniqueID) {
                        isCorrectPatient = true;
                        response = List;

                        var url = List.analyze_url;
                        ansList = List;
                        $scope.PatientEprom.RespondentId = url.substr(url.indexOf("=") + 1);
                    }
                }

                var result = [];
                if (isCorrectPatient) {
                    if (questionList != undefined) {
                        if (questionList.pages != undefined) {
                            //Change Start
                            if (questionList.title.toLowerCase().indexOf("preventive") > -1) {
                                var inc = 0;

                                for (var i = 0; i < questionList.pages.length; i++) {
                                    var qlist = questionList.pages[i];

                                    for (var j = 0; j < qlist.questions.length; j++) {
                                        var id1 = qlist.id;
                                        var qlist1 = qlist.questions[j];

                                        for (var h = 0; h < ansList.pages.length; h++) {
                                            var alist1 = ansList.pages[h];

                                            for (var d = 0; d < alist1.questions.length; d++) {
                                                var id2 = alist1.id;
                                                var alist2 = alist1.questions[d];

                                                if (id1 == id2) {
                                                    if (alist2.answers[0].text != undefined) {
                                                        inc = inc + 1;

                                                        if (inc == 1 || inc == 4) {
                                                            result.push({
                                                                question_title: qlist1.headings[0].heading,
                                                                answer_text: alist2.answers[0].text
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (questionList.title.toLowerCase().indexOf("copd") > -1) {
                                for (var i = 0; i < ansList.pages.length; i++) {
                                    var alist1 = ansList.pages[i];

                                    for (var j = 0; j < alist1.questions.length; j++) {
                                        var alist2 = alist1.questions[j];

                                        if (alist2.answers[0].text != undefined) {
                                            result.push({
                                                question_title: (j + 1),
                                                answer_text: alist2.answers[0].text
                                            });
                                        }
                                    }
                                }
                            }
                            //Change End

                            for (var h = 0; h < questionList.pages.length; h++) {
                                item = {};
                                var qlist = questionList.pages[h];

                                var questions = [];
                                for (var i = 0; i < qlist.questions.length; i++) {
                                    var qlist1 = qlist.questions[i];
                                    var qitem = {};

                                    if (qlist1.answers != undefined) {
                                        if (qlist1.answers.choices != undefined) {
                                            for (var a = 0; a < qlist1.answers.choices.length; a++) {
                                                var alist = qlist1.answers.choices[a];
                                                var alist1 = ansList.pages[h];

                                                for (var d = 0; d < alist1.questions.length; d++) {
                                                    var alist2 = alist1.questions[d];

                                                    if (alist2.answers != undefined) {
                                                        for (var e = 0; e < alist2.answers.length; e++) {
                                                            var alist3 = alist2.answers[e];

                                                            if (alist3.choice_id != undefined) {
                                                                if (alist.id == alist3.choice_id) {

                                                                    if (qlist1.answers.rows != undefined && qlist1.answers.choices != undefined) {
                                                                        for (var z = 0; z < qlist1.answers.rows.length; z++) {
                                                                            var alistRow = qlist1.answers.rows[z];
                                                                            if (alistRow.id == alist3.row_id) {
                                                                                result.push({
                                                                                    question_title: alistRow.text,
                                                                                    answer_text: alist.text
                                                                                });
                                                                            }
                                                                        }
                                                                    }
                                                                    else {
                                                                        result.push({
                                                                            question_title: qlist1.headings[0].heading,
                                                                            answer_text: alist.text
                                                                        });
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                $scope.PatientEprom.AnalyzeResult = result;
                var EpromList = { responselist: result, Eprom_title: questionList.title };
                $scope.PatientEprom.SurveyName = questionList.title;
                $scope.PatientEprom.Services.GetPatientScore(EpromList)
            },

            CreatePatientSurveyStatus: function () {
                var statusItem = { PatientSurveyID: '', Email: '' };
                statusItem.PatientSurveyID = $scope.PatientEprom.PatientSurveyId;
                statusItem.Email = $scope.PatientEprom.Item.UserName;

                $scope.PatientEprom.Services.CreatePatientSurveyStatus(statusItem);
            },

            CheckTodayDate_PatientSurveyStatusExist: function () {
                $scope.PatientEprom.Services.CheckTodayDate_PatientSurveyStatusExist($scope.PatientEprom.PatientSurveyId)
            },

            BindChart: function (heading, ScoreList, table) {
                var xAxis = [];
                var Data = [];
                var array = [];
                var NormValue = 0;

                if (ScoreList != undefined) {
                    var islegendEnable = false;
                    var data1 = [];
                    for (var i = 0; i < ScoreList.length; i++) {
                        var date = $filter('date')(ScoreList[i].CreatedDate, "MM/dd/yyyy");
                        var name = '';
                        var data3 = [];
                        if (xAxis.indexOf(date) == -1) {
                            xAxis.push(date);

                            var score = JSON.parse(ScoreList[i].Score);

                            for (var j = 0; j < score.length; j++) {
                                if (j > 0) {
                                    islegendEnable = true;
                                }
                                name = score[j].Title;
                                if (score.length == 1) {
                                    data1.push(score[j].Value);
                                }
                                else if (score.length > 1) {
                                    var data2 = [];
                                    if (j == 0 || j == 1) {
                                        name = score[j].Title;
                                        if (Data.length > 1) {
                                            for (var x = 0; x < Data.length; x++) {
                                                if (Data[x].name == name) {
                                                    Data[x].data.push(score[j].Value);
                                                }
                                            }
                                        } else {
                                            data2.push(score[j].Value);
                                            Data.push({
                                                name: name,
                                                data: data2
                                            });
                                        }
                                    }
                                }
                            }
                            if (score.length == 1)
                                Data.push({
                                    name: name,
                                    data: data1
                                });
                        }
                    }
                    NormValue = ScoreList[0].NormValue;
                }

                $scope.PatientEprom.Methods.BarChart("score-chart", xAxis, heading, Data, islegendEnable, table, NormValue);
            },

            BarChart: function (ChartId, xAxis, heading, Data, legendEnable, table, NormValue) {
                $('#' + ChartId).highcharts({
                    title: {
                        text: heading,
                        x: -20
                    },
                    xAxis: {
                        categories: xAxis
                    },
                    yAxis: {
                        min: 0,
                        max: 100,
                        tickInterval: 10,
                        title: {
                            text: 'Score'
                        },
                        plotLines: [{
                            value: NormValue,
                            width: 2,
                            color: '#FFA500',
                            zIndex: 4,
                            label: {
                                text: '"Norm" - ' + NormValue
                            }
                        }]
                    },
                    tooltip: {

                    },
                    legend: {
                        enabled: legendEnable,
                    },
                    series: Data
                });

                chart = $('#' + ChartId).highcharts();

                try {
                    var options = {
                        chart: {
                            renderTo: ChartId,
                            type: 'column'
                        },
                        title: {
                            text: heading,
                            x: -20
                        },
                        xAxis: {
                            categories: xAxis
                        },
                        yAxis: {
                            min: 0,
                            max: 100,
                            tickInterval: 10,
                            title: {
                                text: 'Score'
                            },
                            plotLines: [{
                                value: NormValue,
                                width: 2,
                                color: '#FFA500',
                                zIndex: 4,
                                label: {
                                    text: '"Norm" - ' + NormValue
                                }
                            }]
                        },
                        legend: {
                            enabled: legendEnable,
                        },
                        series: Data
                    };

                    var chartimg = $('#score-chart').highcharts()

                    var exportUrl = 'http://export.highcharts.com/';
                    var object = {
                        options: JSON.stringify(options),
                        type: 'image/png',
                        async: true,
                    };

                    var imageURL = '';
                    $.ajax({
                        type: 'POST',
                        data: object,
                        url: exportUrl,
                        async: false,
                        success: function (svgImage) {
                            $scope.PatientEprom.Methods.SendEpromsCompleteMailToPatient(svgImage, heading, table);
                        },
                        error: function (err) {
                            $scope.PatientEprom.IsLoading = false;
                        }
                    });
                } catch (e) {
                    $scope.PatientEprom.IsLoading = false;
                }
            },

            GetPatientSurveyStatusData: function () {
                var date = $filter('date')($scope.PatientEprom.FromDate, "MM/dd/yyyy");
                $scope.PatientEprom.Services.GetPatientSurveyStatusData($scope.PatientEprom.Item.PatientID, $scope.PatientEprom.OrganizationId, $scope.PatientEprom.PracticeId, $scope.PatientEprom.ProviderId, $scope.PatientEprom.SurveyId, date);
            },

            SetPatientSurveyStatusData: function (data) {
                if (data != undefined && data != null && data.length > 0) {
                    var table = '';

                    if ($scope.PatientEprom.SurveyName == "Preventive ePROMs for Population Health Management by GPs ™") {
                        table = "<table border='1'><thead><tr><th></th><th>Result</th><th>Risk Factor</th><th>No Risk Factor</th></tr></thead><tbody>"

                        if (data != undefined) {
                            for (var i = 0; i < data.length; i++) {
                                var score = JSON.parse(data[i].Score);

                                for (var j = 0; j < score.length; j++) {
                                    var splitScore = score[j].Value.split('_');

                                    table += "<tr><td><b>" + score[j].Title + "</b></td><td>" + splitScore[0] + "</td>";
                                    if (splitScore[1] == "risk factor") {
                                        table += "<td style='text-align:center'>&#10004;</td><td></td>";
                                    }
                                    else {
                                        table += "<td></td><td style='text-align:center'>&#10004;</td>";
                                    }

                                    table += "</tr>";
                                }
                            }
                        }

                        table += "</tbody></table>";
                    }

                    $scope.PatientEprom.Methods.BindChart($scope.PatientEprom.SurveyName, data, table);
                }
                else {
                    $scope.PatientEprom.IsLoading = false;
                }
            },

            SendEpromsCompleteMailToPatient: function (svgImage, heading, table) {
                var ScoreList = JSON.parse($scope.PatientEprom.PatientScore);
                var EpromScore = "";

                if (ScoreList != null && ScoreList.length > 0) {
                    if (heading == "Preventive ePROMs for Population Health Management by GPs ™") {
                        var isRisk = false;
                        for (var i = 0; i < ScoreList.length; i++) {
                            var splitSL = ScoreList[i].Value.split('_');

                            if (splitSL[1] == "risk factor") {
                                isRisk = true;
                                EpromScore = "Your current preventive for population health management is risk factor exist";
                            }
                            else {
                                if (!isRisk) {
                                    EpromScore = "Your current preventive for population health management is no risk factor";
                                }
                            }
                        }
                    }
                    else {
                        if (ScoreList.length == 1) {
                            EpromScore = "Your current " + ScoreList[0].Title + " is " + ScoreList[0].Value;
                        }
                        else {
                            EpromScore += "Your current score :<table border='1' cellpadding='5' style='border-collapse: collapse; margin-top:20px; width:50%;'><tr><th> Title </th><th> Score </th></tr>";

                            for (var i = 0; i < ScoreList.length; i++) {
                                EpromScore += "<tr><td>" + ScoreList[i].Title + "</td><td>" + ScoreList[i].Value + "</td></tr>";
                            }
                            EpromScore += "</table>";
                        }
                    }
                }

                $scope.PatientEprom.Services.SendEpromsCompleteMailToPatient($scope.PatientEprom.Item.PatientID, $scope.PatientEprom.PatientSurveyId, $scope.PatientEprom.Item.UserName, $scope.PatientEprom.SurveyName, EpromScore, svgImage, table);
            },

            GetProviderIDByUserName: function () {
                $scope.PatientEprom.Services.GetProviderIDByUserName($scope.PatientEprom.Item.UserName);
            },

            SetProviderIDByUserName: function (providerid) {
                $scope.PatientEprom.ProviderId = providerid;
            },

            ReassignEprom: function (item) {
                $scope.PatientEprom.Reassign = $scope.PatientEprom.Item.surveyList.indexOf(item);
                $scope.PatientEprom.newField = angular.copy(item);
            },

            CancelReassign: function (index) {
                if ($scope.PatientEprom.Reassign !== false) {
                    $scope.PatientEprom.Item.surveyList[index] = $scope.PatientEprom.newField;
                    $scope.PatientEprom.Reassign = false;

                }
            },
        },
        Services: {
            GetPathwayList: function (ProviderId, PracticeID, OrganizationId) {
                $http.get('/api/eproms/GetPathwayList?ProviderId=' + ProviderId + '&PracticeID= ' + PracticeID + '&OrganizationId=' + OrganizationId).success(function (data) {
                    $scope.PatientEprom.PathwayList = data;
                    if (data != null) {
                        $scope.PatientEprom.IsLoading = false;

                    }
                    else
                        $scope.PatientEprom.IsLoading = false;
                });
            },

            CreatePatientSurvey: function (data) {
                $http.post('/api/eproms/AddPatientSurvey/', data).success(function (response) {
                    if (response != "") {
                        $scope.PatientEprom.PatientSurveyId = response;
                        $scope.PatientEprom.Methods.GetSurveyByPatient_Provider_Org_Practice();
                        $scope.PatientEprom.IsLoading = false;
                        toaster.pop('success', '', "eMail containing ePROM link has been sent.");
                        $scope.PatientEprom.issubmitted = false;
                    }
                    else {
                        $scope.PatientEprom.IsLoading = false;
                    }
                });
            },

            DeletePatientSurvey: function (rowNo, Id) {
                $http.delete('/api/eproms/DeletePatientSurvey/' + Id).success(function (response) {
                    $scope.PatientEprom.IsLoading = false;
                    if (response == "1") {
                        $scope.PatientEprom.Item.surveyList.splice(rowNo, 1);
                        toaster.pop('success', '', "Eprom deleted successfully!");
                    }
                    else if (response == "0") {
                        toaster.pop('info', '', "Eprom already in use!");
                    }
                });
            },

            CreatePatientSurveyList: function (organizationID, practiceID, providerID, patientID, surveyID) {
                $http.post('/api/Eproms/AddPatientSurveyEProms?organizationID=' + organizationID + '&practiceID=' + practiceID + '&providerID=' + providerID + '&patientID=' + patientID + '&surveyID=' + surveyID).success(function (response) { });
            },

            GetPatientDetailsByUserName: function (data) {
                $http.get('/api/Patient/GetPatientDetailsByUserName/?UserName=' + data).success($scope.PatientEprom.Methods.SetPatientDetails);
            },

            getSurveyCategory: function () {
                $http.get('/api/category/Get').success($scope.PatientEprom.Methods.SetSurveyCategory);
            },

            getEpromListByCategoryId: function (id, subId) {
                $http.get('/api/eproms/GetSurevy_By_SurveyCategoryID?id=' + id + '&subId=' + subId).success($scope.PatientEprom.Methods.SetEpromListByCategoryId);
            },

            GetSurveyByPatient_Provider_Org_Practice: function (PatientId, ProviderId, OrganizationId, PracticeId) {
                if (window.location.pathname.toLowerCase().indexOf("patienteprom/index") > -1) {
                    $scope.PatientEprom.IsLoading = true;
                }
                $http.get('/api/eproms/GetSurveyByPatient_Provider_Org_Practice_IDs?PatientId=' + PatientId + "&ProviderId=" + ProviderId + "&OrganizationId=" + OrganizationId + "&PracticeId=" + PracticeId + "&isAllPatient=false&isCompleted=false").success($scope.PatientEprom.Methods.SetSurveyListByPatientId);
            },

            GetSurveyByPatient_Provider_ID: function (PatientId, ProviderId) {
                $http.get('/api/eproms/GetSurveyByPatient_Provider_ID?PatientId=' + PatientId + "&ProviderId=" + undefined + "&isCompleted=false").success($scope.PatientEprom.Methods.SetSurveyListByPatientId);
            },

            SendEpromsCompleteMailToPatient: function (PatientId, PatientSurveyId, UserName, ePromTitle, scoreRow, svgimage, table) {
                if (ePromTitle == "Preventive ePROMs for Population Health Management by GPs ™") {
                    ePromTitle = "Preventive ePROMs for Population Health Management by GPs";
                }

                console.log("CONSOLE DATA: " + ePromTitle);

                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/email/SendEpromsCompleteMailToPatient',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'PatientId': PatientId,
                        'PatientSurveyId': PatientSurveyId,
                        'UserName': UserName,
                        'ePromTitle': ePromTitle,
                        'ScoreRow': scoreRow,
                        'Imagefile': svgimage,
                        'TableData': table
                    },
                }).success(function (response) {
                    if (response != "") {
                        $scope.PatientEprom.issubmitted = false;
                    }

                    $scope.PatientEprom.IsLoading = false;
                });
            },

            GetSurveyMonkeyAnalyzeResult: function (surveyId) {
                $http.get('/api/eproms/GetSurveyMonkey_SurveyDetails?surveyId=' + surveyId).success(function (data) {
                    $scope.PatientEprom.QuestionResult = JSON.parse(data);
                    $scope.PatientEprom.Services.GetSurveyMonkeyResponseByCollectorID($scope.PatientEprom.CollectorId);
                });
            },

            GetSurveyMonkeyResponseByCollectorID: function (collectorId) {
                $http.get('/api/eproms/GetSurveyMonkeyResponseBy_CollectorID?collectorId=' + collectorId).success($scope.PatientEprom.Methods.SetSurveyMonkeyResponseResult);
            },

            CreatePatientSurveyStatus: function (data) {
                $http.get('/api/patient/GetPatientSurveyByID?PatientSurveyId=' + data.PatientSurveyID).success(function (response) {
                    if (response != null) {
                        $scope.PatientEprom.ContentCode = response.ContentCode;
                        $window.localStorage.setItem("ExternalId", response.ExternalID);
                        $window.localStorage.setItem("CollectorID", response.CollectorID);
                        $window.localStorage.setItem("SurveyTitle", response.ExternalTitle);
                        $window.localStorage.setItem("userEmail", response.Email);
                        $window.localStorage.setItem("SurveyID", response.SurveyID);
                        $window.localStorage.setItem("OrganizationId", response.OrganizationID);
                        $window.localStorage.setItem("PracticeId", response.PracticeID);
                        $window.localStorage.setItem("ProviderID", response.ProviderID);
                        $window.localStorage.setItem("PatientId", response.PatientID);

                        if (response.IsSurveyWithValidDate) {
                            $http.post('/api/patient/CreatePatientSurveyStatus/', data).success(function (data) {
                                $scope.PatientEprom.UniqueID = data;
                                if ($scope.PatientEprom.ContentCode != null && $scope.PatientEprom.ContentCode != "") {
                                    $("#content-object").attr('src', $scope.PatientEprom.ContentCode + "?email=" + response.Email + "&uniqueId=" + $scope.PatientEprom.UniqueID);
                                }
                            });
                        }
                        else {
                            $window.localStorage.setItem("PSID", data.PatientSurveyID);
                            $window.location.href = "/Patient/InvalidDate?StartDate=" + new Date(response.StartDate).format("dd/mm/yyyy") + "&EndDate=" + new Date(response.EndDate).format("dd/mm/yyyy");
                        }
                    }
                    else {
                        $window.location.href = "/Patient/ResponseSubmitted";
                        $window.localStorage.setItem("EpromMsg", "ePROM is Inactive. Please contact to your Provider to Active ePROM");
                    }
                });
            },

            GetPatientScore: function (data) {
                $http.post("/Eproms/PatientScore/dataList", data).success(function (response) {
                    if (response != null && response != "") {
                        $scope.PatientEprom.PatientScore = angular.toJson(response);
                        var item = { ID: $scope.PatientEprom.UniqueID, Score: $scope.PatientEprom.PatientScore, Status: "Completed" };
                        $scope.PatientEprom.Methods.UpdatePatientSurveyStatus(item);
                    }
                    else {
                        $scope.PatientEprom.IsCompleted = false;
                        $scope.PatientEprom.IsLoading = false;
                    }

                }).error(function (error) {
                    $scope.PatientEprom.IsCompleted = false;
                    $scope.PatientEprom.IsLoading = false;
                });
            },

            UpdatePatientSurveyStatus: function (objstatus) {
                $http.post("/api/Patient/UpdatePatientSurveyStatus/", objstatus).success($scope.PatientEprom.Methods.GetPatientSurveyStatusData);
            },

            CheckTodayDate_PatientSurveyStatusExist: function (PatientSurveyId, PatientId, SurveyId) {
                $http.get("/api/Patient/CheckTodayDate_PatientSurveyStatusExist?PatientSurveyID=" + PatientSurveyId).success(function (response) {
                    $window.localStorage.setItem("EpromMsg", "");
                    if (response == "") {
                        $scope.PatientEprom.Methods.CreatePatientSurveyStatus();
                    }
                    else {
                        // 1 response perday
                        $window.location.href = "/Patient/ResponseSubmitted";
                        $window.localStorage.setItem("EpromMsg", response);
                    }
                });
            },

            GetPatientSurveyStatusData: function (PatientId, OrganizationId, PracticeId, ProviderId, SurveyId, FromDate) {
                $http.get('/api/Patient/GetPatientSurveyStatusData/?PatientID=' + PatientId + '&OrganizationId=' + OrganizationId + '&PracticeId=' + PracticeId + '&ProviderId=' + ProviderId + '&SurveyId=' + SurveyId + '&FromDate=' + FromDate + '&forEmail=' + false).success($scope.PatientEprom.Methods.SetPatientSurveyStatusData);
            },

            GetProviderIDByUserName: function (UserName) {
                $http.get('/api/Provider/GetProviderIDByUserName?UserName=' + UserName).success($scope.PatientEprom.Methods.SetProviderIDByUserName);
            },
        },
        UI: {
        }
    }

    $scope.PatientEprom.Methods.Initialize();

}]);