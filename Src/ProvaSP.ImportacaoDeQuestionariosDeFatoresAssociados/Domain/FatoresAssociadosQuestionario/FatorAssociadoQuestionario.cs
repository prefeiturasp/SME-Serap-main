using ImportacaoDeQuestionariosSME.Domain.Abstractions;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionario
{
    public class FatorAssociadoQuestionario : Entity
    {
        public FatorAssociadoQuestionario()
            : base()
        {
        }

        public int FatorAssociadoQuestionarioId { get; set; }
        public string Edicao { get; set; }
        public string Nome { get; set; }
    }
}