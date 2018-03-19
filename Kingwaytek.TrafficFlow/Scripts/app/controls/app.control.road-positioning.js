var $town = $('#Town'),
    $road1 = $('#Road1'),
    $road2 = $('#Road2'),
    positioningOfIntersection = {};

(function ($) {
    'use strict';
    var $this = $('.Position-Setup');

    if ($this.length === 0) {
        return;
    }

    // init road1 input box with jquery ui of autocomplete
    $road1.autocomplete();

    $this
        .on('change',
        '#Town',
        function () {
            var selectedTown = $town.find('option:selected').val();

            $road1.val('');

            $road2.html('');
            $road2.append('<option>請選擇</option>');

            positioningOfIntersection = {};
            $('#next-1').prop('disabled', true);

            if (selectedTown === '') {
                return;
            }

            $.ajax({
                url: sitepath + 'Location/GetRoadsByTown?town=' + selectedTown,
                method: 'POST',
                beforeSend: function () {
                    showLoading();
                }
            })
                .done(function (data) {
                    if (Array.isArray(data) && data.length > 0) {
                        $road1.autocomplete('option', 'source', data);
                        return;
                    }
                    alert('路口定位道路清單取得失敗');
                })
                .fail(function () {
                    alert('路口定位道路清單取得失敗');
                })
                .always(function () {
                    removeLoading();
                });
        })
        .on('autocompleteselect',
        '#Road1',
        function () {
            var selectedTown = $town.find('option:selected').val(),
                selectedRoad1 = $road1.val();

            $road2.html('');
            $road2.append('<option>請選擇</option>');

            positioningOfIntersection = {};
            $('#next-1').prop('disabled', true);

            if (selectedRoad1 === '' || selectedTown === '') {
                return;
            }

            $.ajax({
                url: sitepath + 'Location/GetRoadsByIntersection?town=' + selectedTown + '&road=' + selectedRoad1,
                method: 'POST',
                beforeSend: function () {
                    showLoading();
                }
            })
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
                })
                .always(function () {
                    removeLoading();
                });
        })
        .on('change',
        '#Road2',
        function () {
            var selectedTown = $town.find('option:selected').val(),
                selectedRoad1 = $road1.val(),
                selectedRoad2 = $road2.find('option:selected').val();

            if (selectedTown === '' || selectedRoad1 === '' || selectedRoad2 === '') {
                return;
            }

            $.ajax({
                url: sitepath + 'Location/GetPositioning?town=' + selectedTown + '&road1=' + selectedRoad1 + '&road2=' + selectedRoad2,
                method: 'POST',
                beforeSend: function () {
                    showLoading();
                }
            })
                .done(function (data) {
                    positioningOfIntersection = data;
                    $('#next-1').prop('disabled', false);
                })
                .fail(function () {
                    positioningOfIntersection = {};
                    $('#next-1').prop('disabled', true);
                    alert('無法取得該交叉路口定位資訊');
                })
                .always(function () {
                    removeLoading();
                });
        });

    function showLoading() {
        $('<div class="featherlight" style="display: block;"><div id="spinner-content" class="featherlight-content" style="vertical-align:middle;width:70px;height:70px;border-radius:5px;"></div></div>')
            .appendTo($this);

        var opts = {
            lines: 13, // The number of lines to draw
            length: 21, // The length of each line
            width: 12, // The line thickness
            radius: 27, // The radius of the inner circle
            scale: 0.45, // Scales overall size of the spinner
            corners: 1, // Corner roundness (0..1)
            color: '#ffffff', // CSS color or array of colors
            fadeColor: 'transparent', // CSS color or array of colors
            opacity: 0.25, // Opacity of the lines
            rotate: 0, // The rotation offset
            direction: 1, // 1: clockwise, -1: counterclockwise
            speed: 1.3, // Rounds per second
            trail: 60, // Afterglow percentage
            fps: 20, // Frames per second when using setTimeout() as a fallback in IE 9
            zIndex: 2e9, // The z-index (defaults to 2000000000)
            className: 'spinner', // The CSS class to assign to the spinner
            top: '50%', // Top position relative to parent
            left: '50%', // Left position relative to parent
            shadow: true, // Box-shadow for the lines
            hwaccel: true, // Whether to use hardware acceleration
            position: 'absolute' // Element positioning
        };
        var target = document.getElementById('spinner-content');
        var spinner = new Spinner(opts).spin(target);
    };

    function removeLoading() {
        $this.find('.featherlight').remove();
    };
})(jQuery);