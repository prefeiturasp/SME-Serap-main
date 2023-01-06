using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class ResultadoPspRepository : ConnectionReadOnly, IResultadoPspRepository
    {
        public ResultadoPspRepository()
        {

        }

        public IEnumerable<ArquivoResultadoPsp> ObterImportacoes(ref Pager pager, string codigoOuNomeArquivo)
        {
            //var retorno = query.Read<ArquivoResultadoPsp>();
            //var count = query.Read<int>().FirstOrDefault();            

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"select 
								[Id]
								,[Codigo]
								,[Nome]
								,[NomeTabelaProvaSp]
								,[CreateDate]
								,[UpdateDate]
								,[State]
								from TipoResultadoPsp
								where [State] = 1
                                and Codigo = @codigoTipoResultado";

                var result = cn.Read<ArquivoResultadoPsp>(sql, new { codigoOuNomeArquivo });
                var count = result.Count;

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
