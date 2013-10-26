var mapa;
var ftOcorrencia;
var html5Lat;
var html5Lon;

var avaliacao;
var tipoAvaliacao;
var viewCurrent = 'inicio';
var viewcHistory = 'inicio';

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
        tipoAvaliacao = ONIBUS;
        mostraView('avaliacao');
    })
    
    $('#btnPonto').click(function(){
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

function mostraView(view) {
    $('#'+viewCurrent).hide();
    $('#'+view).fadeIn("slow");
    viewcHistory = viewCurrent;
    viewCurrent = view;
    if(view == 'mapa') showMapView();
}

function executarBuscaLinha(valorPesquisa, latlong, isSentidoBairro) {
    /*
    $.ajax("http://andodeonibus.cloudapp.net/api/Linha/LocalizaLinhas?lat=10&lon=12", function(data) {

        $.foreach(data.itens, function(intem){
            //$('#ltResultados').append('<p class="linhac" rel="">Linha <strong>'++'</strong></p>');
        });
          
        $('#criteriaBusca').hide();
        $('#resultadoBusca').show();
        
        $('.linhac').click(function(){
            alert(1111);
        });
    });*/
    
    $.ajax({
            url: "http://andodeonibus.cloudapp.net/api/Linha/LocalizaLinhas?lat=10&lon=12",
            type: 'GET',
            error: function (e) { 
                    alert(e);
            },
            success: function(obj) {
                console.log(obj);
            }
        });
}

function showMapView() {
    require(["esri/map", "esri/layers/FeatureLayer", "esri/dijit/InfoWindowLite", "dojo/domReady!"], 
            function(Map, FeatureLayer, InfoWindowLite) {
        mapa = new Map("mapaDiv", {
          basemap: "topo",
          sliderStyle: "small"
        });
    
        ftOcorrencia = new 
            FeatureLayer("http://services2.arcgis.com/Yqsg32QMynROaTjv/arcgis/rest/services/avaliacao/FeatureServer/0");
                
        dojo.connect(mapa, "onLoad", function () {
            setHTML5Location();
        });
    });
}


function salvarOcorrencia() {
    //currentMapPoint 
    ftOcorrencia
    
}