var usuario
var HOSTAPI = "http://10.1.1.45/EuAndodeonibus/api/";

var script = document.createElement( 'script' );
script.async = true;
script.src = document.location.protocol + '//connect.facebook.net/en_US/all.js';
document.getElementById( 'fb-root' ).appendChild( script );

window.fbAsyncInit = function() {
  FB.init({
    appId      : '528046540617591',
    channelUrl : 'http://localhost/channel.html', // Channel File
    status     : true, // check login status
    cookie     : true, // enable cookies to allow the server to access the session
    xfbml      : true  // parse XFBML
  });
}


function fblogin() {
    showOverlay();
    FB.login( function ( response ) {
        if ( response.authResponse ) {
            try {
                FB.api( '/me', function ( response ) {
                    //alert( 'success: ' + response.name );
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

function salvarOcorencia(callback) {
    var linha = 'null';
    var subtipo = 'null';
    if(subtipoSelecionadoCod != null) subtipo = subtipoSelecionadoCod;
    if(codLinhaSelecionado != null) linha = codLinhaSelecionado;
    if(usuario.hometown != undefined) cidade = usuario.hometown.name; 
    $.post(HOSTAPI + 'Ocorrencia?token='+usuario.id+'&tipo='+tipoAvaliacao+'&subtipo='+subtipo+'&status='+avaliacao+'&linha='+linha, function(result){callback(result)});
};
