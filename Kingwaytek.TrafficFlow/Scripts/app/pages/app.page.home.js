(function ($) {
    'use strict';
    $('.left-accordion').accordion({
        collapsible: false,
        disabled: true,
        heightStyle: "content",
        active: 0,
    });

    //this will open 1st accordian.
    $('#next-1').click(function () {
        $(".left-accordion").accordion({ active: 1 });
    });

    //this will open 2nd accordian.
    $('#next-2').click(function () {
        $(".left-accordion").accordion({ active: 2 });
    });

    //this will open 3rd accordian.
    $('#prev-1').click(function () {
        $(".left-accordion").accordion({ active: 0 });
    });

    //this will open 3rd accordian.
    $('#prev-2').click(function () {
        $(".left-accordion").accordion({ active: 1 });
    });
})(jQuery);