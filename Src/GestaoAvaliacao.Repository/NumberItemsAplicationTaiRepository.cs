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
    public class NumberItemsAplicationTaiRepository : ConnectionReadOnly, INumberItemsAplicationTaiRepository
    {

        private int state = (int)EnumState.ativo;

        public NumberItemsAplicationTai GetByTestId(long testId)
        {
            using (IDbConnection cn = Connection)
            {                
                cn.Open();
                var sql = @"select  a.Id,a.[Name],a.[Value],a.[CreateDate],a.[UpdateDate],a.[State],b.[AdvanceWithoutAnswering], b.[BackToPreviousItem] 
                                from NumberItemsAplicationTai a
                                inner join NumberItemTestTai b on a.id = b.ItemAplicationTaiId
                                where b.TestId = @testId
                                and b.[State] = @state";
                return cn.Query<NumberItemsAplicationTai>(sql, new { testId, state }).FirstOrDefault();
            }
        }

        public IEnumerable<NumberItemsAplicationTai> GetAll()
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"select Id,[Name],[Value],[CreateDate],[UpdateDate],[State] 
                                from NumberItemsAplicationTai
                            where [State] = @state";

                return cn.Query<NumberItemsAplicationTai>(sql, new { state });
            }
        }
    }
}
