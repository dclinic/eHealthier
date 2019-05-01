loginApp.factory('AuthInterceptor', ['$log', '$q', function ($log, $q) {
    $log.debug('$log is here to show you that this is a regular factory with injection');

    var authInterceptor = {
        request: function (config) {
            var accessToken = sessionStorage.getItem('accessToken');
            if (accessToken == null || accessToken == "undefined") {
                //$state.go("login");
                alert("access Token is undefined...");
            }
            else {
                config.headers["Authorization"] = "bearer " + accessToken;
            }
            return config;
        },
        requestError: function (config) {
            //$state.go("login");
            alert("request Error...");
            return config;
        },
        response: function (res) {
            return res;
        },
        responseError: function (res) {
            alert(res.status);
            //if (res.status == "401") {
            //    //$state.go("login");
            //}
            //if (res.status == "400") {
            //    //$state.go("login");
            //}
            //if (res.status == "403") {
            //    //$state.go("login");
            //}
            //if (res.status == "404") {
            //    //$state.go("login");
            //}
            $q.reject(res)
            return res;
        }
    };

    return authInterceptor;
}]);