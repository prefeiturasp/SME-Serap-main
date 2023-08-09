using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class ReportStudiesRepository : ConnectionReadOnly, IReportStudiesRepository
    {
        public ReportStudies Save(ReportStudies entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(Entities.Enumerator.EnumState.ativo);

                gestaoAvaliacaoContext.ReportStudies.Add(entity);
                gestaoAvaliacaoContext.SaveChanges();
            }
            return entity;
        }

        public IEnumerable<ReportStudies> ListAll()
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.ReportStudies
                    .Where(r => r.State == Convert.ToByte(Entities.Enumerator.EnumState.ativo))
                    .ToList();
            }
        }

        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var entity = gestaoAvaliacaoContext.ReportStudies.Find(id);
                if (entity != null)
                {
                    entity.State = 3;
                    entity.UpdateDate = DateTime.Now;

                    gestaoAvaliacaoContext.SaveChanges();
                }
            }
        }
    }
}
