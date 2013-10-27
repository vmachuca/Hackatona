///core.bl.evento.js
function Avaliacao() {

    ///Subclass Window
    this.Window = {

        Chart: {
            Variables: {
                DrawToolbar: null,
                Status: 'Selecionar'
            },
            Controls: {
                DrawButton: 'chartDrawButton',
                ChartPie: 'chartPieChart',
                ChartLine: 'chartLineChart'
            },
            Methods: {
                DrawStart: function () {

                    avalBL.Window.Chart.Variables.DrawToolbar.activate(esri.toolbars.Draw['EXTENT']);
                    map.hideZoomSlider();
                    avalBL.Window.Chart.Variables.Status = 'Cancelar';
                    $('#' + avalBL.Window.Chart.Controls.DrawButton + ' span').html(avalBL.Window.Chart.Variables.Status);
                },
                DrawCancel: function () {

                    avalBL.Window.Chart.Variables.DrawToolbar.deactivate();
                    map.showZoomSlider();
                    avalBL.Window.Chart.Variables.Status = 'Selecionar';
                    $('#' + avalBL.Window.Chart.Controls.DrawButton + ' span').html(avalBL.Window.Chart.Variables.Status);
                },

                DrawEnd: function (evt) {

                    avalBL.Window.Chart.Methods.DrawCancel();

                    ///set up query
                    var query = new esri.tasks.Query();
                    query.geometry = evt.geometry;
                    query.outFields = ['*'];
                    query.outSpatialReference = map.spatialReference;
                    query.returnGeometry = false;
                    avalLayer.queryFeatures(query, function (featureSet) {

                        var features = [];

                        if (featureSet && featureSet.features && featureSet.features.length > 0) {

                            features = featureSet.features;
                        }

                        var lineOkList = [];
                        var lineNOkList = [];

                        var lineOkValues = [];
                        var lineNOkValues = [];

                        ///For each feature
                        $(features).each(function (featureIndex, feature) {

                            if (feature.attributes['TIPO_AVALIACAO'] == 'OK')
                                lineOkList.push(feature.attributes);
                            else
                                lineNOkList.push(feature.attributes);
                        });

                        features.sort(function (a, b) {
                            a = new Date(a.attributes['DATA_HORA']);
                            b = new Date(b.attributes['DATA_HORA']);
                            return a < b ? -1 : a > b ? 1 : 0;
                        });

                        var categories = [];
                        var seriesOK = [];
                        var seriesNOK = [];

                        ///For each feature
                        $(features).each(function (featureIndex, feature) {

                            var minute = Globalize.format(new Date(feature.attributes['DATA_HORA']), 'mm');

                            if (categories[minute] == undefined) {
                                categories.push(minute);
                                categories[minute] = true;
                            }

                            if (seriesOK[minute] == undefined)
                                seriesOK[minute] = 0;
                            if (seriesNOK[minute] == undefined)
                                seriesNOK[minute] = 0;

                            if (feature.attributes['TIPO_AVALIACAO'] == 'OK')
                                seriesOK[minute] += 1;
                            else
                                seriesNOK[minute] += 1;
                        });

                        seriesOK['CATEGORIA'] = [];
                        seriesNOK['CATEGORIA'] = [];

                        ///For each feature
                        $(categories).each(function (categoryIndex, category) {

                            seriesOK['CATEGORIA'].push(seriesOK[category]);
                            seriesNOK['CATEGORIA'].push(seriesNOK[category]);
                        });

                        $('#' + avalBL.Window.Chart.Controls.ChartPie).highcharts({
                            chart: {
                                plotBackgroundColor: null,
                                plotBorderWidth: null,
                                plotShadow: false
                            },
                            title: {
                                text: 'Total selecionado: ' + (lineOkList.length + lineNOkList.length)
                            },
                            tooltip: {
                                pointFormat: '{series.name}: <b>{point.y}</b>'
                            },
                            plotOptions: {
                                pie: {
                                    allowPointSelect: true,
                                    cursor: 'pointer',
                                    dataLabels: {
                                        enabled: true,
                                        color: '#000000',
                                        connectorColor: '#000000',
                                        format: '<b>{point.name}</b>: {point.y}'
                                    }
                                }
                            },
                            series: [{
                                type: 'pie',
                                name: 'Quantidade',
                                data: [
                                        {
                                            name: 'Elogios',
                                            y: lineOkList.length,
                                            color: '#0000ff'
                                        },
                                        {
                                            name: 'Críticas',
                                            y: lineNOkList.length,
                                            color: '#ff0000'
                                        }]
                            }]
                        });

                        $('#' + avalBL.Window.Chart.Controls.ChartLine).highcharts({
                            title: {
                                text: 'Elogios / Críticas',
                                x: -20 //center
                            },
                            subtitle: {
                                x: -20
                            },
                            xAxis: {
                                categories: categories
                            },
                            yAxis: {
                                title: {
                                    text: 'Quantidade'
                                },
                                plotLines: [{
                                    value: 0,
                                    width: 1,
                                    color: '#808080'
                                }]
                            },
                            tooltip: {
                            },
                            legend: {
                                layout: 'vertical',
                                align: 'right',
                                verticalAlign: 'middle',
                                borderWidth: 0
                            },
                            series: [{
                                name: 'Elogios',
                                data: seriesOK['CATEGORIA']
                                ,
                                color: '#0000ff'
                            }, {
                                name: 'Críticas',
                                data: seriesNOK['CATEGORIA'],
                                color: '#ff0000'
                            }]
                        });

                    });
                }
            },

            Show: function () {

                ///The content
                var content =
                '<button id="' + avalBL.Window.Chart.Controls.DrawButton + '">' + avalBL.Window.Chart.Variables.Status + '</button><br/>' +
                '<div id="' + avalBL.Window.Chart.Controls.ChartPie + '" style="height: 250px; width: 100%"></div>' +
                '<div id="' + avalBL.Window.Chart.Controls.ChartLine + '" style="height: 250px; width: 100%"></div>';

                ///The post function
                var postFunction = function () {

                    avalBL.Window.Chart.Variables.DrawToolbar = new esri.toolbars.Draw(map);
                    avalBL.Window.Chart.Variables.DrawToolbar.on("draw-end", avalBL.Window.Chart.Methods.DrawEnd);

                    $('#' + avalBL.Window.Chart.Controls.DrawButton)
                        .button()
                        .click(function (event) {

                            if ($('#' + avalBL.Window.Chart.Controls.DrawButton + ' span').html() == 'Selecionar')
                                avalBL.Window.Chart.Methods.DrawStart();
                            else
                                avalBL.Window.Chart.Methods.DrawCancel();
                        });
                };

                ///window options
                var options = {
                    height: 590,
                    width: 480,
                    titleIcon: 'resources/images/header/buttons/chart_24x24.png',
                    float: 'right',
                    x: 2,
                    y: 57
                };

                ///Show the window
                layoutUtils.make('chartWindow', 'Gráficos', content, options, postFunction);

            }
        }
    }

}