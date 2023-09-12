using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
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
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var listNumberItemTestTai = gestaoAvaliacaoContext
                    .NumberItemTestTai
                    .Where(a => a.TestId == testId && a.State == 1)
                    .ToList();

                if (!listNumberItemTestTai.Any()) 
                    return;

                foreach (var numberItemTestTai in listNumberItemTestTai)
                {
                    numberItemTestTai.State = Convert.ToByte(EnumState.excluido);
                    numberItemTestTai.UpdateDate = DateTime.Now;

                    gestaoAvaliacaoContext.Entry(numberItemTestTai).State = System.Data.Entity.EntityState.Modified;
                }

                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void DeleteSaveByTestId(NumberItemTestTai newItem)
        {
            DeleteByTestId(newItem.TestId);
            Save(newItem);
        }
    }
}
