using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using WebApi.Attributes;
using WebApi.Models;

namespace WebApi.Controllers
{
    [AllowCrossSiteJson]
    public class UsuarioController : ApiController
    {
        const string ConnString = "Data Source=NOTEBOOK\\SQLSERVER;Initial Catalog=EuAndoDeOnibus;Integrated Security=False;User ID=sa;Password=abc1234#;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";               

        const string SqlInsertUsuario = "INSERT INTO USUARIO ( Id, FacebookToken, Nome, Cidade, Estado, Idade ) values ( @id, @facebooktoken, @nome, @cidade, @estado, @idade )";

        const string SqlInsertOcorrencia = "INSERT INTO OCORRENCIAUSUARIO ( Id, FacebookToken, TipoOcorrencia, SubtipoOcorrencia, data, status, linha ) values ( @id, @token, @tipo, @subtipo, @data, @status, @linha )";


        const string SqlUsuarioWhereToken = "SELECT * FROM USUARIO WHERE FacebookToken = @token";
        const string SqlUsuarioWhereId = "SELECT * FROM USUARIO WHERE Id = @id";

        [HttpGet]
        [AllowCrossSiteJson]
        public Usuario BuscaId(string id)
        {
            var usuario = new Usuario();


            var adoConn = new SqlConnection(ConnString);
            adoConn.Open();

            try
            {
                var adoCmdSelect = new SqlCommand( SqlUsuarioWhereId  , adoConn);
                adoCmdSelect.Parameters.AddWithValue("id", id);
                var dr = adoCmdSelect.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {                    
                    usuario.Id = Convert.ToInt32(dr["Id"]);
                    usuario.Nome = dr["Nome"].ToString();
                    usuario.FacebookToken = dr["FacebookToken"].ToString();
                    usuario.Cidade = dr["Cidade"].ToString();
                    usuario.Estado = dr["Estado"].ToString();
                    usuario.Idade = Convert.ToInt32(dr["Idade"]);
                    dr.Close();
                }
            }
            finally
            {
                adoConn.Close();
            }


            return usuario;
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public Usuario BuscaToken(string token)
        {
            var usuario = new Usuario();


            var adoConn = new SqlConnection(ConnString);
            adoConn.Open();

            try
            {
                var adoCmdSelect = new SqlCommand(SqlUsuarioWhereToken, adoConn);
                adoCmdSelect.Parameters.AddWithValue("token", token);
                var dr = adoCmdSelect.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {                   
                    usuario.Id = Convert.ToInt32(dr["Id"]);
                    usuario.Nome = dr["Nome"].ToString();
                    usuario.FacebookToken = dr["FacebookToken"].ToString();
                    usuario.Cidade = dr["Cidade"].ToString();
                    usuario.Estado = dr["Estado"].ToString();
                    usuario.Idade = Convert.ToInt32(dr["Idade"]);
                    dr.Close();
                }
            }
            finally
            {
                adoConn.Close();
            }


            return usuario;
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public IEnumerable<Badbadgets> CaracterizaBadgets(string usuarioID)
        {
            return new List<Badbadgets>()
                       {
                           new Badbadgets()
                               {
                                   Id = 1,
                                   Descricao = "Você é uma pessoa ambientalista e anda de onibus "
                               }
                       };
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public Usuario Cadastra(string token,  string nome, string cidade, string estado, int idade)
        {
            var usuario = new Usuario()
                              {
                                  Id = 1,
                                  FacebookToken = token,
                                  Nome = nome,
                                  Cidade = cidade,
                                  Estado = estado,
                                  Idade = idade
                              };


            var adoConn = new SqlConnection(ConnString);
            adoConn.Open();

            try
            {
                var adoCmdSelect = new SqlCommand(SqlUsuarioWhereToken, adoConn);
                adoCmdSelect.Parameters.AddWithValue("token", token);

                var dr = adoCmdSelect.ExecuteReader();

                if (!dr.HasRows)
                {
                    dr.Close();

                    var idCmd = new SqlCommand("SELECT max(Id) from USUARIO", adoConn);
                    var driD = idCmd.ExecuteReader();
                    driD.Read();
                    var cod = System.Convert.ToInt32(driD[0]) + 1;
                    usuario.Id = cod;
                    driD.Close();

                    var adoCmdInsert = new SqlCommand(SqlInsertUsuario, adoConn);

                    adoCmdInsert.Parameters.AddWithValue("id", cod);
                    adoCmdInsert.Parameters.AddWithValue("facebooktoken", token);
                    adoCmdInsert.Parameters.AddWithValue("nome", nome);
                    adoCmdInsert.Parameters.AddWithValue("cidade", cidade);
                    adoCmdInsert.Parameters.AddWithValue("estado", cidade);
                    adoCmdInsert.Parameters.AddWithValue("idade", idade);
                    adoCmdInsert.ExecuteNonQuery();
                }
                else
                {
                    dr.Read();
                    usuario.Id = Convert.ToInt32(dr["Id"]);
                    usuario.Nome = dr["Nome"].ToString();
                    usuario.FacebookToken = dr["FacebookToken"].ToString();
                    usuario.Cidade = dr["Cidade"].ToString();
                    usuario.Estado = dr["Estado"].ToString();
                    usuario.Idade = Convert.ToInt32(dr["Idade"]);
                    dr.Close();
                }
            }           
            finally
            {
                adoConn.Close();
            }
            

            return usuario;
        }


        [HttpPost]
        public IEnumerable<Badbadgets> Ocorrencia(string token, int tipo, int subtipo, int status, string linha)
        {
            var adoConn = new SqlConnection(ConnString);
            adoConn.Open();

            IEnumerable<Badbadgets> bad;

            try
            {
                var idCmd = new SqlCommand("SELECT max(Id) from OCORRENCIAUSUARIO", adoConn);
                var driD = idCmd.ExecuteReader();
                driD.Read();
                var cod = System.Convert.ToInt32(driD[0]) + 1;
                driD.Close();

                var adoCmdInsert = new SqlCommand(SqlInsertOcorrencia, adoConn);

                adoCmdInsert.Parameters.AddWithValue("id", cod);
                adoCmdInsert.Parameters.AddWithValue("token", token);
                adoCmdInsert.Parameters.AddWithValue("tipo", tipo);
                adoCmdInsert.Parameters.AddWithValue("subtipo", subtipo);
                adoCmdInsert.Parameters.AddWithValue("data", DateTime.Now);
                adoCmdInsert.Parameters.AddWithValue("status", status);
                adoCmdInsert.Parameters.AddWithValue("linha", linha);   
                
                adoCmdInsert.ExecuteNonQuery();

                bad = RetornaBadgets(adoConn, token, status);

            }           
            finally
            {
                adoConn.Close();
            }

            return bad;
        }

        private IEnumerable<Badbadgets> RetornaBadgets(SqlConnection adoConn, string token, int status)
        {
            var lst = new List<Badbadgets>();

            if (ReiDoOnibus(adoConn, token) != null)
            {
                lst.Add(ReiDoOnibus(adoConn, token));
            }

            if (PrimeiraCritica(adoConn, token) != null && status == 1)
            {
                lst.Add(PrimeiraCritica(adoConn, token));
            }

            if (PrimeiroElogio(adoConn, token) != null && status == 0)
            {
                lst.Add(PrimeiroElogio(adoConn, token));
            }

            if (OCritico(adoConn, token) != null && status == 1)
            {
                lst.Add(OCritico(adoConn, token));
            }

            if (PassageiroFeliz(adoConn, token) != null && status == 0)
            {
                lst.Add(PassageiroFeliz(adoConn, token));
            }

            return lst.Count > 0 ? lst : null;
        }

        private static Badbadgets PrimeiraCritica(SqlConnection adoConn, string token)
        {
            var command = new SqlCommand("SELECT COUNT(*) FROM OCORRENCIAUSUARIO WHERE Status = 1 and FACEBOOKTOKEN = @token;", adoConn);

            command.Parameters.Add(new SqlParameter() { DbType = DbType.String, Value = token, ParameterName = "token" });

            var count = System.Convert.ToInt32(command.ExecuteScalar());

            if (count == 1)
            {
                var badGet = new Badbadgets
                {
                    Id = 0,
                    Descricao = "Primeira Crítica. SP Trans Agradece !"
                };
                return badGet;
            }

            return null;
        }

        private static Badbadgets PrimeiroElogio(SqlConnection adoConn, string token)
        {
            var command = new SqlCommand("SELECT COUNT(*) FROM OCORRENCIAUSUARIO WHERE Status = 0 and FACEBOOKTOKEN = @token;", adoConn);

            command.Parameters.Add(new SqlParameter() { DbType = DbType.String, Value = token, ParameterName = "token" });

            var count = System.Convert.ToInt32(command.ExecuteScalar());

            if (count == 1)
            {
                var badGet = new Badbadgets
                {
                    Id = 1,
                    Descricao = "Primeiro Elogio. SP Trans Agradece !"
                };
                return badGet;
            }

            return null;
        }

        private static Badbadgets ReiDoOnibus(SqlConnection adoConn, string token)
        {
            var command = new SqlCommand("SELECT COUNT(*) FROM OCORRENCIAUSUARIO WHERE FACEBOOKTOKEN = @token;", adoConn);

            //command.Parameters.AddWithValue("token", token);

            command.Parameters.Add(new SqlParameter() {DbType = DbType.String, Value = token, ParameterName = "token"});

            var count = System.Convert.ToInt32(command.ExecuteScalar());
            
            if (count == 4)
            {
                var badGet = new Badbadgets
                                 {
                                     Id = 2,
                                     Descricao = "Rei do ônibus. SP Trans Agradece !"
                                 };
                return badGet;
            }

            return null;
        }

        private static Badbadgets OCritico(SqlConnection adoConn, string token)
        {
            var command = new SqlCommand("select COUNT(*) from OcorrenciaUsuario where Status = 1 and FACEBOOKTOKEN = @token;", adoConn);

            command.Parameters.AddWithValue("token", token);
            var count = System.Convert.ToInt32(command.ExecuteScalar());

            if (count > 6)
            {
                var badGet = new Badbadgets
                {
                    Id = 3,
                    Descricao = "O Crítico. SP Trans Agradece !"
                };
                return badGet;
            }

            return null;
        }

        private static Badbadgets PassageiroFeliz(SqlConnection adoConn, string token)
        {
            var command = new SqlCommand("select COUNT(*) from OcorrenciaUsuario where Status = 0 and FACEBOOKTOKEN = @token;", adoConn);

            command.Parameters.AddWithValue("token", token);
            var count = System.Convert.ToInt32(command.ExecuteScalar());

            if (count > 5)
            {
                var badGet = new Badbadgets
                {
                    Id = 4,
                    Descricao = "Passageiro Feliz. SP Trans Agradece !"
                };
                return badGet;
            }

            return null;
        }
         
    }
}
