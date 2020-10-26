using System.ComponentModel;

namespace GestaoAvaliacao.Entities.Enumerator
{
    public enum Sessionituation
    {
        [Description("Não iniciada")]
        NotStarted = 1,

        [Description("Iniciada")]
        Started = 2,

        [Description("Finalizada")]
        Done = 3,

        [Description("Finalizada por perda de conexão")]
        DoneByLostConnection = 4,
    }
}