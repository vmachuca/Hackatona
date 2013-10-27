dojo.require("dijit.dijit"); // optimize: load dijit layer
dojo.require("dijit.Menu");
dojo.require("dijit.TooltipDialog");
dojo.require("dijit.Dialog");

dojo.require("dijit.DropDownMenu");
dojo.require("dijit.MenuItem");

dojo.require("dojox.form.BusyButton");
dojo.require("dijit.form.CheckBox");
dojo.require("dijit.form.TextBox");
dojo.require("dijit.form.ComboBox");
dojo.require("dijit.form.Textarea");
dojo.require("dijit.form.DateTextBox");

dojo.require("dijit.form.MultiSelect");
dojo.require("dijit.form.NumberTextBox");
dojo.require("dijit.form.VerticalSlider");

dojo.require("dijit.form.FilteringSelect");
dojo.require("dojo.data.ItemFileReadStore");

dojo.require("dijit.layout.BorderContainer");
dojo.require("dijit.layout.ContentPane");
dojo.require("dijit.layout.AccordionContainer");
dojo.require("dijit.layout.TabContainer");

dojo.require("esri.dijit.Scalebar");
dojo.require("esri.dijit.Measurement");
dojo.require("esri.layers.FeatureLayer");
dojo.require("esri.tasks.query");
dojo.require("dojox.widget.Standby");

dojo.require("dojo.store.Memory");

dojo.require("dojo._base.connect");

///ESRI Libs

dojo.require("esri.map");
dojo.require("esri.toolbars.navigation");
dojo.require("esri.symbols.PictureMarkerSymbol");
dojo.require("esri.symbols.SimpleMarkerSymbol");
dojo.require("esri.renderers.UniqueValueRenderer");
dojo.require("esri.toolbars.draw");

//dojo.require("esri.TimeExtent");
//dojo.require("esri.dijit.TimeSlider");

dojo.require("esri.dijit.editing.AttachmentEditor");
dojo.require("dojo.parser");
dojo.require("dojo.dom");
dojo.require("dijit.layout.BorderContainer");
dojo.require("dijit.layout.ContentPane");
dojo.require("dojo.domReady!");

///Setting the culture
Globalize.culture('pt-BR');

///Business variabels
var avalBL = new Avaliacao();
var avlBL = new AVL();

var utils = new Utils();
var layout = new LayoutWindows();
var layoutUtils = new LayoutUtils();

///Global variables
var map = null;
var mapNavigationToolbar = null;
var mapInitialExtent = null;

var standBy = null;

///Layers variables

var bingMapsLayer = null;
var busStopLayer = null;
var avalLayer = null;

var attachmentEditor = null;

///The onload initialization
dojo.addOnLoad(init);

function init() {

    //////Setting the proxy
    esri.config.defaults.io.proxyUrl = "proxy.ashx";
    esri.config.defaults.io.alwaysUseProxy = false;
    esri.config.defaults.io.timeout = 600000;
    
    mapInitialExtent = new esri.geometry.Extent({
    
		        xmax:	-5190557.9236352965	,
		        xmin:	-5192189.37645762	,
		        ymax:	-2697825.206899761	,
		        ymin: -2698628.990025255  ,

                //xmax:	-5176946.160957651	,
		        //xmin:	-5203049.406117003	,
		        //ymax:	-2692389.220427236	,
		        //ymin:	-2705249.7504361993	,
    
                spatialReference: {
                    wkid: 102113 
                }
            });

    ///Initializes the map and the navigation toolbar
    map = new esri.Map("map", {
            extent: mapInitialExtent
        });

    mapNavigationToolbar = new esri.toolbars.Navigation(map);      

    ///Initializing
    initializeLayers();
    initializeEvents();
    initializeControls();
    initializeWindows();
}

function initializeLayers() {

    heatMapLayerInit();

    ///The bing maps layer
    bingMapsLayer = new esri.virtualearth.VETiledLayer({
        bingMapsKey: 'Ag3iU6CAh_mGTsXyq_WZO74dsAjItGE5TaJYeLVUA2qEuTY2OJ5OS1Mg4uZvhSze',
        mapStyle: esri.virtualearth.VETiledLayer.MAP_STYLE_ROAD,
        label: 'Bing',
        image: 'resources/images/map/layer/globe_16x16.png',
        displayOnPan: true
    });

    busStopLayer = new esri.layers.FeatureLayer('http://10.1.1.213/ArcGIS/rest/services/ONIBUS/MapServer/1', {
        mode: esri.layers.FeatureLayer.MODE_ONDEMAND,
        visible: false,
        label: 'Pontos de Ônibus'
    });

    avalLayer = new esri.layers.FeatureLayer('http://services2.arcgis.com/Yqsg32QMynROaTjv/arcgis/rest/services/avaliacao/FeatureServer/0', {
        mode: esri.layers.FeatureLayer.MODE_ONDEMAND,
        visible: true,
        label: 'Avaliação',
        outFields: ["*"]
    });

    //---------------
    //Attachments

    dojo.connect(avalLayer, "onClick", function (evt) {


        if (evt.graphic.attributes['TIPO_AVALIACAO'] == 'OK') return;

        var objectId = evt.graphic.attributes[avalLayer.objectIdField];

        var url = String.format('http://services2.arcgis.com/Yqsg32QMynROaTjv/arcgis/rest/services/avaliacao/FeatureServer/0/{0}/attachments?f=json', objectId);

        var content =
        '<table style="width: 100%" border="0">' +
            '<tr>' +
                '<th>Facebook</th>' +
                '<td><a href="https://www.facebook.com/' + evt.graphic.attributes['ID_USUARIO'] + '">' + 'Ir para página' + '</a></td>' +
            '</tr>' +
            '<tr>' +
                '<th>Tipo</th>' +
                '<td>' + evt.graphic.attributes['TIPO_OCO'] + '</td>' +
            '</tr>' +
            '<tr>' +
                '<th>Problema</th>' +
                '<td>' + evt.graphic.attributes['SUBTIPO_OCO'] + '</td>' +
            '</tr>' +
            '{0}' +
        '</table>';

        ///Requesting
        utils.Ajax.Get(
            {
                dataType: 'jsonp',
                url: url
            },
            function (json) {

                if (json.attachmentInfos.length > 0) {

                    var imgSrc = String.format('http://services2.arcgis.com/Yqsg32QMynROaTjv/arcgis/rest/services/avaliacao/FeatureServer/0/{0}/attachments/{1}', objectId, json.attachmentInfos[0].id);

                    content = String.format(content,
                    '<tr>' +
                        '<td colspan="2"><img src="' + imgSrc + '"></td>' +
                    '</tr>');
                }
                else
                    content = String.format(content, '');

                map.infoWindow.resize(350, 200);
                map.infoWindow.setContent(content);
                map.infoWindow.show(evt.screenPoint, map.getInfoWindowAnchor(evt.screenPoint));
            },
            function (error) {

                map.infoWindow.resize(350, 200);
                map.infoWindow.setContent(content);
                map.infoWindow.show(evt.screenPoint, map.getInfoWindowAnchor(evt.screenPoint));
            });
    });

    //---------------

    //create renderer
    var renderer = new esri.renderers.UniqueValueRenderer(esri.symbols.SimpleMarkerSymbol(), "TIPO_AVALIACAO");

    //add symbol for each possible value
    renderer.addValue("OK", new esri.symbols.PictureMarkerSymbol('resources/images/map/PIN-LIKE.png', 25, 25));
    renderer.addValue("NOK", new esri.symbols.PictureMarkerSymbol('resources/images/map/PIN-DISLIKE.png', 25, 25));

    avalLayer.setRenderer(renderer);

    utils.Window.HeatMap.Variables.Layers.push(avalLayer);

    ///Adding the layers to the map
    try {
        map.addLayer(bingMapsLayer);
        map.addLayer(busStopLayer);
        map.addLayer(avalLayer);
        map.addLayer(new esri.layers.FeatureLayer('http://10.1.1.213/ArcGIS/rest/services/ONIBUS/MapServer/0', {
            mode: esri.layers.FeatureLayer.MODE_ONDEMAND,
            visible: true,
            label: 'Paradas'
        }));
    }
    catch (e) {
        showMessage('Erro ao carregar os Layers', 'Desculpe, problemas técnicos');
    }
}

function initializeEvents() {

    ///Map
    //dojo.connect(map, "onClick", avlBL.Window.AS.Events.mapclick);
    //dojo.connect(busStopLayer, "click", avlBL.Window.Average.Events.LayerClick);
    dojo.connect(busStopLayer, "onClick", avlBL.Window.Average.Events.LayerClick);
    

    //Window
    dojo.connect(window, "onresize", function () {
        map.resize();
    });
}

function initializeWindows() {

    //eventBL.Window.CriticalEvents.Show();
    //eventBL.Window.Timer.Show();
    //utils.Window.ToC.Show();
}

function initializeControls() {
    $("#opacitySlider").slider({
        range: "min",
        value: 10,
        min: 2,
        max: 10,
        slide: function (event, ui) {

            $('.ui-dialog').css('opacity', ui.value / 10);
        }
    });
}