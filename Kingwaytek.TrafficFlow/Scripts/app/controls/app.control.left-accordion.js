(function ($) {
    'use strict';
    var $this = $('.left-accordion');

    if ($this.length === 0) {
        return;
    }

    $this.accordion({
        collapsible: false,
        disabled: true,
        heightStyle: 'content',
        active: 0
    });
})(jQuery);