using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using System.Collections.Generic;
using System.Linq;

namespace AvaliaMais.FolhaRespostas.Data.MongoDB.Helpers
{
    public static class QuantidadeHelper<TEntity> where TEntity : DRE
	{
		public static Quantidade QuantidadeTotal(IEnumerable<TEntity> obj)
		{
			var quantidade = new Quantidade();
			quantidade.Aderidos = obj.Sum(a => a.AlunoStatus.Aderidos);
			quantidade.Identificados = obj.Sum(a => a.AlunoStatus.Identificados);
			quantidade.Sucesso = obj.Sum(a => a.ProcessamentoStatus.Sucesso);
			quantidade.Conferir = obj.Sum(a => a.ProcessamentoStatus.Conferir);
			quantidade.Ausente = obj.Sum(a => a.ProcessamentoStatus.Ausente);
			quantidade.Erro = obj.Sum(a => a.ProcessamentoStatus.Erro);
			quantidade.Pendente = obj.Sum(a => a.ProcessamentoStatus.Pendente);

			return quantidade;
		}
	}
}
