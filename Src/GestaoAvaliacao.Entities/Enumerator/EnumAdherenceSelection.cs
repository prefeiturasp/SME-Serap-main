using System.ComponentModel;

namespace GestaoAvaliacao.Entities.Enumerator
{
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
}
