app.controller('DashboardController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$cookies', '$timeout', '$element', '$filter', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $cookies, $timeout, $element, $filter) {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }
    $element.find('input').on('keydown', function (ev) {
        ev.stopPropagation();
    });

    $scope.Dashboard = {
        NormValue: null,
        AppList: [],
        ThirdPartyApp: [],
        AppID: '',
        SearchApp: '',
        ThirdPartyAppList: [],
        PracticeId: '',
        OrganizationId: '',
        CurrentScore: '',
        PreviousScore: '',
        ScoreList: [],
        SurveyId: '',
        CollectorId: '',
        EpromsId: '',
        ExternalId: '',
        ExternalTitle: '',
        SurveyFileName: '',
        SuggestionList: { PatientID: '', ProviderID: '', Suggestions: '' },
        Patients: [],
        SurveyStatusData: [],
        PatientIndicatorList: [],
        CollectorId: '',
        AnalyzeResult: [],
        QuestionResult: [],
        AnswerResult: [],
        Item: { User: {}, PatientID: '', ProviderID: '' },
        IsLoading: false,
        SurveyName: "",
        selected: [],
        Questions: [],
        PathwayList: [],
        isFirst: true,
        isSecond: false,
        Methods: {
            Initialize: function () {
                var OrganizationId = $window.localStorage.getItem("OrganizationId");

                if (OrganizationId != undefined && OrganizationId != null && OrganizationId != "") {
                    $scope.Dashboard.OrganizationId = OrganizationId;

                    var PracticeId = $window.localStorage.getItem("PracticeId");

                    if (PracticeId != undefined && PracticeId != null && PracticeId != "")
                        $scope.Dashboard.PracticeId = PracticeId;
                }

                if ($cookies.get("username") != undefined && $cookies.get("username") != null && $cookies.get("username") != "") {
                    $scope.Dashboard.Item.User.UserName = $cookies.get("username");
                    $scope.Dashboard.Methods.GetProviderPatientDetailByUserName();
                }
                else if ($window.localStorage.getItem("username") != null && $window.localStorage.getItem("username") != "") {
                    $scope.Dashboard.Item.User.UserName = $window.localStorage.getItem("username");
                }

                if ($window.localStorage.getItem("ProviderID") != null && $window.localStorage.getItem("ProviderID") != "")
                    $scope.Dashboard.Item.ProviderID = $window.localStorage.getItem("ProviderID");

                if ($window.localStorage.getItem("PatientEmail") != null && $window.localStorage.getItem("PatientEmail") != "")
                    $scope.Dashboard.PatientEmail = $window.localStorage.getItem("PatientEmail");


                if ($window.localStorage.getItem("PatientId") != null && $window.localStorage.getItem("PatientId") != "") {
                    $scope.Dashboard.Item.PatientID = $window.localStorage.getItem("PatientId");
                }

                if ($window.localStorage.getItem("SurveyID") != null && $window.localStorage.getItem("SurveyID") != "") {
                    $scope.Dashboard.Item.SurveyID = $window.localStorage.getItem("SurveyID");
                }

                if ($window.localStorage.getItem("PatientSurveyID") != null && $window.localStorage.getItem("PatientSurveyID") != "") {
                    $scope.Dashboard.PatientSurveyID = $window.localStorage.getItem("PatientSurveyID");
                }

                if (getParameterByName("FromDate") != null && getParameterByName("FromDate") != "") {
                    $scope.Dashboard.FromDate = getParameterByName("FromDate");
                }
                else {
                    var d = new Date();
                    d.setFullYear(d.getFullYear() - 1);
                    $scope.Dashboard.FromDate = d;
                    $scope.Dashboard.ToDate = new Date();
                }

                if (window.location.pathname.indexOf("/Dashboard/MyPatientDashaboard") > -1) {
                    $scope.Dashboard.Methods.GetProviderPatientThirdPartyApp();
                    $scope.Dashboard.Methods.GetPatientSuggestionsbyPatientSurveyID();
                    $scope.Dashboard.Methods.GetPatientIndicatorListByPatientId($scope.Dashboard.Item.PatientID);
                    $scope.Dashboard.Methods.GetPatientSurveyStatusData();
                    $scope.Dashboard.Methods.GetSurveyByPatient_Provider_ID();
                }

                if (window.location.pathname.indexOf("/Patient/Dashboard") > -1 || window.location.pathname.indexOf("/Patient/PatientDashboard") > -1) {
                    $scope.Dashboard.Methods.GetPatientByUserName($scope.Dashboard.Item.User.UserName);
                }

                if (window.location.pathname.indexOf("Dashboard/Population") > -1) {
                    $scope.Dashboard.NormValue = 50;
                    $scope.Dashboard.Methods.GetSurveyByPatient_Provider_Org_Practice_IDs(false);
                }

                if (window.location.pathname.indexOf("/Patient/Dashboard") > -1) {
                    $scope.Dashboard.Methods.GetProviderPatientThirdPartyApp();
                }

                $scope.Dashboard.Methods.GetPathwayList();
            },

            GetPathwayList: function () {
                $scope.Dashboard.Services.GetPathwayList($scope.Dashboard.Item.ProviderID, $scope.Dashboard.PracticeId, $scope.Dashboard.OrganizationId);
            },

            setTabValue: function (val) {
                if (val == 1) {
                    $scope.Dashboard.IsLoading = true;
                    $scope.Dashboard.Services.GetPatientSurveyStatusBy_PatientId($scope.Dashboard.Item.PatientID, false);
                }
                else if (val == 2) {
                    $scope.Dashboard.isSecond = true;

                    $scope.Dashboard.IsLoading = true;
                    $scope.Dashboard.Services.GetPatientSurveyStatusBy_PatientId($scope.Dashboard.Item.PatientID, true);
                }
            },

            CollectIdFromSelectedEprom: function (PatientSurveyId, SurveyId, ProviderId, OrganizationId, PracticeId, fromradio) {
                if (PatientSurveyId != undefined) {
                    if (window.location.pathname.indexOf("Patient/PatientDashboard") > -1 && !fromradio) {
                        $scope.Dashboard.PatientSurveyID = null;
                        $window.localStorage.setItem("PatientSurveyID", "");
                    }
                    else {
                        $scope.Dashboard.PatientSurveyID = PatientSurveyId;
                        $window.localStorage.setItem("PatientSurveyID", PatientSurveyId);
                    }
                }

                if (SurveyId != undefined) {
                    $scope.Dashboard.Item.SurveyID = SurveyId;
                    $window.localStorage.setItem("SurveyID", SurveyId);
                }

                if (ProviderId != undefined) {
                    $scope.Dashboard.Item.ProviderID = ProviderId;
                    $window.localStorage.setItem("ProviderID", ProviderId);
                }

                if (OrganizationId != undefined) {
                    $scope.Dashboard.OrganizationId = OrganizationId;
                    $window.localStorage.setItem("OrganizationId", OrganizationId);
                }

                if (PracticeId != undefined) {
                    $scope.Dashboard.PrcaticeId = PracticeId;
                    $window.localStorage.setItem("PracticeId", PracticeId);
                }
            },

            ClearSearchApp: function () {
                $scope.Dashboard.SearchApp = '';
            },

            redirectToProviderDashboard: function () {
                $window.location.href = "/Provider/Dashboard";
            },

            GetSurveyMonkeydetailsForDashboard: function () {
                $scope.Dashboard.IsLoading = true;
                $scope.Dashboard.Services.GetSurveyMonkeydetailsForDashboard();
            },

            SetSurveyMonkeydetailsForDashboard: function (data) {
                $scope.Dashboard.IsLoading = false;
                $scope.Dashboard.SurveyMonkeyDetails = JSON.parse(data);
            },

            GetSurveyList: function () {
                $scope.Dashboard.IsLoading = true;
                $scope.Dashboard.Services.GetSurveyList();
            },

            SetSurveyListforPatient: function (data) {
                $scope.Dashboard.IsLoading = false;
                $scope.Dashboard.SurveyList = JSON.parse(data);
                if ($scope.Dashboard.SurveyList != null && $scope.Dashboard.SurveyList.length > 0) {
                    if (window.location.pathname.indexOf("Dashboard/MyPatientDashaboard") == -1) {
                        $scope.Dashboard.Item.SurveyID = $scope.Dashboard.SurveyList[0].SurveyID;
                    }
                }
            },

            GetProviderPatientDetailByUserName: function () {
                $scope.Dashboard.Services.GetProviderPatientDetailByUserName($scope.Dashboard.Item.User.UserName, $scope.Dashboard.OrganizationId, $scope.Dashboard.PracticeId);
            },

            SetPatientDetails: function (data) {
                if (data != null && data.length > 0) {
                    if (window.location.pathname.indexOf("Dashboard/MyPatientDashaboard") == -1) {
                        $scope.Dashboard.Item.PatientID = data[0].PatientID;
                    }
                    $scope.Dashboard.Methods.GetPatientSurveyStatus(true);
                }

                $scope.Dashboard.PatientList = data;
            },

            SetQuestionList: function (surveyname, Questions) {
                $scope.Dashboard.SurveyName = surveyname;
                $scope.Dashboard.Questions = Questions;
                var data = '';
                for (var i = 0; i < Questions.length; i++) {
                    data += " <tr><td>" + Questions[i].heading + "</td></tr>"
                }

                $("#tbodyQuestions").html(data)
                $('#QuestionModel').modal('show');
            },

            GetSurveyMonkeyAnalyzeResult: function (Ids) {
                if (Ids != undefined && Ids != null && Ids != "") {
                    var arr = Ids.split('_');
                    var SurveyID = arr[0];
                    $scope.Dashboard.CollectorId = arr[1];
                    $scope.Dashboard.IsLoading = true;
                    $scope.Dashboard.Services.GetSurveyMonkeyAnalyzeResult(SurveyID);
                }
            },

            SetSurveyMonkeyAnalyzeResult: function (data) {
                $scope.Dashboard.IsLoading = false;
                $scope.Dashboard.QuestionResult = JSON.parse(data);
            },

            SetSurveyMonkeyResponseResult: function (response) {
                $scope.Dashboard.AnswerResult = JSON.parse(response);
                var questionList = $scope.Dashboard.QuestionResult;
                var ansList = $scope.Dashboard.AnswerResult.data;

                var result = [];

                if (questionList != undefined) {
                    if (questionList.pages != undefined) {
                        for (var q = 0; q < questionList.pages.length; q++) {
                            item = {};
                            var qlist = questionList.pages[q];
                            var questions = [];
                            for (var i = 0; i < qlist.questions.length; i++) {
                                var qlist1 = qlist.questions[i];
                                var qitem = {};
                                if (qlist1.answers != undefined) {
                                    if (qlist1.answers.choices != undefined) {
                                        var answers = [];
                                        for (var a = 0; a < qlist1.answers.choices.length; a++) {
                                            var alist = qlist1.answers.choices[a];
                                            for (var b = 0; b < ansList.length; b++) {
                                                var alist0 = ansList[b];
                                                var collectorid = alist0.collector_id;
                                                var RespondentId = alist0.id;
                                                var alist1 = alist0.pages[q];
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
                                                                                    answer_text: alist.text,
                                                                                    collectorid: collectorid,
                                                                                    RespondentId: RespondentId
                                                                                });
                                                                            }
                                                                        }
                                                                    }
                                                                    else {
                                                                        result.push({
                                                                            question_title: qlist1.headings[0].heading,
                                                                            answer_text: alist.text,
                                                                            collectorid: collectorid,
                                                                            RespondentId: RespondentId
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

                $scope.Dashboard.IsLoading = false;
            },

            GetPatientIndicatorListByPatientId: function (Id) {
                $scope.Dashboard.IsLoading = true;
                $scope.Dashboard.Services.GetPatientIndicatorListByPatientId(Id);
            },

            SetPatientIndicatorsData: function (data) {
                $scope.Dashboard.IsLoading = false;
                $scope.Dashboard.PatientIndicatorList = JSON.parse(data);
            },

            GetPatientByUserName: function (UserName) {
                $scope.Dashboard.Services.GetPatientByUserName(UserName);
            },

            GetPatientSurveyStatusData: function () {
                $scope.Dashboard.Services.GetPatientSurveyStatusData($scope.Dashboard.Item.PatientID, $scope.Dashboard.OrganizationId, $scope.Dashboard.PracticeId, $scope.Dashboard.Item.ProviderID, $scope.Dashboard.Item.SurveyID, $filter('date')($scope.Dashboard.FromDate, "MM/dd/yyyy"));
            },

            SetPatient: function (data) {
                if (data != undefined && data != null) {
                    $scope.Dashboard.Patients = data;

                    $scope.Dashboard.Item.PatientID = data.ID;
                    $window.localStorage.setItem("PatientId", data.ID);

                    $scope.Dashboard.Methods.GetPatientIndicatorListByPatientId($scope.Dashboard.Item.PatientID);
                    if (window.location.pathname.indexOf("Patient/PatientDashboard") > -1) {
                        $scope.Dashboard.Methods.GetPatientSurveyStatusBy_PatientId();
                    }
                    else if (window.location.pathname.indexOf("/Patient/Dashboard") > -1) {
                        $scope.Dashboard.Methods.GetSurveyByPatient_Provider_ID();
                        $scope.Dashboard.Methods.GetPatientSuggestionsbyPatientSurveyID();
                    }
                    else {
                        $scope.Dashboard.Methods.GetSurveyByPatient_Provider_Org_Practice_IDs(false);
                    }


                    if (window.location.pathname.indexOf("/Patient/Dashboard") > -1 || window.location.pathname.indexOf("/Dashboard/MyPatientDashaboard") > -1) {
                        $scope.Dashboard.Methods.GetPatientSurveyStatusData();
                    }
                }
            },

            GetSurveyByPatient_Provider_Org_Practice_IDs: function (isfromPatient) {
                $scope.Dashboard.IsLoading = true;
                $scope.Dashboard.Services.GetSurveyByPatient_Provider_Org_Practice_IDs(isfromPatient, $scope.Dashboard.Item.PatientID, $scope.Dashboard.Item.ProviderID, $scope.Dashboard.OrganizationId, $scope.Dashboard.PracticeId);
            },

            GetSurveyByPatient_Provider_ID: function () {
                $scope.Dashboard.IsLoading = true;
                $scope.Dashboard.Services.GetSurveyByPatient_Provider_ID($scope.Dashboard.Item.PatientID, $scope.Dashboard.Item.ProviderID);
            },

            GetPatientSurveyStatusBy_PatientId_ProviderId: function () {
                $scope.Dashboard.Services.GetPatientSurveyStatusBy_PatientId_ProviderId($scope.Dashboard.Item.PatientID, $scope.Dashboard.Item.ProviderID)
            },

            GetPatientSurveyStatusBy_PatientId: function () {
                $scope.Dashboard.IsLoading = true;
                $scope.Dashboard.Services.GetPatientSurveyStatusBy_PatientId($scope.Dashboard.Item.PatientID, false);
            },

            SetSurveyList: function (data) {
                $scope.Dashboard.Item.surveyList = JSON.parse(data);
                $scope.Dashboard.IsLoading = false;

                if (window.location.pathname.indexOf("Patient/PatientDashboard") > -1) {
                    if ($scope.Dashboard.isFirst) {
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

                    if ($scope.Dashboard.isSecond) {
                        if ($.fn.DataTable.isDataTable('.sample_2')) {
                            $(".sample_2").dataTable().fnDestroy();
                        }
                        setTimeout(function () {
                            $scope.table = $(".sample_2").dataTable({
                                "paging": true,
                                "ordering": true,
                                "searching": true,
                                "autoFill": true,
                                "dom": 'ftip',
                                "pagingType": "simple_numbers"
                            });
                        }, 300);
                    }
                }

                if ($scope.Dashboard.Item.surveyList.length > 0) {

                    for (var i = 0; i < $scope.Dashboard.Item.surveyList.length; i++) {
                        if ($scope.Dashboard.PatientSurveyID != null && $scope.Dashboard.PatientSurveyID != "") {
                            if ($scope.Dashboard.Item.surveyList[i].ID == $scope.Dashboard.PatientSurveyID) {

                                $scope.Dashboard.Methods.CollectIdFromSelectedEprom($scope.Dashboard.Item.surveyList[i].ID, $scope.Dashboard.Item.surveyList[i].SurveyID, $scope.Dashboard.Item.surveyList[i].ProviderID, $scope.Dashboard.Item.surveyList[i].OrganizationID, $scope.Dashboard.Item.surveyList[i].PracticeID, false);

                                if ($scope.Dashboard.Item.surveyList[i].User != null) {
                                    $scope.Dashboard.PatientName = $scope.Dashboard.Item.surveyList[i].User.FirstName + " " + $scope.Dashboard.Item.surveyList[i].User.LastName;
                                    $scope.Dashboard.DOB = $scope.Dashboard.Item.surveyList[i].User.DOB;
                                    $scope.Dashboard.Email = $scope.Dashboard.Item.surveyList[i].User.Email;
                                }

                                if ($scope.Dashboard.Item.surveyList[i].Patient != null) {
                                    $scope.Dashboard.IHINumber = $scope.Dashboard.Item.surveyList[i].Patient.IHINumber;
                                    $scope.Dashboard.MedicareNumber = $scope.Dashboard.Item.surveyList[i].Patient.MedicareNumber;
                                }

                                $scope.Dashboard.Item.PatientID = $scope.Dashboard.Item.surveyList[i].PatientID;
                                $scope.Dashboard.OrganizationName = $scope.Dashboard.Item.surveyList[i].OrganizationName;
                                $scope.Dashboard.PracticeName = $scope.Dashboard.Item.surveyList[i].PracticeName;
                                $scope.Dashboard.ProviderName = $scope.Dashboard.Item.surveyList[i].ProviderName;
                                $scope.Dashboard.SurveyName = $scope.Dashboard.Item.surveyList[i].ExternalTitle;
                            }
                            else {
                                if (i == 0) {
                                    $scope.Dashboard.Item.PatientSurveyID = $scope.Dashboard.Item.surveyList[i].ID
                                }
                            }
                        }
                        else {
                            if (i == 0) {
                                $scope.Dashboard.Item.PatientSurveyID = $scope.Dashboard.Item.surveyList[i].ID
                            }
                        }

                        if ($scope.Dashboard.Item.surveyList[i].StartDate != null && $scope.Dashboard.Item.surveyList[i].StartDate != "")
                            $scope.Dashboard.Item.surveyList[i].StartDate = new Date($scope.Dashboard.Item.surveyList[i].StartDate);
                        if ($scope.Dashboard.Item.surveyList[i].EndDate != null && $scope.Dashboard.Item.surveyList[i].EndDate != "")
                            $scope.Dashboard.Item.surveyList[i].EndDate = new Date($scope.Dashboard.Item.surveyList[i].EndDate);
                    }
                }
            },

            NotifyToPatient: function () {
                $scope.Dashboard.Services.NotifyToPatient($scope.Dashboard.Item.ProviderID, $scope.Dashboard.Item.PatientID, $scope.Dashboard.OrganizationId, $scope.Dashboard.PracticeId, $scope.Dashboard.Item.SurveyID, $scope.Dashboard.Email, $scope.Dashboard.PatientName, $scope.Dashboard.SurveyName, $scope.Dashboard.SuggestionList.Suggestions);
            },

            CreatePatientSuggestion: function () {
                if ($scope.Dashboard.SuggestionList != null) {
                    if ($scope.Dashboard.SuggestionList.Suggestions != null && $scope.Dashboard.SuggestionList.Suggestions != "") {
                        $scope.Dashboard.IsLoading = true;
                        $scope.Dashboard.Services.CreatePatientSuggestion({
                            ID: $scope.Dashboard.SuggestionList.ID,
                            PatientSurveyID: $scope.Dashboard.PatientSurveyID,
                            Suggestions: $scope.Dashboard.SuggestionList.Suggestions
                        });
                    }
                    else {
                        toaster.pop('info', '', "Enter suggestions and comments.");
                    }
                } else {
                    toaster.pop('info', '', "Enter suggestions and comments.");
                }
            },

            GetPatientSuggestionsbyPatientSurveyID: function () {
                $scope.Dashboard.Services.GetPatientSuggestionsbyPatientSurveyID($scope.Dashboard.PatientSurveyID);
            },

            SetPatientSuggestions: function (data) {
                $scope.Dashboard.SuggestionList = data;
            },

            GetSurveyDetailBySurveyId: function () {
                return $scope.Dashboard.Services.GetSurveyDetailBySurveyId($scope.Dashboard.Item.SurveyID);
            },

            SetSurveyDetailBySurveyID: function (data) {
                if (data != null && data != "") {;
                    $scope.Dashboard.ExternalId = data.ExternalID
                    $scope.Dashboard.CollectorId = data.CollectorID;
                    $scope.Dashboard.Methods.GetPatientScoreResult($scope.Dashboard.ExternalId);
                }
            },

            GetPatientScoreResult: function (Ids) {
                if (Ids != undefined && Ids != null && Ids != "") {
                    var arr = Ids.split('_');
                    var ExternalId = arr[0];
                    if (arr.length > 1) {
                        $scope.Dashboard.CollectorId = arr[1];
                    }
                    $scope.Dashboard.IsLoading = true;
                    $scope.Dashboard.Services.GetPatientScoreResult(ExternalId);
                }
            },

            GetPatientSelection: function (patientId) {
                $scope.Dashboard.Item.PatientID = patientId;
            },

            GetPatientSurveyStatus: function (isFromPatient) {
                if ($scope.Dashboard.Item.SurveyID != '') {
                    if (isFromPatient) {
                        $scope.Dashboard.Methods.GetSurveyByPatient_Provider_Org_Practice_IDs(isFromPatient);
                    }
                    else {
                        var Fromdate = $filter('date')($scope.Dashboard.FromDate, "MM/dd/yyyy");
                        var ToDate = $filter('date')($scope.Dashboard.ToDate, "MM/dd/yyyy");
                        var PathwayID = (($scope.Dashboard.PathwayID != null) && ($scope.Dashboard.PathwayID != undefined)) ? $scope.Dashboard.PathwayID : "";
                        if ($scope.Dashboard.Item.SurveyID != '') {
                            $scope.Dashboard.Services.GetPatientSurveyStatus($scope.Dashboard.Item.SurveyID, $scope.Dashboard.Item.ProviderID, $scope.Dashboard.Item.PatientID, Fromdate, ToDate, PathwayID, $scope.Dashboard.OrganizationId, $scope.Dashboard.PracticeId);
                        }
                    }
                }
            },

            RedirecttoMyPatientDashboard: function () {
                if ($scope.Dashboard.PatientSurveyID != null && $scope.Dashboard.PatientSurveyID != "" && $scope.Dashboard.FromDate != null && $scope.Dashboard.FromDate != "" && $scope.Dashboard.FromDate != undefined) {

                    $window.localStorage.setItem("SurveyID", $scope.Dashboard.Item.SurveyID);
                    var fromDate = $filter('date')($scope.Dashboard.FromDate, "MM/dd/yyyy");

                    $window.location.href = "/Patient/Dashboard?FromDate=" + fromDate;
                }
                else if ($scope.Dashboard.PatientSurveyID == null || $scope.Dashboard.PatientSurveyID == "") {
                    toaster.pop('warning', '', "Please select Eproms!");
                }
                else if ($scope.Dashboard.FromDate == null || $scope.Dashboard.FromDate == "" || $scope.Dashboard.FromDate == undefined) {
                    toaster.pop('warning', '', "Please select from date!");
                }
            },

            SetPatientSurveyStatusData: function (data) {
                if (data != undefined && data != null && data.length > 0) {
                    $scope.Dashboard.Methods.BindChart(data[0].ExternalTitle, data);

                    for (var i = 0; i < data.length; i++) {
                        var scoreData = JSON.parse(data[i].Score);
                        data[i].Score = scoreData;
                    }
                }
                $scope.Dashboard.SurveyStatusData = data;
            },

            BindChart: function (heading, ScoreList) {
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
                $scope.Dashboard.Methods.BarChart("population-chart", xAxis, heading, Data, islegendEnable, NormValue);
            },

            BarChart: function (ChartId, xAxis, heading, Data, legendEnable, Normvalue) {
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
                            color: '#0000FF',
                            width: 1,
                            value: Normvalue
                        }],
                    },
                    tooltip: {

                    },
                    legend: {
                        enabled: legendEnable,
                    },
                    series: Data
                });
                chart = $('#' + ChartId).highcharts();
            },

            GetThirdPartyAppList: function () {
                $scope.Dashboard.Services.GetThirdPartyAppList($scope.Dashboard.Item.SurveyID);
            },

            SetThirdPartyAppList: function (data) {
                $scope.Dashboard.ThirdPartyAppList = JSON.parse(data);
            },

            ManageProviderPatientThirdPartyApp: function (AppId, list) {

                var isExist = false;
                if (list != undefined) {
                    if (list.indexOf(AppId) !== -1) {
                        isExist = true;
                    }
                }

                if (!isExist) {
                    var data = {
                        ThirdPartyAppID: AppId,
                        ProviderID: $scope.Dashboard.Item.ProviderID,
                        PatientID: $scope.Dashboard.Item.PatientID,
                        SurveyID: $scope.Dashboard.Item.SurveyID,
                        OrganizationID: $scope.Dashboard.OrganizationId,
                        PracticeID: $scope.Dashboard.PracticeId
                    }
                    $scope.Dashboard.Methods.SaveProviderPatientThirdPartyApp(data);
                }
                else {
                    $scope.Dashboard.Methods.DeletePatientThirdPartyApp(AppId);
                }
            },

            SaveProviderPatientThirdPartyApp: function (data) {
                $scope.Dashboard.Services.SaveProviderPatientThirdPartyApp(data);
            },

            DeletePatientThirdPartyApp: function (AppId) {
                $scope.Dashboard.Services.DeletePatientThirdPartyApp(AppId, $scope.Dashboard.Item.ProviderID, $scope.Dashboard.OrganizationId, $scope.Dashboard.PracticeId, $scope.Dashboard.Item.PatientID, $scope.Dashboard.Item.SurveyID);
            },

            GetProviderPatientThirdPartyApp: function () {
                $scope.Dashboard.Services.GetProviderPatientThirdPartyApp($scope.Dashboard.Item.PatientID, $scope.Dashboard.OrganizationId, $scope.Dashboard.PracticeId, $scope.Dashboard.Item.ProviderID, $scope.Dashboard.Item.SurveyID);
            },

            SetProviderPatientThirdPartyApp: function (data) {
                var List = [];
                for (var i = 0; i < data.length; i++) {
                    List.push(data[i].ThirdPartyAppID);
                }
                $scope.Dashboard.IsLoading = true;
                $timeout(function () {
                    $scope.Dashboard.IsLoading = false;
                    $scope.Dashboard.ThirdPartyApp = List;
                }, 500);

                $scope.Dashboard.AppList = data;
                if (window.location.pathname.indexOf("/Dashboard/MyPatientDashaboard") > -1) {
                    $scope.Dashboard.Methods.GetThirdPartyAppList();
                }
            },
        },

        Services: {

            GetPathwayList: function (ProviderId, PracticeID, OrganizationId) {
                $http.get('/api/eproms/GetPathwayList?ProviderId=' + ProviderId + '&PracticeID= ' + PracticeID + '&OrganizationId=' + OrganizationId).success(function (data) {
                    $scope.Dashboard.PathwayList = data;
                    if (data != null) {
                    }
                    else
                        $scope.Dashboard.IsLoading = false;
                });
            },

            GetSurveyMonkeyAnalyzeResult: function (surveyId) {
                $http.get('/api/eproms/GetSurveyMonkey_SurveyDetails?surveyId=' + surveyId).success(function (data) {
                    $scope.Dashboard.IsLoading = false;
                    $scope.Dashboard.QuestionResult = JSON.parse(data);
                    $scope.Dashboard.Services.GetSurveyMonkeyResponseByCollectorID($scope.Dashboard.CollectorId);
                });
            },

            GetSurveyMonkeyResponseByCollectorID: function (collectorId) {
                $http.get('/api/eproms/GetSurveyMonkeyResponseBy_CollectorID?collectorId=' + collectorId).success($scope.Dashboard.Methods.SetSurveyMonkeyResponseResult);
            },

            GetSurveyMonkeydetailsForDashboard: function (id) {
                $http.get('/api/provider/GetSurveyMonkeydetailsForDashboard').success($scope.Dashboard.Methods.SetSurveyMonkeydetailsForDashboard);
            },

            GetSurveyList: function (id) {
                $http.get('/api/provider/GetSurveyList').success($scope.Dashboard.Methods.SetSurveyList);
            },

            GetProviderPatientDetailByUserName: function (userName, organizationId, practiceId) {
                $http.get('/api/Patient/GetProviderPatientDetailByUserName?UserName=' + userName + '&OrganizationId=' + organizationId + '&PracticeId=' + practiceId).success($scope.Dashboard.Methods.SetPatientDetails);
            },

            NotifyToPatient: function (ProviderId, PatientId, OrganizationId, PracticeId, SurveyId, ToAddress, PatientName, epromTitle, Suggestion) {
                $http.post('/api/Email/NotifyToPatient?ProviderId=' + ProviderId + "&PatientId=" + PatientId + "&OrganizationId=" + OrganizationId + "&PracticeId=" + PracticeId + "&SurveyId=" + SurveyId + "&ToAddress=" + ToAddress + "&PatientName=" + PatientName + "&epromTitle=" + epromTitle + "&Suggestion=" + Suggestion).success(function (value) {
                    if (value != null) {
                        $scope.Dashboard.IsLoading = false;
                        if (value == "1") {
                            toaster.pop('success', '', "Notification sent successfully.");
                        }
                        else {
                            toaster.pop('success', '', "Mail has been sent already.");
                        }
                    }
                });
            },

            CreatePatientSuggestion: function (data) {
                $http.post('/api/provider/CreatePatientSuggestion', data).success(function (response) {
                    if (response == "1") {
                        $scope.Dashboard.Methods.NotifyToPatient();
                    }
                    else {
                        $scope.Dashboard.IsLoading = false;
                    }
                });
            },

            GetPatientIndicatorListByPatientId: function (id) {
                $http.get('/api/patient/GetPatientIndicatorsByPatientID?patientid=' + id).success($scope.Dashboard.Methods.SetPatientIndicatorsData);
            },

            GetPatientSuggestionsbyPatientSurveyID: function (PatientSurveyId) {
                $http.get('/api/provider/GetPatientSuggestionsbyPatientSurveyID?PatientSurveyId=' + PatientSurveyId).success($scope.Dashboard.Methods.SetPatientSuggestions);
            },

            GetPatientByUserName: function (data) {
                $http.get('/api/Patient/GetPatientDetailsByUserName/?UserName=' + data).success($scope.Dashboard.Methods.SetPatient);
            },

            GetPatientSurveyStatusData: function (PatientId, OrganizationId, PracticeId, ProviderId, SurveyId, FromDate) {
                $http.get('/api/Patient/GetPatientSurveyStatusData/?PatientID=' + PatientId + '&OrganizationId=' + OrganizationId + '&PracticeId=' + PracticeId + '&ProviderId=' + ProviderId + '&SurveyId=' + SurveyId + '&FromDate=' + FromDate + '&forEmail=' + false).success($scope.Dashboard.Methods.SetPatientSurveyStatusData);
            },
            // Change--
            GetSurveyByPatient_Provider_Org_Practice_IDs: function (isfromPatient, PatientId, ProviderId, OrganizationId, PracticeId) {
                $http.get('/api/eproms/GetSurveyByPatient_Provider_Org_Practice_IDs?PatientId=' + PatientId + "&ProviderId=" + ProviderId + "&OrganizationId=" + OrganizationId + "&PracticeId=" + PracticeId + "&isAllPatient=" + isfromPatient + "&isCompleted=true").success(function (data) {
                    $scope.Dashboard.Methods.SetSurveyListforPatient(data);
                    if (isfromPatient) {
                        var Fromdate = $filter('date')($scope.Dashboard.FromDate, "MM/dd/yyyy");
                        var ToDate = $filter('date')($scope.Dashboard.ToDate, "MM/dd/yyyy");
                        var PathwayID = (($scope.Dashboard.PathwayID != null) && ($scope.Dashboard.PathwayID != undefined)) ? $scope.Dashboard.PathwayID : "";
                        $scope.Dashboard.Services.GetPatientSurveyStatus($scope.Dashboard.Item.SurveyID, $scope.Dashboard.Item.ProviderID, $scope.Dashboard.Item.PatientID, Fromdate, ToDate, PathwayID, $scope.Dashboard.OrganizationId, $scope.Dashboard.PracticeId);
                    }
                });
            },

            GetSurveyByPatient_Provider_ID: function (PatientId, ProviderId) {
                $http.get('/api/eproms/GetSurveyByPatient_Provider_ID?PatientId=' + PatientId + "&ProviderId=" + ProviderId + "&isCompleted=true").success($scope.Dashboard.Methods.SetSurveyList);
            },

            GetPatientSurveyStatusBy_PatientId_ProviderId: function (PatientId, ProviderId) {
                $http.get('/api/eproms/GetPatientSurveyStatusBy_PatientId_ProviderId?PatientId=' + PatientId + "&ProviderId=" + ProviderId).success($scope.Dashboard.Methods.SetSurveyList);
            },

            GetPatientSurveyStatusBy_PatientId: function (PatientId, isArchive) {
                $http.get('/api/eproms/GetPatientSurveyStatusBy_PatientId?PatientId=' + PatientId + '&isArchive=' + isArchive).success($scope.Dashboard.Methods.SetSurveyList);
            },

            GetSurveyDetailBySurveyId: function (id) {
                $http.get('/api/eproms/GetSurveyById?id=' + id).success(function (response) {
                    $scope.Dashboard.Methods.SetSurveyDetailBySurveyID(JSON.parse(response));
                })
            },

            GetPatientScoreResult: function (surveyId) {
                $http.get('/api/eproms/GetSurveyMonkey_SurveyDetails?surveyId=' + surveyId).success(function (data) {
                    $scope.Dashboard.QuestionResult = JSON.parse(data);
                    $scope.Dashboard.Services.GetSurveyMonkeyResponseByCollectorID($scope.Dashboard.CollectorId);
                });
            },

            GetSurveyMonkeyResponseByCollectorID: function (collectorId) {
                $http.get('/api/eproms/GetSurveyMonkeyResponseBy_CollectorID?collectorId=' + collectorId).success($scope.Dashboard.Methods.SetSurveyMonkeyResponseResult);
            },

            GetPatientSurveyStatus: function (SurveyId, ProviderId, PatientId, FromDate, ToDate, PathwayID, OrgId, PracticeId) {

                if ((PathwayID == null) || (PathwayID == undefined)) {
                    PathwayID = "";
                }

                var parameters = 'SurveyId=' + SurveyId + '&PatientId=' + PatientId + '&ProviderId=' + ProviderId + '&FromDate=' + FromDate + '&ToDate=' + ToDate + '&OrganizationId=' + OrgId + '&PracticeId=' + PracticeId + '&PathwayID=' + PathwayID + '';

                $http.get('/api/patient/GetPatientSurveyStatus?' + parameters).success(function (response) {
                    if (response != null && response != "") {
                        var data = JSON.parse(response);

                        if (window.location.pathname.indexOf("Dashboard/Population") > -1) {
                            $scope.Dashboard.Methods.SetPatientSurveyStatusData(data);
                        }
                    }
                });
            },

            GetThirdPartyAppList: function (SurveyId) {
                $http.get('/api/ThirdPartyApp/GetThirdPartyAppByCategoryID?SurveyID=' + SurveyId).success($scope.Dashboard.Methods.SetThirdPartyAppList);
            },

            SaveProviderPatientThirdPartyApp: function (data) {
                $http.post('/api/Provider/SaveProviderPatientThirdPartyApp', data).success(function (response) {
                    $scope.Dashboard.Methods.GetProviderPatientThirdPartyApp();
                });
            },

            DeletePatientThirdPartyApp: function (AppID, ProviderID, OrganizationID, PracticeID, PatientID, SurveyID) {
                $http.delete('/api/Provider/DeletePatientThirdPartyApp?ThirdPartyAppId=' + AppID + "&ProviderID=" + ProviderID + "&OrganizationID=" + OrganizationID + "&PracticeID=" + PracticeID + "&PatientID=" + PatientID + "&SurveyID=" + SurveyID).success(function (response) {
                    if (response) {
                        $scope.Dashboard.Methods.GetProviderPatientThirdPartyApp();
                    }
                });
            },

            GetProviderPatientThirdPartyApp: function (PatientID, OrganizationID, PracticeID, ProviderID, SurveyID) {
                $http.get('/api/Provider/GetProviderPatientThirdPartyApp?PatientID=' + PatientID + "&OrganizationID=" + OrganizationID + "&PracticeID=" + PracticeID + "&ProviderID=" + ProviderID + "&SurveyID=" + SurveyID).success($scope.Dashboard.Methods.SetProviderPatientThirdPartyApp);
            },
        }, UI: {
        }
    }
    $scope.Dashboard.Methods.Initialize();
}]);
