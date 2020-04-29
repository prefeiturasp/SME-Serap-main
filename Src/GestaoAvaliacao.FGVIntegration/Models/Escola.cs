using Newtonsoft.Json;

namespace GestaoAvaliacao.FGVIntegration.Models
{
    public class Escola : BaseFGVObject
    {

        public string Email { get; set; }

        public string NomeDaEscola { get; set; }

        public string NomeDoResponsavel { get; set; }

        public string CargoDoResponsavel { get; set; }

        /// <summary>
        /// Apenas números
        /// </summary>
        public string CPFDoResponsavel { get; set; }

        /// <summary>
        /// PES_Pessoa.pes_id
        /// </summary>
        [JsonIgnore]
        public string IdPessoaResponsavel { get; set; }

        /// <summary>
        /// Valores válidos: <see cref="Enums.UnidadeFederativa"/>
        /// </summary>
        public string UF { get; set; }

        /// <summary>
        /// Valores válidos: <see cref="Enums.CidadeIBGE"/>
        /// </summary>
        public int Cidade { get; set; }

        /// <summary>
        /// Apenas números
        /// </summary>
        public string CNPJ { get; set; }

        public string WebSite { get; set; }

        /// <summary>
        /// Valores válidos: <see cref="Enums.TipoEscola"/>
        /// </summary>
        public int Tipo { get; set; }

    }
}
