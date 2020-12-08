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

    public enum StudentTestSentSituation : short
    {
        [Description("Pendente")]
        Pending = 1,

        [Description("Em processamento")]
        InProcess = 2,

        [Description("Finalizada")]
        Done = 3
    }

    public enum EnumSYS_Visao
    {
        [Description("Administração")]
        Administracao = 1,

        [Description("Gestão")]
        Gestao = 2,

        [Description("Unidade Administrativa")]
        UnidadeAdministrativa = 3,

        [Description("Individual")]
        Individual = 4
    }

    public enum StatusProvaEletronica : byte
    {
        [Description("Não iniciada")]
        NaoIniciada = 1,

        [Description("Em andamento")]
        EmAndamento = 2,

        [Description("Finalizada")]
        Finalizada = 3
    }

    public enum EnumStatusCorrection : byte
    {
        [Description("Não iniciada")]
        Pending = 0,

        [Description("Em andamento")]
        Processing = 1,

        [Description("Concluída")]
        Success = 2,

        [Description("Parcialmente concluída")]
        PartialSuccess = 3,

        [Description("Processando turma")]
        ProcessingSection = 4
    }
}