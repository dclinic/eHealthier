app.controller('CINTepromController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', '$cookies', '$filter', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox, $cookies, $filter) {
    $scope.PatientEprom = {
        IsLoading: false,
        Methods: {
            Initialize: function () {
                if ($window.location.pathname.indexOf("CINT/PROMIS10") > -1 || $window.location.pathname.indexOf("CINT/POPN") > -1) {
                    $window.localStorage.setItem("emailCINT", $cookies.get("emailCINT"));
                    $window.localStorage.setItem("IDName", $cookies.get("IDName"));
                    $window.localStorage.setItem("IDValue", $cookies.get("IDValue"));
                    $window.localStorage.setItem("ePROMsType", $cookies.get("ePROMsType"));
                    $window.localStorage.setItem("SurveyMonkeyID", $cookies.get("SurveyMonkeyID"));
                    $window.localStorage.setItem("CollectorID", $cookies.get("CollectorID"));
                    $window.localStorage.setItem("ePROMsLink", $cookies.get("ePROMsLink"));

                    $("#content-object").attr('src', $window.localStorage.getItem("ePROMsLink"));
                }

                if (window.location.pathname.indexOf("CINT/CompleteCINTEprom") > -1) {
                    $scope.PatientEprom.IsLoading = true;
                    $scope.PatientEprom.Services.GetSurveyMonkeyAnalyzeResult($window.localStorage.getItem("SurveyMonkeyID"));
                }
            },
            SetSurveyMonkeyResponseResult: function (response) {
                var isCorrectPatient = false;
                var isPromisGender = true;
                $scope.PatientEprom.AnswerResult = JSON.parse(response);
                var questionList = $scope.PatientEprom.QuestionResult;
                var ansList = {};
                var List = '';
                var response = [];

                for (var i = 0; i < $scope.PatientEprom.AnswerResult.data.length; i++) {
                    List = $scope.PatientEprom.AnswerResult.data[i];
                    if (List.custom_variables.email == $window.localStorage.getItem("emailCINT") && List.custom_variables.uniqueId == $window.localStorage.getItem("IDValue")) {
                        if (!isCorrectPatient) {
                            isCorrectPatient = true;
                            response = List;

                            var url = List.analyze_url;
                            ansList = List;

                            $scope.PatientEprom.RespondentId = url.substr(url.indexOf("=") + 1);
                            $window.localStorage.setItem("RespondentId", $scope.PatientEprom.RespondentId);
                        }
                    }
                }

                var result = [];
                if (isCorrectPatient) {
                    if (questionList != undefined) {
                        if (questionList.pages != undefined) {

                            // POPN
                            if (questionList.title == "Preventive ePROMs for Population Health Management™") {
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
                            // POPN

                            // COMMON
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

                                                            if (alist3.other_id != undefined) {
                                                                if (alist.id == alist3.other_id) {

                                                                    if (qlist1.answers.rows != undefined && qlist1.answers.others != undefined) {
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
                            // COMMON

                            // PROMIS10
                            if (questionList.title == "PROMIS Global SF10") {
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
                                                        if (alist2.answers[0].other_id != undefined) {
                                                            if (isPromisGender) {
                                                                isPromisGender = false;
                                                                result.push({
                                                                    question_title: 'What is your gender?',
                                                                    answer_text: alist2.answers[0].text
                                                                });
                                                            }
                                                        }
                                                        else {
                                                            inc = inc + 1;

                                                            if (inc == 1) {
                                                                result.push({
                                                                    question_title: 'What is your age?',
                                                                    answer_text: alist2.answers[0].text
                                                                });
                                                            }

                                                            if (inc == 4) {
                                                                result.push({
                                                                    question_title: 'Please provide the postcode of where you live',
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
                            }
                            // PROMIS10

                            // POPN
                            if (questionList.title == "Preventive ePROMs for Population Health Management™") {
                                var inc = 0;
                                for (var i = 7; i < questionList.pages.length; i++) {
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
                                                        if (alist2.answers[0].other_id != undefined) {
                                                            if (isPromisGender) {
                                                                isPromisGender = false;
                                                                result.push({
                                                                    question_title: 'What is your gender?',
                                                                    answer_text: alist2.answers[0].text
                                                                });
                                                            }
                                                        }
                                                        else {
                                                            inc = inc + 1;

                                                            if (inc == 1) {
                                                                result.push({
                                                                    question_title: 'What is your age?',
                                                                    answer_text: alist2.answers[0].text
                                                                });
                                                            }

                                                            if (inc == 4) {
                                                                result.push({
                                                                    question_title: 'Please provide the postcode of where you live',
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
                            }
                            // POPN

                        }
                    }
                }

                $scope.PatientEprom.AnalyzeResult = result;
                var EpromList = { responselist: result, Eprom_title: questionList.title };
                $scope.PatientEprom.SurveyName = questionList.title;
                $scope.PatientEprom.Services.GetPatientScore(EpromList);
            },
        },
        Services: {
            GetSurveyMonkeyAnalyzeResult: function (surveyId) {
                $http.get('/api/eproms/GetSurveyMonkey_SurveyDetails?surveyId=' + surveyId).success(function (data) {
                    $scope.PatientEprom.QuestionResult = JSON.parse(data);
                    $scope.PatientEprom.Services.GetSurveyMonkeyResponseByCollectorID($window.localStorage.getItem("CollectorID"));
                });
            },
            GetSurveyMonkeyResponseByCollectorID: function (collectorId) {
                $http.get('/api/eproms/GetSurveyMonkeyResponseBy_CollectorID?collectorId=' + collectorId).success($scope.PatientEprom.Methods.SetSurveyMonkeyResponseResult);
            },
            GetPatientScore: function (data) {
                $http.post("/Eproms/PatientScore/dataList", data).success(function (response) {
                    if (response != null && response != "") {
                        $scope.PatientEprom.PatientScore = angular.toJson(response);

                        debugger;

                        var gender = '';
                        var yob = '';
                        var pc = '';
                        if ($cookies.get("Gender") != null && $cookies.get("Gender") != undefined && $cookies.get("Gender") != "") {
                            gender = $cookies.get("Gender");
                        }
                        if ($cookies.get("Age") != null && $cookies.get("Age") != undefined && $cookies.get("Age") != "") {
                            yob = $cookies.get("Age");
                        }
                        if ($cookies.get("PostCode") != null && $cookies.get("PostCode") != undefined && $cookies.get("PostCode") != "") {
                            pc = $cookies.get("PostCode");
                        }

                        var objCINT = {
                            ePROMsType: $window.localStorage.getItem("ePROMsType"),
                            IDName: $window.localStorage.getItem("IDName"),
                            IDValue: $window.localStorage.getItem("IDValue"),
                            Gender: gender,
                            Age: yob,
                            PostCode: pc,
                            Score: $scope.PatientEprom.PatientScore,
                            RespondedID: $window.localStorage.getItem("RespondentId"),
                            SurveyMonkeyID: $window.localStorage.getItem("SurveyMonkeyID")
                        };
                        $http.post("/api/Patient/CINTScore/", objCINT).success(function (obj) {
                            $scope.PatientEprom.Services.ExportToExcel();
                        }).error(function (error) {
                            $scope.PatientEprom.IsLoading = false;
                        });
                    }
                    else {
                        $scope.PatientEprom.IsLoading = false;
                    }
                }).error(function (error) {
                    $scope.PatientEprom.IsLoading = false;
                });
            },
            ExportToExcel: function () {
                $http.post("/CINT/ExportToExcel/").success(function () {
                    top.location.replace('https://s.cint.com/Survey/Complete?ePROM=' + $window.localStorage.getItem("ePROMsType") + '&SurveyMonkeyID=' + $window.localStorage.getItem("SurveyMonkeyID") + '&' + $window.localStorage.getItem("IDName") + '=' + $window.localStorage.getItem("IDValue"));
                }).error(function (error) {
                    $scope.PatientEprom.IsLoading = false;
                });
            },
        },
    }
    $scope.PatientEprom.Methods.Initialize();
}]);