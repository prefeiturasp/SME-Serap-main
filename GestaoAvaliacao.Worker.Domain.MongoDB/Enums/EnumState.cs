using System.ComponentModel;

namespace GestaoAvaliacao.Worker.Domain.MongoDB.Enums
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