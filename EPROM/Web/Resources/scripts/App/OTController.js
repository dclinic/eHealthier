app.controller('OTController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.OTs = {
        allSelected: false,
        CheckAll: false,
        Check: [],
        OTs: [],
        OTID: [],
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
            $scope.OTs.PageParams.FetchRecords = 10;

            var obj = $window.localStorage.getItem("OTpageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.OTs.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("OTpageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.OTs.CurrentIndex = pageid;
            }

            $scope.OTs.Methods.GetOTSearch($scope.OTs.CurrentIndex);
        },
        Methods: {
            getAllSelectedCheckBox: function () {              
                var selectedItems = $scope.OTs.OTs.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.OTs.OTID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.OTs.OTID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.OTs.OTID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.OTs.OTs.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.OTs.OTs, function (item) {
                    item.Selected = value;

                    var index = $scope.OTs.OTID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.OTs.OTID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.OTs.OTID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.OTs.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.OTs.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.OTs.OTID[0] = id;
                    $scope.OTs.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },

            ChangeStatusBySwitch: function (status) {
                $scope.OTs.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.OTs.OTID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.OTs.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.OTs.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.OTs.Services.DeleteMultiple();
                        }
                        $scope.OTs.OTID = [];
                    }
                }
            },

            OpenDeleteDialog: function (id) {
                $scope.OTs.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.OTs.ID;
                           $scope.OTs.Services.Delete(id);
                       });
            },

            RedirecttoCreate: function () {
                $window.location.href = '/OrganizationType/Create';
            },

            RedirecttoList: function () {
                $window.location.href = '/OrganizationType/Index';
            },

            SetOTData: function (data) {
                $scope.OTs.OTs = JSON.parse(data);
            },

            GetOTById: function (id) {
                return $scope.OTs.Services.GetById(id);
            },

            SetEditOTData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/OrganizationType/Edit?id=' + data.ID;
            },

            Delete: function (id) {
                return $scope.OTs.Services.Delete(id);
            },

            GetOTSearch: function (currentIndex) {
                var isactive = null;
                if ($scope.OTs.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.OTs.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("OTpageObject", JSON.stringify($scope.OTs.PageParams));
                $window.localStorage.setItem("OTpageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.OTs.PageNo = 1;
                }
                else {
                    $scope.OTs.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }

                $scope.OTs.PageParams.StartIndex = (currentIndex * $scope.OTs.PageParams.FetchRecords);
                $scope.OTs.PageParams.EndIndex = (parseInt($scope.OTs.PageParams.StartIndex) + parseInt($scope.OTs.PageParams.FetchRecords));

                $scope.OTs.IsLoading = true;
                $http.get('/api/OrganizationType/GetOTSearch?SearchString=' + $scope.OTs.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.OTs.PageParams.StartIndex + "&EndIndex=" + $scope.OTs.PageParams.EndIndex).success(function (response) {

                    $scope.OTs.IsLoading = false;

                    var responseData = JSON.parse(response);

                    console.log(responseData);

                    $scope.OTs.PageParams.TotalCount = responseData.TotalCount;
                    $scope.OTs.OTs = responseData.OTSearchFilterList;

                    $scope.OTs.PageParams.CurrentStartIndex = $scope.OTs.PageParams.StartIndex;

                    if ($scope.OTs.PageParams.TotalCount < $scope.OTs.PageParams.EndIndex) {
                        $scope.OTs.PageParams.CurrentEndIndex = $scope.OTs.PageParams.TotalCount;
                    }
                    else {
                        $scope.OTs.PageParams.CurrentEndIndex = $scope.OTs.PageParams.EndIndex;
                    }

                    $scope.OTs.Buttons = [];

                    for (var i = 0; i < $scope.OTs.PageParams.TotalCount / $scope.OTs.PageParams.FetchRecords; i++) {
                        $scope.OTs.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.OTs.Methods.ShowMessage();
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
                $http.get('/api/OrganizationType/GetById/' + id).success($scope.OTs.Methods.SetEditOTData)
            },
            GetByStatus: function (status) {
                $http.get('/api/OrganizationType/GetByStatus?status=' + status).success($scope.OTs.Methods.SetOTData)
            },

            UpdateStatus: function (status) {
                var Ids = $scope.OTs.OTID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/OrganizationType/UpdateStatus',
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
                    $scope.OTs.Action = "";
                    var index = $window.localStorage.getItem("OTpageId");
                    $scope.OTs.Methods.GetOTSearch(index);
                });
            },

            Delete: function (id) {
                $http.delete('/api/OrganizationType/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.OTs.Methods.RedirecttoList();
                });
            },
            DeleteMultiple: function () {
                var Ids = $scope.OTs.OTID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/OrganizationType/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.OTs.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.OTs.Methods.RedirecttoList();
                });
            },

        }
    };

    $scope.OTs.Initialize();
}]);