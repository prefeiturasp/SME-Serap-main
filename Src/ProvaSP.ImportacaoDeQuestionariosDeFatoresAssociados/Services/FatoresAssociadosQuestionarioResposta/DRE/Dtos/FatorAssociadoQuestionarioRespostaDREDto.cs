namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.DRE.Dtos
{
    public class FatorAssociadoQuestionarioRespostaDREDto
    {
        public string UadSigla { get; set; }
        public int AnoEscolar { get; set; }
        public int Questao { get; set; }
        public string Resposta { get; set; }
        public decimal Quantidade { get; set; }
    }
}