using ImportacaoDeQuestionariosSME.Domain.Enums;
using ImportacaoDeQuestionariosSME.Services.Abstractions;

namespace ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Dtos
{
    public class CaracterizacaoFamiliasEscolasQuestionarioRespostaDto : BaseDto
    {
        public CaracterizacaoFamiliasEscolasQuestionarioRespostaDto(string edicao, int fatorAssociadoQuestionarioId,
            TipoQuestionarioEnum tipoQuestionario, string caminhoArquivo)
        {
            Edicao = edicao;
            FatorAssociadoQuestionarioId = fatorAssociadoQuestionarioId;
            TipoQuestionario = tipoQuestionario;
            CaminhoArquivo = caminhoArquivo;
        }

        public CaracterizacaoFamiliasEscolasQuestionarioRespostaDto() : this (string.Empty, 0, TipoQuestionarioEnum.Nenhum, string.Empty)
        {
        }

        public string Edicao { get; }
        public int FatorAssociadoQuestionarioId { get; }
        public TipoQuestionarioEnum TipoQuestionario { get; }
        public string CaminhoArquivo { get; }
    }
}
