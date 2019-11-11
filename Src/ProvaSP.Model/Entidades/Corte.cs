using System.Collections.Generic;

namespace ProvaSP.Model.Entidades
{
    public class Corte
    {
        public int CorteId { get; set; }
        public string Nome { get; set; }
        public List<SequenciaDidatica> SequenciasDidaticas { get; set; }
    }
}
