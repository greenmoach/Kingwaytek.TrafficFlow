(function ($) {
    'use strict';

    var $this = $('.home-create'),
        centerMarker = null,
        directA,
        directB,
        directC,
        directD,
        directE;

    if ($this.length === 0) {
        return;
    }

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
            }

            if (file.type !== 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet') {
                alert('上傳的調查資料檔案類型非EXCEL');
            }

            // 上傳檔案上限10MB
            if (file.size > 10 * 1024 * 1024) {
                alert('檔案大小超過10MB');
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
        .on('click', '#ImportInvestigationData', function () {
            var data = {
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
                trafficControlNote: $('input[name = "TrafficControlNote"]').val(),
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
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude + .00015)
                });

                directB = updateMarker({
                    id: 'B',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 0,
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .00015, positioningOfIntersection.Longitude)
                });

                directC = updateMarker({
                    id: 'C',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 90,
                        scale: 5
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
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude + .00015)
                });

                directB = updateMarker({
                    id: 'B',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 0,
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .00015, positioningOfIntersection.Longitude)
                });

                directC = updateMarker({
                    id: 'C',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 90,
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude - .00015)
                });
                directD = updateMarker({
                    id: 'D',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 180,
                        scale: 5
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
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude + .00015)
                });

                directB = updateMarker({
                    id: 'B',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 0,
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .00015, positioningOfIntersection.Longitude)
                });

                directC = updateMarker({
                    id: 'C',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 90,
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude - .00015)
                });
                directD = updateMarker({
                    id: 'D',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 180,
                        scale: 5
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
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude + .00015)
                });

                directB = updateMarker({
                    id: 'B',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 300,
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .0001, positioningOfIntersection.Longitude + .00013)
                });

                directC = updateMarker({
                    id: 'C',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 30,
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude - .00015, positioningOfIntersection.Longitude - .00013)
                });
                directD = updateMarker({
                    id: 'D',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 90,
                        scale: 5
                    },
                    draggable: true,
                    latLng: new google.maps.LatLng(positioningOfIntersection.Latitude, positioningOfIntersection.Longitude - .00015)
                });
                directE = updateMarker({
                    id: 'E',
                    icon: {
                        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                        rotation: 180,
                        scale: 5
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