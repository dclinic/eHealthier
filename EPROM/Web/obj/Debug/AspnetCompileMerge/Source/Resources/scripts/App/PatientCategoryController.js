app.controller('PatientCategoryController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.PatientCategory = {
        PatientCategory: [],
        allSelected: false,
        CheckAll: false,
        Check: [],
        PatientCategoryID: [],
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
            $scope.PatientCategory.PageParams.FetchRecords = 10;

            var obj = $window.localStorage.getItem("PatientCatpageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.PatientCategory.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("PatientCatpageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.PatientCategory.CurrentIndex = pageid;
            }

            $scope.PatientCategory.Methods.GetCategorySearch($scope.PatientCategory.CurrentIndex);
        },
        Methods: {
            getAllSelectedCheckBox: function () {
                var selectedItems = $scope.PatientCategory.PatientCategory.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.PatientCategory.PatientCategoryID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.PatientCategory.PatientCategoryID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.PatientCategory.PatientCategoryID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.PatientCategory.PatientCategory.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.PatientCategory.PatientCategory, function (item) {
                    item.Selected = value;

                    var index = $scope.PatientCategory.PatientCategoryID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.PatientCategory.PatientCategoryID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.PatientCategory.PatientCategoryID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.PatientCategory.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.PatientCategory.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.PatientCategory.PatientCategoryID[0] = id;
                    $scope.PatientCategory.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },

            ChangeStatusBySwitch: function (status) {
                $scope.PatientCategory.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.PatientCategory.PatientCategoryID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.PatientCategory.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.PatientCategory.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.PatientCategory.Services.DeleteMultiple();
                        }
                        $scope.PatientCategory.PatientCategoryID = [];
                    }
                }
            },

            OpenDeleteDialog: function (id) {
                $scope.PatientCategory.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.PatientCategory.ID;
                           $scope.PatientCategory.Services.Delete(id);
                       });
            },

            RedirecttoCreate: function () {
                $window.location.href = '/PatientCategory/Create';
            },

            RedirecttoList: function () {
                $window.location.href = '/PatientCategory/Index';
            },

            SetCategoryData: function (data) {
                $scope.PatientCategory.PatientCategory = JSON.parse(data);
            },

            GetCategoryById: function (id) {
                return $scope.PatientCategory.Services.GetById(id);
            },

            SetEditCategoryData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/PatientCategory/Edit?id=' + data.ID;
            },

            Delete: function (id) {
                return $scope.PatientCategory.Services.Delete(id);
            },
            GetCategorySearch: function (currentIndex) {
                var isactive = null;
                if ($scope.PatientCategory.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.PatientCategory.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("PatientCatpageObject", JSON.stringify($scope.PatientCategory.PageParams));
                $window.localStorage.setItem("PatientCatpageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.PatientCategory.PageNo = 1;
                }
                else {
                    $scope.PatientCategory.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }

                $scope.PatientCategory.PageParams.StartIndex = (currentIndex * $scope.PatientCategory.PageParams.FetchRecords);
                $scope.PatientCategory.PageParams.EndIndex = (parseInt($scope.PatientCategory.PageParams.StartIndex) + parseInt($scope.PatientCategory.PageParams.FetchRecords));

                $scope.PatientCategory.IsLoading = true;
                $http.get('/api/PatientCategory/GetPatientCategorySearch?SearchString=' + $scope.PatientCategory.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.PatientCategory.PageParams.StartIndex + "&EndIndex=" + $scope.PatientCategory.PageParams.EndIndex).success(function (response) {

                    $scope.PatientCategory.IsLoading = false;

                    var responseData = JSON.parse(response);
                    $scope.PatientCategory.PageParams.TotalCount = responseData.TotalCount;
                    $scope.PatientCategory.PatientCategory = responseData.PatientCategorySearchFilterList;

                    $scope.PatientCategory.PageParams.CurrentStartIndex = $scope.PatientCategory.PageParams.StartIndex;

                    if ($scope.PatientCategory.PageParams.TotalCount < $scope.PatientCategory.PageParams.EndIndex) {
                        $scope.PatientCategory.PageParams.CurrentEndIndex = $scope.PatientCategory.PageParams.TotalCount;
                    }
                    else {
                        $scope.PatientCategory.PageParams.CurrentEndIndex = $scope.PatientCategory.PageParams.EndIndex;
                    }

                    $scope.PatientCategory.Buttons = [];

                    for (var i = 0; i < $scope.PatientCategory.PageParams.TotalCount / $scope.PatientCategory.PageParams.FetchRecords; i++) {
                        $scope.PatientCategory.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.PatientCategory.Methods.ShowMessage();
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
                $http.get('/api/PatientCategory/GetById/' + id).success($scope.PatientCategory.Methods.SetEditCategoryData)
            },
            GetByStatus: function (status) {
                $http.get('/api/PatientCategory/GetByStatus?status=' + status).success($scope.PatientCategory.Methods.SetCategoryData)
            },

            UpdateStatus: function (status) {
                var Ids = $scope.PatientCategory.PatientCategoryID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/PatientCategory/UpdateStatus',
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
                    $scope.PatientCategory.Action = "";
                    var index = $window.localStorage.getItem("PatientCatpageId");
                    $scope.PatientCategory.Methods.GetCategorySearch(index);
                });
            },

            Delete: function (id) {
                $http.delete('/api/PatientCategory/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.PatientCategory.Methods.RedirecttoList();
                });
            },
            DeleteMultiple: function () {
                var Ids = $scope.PatientCategory.PatientCategoryID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/PatientCategory/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.PatientCategory.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.PatientCategory.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.PatientCategory.Initialize();
}]);