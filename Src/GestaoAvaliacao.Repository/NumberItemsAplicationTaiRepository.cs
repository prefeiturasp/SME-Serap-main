using Dapper;
using GestaoAvaliacao.Entities;
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

        public NumberItemsAplicationTai GetByTestId(long testId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"select a.Id,a.[Name],a.[Value],a.[CreateDate],a.[UpdateDate],a.[State] 
                                from NumberItemsAplicationTai a
                                inner join NumberItemTestTai b on a.id = b.ItemAplicationTaiId
                                where b.TestId = @testId";
                return cn.Query<NumberItemsAplicationTai>(sql, new { testId }).FirstOrDefault();
            }
        }

        public IEnumerable<NumberItemsAplicationTai> GetAll()
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"select Id,[Name],[Value],[CreateDate],[UpdateDate],[State] 
                                from NumberItemsAplicationTai";

                return cn.Query<NumberItemsAplicationTai>(sql);
            }
        }
    }
}
