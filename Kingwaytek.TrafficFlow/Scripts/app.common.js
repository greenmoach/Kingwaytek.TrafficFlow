(function ($) {
    'use strict';

    $.kingwaytek = {
        util: {},
        eventBind: {}
    };

    $.kingwaytek.util.eventBootLoader = function () {
        for (var key in $.kingwaytek.eventBind) {
            if ($.kingwaytek.eventBind.hasOwnProperty(key)) {
                $.kingwaytek.eventBind[key]();
            }
        }
    };
})(jQuery);