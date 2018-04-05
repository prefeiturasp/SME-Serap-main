using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Util
{
    public class Pager
    {
        public Pager()
        {
            //Valor padrão do page size 
            PageSize = 10;
        }

        public int RecordsCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public int SetTotalPages(int total)
        {
            TotalPages = total;

            return TotalPages;
        }

        public int SetTotalItens(int total)
        {
            RecordsCount = total;

            return RecordsCount;
        }

        public IEnumerable<TModel> Paginate<TModel>(IQueryable<TModel> source)
        {
            // Atualiza o total de páginas
            SetTotalItens(source.Count());
            SetTotalPages((int)Math.Ceiling(source.Count() / (double)PageSize));

            return source.Skip(CurrentPage * PageSize).Take(PageSize).ToList();
        }
    }


}
