app.controller('CategoryController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.Category = {
        //visibleflag: true,
        allSelected: false,
        CheckAll: false,
        Check: [],
        Category: [],
        CategoryID: [],
        PageNo: 1,
        ID: 0,
        CurrentIndex: 0,
        issubmitted: false,
        IsLoading: false,
        Filter: "All",
        SubCategoryList: [],
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
            $scope.Category.PageParams.FetchRecords = 10;

            var obj = $window.localStorage.getItem("CatpageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.Category.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("CatpageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.Category.CurrentIndex = pageid;
            }

            $scope.Category.Methods.GetCategorySearch($scope.Category.CurrentIndex);
        },
        Methods: {
            getAllSelectedCheckBox: function () {
                var selectedItems = $scope.Category.Category.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.Category.CategoryID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Category.CategoryID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Category.CategoryID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.Category.Category.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.Category.Category, function (item) {
                    item.Selected = value;

                    var index = $scope.Category.CategoryID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Category.CategoryID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Category.CategoryID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.Category.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.Category.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.Category.CategoryID[0] = id;
                    $scope.Category.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },

            ChangeStatusBySwitch: function (status) {
                $scope.Category.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.Category.CategoryID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.Category.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.Category.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.Category.Services.DeleteMultiple();
                        }
                        $scope.Category.CategoryID = [];
                    }
                }
            },

            OpenDeleteDialog: function (id) {
                $scope.Category.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.Category.ID;
                           $scope.Category.Services.Delete(id);
                       });
            },

            RedirecttoCreate: function () {
                $window.location.href = '/Category/Create';
            },

            RedirecttoList: function () {
                $window.location.href = '/Category/Index';
            },

            SetCategoryData: function (data) {
                $scope.Category.Category = JSON.parse(data);
            },

            GetCategoryById: function (id) {
                return $scope.Category.Services.GetById(id);
            },

            SetEditCategoryData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/Category/Edit?id=' + data.ID;
            },

            Delete: function (id) {
                return $scope.Category.Services.Delete(id);
            },

            GetSubCategoryById: function (Id) {
                return $scope.Category.Services.GetSubCategoryById(Id);
            },
            SetSubCategoryData: function (data) {
                $scope.Category.SubCategoryList = JSON.parse(data);
            },
            GetCategorySearch: function (currentIndex) {
                var isactive = null;
                if ($scope.Category.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.Category.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("CatpageObject", JSON.stringify($scope.Category.PageParams));
                $window.localStorage.setItem("CatpageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.Category.PageNo = 1;
                }
                else {
                    $scope.Category.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }

                $scope.Category.PageParams.StartIndex = (currentIndex * $scope.Category.PageParams.FetchRecords);
                $scope.Category.PageParams.EndIndex = (parseInt($scope.Category.PageParams.StartIndex) + parseInt($scope.Category.PageParams.FetchRecords));

                $scope.Category.IsLoading = true;
                $http.get('/api/category/GetCategorySearch?SearchString=' + $scope.Category.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.Category.PageParams.StartIndex + "&EndIndex=" + $scope.Category.PageParams.EndIndex).success(function (response) {

                    $scope.Category.IsLoading = false;

                    var responseData = JSON.parse(response);
                    $scope.Category.PageParams.TotalCount = responseData.TotalCount;
                    $scope.Category.Category = responseData.SurveyCategorySearchFilterList;

                    $scope.Category.PageParams.CurrentStartIndex = $scope.Category.PageParams.StartIndex;

                    if ($scope.Category.PageParams.TotalCount < $scope.Category.PageParams.EndIndex) {
                        $scope.Category.PageParams.CurrentEndIndex = $scope.Category.PageParams.TotalCount;
                    }
                    else {
                        $scope.Category.PageParams.CurrentEndIndex = $scope.Category.PageParams.EndIndex;
                    }

                    $scope.Category.Buttons = [];

                    for (var i = 0; i < $scope.Category.PageParams.TotalCount / $scope.Category.PageParams.FetchRecords; i++) {
                        $scope.Category.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.Category.Methods.ShowMessage();
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
                $http.get('/api/category/GetById/' + id).success($scope.Category.Methods.SetEditCategoryData)
            },
            GetByStatus: function (status) {
                $http.get('/api/category/GetByStatus?status=' + status).success($scope.Category.Methods.SetCategoryData)
            },

            UpdateStatus: function (status) {
                var Ids = $scope.Category.CategoryID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/category/UpdateStatus',
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
                    $scope.Category.Action = "";
                    var index = $window.localStorage.getItem("CatpageId");
                    $scope.Category.Methods.GetCategorySearch(index);
                });
            },

            Delete: function (id) {
                $http.delete('/api/category/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.Category.Methods.RedirecttoList();
                });
            },
            DeleteMultiple: function () {
                var Ids = $scope.Category.CategoryID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/category/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.Category.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.Category.Methods.RedirecttoList();
                });
            },
            GetSubCategoryById: function (id) {
                $http.get('/api/category/GetSubCategoryById/' + id).success($scope.Category.Methods.SetSubCategoryData)
            }
        }
    };

    $scope.Category.Initialize();
}]);