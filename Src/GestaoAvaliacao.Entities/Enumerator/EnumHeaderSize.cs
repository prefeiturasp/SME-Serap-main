using System.ComponentModel;

namespace GestaoAvaliacao.Entities.Enumerator
{
    public enum EnumSize
	{
		[Description("Médio (padrão)")]
		Default = 1,
		[Description("Pequeno")]
		Small = 2,
		[Description("Grande")]
		Big = 3
	}
}
