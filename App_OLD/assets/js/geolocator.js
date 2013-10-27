var currentMapPoint;

function setHTML5Location() {
    if (navigator.geolocation) {

        var html5TimeStamp = null;
        var html5Accuracy = null;

        navigator.geolocation.getCurrentPosition(function (position, html5Error) {
            processGeolocationResult(position);
        });

        navigator.geolocation.watchPosition(function (position, html5Error) {
            processGeolocationResult(position);
        }); ;
    }
    else {
        alert("Desculpe mas seu nagegador nao suporta geolocalização");
    }
}

function html5Error(error) {
    alert('Houve um problema para recuperar sua geolocalização: ' + error);
}

function processGeolocationResult(position) {
    html5Lat = position.coords.latitude;
    html5Lon = position.coords.longitude;
    html5TimeStamp = position.timestamp;
    html5Accuracy = position.coords.accuracy;
    
    if (html5Lat != null && html5Lon != null) {
        zoomToLocation(html5Lat, html5Lon);
    }
}

function zoomToLocation(myLat, myLong) {
    var wgsPt = new esri.geometry.Point(myLong, myLat, new esri.SpatialReference({ wkid: 4326 }))
    var webMercatorMapPoint = esri.geometry.geographicToWebMercator(wgsPt);
    mapa.centerAndZoom(webMercatorMapPoint, 18);
    showLocation(myLat,myLong,webMercatorMapPoint);
}

function showLocation(myLat,myLong,mapPoint) {
    currentMapPoint = mapPoint;
    var pinName = avaliacao == POSITIVO ? "PIN-LIKE" : "PIN-DISLIKE";
    var HomeSymbol = new esri.symbol.PictureMarkerSymbol("images/"+pinName+".png", 40, 56).setColor(new dojo.Color([0, 0, 255]));
    var pictureGraphic = new esri.Graphic(mapPoint, HomeSymbol)
    /*
    mapa.infoWindow.setTitle("Vocë esta aqui");
    mapa.infoWindow.setContent('Lat : ' + myLat.toFixed(4) + ", " + ' Long: ' + myLong.toFixed(4));
    mapa.infoWindow.resize(200,45);
    mapa.infoWindow.show(mapPoint, esri.dijit.InfoWindowLite.ANCHOR_LOWERLEFT);*/

    mapa.graphics.clear();
    mapa.graphics.add(pictureGraphic);
    hideOverlay();
}