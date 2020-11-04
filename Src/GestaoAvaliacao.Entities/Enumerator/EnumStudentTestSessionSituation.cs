using System.ComponentModel;

namespace GestaoAvaliacao.Entities.Enumerator
{
    public enum AccoplishmentoSituation : short
    {
        [Description("Iniciada")]
        Started = 1,

        [Description("Finalizada")]
        Done = 2,

        [Description("Finalizada com sessões pendentes")]
        DoneWithIncompleteSessions = 3,
    }

    public enum Sessionituation : short
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