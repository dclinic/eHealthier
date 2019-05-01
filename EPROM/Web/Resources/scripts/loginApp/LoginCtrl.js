loginApp.controller('LoginCtrl', function ($scope, LoginService) {
    $scope.user = [];
    $scope.userName = "";
    $scope.password = "";
    $scope.userDisplayName = "";
    $scope.isAuthenticated = true;
    $scope.logout = function () {

    };
    $scope.loggedIn = function () {
        $scope.user = {
            grant_type: 'password',
            username: $scope.userName,
            password: $scope.password
        };
        if ($scope.userName == "" || $scope.password == "") {
            $scope.isAuthenticated = false;
            window.location.href = "/Login";
        }
        else {
            var promise = LoginService.login($scope.user);
            promise.then(function (resp) {
                if (resp.data != null) {
                    if (resp.data.error == "invalid_grant") {
                        $scope.isAuthenticated = false;
                        window.location.href = "/Login";
                    }
                    else {
                        $scope.isAuthenticated = true;
                        sessionStorage.setItem('userName', $scope.userName);
                        sessionStorage.setItem('accessToken', resp.data.access_token);
                        window.location.href = "/Patient/epromAllocation";
                    }
                } else {
                    $scope.isAuthenticated = false;
                    window.location.href = "/Login";
                }
            }, function () {
                $scope.isAuthenticated = false;
                window.location.href = "/Login";
            });
        }
    };
});