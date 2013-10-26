var mapa;

var avaliacao;
var tipoAvaliacao;
var viewCurrent = 'inicio';
var viewcHistory = 'inicio';

var ONIBUS = 0;
var PONTO = 1;
var POSITIVO = 1;
var NEGATIVO = 0;

$(document).ready(function() {  
    mapeiaBotoes();
    $("html").niceScroll({cursorcolor:"#fff"});
});

$('.nav-collapse .nav > li > a').click(function() {
    $('.collapse.in').removeClass('in').css('height', '0');
});


function mapeiaBotoes() {
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
        mostraView(tipoAvaliacao == ONIBUS ? 'selOnibus' : 'probPonto');
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

function showMapView() {
    require(["esri/map", "dojo/domReady!"], function(Map) {
        mapa = new Map("mapaDiv", {
          basemap: "topo",
          center: [-122.45,37.75], // long, lat
          zoom: 13,
          sliderStyle: "small"
        });
    });
}
