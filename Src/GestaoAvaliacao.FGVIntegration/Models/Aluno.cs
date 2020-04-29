using Newtonsoft.Json;

namespace GestaoAvaliacao.FGVIntegration.Models
{
    public class Aluno : BaseFGVObject
    {

        public string EmailDaEscola { get; set; }

        public string EmailDoAluno { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        /// <summary>
        /// Apenas dígitos
        /// </summary>
        public string CPF { get; set; }

        public string RG { get; set; }

        /// <summary>
        /// Valores válidos: <see cref="Enums.Sexo"/>
        /// </summary>
        public string Sexo { get; set; }

        /// <summary>
        /// Formato: DD/MM/YYYY
        /// </summary>
        public string DataNascimento { get; set; }

        public string CodigoDaTurma { get; set; }

    }
}
