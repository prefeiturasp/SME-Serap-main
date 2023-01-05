using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
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

            var retorno = new List<ArquivoResultadoPsp>();
            retorno.Add(new ArquivoResultadoPsp { Id = 1, NomeArquivo = "teste_1.csv", Status = 1, CreateDate = DateTime.Now.AddDays(-1) });
            retorno.Add(new ArquivoResultadoPsp { Id = 2, NomeArquivo = "teste_2.csv", Status = 2, CreateDate = DateTime.Now });
            retorno.Add(new ArquivoResultadoPsp { Id = 3, NomeArquivo = "teste_3.csv", Status = 3, CreateDate = DateTime.Now.AddDays(-2) });
            retorno.Add(new ArquivoResultadoPsp { Id = 4, NomeArquivo = "teste_4.csv", Status = 4, CreateDate = DateTime.Now.AddDays(-5) });
            retorno.Add(new ArquivoResultadoPsp { Id = 5, NomeArquivo = "teste_5.csv", Status = 4, CreateDate = DateTime.Now.AddDays(-6) });
            var count = retorno.Count;

            pager.SetTotalItens(count);
            pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));

            return retorno.AsEnumerable();

        }

    }
}
