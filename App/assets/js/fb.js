var usuario
var HOSTAPI = "http://localhost/EuAndodeonibus/api/";

var script = document.createElement('script');
script.async = true;
script.src = document.location.protocol + '//connect.facebook.net/en_US/all.js';
document.getElementById('fb-root').appendChild( script );

window.fbAsyncInit = function() {
  FB.init({
    appId      : '528046540617591',
    channelUrl : 'http://localhost/channel.html',
    status     : true,
    cookie     : true,
    xfbml      : true
  });
}

function fblogin() {
    showOverlay();
    FB.login( function ( response ) {
        if ( response.authResponse ) {
            try {
                FB.api( '/me', function ( response ) {
                    usuario = response;
                    salvarUsuario();
                    $('#ola').empty().append('Olá '+usuario.first_name+', o que deseja avaliar?');
                    $('#loginfb').hide();
                    $('#inicio').show();
                    hideOverlay();
                } );
            } catch ( error ) {
                hideOverlay();
                alert( 'error: ' + error );
            };
        } else {
            hideOverlay();
            alert( 'unauthorized' );
        };
    }, { scope: 'email' } );

};

function salvarUsuario() {
    var cidade = 'null';
    if(usuario.hometown != undefined) cidade = usuario.hometown.name; 
    $.post(HOSTAPI + 'Usuario/Cadastra?token='+usuario.id+'0&nome='+usuario.name+'&cidade='+cidade +'&estado=null&idade=0', function(result){});
};

function salvarOcorenciaBd(callback) {
    var linha = 'null';
    var subtipo = 'null';
    if(subtipoSelecionadoCod != null) subtipo = subtipoSelecionadoCod;
    if(codLinhaSelecionado != null) linha = codLinhaSelecionado;
    $.post(HOSTAPI + 'Usuario/Ocorrencia?token='+usuario.id+'&tipo='+tipoAvaliacao+'&subtipo='+subtipo+'&status='+avaliacao+'&linha='+linha, function(result){callback(result)});
};
