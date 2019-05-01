app.controller('ThirdPartyAppController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.ThirdPartyApp = {
        ThirdPartyApp: [],
        allSelected: false,
        CheckAll: false,
        Check: [],
        ThirdPartyAppID: [],
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
            $scope.ThirdPartyApp.PageParams.FetchRecords = 10;

            var obj = $window.localStorage.getItem("3rdPartyAppPageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.ThirdPartyApp.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("3rdPartyAppPageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.ThirdPartyApp.CurrentIndex = pageid;
            }

            $scope.ThirdPartyApp.Methods.GetThirdPartyAppSearch($scope.ThirdPartyApp.CurrentIndex);
        },
        Methods: {
            getAllSelectedCheckBox: function () {
                var selectedItems = $scope.ThirdPartyApp.ThirdPartyApp.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.ThirdPartyApp.ThirdPartyAppID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.ThirdPartyApp.ThirdPartyAppID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.ThirdPartyApp.ThirdPartyAppID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.ThirdPartyApp.ThirdPartyApp.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.ThirdPartyApp.ThirdPartyApp, function (item) {
                    item.Selected = value;

                    var index = $scope.ThirdPartyApp.ThirdPartyAppID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.ThirdPartyApp.ThirdPartyAppID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.ThirdPartyApp.ThirdPartyAppID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.ThirdPartyApp.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.ThirdPartyApp.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.ThirdPartyApp.ThirdPartyAppID[0] = id;
                    $scope.ThirdPartyApp.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },

            ChangeStatusBySwitch: function (status) {
                $scope.ThirdPartyApp.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.ThirdPartyApp.ThirdPartyAppID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.ThirdPartyApp.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.ThirdPartyApp.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.ThirdPartyApp.Services.DeleteMultiple();
                        }
                        $scope.ThirdPartyApp.ThirdPartyAppID = [];
                    }
                }
            },

            OpenDeleteDialog: function (id) {
                $scope.ThirdPartyApp.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.ThirdPartyApp.ID;
                           $scope.ThirdPartyApp.Services.Delete(id);
                       });
            },

            RedirecttoCreate: function () {
                $window.location.href = '/ThirdPartyApp/Create';
            },

            RedirecttoList: function () {
                $window.location.href = '/ThirdPartyApp/Index';
            },

            SetCategoryData: function (data) {
                $scope.ThirdPartyApp.ThirdPartyApp = JSON.parse(data);
            },

            GetThirdPartyAppById: function (id) {
                return $scope.ThirdPartyApp.Services.GetById(id);
            },

            SetEditAppData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/ThirdPartyApp/Edit?id=' + data.ID;
            },

            Delete: function (id) {
                return $scope.ThirdPartyApp.Services.Delete(id);
            },
            GetThirdPartyAppSearch: function (currentIndex) {
                var isactive = null;
                if ($scope.ThirdPartyApp.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.ThirdPartyApp.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("3rdPartyAppPageObject", JSON.stringify($scope.ThirdPartyApp.PageParams));
                $window.localStorage.setItem("3rdPartyAppPageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.ThirdPartyApp.PageNo = 1;
                }
                else {
                    $scope.ThirdPartyApp.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }

                $scope.ThirdPartyApp.PageParams.StartIndex = (currentIndex * $scope.ThirdPartyApp.PageParams.FetchRecords);
                $scope.ThirdPartyApp.PageParams.EndIndex = (parseInt($scope.ThirdPartyApp.PageParams.StartIndex) + parseInt($scope.ThirdPartyApp.PageParams.FetchRecords));

                $scope.ThirdPartyApp.IsLoading = true;
                $http.get('/api/ThirdPartyApp/GetThirdPartyAppSearch?SearchString=' + $scope.ThirdPartyApp.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.ThirdPartyApp.PageParams.StartIndex + "&EndIndex=" + $scope.ThirdPartyApp.PageParams.EndIndex).success(function (response) {
                    $scope.ThirdPartyApp.IsLoading = false;

                    var responseData = JSON.parse(response);
                    $scope.ThirdPartyApp.PageParams.TotalCount = responseData.TotalCount;
                    $scope.ThirdPartyApp.ThirdPartyApp = responseData.ThirdPartyAppSearchFilterList;

                    $scope.ThirdPartyApp.PageParams.CurrentStartIndex = $scope.ThirdPartyApp.PageParams.StartIndex;

                    if ($scope.ThirdPartyApp.PageParams.TotalCount < $scope.ThirdPartyApp.PageParams.EndIndex) {
                        $scope.ThirdPartyApp.PageParams.CurrentEndIndex = $scope.ThirdPartyApp.PageParams.TotalCount;
                    }
                    else {
                        $scope.ThirdPartyApp.PageParams.CurrentEndIndex = $scope.ThirdPartyApp.PageParams.EndIndex;
                    }

                    $scope.ThirdPartyApp.Buttons = [];

                    for (var i = 0; i < $scope.ThirdPartyApp.PageParams.TotalCount / $scope.ThirdPartyApp.PageParams.FetchRecords; i++) {
                        $scope.ThirdPartyApp.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.ThirdPartyApp.Methods.ShowMessage();
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
                $http.get('/api/ThirdPartyApp/GetById/' + id).success($scope.ThirdPartyApp.Methods.SetEditAppData)
            },
            GetByStatus: function (status) {
                $http.get('/api/ThirdPartyApp/GetByStatus?status=' + status).success($scope.ThirdPartyApp.Methods.SetCategoryData)
            },

            UpdateStatus: function (status) {
                var Ids = $scope.ThirdPartyApp.ThirdPartyAppID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/ThirdPartyApp/UpdateStatus',
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
                    $scope.ThirdPartyApp.Action = "";
                    var index = $window.localStorage.getItem("3rdPartyAppPageId");
                    $scope.ThirdPartyApp.Methods.GetThirdPartyAppSearch(index);
                });
            },

            Delete: function (id) {
                $http.delete('/api/ThirdPartyApp/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.ThirdPartyApp.Methods.RedirecttoList();
                });
            },
            DeleteMultiple: function () {
                var Ids = $scope.ThirdPartyApp.ThirdPartyAppID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/ThirdPartyApp/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.ThirdPartyApp.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.ThirdPartyApp.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.ThirdPartyApp.Initialize();
}]);