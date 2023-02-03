using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class TipoResultadoPsp : EntityBase
    {
        public TipoResultadoPsp()
        {

        }

        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string NomeTabelaProvaSp { get; set; }
        public string ModeloArquivo { get; set; }

    }
}
