/*!
 * Custom js 
 * Created By Hetal Rupareliya
 * Created Date 22/02/2016
 */
var liheight = 0, opt2height = 0;
$(document).ready(function () {

    var flg = 1;
    var window_height, menu_height = 0;
    var menuflag = false;
    /*--------------------Menu scroll set--------------------*/
    $(".menu-scroll").hide();
    //$(".menu-scroll").tooltip();
    $(".menu-scroll").click(function () {
        updown_scroll(this);
    });
    menu_height = $(window).height() - $("#header").height() - 65;
    $("div.leftnav").css('height', menu_height + "px");

    $('.leftnav_scroll').scroll(function () {
        if ($(".leftnav_scroll").scrollTop() != 0) {
            $('.menu-scroll').removeClass("down").addClass("up");
            $('.menu-scroll').children("i").removeClass("fa fa-arrow-down").addClass("fa fa-arrow-up");
        }
        else {
            $('.menu-scroll').removeClass("up").addClass("down");
            $('.menu-scroll').children("i").removeClass("fa fa-arrow-up").addClass("fa fa-arrow-down");
        }
    });

    liheight = $("ul#ULAdminMenu").height();
    $("li.collapsed", ".MenuLevel1").click(function () {
        if ($(this).hasClass("collapsed")) {
            liheight = liheight + $(this).next("li.collapse").children("ul").height();
        }
        else {
            liheight = liheight - $(this).next("li.collapse").children("ul").height();
        }
        if (liheight >= menu_height) {
            $('.menu-scroll').show();
        }
        else {
            $('.menu-scroll').hide();
        }
    });

    /*--------------------Page body set--------------------*/
    window_height = $(window).height() - $("#header").height();
    if ($(".fix-head-line").length) {
        window_height = window_height - $(".fix-head-line").height();
    }
    if ($(".search-options-block").length) {
        window_height = window_height - $(".search-options-block").height();
    }
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        menu_height = menu_height - 5;
    }
    if ($('#DivPageNumber').length) {
        window_height = window_height - $('#DivPageNumber').height();
    }
    if ($('.fix-body-line').length) {
        window_height = window_height - $('.fix-body-line').height();
    }
    if ($('.fix-body-line.extra').length) {
        window_height = window_height - $('.fix-body-line.extra').height();
    }
    if ($('.region-block').length) {
    }
    $(".page-body").css('height', window_height - 25 + "px");
    //$(".page-body").css('height', window_height + "px");
    $(".fix-head-line").insertBefore($(".page-body"));
    $(".fix-body-line").insertBefore($(".page-body"));

    $(".poweredby-updown").click(function () {
        try {
            if ($(this).hasClass("fa fa-chevron-up")) {
                menu_height -= 40;
                setTimeout(function () {
                    $("div.leftnav").css('height', menu_height + "px");
                }, 300);
                $(this).parent().css('bottom', '0px');
                $(this).removeClass("fa fa-chevron-up").addClass("fa fa-chevron-down");

            }
            else {
                menu_height += 40;
                $("div.leftnav").delay("slow").css('height', menu_height + "px");
                $(this).parent().css('bottom', '-42px');
                $(this).removeClass("fa fa-chevron-down").addClass("fa fa-chevron-up");
            }
        }
        catch (e) {
        }

    });

    $("div.sidebar-toggler").click(function () {
        try {
            var mode = $.cookie("sidebar");

            if (mode) {
                if (mode == "full" || mode == " ") {
                    $("div.rightpanel").css("margin-left", "44px");
                    $(".page_block").css("right", "55px");
                    $("div.leftnav-icon").show();
                    $("div.leftnav").hide();
                    $(".poweredby-icon").show();
                    $(".poweredby-logo").hide();
                    $.cookie("sidebar", "collapse");
                } else if (mode == "collapse") {
                    $("div.rightpanel").css("margin-left", "230px");
                    $(".page_block").css("right", "240px");
                    $("div.leftnav-icon").hide();
                    $("div.leftnav").show();
                    $(".poweredby-icon").hide();
                    $(".poweredby-logo").show();
                    $.cookie("sidebar", "full");
                }
            } else {
                $("div.rightpanel").css("margin-left", "230px");
                $(".page_block").css("right", "240px");
                $("div.leftnav-icon").hide();
                $("div.leftnav").show();
                $(".poweredby-icon").hide();
                $(".poweredby-logo").show();
                $.cookie("sidebar", "collapse");
            }

        }
        catch (e) {
        }
    });
    /*-----------------Mobile View Menu----------------------*/
    $("div.sidebar-toggler-mob").click(function () {
        try {
            $("div.leftnav").show();
            $('.leftpanel').animate({
                width: 'toggle'
            }, "fast")
        }
        catch (e) {
        }
    });
    /*-----------------Menu Bar----------------------*/
    $("div.leftnav .MenuOptionLevel1").click(function () {
        try {
            $("div.leftnav .MenuOptionLevel1").each(function () {
                $(this).removeClass("active");
            });
            $(this).toggleClass("active");
        }
        catch (e) {
        }
    });
    $("div.leftnav .MenuOptionLevel2").click(function () {
        try {
            //   $(this).children("a").find("span.arrow").toggleClass("open");
        }
        catch (e) {
        }
    });
    $("div.leftnav-icon .MenuOptionLevel2").click(function () {
        try {
            //   $(this).children("a").find("span.arrow").toggleClass("open");
        }
        catch (e) {
        }
    });

    $("div.mob-btn-search").click(function () {
        try {
            if (flg == 1) {
                $(this).find("i").removeClass("fa fa-search").addClass("fa fa-remove");
                $("div.mob-search-block").css("display", "inline-block");
                flg = 0;
            }
            else {
                $(this).find("i").removeClass("fa fa-remove").addClass("fa fa-search");
                $("div.mob-search-block").css("display", "none");
                flg = 1;
            }
        }
        catch (e) {
        }
    });
    /*------------------Advance Search Text-----------------*/
    $(".advance-btn").click(function () {
        try {
            $(".search-options-block").slideToggle("fast");
        }
        catch (e) {
        }
    });
});

$(window).load(function () {
    try {
        icon_menu();
    }
    catch (e) { }
});

function Page_ClientValidateReset(ValidationGroupName) {
    try {
        var val = Page_ClientValidate(ValidationGroupName);
        if (!val) {
            for (var j = 0; j < Page_Validators.length; j++) {
                if (!Page_Validators[j].isvalid) {
                    $("#" + Page_Validators[j].controltovalidate).removeClass('requiredtext');
                }
            }
        }
        if (typeof (Page_Validators) != "undefined") {
            for (var i = 0; i < Page_Validators.length; i++) {
                var validator = Page_Validators[i];
                if (validator.validationGroup == ValidationGroupName) {
                    validator.isvalid = true;
                    ValidatorUpdateDisplay(validator);
                }
            }
        }
    }
    catch (e) {
    }
}

function Page_TextboxRequired(ValidationGroupName) {
    try {
        var val = Page_ClientValidate(ValidationGroupName);
        if (!val) {
            for (var j = 0; j < Page_Validators.length; j++) {
                if (!Page_Validators[j].isvalid) {
                    $("#" + Page_Validators[j].controltovalidate).addClass('requiredtext');
                }
            }
        }
    }
    catch (e) {
    }
}

/*-----------------Set Cookie of Menu ----------------------*/

function icon_menu() {
    try {
        var mode = $.cookie("sidebar");
        if (mode) {
            if (mode == "collapse" || mode == " ") {
                $("div.rightpanel").css("margin-left", "44px");
                $(".page_block").css("right", "55px");
                $("div.leftnav-icon").show();
                $("div.leftnav").hide();
                $(".poweredby-icon").show();
                $(".poweredby-logo").hide();
            } else if (mode == "full") {
                $("div.rightpanel").css("margin-left", "230px");
                $(".page_block").css("right", "240px");
                $("div.leftnav-icon").hide();
                $("div.leftnav").show();
                $(".poweredby-icon").hide();
                $(".poweredby-logo").show();
            }
        }
    }
    catch (e)
    { }
}

/*-----------------Dyanamically Set Pagebody Height------------*/

function cus_pagebody(obj) {
    try {
        var height = $(".page-body").height() - $("." + obj).height();
        $(".page-body").css('height', height - 10 + "px");
    }
    catch (e)
    { }
}

/*-----------------Menu Scroll------------*/

function updown_scroll(obj) {
    try {
        if ($(obj).hasClass("down")) {
            $('.leftnav_scroll').animate({
                scrollTop: $('.leftnav_scroll').height()
            }, 600);
            $(obj).removeClass("down").addClass("up");
            $(obj).children("i").removeClass("fa fa-arrow-down").addClass("fa fa-arrow-up");
        }
        else {
            $('.leftnav_scroll').animate({
                scrollTop: '0px'
            }, 600);
            $(obj).removeClass("up").addClass("down");
            $(obj).children("i").removeClass("fa fa-arrow-up").addClass("fa fa-arrow-down");
        }
    }
    catch (e)
    { }
}

/*------------------Status Message Get, Display, Clear Methods-----------------*/

function ShowStatusMessageDialog(Message) {
    try {
        if (Message != null && Message != "") {
            $("#LBLPopupStatusMessage").html(Message);
            $("div#DialogStatusMessage").show();

            setTimeout(function () {
                $("div#DialogStatusMessage").hide();
            }, 3000);
        }
    } catch (e) {
    }
}