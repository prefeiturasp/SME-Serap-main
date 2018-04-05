using System.ComponentModel;

namespace GestaoAvaliacao.Entities.Enumerator
{
    public enum EnumState
    {
        [Description("Ativo")]
        ativo = 1,
        [Description("Inativo")]
        inativo = 2,
        [Description("Excluído")]
        excluido = 3
    }
}
