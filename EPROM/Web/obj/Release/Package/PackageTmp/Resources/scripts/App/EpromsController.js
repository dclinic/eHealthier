app.controller('EpromsController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {
    $scope.Eproms = {
        allSelected: false,
        CheckAll: false,
        Check: [],
        Eproms: [],
        EpromsID: [],
        PageNo: 1,
        ID: 0,
        CurrentIndex: 0,
        issubmitted: false,
        IsLoading: false,
        Filter: "All",
        FlagGroupName: "",
        isLoad: true,
        isEdit: false,
        days: 0,
        SID: 0,
        PageParams: {
            TotalCount: 0,
            StartIndex: 0,
            EndIndex: 2,
            CurrentStartIndex: 0,
            CurrentEndIndex: 0,
            FetchRecords: 10,
        },
        PageSize: [{ id: 5, name: 5 }, { id: 10, name: 10 }, { id: 20, name: 20 }, { id: 50, name: 50 }, { id: 100, name: 100 }, { id: 200, name: 200 }],
        Initialize: function () {
            $scope.Eproms.PageParams.FetchRecords = 10;
            var obj = $window.localStorage.getItem("EpromspageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.Eproms.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("EpromspageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.Eproms.CurrentIndex = pageid;
            }

            $scope.Eproms.Methods.GetEpromsSearch($scope.Eproms.CurrentIndex);
        },
        Methods: {
            getAllSelectedCheckBox: function () {
                var selectedItems = $scope.Eproms.Eproms.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.Eproms.EpromsID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Eproms.EpromsID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Eproms.EpromsID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.Eproms.Eproms.length;
            },
            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.Eproms.Eproms, function (item) {
                    item.Selected = value;

                    var index = $scope.Eproms.EpromsID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Eproms.EpromsID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Eproms.EpromsID.splice(index, 1);
                    }
                });
            },
            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.Eproms.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.Eproms.Methods.getAllSelectedCheckBox();
                }
            },
            SwitchStatus: function (id, status) {
                try {
                    $scope.Eproms.EpromsID[0] = id;
                    $scope.Eproms.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },
            ChangeStatusBySwitch: function (status) {
                $scope.Eproms.Services.UpdateStatus(status);
            },
            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.Eproms.EpromsID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.Eproms.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.Eproms.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.Eproms.Services.DeleteMultiple();
                        }
                        $scope.Eproms.EpromsID = [];
                    }
                }
            },
            RedirecttoCreate: function () {
                $window.location.href = '/Eproms/Create';
            },
            RedirecttoList: function () {
                $window.location.href = '/Eproms/Index';
            },
            GetEpromsSearch: function (currentIndex) {
                $scope.Eproms.IsLoading = true;
                var isactive = null;
                if ($scope.Eproms.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.Eproms.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }
                $window.localStorage.setItem("EpromspageObject", JSON.stringify($scope.Eproms.PageParams));
                $window.localStorage.setItem("EpromspageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.Eproms.PageNo = 1;
                }
                else {
                    $scope.Eproms.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }
                $scope.Eproms.PageParams.StartIndex = (currentIndex * $scope.Eproms.PageParams.FetchRecords);
                $scope.Eproms.PageParams.EndIndex = (parseInt($scope.Eproms.PageParams.StartIndex) + parseInt($scope.Eproms.PageParams.FetchRecords));

                $scope.Eproms.IsLoading = true;

                $http.get('/api/eproms/GetSurveySearch?SearchString=' + $scope.Eproms.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.Eproms.PageParams.StartIndex + "&EndIndex=" + $scope.Eproms.PageParams.EndIndex).success(function (response) {

                    $scope.Eproms.IsLoading = false;

                    var responseData = JSON.parse(response);
                    $scope.Eproms.PageParams.TotalCount = responseData.TotalCount;
                    $scope.Eproms.Eproms = responseData.SurveySearchFilterList;

                    $scope.Eproms.PageParams.CurrentStartIndex = $scope.Eproms.PageParams.StartIndex;

                    if ($scope.Eproms.PageParams.TotalCount < $scope.Eproms.PageParams.EndIndex) {
                        $scope.Eproms.PageParams.CurrentEndIndex = $scope.Eproms.PageParams.TotalCount;
                    }
                    else {
                        $scope.Eproms.PageParams.CurrentEndIndex = $scope.Eproms.PageParams.EndIndex;
                    }

                    $scope.Eproms.Buttons = [];

                    for (var i = 0; i < $scope.Eproms.PageParams.TotalCount / $scope.Eproms.PageParams.FetchRecords; i++) {
                        $scope.Eproms.Buttons.push({ currentindex: (i + 1) });
                    }

                    $scope.Eproms.Methods.ShowMessage();

                    $scope.Eproms.IsLoading = false;
                });
            },

            ShowMessage: function () {
                var Message = $window.localStorage.getItem("Message");
                if (Message != null && Message != "") {
                    if (Message == "Child Record Exists" || Message == "ePROM already in Use") {
                        toaster.pop('info', '', Message + '.');
                    }
                    else {
                        toaster.pop('success', '', Message);
                    }
                    $window.localStorage.setItem("Message", "");
                }
            },

            GetEpromsById: function (id) {
                return $scope.Eproms.Services.GetById(id);
            },

            GetEpromsById_Time: function (id) {
                $scope.Eproms.isLoad = false;
                $scope.Eproms.isEdit = true;
                $scope.Eproms.IsLoading = true;
                $scope.Eproms.SID = id;

                $http.get('/api/eproms/getDays?SurveyID=' + id).success(function (response) {
                    $scope.Eproms.days = response;

                    $scope.Eproms.IsLoading = false;
                })
            },

            SetEditData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/Eproms/Edit?id=' + data.SurveyID;
            },

            OpenDeleteDialog: function (id) {
                $scope.Eproms.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.Eproms.ID;
                           $scope.Eproms.Services.Delete(id);
                       });
            },
            OpenUploadModal: function (epromId) {
                $scope.Eproms.SurveyID = epromId;
                $("#UploadCsvModal").modal('show');
            },
            UploadCsvFile: function (e) {
                if ($("#UploadCsvFile")[0].files.length > 0) {
                    for (var i = 0; i < $("#UploadCsvFile")[0].files.length; i++) {
                        $scope.Eproms.files = [];
                        $scope.Eproms.files.push($("#UploadCsvFile")[0].files[0])
                    }
                    $scope.Eproms.Services.UploadCsvFile($scope.Eproms.files, $scope.Eproms.SurveyID);
                }
                else {
                    toaster.pop('warning', '', "Please choose a file!");
                }
            },
            Put: function () {
                var surveyID = $scope.Eproms.SID;
                var days = $scope.Eproms.days;

                $scope.Eproms.issubmitted = true;
                if ($scope.formDays.$valid) {
                    $scope.Eproms.IsLoading = true;

                    $http.post('/api/eproms/updateDays?SurveyID=' + surveyID + '&Days=' + days).success(function () {
                        $scope.Eproms.IsLoading = false;
                        
                        $scope.Eproms.Methods.Back();
                    });
                }
            },
            Back: function () {
                $scope.Eproms.isLoad = true;
                $scope.Eproms.isEdit = false;

                $scope.Eproms.days = 0;
            },
        },
        Services: {
            GetById: function (id) {
                $http.get('/api/eproms/GetSurveyById?id=' + id).success(function (response) {
                    $scope.Eproms.Methods.SetEditData(response);
                })
            },

            Delete: function (id) {
                $http.delete('/api/Eproms/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.Eproms.Methods.RedirecttoList();
                });
            },

            DeleteMultiple: function () {
                var Ids = $scope.Eproms.EpromsID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/Eproms/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.Eproms.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.Eproms.Methods.RedirecttoList();
                });
            },

            UpdateStatus: function (status) {
                var Ids = $scope.Eproms.EpromsID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/Eproms/UpdateStatus',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(","),
                        'Status': status
                    },
                }).success(function () {
                    var isactive = "";
                    if (status == true) {
                        isactive = "Activated!";
                    }
                    else {
                        isactive = "Deactivated!"
                    }
                    toaster.pop('success', '', "Status " + isactive);
                    $scope.Eproms.Action = "";
                    var index = $window.localStorage.getItem("EpromspageId");
                    $scope.Eproms.Methods.GetEpromsSearch(index);
                });
            },
            UploadCsvFile: function (data, SurveyID) {
                var fd = new FormData();
                for (var i = 0; i < data.length; i++) {
                    fd.append('file' + [i + 1], data[i]);
                }

                $http.post('/Eproms/UploadEpromFile', fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined, 'surveyId': SurveyID }
                }).success(function (data) {
                    if (data != "") {
                        $scope.Eproms.Services.UpdateSurveyFileName(data);
                    }
                    else {
                        toaster.pop('warning', '', "There is some issue. Try again later.");
                    }
                }).error(function (error) {
                });
            },
            UpdateSurveyFileName: function (FileName) {
                $http.post('/api/Eproms/UpdateSurveyFileName?Id=' + $scope.Eproms.SurveyID + '&FileName=' + FileName).success(function (data) {
                    if (data != "") {
                        toaster.pop('success', '', "File uploaded successfully!");
                    }
                    else {
                        toaster.pop('warning', '', "There is some issue. Try again later.");
                    }
                }).error(function (error) {
                });
            },
        }
    };

    $scope.Eproms.Initialize();

}]);