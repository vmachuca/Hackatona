using System;

namespace WebApi.Models
{
    public class Ocorrencia
    {
        public int Id { get; set; }

        public string Avaliacao { get; set; }

        public Linha Linha { get; set; }

        public DateTime Data { get; set; }

        public string Tipo { get; set; }

        public string Subtipo { get; set; }

        public string Sentido { get; set; }
    }
}