namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Escolas.Dtos
{
    public class FatorAssociadoQuestionarioRespostaEscolaDto
    {
        public string EscCodigo { get; set; }
        public string UadSigla { get; set; }
        public int AnoEscolar { get; set; }
        public int Questao { get; set; }
        public string Resposta { get; set; }
        public decimal Quantidade { get; set; }
    }
}