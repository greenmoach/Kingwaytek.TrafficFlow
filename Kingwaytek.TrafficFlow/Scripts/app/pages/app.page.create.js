(function ($) {
    'use strict';

    var $this = $('.home-create, .home-edit'),
        centerMarker = null,
        directSet =
            {
                directA: null,
                directB: null,
                directC: null,
                directD: null,
                directE: null
            };

    if ($this.length === 0) {
        return;
    }

    var markerOption =
        {
            fillColor: 'red',
            fillOpacity: 1,
            scale: 1,
            strokeColor: 'red',
            strokeWeight: 1,
            anchor: new google.maps.Point(21, 16)
        };

    $(function () {
        var model = $this.find('.content').data('model');
        if (model !== null) {
            // 調查資料編輯
            $('input[name="Id"]').val(model.id);
            $('#InvestigationType').val($('#HiddenInvestigationType').val());
            $('input[name = "InvestigaionTime"]').val(model.investigaionTime);
            $('textarea[name = "TrafficControlNote"]').val(model.trafficControlNote);
            positioningOfIntersection.Id = model.positioningId;
            positioningOfIntersection.CityName = model.positioning.city;
            positioningOfIntersection.TownName = model.positioning.town;
            positioningOfIntersection.Road1 = model.positioning.road1;
            positioningOfIntersection.Road2 = model.positioning.road2;
            positioningOfIntersection.Latitude = model.positioning.latitude;
            positioningOfIntersection.Longitude = model.positioning.longitude;
            $('body').append(
                $('<div id="Modal-upload-success">').append(
                    $('<input>').attr({
                        type: 'hidden',
                        id: 'FileIdentification',
                        name: 'FileIdentification',
                        value: model.fileName
                    })));

            createMarkersInEditMode(JSON.parse(model.positioning.positioning1));
            $('.left-accordion').accordion({ active: 1 });
        }
    });

    $('.traffic-create').parent().addClass('active');

    $('.t_date').datetimepicker({
        closeOnDateSelect: true,
        timepicker: false,
        defaultDate: new Date(),
        maxDate: new Date(),
        format: 'Y/m/d',
        lang: 'zh-TW'
    });

    $this
        .on('click',
        '#next-1',
        function () {
            //this will open 1st accordian.
            $.ajax({
                url: sitepath + 'Location/GetDirects?positionId=' + positioningOfIntersection.Id + '&latitude=' + positioningOfIntersection.Latitude + '&longitude=' + positioningOfIntersection.Longitude + '&type=' + $('select[name="InvestigationType"] option:selected').val(),
                method: 'POST'
            })
                .done(function (data) {
                    positioningOfIntersection.Positioning = data;
                    positioningMarkers();
                    $('.left-accordion').accordion({ active: 1 });
                })
                .fail(function () {
                    positioningOfIntersection.Positioning = {};
                    alert('路口定位資料取得失敗');
                });
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
        .on('click', '#FileUploadButton', function () {
            // 檢查調查日期是否填寫
            if (!$('input[name="InvestigaionTime"]').val()) {
                alert('請先選擇調查日期');
                return;
            }

            var formData = new FormData(),
                url = sitepath + 'Home/UploadInvestigation',
                file = document.getElementById('FileUpload').files[0];
            if (!file) {
                alert('請選擇要會上傳的調查資料');
                return;
            }

            if (file.type !== 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet') {
                alert('上傳的調查資料檔案類型非EXCEL');
                return;
            }

            // 上傳檔案上限10MB
            if (file.size > 10 * 1024 * 1024) {
                alert('檔案大小超過10MB');
                return;
            }

            formData.append('file', file);
            formData.append('type', $('#InvestigationType').find('option:selected').val());
            formData.append('date', $('input[name="InvestigaionTime"]').val());
            formData.append('positioningId', positioningOfIntersection.Id);
            $.ajax(url, {
                method: 'post',
                processData: false,
                contentType: false,
                data: formData
            }).done(function (data) {
                $('#Modal-upload-success').remove();
                $('body').append(data);

                if ($('#HasOverlayData').val() === 'True') {
                    var r = confirm('匯入資料已有部份時段存有資料，是否進行覆蓋?');
                    if (r) {
                        $('#Modal-upload-success').modal({
                            backdrop: 'static',
                            keyboard: false
                        });
                    }
                } else {
                    $('#Modal-upload-success').modal({
                        backdrop: 'static',
                        keyboard: false
                    });
                }
            }).fail(function () {
                alert('檔案上傳錯誤，請確認網路設定並稍後再試');
            });
        })
        .on('click', '#ImportInvestigationData, #SubmitNoUpload', function () {
            var data = {
                id: $('input[name="Id"]').val(),
                type: $('#InvestigationType').find('option:selected').val(),
                positioningId: positioningOfIntersection.Id,
                city: positioningOfIntersection.CityName,
                town: positioningOfIntersection.TownName,
                road1: positioningOfIntersection.Road1,
                road2: positioningOfIntersection.Road2,
                latitude: positioningOfIntersection.Latitude,
                longitude: positioningOfIntersection.Longitude,
                positioning: JSON.stringify(getPositionInfoOfMarkers()),
                investigaionTime: $('input[name = "InvestigaionTime"]').val(),
                trafficControlNote: $('textarea[name = "TrafficControlNote"]').val(),
                weather: $('#Weather').val(),
                fileIdentification: $('#FileIdentification').val(),
                intersectionId: $('#IntersectionId').val()
            }

            $.post(sitepath + 'home/create', data)
                .done(function () {
                    alert('調查資料匯入成功');
                    location.reload();
                })
                .fail(function () {
                    alert('調查資料匯入失敗，請與系統管理員聯繫');
                });
        });

    function getPositionInfoOfMarkers() {
        var selectedType = $('select[name="InvestigationType"] option:selected').val();

        var markers = {
            center: {
                latitude: centerMarker.getPosition().lat(),
                longitude: centerMarker.getPosition().lng()
            },
            directions: []
        };

        markers.directions.push({
            id: directSet.directA.id,
            latitude: directSet.directA.getPosition().lat(),
            longitude: directSet.directA.getPosition().lng(),
            rotate: $('.roadName:contains("A")').siblings('.rotate').val()
        });

        markers.directions.push({
            id: directSet.directB.id,
            latitude: directSet.directB.getPosition().lat(),
            longitude: directSet.directB.getPosition().lng(),
            rotate: $('.roadName:contains("B")').siblings('.rotate').val()
        });

        markers.directions.push({
            id: directSet.directC.id,
            latitude: directSet.directC.getPosition().lat(),
            longitude: directSet.directC.getPosition().lng(),
            rotate: $('.roadName:contains("C")').siblings('.rotate').val()
        });

        if (selectedType === 'Intersection' || selectedType === 'Pedestrians' || selectedType === 'FiveWay') {
            markers.directions.push({
                id: directSet.directD.id,
                latitude: directSet.directD.getPosition().lat(),
                longitude: directSet.directD.getPosition().lng(),
                rotate: $('.roadName:contains("D")').siblings('.rotate').val()
            });
        }

        if (selectedType === 'FiveWay') {
            markers.directions.push({
                id: directSet.directE.id,
                latitude: directSet.directE.getPosition().lat(),
                longitude: directSet.directE.getPosition().lng(),
                rotate: $('.roadName:contains("E")').siblings('.rotate').val()
            });
        }

        return markers;
    }

    function positioningMarkers() {
        var selectedType = $('select[name="InvestigationType"] option:selected').val();

        // clean center and direct markers
        if (centerMarker) { centerMarker.setMap(null); centerMarker = null; }
        ['A', 'B', 'C', 'D', 'E'].forEach(function (dir) {
            var direct = directSet['direct' + dir];
            if (direct) { direct.setMap(null); direct = null; }
        });

        // update conter marker
        var centerLatLng =
            new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude);
        var icon;
        if (selectedType === 'Pedestrians') {
            icon = {
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(20, 20),
                url: sitepath + 'content/images/arrow-walking02.svg'
            }
        } else {
            icon = sitepath + 'content/images/center.svg';
        }
        centerMarker = updateMarker({
            id: 'center',
            icon: icon,
            latLng: centerLatLng,
            draggable: false
        });

        // update direct markers
        var anchor, path;
        switch (selectedType) {
            case 'TRoad':
                anchor = new google.maps.Point(21, 20);
                path = kingwaytek.SymbolPath.TRoad;

                break;
            case 'Intersection':
                anchor = new google.maps.Point(25, 20);
                path = kingwaytek.SymbolPath.Intersection;

                break;
            case 'Pedestrians':
                anchor = new google.maps.Point(7, 24);
                path = kingwaytek.SymbolPath.Pedestrians;
                break;

            case 'FiveWay':
                anchor = new google.maps.Point(28, 20);
                path = kingwaytek.SymbolPath.FiveWay;
                break;
            default:
        }

        $('#roadDirect div,#roadDirect br').remove();
        var positioning = JSON.parse(positioningOfIntersection.Positioning);
        positioning.directions.forEach(function (direct) {
            directSet['direct' + direct.id] = updateMarker({
                id: direct.id,
                icon: $.extend(Object.assign({}, markerOption), {
                    anchor: anchor,
                    path: path,
                    rotation: direct.rotate
                }),
                draggable: true,
                latLng: new google.maps.LatLng(direct.latitude, direct.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('.slider')
                    .attr('data-slider-value', direct.rotate)
                    .end()
                    .find('span.roadName')
                    .text(direct.id)
                    .end()
                    .find('.rotate')
                    .attr('value', direct.rotate)
                    .end()
                    .html());
        });

        $('#roadDirect').find('.slider').bootstrapSlider();
        $('#roadDirect').find('.slider').on('slide', function (slideEvt) {
            var $target = $(slideEvt.target);
            $target.parent().next().find('.rotate').val(slideEvt.value);
            rotateMarker($target.parent().next().find('span.roadName').text(), slideEvt.value);
        });

        geeMap.panTo(centerLatLng);
        geeMap.setZoom(21);
    }

    function createMarkersInEditMode(positioning) {
        var selectedType = $('select[name="InvestigationType"] option:selected').val();
        // update conter marker
        var centerLatLng =
            new google.maps.LatLng(positioning.center.latitude, positioning.center.longitude);
        var icon;
        if (selectedType === 'Pedestrians') {
            icon = {
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(20, 20),
                url: sitepath + 'content/images/arrow-walking02.svg'
            }
        } else {
            icon = sitepath + 'content/images/center.svg';
        }
        centerMarker = updateMarker({
            id: 'center',
            icon: icon,
            latLng: centerLatLng,
            draggable: false
        });

        var iconPath = {
            TRoad: {
                anchor: new google.maps.Point(21, 20),
                path: kingwaytek.SymbolPath.TRoad
            },
            Intersection: {
                anchor: new google.maps.Point(25, 20),
                path: kingwaytek.SymbolPath.Intersection
            },
            Pedestrians: {
                anchor: new google.maps.Point(7, 24),
                path: kingwaytek.SymbolPath.Pedestrians
            },
            FiveWay: {
                anchor: new google.maps.Point(28, 20),
                path: kingwaytek.SymbolPath.FiveWay
            }
        };

        var objectA = positioning.directions.find(function (o) { return o.id === 'A' });
        var objectB = positioning.directions.find(function (o) { return o.id === 'B' });
        var objectC = positioning.directions.find(function (o) { return o.id === 'C' });
        var objectD = positioning.directions.find(function (o) { return o.id === 'D' });
        var objectE = positioning.directions.find(function (o) { return o.id === 'E' });

        // update direct markers
        $('#roadDirect div,#roadDirect br').remove();

        if (objectA) {
            directSet.directA = updateMarker({
                id: 'A',
                icon: $.extend(Object.assign({}, markerOption), iconPath[selectedType], {
                    rotation: objectA.rotate
                }),
                draggable: true,
                latLng: new google.maps.LatLng(objectA.latitude, objectA.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('.slider')
                    .attr('data-slider-value', objectA.rotate)
                    .end()
                    .find('span.roadName')
                    .text('A')
                    .end()
                    .find('.rotate')
                    .attr('value', objectA.rotate)
                    .end()
                    .html()
            );
        }

        if (objectB) {
            directSet.directB = updateMarker({
                id: 'B',
                icon: $.extend(Object.assign({}, markerOption), iconPath[selectedType], {
                    rotation: objectB.rotate
                }),
                draggable: true,
                latLng: new google.maps.LatLng(objectB.latitude, objectB.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('.slider')
                    .attr('data-slider-value', objectB.rotate)
                    .end()
                    .find('span.roadName')
                    .text('B')
                    .end()
                    .find('.rotate')
                    .attr('value', objectB.rotate)
                    .end()
                    .html()
            );
        }

        if (objectC) {
            directSet.directC = updateMarker({
                id: 'C',
                icon: $.extend(Object.assign({}, markerOption), iconPath[selectedType], {
                    rotation: objectC.rotate
                }),
                draggable: true,
                latLng: new google.maps.LatLng(objectC.latitude, objectC.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('.slider')
                    .attr('data-slider-value', objectC.rotate)
                    .end()
                    .find('span.roadName')
                    .text('C')
                    .end()
                    .find('.rotate')
                    .attr('value', objectC.rotate)
                    .end()
                    .html()
            );
        }

        if (objectD) {
            directSet.directD = updateMarker({
                id: 'D',
                icon: $.extend(Object.assign({}, markerOption), iconPath[selectedType], {
                    rotation: objectD.rotate
                }),
                draggable: true,
                latLng: new google.maps.LatLng(objectD.latitude, objectD.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('.slider')
                    .attr('data-slider-value', objectD.rotate)
                    .end()
                    .find('span.roadName')
                    .text('D')
                    .end()
                    .find('.rotate')
                    .attr('value', objectD.rotate)
                    .end()
                    .html()
            );
        }

        if (objectE) {
            directSet.directE = updateMarker({
                id: 'E',
                icon: $.extend(Object.assign({}, markerOption), iconPath[selectedType], {
                    rotation: objectE.rotate
                }),
                draggable: true,
                latLng: new google.maps.LatLng(objectE.latitude, objectE.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('.slider')
                    .attr('data-slider-value', objectE.rotate)
                    .end()
                    .find('span.roadName')
                    .text('E')
                    .end()
                    .find('.rotate')
                    .attr('value', objectE.rotate)
                    .end()
                    .html()
            );
        }

        $('#roadDirect').find('.slider').bootstrapSlider();
        $('#roadDirect').find('.slider').on('slide', function (slideEvt) {
            var $target = $(slideEvt.target);
            $target.parent().next().find('.rotate').val(slideEvt.value);
            rotateMarker($target.parent().next().find('span.roadName').text(), slideEvt.value);
        });

        geeMap.panTo(centerLatLng);
        geeMap.setZoom(21);
    }

    function updateMarker(markerOpts) {
        var marker = new google.maps.Marker({
            id: markerOpts.id,
            position: markerOpts.latLng,
            icon: markerOpts.icon,
            draggable: markerOpts.draggable,
            map: geeMap
        });

        return marker;
    }

    function rotateMarker(code, rotate) {
        var direct = directSet['direct' + code];

        var icon = direct.getIcon();
        icon.rotation = rotate;
        direct.setIcon(icon);
    }
})(jQuery);