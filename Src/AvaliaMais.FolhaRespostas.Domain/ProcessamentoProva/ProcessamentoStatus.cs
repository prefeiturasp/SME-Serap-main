namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva
{
    public class ProcessamentoStatus
	{
		public int Sucesso { get; set; }
		public int Ausente { get; set; }
		public int Erro { get; set; }
		public int Pendente { get; set; }
		public int Conferir { get; set; }
	}
}
