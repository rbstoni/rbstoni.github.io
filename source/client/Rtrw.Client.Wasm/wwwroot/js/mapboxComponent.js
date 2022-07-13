import 'https://api.mapbox.com/mapbox-gl-js/v2.8.1/mapbox-gl.js';
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-geocoder/v5.0.0/mapbox-gl-geocoder.min.js';

mapboxgl.accessToken = "pk.eyJ1IjoiZGhhcmlvc3V0ZWpvIiwiYSI6ImNrenp0anQ3MzBkbmszZHFjMXVwMWZ5Z2wifQ.QS_6oRMj0Yriiy9oX5GYaQ";

export function addMapToElement(element, longitude, latitude) {
    return new mapboxgl.Map({
        container: element,
        style: "mapbox://styles/dhariosutejo/cl4cfdjvt000115tadd9zco0m",
        center: [longitude, latitude],
        minZoom: 12,
        zoom: 14,
        maxZoom: 16,
        attributionControl: false
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
    map.flyTo({ center: [longitude, latitude], essential: true });

    map.on("moveend", () => {
        console.log(longitude, latitude);
    });
}

//const popup = new mapboxgl.Popup({
//    closeButton: false,
//});

//const marker = new mapboxgl.Marker();

export function getMapFeatures(map, dotNetObject) {
    map.on("load", () => {
        map.addLayer({
            id: "kelurahan-highlighted",
            type: "fill",
            source: "composite", "source-layer": "idn_admbnda_adm4_ID3_bps_2020-dtakwq",
            paint: {
                "fill-outline-color": "#0DB201",
                "fill-color": "#0DB201",
                "fill-opacity": 0.15,
            },
            filter: ["in", "ADM4_EN", ""],
        });

        map.on("click", "kelurahan-jawa-boundaries-area", (e) => {
            map.getCanvas().style.cursor = "pointer";
            const feature = e.features[0];
            //map.flyTo(e.lngLat);
            //marker.setLngLat(e.lngLat).addTo(map);
            const relatedKelurahan = map.querySourceFeatures("composite", {
                sourceLayer: "idn_admbnda_adm4_ID3_bps_2020-dtakwq",
                filter: ["in", "ADM4_EN", feature.properties.ADM4_EN],
            });
            const uniqueKelurahan = getUniqueFeatures(relatedKelurahan, "Kelurahan");
            map.setFilter("kelurahan-highlighted", ["in", "ADM4_EN", feature.properties.ADM4_EN]);
            var properties = uniqueKelurahan[0].properties;
            dotNetObject.invokeMethodAsync("GetFeatureProperties", JSON.stringify(properties));
            //popup.setLngLat(e.lngLat).setText(feature.properties.ADM4_EN).addTo(map);
            ;
        });
    });
}

export function addMapControl(map) {
    //map.addControl(
    //    new MapboxGeocoder({
    //        accessToken: mapboxgl.accessToken,
    //        mapboxgl: mapboxgl
    //    }));
    map.addControl(new mapboxgl.AttributionControl(), 'top-left');
}

function getUniqueFeatures(features, comparatorProperty) {
    const uniqueIds = new Set();
    const uniqueFeatures = [];
    for (const feature of features) {
        const id = feature.properties[comparatorProperty];
        if (!uniqueIds.has(id)) {
            uniqueIds.add(id);
            uniqueFeatures.push(feature);
        }
    }
    return uniqueFeatures;
}

