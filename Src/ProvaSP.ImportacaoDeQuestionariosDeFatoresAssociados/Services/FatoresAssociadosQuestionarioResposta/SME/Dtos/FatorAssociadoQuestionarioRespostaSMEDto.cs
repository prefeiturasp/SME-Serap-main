namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.SME.Dtos
{
    public class FatorAssociadoQuestionarioRespostaSMEDto
    {
        public int AnoEscolar { get; set; }
        public int Questao { get; set; }
        public string Resposta { get; set; }
        public decimal Quantidade { get; set; }
    }
}