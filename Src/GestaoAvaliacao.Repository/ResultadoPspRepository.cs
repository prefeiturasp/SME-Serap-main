using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class ResultadoPspRepository : ConnectionReadOnly, IResultadoPspRepository
    {
        public ResultadoPspRepository()
        {

        }

        public IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager, string codigoOuNomeArquivo)
        {

            using (IDbConnection cn = Connection)
            {

                string and = "";
                if (!string.IsNullOrEmpty(codigoOuNomeArquivo))
                {
                    int codigo = 0;
                    if (int.TryParse(codigoOuNomeArquivo, out codigo))
                        and = $" and Id = {codigoOuNomeArquivo}";
                    and = $" and NomeOriginalArquivo like '%{codigoOuNomeArquivo}%'";
                }

                var sql = new StringBuilder($@"

                    			WITH Resultado AS (
								select 
								Id
								,FileId
								,CodigoTipoResultado
								,NomeArquivo
								,NomeOriginalArquivo
								,CreateDate
								,UpdateDate
								,[State]
								,ROW_NUMBER() OVER (ORDER BY CreateDate DESC) AS RowNumber
								from ArquivoResultadoPsp WITH (NOLOCK)
								where 1=1
                                {and}
								)
								select Id,FileId,CodigoTipoResultado,NomeArquivo,NomeOriginalArquivo,CreateDate,UpdateDate,[State] 
                                from Resultado
								WHERE RowNumber > ( @pageSize * @page ) 
								AND RowNumber <= ( ( @page + 1 ) * @pageSize ) 

								select count(Id) total
								from ArquivoResultadoPsp WITH (NOLOCK)
								where 1=1
                                {and}
                ");

                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), 
                    new { pageSize = pager.PageSize, page = pager.CurrentPage });

                var result = query.Read<ArquivoResultadoPsp>();
                var count = query.Read<int>().FirstOrDefault();

                pager.SetTotalItens(count);
                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));                

                return result;
            }

        }

        public ArquivoResultadoPsp InserirNovo(ArquivoResultadoPsp arquivoResultado)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                GestaoAvaliacaoContext.ArquivoResultadoPsp.Add(arquivoResultado);
                GestaoAvaliacaoContext.SaveChanges();

                return arquivoResultado;
            }
        }

    }
}
