using ImportacaoDeQuestionariosSME.Services.Abstractions;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionario.Dtos
{
    public class ImportacaoDeFatorAssociadoQuestionarioDto : BaseDto
    {
        public ImportacaoDeFatorAssociadoQuestionarioDto()
            : base()
        {
        }

        public string Edicao { get; set; }
    }
}