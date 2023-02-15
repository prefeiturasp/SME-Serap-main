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

        public IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager)
        {

            using (IDbConnection cn = Connection)
            {

                var sql = new StringBuilder($@"
                    			WITH Resultado AS (
								select 
								 Arp.Id
								,Arp.CodigoTipoResultado
								,Arp.NomeArquivo
								,Arp.NomeOriginalArquivo
								,Arp.CreateDate
								,Arp.UpdateDate
                                ,Trp.Nome as Tipo
								,Arp.State
								,ROW_NUMBER() OVER (ORDER BY Arp.CreateDate DESC) AS RowNumber
								from ArquivoResultadoPsp Arp WITH (NOLOCK)
                                inner join TipoResultadoPsp Trp WITH (NOLOCK) on Trp.Codigo = Arp.CodigoTipoResultado
								)
								select Id,CodigoTipoResultado,NomeArquivo,NomeOriginalArquivo,CreateDate,UpdateDate,Tipo,[State] 
                                from Resultado
								WHERE RowNumber > ( @pageSize * @page ) 
								AND RowNumber <= ( ( @page + 1 ) * @pageSize ) 
								select count(Arp.Id) total
								from ArquivoResultadoPsp Arp WITH (NOLOCK)								
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

        public IEnumerable<ArquivoResultadoPsp> ObterImportacoes(string codigoOuNomeArquivoOuTipo)
        {

            using (IDbConnection cn = Connection)
            {
                string and = "";

                if (!string.IsNullOrEmpty(codigoOuNomeArquivoOuTipo))
                {
                    int codigo = 0;

                    if (int.TryParse(codigoOuNomeArquivoOuTipo, out codigo))
                        and = $" and Arp.Id = {codigoOuNomeArquivoOuTipo} ";
                    else
                        and = $" and ((Arp.NomeOriginalArquivo like '%{codigoOuNomeArquivoOuTipo}%') or (Trp.Nome like '%{codigoOuNomeArquivoOuTipo}%')) ";
                }

                var sql = new StringBuilder($@"
                    			WITH Resultado AS (
								select 
								 Arp.Id
								,Arp.CodigoTipoResultado
								,Arp.NomeArquivo
								,Arp.NomeOriginalArquivo
								,Arp.CreateDate
								,Arp.UpdateDate
                                ,Trp.Nome as Tipo
								,Arp.State
								from ArquivoResultadoPsp Arp WITH (NOLOCK)
                                inner join TipoResultadoPsp Trp WITH (NOLOCK) on Trp.Codigo = Arp.CodigoTipoResultado
                                where 1=1 
                                {and}
								)
								select Id,CodigoTipoResultado,NomeArquivo,NomeOriginalArquivo,CreateDate,UpdateDate,Tipo,[State]
                                from Resultado
                ");

                cn.Open();

                var query = cn.QueryMultiple(sql.ToString());
                var result = query.Read<ArquivoResultadoPsp>();

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

        public bool ExcluirPorId(long id)
        {
            var sql = new StringBuilder("delete from ArquivoResultadoPsp where id = @id");
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                cn.Execute(sql.ToString(),
                    new
                    {
                        id = id,
                    });
            }
            return true;
        }

    }
}
