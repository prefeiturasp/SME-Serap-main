using Newtonsoft.Json;

namespace GestaoAvaliacao.FGVIntegration.Models
{
    public class Turma : BaseFGVObject
    {

        public string EmailDaEscola { get; set; }

        /// <summary>
        /// Valores válidos: <see cref="Enums.Serie"/>
        /// </summary>
        public int Serie { get; set; }

        /// <summary>
        /// Usado como identificador único da Turma
        /// </summary>
        public string CodigoDaTurma { get; set; }

        public string NomeDaTurma { get; set; }

        /// <summary>
        /// Valores válidos: <see cref="Enums.Turno"/>
        /// </summary>
        public string Turno { get; set; }

    }
}
