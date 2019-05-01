
$(function () {
    setTimezoneCookie();
});



/*------------------Set cookies for display dates and times in clients timezone-----------------*/
function setTimezoneCookie() {
    var timezone_cookie = "timezoneoffset";
    var datetimezone_cookie = "datetimezoneoffset1";
    //var MobileAppCurrentDateTime = ""; // for mobile app
    // if the timezone cookie not exists create one.

    if (!$.cookie(timezone_cookie)) {
        // check if the browser supports cookie
        var test_cookie = 'test cookie';
        $.cookie(test_cookie, true);

        // browser supports cookie
        if ($.cookie(test_cookie)) {

            // delete the test cookie
            $.cookie(test_cookie, null);
            // create a new cookie 
            $.cookie(timezone_cookie, new Date().getTimezoneOffset(), { path: '/' });

        }
        else {
            $.cookie(timezone_cookie, new Date().getTimezoneOffset(), { path: '/' });
        }
    }
        // if the current timezone and the one stored in cookie are different
        // then store the new timezone in the cookie and refresh the page.
    else {
        var storedOffset = parseInt($.cookie(timezone_cookie));
        var currentOffset = "";
        currentOffset = new Date().getTimezoneOffset();

        // user may have changed the timezone
        if (storedOffset !== currentOffset) {
            $.cookie(timezone_cookie, new Date().getTimezoneOffset(), { path: '/' });
        }
    }




    if (!$.cookie(datetimezone_cookie)) {
        // create a new cookie 
        $.cookie(datetimezone_cookie, new Date().format("mm/dd/yyyy hh:MM:ss TT"), { path: '/' });
    }
    else {
        var olddatetime = $.cookie(datetimezone_cookie);
        var currentdatetime = new Date().format("mm/dd/yyyy hh:MM:ss TT");

        // user may have changed the timezone
        if (olddatetime !== currentdatetime) {
            $.cookie(datetimezone_cookie, new Date().format("mm/dd/yyyy hh:MM:ss TT"), { path: '/' });
        }
    }
}