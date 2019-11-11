using System.Collections.Generic;

namespace ProvaSP.Model.Entidades
{
    public class FatorAssociado
    {
        public Constructo Constructo { get; set; }
        public List<FatorAssociadoResultado> Resultados { get; set; }
        public List<Variavel> Variaveis { get; set; }
    }

    public class FatorAssociadoResultado
    {
        public int AreaConhecimentoId { get; set; }
        public string AreaConhecimentoNome { get; set; }
        public string Pontos { get; set; }
    }

    public class Variavel
    {
        public string VariavelId { get; set; }
        public string VariavelDescricao { get; set; }
    }

    public class VariavelItem
    {
        public string VariavelDescricao { get; set; }
        public string ItemDescricao { get; set; }
        public string Valor { get; set; }
        public string ValorSuperior { get; set; }
    }
}
