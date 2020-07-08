using ImportacaoDeQuestionariosSME.Domain.Abstractions;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociados
{
    public class FatorAssociado : Entity
    {
        public FatorAssociado()
            : base()
        {
        }

        public int ConstructoId { get; set; }
        public int AreaConhecimentoId { get; set; }
        public string Pontos { get; set; }
    }
}