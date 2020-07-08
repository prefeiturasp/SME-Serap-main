using ImportacaoDeQuestionariosSME.Domain.Abstractions;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.Escolas
{
    public class FatorAssociadoQuestionarioRespostaEscolaConstructo : Entity
    {
        public FatorAssociadoQuestionarioRespostaEscolaConstructo()
            : base()
        {
        }

        public int FatorAssociadoQuestionarioRespostaEscolaId { get; set; }
        public int ConstructoId { get; set; }
    }
}