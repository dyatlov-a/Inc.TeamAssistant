let mapControlName = 'leaflet-base-layers_';

function createMarker(
    location,
    featureNamesLookup,
    index,
    isActual,
    hasHistory,
    showRouteText,
    hideRouteText){
    let popupContent = "<p><div class=\"map-popup\">";
    popupContent += "<img src=\"/photos/" + location.personId + "\" alt=\"" + location.personDisplayName + "\" class=\"map-popup__user-avatar\" />";
    popupContent += "<div class=\"map-popup__content\">";
    popupContent += "<b>" + location.personDisplayName + "</b><br>";
    popupContent += location.countryName + "<br>";
    popupContent += location.workSchedule + " " + location.displayTimeOffset + "<br><br>";
    location.stats.forEach(s => {
        popupContent += featureNamesLookup[s.featureName] + " ";
        for (let i = 0; i < s.starCount; i++){
            popupContent += "⭐";
        }
        popupContent += "<br>";
    });
    
    if (hasHistory) {
        popupContent += "<br><button type='button' onclick='locations.markerClickHandler("
            + index + ")' class='marker-btn'>" + (index === 0 ? hideRouteText : showRouteText)
            + "</button>";
    }

    popupContent += "</div></div></p>";

    return L.marker([location.latitude, location.longitude], {opacity: isActual ? 1 : 0.5}).bindPopup(popupContent);
}

function createLayers(data, featureNamesLookup, layerTitle, showRouteText, hideRouteText){
    let layers = {};
    let markers = [];
    let index = 0;
    
    for (let [key, value] of Object.entries(data)) {
        index++;
        markers[markers.length] = createMarker(
            value[0],
            featureNamesLookup,
            index,
            true,
            value.length > 1,
            showRouteText,
            hideRouteText);
    }
    layers[layerTitle] = L.layerGroup(markers);
    return layers;
}

function createRoutes(data, featureNamesLookup, showRouteText, hideRouteText) {
    let routes = {};
    
    for (let [key, values] of Object.entries(data)) {
        let hasHistory = values.length > 1;
        let isActual = true;
        let markers = [];
        let points = [];

        values.forEach(v => {
            markers[markers.length] = createMarker(
                v,
                featureNamesLookup,
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

export function initialize(hostElement, data, featureNamesLookup, showRouteText, hideRouteText, layerTitle) {
    let osm = L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}.png', {
        minZoom: 2,
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    });
    let layers = createLayers(data, featureNamesLookup, layerTitle, showRouteText, hideRouteText);
    let routes = createRoutes(data, featureNamesLookup, showRouteText, hideRouteText);
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
    mapControlName += control._leaflet_id;

    window.locations = {
        markerClickHandler: function markerClickHandler(index) {
            document.getElementsByName(mapControlName)[index].click();
        }
    }
}