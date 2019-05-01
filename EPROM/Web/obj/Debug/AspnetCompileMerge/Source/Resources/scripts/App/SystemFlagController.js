app.controller('SystemFlagController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.SystemFlag = {
        allSelected: false,
        CheckAll: false,
        Check: [],
        SystemFlag: [],
        SystemFlagID: [],
        PageNo: 1,
        ID: 0,
        CurrentIndex: 0,
        issubmitted: false,
        IsLoading: false,
        Filter: "All",
        FlagGroupName: "",
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
            $scope.SystemFlag.PageParams.FetchRecords = 10;
            var obj = $window.localStorage.getItem("SystemFlagpageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.SystemFlag.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("SystemFlagpageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.SystemFlag.CurrentIndex = pageid;
            }

            $scope.SystemFlag.Methods.GetSystemFlagSearch($scope.SystemFlag.CurrentIndex);
        },
        Methods: {

            getAllSelectedCheckBox: function () {
                var selectedItems = $scope.SystemFlag.SystemFlag.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.SystemFlag.SystemFlagID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.SystemFlag.SystemFlagID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.SystemFlag.SystemFlagID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.SystemFlag.SystemFlag.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.SystemFlag.SystemFlag, function (item) {
                    item.Selected = value;

                    var index = $scope.SystemFlag.SystemFlagID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.SystemFlag.SystemFlagID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.SystemFlag.SystemFlagID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.SystemFlag.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.SystemFlag.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.SystemFlag.SystemFlagID[0] = id;
                    $scope.SystemFlag.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },
            ChangeStatusBySwitch: function (status) {
                $scope.SystemFlag.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.SystemFlag.SystemFlagID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.SystemFlag.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.SystemFlag.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.SystemFlag.Services.DeleteMultiple();
                        }
                        $scope.SystemFlag.SystemFlagID = [];
                    }
                }
            },

            RedirecttoCreate: function () {
                $window.location.href = '/SystemFlag/Create';
            },

            RedirecttoList: function () {
                $window.location.href = '/SystemFlag/Index';
            },

            GetSystemFlagSearch: function (currentIndex) {
                var isactive = null;
                if ($scope.SystemFlag.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.SystemFlag.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("SystemFlagpageObject", JSON.stringify($scope.SystemFlag.PageParams));
                $window.localStorage.setItem("SystemFlagpageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.SystemFlag.PageNo = 1;
                }
                else {
                    $scope.SystemFlag.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }
                $scope.SystemFlag.PageParams.StartIndex = (currentIndex * $scope.SystemFlag.PageParams.FetchRecords);
                $scope.SystemFlag.PageParams.EndIndex = (parseInt($scope.SystemFlag.PageParams.StartIndex) + parseInt($scope.SystemFlag.PageParams.FetchRecords));

                $scope.SystemFlag.IsLoading = true;

                $http.get('/api/SystemFlag/GetSystemFlagSearch?SearchString=' + $scope.SystemFlag.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.SystemFlag.PageParams.StartIndex + "&EndIndex=" + $scope.SystemFlag.PageParams.EndIndex).success(function (response) {

                    $scope.SystemFlag.IsLoading = false;

                    var responseData = JSON.parse(response);
                    $scope.SystemFlag.PageParams.TotalCount = responseData.TotalCount;
                    $scope.SystemFlag.SystemFlag = responseData.SystemFlagSearchFilterList;

                    $scope.SystemFlag.PageParams.CurrentStartIndex = $scope.SystemFlag.PageParams.StartIndex;

                    if ($scope.SystemFlag.PageParams.TotalCount < $scope.SystemFlag.PageParams.EndIndex) {
                        $scope.SystemFlag.PageParams.CurrentEndIndex = $scope.SystemFlag.PageParams.TotalCount;
                    }
                    else {
                        $scope.SystemFlag.PageParams.CurrentEndIndex = $scope.SystemFlag.PageParams.EndIndex;
                    }

                    $scope.SystemFlag.Buttons = [];

                    for (var i = 0; i < $scope.SystemFlag.PageParams.TotalCount / $scope.SystemFlag.PageParams.FetchRecords; i++) {
                        $scope.SystemFlag.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.SystemFlag.Methods.ShowMessage();
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

            GetSystemFlagById: function (id) {
                return $scope.SystemFlag.Services.GetById(id);
            },

            SetEditData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/SystemFlag/Edit?id=' + data.ID;
            },

            OpenDeleteDialog: function (id) {
                $scope.SystemFlag.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.SystemFlag.ID;
                           $scope.SystemFlag.Services.Delete(id);
                       });
            },
            GetFlagGroupById: function (FlagGroupID) {
                return $scope.SystemFlag.Services.GetFlagGroupById(FlagGroupID);
            }

        },
        Services: {
            GetById: function (id) {
                $http.get('/api/SystemFlag/GetById/' + id).success($scope.SystemFlag.Methods.SetEditData)
            },

            Delete: function (id) {
                $http.delete('/api/SystemFlag/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.SystemFlag.Methods.RedirecttoList();
                });
            },

            DeleteMultiple: function () {
                var Ids = $scope.SystemFlag.SystemFlagID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/SystemFlag/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.SystemFlag.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.SystemFlag.Methods.RedirecttoList();
                });
            },

            UpdateStatus: function (status) {
                var Ids = $scope.SystemFlag.SystemFlagID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/SystemFlag/UpdateStatus',
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
                    $scope.SystemFlag.Action = "";
                    var index = $window.localStorage.getItem("CatpageId");
                    $scope.SystemFlag.Methods.GetSystemFlagSearch(index);
                });
            },

            GetFlagGroupById: function (id) {
                $http.get('/api/FlagGroup/GetById?Id=' + id).success(function (response) {
                    var data = JSON.parse(response);
                    $scope.SystemFlag.FlagGroupName = data.FlagGroupName;
                }).error(function (data, status, headers, config) {
                    console.log(data);
                });
            }
        }
    };

    $scope.SystemFlag.Initialize();

}]);