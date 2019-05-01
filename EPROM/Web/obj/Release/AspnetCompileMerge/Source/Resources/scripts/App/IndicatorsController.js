app.controller('IndicatorsController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.Indicators = {
        allSelected: false,
        CheckAll: false,
        Check: [],
        Indicators: [],
        IndicatorID: [],
        PageNo: 1,
        ID: 0,
        CurrentIndex: 0,
        issubmitted: false,
        IsLoading: false,
        Filter: "All",
        PageParams: {
            TotalCount: 0,
            StartIndex: 0,
            EndIndex: 2,
            CurrentStartIndex: 0,
            CurrentEndIndex: 0,
            FetchRecords: 10
        },
        PageSize: [{ id: 5, name: 5 }, { id: 10, name: 10 }, { id: 20, name: 20 }, { id: 50, name: 50 }, { id: 100, name: 100 }, { id: 200, name: 200 }],

        Initialize: function () {
            $scope.Indicators.PageParams.FetchRecords = 10;

            var obj = $window.localStorage.getItem("IndicatorpageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.Indicators.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("IndicatorpageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.Indicators.CurrentIndex = pageid;
            }

            $scope.Indicators.Methods.GetIndicatorSearch($scope.Indicators.CurrentIndex);
        },
        Methods: {
            getAllSelectedCheckBox: function () {              
                var selectedItems = $scope.Indicators.Indicators.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.Indicators.IndicatorID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Indicators.IndicatorID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Indicators.IndicatorID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.Indicators.Indicators.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.Indicators.Indicators, function (item) {
                    item.Selected = value;

                    var index = $scope.Indicators.IndicatorID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Indicators.IndicatorID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Indicators.IndicatorID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.Indicators.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.Indicators.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.Indicators.IndicatorID[0] = id;
                    $scope.Indicators.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },

            ChangeStatusBySwitch: function (status) {
                $scope.Indicators.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.Indicators.IndicatorID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.Indicators.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.Indicators.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.Indicators.Services.DeleteMultiple();
                        }
                        $scope.Indicators.IndicatorID = [];
                    }
                }
            },

            OpenDeleteDialog: function (id) {
                $scope.Indicators.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.Indicators.ID;
                           $scope.Indicators.Services.Delete(id);
                       });
            },

            RedirecttoCreate: function () {
                $window.location.href = '/Indicators/Create';
            },

            RedirecttoList: function () {
                $window.location.href = '/Indicators/Index';
            },

            SetIndicatorData: function (data) {
                $scope.Indicators.Indicators = JSON.parse(data);
            },

            GetIndicatorById: function (id) {
                return $scope.Indicators.Services.GetById(id);
            },

            SetEditIndicatorData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/Indicators/Edit?id=' + data.ID;
            },

            Delete: function (id) {
                return $scope.Indicators.Services.Delete(id);
            },

            GetIndicatorSearch: function (currentIndex) {
                var isactive = null;
                if ($scope.Indicators.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.Indicators.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("IndicatorpageObject", JSON.stringify($scope.Indicators.PageParams));
                $window.localStorage.setItem("IndicatorpageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.Indicators.PageNo = 1;
                }
                else {
                    $scope.Indicators.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }

                $scope.Indicators.PageParams.StartIndex = (currentIndex * $scope.Indicators.PageParams.FetchRecords);
                $scope.Indicators.PageParams.EndIndex = (parseInt($scope.Indicators.PageParams.StartIndex) + parseInt($scope.Indicators.PageParams.FetchRecords));

                $scope.Indicators.IsLoading = true;
                $http.get('/api/Indicators/GetIndicatorSearch?SearchString=' + $scope.Indicators.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.Indicators.PageParams.StartIndex + "&EndIndex=" + $scope.Indicators.PageParams.EndIndex).success(function (response) {

                    $scope.Indicators.IsLoading = false;

                    var responseData = JSON.parse(response);
                    $scope.Indicators.PageParams.TotalCount = responseData.TotalCount;
                    $scope.Indicators.Indicators = responseData.IndicatorsSearchFilterList;

                    $scope.Indicators.PageParams.CurrentStartIndex = $scope.Indicators.PageParams.StartIndex;

                    if ($scope.Indicators.PageParams.TotalCount < $scope.Indicators.PageParams.EndIndex) {
                        $scope.Indicators.PageParams.CurrentEndIndex = $scope.Indicators.PageParams.TotalCount;
                    }
                    else {
                        $scope.Indicators.PageParams.CurrentEndIndex = $scope.Indicators.PageParams.EndIndex;
                    }

                    $scope.Indicators.Buttons = [];

                    for (var i = 0; i < $scope.Indicators.PageParams.TotalCount / $scope.Indicators.PageParams.FetchRecords; i++) {
                        $scope.Indicators.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.Indicators.Methods.ShowMessage();
                });
            },

            ShowMessage: function () {
                var Message = $window.localStorage.getItem("Message");
                if (Message != null && Message != "") {
                    if (Message == "Child Record Exists") {
                        toaster.pop('info', '', Message + '.');
                    }
                    else {
                        toaster.pop('success', '', Message);
                    }
                    $window.localStorage.setItem("Message", "");
                }
            },
        },
        Services: {

            GetById: function (id) {
                $http.get('/api/Indicators/GetById/' + id).success($scope.Indicators.Methods.SetEditIndicatorData)
            },
            GetByStatus: function (status) {
                $http.get('/api/Indicators/GetByStatus?status=' + status).success($scope.Indicators.Methods.SetIndicatorData)
            },

            UpdateStatus: function (status) {
                var Ids = $scope.Indicators.IndicatorID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/Indicators/UpdateStatus',
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
                    $scope.Indicators.Action = "";
                    var index = $window.localStorage.getItem("IndicatorpageId");
                    $scope.Indicators.Methods.GetIndicatorSearch(index);
                });
            },

            Delete: function (id) {
                $http.delete('/api/Indicators/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.Indicators.Methods.RedirecttoList();
                });
            },
            DeleteMultiple: function () {
                var Ids = $scope.Indicators.IndicatorID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/Indicators/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.Indicators.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.Indicators.Methods.RedirecttoList();
                });
            },

        }
    };

    $scope.Indicators.Initialize();
}]);