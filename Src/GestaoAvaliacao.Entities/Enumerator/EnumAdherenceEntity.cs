using System.ComponentModel;

namespace GestaoAvaliacao.Entities.Enumerator
{
    public enum EnumAdherenceEntity : byte
	{
		[Description("Turma")]
		Section = 1,
		[Description("Escola")]
		School = 2,
        [Description("Aluno")]
        Student = 3,
    }
}
