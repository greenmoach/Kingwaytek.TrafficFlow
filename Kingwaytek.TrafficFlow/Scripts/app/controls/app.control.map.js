var geeMap;

(function ($) {
    if ($('#map_canvas').length === 0) {
        return;
    }

    $(function () {
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

var kingwaytek =
    {
        SymbolPath: {
            Intersection: 'M25.006,6.339 21.66,2 18.314,6.339 19.728,6.339 19.728,32.219 23.593,32.219 23.593,6.339 z' +
            'M6.433,13.631 6.433,12.201 2.094,15.548 6.433,18.893 6.433,17.463 13.069,17.463 13.069,32.219 16.901,32.219 16.901,13.631 z' +
            'M41.32,15.548 36.981,12.201 36.981,13.631 26.499,13.631 26.499,32.219 30.331,32.219 30.331,17.463 36.981,17.463 36.981,18.893 z'
        }
    };