using ImportacaoDeQuestionariosSME.Domain.Abstractions;

namespace ImportacaoDeQuestionariosSME.Domain.Escolas
{
    public class Escola : Entity
    {
        public Escola()
            : base()
        {
        }

        public string EscCodigo { get; set; }
        public string UadCodigo { get; set; }
    }
}