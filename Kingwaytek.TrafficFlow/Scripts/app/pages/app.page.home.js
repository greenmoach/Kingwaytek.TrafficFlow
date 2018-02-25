(function ($) {
    'use strict';

    var $this = $('.home-create');

    if ($this.length === 0) {
        return;
    }

    var $town = $('#Town'),
        $road1 = $('#Road1'),
        $road2 = $('#Road2'),
        positioningOfIntersection = {};

    $('.left-accordion').accordion({
        collapsible: false,
        disabled: true,
        heightStyle: 'content',
        active: 0
    });

    $('.t_date').datetimepicker({
        timepicker: false,
        mounthpicker: false,
        defaultDate: new Date(),
        format: 'Y/m/d',
        lang: 'ch'
    });

    $this
        .on('click',
        '#next-1',
        function () {
            //this will open 1st accordian.
            $('.left-accordion').accordion({ active: 1 });
        })
        .on('click',
        '#next-2',
        function () {
            //this will open 2nd accordian.
            $('.left-accordion').accordion({ active: 2 });
        })
        .on('click',
        '#prev-1',
        function () {
            //this will open 3rd accordian.
            $('.left-accordion').accordion({ active: 0 });
        })
        .on('click',
        '#prev-2',
        function () {
            //this will open 3rd accordian.
            $('.left-accordion').accordion({ active: 1 });
        })
        .on('change', '#Town', function () {
            var selectedTown = $town.find('option:selected').val();

            $road1.html('');
            $road1.append('<option>請選擇</option>');

            $road2.html('');
            $road2.append('<option>請選擇</option>');

            positioningOfIntersection = {};
            $('#next-1').prop('disabled', true);

            if (selectedTown === '') {
                return;
            }

            $.post('/Location/GetRoadsByTown?town=' + selectedTown)
                .done(function (data) {
                    if (Array.isArray(data) && data.length > 0) {
                        data.forEach(function (value) {
                            $road1.append('<option value="' + value + '">' + value + '</option>');
                        });
                        return;
                    }
                    alert('路口定位道路清單取得失敗');
                })
                .fail(function () {
                    alert('路口定位道路清單取得失敗');
                });
        })
        .on('change', '#Road1', function () {
            var selectedTown = $town.find('option:selected').val(),
                selectedRoad1 = $road1.find('option:selected').val();

            $road2.html('');
            $road2.append('<option>請選擇</option>');

            positioningOfIntersection = {};
            $('#next-1').prop('disabled', true);

            if (selectedRoad1 === '' || selectedTown === '') {
                return;
            }

            $.post('/Location/GetRoadsByIntersection?town=' + selectedTown + '&road=' + selectedRoad1)
                .done(function (data) {
                    if (Array.isArray(data) && data.length > 0) {
                        data.forEach(function (value) {
                            $road2.append('<option value="' + value + '">' + value + '</option>');
                        });
                        return;
                    }
                    alert('路口定位道路清單取得失敗');
                })
                .fail(function () {
                    alert('路口定位道路清單取得失敗');
                });
        })
        .on('change', '#Road2', function () {
            var selectedTown = $town.find('option:selected').val(),
                selectedRoad1 = $road1.find('option:selected').val(),
                selectedRoad2 = $road2.find('option:selected').val();

            if (selectedTown === ''
                || selectedRoad1 === ''
                || selectedRoad2 === '') {
                return;
            }

            $.post('/Location/GetPositioning?town=' + selectedTown + '&road1=' + selectedRoad1 + '&road2=' + selectedRoad2)
                .done(function (data) {
                    positioningOfIntersection = data;
                    $('#next-1').prop('disabled', false);
                })
                .fail(function () {
                    alert('無法取得該交叉路口定位資訊');
                });
        });
})(jQuery);