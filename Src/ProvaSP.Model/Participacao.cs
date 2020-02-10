using System.Collections.Generic;

namespace ProvaSP.Model.Entidades
{
    public class Participacao
    {
        public string Sigla { get; set; }
        public string Titulo { get; set; }
        public string AnoEscolar { get; set; }
        public int TotalPrevistoGeral { get; set; }
        public int TotalPresenteGeral { get; set; }
        public float PercentualParticipacaoGeral { get; set; }

        public int AreaConhecimentoID { get; set; }
        public int TotalPrevistoAreaConhecimento { get; set; }
        public int TotalPresenteAreaConhecimento { get; set; }
        public float PercentualParticipacaoAreaConhecimento { get; set; }
        public List<Participacao> Itens { get; set; }
    }
}
