(function ($) {
    'use strict';

    var $this = $('.home-create, .home-edit'),
        centerMarker = null,
        strokeColor = '#F00',
        scale = 5,
        directA,
        directB,
        directC,
        directD,
        directE;

    if ($this.length === 0) {
        return;
    }
    $(function () {
        var model = $this.find('.content').data('model');
        if (model !== null) {
            // 調查資料編輯
            $('input[name="Id"]').val(model.id);
            $('#InvestigationType').val($('#HiddenInvestigationType').val());
            $('input[name = "InvestigaionTime"]').val(model.investigaionTime);
            $('textarea[name = "TrafficControlNote"]').val(model.trafficControlNote);
            positioningOfIntersection.Id = model.positioningId;
            positioningOfIntersection.CityName = model.positioningCity;
            positioningOfIntersection.TownName = model.positioningTown;
            positioningOfIntersection.Road1 = model.positioningRoad1;
            positioningOfIntersection.Road2 = model.positioningRoad2;
            positioningOfIntersection.Latitude = model.positioningLatitude;
            positioningOfIntersection.Longitude = model.positioningLongitude;
            $('body').append(
                $('<div id="Modal-upload-success">').append(
                    $('<input>').attr({
                        type: 'hidden',
                        id: 'FileIdentification',
                        name: 'FileIdentification',
                        value: model.fileName
                    })));

            createMarkersInEditMode(JSON.parse(model.positioning));
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
            positioningMarkers();
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
            $.ajax(url, {
                method: 'post',
                processData: false,
                contentType: false,
                data: formData
            }).done(function (data) {
                $('#Modal-upload-success').remove();
                $('body').append(data);
                $('#Modal-upload-success').modal({
                    backdrop: 'static',
                    keyboard: false
                });
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
        })
        .on('change', '.rotate', function () {
            var $this = $(this),
                direct;
            switch ($this.closest('.input-group').prop('id')) {
                case 'roadNameA':
                    direct = directA;
                    break;
                case 'roadNameB':
                    direct = directB;
                    break;
                case 'roadNameC':
                    direct = directC;
                    break;
                case 'roadNameD':
                    direct = directD;
                    break;
                case 'roadNameE':
                    direct = directE;
                    break;
                default:
            }

            var icon = direct.getIcon();
            icon.rotation = $this.val();
            direct.setIcon(icon);
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
            id: directA.id,
            name: $('#roadNameA > input[type="text"]').val(),
            latitude: directA.getPosition().lat(),
            longitude: directA.getPosition().lng(),
            rotate: $('#roadNameA > input[type="number"]').val()
        });

        markers.directions.push({
            id: directB.id,
            name: $('#roadNameB > input[type="text"]').val(),
            latitude: directB.getPosition().lat(),
            longitude: directB.getPosition().lng(),
            rotate: $('#roadNameB > input[type="number"]').val()
        });

        markers.directions.push({
            id: directC.id,
            name: $('#roadNameC > input[type="text"]').val(),
            latitude: directC.getPosition().lat(),
            longitude: directC.getPosition().lng(),
            rotate: $('#roadNameC > input[type="number"]').val()
        });

        if (selectedType === 'Intersection' || selectedType === 'Pedestrians' || selectedType === 'FiveWay') {
            markers.directions.push({
                id: directD.id,
                name: $('#roadNameD > input[type="text"]').val(),
                latitude: directD.getPosition().lat(),
                longitude: directD.getPosition().lng(),
                rotate: $('#roadNameD > input[type="number"]').val()
            });
        }

        if (selectedType === 'FiveWay') {
            markers.directions.push({
                id: directE.id,
                name: $('#roadNameE > input[type="text"]').val(),
                latitude: directE.getPosition().lat(),
                longitude: directE.getPosition().lng(),
                rotate: $('#roadNameE > input[type="number"]').val()
            });
        }

        return markers;
    }

    function positioningMarkers() {
        var selectedType = $('select[name="InvestigationType"] option:selected').val();

        // clean center and direct markers
        if (centerMarker) { centerMarker.setMap(null); centerMarker = null; }
        if (directA) { directA.setMap(null); directA = null; }
        if (directB) { directB.setMap(null); directB = null; }
        if (directC) { directC.setMap(null); directC = null; }
        if (directD) { directD.setMap(null); directD = null; }
        if (directE) { directE.setMap(null); directE = null; }

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
        switch (selectedType) {
            case 'TRoad':
                directA = updateMarker({
                    id: 'A',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 270,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude + .00015)
                });

                directB = updateMarker({
                    id: 'B',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 0,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .00015, positioningOfIntersection.Longitude)
                });

                directC = updateMarker({
                    id: 'C',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 90,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude - .00015)
                });

                $('#roadDirect div,#roadDirect br').remove();
                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameA')
                        .end()
                        .find('span.roadName')
                        .text('A')
                        .end()
                        .find('.rotate')
                        .attr('value', '270')
                        .end()
                        .html()
                );

                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameB')
                        .end()
                        .find('span.roadName')
                        .text('B')
                        .end()
                        .find('.rotate')
                        .attr('value', '0')
                        .end()
                        .html()
                );

                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameC')
                        .end()
                        .find('span.roadName')
                        .text('C')
                        .end()
                        .find('.rotate')
                        .attr('value', '90')
                        .end()
                        .html()
                );
                break;
            case 'Intersection':
                directA = updateMarker({
                    id: 'A',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 270,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude + .00015)
                });

                directB = updateMarker({
                    id: 'B',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 0,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .00015, positioningOfIntersection.Longitude)
                });

                directC = updateMarker({
                    id: 'C',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 90,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude - .00015)
                });
                directD = updateMarker({
                    id: 'D',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 180,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude + .00015, positioningOfIntersection.Longitude)
                });

                $('#roadDirect div,#roadDirect br').remove();
                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameA')
                        .end()
                        .find('span')
                        .text('A')
                        .end()
                        .find('.rotate')
                        .attr('value', '270')
                        .end()
                        .html()
                );

                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameB')
                        .end()
                        .find('span')
                        .text('B')
                        .end()
                        .find('.rotate')
                        .attr('value', '0')
                        .end()
                        .html()
                );

                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameC')
                        .end()
                        .find('span')
                        .text('C')
                        .end()
                        .find('.rotate')
                        .attr('value', '90')
                        .end()
                        .html()
                );
                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameD')
                        .end()
                        .find('span')
                        .text('D')
                        .end()
                        .find('.rotate')
                        .attr('value', '180')
                        .end()
                        .html()
                );
                break;
            case 'Pedestrians':
                directA = updateMarker({
                    id: 'A',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 270,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude + .00015)
                });

                directB = updateMarker({
                    id: 'B',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 0,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .00015, positioningOfIntersection.Longitude)
                });

                directC = updateMarker({
                    id: 'C',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 90,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude - .00015)
                });
                directD = updateMarker({
                    id: 'D',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 180,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude + .00015, positioningOfIntersection.Longitude)
                });

                $('#roadDirect div,#roadDirect br').remove();
                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameA')
                        .end()
                        .find('span')
                        .text('A')
                        .end()
                        .find('.rotate')
                        .attr('value', '270')
                        .end()
                        .html()
                );

                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameB')
                        .end()
                        .find('span')
                        .text('B')
                        .end()
                        .find('.rotate')
                        .attr('value', '0')
                        .end()
                        .html()
                );

                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameC')
                        .end()
                        .find('span')
                        .text('C')
                        .end()
                        .find('.rotate')
                        .attr('value', '90')
                        .end()
                        .html()
                );
                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameD')
                        .end()
                        .find('span')
                        .text('D')
                        .end()
                        .find('.rotate')
                        .attr('value', '180')
                        .end()
                        .html()
                );
                break;

            case 'FiveWay':
                directA = updateMarker({
                    id: 'A',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 270,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude + .00015)
                });

                directB = updateMarker({
                    id: 'B',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 300,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .0001, positioningOfIntersection.Longitude + .00013)
                });

                directC = updateMarker({
                    id: 'C',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 30,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .00015, positioningOfIntersection.Longitude - .00013)
                });
                directD = updateMarker({
                    id: 'D',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 90,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude - .00015)
                });
                directE = updateMarker({
                    id: 'E',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 180,
                        scale: scale,
                        strokeColor: strokeColor
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude + .00015, positioningOfIntersection.Longitude)
                });

                $('#roadDirect div,#roadDirect br').remove();
                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameA')
                        .end()
                        .find('span')
                        .text('A')
                        .end()
                        .find('.rotate')
                        .attr('value', '270')
                        .end()
                        .html()
                );

                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameB')
                        .end()
                        .find('span')
                        .text('B')
                        .end()
                        .find('.rotate')
                        .attr('value', '300')
                        .end()
                        .html()
                );

                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameC')
                        .end()
                        .find('span')
                        .text('C')
                        .end()
                        .find('.rotate')
                        .attr('value', '30')
                        .end()
                        .html()
                );
                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameD')
                        .end()
                        .find('span')
                        .text('D')
                        .end()
                        .find('.rotate')
                        .attr('value', '90')
                        .end()
                        .html()
                );
                $('#roadDirect').append(
                    $('#roadDirectName').clone()
                        .find('div.input-group')
                        .attr('id', 'roadNameE')
                        .end()
                        .find('span')
                        .text('E')
                        .end()
                        .find('.rotate')
                        .attr('value', '180')
                        .end()
                        .html()
                );
                break;
            default:
        }

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

        var objectA = positioning.directions.find(function (o) { return o.id === 'A' });
        var objectB = positioning.directions.find(function (o) { return o.id === 'B' });
        var objectC = positioning.directions.find(function (o) { return o.id === 'C' });
        var objectD = positioning.directions.find(function (o) { return o.id === 'D' });
        var objectE = positioning.directions.find(function (o) { return o.id === 'E' });

        // update direct markers
        $('#roadDirect div,#roadDirect br').remove();

        if (objectA) {
            directA = updateMarker({
                id: 'A',
                icon: {
                    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                    rotation: objectA.rotate,
                    scale: scale,
                    strokeColor: strokeColor
                },
                draggable: true,
                latLng: new google.maps.LatLng(objectA.latitude, objectA.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('div.input-group')
                    .attr('id', 'roadNameA')
                    .end()
                    .find('.roadtext')
                    .attr('value', objectA.name)
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
            directB = updateMarker({
                id: 'B',
                icon: {
                    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                    rotation: objectB.rotate,
                    scale: scale,
                    strokeColor: strokeColor
                },
                draggable: true,
                latLng: new google.maps.LatLng(objectB.latitude, objectB.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('div.input-group')
                    .attr('id', 'roadNameB')
                    .val(objectB.name)
                    .end()
                    .find('.roadtext')
                    .attr('value', objectB.name)
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
            directC = updateMarker({
                id: 'C',
                icon: {
                    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                    rotation: objectC.rotate,
                    scale: scale,
                    strokeColor: strokeColor
                },
                draggable: true,
                latLng: new google.maps.LatLng(objectC.latitude, objectC.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('div.input-group')
                    .attr('id', 'roadNameC')
                    .val(objectC.name)
                    .end()
                    .find('.roadtext')
                    .attr('value', objectC.name)
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
            directD = updateMarker({
                id: 'D',
                icon: {
                    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                    rotation: objectD.rotate,
                    scale: scale,
                    strokeColor: strokeColor
                },
                draggable: true,
                latLng: new google.maps.LatLng(objectD.latitude, objectD.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('div.input-group')
                    .attr('id', 'roadNameD')
                    .val(objectD.name)
                    .end()
                    .find('.roadtext')
                    .attr('value', objectD.name)
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
            directE = updateMarker({
                id: 'E',
                icon: {
                    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                    rotation: objectE.rotate,
                    scale: scale,
                    strokeColor: strokeColor
                },
                draggable: true,
                latLng: new google.maps.LatLng(objectE.latitude, objectE.longitude)
            });

            $('#roadDirect').append(
                $('#roadDirectName').clone()
                    .find('div.input-group')
                    .attr('id', 'roadNameE')
                    .val(objectE.name)
                    .end()
                    .find('.roadtext')
                    .attr('value', objectE.name)
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
})(jQuery);