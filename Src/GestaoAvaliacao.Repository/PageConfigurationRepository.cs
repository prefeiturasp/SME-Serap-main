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

namespace GestaoAvaliacao.Repository
{
    public class PageConfigurationRepository : ConnectionReadOnly, IPageConfigurationRepository
    {
        public PageConfiguration Get(long id)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.PageConfiguration.Where(p => p.Id == id).FirstOrDefault();
            }
        }

        public PageConfiguration Find(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT 
	                            PC.Id,
                                PC.Category,
                                PC.Title,
                                PC.[Description],
                                PC.ButtonDescription,
                                PC.Link,
                                PC.FileIllustrativeImage_Id,
                                PC.FileVideo_Id,
                                PC.Featured,
	                            F.[Path] AS CaminhoIcone,
	                            FV.[Path] AS CaminhoVideo
                            FROM PageConfiguration AS PC WITH (NOLOCK)
                            LEFT JOIN [File] AS F WITH (NOLOCK)
	                            ON F.Id = PC.FileIllustrativeImage_Id
                            LEFT JOIN [File] AS FV WITH (NOLOCK)
	                            ON FV.Id = PC.FileVideo_Id
                            WHERE PC.[State] <> @state
	                            AND PC.Id = @id";

                var pageConfiguration = cn.Query<PageConfiguration>(sql, new { id = id, state = EnumState.excluido }).FirstOrDefault();

                return pageConfiguration;
            }
        }

        public IEnumerable<PageConfiguration> LoadAll()
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT 
	                            PC.Id,
                                PC.Category,
                                PC.Title,
                                PC.[Description],
                                PC.ButtonDescription,
                                PC.Link,
                                PC.FileIllustrativeImage_Id,
                                PC.FileVideo_Id,
                                PC.Featured,
	                            F.[Path] AS CaminhoIcone,
	                            FV.[Path] AS CaminhoVideo
                            FROM PageConfiguration AS PC WITH (NOLOCK)
                            LEFT JOIN [File] AS F WITH (NOLOCK)
	                            ON F.Id = PC.FileIllustrativeImage_Id
                            LEFT JOIN [File] AS FV WITH (NOLOCK)
	                            ON FV.Id = PC.FileVideo_Id
                            WHERE PC.[State] <> @state";

                var pageConfiguration = cn.Query<PageConfiguration>(sql, new { state = EnumState.excluido });

                return pageConfiguration;
            }
        }

        public IEnumerable<PageConfiguration> Load(ref Pager pager)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"WITH NumberedPageConfiguration AS " +
                            "( " +
                               "SELECT Id, Category, Title, Description, Featured, " +
                               "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
                               "FROM PageConfiguration WITH (NOLOCK) " +
                               "WHERE State = @state " +
                            ") " +
                           "SELECT Id, Category, Title, Description, Featured " +
                           "FROM NumberedPageConfiguration " +
                           "WHERE RowNumber > ( @pageSize * @page ) " +
                           "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
                           "ORDER BY RowNumber";

                var countSql = @"SELECT COUNT(id) " +
                                "FROM PageConfiguration WITH (NOLOCK) " +
                                "WHERE State = @state ";

                var pageConfiguration = cn.Query<PageConfiguration>(sql, new { state = EnumState.ativo, pageSize = pager.PageSize, page = pager.CurrentPage });
                var count = (int)cn.ExecuteScalar(countSql, new { state = EnumState.ativo });

                pager.SetTotalPages((int)Math.Ceiling(pageConfiguration.Count() / (double)pager.PageSize));
                pager.SetTotalItens(count);

                return pageConfiguration;
            }
        }

        public IEnumerable<PageConfiguration> Search(string search, string category, ref Pager pager)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"WITH NumberedPageConfiguration AS 
                            ( 
                                SELECT Id, Category, Title, Description, Featured, 
                                ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber
                                FROM PageConfiguration WITH (NOLOCK) 
                                WHERE State  = @state 
                                AND (@search IS NULL OR Title LIKE '%' + @search + '%') 
                                AND (@category IS NULL OR 
			                            CASE Category 
				                            WHEN 1 THEN 'Texto principal'
				                            WHEN 2 THEN 'Link de acesso externo' 
				                            WHEN 3 THEN 'Ferramenta de destaque' 
				                            WHEN 4 THEN 'Ferramenta' 
				                            WHEN 5 THEN 'Vídeo' 
			                            END LIKE '%' + @category + '%')
                            ) 
                           SELECT Id, Category, Title, Description, Featured 
                           FROM NumberedPageConfiguration 
                           WHERE RowNumber > ( @pageSize * @page ) 
                           AND RowNumber <= ( ( @page + 1 ) * @pageSize ) 
                           ORDER BY RowNumber";

                var countSql = @"SELECT COUNT(id) 
                                FROM PageConfiguration WITH (NOLOCK) 
                                WHERE State = @state 
                                AND (@search IS NULL OR Title Like '%' + @search + '%') 
                                AND (@category IS NULL OR 
			                            CASE Category 
				                            WHEN 1 THEN 'Texto principal'
				                            WHEN 2 THEN 'Link de acesso externo' 
				                            WHEN 3 THEN 'Ferramenta de destaque' 
				                            WHEN 4 THEN 'Ferramenta' 
				                            WHEN 5 THEN 'Vídeo' 
			                            END LIKE '%' + @category + '%')";

                var pageConfiguration = cn.Query<PageConfiguration>(sql, new { state = EnumState.ativo, pageSize = pager.PageSize, page = pager.CurrentPage, search = search, category = category });
                var count = (int)cn.ExecuteScalar(countSql, new { state = EnumState.ativo, search = search, category = category });

                pager.SetTotalPages((int)Math.Ceiling(pageConfiguration.Count() / (double)pager.PageSize));
                pager.SetTotalItens(count);

                return pageConfiguration;
            }
        }

        public bool ExistsModelDescription(long id, string description)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(Id) " +
                           "FROM PageConfiguration " +
                           "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
                           "AND Id <> @id " +
                           "AND State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { id = id, Description = description, state = EnumState.excluido });

                return count > 0;
            }
        }

        public bool ExistsFeaturedVideo(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(Id) " +
                           "FROM PageConfiguration " +
                           "WHERE Id <> @id " +
                           "AND Featured = 1 " +
                           "AND Category = @category " +
                           "AND State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { id = id, category = (short)PageConfigurationCategory.Video, state = EnumState.excluido });

                return count > 0;
            }
        }

        #region CRUD

        public PageConfiguration Save(PageConfiguration entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                PageConfiguration _entity = new PageConfiguration
                {
                    Category = entity.Category,
                    Title = entity.Title,
                    Description = entity.Description,
                    ButtonDescription = entity.ButtonDescription,
                    Link = entity.Link,
                    Featured = entity.Featured,
                    State = entity.State
                };

                if (entity.FileIllustrativeImage != null && entity.FileIllustrativeImage.Id > 0)
                    _entity.FileIllustrativeImage_Id = gestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == entity.FileIllustrativeImage.Id).Id;

                if (entity.FileVideo != null && entity.FileVideo.Id > 0)
                    _entity.FileVideo_Id = gestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == entity.FileVideo.Id).Id;

                gestaoAvaliacaoContext.PageConfiguration.Add(_entity);
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }
        }

        public PageConfiguration Update(PageConfiguration entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                var _entity = gestaoAvaliacaoContext.PageConfiguration.Where(x => x.Id == entity.Id).FirstOrDefault();

                gestaoAvaliacaoContext.Entry(_entity).CurrentValues.SetValues(entity);

                if (entity.FileIllustrativeImage != null && entity.FileIllustrativeImage.Id > 0)
                    _entity.FileIllustrativeImage_Id = gestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == entity.FileIllustrativeImage.Id).Id;

                if (entity.FileVideo != null && entity.FileVideo.Id > 0)
                    _entity.FileVideo_Id = gestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == entity.FileVideo.Id).Id;

                _entity.UpdateDate = dateNow;

                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }

        }

        public void Delete(PageConfiguration entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                PageConfiguration pageConfiguration = GestaoAvaliacaoContext.PageConfiguration.FirstOrDefault(a => a.Id == entity.Id);

                pageConfiguration.State = Convert.ToByte(EnumState.excluido);
                pageConfiguration.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(pageConfiguration).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion
    }
}
