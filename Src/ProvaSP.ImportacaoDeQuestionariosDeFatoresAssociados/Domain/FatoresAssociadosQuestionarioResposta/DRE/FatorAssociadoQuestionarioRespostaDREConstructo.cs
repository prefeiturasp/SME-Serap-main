using ImportacaoDeQuestionariosSME.Domain.Abstractions;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.DRE
{
    public class FatorAssociadoQuestionarioRespostaDREConstructo : Entity
    {
        public FatorAssociadoQuestionarioRespostaDREConstructo()
            : base()
        {
        }

        public int FatorAssociadoQuestionarioRespostaDREId { get; set; }
        public int ConstructoId { get; set; }
    }
}