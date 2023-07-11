export function init(apiKey, libraries, mapTypeId, elementId, eventDotNetRef) {
    if (!apiKey || !elementId || !eventDotNetRef) {
        return;
    }

    storeMapInfo(_maps, elementId, eventDotNetRef, libraries, mapTypeId);

    let googleMapApiUrl = "https://maps.googleapis.com/maps/api/js?key=";
    let scriptsIncluded = false;

    let scriptTags = document.querySelectorAll("head > script");
    scriptTags.forEach(scriptTag => {
        if (scriptTag) {
            let srcAttribute = scriptTag.getAttribute("src");

            if (srcAttribute && srcAttribute.startsWith(googleMapApiUrl)) {
                scriptsIncluded = true;
            }
        }
    });

    if (scriptsIncluded) {
        if (window.google) {
            window.initGoogleMaps();
        }
        return;
    }

    let polyScriptTag = document.createElement("script");
    polyScriptTag.src = "https://polyfill.io/v3/polyfill.min.js?features=default";
    document.head.appendChild(polyScriptTag);

    let mapScriptTag = document.createElement("script");
    mapScriptTag.src = googleMapApiUrl + apiKey + "&libraries=" + libraries + "&callback=initGoogleMaps&v=beta";
    mapScriptTag.defer = true;
    document.head.appendChild(mapScriptTag);
}

window.initGoogleMaps = () => {
    cleanMaps();
    for (let i = 0; i < _maps.length; i++) {
        let elementId = _maps[i].key;
        let mapInfo = _maps[i].value;

        if (mapInfo.map) { //Map already created
            continue;
        }

        let el = document.getElementById(elementId);
        if (!el) {
            console.warn("No such element [" + elementId + "]");
            continue;
        }

        let mapTypeId = _maps[i].value.mapTypeId;
        let map = new google.maps.Map(el, {
            mapId: elementId,
            mapTypeId: mapTypeId
        });
        map.elementId = elementId;

        let libraries = _maps[i].value.libraries;
        if (libraries.includes("drawing") && !mapInfo.drawingManager) {
            let drawingManager = new google.maps.drawing.DrawingManager({
                map: map,
            });

            google.maps.event.addListener(drawingManager, "polygoncomplete", (polygon) => {
                if (map && map.elementId) {
                    let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, map.elementId);
                    if (mapWithDotnetRef) {

                        let polygonBounds = polygon.getPath();
                        let bounds = [];
                        polygonBounds.forEach((bound) => {
                            bounds.push({
                                lat: bound.lat(),
                                lng: bound.lng(),
                            });
                        });

                        polygon.id = crypto.randomUUID();

                        polygon.getPaths().forEach(function (path, index) {
                            google.maps.event.addListener(path, 'insert_at', function () {
                                updatePolygonBounds(map.elementId, polygon.id);
                            });

                            google.maps.event.addListener(path, 'remove_at', function () {
                                updatePolygonBounds(map.elementId, polygon.id);
                            });

                            google.maps.event.addListener(path, 'set_at', function () {
                                updatePolygonBounds(map.elementId, polygon.id);
                            });
                        });

                        mapWithDotnetRef.polygons.push(polygon);
                        mapWithDotnetRef.ref.invokeMethodAsync("DrawingPolygonCompleted", polygon.id, bounds);
                    }
                }
            });

            mapInfo.drawingManager = drawingManager;
        }

        map.addListener("resize", () => {
            if (map && map.elementId) {
                let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, map.elementId);
                if (mapWithDotnetRef) {
                    let arg = {
                        Width: map.getDiv().offsetWidth,
                        Height: map.getDiv().offsetHeight
                    };
                    mapWithDotnetRef.ref.invokeMethodAsync("MapResized", arg);
                }
            }
        });
        map.addListener("zoom_changed", () => {
            if (map && map.elementId) {
                let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, map.elementId);
                if (mapWithDotnetRef) {
                    mapWithDotnetRef.ref.invokeMethodAsync("MapZoomChanged", map.getZoom());
                }
            }
        });

        _maps[i].value.map = map;

        _maps[i].value.ref.invokeMethodAsync("MapInitialized", elementId);
    }
}

let _maps = [];

function storeMapInfo(dict, elementId, eventDotNetRef, libraries, mapTypeId) {
    let elementFound = false;
    for (let i = 0; i < dict.length; i++) {
        if (dict[i].key === elementId) {
            return;
        }
    }

    if (!elementFound) {
        dict.push({
            key: elementId,
            value: {
                ref: eventDotNetRef,
                map: null,
                libraries: libraries,
                mapTypeId: mapTypeId,
                drawingManager: null,
                polygons: [],
                circles: [],
                advancedMarkers: [],
                mask: null,
                overlay: null,
            },
        });
    }
}
function removeMapInfo(dict, elementId) {
    for (let i = 0; i < dict.length; i++) {
        if (dict[i].key === elementId) {
            dict.splice(i, 1);
            break;
        }
    }
}
function getElementIdWithDotnetRef(dict, elementId) {
    for (let i = 0; i < dict.length; i++) {
        if (dict[i].key === elementId) {
            return dict[i].value;
        }
    }
}
function cleanMaps() {
    if (!_maps || _maps.length == 0) {
        return;
    } 

    let purgeList = [];
    for (let i = 0; i < _maps.length; i++) {
        let el = document.getElementById(_maps[i].key);
        if (!el) {
            purgeList.push(_maps[i].key);
        }
    }

    if (purgeList.length > 0) {
        for (let j = 0; j < purgeList.length; j++) {
            dispose(purgeList[j]);
        }
    }
}

export function setCenterCoords(elementId, latitude, longitude) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            mapWithDotnetRef.map.setCenter({ lat: latitude, lng: longitude });
        }
    }
}
export function panToCoords(elementId, latitude, longitude) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            mapWithDotnetRef.map.panTo({ lat: latitude, lng: longitude });
        }
    }
}
export function setCenterAddress(elementId, address) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            geocodeAddress(address, function (results) {
                if (results) {
                    mapWithDotnetRef.map.setCenter(results[0].geometry.location);
                }
            });
        }
    }
}
export function panToAddress(elementId, address) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            geocodeAddress(address, function (results) {
                if (results) {
                    mapWithDotnetRef.map.panTo(results[0].geometry.location);
                }
            });
        }
    }
}
export function resizeMap(elementId) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            google.maps.event.trigger(mapWithDotnetRef.map, "resize");
        }
    }
}
export function setZoom(elementId, zoom) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            mapWithDotnetRef.map.setZoom(zoom);
        }
    }
}
export function setOptions(elementId, options) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            mapWithDotnetRef.map.setOptions(options);
        }
    }
}
export function fitBounds(elementId, eastLongitude, northLatitude, southLatitude, westLongitude) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            const bounds = {
                north: northLatitude,
                south: southLatitude,
                east: eastLongitude,
                west: westLongitude,
            };
            mapWithDotnetRef.map.fitBounds(bounds);
        }
    }
}
export function maskMap(elementId, shape, polygonOptions) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            const worldPoly = getWorldPoly(polygonOptions, shape);

            if (mapWithDotnetRef.mask) {
                mapWithDotnetRef.mask.setMap(null);
                mapWithDotnetRef.mask = null;
            }

            worldPoly.setMap(mapWithDotnetRef.map);
            mapWithDotnetRef.mask = worldPoly;
        }
    }
}
export function setCustomOverlay(elementId, imageSrc, southWestLatitude, southWestLongitude, northEastLatitude, northEastLongitude) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            const bounds = new google.maps.LatLngBounds(
                new google.maps.LatLng(southWestLatitude, southWestLongitude),
                new google.maps.LatLng(northEastLatitude, northEastLongitude)
            );

            class CustomOverlay extends google.maps.OverlayView {
                bounds;
                imageSrc;
                div;

                constructor(bounds, imageSrc) {
                    super();
                    this.bounds = bounds;
                    this.imageSrc = imageSrc;
                }

                /**
                 * We must implement three methods: onAdd(), draw(), and onRemove().
                 * https://developers.google.com/maps/documentation/javascript/reference/overlay-view#OverlayView
                 */
                onAdd() {
                    this.div = document.createElement("div");
                    this.div.style.borderStyle = "none";
                    this.div.style.borderWidth = "0";
                    this.div.style.position = "absolute";

                    const img = document.createElement("img");
                    img.src = this.imageSrc;
                    img.style.width = "100%";
                    img.style.height = "100%";
                    img.style.position = "absolute";
                    this.div.appendChild(img);

                    const panes = this.getPanes();
                    panes.overlayLayer.appendChild(this.div);
                }

                draw() {
                    const overlayProjection = this.getProjection();
                    const sw = overlayProjection.fromLatLngToDivPixel(this.bounds.getSouthWest());
                    const ne = overlayProjection.fromLatLngToDivPixel(this.bounds.getNorthEast());
                    if (this.div) {
                        this.div.style.left = sw.x + "px";
                        this.div.style.top = ne.y + "px";
                        this.div.style.width = ne.x - sw.x + "px";
                        this.div.style.height = sw.y - ne.y + "px";
                    }
                }

                onRemove() {
                    if (this.div) {
                        this.div.parentNode.removeChild(this.div);
                        delete this.div;
                    }
                }

                /**
                 * Custom methods
                 */
                removeFromDOM() {
                    if (this.getMap()) {
                        this.setMap(null);
                    } else {
                        this.setMap(map);
                    }
                }
            }

            const overlay = new CustomOverlay(bounds, imageSrc);
            overlay.setMap(mapWithDotnetRef.map);
            mapWithDotnetRef.overlay = overlay;
        }
    }
}
export function clearCustomOverlay(elementId) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map && mapWithDotnetRef.overlay) {
            overlay.removeFromDOM();
        }
    }
}
function getWorldPoly(polygonOptions, shape) {
    let shapeCoords = [];
    if (shape) {
        shape.bounds.forEach(x => shapeCoords.push({
            lat: x.lat,
            lng: x.lng,
        }));
    }

    // Find the min/max latitude
    const maxLat = Math.atan(Math.sinh(Math.PI)) * 180 / Math.PI;

    const worldCoords = [
        new google.maps.LatLng(-maxLat, -180),
        new google.maps.LatLng(maxLat, -180),
        new google.maps.LatLng(maxLat, 180),
        new google.maps.LatLng(-maxLat, 180),
        new google.maps.LatLng(-maxLat, 0)];

    if (shape) {
        return new google.maps.Polygon({
            paths: [worldCoords, shapeCoords],
            strokeColor: polygonOptions.strokeColor,
            strokeOpacity: polygonOptions.strokeOpacity,
            strokeWeight: polygonOptions.strokeWeight,
            fillColor: polygonOptions.fillColor,
            fillOpacity: polygonOptions.fillOpacity,
        });
    }
    else {
        return new google.maps.Polygon({
            path: worldCoords,
            strokeColor: polygonOptions.strokeColor,
            strokeOpacity: polygonOptions.strokeOpacity,
            strokeWeight: polygonOptions.strokeWeight,
            fillColor: polygonOptions.fillColor,
            fillOpacity: polygonOptions.fillOpacity,
        });
    }
}

//Google Map Drawing
export function setDrawingOptions(elementId, options) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map && mapWithDotnetRef.drawingManager) {
            mapWithDotnetRef.drawingManager.setOptions(options);
        }
    }
}
export function setDrawingMode(elementId, drawingMode) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map && mapWithDotnetRef.drawingManager) {
            mapWithDotnetRef.drawingManager.setDrawingMode(drawingMode);
        }
    }
}
export function clearAllPolygons(elementId) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            mapWithDotnetRef.polygons.forEach(polygon => polygon.setMap(null));
            mapWithDotnetRef.polygons = [];
        }
    }
}
export function drawPolygon(elementId, shape, polygonOptions, editable) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            let paths = [];
            shape.bounds.forEach(x => paths.push({
                lat: x.lat,
                lng: x.lng,
            }));

            if (!polygonOptions) {
                polygonOptions = {};
            }

            polygonOptions.paths = paths;
            polygonOptions.editable = editable;
            const polygon = new google.maps.Polygon(polygonOptions)

            polygon.id = shape.id;
            polygon.setMap(mapWithDotnetRef.map);

            polygon.getPaths().forEach(function (path, index) {
                google.maps.event.addListener(path, 'insert_at', function () {
                    updatePolygonBounds(elementId, polygon.id);
                });

                google.maps.event.addListener(path, 'remove_at', function () {
                    updatePolygonBounds(elementId, polygon.id);
                });

                google.maps.event.addListener(path, 'set_at', function () {
                    console.log('set_at!');
                    updatePolygonBounds(elementId, polygon.id);
                });
            });

            mapWithDotnetRef.polygons.push(polygon);
        }
    }
}
export function computeArea(elementId, shapeId) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map && mapWithDotnetRef.libraries.includes("geometry")) {
            let shape = mapWithDotnetRef.polygons.find(x => x.id == shapeId);
            if (shape) {
                let area = google.maps.geometry.spherical.computeArea(shape.getPath());
                return (area / 10000).toFixed(0);
            }
        }
    }
}
export function drawCircle(elementId, shape, circleOptions) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            if (!circleOptions) {
                circleOptions = {};
            }

            const circle = new google.maps.Circle(circleOptions)

            circle.id = shape.id;
            circle.setMap(mapWithDotnetRef.map);

            mapWithDotnetRef.circles.push(circle);
        }
    }
}
export function clearAllCircles(elementId) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            mapWithDotnetRef.circles.forEach(circle => circle.setMap(null));
            mapWithDotnetRef.circles = [];
        }
    }
}
export function setCircleCenter(elementId, circleId, center) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            let circle = mapWithDotnetRef.circles.find(x => x.id == circleId);
            if (circle) {
                circle.setCenter(center);
            }
        }
    }
}

//Google Marker
export function drawAdvancedMarker(elementId, markerId, position, content) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            const contentTag = document.createElement("div");
            contentTag.className = "marker";
            contentTag.innerHTML = content;

            const marker = new google.maps.marker.AdvancedMarkerElement({
                map: mapWithDotnetRef.map,
                position: position,
                content: contentTag,
            });

            if (markerId) {
                marker.id = markerId;
            }

            marker.addEventListener("gmp-click", () => {
                toggleClass(marker, "highlight");
            });

            mapWithDotnetRef.advancedMarkers.push(marker);
        }
    }
}

export function setAdvancedMarkerPosition(elementId, markerId, position) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            let marker = mapWithDotnetRef.advancedMarkers.find(x => x.id == markerId);

            if (marker) {
                marker.position = position;
            }
        }
    }
}

export function setAdvancedMarkerContent(elementId, markerId, content) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            let marker = mapWithDotnetRef.advancedMarkers.find(x => x.id == markerId);

            if (marker) {
                marker.content.innerHTML = content;
            }
        }
    }
}

function toggleClass(marker, className) {
    if (marker.content.classList.contains(className)) {
        marker.content.classList.remove(className);
        marker.zIndex = null;
    } else {
        marker.content.classList.add(className);
        marker.zIndex = 1;
    }
}


function updatePolygonBounds(elementId, polygonId) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);
        if (mapWithDotnetRef && mapWithDotnetRef.map) {
            let polygon = mapWithDotnetRef.polygons.find(x => x.id == polygonId);
            if (polygon) {
                let polygonBounds = polygon.getPath();
                let bounds = [];
                polygonBounds.forEach((bound) => {
                    bounds.push({
                        lat: bound.lat(),
                        lng: bound.lng(),
                    });
                });

                mapWithDotnetRef.ref.invokeMethodAsync("PolygonUpdated", polygon.id, bounds);
            }
        }
    }
}

//Google GeoCoder
function geocodeAddress(address, successCallback) {
    let geocoder = new google.maps.Geocoder();
    geocoder.geocode({
        'address': address
    }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            successCallback(results);
        }
    });
}

export function dispose(elementId) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_maps, elementId);

        if (mapWithDotnetRef.map) {
            mapWithDotnetRef.map = null;
        }

        if (mapWithDotnetRef.ref) {
            mapWithDotnetRef.ref = null;
        }

        if (mapWithDotnetRef.libraries) {
            mapWithDotnetRef.libraries = null;
        }

        if (mapWithDotnetRef.drawingManager) {
            mapWithDotnetRef.drawingManager = null;
        }

        if (mapWithDotnetRef.polygons) {
            mapWithDotnetRef.polygons = null;
        }

        if (mapWithDotnetRef.mask) {
            mapWithDotnetRef.mask = null;
        }

        removeMapInfo(_maps, elementId);
    }
}
