let mapControlName;

function createMarker(
    displayName,
    longitude,
    latitude,
    timeOffset,
    index,
    isActual,
    hasHistory,
    showRouteText,
    hideRouteText){
    let popupContent = "<p><b>" + displayName + "</b><br>UTC " + timeOffset;

    if (hasHistory) {
        popupContent += "<br><button type='button' onclick='locations.markerClickHandler("
            + index + ")' class='marker-btn'>" + (index === 0 ? hideRouteText : showRouteText)
            + "</button>";
    }

    popupContent += "</p>";

    return L.marker([latitude, longitude], {opacity: isActual ? 1 : 0.5}).bindPopup(popupContent);
}

function createLayers(data, layerTitle, showRouteText, hideRouteText){
    let layers = {};
    let markers = [];
    let index = 0;
    
    for (let [key, value] of Object.entries(data)) {
        index++;
        markers[markers.length] = createMarker(
            value[0].displayName,
            value[0].longitude,
            value[0].latitude,
            value[0].displayOffset,
            index,
            true,
            value.length > 1,
            showRouteText,
            hideRouteText);
    }
    layers[layerTitle] = L.layerGroup(markers);
    return layers;
}

function createRoutes(data, showRouteText, hideRouteText) {
    let routes = {};
    
    for (let [key, values] of Object.entries(data)) {
        let hasHistory = values.length > 1;
        let isActual = true;
        let markers = [];
        let points = [];

        values.forEach(v => {
            markers[markers.length] = createMarker(
                v.displayName,
                v.longitude,
                v.latitude,
                v.displayOffset,
                0,
                isActual,
                hasHistory,
                showRouteText,
                hideRouteText);
            points[points.length] = new L.LatLng(v.latitude, v.longitude);

            isActual = false;
        });

        let layerGroup = L.layerGroup(markers);
        let polyline = new L.Polyline(points, {
            color: 'blue',
            weight: 2,
            opacity: 0.5,
            smoothFactor: 1
        });
        routes[key] = layerGroup;
        polyline.addTo(layerGroup);
    }
    
    return routes;
}

export function initialize(hostElement, data, showRouteText, hideRouteText, layerTitle) {
    let osm = L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}.png', {
        minZoom: 2,
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    });
    let layers = createLayers(data, layerTitle, showRouteText, hideRouteText);
    let routes = createRoutes(data, showRouteText, hideRouteText);
    let defaultLayers = [osm];
    
    Object.keys(layers).forEach(function (key) {
        defaultLayers[defaultLayers.length] = layers[key];
    });
    let map = L.map(hostElement, {
        center: [48.073777, 67.402377],
        zoom: 3,
        layers: defaultLayers
    });

    let control = L.control.layers({...layers, ...routes}).addTo(map);
    mapControlName = 'leaflet-base-layers_' + control._leaflet_id;

    window.locations = {
        markerClickHandler: function markerClickHandler(index) {
            document.getElementsByName(mapControlName)[index].click();
        }
    }
}