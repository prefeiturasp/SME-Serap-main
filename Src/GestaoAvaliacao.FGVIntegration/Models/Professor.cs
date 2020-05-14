using Newtonsoft.Json;

namespace GestaoAvaliacao.FGVIntegration.Models
{
    public class Professor : BaseFGVObject, IIdentificadorEscola, IPessoaNomeSobrenome
    {

        public string EmailDaEscola { get; set; }

        [JsonIgnore]
        public string CodigoDaEscola { get; set; }

        public string EmailDoProfessor { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        /// <summary>
        /// Apenas dígitos
        /// </summary>
        public string CPF { get; set; }

        /// <summary>
        /// Valores válidos: <see cref="Enums.Sexo"/>
        /// </summary>
        public string Sexo { get; set; }

        /// <summary>
        /// Formato: DD/MM/YYYY
        /// </summary>
        public string DataNascimento { get; set; }

    }
}
