import 'https://api.mapbox.com/mapbox-gl-js/v2.8.1/mapbox-gl.js';
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-geocoder/v5.0.0/mapbox-gl-geocoder.min.js';

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

mapboxgl.accessToken = "pk.eyJ1IjoiZGhhcmlvc3V0ZWpvIiwiYSI6ImNrenp0anQ3MzBkbmszZHFjMXVwMWZ5Z2wifQ.QS_6oRMj0Yriiy9oX5GYaQ";
export function loadMapToElement(element) {
    return new mapboxgl.Map({
        container: element,
        style: "mapbox://styles/dhariosutejo/cl4cfdjvt000115tadd9zco0m",
        minZoom: 16,
        zoom: 16,
        maxZoom: 20,
    });
}

export function setMapCenter(map, longitude, latitude) {
    const center = [longitude, latitude];
    map.setCenter([longitude, latitude]);
    new mapboxgl.Marker()
        .setLngLat(center)
        .addTo(map);
}

export function addMapToElement(element, longitude, latitude, dotNetObject) {
    const map = new mapboxgl.Map({
        container: element,
        style: "mapbox://styles/dhariosutejo/cl4cfdjvt000115tadd9zco0m",
        center: [longitude, latitude],
        minZoom: 10,
        zoom: 14,
        maxZoom: 16,
        attributionControl: false
    });

    const point = map.project([longitude, latitude]);
    map.getCanvas().style.cursor = "pointer";

    map.on('load', () => {
        map.addLayer({
            id: "kelurahan-highlighted",
            type: "fill",
            source: "composite",
            "source-layer": "idn_admbnda_adm4_ID3_bps_2020-dtakwq",
            paint: {
                "fill-outline-color": "#0DB201",
                "fill-color": "#0DB201",
                "fill-opacity": 0.15,
            },
            filter: ["in", "ADM4_EN", ""],
        });

        let features = map.queryRenderedFeatures(point, { point, layers: ['kelurahan-jawa-boundaries-area'] });
        console.log(features);
        let feature = features[0];
        console.log(feature.properties.ADM4_EN);
        map.setFilter('kelurahan-highlighted', ['in', "ADM4_EN", feature.properties.ADM4_EN]);
        dotNetObject.invokeMethodAsync("GetQueryRenderedFeatures", JSON.stringify(feature.properties));

        map.on("click", "kelurahan-jawa-boundaries-area", (e) => {
            const feature = e.features[0];
            const relatedKelurahan = map.querySourceFeatures("composite",
                {
                    sourceLayer: "idn_admbnda_adm4_ID3_bps_2020-dtakwq",
                    filter: ["in", "ADM4_EN", feature.properties.ADM4_EN],
                });
            //const uniqueKelurahan = getUniqueFeatures(relatedKelurahan, "Kelurahan");
            map.setFilter("kelurahan-highlighted", ["in", "ADM4_EN", feature.properties.ADM4_EN]);
            dotNetObject.invokeMethodAsync("GetQueryRenderedFeatures", JSON.stringify(feature.properties));
        });
    });
}

export function queryRenderedFeaturesPoint(map, longitude, latitude) {
    const features = map.queryRenderedFeatures(
        [longitude, latitude],
        {
            layers: ['idn_admbnda_adm4_ID3_bps_2020-dtakwq']
        });

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

    map.setFilter('kelurahan-highlighted', ['in', "ADM4_EN", features[0].properties.ADM4_EN]);
}