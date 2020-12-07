using System.ComponentModel;

namespace GestaoAvaliacao.Worker.Domain.Enums
{
    public enum EnumAdherenceEntity : byte
    {
        [Description("Turma")]
        Section = 1,

        [Description("Escola")]
        School = 2,

        [Description("Aluno")]
        Student = 3,
    }

    public enum EnumAdherenceSelection : byte
    {
        [Description("Selecionado")]
        Selected = 1,

        [Description("Parcial")]
        Partial = 2,

        [Description("Não Selecionado")]
        NotSelected = 3,

        [Description("Bloqueado")]
        Blocked = 4
    }

    public enum EnumSituation
    {
        [Description("Não anulado")]
        NotRevoked = 1,

        [Description("Aguardando")]
        Waiting = 2,

        [Description("Anulado Prova")]
        RevokedTest = 3,

        [Description("Anulado Prova e Item")]
        Revoked = 4,

        [Description("Recusado")]
        Refused = 5
    }
}