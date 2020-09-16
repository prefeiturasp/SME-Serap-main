using System.ComponentModel;

namespace GestaoAvaliacao.Entities.Enumerator
{
    public enum EnumState
    {
        [Description("Não definido")]
        naoDefinido = 0,
        [Description("Ativo")]
        ativo = 1,
        [Description("Inativo")]
        inativo = 2,
        [Description("Excluído")]
        excluido = 3
    }
}
