

(function ($) {



    var geeMap;

    $(function() {

        // Define map.
        var mapOpts = {
            zoom: 3,
            center: new google.maps.LatLng(44, 0),
            mapTypeControl: false,
            panControl: false,
            zoomControl: false,
            scaleControl: true,
            streetViewControl: false
        };
        // Create map.
        geeMap = geeCreateFusionMap('map_canvas', geeServerDefs, mapOpts);

    });


})(jQuery);

