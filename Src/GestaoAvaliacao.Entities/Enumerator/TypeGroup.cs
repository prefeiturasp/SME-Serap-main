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
        [Description("Geral")]
        GERAL = 4,
        [Description("Público Geral")]
        PUBLICO = 5
    }
}
