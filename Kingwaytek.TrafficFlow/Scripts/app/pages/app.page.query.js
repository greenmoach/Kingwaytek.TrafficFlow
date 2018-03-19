(function ($) {
    'use strict';
    var $this = $('.home-query'),
        singleData,
        hourlyVehicleChart,
        historicalVehicleChart,
        historicalDirectChart,
        centerMarker,
        strokeColor = '#FF0000',
        scale = 5,
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
            $('.div-Info').fadeOut('fast');
        })
        .on('click', '#close-history', function () {
            $('.div-history').fadeOut('fast');
        })
        .on('click', '#close-turn-history', function () {
            $('.div-turn-history').fadeOut('fast');
        })
        .on('change', '#HourlyIntervals', function () {
            var hourlyData = singleData.HourlyIntervals.find(function (item) {
                return item.HourlyInterval === $('#HourlyIntervals option:selected').val();
            });
            updateInfoWindows(hourlyData);
            updateHourlyVehicleChart(hourlyData);
        })
        .on('change', '#InvestigaionTime', function () {
            var investigaionTime = $('#InvestigaionTime option:selected').val();
            submitQuery(investigaionTime);
        })
        .on('click', '#div-survey', function () {
            var parameters = {
                queryType: $('input[name="area"]:checked').val(),
                hourlyInterval: $('#HourlyIntervals option:selected').val(),
                positioningId: positioningOfIntersection.Id
            };

            $.post(sitepath + 'home/VehicleHistoricalData', parameters)
                .done(function (data) {
                    $('.div-history').fadeIn();
                    updateHistoricalVehicleChart(data);
                })
                .fail(function () {
                    $('.div-history').fadeOut();
                    alert('取得歷次調查車種資料失敗');
                });
        });

    // Set div draggable
    $('.div-history').draggable({
        handle: 'legend',
        containment: ".gis-map",
        scroll: false
    });

    $('.div-turn-history').draggable({
        handle: 'legend',
        containment: ".gis-map",
        scroll: false
    });

    function submitQuery(dateTime) {
        var parameters = {
            queryType: $('input[name="area"]:checked').val(),
            positioningId: positioningOfIntersection.Id,
            investigaionTime: dateTime
        };

        $.post(sitepath + 'home/query', parameters)
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
        $('.div-history').fadeOut('fast');
        $('.div-turn-history').fadeOut('fast');

        // 清空info windows
        if (infos.length !== 0) {
            infos.forEach(function (info) { info.close(); });
            infos = [];
        }

        var centerIcon;
        // 車流量 or 行人量
        if (singleData.InvestigationType === 3) {
            centerIcon = {
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(20, 20),
                url: sitepath + 'content/images/arrow-walking02.svg'
            };
        }
        else {
            centerIcon = sitepath + 'content/images/center.svg';
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
            var m = new google.maps.Marker({
                id: d.id,
                position: new google.maps.LatLng(d.latitude, d.longitude),
                icon: {
                    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                    rotation: d.rotate,
                    scale: scale,
                    strokeColor: strokeColor
                },
                draggable: false,
                map: geeMap
            });
            if (singleData.InvestigationType !== 3) {
                m.addListener('click', function () {
                    // 歷年轉向變化查詢
                    var parameters = {
                        Intersection: m.id,
                        hourlyInterval: $('#HourlyIntervals option:selected').val(),
                        positioningId: positioningOfIntersection.Id
                    };

                    $.post(sitepath + 'home/DirectHistoricalData', parameters)
                        .done(function (data) {
                            $('.div-turn-history').fadeIn();
                            updateHistoricalDirectChart(data);
                        })
                        .fail(function () {
                            $('.div-turn-history').fadeOut();
                            alert('取得歷次調查車種資料失敗');
                        });
                });
            }
            markers.push(m);
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
        $('#DownloadInvestigaion').attr('href', sitepath + 'home/DownloadInvestigation?filename=' + singleData.FileIdentification);

        // 車種統計圖
        initHourlyVehicleChart();

        // 歷次調查車種比例變化統計圖
        initHistoricalVehicleChart();

        // 歷年轉向流量變化統計圖
        initHistoricalDirectChart();

        // 調查日期時間選擇器
        $('#InvestigaionTime').html('');
        singleData.OtherInvestigaionTime.forEach(function (otherTime) {
            $('<option value="' + otherTime + '">' + otherTime + '</option>').appendTo('#InvestigaionTime');
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

    function initHistoricalVehicleChart() {
        var isVehicle = $('input[name="area"]:checked').val() === 'vehicle';
        $('#div-survey').text(isVehicle ? '歷次調查車種比例變化' : '歷次行人量數據變化');
        $('.div-history').find('legend').text(isVehicle ? '歷次調查車種比例變化' : '歷次行人量數據變化');

        // Create the data table.
        var data = new google.visualization.DataTable();

        // Set chart options
        var options = {
            width: 288,
            height: 234,
            curveType: 'function',
            legend: { position: 'bottom' }
        };

        // Instantiate and draw our chart, passing in some options.
        historicalVehicleChart = new google.visualization.LineChart(document.getElementById('HistoricalVehicleChart'));
        historicalVehicleChart.draw(data, options);
    }

    function initHistoricalDirectChart() {
        // Create the data table.
        var data = new google.visualization.DataTable();

        // Set chart options
        var options = {
            width: 288,
            height: 234,
            curveType: 'function',
            legend: { position: 'bottom' }
        };

        // Instantiate and draw our chart, passing in some options.
        historicalDirectChart = new google.visualization.LineChart(document.getElementById('HistoricalDirectChart'));
        historicalDirectChart.draw(data, options);
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

    function updateHistoricalVehicleChart(objects) {
        var rows, data;
        if ($('input[name="area"]:checked').val() === 'vehicle') {
            rows = objects.map(function (element) {
                return [
                    element.InvestigaionTime, element.LargeVehicle, element.LightVehicle, element.Motocycle, element.Bicycle
                ];
            });

            data = new google.visualization.DataTable();
            data.addColumn('string', '日期');
            data.addColumn('number', '大型車');
            data.addColumn('number', '小型車');
            data.addColumn('number', '機車');
            data.addColumn('number', '自行車');
            data.addRows(rows);
            historicalVehicleChart.draw(data);
        } else {
            rows = objects.map(function (element) {
                return [
                    element.InvestigaionTime, element.Pedestrians
                ];
            });

            data = new google.visualization.DataTable();
            data.addColumn('string', '日期');
            data.addColumn('number', '行人');
            data.addRows(rows);
            historicalVehicleChart.draw(data);
        }
    }

    function updateHistoricalDirectChart(objects) {
        var rows = objects.map(function (element) {
            var row = Object.values(element.Directions);
            row.unshift(element.InvestigaionTime);
            return row;
        });

        var data = new google.visualization.DataTable();
        data.addColumn('string', '日期');
        Object.keys(objects[0].Directions).forEach(function (e) {
            data.addColumn('number', e);
        });
        data.addRows(rows);

        historicalDirectChart.draw(data);
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