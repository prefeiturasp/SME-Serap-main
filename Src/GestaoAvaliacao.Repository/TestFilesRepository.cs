using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
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
    public class TestFilesRepository : ConnectionReadOnly, ITestFilesRepository
    {
        #region Read

        public IEnumerable<File> GetFiles(long Id, EnumFileType answerSheetType)
        {
            var sql = new StringBuilder("SELECT F.Id, F.Name, F.OriginalName, F.OwnerType, F.Path, F.ContentType ");
            sql.AppendLine("FROM TestFiles T WITH (NOLOCK) ");
            sql.AppendLine("INNER JOIN [File] F WITH (NOLOCK) ON F.Id = T.File_Id AND F.State <> @state ");
            sql.AppendLine("WHERE T.State <> @state AND T.Test_Id = @id AND F.OwnerType = @OwnerType ");
            sql.AppendLine("UNION ");
            sql.AppendLine("SELECT F.Id, F.Name, F.OriginalName, F.OwnerType, F.Path, F.ContentType ");
            sql.AppendLine("FROM [File] F WITH (NOLOCK) ");
            sql.AppendLine("INNER JOIN [Test] TT WITH (NOLOCK) ON TT.Id = F.ParentOwnerId AND TT.State <> @state AND TT.TestSituation <> @TestSituation ");
            sql.AppendLine("WHERE F.State <> @state AND F.ParentOwnerId = @id ");
            sql.AppendLine("AND (F.OwnerType = @OwnerTypeTest OR F.OwnerType = @OwnerTypeAnswerSheet OR (TT.[PublicFeedback] = 1 AND F.OwnerType = @OwnerTypeFeedback))");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(),
                    new
                    {
                        id = Id,
                        state = Convert.ToByte(EnumState.excluido),
                        OwnerType = Convert.ToByte(EnumFileType.File),
                        OwnerTypeTest = Convert.ToByte(EnumFileType.Test),
                        OwnerTypeAnswerSheet = Convert.ToByte(answerSheetType),
                        OwnerTypeFeedback = Convert.ToByte(EnumFileType.TestFeedback),
                        TestSituation = Convert.ToByte(EnumTestSituation.Pending)
                    });

                var entities = query.Read<File>();

                return entities;
            }
        }

        public IEnumerable<TestFiles> GetTestFiles(long Id)
        {
            var sql = new StringBuilder("SELECT T.Id, T.File_Id, T.Test_Id, T.CreateDate, T.UserId ");
            sql.Append("FROM TestFiles T WITH (NOLOCK) ");
            sql.Append("INNER JOIN [File] F WITH (NOLOCK) ON F.Id = T.File_Id AND F.State <> @state ");
            sql.Append("WHERE T.State <> @state AND T.Test_Id = @id AND F.OwnerType = @OwnerType ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), new { id = Id, state = Convert.ToByte(EnumState.excluido), OwnerType = Convert.ToByte(EnumFileType.File) });

                var entities = query.Read<TestFiles>();

                return entities;
            }
        }

        public bool GetChecked(long Id, long OwnerId)
        {
            var sql = new StringBuilder("SELECT COUNT(F.Id) ");
            sql.Append("FROM TestFiles F WITH (NOLOCK) ");
            sql.Append("WHERE F.State <> @state AND F.File_Id = @id AND F.Test_Id = @testId ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), new { id = Id, testId = OwnerId, State = Convert.ToByte(EnumState.excluido) });

                return query.Read<int>().FirstOrDefault() > 0;
            }
        }

        #endregion

        #region Write

        public void SaveList(List<TestFiles> list)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                foreach (TestFiles entity in list)
                {
                    TestFiles _entity = new TestFiles();

                    if (entity.File_Id > 0)
                        _entity.File_Id = entity.File_Id;
                    if (entity.Test_Id > 0)
                        _entity.Test_Id = entity.Test_Id;
                    if (entity.UserId != null)
                        _entity.UserId = entity.UserId;

                    GestaoAvaliacaoContext.TestFiles.Add(_entity);
                    GestaoAvaliacaoContext.SaveChanges();
                }
            }            
        }

        public void UpdateList(List<TestFiles> list)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                foreach (TestFiles entity in list)
                {
                    TestFiles file = GestaoAvaliacaoContext.TestFiles.FirstOrDefault(a => a.Id == entity.Id);

                    if (entity.File_Id > 0)
                        file.File_Id = entity.File_Id;
                    if (entity.Test_Id > 0)
                        file.Test_Id = entity.Test_Id;
                    if (entity.UserId != null)
                        file.UserId = entity.UserId;

                    file.UpdateDate = DateTime.Now;

                    GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
                    GestaoAvaliacaoContext.SaveChanges();
                }
            }            
        }

        public IEnumerable<TestFiles> Update(TestFiles test, List<TestFiles> files)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                IEnumerable<TestFiles> testFiles = GetTestFiles(test.Test_Id);

                if (files != null)
                {
                    files.ForEach(i =>
                    {
                        i.Test_Id = test.Test_Id;
                    });
                }

                List<TestFiles> items = new List<TestFiles>();

                var itemsFront = files.Select(s => s.File_Id);
                var itemsDatabase = testFiles.Where(s => s.State == (Byte)EnumState.ativo).Select(s => s.File_Id);
                if (itemsFront != null && itemsDatabase != null)
                {
                    var itemsToExclude = itemsDatabase.Except(itemsFront);
                    if (itemsToExclude != null && itemsToExclude.Any())
                    {
                        foreach (var item in testFiles.Where(s => s.State == (Byte)EnumState.ativo && itemsToExclude.Contains(s.File_Id)))
                        {
                            if (item != null)
                            {
                                item.State = Convert.ToByte(EnumState.excluido);
                                item.UpdateDate = dateNow;
                                items.Add(item);
                            }
                        }
                    }

                    foreach (TestFiles itemFront in files)
                    {
                        if (itemFront != null)
                        {
                            TestFiles itemDB = testFiles.FirstOrDefault(e => e.File_Id.Equals(itemFront.File_Id) && e.Test_Id.Equals(itemFront.Test_Id) && e.State.Equals((Byte)EnumState.ativo));

                            if (itemDB == null)
                            {
                                itemFront.State = Convert.ToByte(EnumState.ativo);
                                itemFront.CreateDate = dateNow;
                                items.Add(itemFront);
                            }
                        }
                    }
                }

                if (items != null && items.Count > 0)
                {
                    items.ForEach(i =>
                    {
                        i.UserId = test.UserId;
                        i.Test_Id = test.Test_Id;
                    });

                    foreach (TestFiles _entity in items)
                    {
                        if (_entity.State == Convert.ToByte(EnumState.excluido))
                            gestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                        else if (_entity.State == Convert.ToByte(EnumState.ativo))
                            gestaoAvaliacaoContext.TestFiles.Add(_entity);

                        gestaoAvaliacaoContext.SaveChanges();
                    }
                }

                return items.Union(testFiles).Where(i => i.State == Convert.ToByte(EnumState.ativo));
            }
        }

        #endregion
    }
}
