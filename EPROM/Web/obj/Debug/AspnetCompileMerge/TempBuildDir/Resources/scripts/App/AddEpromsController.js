app.controller('AddEpromsController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$cookies', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $cookies, $ngBootbox) {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $scope.AddEproms = {
        Item: { UserName: '', IsPublish: true, StartDate: '', EndDate: '' },
        list_SurvayCategory: [],
        list_SubCategory: [],
        list_SurvayType: [],
        list_SurvayMonkeyData: [],
        issubmitted: false,
        IsLoading: false,
        datepickerStartDate: {},
        datepickerEndDate: {},
        IsDateValid: true,

        Initialize: function () {
            if ($cookies.get("username") != undefined) {
                $scope.AddEproms.Item.UserName = $cookies.get("username");
            }
            
            if (getParameterByName("id") != null && getParameterByName("id") != "") {
                var id = getParameterByName("id");
                $scope.AddEproms.Item.ID = parseInt(id);
                $scope.AddEproms.Methods.GetEpromsById($scope.AddEproms.Item.ID);
            }

            $scope.AddEproms.Methods.GetSurveyCategory();
            $scope.AddEproms.Methods.GetSurveyTypes();
            $scope.AddEproms.Methods.GetEpromsBySurveyMonkey();
        },

        Methods: {
            RedirecttoList: function () {
                $window.location.href = '/Eproms/Index';
            },
            SetExternalData: function (item) {
                $scope.AddEproms.Methods.GetSurveyMonkeyCollectorIDBySurveyID(item.id, true);
                $scope.AddEproms.Item.ExternalTitle = item.title;
                $scope.AddEproms.Item.ExternalID = item.id;
                $scope.AddEproms.Item.URL = item.href;
            },
            EpromsPreview: function (item) {
                $scope.AddEproms.Methods.GetSurveyMonkeyCollectorIDBySurveyID(item.id);
            },
            Create: function () {
                $scope.AddEproms.issubmitted = true;

                if ($scope.formAddEproms.$valid && $scope.AddEproms.IsDateValid) {
                    $scope.AddEproms.IsLoading = true;
                    $scope.AddEproms.Services.Create($scope.AddEproms.Item)
                }
            },
            Put: function () {
                $scope.AddEproms.issubmitted = true;
                if ($scope.formAddEproms.$valid && $scope.AddEproms.IsDateValid) {
                    $scope.AddEproms.IsLoading = true;
                    $scope.AddEproms.Services.Put($scope.AddEproms.Item)
                }
            },
            GetEpromsBySurveyMonkey: function () {
                $scope.AddEproms.IsLoading = true;
                $scope.AddEproms.Services.GetEpromsBySurveyMonkey();
            },

            SetEpromsBySurveyMonkey: function (data) {
                $scope.AddEproms.IsLoading = false;
                $scope.AddEproms.list_SurvayMonkeyData = JSON.parse(data);
            },

            GetSurveyCategory: function () {
                $scope.AddEproms.Services.GetSurveyCategory();
            },
            SetSurveyCategory: function (data) {
                $scope.AddEproms.list_SurvayCategory = JSON.parse(data);
            },
            GetSubCategoryById: function (Id) {
                if (Id == undefined) {
                    $scope.AddEproms.list_SubCategory = [];
                }
                return $scope.AddEproms.Services.GetSubCategoryById(Id);
            },
            SetSubCategoryData: function (data) {
                $scope.AddEproms.list_SubCategory = JSON.parse(data);
            },
            GetSurveyTypes: function () {
                $scope.AddEproms.Services.GetSurveyTypes();
            },

            SetSurveyTypes: function (data) {
                $scope.AddEproms.list_SurvayType = JSON.parse(data);
            },

            GetEpromsById: function (id) {
                return $scope.AddEproms.Services.GetById(id);
            },
            SetEditData: function (response) {
                var data = JSON.parse(response);
                $scope.AddEproms.Item = data;
                if (data != null) {
                    $scope.AddEproms.Methods.GetSubCategoryById(data.SurveyCategoryID)
                }

                if (data.StartDate != null && data.StartDate != undefined && data.StartDate != '')
                    $scope.AddEproms.Item.StartDate = new Date(data.StartDate);

                if (data.EndDate != null && data.EndDate != undefined && data.EndDate != '')
                    $scope.AddEproms.Item.EndDate = new Date(data.EndDate);

            },
            CheckDateValidation: function () {
                $scope.errMessage = '';
                var curDate = new Date();
                if ($scope.AddEproms.Item.EndDate != null && $scope.AddEproms.Item.EndDate != undefined && $scope.AddEproms.Item.EndDate != '') {
                    if (new Date($scope.AddEproms.Item.StartDate) > new Date($scope.AddEproms.Item.EndDate)) {
                        $scope.errMessage = 'End Date should be greater than start date';
                        $scope.AddEproms.IsDateValid = false;
                        return false;
                    }
                    else {
                        $scope.AddEproms.IsDateValid = true;
                    }
                }
            },
            opendatePicker1: function () {
                $scope.AddEproms.datepickerStartDate.opened = true;
            },
            opendatePicker2: function () {
                $scope.AddEproms.datepickerEndDate.opened = true;
            },
            GetSurveyMonkeyCollectorIDBySurveyID: function (SurveyID, Isselected) {
                $scope.AddEproms.Services.GetSurveyMonkeyCollectorIDBySurveyID(SurveyID, Isselected);
            },
            SetSurveyMonkeyCollectorID: function (collectorID, Isselected) {
                $scope.AddEproms.Item.collectorID = collectorID;
                $scope.AddEproms.Methods.GetSurveyMonkeyCollectorDetails(collectorID, Isselected);
            },
            GetSurveyMonkeyCollectorDetails: function (CollectorID, Isselected) {
                $scope.AddEproms.Services.GetSurveyMonkeyCollectorDetails(CollectorID, Isselected);
            },
            SetSurveyMonkeyCollectorDetails: function (data, Isselected) {
                if (data != null && data != "") {
                    var response = JSON.parse(data);
                    if (response != null) {
                        $("#previewEproms").attr("src", response.url);

                        if (Isselected == true)
                            $scope.AddEproms.Item.ContentCode = response.url;
                    }
                }
                else {
                    $scope.AddEproms.Item.ContentCode = "";
                }
            },
        },


        Services: {
            GetEpromsBySurveyMonkey: function () {
                $http.get('/api/eproms/GetEpromsBySurveyMonkey').success($scope.AddEproms.Methods.SetEpromsBySurveyMonkey);
            },
            GetSurveyMonkeyCollectorIDBySurveyID: function (surveyId, Isselected) {
                $http.get('/api/eproms/GetSurveyMonkeyCollectorID?surveyId=' + surveyId).success(function (data) {
                    $scope.AddEproms.Methods.SetSurveyMonkeyCollectorID(data, Isselected);
                })
            },
            GetSurveyMonkeyCollectorDetails: function (collectorId, Isselected) {
                $http.get('/api/eproms/GetSurveyMonkeyCollectorDetails_CollectorID?CollectorId=' + collectorId).success(function (data) {
                    $scope.AddEproms.Methods.SetSurveyMonkeyCollectorDetails(data, Isselected);
                })
            },
            GetById: function (id) {
                $http.get('/api/eproms/GetSurveyById?id=' + id).success(function (response) {
                    $scope.AddEproms.Methods.SetEditData(response);
                })
            },
            GetSurveyCategory: function () {
                $http.get('/api/category/Get').success($scope.AddEproms.Methods.SetSurveyCategory)
            },
            GetSubCategoryById: function (id) {
                $http.get('/api/category/GetSubCategoryById/' + id).success($scope.AddEproms.Methods.SetSubCategoryData)
            },
            GetSurveyTypes: function () {
                $http.get('/api/eproms/GetSurveyTypes').success($scope.AddEproms.Methods.SetSurveyTypes)
            },
            Create: function (data) {
                $http.post('/api/eproms/Post', data).success(function () {
                    $scope.AddEproms.IsLoading = false;
                    $scope.AddEproms.Methods.RedirecttoList();
                });
            },
            Put: function (data) {
                $http.post('/api/eproms/Put', data).success(function () {
                    $scope.AddEproms.IsLoading = false;
                    $window.localStorage.setItem("Message", "Successfully Updated!");
                    $scope.AddEproms.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.AddEproms.Initialize();

}]);