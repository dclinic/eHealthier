app.controller('FlagGroupController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.FlagGroup = {
        allSelected: false,
        CheckAll: false,
        Check: [],
        FlagGroup: [],
        FlagGroupID: [],
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
            $scope.FlagGroup.PageParams.FetchRecords = 10;
            var obj = $window.localStorage.getItem("FlagGrouppageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.FlagGroup.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("FlagGrouppageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.FlagGroup.CurrentIndex = pageid;
            }

            $scope.FlagGroup.Methods.GetFlagGroupSearch($scope.FlagGroup.CurrentIndex);
        },
        Methods: {

            getAllSelectedCheckBox: function () {
                var selectedItems = $scope.FlagGroup.FlagGroup.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.FlagGroup.FlagGroupID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.FlagGroup.FlagGroupID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.FlagGroup.FlagGroupID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.FlagGroup.FlagGroup.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.FlagGroup.FlagGroup, function (item) {
                    item.Selected = value;

                    var index = $scope.FlagGroup.FlagGroupID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.FlagGroup.FlagGroupID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.FlagGroup.FlagGroupID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.FlagGroup.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.FlagGroup.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.FlagGroup.FlagGroupID[0] = id;
                    $scope.FlagGroup.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },
            ChangeStatusBySwitch: function (status) {
                $scope.FlagGroup.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.FlagGroup.FlagGroupID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.FlagGroup.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.FlagGroup.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.FlagGroup.Services.DeleteMultiple();
                        }
                        $scope.FlagGroup.FlagGroupID = [];
                    }
                }
            },

            RedirecttoCreate: function () {
                $window.location.href = '/FlagGroup/Create';
            },

            RedirecttoList: function () {
                $window.location.href = '/FlagGroup/Index';
            },

            GetFlagGroupSearch: function (currentIndex) {
                var isactive = null;
                if ($scope.FlagGroup.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.FlagGroup.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("FlagGrouppageObject", JSON.stringify($scope.FlagGroup.PageParams));
                $window.localStorage.setItem("FlagGrouppageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.FlagGroup.PageNo = 1;
                }
                else {
                    $scope.FlagGroup.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }
                $scope.FlagGroup.PageParams.StartIndex = (currentIndex * $scope.FlagGroup.PageParams.FetchRecords);
                $scope.FlagGroup.PageParams.EndIndex = (parseInt($scope.FlagGroup.PageParams.StartIndex) + parseInt($scope.FlagGroup.PageParams.FetchRecords));

                $scope.FlagGroup.IsLoading = true;

                $http.get('/api/FlagGroup/GetFlagGroupSearch?SearchString=' + $scope.FlagGroup.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.FlagGroup.PageParams.StartIndex + "&EndIndex=" + $scope.FlagGroup.PageParams.EndIndex).success(function (response) {

                    $scope.FlagGroup.IsLoading = false;

                    var responseData = JSON.parse(response);
                    $scope.FlagGroup.PageParams.TotalCount = responseData.TotalCount;
                    $scope.FlagGroup.FlagGroup = responseData.FlagGroupSearchFilterList;

                    $scope.FlagGroup.PageParams.CurrentStartIndex = $scope.FlagGroup.PageParams.StartIndex;

                    if ($scope.FlagGroup.PageParams.TotalCount < $scope.FlagGroup.PageParams.EndIndex) {
                        $scope.FlagGroup.PageParams.CurrentEndIndex = $scope.FlagGroup.PageParams.TotalCount;
                    }
                    else {
                        $scope.FlagGroup.PageParams.CurrentEndIndex = $scope.FlagGroup.PageParams.EndIndex;
                    }

                    $scope.FlagGroup.Buttons = [];

                    for (var i = 0; i < $scope.FlagGroup.PageParams.TotalCount / $scope.FlagGroup.PageParams.FetchRecords; i++) {
                        $scope.FlagGroup.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.FlagGroup.Methods.ShowMessage();
                });
            },

            ShowMessage: function () {
                var Message = $window.localStorage.getItem("Message");
                if (Message != null && Message != "") {
                    if (Message == "Child Record Exists" || Message == "Already in used") {
                        toaster.pop('info', '', Message + '.');
                    } else {
                        toaster.pop('success', '', Message);
                    }
                    $window.localStorage.setItem("Message", "");
                }
            },

            GetFlagGroupById: function (id) {
                return $scope.FlagGroup.Services.GetById(id);
            },

            SetEditCategoryData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/FlagGroup/Edit?id=' + data.ID;
            },

            OpenDeleteDialog: function (id) {
                $scope.FlagGroup.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.FlagGroup.ID;
                           $scope.FlagGroup.Services.Delete(id);
                       });
            },
            //GetFlagGroupById: function (FlagGroupID) {
            //    return $scope.FlagGroup.Services.GetFlagGroupById(FlagGroupID);
            //}
        },
        Services: {
            GetById: function (id) {
                $http.get('/api/FlagGroup/GetById/' + id).success($scope.FlagGroup.Methods.SetEditCategoryData)
            },

            Delete: function (id) {
                $http.delete('/api/FlagGroup/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.FlagGroup.Methods.RedirecttoList();
                });
            },

            DeleteMultiple: function () {
                var Ids = $scope.FlagGroup.FlagGroupID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/FlagGroup/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.FlagGroup.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.FlagGroup.Methods.RedirecttoList();
                });
            },

            UpdateStatus: function (status) {
                var Ids = $scope.FlagGroup.FlagGroupID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/FlagGroup/UpdateStatus',
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
                    $scope.FlagGroup.Action = "";
                    var index = $window.localStorage.getItem("CatpageId");
                    $scope.FlagGroup.Methods.GetFlagGroupSearch(index);
                });
            },

            GetFlagGroupById: function (id) {
                $http.get('/api/FlagGroup/GetById?Id=' + id).success(function (response) {
                    var data = JSON.parse(response);
                    $scope.FlagGroup.FlagGroupName = data.FlagGroupName;
                }).error(function (data, status, headers, config) {
                    console.log(data);
                });
            }
        }
    };

    $scope.FlagGroup.Initialize();

}]);