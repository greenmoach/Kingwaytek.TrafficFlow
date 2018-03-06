﻿var $town = $('#Town'),
    $road1 = $('#Road1'),
    $road2 = $('#Road2'),
    positioningOfIntersection = {};

(function ($) {
    'use strict';
    var $this = $('.Position-Setup');

    if ($this.length === 0) {
        return;
    }

    $this
        .on('change',
        '#Town',
        function () {
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
        .on('change',
        '#Road1',
        function () {
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
        .on('change',
        '#Road2',
        function () {
            var selectedTown = $town.find('option:selected').val(),
                selectedRoad1 = $road1.find('option:selected').val(),
                selectedRoad2 = $road2.find('option:selected').val();

            if (selectedTown === '' || selectedRoad1 === '' || selectedRoad2 === '') {
                return;
            }

            $.post('/Location/GetPositioning?town=' +
                selectedTown +
                '&road1=' +
                selectedRoad1 +
                '&road2=' +
                selectedRoad2)
                .done(function (data) {
                    positioningOfIntersection = data;
                    $('#next-1').prop('disabled', false);
                })
                .fail(function () {
                    positioningOfIntersection = {};
                    $('#next-1').prop('disabled', true);
                    alert('無法取得該交叉路口定位資訊');
                });
        });
})(jQuery);