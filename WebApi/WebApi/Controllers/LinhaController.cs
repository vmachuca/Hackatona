using System.Collections.Generic;
using System.Web.Http;
using WebApi.Attributes;
using WebApi.Models;

namespace WebApi.Controllers
{
    [AllowCrossSiteJson]
    public class LinhaController : ApiController
    {
        [HttpGet]
        [AllowCrossSiteJson]
        public string Busca(string codigo)
        {
            return "Linha ABC XYZ " + codigo;        
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public IEnumerable<Linha> LocalizaLinhas(string lat, string lon)
        {
            return new List<Linha>(){ new Linha()
                                          {
                                              Codigo = "COD001",
                                              Descricao = "Linha Azul"                                             
                                          },
                                          new Linha()
                                          {
                                              Codigo = "COD002",
                                              Descricao = "Linha Azul 1"                                             
                                          },
                                          new Linha()
                                          {
                                              Codigo = "COD003",
                                              Descricao = "Linha Azul 2"                                             
                                          }
                                    };
        }
    }
}
