using Newtonsoft.Json;

namespace GestaoAvaliacao.FGVIntegration.Models
{
    public class ProfessorTurma : BaseFGVObject
    {

        public string EmailDoProfessor { get; set; }

        public string CodigoDaTurma { get; set; }


        /// <summary>
        /// Valores válidos: <see cref="Enums.Disciplina"/>
        /// </summary>
        public string Disciplina { get; set; }

    }
}
