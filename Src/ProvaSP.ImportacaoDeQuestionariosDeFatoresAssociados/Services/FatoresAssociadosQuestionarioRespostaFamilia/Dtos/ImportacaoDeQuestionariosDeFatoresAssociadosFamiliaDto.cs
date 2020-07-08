using ImportacaoDeQuestionariosSME.Services.Abstractions;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Dtos
{
    public class ImportacaoDeQuestionariosDeFatoresAssociadosFamiliaDto : BaseDto
    {
        public ImportacaoDeQuestionariosDeFatoresAssociadosFamiliaDto()
            : base()
        {
        }

        public string Edicao { get; set; }
        public string CaminhoDaPlanilhaQuesitonarios { get; set; }
    }
}