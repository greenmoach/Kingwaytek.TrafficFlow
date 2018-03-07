(function ($) {
    'use strict';
    var $this = $('.home-query'),
        singleData,
        hourlyVehicleChart,
        centerMarker,
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
        .on('click', '#next-1', function () {
            submitQuery();
        })
        .on('click', '#close-info', function () {
            $(".div-Info").fadeOut("fast");
        })
        .on('change', '#HourlyIntervals', function () {
            var hourlyData = singleData.HourlyIntervals.find(function (item) {
                return item.HourlyInterval === $('#HourlyIntervals option:selected').val();
            });
            updateInfoWindows(hourlyData);
            updateHourlyVehicleChart(hourlyData);
        });

    function submitQuery(dateTime) {
        var data = {
            queryType: $('input[name="area"]:checked').val(),
            positioningId: positioningOfIntersection.Id,
            investigaionTime: dateTime
        };

        $.post('/home/query', data)
            .done(function (data) {
                singleData = data;
                initDisplayObjects();
            })
            .fail(function () {
                alert('該路口尚未有調查資料');
            });
    }

    function initDisplayObjects() {
        // cleaning markers
        if (centerMarker) if (centerMarker) { centerMarker.setMap(null); centerMarker = null; }
        if (markers.length !== 0) {
            markers.forEach(function (marker) {
                if (marker) {
                    marker.setMap(null);
                    marker = null;
                }
            });
            markers = [];
        }

        // Hide hourly query panel
        $('.div-Info').fadeOut('fast');

        // 清空info windows
        if (infos.length !== 0) {
            infos.forEach(function (info) { info.close(); });
            infos = [];
        }

        var centerIcon,
            directionIcon;
        // 車流量 or 行人量
        if (singleData.InvestigationType === 3) {
            centerIcon = {
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(20, 20),
                url: '/content/images/arrow-walking02.svg'
            };
            directionIcon = '/content/images/arrow-walking01.svg';
        }
        else {
            centerIcon = '/content/images/center.svg';
            directionIcon = '/content/images/arrow-01.svg';
        }

        var positionObj = JSON.parse(singleData.Positioning);
        var centerLatLng = new google.maps.LatLng(positionObj.center.latitude, positionObj.center.longitude);
        centerMarker = new google.maps.Marker({
            id: 'center',
            position: centerLatLng,
            icon: centerIcon,
            draggable: false,
            map: geeMap
        });

        centerMarker.addListener('click',
            function () {
                $('.div-Info').fadeToggle('fast', function () {
                    if ($('.div-Info').is(':visible')) {
                        // 顯示調查資料
                        var hourlyData = singleData.HourlyIntervals.find(function (item) {
                            return item.HourlyInterval === $('#HourlyIntervals option:selected').val();
                        });
                        updateInfoWindows(hourlyData);
                        updateHourlyVehicleChart(hourlyData);
                    } else {
                        // 清空info windows
                        if (infos.length !== 0) {
                            infos.forEach(function (info) { info.close(); });
                            infos = [];
                        }
                    }
                });
            });

        positionObj.directions.forEach(function (d) {
            markers.push(new google.maps.Marker({
                id: d.id,
                position: new google.maps.LatLng(d.latitude, d.longitude),
                icon: directionIcon,
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

        $('#InvestigaionTime').val(singleData.InvestigaionTime);
        $('#IntersectionId').text(singleData.IntersectionId === null ? '' : singleData.IntersectionId);
        $('#IntersectionName').text(singleData.IntersectionName === null ? '' : singleData.IntersectionName);
        $('#Weather').text(singleData.Weather === null ? '' : singleData.Weather);
        $('#TrafficControlNote').text(singleData.TrafficControlNote === null ? '' : singleData.TrafficControlNote);
        $('#DownloadInvestigaion').attr('href', '/home/DownloadInvestigation?filename=' + singleData.FileIdentification);

        // 車種統計圖
        initHourlyVehicleChart();

        // 調查日期時間選擇器
        $('.t_date').datetimepicker({
            timepicker: false,
            defaultDate: singleData.InvestigaionTime,
            format: 'Y/m/d',
            lang: 'zh-TW',
            allowDates: singleData.OtherInvestigaionTime,
            onSelectDate: function (ct, $i) {
                submitQuery(ct.toLocaleDateString());
            }
        });

        geeMap.panTo(centerLatLng);
        geeMap.setZoom(21);
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
        if (singleData.InvestigationType === 3) {
            $('#HourlyVehicleChart').hide();
        }
        else {
            $('#HourlyVehicleChart').show();
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
    }

    function updateInfoWindows(hourlyData) {
        // Cleaning
        if (infos.length !== 0) {
            infos.forEach(function (info) { info.close(); });
            infos = [];
        }

        // 車流量 or 行人量
        if (singleData.InvestigationType === 3) {
            var aData = hourlyData.TrafficData.find(function (item) {
                return item.Intersection === 'A' && item.Direction === 'BD';
            });
            var bData = hourlyData.TrafficData.find(function (item) {
                return item.Intersection === 'B' && item.Direction === 'AC';
            });
            var cData = hourlyData.TrafficData.find(function (item) {
                return item.Intersection === 'C' && item.Direction === 'BD';
            });
            var dData = hourlyData.TrafficData.find(function (item) {
                return item.Intersection === 'D' && item.Direction === 'AC';
            });
            var crossData = hourlyData.TrafficData.filter(function (item) {
                return item.Direction === 'AD2BC' || item.Direction === 'AB2CD';
            });

            var crossAmount = crossData.reduce(function (accumulator, currentValue) {
                return accumulator + currentValue.Amount;
            }, 0);

            var sum = aData.Amount + bData.Amount + cData.Amount + dData.Amount + crossAmount;

            var infoHtml = $('#PedestriansInfoWindows')
                .clone().find('#sum').text(sum)
                .end().find('#A').text(aData.Amount)
                .end().find('#B').text(bData.Amount)
                .end().find('#C').text(cData.Amount)
                .end().find('#D').text(dData.Amount)
                .end().find('#Cross').text(crossAmount)
                .end().html();

            var info = new google.maps.InfoWindow({
                content: infoHtml,
                maxWidth: 300
            });
            info.open(geeMap, centerMarker);
            infos.push(info);
        }
        else {
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
    }
})(jQuery);