using System.Collections.Generic;

namespace WebApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }        

        public string FacebookToken { get; set; }

        public string Nome { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public int Idade { get; set; }

        public IEnumerable<Badbadgets> Badbadgets { get; set; }
 
    }
}