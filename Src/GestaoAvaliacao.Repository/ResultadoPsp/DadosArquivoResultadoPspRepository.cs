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
    public class DadosArquivoResultadoPspRepository : ConnectionReadOnly, IDadosArquivoResultadoPspRepository
    {
        public DadosArquivoResultadoPspRepository()
        {

        }

        public DadosArquivoResultadoPsp InserirNovo(DadosArquivoResultadoPsp dadosArquivoResultadoPsp)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                GestaoAvaliacaoContext.DadosArquivoResultadoPsp.Add(dadosArquivoResultadoPsp);
                GestaoAvaliacaoContext.SaveChanges();

                return dadosArquivoResultadoPsp;
            }
        }
    }
}
