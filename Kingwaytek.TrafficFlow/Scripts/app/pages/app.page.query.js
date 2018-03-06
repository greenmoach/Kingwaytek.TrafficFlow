(function ($) {
    'use strict';
    var $this = $('.home-query'),
        singleData,
        hourlyVehicleChart,
        centerMarkers,
        markers = [],
        infos = [];

    if ($this.length === 0) {
        return;
    }

    // active function tab
    $('.traffic-query').parent().addClass('active');

    // load google chats package
    google.charts.load('current', { packages: ['corechart'] });

    $this
        .on('click',
        '#next-1',
        function () {
            var data = {
                queryType: $('input[name="area"]:checked').val(),
                positioningId: positioningOfIntersection.Id
            };

            $.post('/home/query', data)
                .done(function (data) {
                    singleData = data;
                    alert('success');
                    initDisplayObjects();
                })
                .fail(function () {
                    alert('error');
                });
        })
        .on('change', '#HourlyIntervals', function () {
            var hourlyData = singleData.HourlyIntervals.find(function (item) {
                return item.HourlyInterval === $('#HourlyIntervals option:selected').val();
            });
            updateInfoWindows(hourlyData);
            updateHourlyVehicleChart(hourlyData);
        });

    function initDisplayObjects() {
        var positionObj = JSON.parse(singleData.Positioning);
        var centerLatLng = new google.maps.LatLng(positionObj.center.latitude, positionObj.center.longitude);
        centerMarkers = new google.maps.Marker({
            id: 'center',
            position: centerLatLng,
            icon: '/content/images/center.svg',
            draggable: false,
            map: geeMap
        });

        centerMarkers.addListener('click',
            function () {
                $('.div-Info').fadeToggle();
            });

        positionObj.directions.forEach(function (d) {
            markers.push(new google.maps.Marker({
                id: d.id,
                position: new google.maps.LatLng(d.latitude, d.longitude),
                icon: '/content/images/arrow-01.svg',
                draggable: false,
                map: geeMap
            }));
        });

        // 更新調查資料查詢介面
        $('#HourlyIntervals').html('');
        singleData.HourlyIntervals.forEach(function (interval) {
            $('<option value="' +
                interval.HourlyInterval +
                '">' +
                interval.HourlyInterval.substring(0, 4) +
                '-' +
                interval.HourlyInterval.substring(4, 8) +
                '</option>').appendTo('#HourlyIntervals');
        });

        $('#IntersectionId').text(singleData.IntersectionId === null ? '' : singleData.IntersectionId);
        $('#IntersectionName').text(singleData.IntersectionName === null ? '' : singleData.IntersectionName);
        $('#Weather').text(singleData.Weather === null ? '' : singleData.Weather);
        $('#TrafficControlNote').text(singleData.TrafficControlNote === null ? '' : singleData.TrafficControlNote);

        // 車種統計圖
        initHourlyVehicleChart();

        geeMap.panTo(centerLatLng);
        geeMap.setZoom(20);
    }

    function initHourlyVehicleChart() {
        // Create the data table.
        var data = new google.visualization.DataTable();
        data.addColumn('string', '車種');
        data.addColumn('number', '數量');
        data.addRows([
            ['大型車', 0],
            ['小型車', 0],
            ['機車', 0],
            ['自行車', 0]
        ]);

        // Set chart options
        var options = {
            'title': 'PCU',
            'width': 278,
            'height': 150
        };

        // Instantiate and draw our chart, passing in some options.
        hourlyVehicleChart = new google.visualization.ColumnChart(document.getElementById('HourlyVehicleChart'));
        hourlyVehicleChart.draw(data, options);
    }

    function updateHourlyVehicleChart(hourlyData) {
        var data = new google.visualization.DataTable();
        data.addColumn('string', '車種');
        data.addColumn('number', '數量');
        data.addRows([
            ['大型車', hourlyData.LargeVehicle],
            ['小型車', hourlyData.LightVehicle],
            ['機車', hourlyData.Motocycle],
            ['自行車', hourlyData.Bicycle]
        ]);

        hourlyVehicleChart.draw(data);
    }

    function updateInfoWindows(hourlyData) {
        // Cleaning
        infos.forEach(function (info) { info.close(); });
        infos = [];

        markers.forEach(function (marker) {
            var trafficData = hourlyData.TrafficData.filter(function (item) {
                return item.Intersection === marker.id;
            });
            var sum = trafficData.reduce(function (accumulator, currentValue) {
                return accumulator + currentValue.Amount;
            }, 0);

            var infoHtml, maxWidth;

            if (singleData.InvestigationType === 1 || singleData.InvestigationType === 2) {
                var leftData = trafficData.find(function (item) { return item.Direction === '左轉'; });
                var straightData = trafficData.find(function (item) { return item.Direction === '直行'; });
                var rightData = trafficData.find(function (item) { return item.Direction === '右轉'; });

                infoHtml = $('#IntersectionInfoWindows')
                    .clone().find('#sum').text(sum)
                    .end().find('#left').text(leftData.Amount + '(' + (Math.round(leftData.Amount / sum * 1000) / 10) + '%)')
                    .end().find('#straight').text(straightData.Amount + '(' + (Math.round(straightData.Amount / sum * 1000) / 10) + '%)')
                    .end().find('#right').text(rightData.Amount + '(' + (Math.round(rightData.Amount / sum * 1000) / 10) + '%)')
                    .end().html();

                maxWidth = 250;
            }

            if (singleData.InvestigationType === 4) {
                var aData = trafficData.find(function (item) { return item.Direction === 'A'; });
                var bData = trafficData.find(function (item) { return item.Direction === 'B'; });
                var cData = trafficData.find(function (item) { return item.Direction === 'C'; });
                var dData = trafficData.find(function (item) { return item.Direction === 'D'; });
                var eData = trafficData.find(function (item) { return item.Direction === 'E'; });

                infoHtml = $('#FivewayInfoWindows')
                    .clone().find('#sum').text(sum)
                    .end().find('#A').text(aData.Amount + '(' + (Math.round(aData.Amount / sum * 1000) / 10) + '%)')
                    .end().find('#B').text(bData.Amount + '(' + (Math.round(bData.Amount / sum * 1000) / 10) + '%)')
                    .end().find('#C').text(cData.Amount + '(' + (Math.round(cData.Amount / sum * 1000) / 10) + '%)')
                    .end().find('#D').text(dData.Amount + '(' + (Math.round(dData.Amount / sum * 1000) / 10) + '%)')
                    .end().find('#E').text(eData.Amount + '(' + (Math.round(eData.Amount / sum * 1000) / 10) + '%)')
                    .end().html();

                maxWidth = 350;
            }

            var info = new google.maps.InfoWindow({
                content: infoHtml,
                maxWidth: maxWidth
            });
            info.open(geeMap, marker);
            infos.push(info);
        });
    }
})(jQuery);