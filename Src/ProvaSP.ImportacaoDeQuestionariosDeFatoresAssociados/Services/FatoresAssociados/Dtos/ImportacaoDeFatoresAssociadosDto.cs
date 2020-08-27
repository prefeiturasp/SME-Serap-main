using ImportacaoDeQuestionariosSME.Services.Abstractions;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociados.Dtos
{
    public class ImportacaoDeFatoresAssociadosDto : BaseDto
    {
        public ImportacaoDeFatoresAssociadosDto()
            : base()
        {
        }

        public string Edicao { get; set; }
        public string CaminhoDaPlanilha { get; set; }
    }
}