using System.Collections.Generic;

namespace ProvaSP.Model.Entidades
{
    public class Participacao
    {
        public string Sigla { get; set; }
        public string Titulo { get; set; }
        public string AnoEscolar { get; set; }
        public int TotalPrevisto { get; set; }
        public int TotalPresente { get; set; }
        public float PercentualParticipacao { get; set; }
        public List<Participacao> Itens { get; set; }
    }
}
