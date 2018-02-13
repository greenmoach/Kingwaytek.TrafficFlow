

(function ($) {



    var geeMap;

    $(function() {

        // Define map.
        var mapOpts = {
            zoom: 10,
            center: new google.maps.LatLng(24.9927278, 121.3004241),
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

