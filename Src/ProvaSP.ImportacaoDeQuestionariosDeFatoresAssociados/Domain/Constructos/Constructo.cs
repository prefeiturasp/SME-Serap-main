using ImportacaoDeQuestionariosSME.Domain.Abstractions;

namespace ImportacaoDeQuestionariosSME.Domain.Constructos
{
    public class Constructo : Entity
    {
        public Constructo()
            : base()
        {
        }

        public int ConstructoId { get; set; }
        public string Nome { get; set; }
        public string Edicao { get; set; }
        public int CicloId { get; set; }
        public int FatorAssociadoQuestionarioId { get; set; }
        public string Referencia { get; set; }
        public int AnoEscolar { get; set; }
    }
}