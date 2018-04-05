using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Data;
using System.Linq;

namespace GestaoAvaliacao.Repository
{
    public class TestSubGroupRepository : ConnectionReadOnly, ITestSubGroupRepository
    {
        public TestSubGroup Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description, CreateDate, UpdateDate, [State], [Order] " +
                            "FROM TestSubGroup " +
                            "WHERE Id = @id ";

                var testSubGroup = cn.Query<TestSubGroup>(sql, new { id = id }).FirstOrDefault();

                return testSubGroup;
            }
        }

        public TestSubGroup Update(TestSubGroup entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                TestSubGroup _entity = gestaoAvaliacaoContext.TestSubGroup.FirstOrDefault(a => a.Id == entity.Id);

                if (!string.IsNullOrEmpty(entity.Description))
                    _entity.Description = entity.Description;

                _entity.State = entity.State;
                _entity.UpdateDate = DateTime.Now;
                _entity.Order = entity.Order;

                gestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }

        }
    }
}
