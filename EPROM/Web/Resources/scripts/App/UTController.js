app.controller('UTController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.UTs = {
        allSelected: false,
        CheckAll: false,
        Check: [],
        UTs: [],
        UTID: [],
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
            $scope.UTs.PageParams.FetchRecords = 10;

            var obj = $window.localStorage.getItem("UTpageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.UTs.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("UTpageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.UTs.CurrentIndex = pageid;
            }

            $scope.UTs.Methods.GetUTSearch($scope.UTs.CurrentIndex);
        },
        Methods: {
            getAllSelectedCheckBox: function () {
                var selectedItems = $scope.UTs.UTs.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.UTs.UTID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.UTs.UTID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.UTs.UTID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.UTs.UTs.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.UTs.UTs, function (item) {
                    item.Selected = value;

                    var index = $scope.UTs.UTID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.UTs.UTID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.UTs.UTID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.UTs.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.UTs.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.UTs.UTID[0] = id;
                    $scope.UTs.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },

            ChangeStatusBySwitch: function (status) {
                $scope.UTs.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.UTs.UTID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.UTs.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.UTs.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.UTs.Services.DeleteMultiple();
                        }
                        $scope.UTs.UTID = [];
                    }
                }
            },

            OpenDeleteDialog: function (id) {
                $scope.UTs.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.UTs.ID;
                           $scope.UTs.Services.Delete(id);
                       });
            },

            RedirecttoCreate: function () {
                $window.location.href = '/UserType/Create';
            },

            RedirecttoList: function () {
                $window.location.href = '/UserType/Index';
            },

            SetUTData: function (data) {
                $scope.UTs.UTs = JSON.parse(data);
            },

            GetUTById: function (id) {
                return $scope.UTs.Services.GetById(id);
            },

            SetEditUTData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/UserType/Edit?id=' + data.ID;
            },

            Delete: function (id) {
                return $scope.UTs.Services.Delete(id);
            },

            GetUTSearch: function (currentIndex) {
                var isactive = null;
                if ($scope.UTs.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.UTs.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("UTpageObject", JSON.stringify($scope.UTs.PageParams));
                $window.localStorage.setItem("UTpageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.UTs.PageNo = 1;
                }
                else {
                    $scope.UTs.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }

                $scope.UTs.PageParams.StartIndex = (currentIndex * $scope.UTs.PageParams.FetchRecords);
                $scope.UTs.PageParams.EndIndex = (parseInt($scope.UTs.PageParams.StartIndex) + parseInt($scope.UTs.PageParams.FetchRecords));

                $scope.UTs.IsLoading = true;
                $http.get('/api/UserType/GetUTSearch?SearchString=' + $scope.UTs.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.UTs.PageParams.StartIndex + "&EndIndex=" + $scope.UTs.PageParams.EndIndex).success(function (response) {

                    $scope.UTs.IsLoading = false;

                    var responseData = JSON.parse(response);

                    console.log(responseData);

                    $scope.UTs.PageParams.TotalCount = responseData.TotalCount;
                    $scope.UTs.UTs = responseData.UTSearchFilterList;

                    $scope.UTs.PageParams.CurrentStartIndex = $scope.UTs.PageParams.StartIndex;

                    if ($scope.UTs.PageParams.TotalCount < $scope.UTs.PageParams.EndIndex) {
                        $scope.UTs.PageParams.CurrentEndIndex = $scope.UTs.PageParams.TotalCount;
                    }
                    else {
                        $scope.UTs.PageParams.CurrentEndIndex = $scope.UTs.PageParams.EndIndex;
                    }

                    $scope.UTs.Buttons = [];

                    for (var i = 0; i < $scope.UTs.PageParams.TotalCount / $scope.UTs.PageParams.FetchRecords; i++) {
                        $scope.UTs.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.UTs.Methods.ShowMessage();
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
                $http.get('/api/UserType/GetById/' + id).success($scope.UTs.Methods.SetEditUTData)
            },
            GetByStatus: function (status) {
                $http.get('/api/UserType/GetByStatus?status=' + status).success($scope.UTs.Methods.SetUTData)
            },

            UpdateStatus: function (status) {
                var Ids = $scope.UTs.UTID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/UserType/UpdateStatus',
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
                    $scope.UTs.Action = "";
                    var index = $window.localStorage.getItem("UTpageId");
                    $scope.UTs.Methods.GetUTSearch(index);
                });
            },

            Delete: function (id) {
                $http.delete('/api/UserType/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.UTs.Methods.RedirecttoList();
                });
            },
            DeleteMultiple: function () {
                var Ids = $scope.UTs.UTID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/UserType/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.UTs.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.UTs.Methods.RedirecttoList();
                });
            },

        }
    };

    $scope.UTs.Initialize();
}]);