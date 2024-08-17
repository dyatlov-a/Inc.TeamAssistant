let markers = [], points = [], routes = {}, layers = {}, mapControlName;

function addMarker(
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

    markers[markers.length] = L
        .marker([latitude, longitude], {opacity: isActual ? 1 : 0.5})
        .bindPopup(popupContent)
    points[points.length] = new L.LatLng(latitude, longitude);
}

function addLayer(title) {
    layers[title] = L.layerGroup(markers);
    markers = [];
    points = [];
}

function addRoute(title) {
    let layerGroup = L.layerGroup(markers);
    let polyline = new L.Polyline(points, {
        color: 'blue',
        weight: 2,
        opacity: 0.5,
        smoothFactor: 1
    });
    routes[title] = layerGroup;
    polyline.addTo(layerGroup);
    markers = [];
    points = [];
}

function build(hostElement) {
    let osm = L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}.png', {
        minZoom: 2,
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    });

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

export function initialize(hostElement, data, showRouteText, hideRouteText, defaultLayerTitle) {
    console.log(data);
    let index = 0;
    for (let [key, value] of Object.entries(data)) {
        index++;
        addMarker(
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
    
    addLayer(defaultLayerTitle);

    for (let [key, values] of Object.entries(data)) {
        let hasHistory = values.length > 1;
        let first = true;
        
        values.forEach(v => {
            addMarker(
                v.displayName,
                v.longitude,
                v.latitude,
                v.displayOffset,
                0,
                first,
                hasHistory,
                showRouteText,
                hideRouteText);

            first = false;
        });
        
        addRoute(key);
    }
    
    build(hostElement);
}