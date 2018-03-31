$(function () {
    //tooltip
    $('.map-tools').tooltip({
        position: {
            my: "center top+1",
            at: "center bottom",
            using: function (position, feedback) {
                $(this).css(position);
                $("<div>")
                    .addClass("arrow-tooltip top")
                    .addClass(feedback.vertical)
                    .addClass(feedback.horizontal)
                    .appendTo(this);
            }
        }
    });
});

//左側縮合
$(document).ready(function () {
    //隱藏
    $('.btn-toggle.l-hide').click(function () {
        $('.gis-left,.gis-right,.page-right').animate({ left: '-=325px' }, 300, function () {
            // 右邊測攔縮合，重新調整地圖大小
            google.maps.event.trigger(geeMap, 'resize');
        });
        $(this).hide(); $('.l-show').show();
    });
    //展開
    $('.btn-toggle.l-show').click(function () {
        $('.gis-left,.gis-right,.page-right').animate({ left: '+=325px' }, 300, function () {
            // 右邊測攔縮合，重新調整地圖大小
            google.maps.event.trigger(geeMap, 'resize');
        });
        $(this).hide(); $('.l-hide').show();

        // 右邊測攔縮合，重新調整地圖大小
        google.maps.event.trigger(geeMap, 'resize');
    });
});

//圖層選單的tree view
$(document).ready(function () {
    $('.p-tree li').each(function () {
        if ($(this).children('ul').length > 0) {
            $(this).addClass('parent');
            $(this).prepend("<i></i>");
        }
    });

    $('.p-tree li.parent i ').click(function () {
        $(this).parent().toggleClass('active');
        $(this).parent().children('ul').slideToggle('fast');
    });
});

//InfoWindow
$(document).ready(function () {
    $('.InfoWindow .close')
        .click(function () {
            $(this).parent().fadeOut();
        });
});

//圖台工具的按鈕變化
//$('.btn-map-tools .btn').click(function(){
//	$('.btn-map-tools .btn').removeClass("active");
//	$(this).addClass("active");
//});

$(function () { $("[data-toggle='popover']").popover(); });

$('.btn-shapes .btn').click(function () {
    $('.btn-shapes .btn').removeClass("active");
    $(this).addClass("active");
});

$('.tb-Level-info .popover').show()

$('.btn-Circle').click(function () {
    $('.draw-info-Square,.draw-info-Polygon').hide();
    $('.draw-info-Circle').show()
});

$('.btn-Square').click(function () {
    $('.draw-info-Circle,.draw-info-Polygon').hide();
    $('.draw-info-Square').show()
});

$('.btn-Polygon').click(function () {
    $('.draw-info-Circle,.draw-info-Square').hide();
    $('.draw-info-Polygon').show()
});