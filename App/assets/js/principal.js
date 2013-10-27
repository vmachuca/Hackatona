var mapa;
var ftOcorrencia;
var html5Lat;
var html5Lon;
var temFoto;

var avaliacao;
var tipoAvaliacao;
var viewCurrent = 'inicio';
var viewcHistory = 'inicio';
var codLinhaSelecionado;
var subtipoSelecionado;
var subtipoSelecionadoCod;

var ONIBUS = 0;
var PONTO = 1;
var POSITIVO = 1;
var NEGATIVO = 0;

$(document).ready(function() { 
    $.support.cors = true;
    mapeiaBotoes();
    $("html").niceScroll({cursorcolor:"#fff"});
    $('input.myClass').prettyCheckable();
    $('input[type=file]').bootstrapFileInput();
    hideOverlay();
});

$('.nav-collapse .nav > li > a').click(function() {
    $('.collapse.in').removeClass('in').css('height', '0');
});


function mapeiaBotoes() {
    
    
    $('#btnfb').click(function(){
        fblogin();
    })
    
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
    
    $('#btnConfLinha').click(function(){
        mostraView('mapa');
    });
    
    $('#btnConf').click(function(){
        salvarOcorrencia();
    });
    
    $('.op').click(function(){
        subtipoSelecionado = selecionaSubtipo(this);
        
        if(avaliacao == POSITIVO)
            mostraView(tipoAvaliacao == ONIBUS ? 'selOnibus' : 'mapa');
        else
            mostraView('foto');
    });
    
    $('#btnEnviarFoto').click(function(){
        temFoto = true; 
        mostraView(tipoAvaliacao == ONIBUS ? 'selOnibus' : 'mapa');
    });
    
    $('#btnFotoPular').click(function(){
        mostraView(tipoAvaliacao == ONIBUS ? 'selOnibus' : 'mapa');
    });
    
    $('.voltar').click(function(){
        mostraView(viewcHistory);
    });
    
    $('.voltarIni').click(function(){
        mostraView('inicio');
    });
}

function validarLogado() {
    temFoto = false;
    codLinhaSelecionado = null;
    subtipoSelecionado = null;
    subtipoSelecionadoCod = null;
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

function mostrarInsignia(id) {
    $('.bd').hide();
    if(id == 0)
        $('.bc').show();
    else if(id == 1)
        $('.be').show();
    else if(id == 2)
        $('.bk').show();
    else if(id == 3)
        $('.bp').show();
    else
        $('.bh').show();
    mostraView('newBedge');
}

function executarBuscaLinha(valorPesquisa, latlong, isSentidoBairro) {
    if(valorPesquisa == "") {
        bootbox.alert("Preencha o código da linha");
        return;
    }
    
    showOverlay();
    
    $('#ltResultados').empty();
    var query = new esri.tasks.Query();
    query.text = valorPesquisa;
    query.outFields = ["*"];
    var queryTask = new esri.tasks.QueryTask("http://10.1.1.213/ArcGIS/rest/services/ONIBUS/MapServer/2");
    queryTask.execute(query, function(featureSet){ 
        if(featureSet.features.length == 0) {
            hideOverlay();
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
        hideOverlay();
    });
}

function showMapView() {
    showOverlay();
    
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
    
        ftOcorrencia = new 
            FeatureLayer("http://services2.arcgis.com/Yqsg32QMynROaTjv/arcgis/rest/services/avaliacao/FeatureServer/0");    
    });
}

function salvarOcorrencia() {
    showOverlay();
    var attr = {
        "ID_USUARIO":usuario.id,
        "TIPO_AVALIACAO":avaliacao == POSITIVO ? "OK" : "NOK",
        "DATA_HORA":new Date().getTime(),
        "TIPO_OCO":tipoAvaliacao == ONIBUS ? "ONIBUS" : "PONTO",
        "SUBTIPO_OCO":subtipoSelecionado == null ? "" : subtipoSelecionado,
        "SENTIDO":"",
        "LINHA":codLinhaSelecionado == null ? "" : codLinhaSelecionado,
    };
    var ocorrencia = new esri.Graphic(currentMapPoint, null, attr, null);
    ftOcorrencia.applyEdits([ocorrencia], null, null, function(s){
        
        if(temFoto)
            ftOcorrencia.addAttachment(s[0].objectId, document.getElementById('formFoto'));
        
        salvarOcorenciaBd(function(result){
            hideOverlay();
            bootbox.alert("Ocorrência #"+s[0].objectId+" criada com sucesso!"); 
            
            if(result == null)
                mostraView('inicio');
            else
                mostrarInsignia(result[0].Id);       
        });
        //hideOverlay();
        //bootbox.alert("Ocorrência #"+s[0].objectId+" criada com sucesso!"); 
        //mostraView('inicio');
        
    }, function(err){
        hideOverlay();
        bootbox.alert("Ocorreu um erro para salvar a ocorrência.");    
    });
}                            

function selecionaSubtipo(item) {
    subtipoSelecionadoCod = item.id.substring(2);
    var codigo = subtipoSelecionadoCod;
    
    if(tipoAvaliacao == ONIBUS){
        if(codigo == 1)
            return 'DEFEITOS';
        else if(codigo == 2)
            return 'SUPERLOTADO';
        else if(codigo == 3)
            return 'NAO PAROU';
        else
            return 'FALTA DE URBANIDADE';
    }
    
    if(codigo == 1)
        return 'SUJO';
    else if(codigo == 2)
        return 'SUPERLOTADO';
    else if(codigo == 3)
        return 'TETO QUEBRADO';
    else if(codigo == 4)
        return 'BANCO QUEBRADO';
    else if(codigo == 5)
        return 'ONIBUS ATRASADO';
    else
        return 'MAL ILUMINADO';
}

function showOverlay() {
    $('#overlayapp').show();
}

function hideOverlay() {
    $('#overlayapp').hide();
}