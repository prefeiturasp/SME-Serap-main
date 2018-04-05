using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class PaginatorData<T>
	{
		public Pager Pager { get; set; }
		public IEnumerable<T> Data { get; set; }
	}
}
