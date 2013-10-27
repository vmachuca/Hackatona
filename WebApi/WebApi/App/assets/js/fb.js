var usuario

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
    FB.login( function ( response ) {
        if ( response.authResponse ) {
            try {

                FB.api( '/me', function ( response ) {
                    //alert( 'success: ' + response.name );
                    usuario = response;
                    salvarUsuario();
                } );
            } catch ( error ) {
                alert( 'error: ' + error );
            };
        } else {
            alert( 'unauthorized' );
        };
    }, { scope: 'email' } );

};

function salvarUsuario() {
        
    $.get('http://10.1.1.45/EuAndodeonibus/api/Usuario/Cadastra?token='+usuario.id+'0&nome='+usuario.name+'&cidade='+usuario.hometown.name +'&estado=null&idade=24', function(result){
        console.log(result);
    });
};

