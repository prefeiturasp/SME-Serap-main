using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
