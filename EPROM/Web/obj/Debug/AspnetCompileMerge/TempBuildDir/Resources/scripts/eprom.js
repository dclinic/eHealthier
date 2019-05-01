function tab_height() {
    var window_height = $(window).outerHeight();

    var tab_height = $('.tabbable > ul.nav-tabs').outerHeight();

    var tab_content_height = (window_height - tab_height) - 105;

    $('.tabbable > div.tab-content').css('max-height', tab_content_height);
}

$(document).ready(function () {
    tab_height();

    $('.phone-mask').inputmask("(9{1,3}) 9{1,3}-9{1,4}"); //mask with dynamic syntax
    $('[data-toggle="tooltip"]').tooltip();
    //$('#datetimepicker1').datetimepicker();


});

$(window).resize(function () {
    tab_height();
});