using ImportacaoDeQuestionariosSME.Services.Abstractions;

namespace ImportacaoDeQuestionariosSME.Services.Constructos.Dtos
{
    public class ImportacaoDeConstructosDto : BaseDto
    {
        public ImportacaoDeConstructosDto()
            : base()
        {
        }

        public string Edicao { get; set; }
    }
}