let markers = [], points = [], routes = {}, layers = {}, mapControlName;

export function addMarker(
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

export function addLayer(title) {
    layers[title] = L.layerGroup(markers);
    markers = [];
    points = [];
}

export function addRoute(title) {
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

export function build(hostElement) {
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

