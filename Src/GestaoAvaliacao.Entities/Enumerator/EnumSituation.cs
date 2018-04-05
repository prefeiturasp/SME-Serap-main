using System.ComponentModel;


namespace GestaoAvaliacao.Entities.Enumerator
{
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
