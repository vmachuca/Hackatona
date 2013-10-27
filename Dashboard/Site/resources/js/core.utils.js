function Utils() {
    this.Window = {

        HeatMap: {

            Variables: {
                Layers: [],
                HeatLayers: []
            },
            Controls: {
                Container: 'containerHeatMap',
                HeatLayer: 'heatLayer',
                CheckButton: 'checkButton'
            },
            Show: function () {


                var heatButtons = '';

                // get the features within the current extent from the feature layer
                $(utils.Window.HeatMap.Variables.Layers).each(function (layerIndex, layer) {
                    heatButtons += '<input style="width:100%" checked="checked" type="checkbox" id="' + layer.id + utils.Window.HeatMap.Controls.CheckButton + '" /><label for="' + layer.id + utils.Window.HeatMap.Controls.CheckButton + '">' + layer._params.label + '</label><br/>';
                });

                ///The content
                var content = heatButtons;


                ///The post function
                var postFunction = function () {

                    var heatDivs = '';

                    // get the features within the current extent from the feature layer
                    $(utils.Window.HeatMap.Variables.Layers).each(function (layerIndex, layer) {
                        heatDivs += '<div id="' + layer.id + utils.Window.HeatMap.Controls.HeatLayer + '" ></div>';

                        $('#' +  layer.id + utils.Window.HeatMap.Controls.CheckButton)
                        .button()
                        .click(function( event ) {

                            if (layer['heatMap'])
                            {
                                if (layer['heatMap'].visible) 
                                    layer['heatMap'].hide() ;
                                else  
                                    layer['heatMap'].show();
                            }
                        });;
                    });

                    $('#' + utils.Window.HeatMap.Controls.Container).html(heatDivs);

                    // get the features within the current extent from the feature layer
                    $(utils.Window.HeatMap.Variables.Layers).each(function (layerIndex, layer) {


                        if (layer['heatMap'] == undefined)
                        {
                            // Variables
                            var heatLayer = new HeatmapLayer({
                                "map": map,
                                "domNodeId": layer.id + utils.Window.HeatMap.Controls.HeatLayer,
                                "opacity": 0.85
                            });
                       
                            layer['heatMap'] = heatLayer;

                            map.addLayer(layer['heatMap']);
                        }

                        // set up query
                        var query = new esri.tasks.Query();
                        // only within extent
                        query.geometry = map.extent;
                        // give me all of them!
                        query.where = "1=1";
                        // make sure I get them back in my spatial reference
                        query.outSpatialReference = map.spatialReference;
                        // get em!
                        layer.queryFeatures(query, function (featureSet) {

                            var data = [];
                            // if we get results back
                            if (featureSet && featureSet.features && featureSet.features.length > 0) {
                                // set data to features
                                data = featureSet.features;
                            }

                            // set heatmap data
                            layer['heatMap'].setData(data);
                        });
                    });

                };

                ///window options
                var options = {
                    height: 270,
                    width: 200,
                    titleIcon: 'resources/images/header/buttons/fire_24x24.png',
                    float: 'left',
                    x: 60,
                    y: 57
                };

                ///Show the window
                layoutUtils.make('heatMapWindow', 'Mapa de Calor', content, options, postFunction);

            }
        },



        About: {
            Controls: {
                MailToButton: 'mailToButton'
            },
            Show: function () {

                ///The content
                var content =
                            '<div style="width:100%">' +
                                '<b>Equipe DLJ2V</b><br /><br />' +
                                'Lucas Soares<br/>' +
                                'Djonatas Tenfen<br/>' +
                                'Jaime Flores<br/>' +
                                'José Luiz Losnak<br/>' +
                                'Vinicius Machuca<br/>' +
                                '<br />' +
                                '<img src="resources/images/img/logo.jpg" />' +
                            '</div><br />' +
                            '<div style="width:100%; text-align:right">' +
                                '<button id="' + utils.Window.About.Controls.MailToButton + '">Contato</button>' +
                            '</div>';


                ///The post function
                var postFunction = function () {
                    $('#' + utils.Window.About.Controls.MailToButton).button(
                    {
                        icons: {
                            primary: "ui-icon-mail-closed"
                        }
                    })
                    .click(function () {
                        window.open('mailto:lucas.ribeiro@img.com.br;%20dtenfen@img.com.br;%20jlosnak@img.com.br;%20jlopes@img.com.br;%20vmachuca@img.com.br?Subject=Eu%20Ando%20De%20Onibus', '_blank')
                    });
                }

                ///window options
                var options = {
                    height: 290,
                    width: 300,
                    noInitPosition: true,
                    titleIcon: 'resources/images/header/buttons/about_24x24.png'
                };

                ///Show the window
                layoutUtils.make('aboutWindow', 'Sobre', content, options, postFunction);
            }
        },

        ToC: {
            Controls: {
                MainDiv: 'tocDiv',
                CountLabel: 'timerCountLabel',
                LoadingImage: 'timerLoadingImage'
            },
            OperationalLayers: [],
            Refresh: function () {

                ///For each map dynamic or tiled layer
                $(map.graphicsLayerIds).each(function (layerIdIndex, layerId) {

                    ///Getting the layer
                    var layer = map.getLayer(layerId);

                    ///Refreshing
                    $('#' + utils.Window.ToC.Controls.CountLabel + layerId).html(layer.graphics.length);

                });
            },
            ExtentChange: function (extent, a) {

                utils.Window.ToC.Refresh();

                ///For each map dynamic or tiled layer
                $.each(map.graphicsLayerIds, function (layerIdIndex, layerId) {

                    var layer = map.getLayer(layerId);

                    if (layer._params.showLevel == undefined) return;

                    ///Verify the leve
                    if (map.getLevel() >= layer._params.showLevel)
                        $('#' + layer.id).attr('disabled', false);
                    else
                        $('#' + layer.id).attr('disabled', 'disabled');

                    utils.Window.ToC.Visible(layer.id);
                });

                ///For each map dynamic or tiled layer
                $(utils.Window.ToC.OperationalLayers).each(function (olIndex, ol) {

                    var layer = map.getLayer(ol.id);

                    if (ol.showLevel == undefined) return;

                    ///Verify the leve
                    if (map.getLevel() >= ol.showLevel)
                        $('#' + layer.id).attr('disabled', false);
                    else
                        $('#' + layer.id).attr('disabled', 'disabled');

                    utils.Window.ToC.Visible(layer.id);
                });

            },
            Visible: function (layerId) {

                ///Gets the map layer
                var layer = map.getLayer(layerId);

                if (layer != null) {

                    if ($('#' + layerId).length == 0) return;

                    if ($('#' + layerId).attr('disabled') == undefined && $("#" + layerId).attr('checked') == 'checked') {
                        if (layer.visible != true) {

                            layer.show();
                            layer.refresh();
                        }
                    }
                    else {
                        if (layer.visible != false)
                            layer.hide();
                    }
                }
            },
            Show: function () {

                var checkboxTree = '<br/>';
                var checkboxBingTree = '';
                var checkboxMain = '';

                ///For each map dynamic or tiled layer

                for (var layerIdIndex = map.layerIds.length - 1; layerIdIndex >= 0; layerIdIndex--) {

                    var layerId = map.layerIds[layerIdIndex];

                    //Getting the layer
                    var layer = map.getLayer(layerId);

                    ///Verify if exists a bing maps key
                    if (layer['bingMapsKey'])

                        checkboxBingTree += String.format(
                                '<div></div>' +
                                '<div id="bing{0}" style="width: 90%;">' +
                                    '<input style="width: 33%" type="radio" mapType="{3}" layerName="{0}" id="bingRoads{0}"       name="radio" {6}/><label for="bingRoads{0}"      >Mapa</label>' +
                                    '<input style="width: 33%" type="radio" mapType="{1}" layerName="{0}" id="bingAerial{0}"      name="radio" {4}/><label for="bingAerial{0}"     >Satélite</label>' +
                                    '<input style="width: 33%" type="radio" mapType="{2}" layerName="{0}" id="bingAerialRoads{0}" name="radio" {5}/><label for="bingAerialRoads{0}">Híbrido</label>' +
                                '</div><br/><br/>',
                                layer.id,
                                esri.virtualearth.VETiledLayer.MAP_STYLE_AERIAL,
                                esri.virtualearth.VETiledLayer.MAP_STYLE_AERIAL_WITH_LABELS,
                                esri.virtualearth.VETiledLayer.MAP_STYLE_ROAD,
                                layer.mapStyle == esri.virtualearth.VETiledLayer.MAP_STYLE_AERIAL ? 'checked = "checked"' : '',
                                layer.mapStyle == esri.virtualearth.VETiledLayer.MAP_STYLE_AERIAL_WITH_LABELS ? 'checked = "checked"' : '',
                                layer.mapStyle == esri.virtualearth.VETiledLayer.MAP_STYLE_ROAD ? 'checked = "checked"' : '')
                }

                for (var layerIdIndex = map.graphicsLayerIds.length - 1; layerIdIndex >= 0; layerIdIndex--) {

                    var layerId = map.graphicsLayerIds[layerIdIndex];

                    //Getting the layer
                    var layer = map.getLayer(layerId);

                    checkboxTree += String.format(
                                '<input type="checkbox" id="{0}" onclick="utils.Window.ToC.Visible(\'{0}\');" {2}> {3}   ' +
                                '<label for="{0}">{1} </label>' +
                                '<label id="' + utils.Window.ToC.Controls.CountLabel + '{0}" style="font-weight: bold"></label>' +
                                '<img style="display:none" id="' + utils.Window.ToC.Controls.LoadingImage + '{0}" src="resources/images/ui/loading_12x12.gif" /><br/><br/>',
                                layer.id,
                                layer._params.label,
                                layer.visible ? 'checked="checked"' : '',
                                layer._params.image != undefined ? '<img src="' + layer._params.image + '"/>' : '');
                }

                ///For each operational layer
                for (var olIndex = utils.Window.ToC.OperationalLayers.length - 1; olIndex >= 0; olIndex--) {

                    var ol = utils.Window.ToC.OperationalLayers[olIndex];

                    checkboxTree += String.format(
                            '<input type="checkbox" id="{0}" onclick="utils.Window.ToC.Visible(\'{0}\');" {2}> {3}   ' +
                            '<label for="{0}">{1} </label>' +
                            '<label id="' + utils.Window.ToC.Controls.CountLabel + '{0}" style="font-weight: bold"></label>' +
                            '<img style="display:none" id="' + utils.Window.ToC.Controls.LoadingImage + '{0}" src="resources/images/ui/loading_12x12.gif" /><br/><br/>',
                            ol.id,
                            ol.label,
                            ol.visible ? 'checked="checked"' : '',
                            ol.image != undefined ? '<img height="14px" width="14px" src="' + ol.image + '"/>' : '');

                }

                checkboxMain += String.format('<div style="width:100%; margin-left:10px;">{0}{1}</div>', checkboxTree, checkboxBingTree);

                ///The content
                var content = '<div id="' + utils.Window.ToC.Controls.MainDiv + '" style="width: 100%; ">' + checkboxMain + '</div>';

                ///The post function
                var postFunction = function () {

                    utils.Window.ToC.ExtentChange();

                    ///For each map dynamic or tiled layer
                    $.each(map.layerIds, function (layerIdIndex, layerId) {

                        //Getting the layer
                        var layer = map.getLayer(layerId);

                        ///Verify if exists a bing maps key
                        if (layer['bingMapsKey']) {
                            $('#bing' + layer.id).buttonset().find('label').css('width', '33%');
                            $('#bing' + layer.id + ' input').click(function () {

                                ///Setting the map style
                                map.getLayer($(this).attr('layerName')).setMapStyle($(this).attr('mapType'));

                            });
                        }

                    });
                }

                ///window options
                var options = {
                    height: 240,
                    width: 290,
                    float: 'right',
                    titleIcon: 'resources/images/header/buttons/layers_white.png',
                    x: 2,
                    y: 39
                };

                ///Show the window
                layoutUtils.make('tableOfContentsWindows', 'Camadas', content, options, postFunction);
            }
        }
    }
    this.Ajax = {

        Get: function (options, successFunction, errorFunction) {

            if (options == undefined) options = {};
            if (options['type'] == undefined) options['type'] = 'GET';
            if (options['dataType'] == undefined) options['dataType'] = 'json';
            if (options['async'] == undefined) options['async'] = true;
            if (options['data'] == undefined) options['data'] = { rdm: Math.random() };
            if (options['data']['rdm'] == undefined) options['data']['rdm'] = Math.random();

            ///Requesting
            $.ajax(options).
              done(successFunction).
              fail(errorFunction);
        }
    }


    this.GIS = {

        Geometry: {
            Multipoint: function (points) {
                var multipoint = new esri.geometry.Multipoint(map.spatialReference);

                $(points).each(function (pointIndex, point) {
                    multipoint.addPoint(point);
                })

                return multipoint;
            },
            Zoom: function (geometry) {
                /// <reference path="../../jsonp.svc" />

                ///The extent variable
                var extent;

                //calculate map coords represented per pixel
                var pixelWidth = 19567.8792409999;

                //calculate map coords for tolerance in pixel
                var toleraceInMapCoords = 4 * pixelWidth;

                ///Verify if the geometry is a point
                if (geometry.type == 'point') {

                    //calculate & return computed extent
                    extent = new esri.geometry.Extent(
                                                      geometry.x - toleraceInMapCoords,
                                                      geometry.y - toleraceInMapCoords,
                                                      geometry.x + toleraceInMapCoords,
                                                      geometry.y + toleraceInMapCoords,
                                                      map.spatialReference);
                }
                else if (geometry.type == 'multipoint') {
                    if (geometry.points.length == 1) {
                        utils.GIS.Geometry.Zoom(new esri.geometry.Point(geometry.points[0][0], geometry.points[0][1]));
                        return;
                    }
                    else
                        extent = geometry.getExtent();
                }
                else
                ///If is the other feature types
                    extent = geometry.getExtent();


                //calculate & return computed extent
                var newExtent = new esri.geometry.Extent(
                extent.xmin - toleraceInMapCoords,
                extent.ymin - toleraceInMapCoords,
                extent.xmax + toleraceInMapCoords,
                extent.ymax + toleraceInMapCoords,
                map.spatialReference);

                ///Setting the extent
                map.setExtent(newExtent);
            }
        },
        MarkerGraphic: function (geometry, rgbColor, size, attributes) {

            ///Creating the graphic and the text symbols
            var symbol = new esri.symbol.SimpleMarkerSymbol();
            symbol.setColor(new dojo.Color(rgbColor));
            symbol.setSize(size);
            symbol.setStyle(esri.symbol.SimpleMarkerSymbol.STYLE_CIRCLE);
            symbol.setOutline(new esri.symbol.SimpleLineSymbol(esri.symbol.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([0, 0, 0]), 1));

            var graphic = new esri.Graphic();
            graphic.setGeometry(geometry);
            graphic.setSymbol(symbol);
            graphic.attributes = attributes;

            return graphic;
        },

        TextGraphic: function (geometry, text, offset) {

            ///Creating the graphic and the text symbols
            var symbolFont = new esri.symbol.Font("12px", esri.symbol.Font.STYLE_NORMAL, esri.symbol.Font.VARIANT_NORMAL, esri.symbol.Font.WEIGHT_BOLDER, "Arial");

            var textSymbol = new esri.symbol.TextSymbol(text, symbolFont);
            textSymbol.yoffset = offset;

            ///The site text
            var graphic = new esri.Graphic();
            graphic.setGeometry(geometry);
            graphic.setSymbol(textSymbol);

            return graphic;
        }
    }

}