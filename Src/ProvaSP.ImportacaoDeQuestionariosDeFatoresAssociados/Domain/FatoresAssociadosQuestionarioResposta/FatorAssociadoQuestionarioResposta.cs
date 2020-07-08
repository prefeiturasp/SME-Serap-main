using ImportacaoDeQuestionariosSME.Domain.Abstractions;

namespace ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta
{
    public abstract class FatorAssociadoQuestionarioResposta : Entity
    {
        public FatorAssociadoQuestionarioResposta()
            : base()
        {
        }

        public int AnoEscolar { get; set; }
        public string Edicao { get; set; }
        public int CicloId { get; set; }
        public int FatorAssociadoQuestionarioId { get; set; }
        public string VariavelId { get; set; }
        public int ItemId { get; set; }
        public string VariavelDescricao { get; set; }
        public string ItemDescricao { get; set; }
        public decimal Valor { get; set; }
        public int Id { get; set; }
        public decimal QuantidadeDeRespostas { get; set; }
    }
}