using GestaoAvaliacao.Util;
using System.Collections.Generic;
using System.Linq;

namespace AvaliaMais.FolhaRespostas.Application.Adapters
{
    public static class PaginacaoHelper<TEntity> where TEntity : class
	{
		public static IEnumerable<TEntity> PaginarResultado(ref Pager pagina, IEnumerable<TEntity> obj)
		{
			pagina.SetTotalItens(obj.Count());
			pagina.SetTotalPages(pagina.RecordsCount / pagina.PageSize);
			return obj.Skip(pagina.CurrentPage * pagina.PageSize).Take(pagina.PageSize).ToList();
		}
	}
}
