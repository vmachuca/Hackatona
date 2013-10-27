///core.bl.evento.js
function AVL() {

    ///Subclass Window
    this.Window = {

        Average: {
            Events: {
                LayerClick: function (evt) {

                    ///Vefify if the layer is visible
                    if (busStopLayer.visible) {

                        ///Creating the parameters
                        var params = new esri.tasks.BufferParameters();
                        params.geometries = [evt.graphic.geometry];
                        params.distances = [50];
                        params.unit = esri.tasks.GeometryService.UNIT_METER;

                        ///Creating the geometry service and buffering
                        var geometryService = new esri.tasks.GeometryService('http://192.168.245.53/ArcGIS/rest/services/Geometry/GeometryServer');
                        geometryService.buffer(params, function (geometries) {

                            ///The query parameters
                            var query = new esri.tasks.Query();
                            query.geometry = geometries[0];
                            query.returnGeometry = true;
                            query.outSpatialReference = map.spatialReference;
                            query.outFields = ["*"];

                            ///The query task
                            var queryTask = new esri.tasks.QueryTask('http://192.168.245.53/ArcGIS/rest/services/ONIBUS/MapServer/0');
                            queryTask.execute(query, function (data) {

                                //Getting the features
                                var features = data.features;

                                features.sort(function (a, b) {
                                    a = new Date(Globalize.parseDate(a.attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000'));
                                    b = new Date(Globalize.parseDate(b.attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000'));
                                    return a < b ? -1 : a > b ? 1 : 0;
                                });

                                //************************************
                                //AverageTime
                                var dictionatyFeatures = [];
                                dictionatyFeatures['KEYS'] = [];

                                $(features).each(function (featureIndex, feature) {

                                    var key = feature.attributes['CD_LINHA'] + '-' + feature.attributes['VEICULO'];

                                    if (dictionatyFeatures[key] == undefined) {
                                        dictionatyFeatures[key] = [];
                                        dictionatyFeatures['KEYS'].push(key);
                                    }

                                    dictionatyFeatures[key].push(feature);
                                });

                                var analysisFeatures = [];

                                $(dictionatyFeatures['KEYS']).each(function (keyIndex, key) {


                                    if (dictionatyFeatures[key].length == 1)
                                        return;
                                    else if (dictionatyFeatures[key].length == 2) {

                                        var a = Globalize.parseDate(dictionatyFeatures[key][0].attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000');
                                        var b = Globalize.parseDate(dictionatyFeatures[key][1].attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000');

                                        var hor = (Globalize.format(b, 'HH') - Globalize.format(a, 'HH')) * 60 * 60;
                                        var min = (Globalize.format(b, 'mm') - Globalize.format(a, 'mm')) * 60;
                                        var seg = Globalize.format(b, 'ss') - Globalize.format(a, 'ss');

                                        analysisFeatures.push(hor + min + seg);
                                    }
                                    else {

                                        var first = null;
                                        var last = null;

                                        for (var anaIndex = 0; anaIndex < dictionatyFeatures[key].length - 1; anaIndex++) {


                                            var a = Globalize.parseDate(dictionatyFeatures[key][anaIndex].attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000');
                                            var b = Globalize.parseDate(dictionatyFeatures[key][anaIndex + 1].attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000');

                                            var hor = (Globalize.format(b, 'HH') - Globalize.format(a, 'HH')) * 60 * 60;
                                            var min = (Globalize.format(b, 'mm') - Globalize.format(a, 'mm')) * 60;
                                            var seg = Globalize.format(b, 'ss') - Globalize.format(a, 'ss');

                                            tot = hor + min + seg;

                                            if (!(tot >= (30 * 60))) {

                                                if (first == null)
                                                    first = dictionatyFeatures[key][anaIndex];
                                                else {
                                                    last = dictionatyFeatures[key][anaIndex];

                                                    var aTot = Globalize.parseDate(first.attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000');
                                                    var bTot = Globalize.parseDate(last.attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000');

                                                    var horTot = (Globalize.format(bTot, 'HH') - Globalize.format(aTot, 'HH')) * 60 * 60;
                                                    var minTot = (Globalize.format(bTot, 'mm') - Globalize.format(aTot, 'mm')) * 60;
                                                    var segTot = Globalize.format(bTot, 'ss') - Globalize.format(aTot, 'ss');

                                                    analysisFeatures.push(horTot + minTot + segTot);
                                                    break;
                                                }
                                            }
                                        }

                                    }
                                });

                                var averageTime = 0;

                                $(analysisFeatures).each(function (valueIndex, value) {
                                    averageTime += value;
                                });

                                averageTime = averageTime / analysisFeatures.length;

                                if (averageTime < 60)
                                    $('#' + avlBL.Window.Average.Controls.MessageAverrageTime).html(averageTime + ' segundos');
                                else
                                    $('#' + avlBL.Window.Average.Controls.MessageAverrageTime).html(new Number((averageTime / 60)).toFixed(2) + ' minutos');

                                //************************************
                                //AverageSpeed
                                var dictionatyFeatures = [];
                                dictionatyFeatures['KEYS'] = [];

                                $(features).each(function (featureIndex, feature) {

                                    var key = feature.attributes['CD_LINHA'] + '-' + feature.attributes['VEICULO'];

                                    if (dictionatyFeatures[key] == undefined) {
                                        dictionatyFeatures[key] = [];
                                        dictionatyFeatures['KEYS'].push(key);
                                    }

                                    if (dictionatyFeatures[key].length == 2)
                                        dictionatyFeatures[key][1] = feature;

                                    else
                                        dictionatyFeatures[key].push(feature);
                                });

                                var averageSpeedArray = [];

                                $(dictionatyFeatures['KEYS']).each(function (keyIndex, key) {

                                    if (dictionatyFeatures[key].length == 2) {

                                        var a = Globalize.parseDate(dictionatyFeatures[key][0].attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000');
                                        var b = Globalize.parseDate(dictionatyFeatures[key][1].attributes['DT_AVL'], 'yyyy-MM-dd HH:mm:ss.000');

                                        var hor = (Globalize.format(b, 'HH') - Globalize.format(a, 'HH'));
                                        var min = (Globalize.format(b, 'mm') - Globalize.format(a, 'mm')) / 60;
                                        var seg = Globalize.format(b, 'ss') - Globalize.format(a, 'ss') / 60 / 60;

                                        var totalHours = hor + min + seg;

                                        var xa = dictionatyFeatures[key][0].geometry.x;
                                        var ya = dictionatyFeatures[key][0].geometry.y;

                                        var xb = dictionatyFeatures[key][1].geometry.x;
                                        var yb = dictionatyFeatures[key][1].geometry.y;

                                        var averageSpeedItem = function (totalHours, xa, ya, xb, yb) {

                                            var diffX = Math.abs(xa - xb);
                                            var diffY = Math.abs(ya - yb);
                                            var catX = Math.pow(diffX, 2);
                                            var catY = Math.pow(diffY, 2);
                                            var cats = catX + catY;
                                            var hip = Math.sqrt(cats);
                                            var hipKm = hip / 1000;
                                            var hipKmPerHour = hipKm / totalHours;

                                            return hipKmPerHour;

                                        } (totalHours, xa, ya, xb, yb);

                                        averageSpeedArray.push(averageSpeedItem);
                                    }
                                });

                                var averageSpeedSum = 0;

                                $(averageSpeedArray).each(function (valueIndex, value) {
                                    averageSpeedSum += value;
                                });

                                averageSpeedFinal = averageSpeedSum / averageSpeedArray.length;
                                $('#' + avlBL.Window.Average.Controls.MessageAverrageSpeed).html(averageSpeedFinal + ' km/h');
                            },
                            function (err, a, b) {

                            });
                        },
                        function (error) {

                        });

                    }
                }
            },
            Controls: {
                Button: 'averageTimeButton',
                MessageAverrageTime: 'averageTimeSpan',
                MessageAverrageSpeed: 'averageSpeedSpan'
            },
            Show: function () {

                ///The content
                var content = '<input style="width:100%" type="checkbox" id="' + avlBL.Window.Average.Controls.Button + '" /><label for="' + avlBL.Window.Average.Controls.Button + '">' + busStopLayer._params.label + '</label><br/>Habilite a camada de Pontos de ônibus, e clique para consultar o tempo médio de espera e a velocidade média<br/><br/>' +
                              'Tempo médio: <span id="' + avlBL.Window.Average.Controls.MessageAverrageTime + '" style="font-weight: bold"/><br/><br/>' +
                              'Velocidade média: <span id="' + avlBL.Window.Average.Controls.MessageAverrageSpeed + '" style="font-weight: bold"/>';

                ///The post function
                var postFunction = function () {

                    $('#' + avlBL.Window.Average.Controls.Button)
                        .button()
                        .click(function (event) {
                            if (busStopLayer.visible)
                                busStopLayer.hide();
                            else
                                busStopLayer.show();
                        });
                };

                ///window options
                var options = {
                    height: 200,
                    width: 300,
                    titleIcon: 'resources/images/header/buttons/clock_24x24.png',
                    float: 'right',
                    x: 2,
                    y: 57
                };

                ///Show the window
                layoutUtils.make('averageWindow', 'Média de Tempo e Velocidade', content, options, postFunction);

            }
        }
    }
}