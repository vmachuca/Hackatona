var autentiqueUrl = 'http://10.126.111.203/autenticacao/rest/jsonp.svc/';

function validatePermission(domain, userLogin, password) {

    $.ajax({
        cache: false,
        type: "GET",
        dataType: "jsonp",
        async: true,
        url: autentiqueUrl + 'PermissaoUsuario',
        data: {
            pDominio: domain,
            pUsuario: userLogin,
            pSenha: password,
            pSistema: 'sig-di',
            pGerarToken: true,
            output: 'json'
        },
        success: function (result, status, att) {

            $('#loading').hide();

            var rede, sistema, token, telefonica, cookie;
            rede = eval(result.AutenticadoRede);
            sistema = eval(result.AutenticadoSistema);
            token = result.Token;
            cookieValue = result.Cookie + ";" + userLogin;

            if (sistema && rede) {
                if (token != null && token != '') {

                    ///The url parameters
                    var urlParameters = '';

                    ///Getting the parameters
                    var parameters = getUrlVars();

                    ///For each parameter
                    $(parameters).each(function (parameterIndex, parameter) {

                        urlParameters += String.format('&{0}={1}', parameter, parameters[parameter]);

                    });

                    Set_Cookie(token, cookieValue, '', '/', '', '');
                    $(location).attr('href', String.format('Default.aspx?k={0}{1}', token, urlParameters));
                }
                else
                    setLoginMessage(true, 'Problemas no servidor. Favor entrar em contato com o administrador.');
            }
            else if (!rede) {
                setLoginMessage(true, 'Usuário ou Senha inválidos');
            }
            else if (!sistema) {
                setLoginMessage(true, 'Você não tem acesso a este sistema');
            }
            else {
                setLoginMessage(true, 'Erro na autenticação');
            }

            $('#txtSenha').val('');
        }
    });

    setLoginMessage(true, '');
    $('#loading').show();
}