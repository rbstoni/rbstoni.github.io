import 'https://api.mapbox.com/mapbox-gl-js/v2.8.1/mapbox-gl.js';
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-geocoder/v5.0.0/mapbox-gl-geocoder.min.js';

mapboxgl.accessToken = "pk.eyJ1IjoiZGhhcmlvc3V0ZWpvIiwiYSI6ImNrenp0anQ3MzBkbmszZHFjMXVwMWZ5Z2wifQ.QS_6oRMj0Yriiy9oX5GYaQ";

export function addMapToElement(element) {
    return new mapboxgl.Map({
        container: element,
        style: "mapbox://styles/dhariosutejo/cl4cfdjvt000115tadd9zco0m", // style URL
        center: [106.826, -6.175], // starting position [lng, lat]
        zoom: 14, // starting zoom
    });
}

export function addGeocoderToElement(element) {
    return new MapboxGeocoder({
        accessToken: mapboxgl.accessToken,
        placeholder: 'Kode Pos/Kelurahan/Desa',
        mapboxgl: mapboxgl,
        proximity: 'ip',
        types: 'place,postcode,locality,neighborhood'
    }).addTo(element);
}

export function setMapCenter(map, latitude, longitude) {
    map.setCenter([longitude, latitude]);
}

export function addMapControl(map) {
    map.addControl(
        new MapboxGeocoder({
            mapboxgl: mapboxgl
        })
    );
}