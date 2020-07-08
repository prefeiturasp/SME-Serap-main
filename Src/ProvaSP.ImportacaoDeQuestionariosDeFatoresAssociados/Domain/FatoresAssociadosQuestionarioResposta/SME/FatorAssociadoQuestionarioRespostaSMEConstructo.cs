using ImportacaoDeQuestionariosSME.Domain.Abstractions;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.SME
{
    public class FatorAssociadoQuestionarioRespostaSMEConstructo : Entity
    {
        public FatorAssociadoQuestionarioRespostaSMEConstructo()
            : base()
        {
        }

        public int FatorAssociadoQuestionarioRespostaSMEId { get; set; }
        public int ConstructoId { get; set; }
    }
}