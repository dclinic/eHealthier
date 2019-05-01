app.controller('CustomerController', ['$scope', '$http', '$window', '$location', '$upload', 'toaster', '$ngBootbox', function ($scope, $http, $window, $location, $upload, toaster, $ngBootbox) {

    $scope.Customer = {
        //visibleflag: true,
        allSelected: false,
        CheckAll: false,
        Check: [],
        Customer: [],
        CustomerID: [],
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
            $scope.Customer.PageParams.FetchRecords = 10;

            var obj = $window.localStorage.getItem("CustpageObject");
            if (obj != typeof (undefined) && obj != null && obj != '' && obj != 'null') {
                $scope.Customer.PageParams = JSON.parse(obj);
            }

            var pageid = $window.localStorage.getItem("CustpageId");
            if (pageid != typeof (undefined) && pageid != null && pageid != '' && pageid != 'null') {
                $scope.Customer.CurrentIndex = pageid;
            }

            $scope.Customer.Methods.GetCustomerSearch($scope.Customer.CurrentIndex);
        },
        Methods: {
            getAllSelectedCheckBox: function () {
                var selectedItems = $scope.Customer.Customer.filter(function (item) {
                    var value = item.Selected;
                    var index = $scope.Customer.CustomerID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Customer.CustomerID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Customer.CustomerID.splice(index, 1);
                    }
                    return value;
                });
                return selectedItems.length === $scope.Customer.Customer.length;
            },

            setAllSelectedCheckBox: function (value) {
                angular.forEach($scope.Customer.Customer, function (item) {
                    item.Selected = value;

                    var index = $scope.Customer.CustomerID.indexOf(item.ID);

                    if (value && index == -1) {
                        $scope.Customer.CustomerID.push(item.ID);
                    } else if (!value && index != -1) {
                        $scope.Customer.CustomerID.splice(index, 1);
                    }
                });
            },

            allSelected: function (value) {
                if (value !== undefined) {
                    return $scope.Customer.Methods.setAllSelectedCheckBox(value);
                } else {
                    return $scope.Customer.Methods.getAllSelectedCheckBox();
                }
            },

            SwitchStatus: function (id, status) {
                try {
                    $scope.Customer.CustomerID[0] = id;
                    $scope.Customer.Methods.ChangeStatusBySwitch(status);
                } catch (e) {
                }
            },
            ChangeStatusBySwitch: function (status) {
                $scope.Customer.Services.UpdateStatus(status);
            },

            UpdateIsActiveStatus: function (obj, status) {
                if (obj != null && obj != "") {
                    if ($scope.Customer.CustomerID.length < 1) {
                        toaster.pop('warning', '', "Select atleast one record.");
                        $scope.Customer.Action = "";
                    }
                    else {
                        if (obj == "Active" || obj == "Inactive") {
                            var status = false;
                            if (obj == "Active")
                                status = true;
                            else if (obj == "Inactive")
                                status = false;

                            $scope.Customer.Services.UpdateStatus(status);
                        }
                        else if (obj == "Delete") {
                            $scope.Customer.Services.DeleteMultiple();
                        }
                        $scope.Customer.CustomerID = [];
                    }
                }
            },
            OpenDeleteDialog: function (id) {
                $scope.Customer.ID = id;
                $ngBootbox.confirm('Are you sure ypu want to delete ?')
                       .then(function () {
                           var id = $scope.Customer.ID;
                           $scope.Customer.Services.Delete(id);
                       });
            },
            RedirecttoList: function () {                
                $window.location.href = '/Customer/Index';
            },
            SetCustomerData: function (data) {
                $scope.Customer.Customer = JSON.parse(data);
            },
            GetCustomerById: function (id) {
                return $scope.Customer.Services.GetById(id);
            },

            SetEditCustomerData: function (response) {
                var data = JSON.parse(response);
                $window.location.href = '/Customer/Edit?id=' + data.ID;
            },
            Delete: function (id) {
                return $scope.Customer.Services.Delete(id);
            },
            GetCustomerSearch: function (currentIndex) {
                var isactive = null;
                if ($scope.Customer.Filter == "Active") {
                    isactive = true;
                }
                else if ($scope.Customer.Filter == "Inactive") {
                    isactive = false;
                }
                else {
                    isactive = null;
                }

                $window.localStorage.setItem("CustpageObject", JSON.stringify($scope.Customer.PageParams));
                $window.localStorage.setItem("CustpageId", currentIndex);

                if (currentIndex == 0) {
                    $scope.Customer.PageNo = 1;
                }
                else {
                    $scope.Customer.PageNo = currentIndex;
                    currentIndex = currentIndex - 1;
                }
                $scope.Customer.PageParams.StartIndex = (currentIndex * $scope.Customer.PageParams.FetchRecords);
                $scope.Customer.PageParams.EndIndex = (parseInt($scope.Customer.PageParams.StartIndex) + parseInt($scope.Customer.PageParams.FetchRecords));

                $scope.Customer.IsLoading = true;
                $http.get('/api/customer/GetCustomerSearch?SearchString=' + $scope.Customer.Search + "&IsActive=" + isactive + "&StartIndex=" + $scope.Customer.PageParams.StartIndex + "&EndIndex=" + $scope.Customer.PageParams.EndIndex).success(function (response) {

                    $scope.Customer.IsLoading = false;

                    var responseData = JSON.parse(response);
                    $scope.Customer.PageParams.TotalCount = responseData.TotalCount;
                    $scope.Customer.Customer = responseData.CustomerSearchFilterList;

                    $scope.Customer.PageParams.CurrentStartIndex = $scope.Customer.PageParams.StartIndex;

                    if ($scope.Customer.PageParams.TotalCount < $scope.Customer.PageParams.EndIndex) {
                        $scope.Customer.PageParams.CurrentEndIndex = $scope.Customer.PageParams.TotalCount;
                    }
                    else {
                        $scope.Customer.PageParams.CurrentEndIndex = $scope.Customer.PageParams.EndIndex;
                    }

                    $scope.Customer.Buttons = [];

                    for (var i = 0; i < $scope.Customer.PageParams.TotalCount / $scope.Customer.PageParams.FetchRecords; i++) {
                        $scope.Customer.Buttons.push({ currentindex: (i + 1) });
                    }
                    $scope.Customer.Methods.ShowMessage();
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
                $http.get('/api/customer/GetById/' + id).success($scope.Customer.Methods.SetEditCustomerData)
            },
            GetByStatus: function (status) {
                $http.get('/api/customer/GetByStatus?status=' + status).success($scope.Customer.Methods.SetCustomerData)
            },
            UpdateStatus: function (status) {
                var Ids = $scope.Customer.CustomerID;
                $http({
                    method: 'POST',
                    cache: false,
                    url: '/api/customer/UpdateStatus',
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
                    $scope.Customer.Action = "";
                    var index = $window.localStorage.getItem("CustpageId");
                    $scope.Customer.Methods.GetCustomerSearch(index);
                });
            },
            Delete: function (id) {
                $http.delete('/api/customer/Delete/' + id).success(function (response) {
                    $window.localStorage.setItem("Message", response);
                    $scope.Customer.Methods.RedirecttoList();
                });
            },
            DeleteMultiple: function () {
                var Ids = $scope.Customer.CustomerID;
                $http({
                    method: 'DELETE',
                    cache: false,
                    url: '/api/customer/DeleteMultiple',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        'Ids': Ids.join(",")
                    },
                }).success(function (response) {
                    $scope.Customer.Action = "";
                    $window.localStorage.setItem("Message", response);
                    $scope.Customer.Methods.RedirecttoList();
                });
            },
        }
    };

    $scope.Customer.Initialize();
}]);