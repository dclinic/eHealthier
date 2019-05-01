loginApp.factory("LoginService", function LoginService($http, appSetting) {
    this.login = function (userData) {
        var response = $http({
            url: appSetting.apiBaseUrl + 'api/token',
            method: 'POST',
            data: $.param({
                grant_type: userData.grant_type,
                username: userData.username,
                password: userData.password
            }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        });
        return response;
    };
    return {
        login: this.login
    }
});