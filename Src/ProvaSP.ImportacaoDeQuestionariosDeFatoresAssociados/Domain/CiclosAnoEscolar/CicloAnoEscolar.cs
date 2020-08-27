using ImportacaoDeQuestionariosSME.Domain.Abstractions;

namespace ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar
{
    public class CicloAnoEscolar : Entity
    {
        public CicloAnoEscolar()
            : base()
        {
        }

        public int CicloId { get; set; }
        public int AnoEscolar { get; set; }
    }
}