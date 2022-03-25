using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class NumberItemTestTaiRepository : ConnectionReadOnly, INumberItemTestTaiRepository
    {
        public NumberItemTestTai Save(NumberItemTestTai entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                GestaoAvaliacaoContext.NumberItemTestTai.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void DeleteByTestId(long testId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                NumberItemTestTai numberItemTestTai = GestaoAvaliacaoContext.NumberItemTestTai.FirstOrDefault(a => a.TestId == testId);

                numberItemTestTai.State = Convert.ToByte(EnumState.excluido);
                numberItemTestTai.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(numberItemTestTai).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }
    }
}
