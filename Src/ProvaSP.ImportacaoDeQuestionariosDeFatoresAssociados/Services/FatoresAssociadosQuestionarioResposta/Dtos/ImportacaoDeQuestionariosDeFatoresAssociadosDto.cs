using ImportacaoDeQuestionariosSME.Services.Abstractions;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos
{
    public class ImportacaoDeQuestionariosDeFatoresAssociadosDto : BaseDto
    {
        public ImportacaoDeQuestionariosDeFatoresAssociadosDto()
            : base()
        {
        }

        public string Edicao { get; set; }
        public string CaminhoDaPlanilha { get; set; }
        public int QuestionarioId { get; set; }
    }
}