namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva
{
    public abstract class Component
	{
		public string Nome { get; private set; }
		public object Codigo { get; private set; }
		public AlunoStatus AlunoStatus { get; private set; }
		public ProcessamentoStatus ProcessamentoStatus { get; private set; }

		public abstract void Add(Component p);
		public abstract void Remove(Component p);

		public void DefinirAlunoStatus(AlunoStatus alunoStatus)
		{
			AlunoStatus = alunoStatus;
		}

		public void DefinirProcessamentoStatus(ProcessamentoStatus processamentoStatus)
		{
			ProcessamentoStatus = processamentoStatus;
		}

		public void DefinirNome(string nome)
		{
			Nome = nome;
		}

		public void DefinirCodigo(object codigo)
		{
			Codigo = codigo;
		}
	}
}
