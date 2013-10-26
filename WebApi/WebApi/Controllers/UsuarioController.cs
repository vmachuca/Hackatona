using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using WebApi.Attributes;
using WebApi.Models;

namespace WebApi.Controllers
{
    [AllowCrossSiteJson]
    public class UsuarioController : ApiController
    {
        const string ConnString = "Data Source=ktqkvifuvn.database.windows.net;Initial Catalog=euandodeonibus;Persist Security Info=True;User ID=euandodeonibus;Password=abc1234#";

        [HttpGet]
        [AllowCrossSiteJson]
        public Usuario BuscaId(string id)
        {
            return Busca();
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public Usuario BuscaToken(string token)
        {
            return Busca();
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

            var sql = "SELECT * FROM USUARIO WHERE FacebookToken = @facebooktoken";
            var adoCmdSelect  = new SqlCommand(sql, adoConn);
            adoCmdSelect.Parameters.AddWithValue("facebooktoken", token);

            var dr = adoCmdSelect.ExecuteReader();

            if (!dr.HasRows)
            {
                // novo command
                sql = "INSERT INTO USUARIO ( Id, FacebookToken, Nome, Cidade, Estado, Idade ) values ( @id, @facebooktoken, @nome, @cidade, @estado, @idade )";
                var adoCmd = new SqlCommand(sql, adoConn);

                adoCmd.Parameters.AddWithValue("id", 1);
                adoCmd.Parameters.AddWithValue("facebooktoken", token);
                adoCmd.Parameters.AddWithValue("nome", nome);
                adoCmd.Parameters.AddWithValue("cidade", cidade);
                adoCmd.Parameters.AddWithValue("estado", cidade);
                adoCmd.Parameters.AddWithValue("idade", idade);

                adoCmd.ExecuteNonQuery();                
            }
            else
            {
                usuario.Id = System.Convert.ToInt32(dr["Id"]);
                usuario.Nome = dr["Nome"].ToString();
                usuario.FacebookToken = dr["FacebookToken"].ToString();
                usuario.Cidade = dr["Cidade"].ToString();
                usuario.Estado = dr["Estado"].ToString();
                usuario.Idade = System.Convert.ToInt32(dr["Idade"]);
            }

            return usuario;
        }


        private static Usuario Busca()
        {
            return
                new Usuario()
                    {
                        Id = 1,
                        Nome = "Djonatas",
                        Cidade = "São josé dos campos",
                        Estado = "SP",
                        FacebookToken = "01236214fdsafsaf2",
                        Idade = 27
                    };
        }
    }
}
