(function ($) {
    'use strict';

    var $this = $('.home-index');

    if ($this.length === 0) {
        return;
    }

    $('.traffic-create').parent().addClass('active');

    $this
        .on('show.bs.modal', '#Modal-Delete', function (event) {
            var $button = $(event.relatedTarget);
            var investigationId = $button.data('investigate-id');
            var $modal = $(this);
            $modal.find('#deleteTarget').val(investigationId);
        })
        .on('click', '#deleteConfirmBtn', function () {
            $.post('/home/delete/' + $('#deleteTarget').val())
                .done(function () {
                    alert('調查資料刪除成功');
                    $('.home-index').find('form').submit();
                })
                .fail(function () {
                    alert('調查資料刪除失敗，請與系統管理員聯繫');
                });
        });
})(jQuery);