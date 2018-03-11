(function ($) {
    'use strict';

    var $this = $('.control_pager');

    if ($this.length === 0) {
        return;
    }

    $this
        .on('submit', '.paged-query-form', function () {
            var $f = $(this),
                data = $f.serialize(),
                obj = {};
            // form submit data tansfer to object
            $f.serializeArray().forEach(function (x) {
                obj[x.name] = x.value;
            });

            // 如果是查詢，做ajax查詢
            $.post($f.attr('action'), data, function (html) {
                $this.empty()
                    .html(html);
                $.kingwaytek.util.eventBootLoader();
            });

            return false;
        })
        .on('click', '.pagination a', function () {
            var $a = $(this);
            if ($a.is('.disabled') === false) {
                var $f = $this.children('.paged-query-form'),
                    s = parseInt($f.find('#Page').val(), 10),
                    h = $a.attr('href');
                switch (true) {
                    case /^#Prev$/.test(h):
                        s -= 1;
                        break;
                    case /^#Next$/.test(h):
                        s += 1;
                        break;
                    case /^#Page_\d+$/.test(h):
                        s = parseInt(/^#Page_(\d+)$/.exec(h)[1], 10);
                        break;
                }
                $f.find('#Page')
                    .val(s)
                    .end()
                    .trigger('submit');
            }

            return false;
        });
})(jQuery);