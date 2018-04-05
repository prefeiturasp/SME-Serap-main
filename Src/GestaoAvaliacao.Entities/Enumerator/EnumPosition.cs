using System.ComponentModel;

namespace GestaoAvaliacao.Entities.Enumerator
{
    public enum EnumPosition
	{
		[Description("Esquerda")]
		Left = 1,
		[Description("Centro")]
		Center = 2,
		[Description("Direita")]
		Right = 3
	}
}
