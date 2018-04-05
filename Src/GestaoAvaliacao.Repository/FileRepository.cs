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
    public class FileRepository : ConnectionReadOnly, IFileRepository
    {
        #region Read

        public File Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT [Id]
                              ,[Name]
                              ,[Path]
                              ,[CreateDate]
                              ,[UpdateDate]
                              ,[State]
                              ,[ContentType]
                              ,[OwnerId]
                              ,[OwnerType]
                              ,[ParentOwnerId]
                              ,[OriginalName]
                              ,[CreatedBy_Id]
                              ,[DeletedBy_Id] 
                           FROM [File]
                           WHERE Id = @id ";

                var file = cn.Query<File>(sql, new { id = id }).FirstOrDefault();
                return file;
            }
        }

        public IEnumerable<File> Load(ref Pager pager)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return
                    pager.Paginate(
                        gestaoAvaliacaoContext.File.AsNoTracking().Where(
                            x => x.State == (Byte)EnumState.ativo).OrderByDescending(x => x.Id));
            }
        }

        public List<File> _GetFilesNotUsed(double days, int numFiles)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateFilter = DateTime.Now.AddDays(-days);
                List<File> files = GestaoAvaliacaoContext.File.AsNoTracking().Where(a => a.CreateDate < dateFilter && (a.OwnerId == 0 || a.OwnerType == 0 || a.ParentOwnerId == 0)).Take(numFiles).ToList();

                return files;
            }
        }

        public List<File> GetFilesByOwner(long ownerId, long parentId, EnumFileType ownerType)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT [Id]
                              ,[Name]
                              ,[Path]
                              ,[CreateDate]
                              ,[UpdateDate]
                              ,[State]
                              ,[ContentType]
                              ,[OwnerId]
                              ,[OwnerType]
                              ,[ParentOwnerId]
                              ,[OriginalName]
                              ,[CreatedBy_Id]
                              ,[DeletedBy_Id] 
                           FROM [File]
                           WHERE OwnerId = @ownerId
                           AND OwnerType = @ownerType
                           AND ParentOwnerId = @parentId
                           AND State = @state";

                var file = cn.Query<File>(sql,
                    new
                    {
                        ownerId = ownerId,
                        ownerType = ownerType,
                        parentId = parentId,
                        state = (Byte)EnumState.ativo
                    });
                return file.ToList();
            }
        }

        public List<File> _GetFilesByParent(long parentId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return GestaoAvaliacaoContext.File.AsNoTracking().Where(f => f.ParentOwnerId == parentId && f.State == (Byte)EnumState.ativo).ToList();
            }
        }

        public IEnumerable<File> SearchUploadedFiles(ref Pager pager, FileFilter filter)
        {
            var sql = new StringBuilder("WITH NumberedResult AS ");
            sql.Append("( ");
            sql.Append("SELECT F.Id,F.Path,F.CreateDate,F.OriginalName, ");
            sql.Append("ROW_NUMBER() OVER (ORDER BY F.CreateDate DESC) AS RowNumber ");
            sql.Append("FROM [File] F WITH (NOLOCK) ");

            if (filter.CoreVisionId != (int)EnumSYS_Visao.Individual)
            {
                sql.Append("INNER JOIN [Synonym_Core_SYS_Grupo] G WITH (NOLOCK) ON G.vis_id = @VisionId AND G.sis_id = @SystemId ");
                sql.Append("INNER JOIN [Synonym_Core_SYS_UsuarioGrupo] U WITH (NOLOCK) ON U.gru_id = G.gru_id AND U.usu_id = F.CreatedBy_Id ");
            }

            if (filter.OwnerId > 0 && filter.ShowLinks)
                sql.Append("INNER JOIN [TestFiles] T WITH (NOLOCK) ON T.File_Id = F.Id ");

            sql.Append("WHERE F.State <> @State AND F.OwnerType = @OwnerFileType ");

            if (filter.OwnerId > 0 && filter.ShowLinks)
                sql.Append("AND T.Test_Id = @OwnerId AND T.State <> @State ");

            if (filter.CoreVisionId == (int)EnumSYS_Visao.Individual)
                sql.Append("AND F.CreatedBy_Id = @UserId ");

            if (!string.IsNullOrEmpty(filter.Description))
                sql.Append("AND F.OriginalName LIKE '%' + @Search + '%' ");

            if (filter.StartDate != null && filter.StartDate.Equals(DateTime.MinValue))
                filter.StartDate = null;

            if (filter.EndDate != null && filter.EndDate.Equals(DateTime.MinValue))
                filter.EndDate = null;

            if (filter.StartDate == null && filter.EndDate != null)
                sql.Append("AND CAST(F.CreateDate AS DATE) <= CAST(@EndDate AS DATE) ");
            else if (filter.StartDate != null && filter.EndDate == null)
                sql.Append("AND CAST(F.CreateDate AS DATE) >= CAST(@StartDate AS DATE) ");
            else if (filter.StartDate != null && filter.EndDate != null)
                sql.Append("AND CAST(F.CreateDate AS DATE) >= CAST(@StartDate AS DATE) AND CAST(F.CreateDate AS DATE) <= CAST(@EndDate AS DATE) ");

            sql.Append(") ");

            sql.Append("SELECT Id,Path,CreateDate,OriginalName ");
            sql.Append("FROM NumberedResult ");
            sql.Append("WHERE RowNumber > ( @pageSize * @page ) ");
            sql.Append("AND RowNumber <= ( ( @page + 1 ) * @pageSize ) ");

            sql.Append("SELECT COUNT(F.Id) ");
            sql.Append("FROM [File] F WITH (NOLOCK) ");

            if (filter.CoreVisionId != (int)EnumSYS_Visao.Individual)
            {
                sql.Append("INNER JOIN [Synonym_Core_SYS_Grupo] G WITH (NOLOCK) ON G.vis_id = @VisionId AND G.sis_id = @SystemId ");
                sql.Append("INNER JOIN [Synonym_Core_SYS_UsuarioGrupo] U WITH (NOLOCK) ON U.gru_id = G.gru_id AND U.usu_id = F.CreatedBy_Id ");
            }

            if (filter.OwnerId > 0 && filter.ShowLinks)
                sql.Append("INNER JOIN [TestFiles] T WITH (NOLOCK) ON T.File_Id = F.Id ");

            sql.Append("WHERE F.State <> @State AND F.OwnerType = @OwnerFileType ");

            if (filter.OwnerId > 0 && filter.ShowLinks)
                sql.Append("AND T.Test_Id = @OwnerId AND T.State <> @State ");

            if (filter.CoreVisionId == (int)EnumSYS_Visao.Individual)
                sql.Append("AND F.CreatedBy_Id = @UserId ");

            if (!string.IsNullOrEmpty(filter.Description))
                sql.Append("AND F.OriginalName LIKE '%' + @Search + '%' ");

            if (filter.StartDate == null && filter.EndDate != null)
                sql.Append("AND CAST(F.CreateDate AS DATE) <= CAST(@EndDate AS DATE) ");
            else if (filter.StartDate != null && filter.EndDate == null)
                sql.Append("AND CAST(F.CreateDate AS DATE) >= CAST(@StartDate AS DATE) ");
            else if (filter.StartDate != null && filter.EndDate != null)
                sql.Append("AND CAST(F.CreateDate AS DATE) >= CAST(@StartDate AS DATE) AND CAST(F.CreateDate AS DATE) <= CAST(@EndDate AS DATE) ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), new { UserId = filter.UserId, VisionId = filter.CoreVisionId, SystemId = filter.CoreSystemId, State = Convert.ToByte(EnumState.excluido), OwnerId = filter.OwnerId, OwnerFileType = (Byte)EnumFileType.File, Search = filter.Description, pageSize = pager.PageSize, page = pager.CurrentPage, StartDate = filter.StartDate, EndDate = filter.EndDate });

                var files = query.Read<File>();
                var count = query.Read<int>().FirstOrDefault();

                pager.SetTotalItens(count);
                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));

                foreach (var file in files)
                {
                    var sqlMulti = new StringBuilder("SELECT L.File_Id, L.Test_Id ");
                    sqlMulti.Append("FROM TestFiles L WITH (NOLOCK) ");
                    sqlMulti.Append("WHERE L.State = @state ");
                    sqlMulti.Append("AND L.Test_Id = @OwnerId");

                    var listFiles = cn.Query<TestFiles>(sqlMulti.ToString(), new { state = (Byte)EnumState.ativo, OwnerId = filter.OwnerId });

                    file.TestFiles.AddRange(listFiles);
                }

                return files;
            }
        }

        public int GetAllFiles(FileFilter filter)
        {
            var sql = new StringBuilder("SELECT COUNT(F.Id) ");
            sql.Append("FROM [File] F WITH (NOLOCK) ");

            if (filter.CoreVisionId != (int)EnumSYS_Visao.Individual)
            {
                sql.Append("INNER JOIN [Synonym_Core_SYS_Grupo] G WITH (NOLOCK) ON G.vis_id = @VisionId AND G.sis_id = @SystemId ");
                sql.Append("INNER JOIN [Synonym_Core_SYS_UsuarioGrupo] U WITH (NOLOCK) ON U.gru_id = G.gru_id AND U.usu_id = F.CreatedBy_Id ");
            }

            sql.Append("WHERE F.State <> @State AND F.OwnerType = @OwnerFileType ");

            if (filter.CoreVisionId == (int)EnumSYS_Visao.Individual)
                sql.Append("AND F.CreatedBy_Id = @UserId ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), new { UserId = filter.UserId, VisionId = filter.CoreVisionId, SystemId = filter.CoreSystemId, State = Convert.ToByte(EnumState.excluido), OwnerFileType = (Byte)EnumFileType.File });

                return query.Read<int>().FirstOrDefault();
            }
        }

        public IEnumerable<string> GetTestNames(long Id)
        {
            var sql = new StringBuilder("SELECT T.Description ");
            sql.Append("FROM TestFiles F WITH (NOLOCK) ");
            sql.Append("INNER JOIN Test T WITH (NOLOCK) ON T.Id = F.Test_Id AND T.State <> @state ");
            sql.Append("WHERE F.State <> @state AND F.File_Id = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), new { id = Id, state = Convert.ToByte(EnumState.excluido) });

                var entities = query.Read<string>();

                return entities;
            }
        }

        public IEnumerable<File> GetFilesByParent(long parentId, EnumFileType ownerType)
        {
            var sql = new StringBuilder(@"SELECT [Id]
                                                  ,[Name]
                                                  ,[OriginalName]
                                                  ,[ContentType]
                                                  ,[Path]
                                                  ,[OwnerId]
                                                  ,[OwnerType]
                                                  ,[CreateDate]
                                                  ,[UpdateDate]");
            sql.AppendLine("FROM [File] WITH (NOLOCK) ");
            sql.AppendLine("WHERE State <> @state AND ParentOwnerId = @parentId AND [OwnerType] = @ownerType");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(),
                    new
                    {
                        parentId = parentId,
                        state = Convert.ToByte(EnumState.excluido),
                        ownerType = ownerType
                    });

                var entities = query.Read<File>();

                return entities;
            }
        }

        public IEnumerable<File> GetAllFilesByType(EnumFileType ownerType, DateTime limitDate)
        {
            var sql = new StringBuilder("SELECT [Id],[Path] FROM [File] WITH (NOLOCK)");
            sql.AppendLine("WHERE [OwnerType] = @ownerType");

            if (limitDate != null)
                sql.AppendLine("AND [CreateDate] < @limitDate");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(),
                    new
                    {
                        ownerType = ownerType,
                        limitDate = limitDate
                    });

                var entities = query.Read<File>();

                return entities;
            }
        }

        #endregion

        #region Write

        public File Save(File entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                GestaoAvaliacaoContext.File.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public void Update(long Id, File entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                File file = GestaoAvaliacaoContext.File.FirstOrDefault(a => a.Id == entity.Id);

                if (!string.IsNullOrEmpty(entity.Name))
                    file.Name = entity.Name;
                if (!string.IsNullOrEmpty(entity.Path))
                    file.Path = entity.Path;
                if (!string.IsNullOrEmpty(entity.OriginalName))
                    file.OriginalName = entity.OriginalName;

                if (entity.OwnerId > 0)
                    file.OwnerId = entity.OwnerId;
                if (entity.ParentOwnerId > 0)
                    file.ParentOwnerId = entity.ParentOwnerId;

                file.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                File file = GestaoAvaliacaoContext.File.FirstOrDefault(a => a.Id == id);

                file.State = Convert.ToByte(EnumState.excluido);
                file.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void SaveList(List<File> list)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                foreach (File entity in list)
                {
                    File file = GestaoAvaliacaoContext.File.FirstOrDefault(a => a.Id == entity.Id);

                    File _entity = new File();

                    if (!string.IsNullOrEmpty(file.Name))
                        _entity.Name = file.Name;
                    if (!string.IsNullOrEmpty(file.Path))
                        _entity.Path = file.Path;
                    if (!string.IsNullOrEmpty(file.ContentType))
                        _entity.ContentType = file.ContentType;

                    if (entity.OwnerId > 0)
                        _entity.OwnerId = entity.OwnerId;
                    if (entity.OwnerType > 0)
                        _entity.OwnerType = entity.OwnerType;
                    if (entity.ParentOwnerId > 0)
                        _entity.ParentOwnerId = entity.ParentOwnerId;

                    GestaoAvaliacaoContext.File.Add(_entity);
                    GestaoAvaliacaoContext.SaveChanges();
                }
            }
        }

        public void UpdateList(List<File> list)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                foreach (File entity in list)
                {
                    File file = GestaoAvaliacaoContext.File.FirstOrDefault(a => a.Id == entity.Id);

                    if (!string.IsNullOrEmpty(entity.Name))
                        file.Name = entity.Name;
                    if (!string.IsNullOrEmpty(entity.Path))
                        file.Path = entity.Path;
                    if (entity.OwnerId > 0)
                        file.OwnerId = entity.OwnerId;
                    if (entity.OwnerType > 0)
                        file.OwnerType = entity.OwnerType;
                    if (entity.ParentOwnerId > 0)
                        file.ParentOwnerId = entity.ParentOwnerId;

                    file.UpdateDate = DateTime.Now;

                    GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
                    GestaoAvaliacaoContext.SaveChanges();
                }
            }   
        }

        public void DeleteList(List<File> list)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                foreach (File entity in list)
                {
                    File file = GestaoAvaliacaoContext.File.FirstOrDefault(a => a.Id == entity.Id);

                    file.State = Convert.ToByte(EnumState.excluido);
                    file.UpdateDate = DateTime.Now;

                    GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
                    GestaoAvaliacaoContext.SaveChanges();
                }
            }
            
        }

        public void DeleteByParentId(long parentId, EnumFileType ownerType)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                List<File> files = GestaoAvaliacaoContext.File.Where(a => a.ParentOwnerId == parentId && a.OwnerType == (Byte)ownerType).ToList();
                foreach (File file in files)
                {
                    file.State = Convert.ToByte(EnumState.excluido);
                    file.UpdateDate = DateTime.Now;

                    GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
                    GestaoAvaliacaoContext.SaveChanges();
                }
            }
        }

        public void DeleteFilesNotUsed(File file)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Deleted;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void AssociateFilesToEntity(long identity, List<File> files)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                foreach (var file in files)
                {
                    var entity = GestaoAvaliacaoContext.File.First(f => f.Id.Equals(file.Id));
                    if (identity > 0)
                    {
                        entity.OwnerId = identity;
                        entity.ParentOwnerId = identity;
                    }
                    else
                    {
                        entity.OwnerId = file.OwnerId;
                        entity.ParentOwnerId = file.ParentOwnerId;
                    }

                    GestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    GestaoAvaliacaoContext.SaveChanges();
                }
            }
        }

        public void LogicalDelete(long id, Guid UserId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                File file = GestaoAvaliacaoContext.File.Include("TestFiles").FirstOrDefault(a => a.Id == id);

                if (file.TestFiles != null)
                {
                    file.TestFiles.ForEach(i =>
                    {
                        i.State = Convert.ToByte(EnumState.excluido);
                        i.UpdateDate = DateTime.Now;
                        GestaoAvaliacaoContext.Entry(i).State = System.Data.Entity.EntityState.Modified;
                    });
                }

                file.State = Convert.ToByte(EnumState.excluido);
                file.UpdateDate = DateTime.Now;
                if (UserId != null && UserId != Guid.Empty)
                    file.DeletedBy_Id = UserId;

                GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void UpdateOwnerAndParentId(long Id, long ownerId, long? parentId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                File file = GestaoAvaliacaoContext.File.FirstOrDefault(a => a.Id == Id);

                file.OwnerId = ownerId;
                if (parentId != null)
                    file.ParentOwnerId = (long)parentId;
                file.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(file).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void DeleteFilesByType(EnumFileType ownerType, DateTime limitDate)
        {
            var sql = new StringBuilder("DELETE FROM [File] ");
            sql.AppendLine("WHERE [OwnerType] = @ownerType");

            if (limitDate != null)
                sql.AppendLine("AND [CreateDate] < @limitDate");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                cn.Execute(sql.ToString(),
                    new
                    {
                        ownerType = ownerType,
                        limitDate = limitDate
                    });
            }
        }

        #endregion
    }
}
