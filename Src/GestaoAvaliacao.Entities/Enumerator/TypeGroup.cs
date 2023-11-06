using System.ComponentModel;

namespace GestaoAvaliacao.Entities.Enumerator
{
    public enum EnumTypeGroup
    {
        [Description("UE")]
        UE = 1,
        [Description("DRE")]
        DRE = 2,
        [Description("SME")]
        SME = 3,
        [Description("GERAL")]
        GERAL = 4,
        [Description("PÚBLICO")]
        PUBLICO = 5
    }
}
