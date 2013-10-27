var mapa;
var ftOcorrencia;
var html5Lat;
var html5Lon;

var avaliacao;
var tipoAvaliacao;
var viewCurrent = 'inicio';
var viewcHistory = 'inicio';
var codLinhaSelecionado;

var ONIBUS = 0;
var PONTO = 1;
var POSITIVO = 1;
var NEGATIVO = 0;

$(document).ready(function() { 
    $.support.cors = true;
    mapeiaBotoes();
    $("html").niceScroll({cursorcolor:"#fff"});
    $('input.myClass').prettyCheckable();
});

$('.nav-collapse .nav > li > a').click(function() {
    $('.collapse.in').removeClass('in').css('height', '0');
});


function mapeiaBotoes() {
    
    $('#btnLogin').click(function(){
        fblogin();
    })
    
    $('#btnBus').click(function(){
        if(!validarLogado())return;
        tipoAvaliacao = ONIBUS;
        mostraView('avaliacao');
    })
    
    $('#btnPonto').click(function(){
        if(!validarLogado())return;
        tipoAvaliacao = PONTO;
        mostraView('avaliacao');
    });
    
    $('#btnPositivo').click(function(){
        avaliacao = POSITIVO;
        mostraView(tipoAvaliacao == ONIBUS ? 'selOnibus' : 'mapa');
    });
    
    $('#btnNegativo').click(function(){
        avaliacao = NEGATIVO;
        mostraView(tipoAvaliacao == ONIBUS ? 'tOnibus' : 'tPonto');
    });
    
    $('#BtnBuscarOnibus').click(function(){
        executarBuscaLinha($('#TxtCodigoLinha').val(), null, true);
    });
    
    $('#refazerBusca').click(function(){
        $('#resultadoBusca').hide();
        $('#criteriaBusca').show();
    });
    
    $('#btnConf').click(function(){
        salvarOcorrencia();
    });
    
    $('.voltar').click(function(){
        mostraView(viewcHistory);
    });
}

function validarLogado() {
    if(usuario == undefined || usuario == null) {
        bootbox.alert("Faça o login com o facebook primeiramente.");
        return false;
    }
    return true;
}

function mostraView(view) {
    $('#'+viewCurrent).hide();
    $('#'+view).fadeIn("slow");
    viewcHistory = viewCurrent;
    viewCurrent = view;
    if(view == 'mapa') showMapView();
}

function executarBuscaLinha(valorPesquisa, latlong, isSentidoBairro) {
    if(valorPesquisa == "") {
        bootbox.alert("Preencha o código da linha");
        return;
    }
    
    $('#ltResultados').empty();
    var query = new esri.tasks.Query();
    query.text = valorPesquisa;
    query.outFields = ["*"];
    var queryTask = new esri.tasks.QueryTask("http://10.1.1.213/ArcGIS/rest/services/ONIBUS/MapServer/2");
    queryTask.execute(query, function(featureSet){ 
        if(featureSet.features.length == 0) {
            bootbox.alert("Nenhuma linha foi encontrada");
            return;
        }
    
        $('.resultCount').empty().append(featureSet.features.length+' linha(s) encontrada(s)');
        
        $.each(featureSet.features, function(){
            var cdLinha = this.attributes["CD_LINHA"];
            var letreiro = this.attributes["LETREIRO"];
            $('#ltResultados').append('<p class="linhac" rel="'+cdLinha+'">Letreiro <strong>'+letreiro+'</strong></p>');
        })
        
        $(".linhac").click(function(){
            $(".linhac").removeClass('linhacs');
            $(this).addClass('linhacs');
            codLinhaSelecionado = $(this).attr('rel');
        });
        
        $('#criteriaBusca').hide();
        $('#resultadoBusca').show();
    });
}

function showMapView() {
    
    dojo.require("esri.tasks.query");
    
    require(["esri/map", 
             "esri/layers/FeatureLayer",
             "esri/graphic",    
             "esri/dijit/InfoWindowLite", 
             "dojo/domReady!"], 
            function(Map, FeatureLayer, Graphic, InfoWindowLite) {
        mapa = new Map("mapaDiv", {
          basemap: "topo",
          sliderStyle: "small"
        });
                
        dojo.connect(mapa, "onLoad", function () {
            setHTML5Location();
        });
                
        ftLinha = new 
            FeatureLayer("http://services2.arcgis.com/Yqsg32QMynROaTjv/arcgis/rest/services/avaliacao/FeatureServer/0");  
    
        ftOcorrencia = new 
            FeatureLayer("http://services2.arcgis.com/Yqsg32QMynROaTjv/arcgis/rest/services/avaliacao/FeatureServer/0");    
    });
}


function salvarOcorrencia() {
    var attr = {
        "ID_USUARIO":usuario.id,
        "TIPO_AVALIACAO":avaliacao == POSITIVO ? "OK" : "NOK",
        "DATA_HORA":new Date().getTime(),
        "TIPO_OCO":tipoAvaliacao == ONIBUS ? "ONIBUS" : "PONTO",
        "SUBTIPO_OCO":"MAL ILUMINADO",
        "SENTIDO":"",
        "LINHA":"",
    };
    var ocorrencia = new esri.Graphic(currentMapPoint, null, attr, null);
    ftOcorrencia.applyEdits([ocorrencia], null, null);
}