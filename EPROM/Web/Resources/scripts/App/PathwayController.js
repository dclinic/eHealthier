app.controller('PathwayController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.Pathway = {
        Pathway: [],
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
            $scope.Pathway.PageParams.FetchRecords = 10;

            var obj = $window.localStorage.getItem("PathwaypageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.Pathway.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("PathwayId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.Pathway.CurrentIndex = pageid;
            }

            $scope.Pathway.Methods.GetCategorySearch($scope.Pathway.CurrentIndex);
        },
        Methods: {
            getAllSelectedCheckBox: function () {
                var selectedItems = $scope.Pathway.Pathway.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.Pathway.PatientCategoryID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Pathway.PatientCategoryID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Pathway.PatientCategoryID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.Pathway.Pathway.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.Pathway.Pathway, function (item) {
                    item.Selected = value;

                    var index = $scope.Pathway.PatientCategoryID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Pathway.PatientCategoryID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Pathway.PatientCategoryID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.Pathway.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.Pathway.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.Pathway.PatientCategoryID[0] = id;
                    $scope.Pathway.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },

            ChangeStatusBySwitch: function (status) {
                $scope.Pathway.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.Pathway.PatientCategoryID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.Pathway.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.Pathway.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.Pathway.Services.DeleteMultiple();
                        }
                        $scope.Pathway.PatientCategoryID = [];
                    }
                }
            },

            OpenDeleteDialog: function (id) {
                $scope.Pathway.ID = id;
                $ngBootbox.confirm('Are you sure you want to delete ?')
                       .then(function () {
                           var id = $scope.Pathway.ID;
                           $scope.Pathway.Services.Delete(id);
                       });
            },

            RedirecttoCreate: function () {
                $window.location.href = '/Pathway/Create';
            },

            RedirecttoList: function () {
                $window.location.href = '/Pathway/Index';
            },

            SetCategoryData: function (data) {
                $scope.Pathway.Pathway = JSON.parse(data);
            },

            GetCategoryById: function (id) {
                return $scope.Pathway.Services.GetById(id);
            },

            SetEditCategoryData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/Pathway/Edit?id=' + data.ID;
            },

            Delete: function (id) {
                return $scope.Pathway.Services.Delete(id);
            },
            GetCategorySearch: function (currentIndex) {
                var isactive = null;
                if ($scope.Pathway.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.Pathway.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("PathwaypageObject", JSON.stringify($scope.Pathway.PageParams));
                $window.localStorage.setItem("PathwayId", currentIndex);

                if (currentIndex == 0) {
                    $scope.Pathway.PageNo = 1;
                }
                else {
                    $scope.Pathway.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }

                $scope.Pathway.PageParams.StartIndex = (currentIndex * $scope.Pathway.PageParams.FetchRecords);
                $scope.Pathway.PageParams.EndIndex = (parseInt($scope.Pathway.PageParams.StartIndex) + parseInt($scope.Pathway.PageParams.FetchRecords));

                $scope.Pathway.IsLoading = true;
                $http.get('/api/Pathway/GetPatientCategorySearch?SearchString=' + $scope.Pathway.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.Pathway.PageParams.StartIndex + "&EndIndex=" + $scope.Pathway.PageParams.EndIndex).success(function (response) {

                    $scope.Pathway.IsLoading = false;

                    var responseData = JSON.parse(response);
                    $scope.Pathway.PageParams.TotalCount = responseData.TotalCount;
                    $scope.Pathway.Pathway = responseData.PathwaySearchFilterList;

                    $scope.Pathway.PageParams.CurrentStartIndex = $scope.Pathway.PageParams.StartIndex;

                    if ($scope.Pathway.PageParams.TotalCount < $scope.Pathway.PageParams.EndIndex) {
                        $scope.Pathway.PageParams.CurrentEndIndex = $scope.Pathway.PageParams.TotalCount;
                    }
                    else {
                        $scope.Pathway.PageParams.CurrentEndIndex = $scope.Pathway.PageParams.EndIndex;
                    }

                    $scope.Pathway.Buttons = [];

                    for (var i = 0; i < $scope.Pathway.PageParams.TotalCount / $scope.Pathway.PageParams.FetchRecords; i++) {
                        $scope.Pathway.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.Pathway.Methods.ShowMessage();
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
                $http.get('/api/Pathway/GetById/' + id).success($scope.Pathway.Methods.SetEditCategoryData)
            },
            GetByStatus: function (status) {
                $http.get('/api/Pathway/GetByStatus?status=' + status).success($scope.Pathway.Methods.SetCategoryData)
            },

            UpdateStatus: function (status) {
                var Ids = $scope.Pathway.PatientCategoryID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/Pathway/UpdateStatus',
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
                    $scope.Pathway.Action = "";
                    var index = $window.localStorage.getItem("PathwayId");
                    $scope.Pathway.Methods.GetCategorySearch(index);
                });
            },

            Delete: function (id) {
                $http.delete('/api/Pathway/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.Pathway.Methods.RedirecttoList();
                });
            },
            DeleteMultiple: function () {
                var Ids = $scope.Pathway.PatientCategoryID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/Pathway/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.Pathway.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.Pathway.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.Pathway.Initialize();
}]);