(function ($) {
    'use strict';
    var $this = $('.t_date');

    if ($this.length === 0) {
        return;
    }

    $this.datetimepicker({
        timepicker: false,
        mounthpicker: false,
        defaultDate: new Date(),
        maxDate: new Date(),
        format: 'Y/m/d',
        lang: 'ch'
    });
})(jQuery);